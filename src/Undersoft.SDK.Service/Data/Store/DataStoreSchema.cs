namespace Undersoft.SDK.Service.Data.Store
{
    public static class DataStoreSchema
    {
        public static string LocalSchema { get; } = "Local";
        public static string RemoteSchema { get; } = "Remote";
        public static string IdentifierSchema { get; } = "Identifier";
        public static string RelationSchema { get; } = "Relation";
        public static string PropertySchema { get; } = "Property";
    }
}