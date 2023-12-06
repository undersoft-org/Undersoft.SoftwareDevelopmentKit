using Microsoft.OData.Edm;
using System;
using Undersoft.SDK.Service.Data.Store;

namespace Undersoft.SDK.Service.Data.Service
{
    public partial class StreamDataService<TStore> where TStore : IDataServiceStore
    {
        public StreamDataService(Uri serviceUri)
        {
        }
    }
}