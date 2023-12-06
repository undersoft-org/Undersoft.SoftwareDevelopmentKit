namespace Undersoft.SDK.IntegrationTests.Extracting.Mocks
{
    using System.Linq;
    using System.Reflection;

    public static class InstantMocks
    {
        public static MemberInfo[] Instant_MemberRubric_FieldsAndPropertiesModel()
        {
            return typeof(FieldsAndPropertiesModel)
                .GetMembers()
                .Select(
                    m =>
                        m.MemberType == MemberTypes.Field
                            ? new MemberRubric((FieldInfo)m)
                            : m.MemberType == MemberTypes.Property
                                ? new MemberRubric((PropertyInfo)m)
                                : null
                )
                .Where(p => p != null)
                .ToArray();
        }
    }
}
