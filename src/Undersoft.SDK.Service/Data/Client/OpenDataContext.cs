using Microsoft.OData.Client;
using Microsoft.OData.Edm;
using Undersoft.SDK.Security;
using Undersoft.SDK.Service.Data.Remote;

namespace Undersoft.SDK.Service.Data.Client
{
    public partial class OpenDataContext : DataServiceContext
    {
        private ISecurityString _securityString;

        public OpenDataContext(Uri serviceUri) : base(serviceUri)
        {
            MergeOption = MergeOption.AppendOnly;
            HttpRequestTransportMode = HttpRequestTransportMode.HttpClient;
            IgnoreResourceNotFoundException = true;
            KeyComparisonGeneratesFilterQuery = false;
            DisableInstanceAnnotationMaterialization = true;
            EnableWritingODataAnnotationWithoutPrefix = false;
            AddAndUpdateResponsePreference = DataServiceResponsePreference.NoContent;
            SaveChangesDefaultOptions = SaveChangesOptions.BatchWithSingleChangeset;
            ResolveName = (t) => this.GetMappedName(t);
            ResolveType = (n) => this.GetMappedType(n);
            SendingRequest2 += RequestAuthorization;
            Format.LoadServiceModel = GetServiceModel;
        }

        public Registry<RemoteRelation> Remotes { get; set; } = new Registry<RemoteRelation>(true);

        private void RequestAuthorization(object sender, SendingRequest2EventArgs e)
        {
            if (_securityString != null)
                e.RequestMessage.SetHeader("Authorization", _securityString.Encoded);
        }

        public async Task<IEdmModel> CreateServiceModel()
        {
            var edmModel = await AddServiceModel();
            Format.UseJson();
            return edmModel;
        }

        public async Task<IEdmModel> AddServiceModel()
        {
            string t = GetType().FullName;
            if (!OpenDataRegistry.EdmModels.TryGet(t, out IEdmModel edmModel))
                OpenDataRegistry.EdmModels.Add(t, edmModel = OnModelCreating(await this.GetEdmModel()));
            return edmModel;
        }

        public IEdmModel GetServiceModel()
        {
            return OpenDataRegistry.EdmModels.Get(GetType().FullName);
        }

        protected virtual IEdmModel OnModelCreating(IEdmModel builder)
        {
            return builder;
        }

        public override DataServiceQuery<T> CreateQuery<T>(string resourcePath, bool isComposable)
        {
            return base.CreateQuery<T>(resourcePath, isComposable);
        }

        public void SetAuthorizationHeader(string securityString)
        {
            _securityString = null;

            if (securityString != null)
            {
                var strings = securityString.Split(" ");
                string prefix = strings.Length > 0 ? strings[0] : null;
                _securityString = new SecurityString(strings.LastOrDefault(), prefix);
            }
        }
    }
}