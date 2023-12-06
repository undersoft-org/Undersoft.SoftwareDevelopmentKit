using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using IdentityModel;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Undersoft.SDK.Service.Application;

using DataServer;
using Documentation;
using Account;
using Account.Email;
using Data.Store;

public partial class ApplicationSetup : ServiceSetup, IApplicationSetup
{
    public ApplicationSetup(IServiceCollection services, IMvcBuilder mvcBuilder = null)
        : base(services, mvcBuilder) { }

    public ApplicationSetup(IServiceCollection services, IConfiguration configuration)
        : base(services, configuration) { }

    public IApplicationSetup AddDataServer<TServiceStore>(
        DataServerTypes dataServerTypes,
        Action<DataServerBuilder> builder = null
    ) where TServiceStore : IDataServiceStore
    {
        DataServerBuilder.ServiceTypes = dataServerTypes;
        if ((dataServerTypes & DataServerTypes.OData) > 0)
        {
            var ds = new OpenDataServerBuilder<TServiceStore>();
            if(builder != null)
                builder.Invoke(ds);
            ds.Build();
            ds.AddODataServicer(mvc);
        }
        if ((dataServerTypes & DataServerTypes.Grpc) > 0)
        {
            var ds = new GrpcDataServerBuilder<TServiceStore>();
            if (builder != null)
                builder.Invoke(ds);
            ds.Build();
            ds.AddGrpcServicer();
        }
        if ((dataServerTypes & DataServerTypes.Rest) > 0)
        {
            var ds = new RestDataServerBuilder<TServiceStore>();
            if (builder != null)
                builder.Invoke(ds);
            ds.Build();
        }
        return this;
    }  

    public IApplicationSetup AddIdentityService<TContext>() where TContext : DbContext
    {
        registry.Services
            .AddIdentity<IdentityUser<long>, IdentityRole<long>>(
            options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Tokens.ProviderMap.Add("AccountEmailConfirmationTokenProvider",
              new TokenProviderDescriptor(
            typeof(AccountConfirmationTokenProvider<IdentityUser>)));
                options.Tokens.EmailConfirmationTokenProvider = "AccountEmailConfirmationTokenProvider";
                options.Tokens.ProviderMap.Add("AccountPasswordResetTokenProvider",
                new TokenProviderDescriptor(
                typeof(AccountPasswordResetTokenProvider<IdentityUser>)));
                options.Tokens.PasswordResetTokenProvider = "AccountPasswordResetTokenProvider";
                options.Tokens.ProviderMap.Add("AccountChangeEmailTokenProvider",
            new TokenProviderDescriptor(
                typeof(AccountChangeEmailTokenProvider<IdentityUser>)));
                options.Tokens.ChangeEmailTokenProvider = "AccountChangeEmailTokenProvider";
                options.Tokens.ProviderMap.Add("AccountRegistrationProcessTokenProvider",
            new TokenProviderDescriptor(
            typeof(AccountRegistrationProcessTokenProvider<IdentityUser>)));
                options.Tokens.ChangePhoneNumberTokenProvider = "AccountRegistrationProcessTokenProvider";
                options.User.RequireUniqueEmail = true;
            }
            )
            .AddEntityFrameworkStores<TContext>();

        registry.Configure<DataProtectionTokenProviderOptions>(o =>
        o.TokenLifespan = TimeSpan.FromHours(3));

        registry.AddScoped<IAccountIdentityManager, AccountIdentityManager>();
        registry.AddScoped<AccountIdentityService>();
        registry.AddTransient<IEmailSender, AccountEmailSender>();
        registry.Configure<AccountEmailSenderOptions>(configuration);

        AddIdentityAuthentication();
        AddIdentityAuthorization();
        
        return this;
    }

    public IApplicationSetup AddIdentityAuthentication() 
    {
        var jwtOptions = new AccountIdentityJWTOptions();
        var jwtFactory = new AccountIdentityJWTFactory(30, jwtOptions);

        registry.AddObject(jwtFactory);

        registry.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(jwtOptions.SecurityKey),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });
        return this;
    }

    public IApplicationSetup AddIdentityAuthorization()
    {
        var ic = configuration.Identity;

        registry.Services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            ic.Scopes.ForEach(
                s => options.AddPolicy(s, policy => policy.RequireClaim("access", s))
            );

            ic.Roles.ForEach(s => options.AddPolicy(s, policy => policy.RequireRole(s)));

            options.AddPolicy(
                "Administrators",
                policy =>
                    policy.RequireAssertion(
                        context =>
                            context.User.HasClaim(
                                c =>
                                    (
                                        (
                                            c.Type == JwtClaimTypes.Role
                                            && c.Value == ic.AdministrationRole
                                        )
                                    )
                            )
                    )
            );
        });

        return this;
    }

    public IApplicationSetup AddSwagger()
    {
        string ver = configuration.Version;
        var ao = configuration.Identity;
        registry.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(
                ao.ApiVersion,
                new OpenApiInfo { Title = ao.ApiName, Version = ao.ApiVersion }
            );
            options.OperationFilter<SwaggerJsonIgnoreFilter>();
            options.DocumentFilter<IgnoreApiDocument>();          

            options.AddSecurityDefinition(
                "oauth2",
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Flows = new OpenApiOAuthFlows
                    {
                        Password = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri($"{ao.BaseUrl}/connect")
                        }
                    }
                }
            );
            options.OperationFilter<AuthorizeCheckOperationFilter>();
        });
        return this;
    }

    public IApplicationSetup AddApiVersions(string[] apiVersions)
    {
        this.apiVersions = apiVersions;
        return this;
    }

    public IApplicationSetup ConfigureApplication(
        bool includeSwagger = true,
        Assembly[] assemblies = null
    )
    {
        Assemblies ??= assemblies ??= AppDomain.CurrentDomain.GetAssemblies();

        base.ConfigureServices(Assemblies)
            .Services
            .AddHttpContextAccessor();

        AddApplicationSetupInternalCQRSImplementations(assemblies);
        AddApplicationSetupInternalCQRSActionImplementations(assemblies);
        AddApplicationSetupRemoteCQRSImplementations(assemblies);
        AddApplicationSetupRemoteCQRSActionImplementations(assemblies);

        if (includeSwagger)
            AddSwagger();

        Services.MergeServices();

        return this;
    }
}
