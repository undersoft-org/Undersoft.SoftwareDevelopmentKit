namespace Undersoft.SDK.Instant
{
    using Undersoft.SDK.Instant.Attributes;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class RubricAttribute : InstantAttribute
    {
        public RubricAttribute()
        {
        }
    }
}
