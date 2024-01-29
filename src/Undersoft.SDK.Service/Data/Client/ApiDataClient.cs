using Microsoft.OData.Edm;
using System;
using Undersoft.SDK.Service.Data.Store;

namespace Undersoft.SDK.Service.Data.Client
{
    public partial class ApiDataClient<TStore> where TStore : IDataServiceStore
    {
        public ApiDataClient(Uri serviceUri)
        {
        }
    }
}