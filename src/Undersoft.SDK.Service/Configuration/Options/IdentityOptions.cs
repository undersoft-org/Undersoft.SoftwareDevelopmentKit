namespace Undersoft.SDK.Service.Configuration.Options
{
    public class IdentityOptions
    {
        public string ApiName { get; set; }

        public string ApiVersion { get; set; }

        public string BaseUrl { get; set; }

        public string ApiBaseUrl { get; set; }

        public string SwaggerClientId { get; set; }

        public bool RequireHttpsMetadata { get; set; }

        public string Audience { get; set; }

        public string Issuer { get; set; }

        public string[] Scopes { get; set; }

        public string[] Roles { get; set; }

        public string[] Claims { get; set; }

        public string AdministrationRole { get; set; }

        public bool CorsAllowAnyOrigin { get; set; }

        public string[] CorsAllowOrigins { get; set; }
    }
}