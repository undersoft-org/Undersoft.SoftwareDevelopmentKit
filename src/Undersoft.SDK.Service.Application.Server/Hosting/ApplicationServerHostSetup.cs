using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Hosting;
using Undersoft.SDK.Service.Server.Hosting;

namespace Undersoft.SDK.Service.Application.Server.Hosting;

public class ApplicationServerHostSetup : ServerHostSetup, IApplicationServerHostSetup
{
    public ApplicationServerHostSetup(IApplicationBuilder application) : base(application) { }

    public ApplicationServerHostSetup(
        IApplicationBuilder application,
        IWebHostEnvironment environment
    ) : base(application, environment) { }

    public IApplicationServerHostSetup UseServiceApplication()
    {
        UseHeaderForwarding();

        if (LocalEnvironment.IsDevelopment())
        {
            _builder.UseDeveloperExceptionPage()
                .UseWebAssemblyDebugging();
        }
        else
        {
            _builder.UseExceptionHandler("/Error")
                .UseHsts();
        }

        _builder
            .UseHttpsRedirection()
            .UseODataBatching()
            .UseODataQueryRequest()
            .UseBlazorFrameworkFiles()
            .UseStaticFiles()
            .UseRouting()
            .UseCors();

        UseSwaggerSetup(new[] { "v1.0" });

        _builder.UseAuthentication().UseAuthorization();

        UseJwtMiddleware();
        UseEndpoints(true);

        return this;
    }
}
