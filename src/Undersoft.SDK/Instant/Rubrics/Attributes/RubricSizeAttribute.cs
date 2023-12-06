using System.Runtime.InteropServices;

namespace Undersoft.SDK.Instant.Rubrics.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class RubricSizeAttribute : RubricAttribute
    {
        public int SizeConst;
        public UnmanagedType Value;

        public RubricSizeAttribute(int size)
        {
            SizeConst = size;
        }
    }
}
