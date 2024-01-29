namespace Undersoft.SDK.Instant.Rubrics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Undersoft.SDK.Instant.Proxies;
    using Undersoft.SDK.Series;

    public class RubricBuilder
    {
        public ISeries<RubricModel> Create(Type modelType)
        {
            Registry<RubricModel> rubricBuilders = new Registry<RubricModel>(true);

            var memberRubrics = PrepareMembers(GetMembers(modelType));

            return Create(memberRubrics);
        }

        public ISeries<RubricModel> Create(MemberRubric[] memberRubrics)
        {
            Registry<RubricModel> rubricBuilders = new Registry<RubricModel>();

            memberRubrics.ForEach(
                (member) =>
                {
                    if (!rubricBuilders.TryGet(member.RubricName, out RubricModel fieldProperty))
                        rubricBuilders.Put(new RubricModel(member));
                    else
                        fieldProperty.SetMember(member);
                }
            );
            rubricBuilders.ForEach(
                (fp, x) =>
                {                   
                    fp.Member.RubricId = x;
                    fp.Member.FieldId = x;
                }
            );

            return rubricBuilders;
        }

        public MemberRubric[] PrepareMembers(IEnumerable<MemberInfo> membersInfo)
        {
            return membersInfo
                .Select(
                    m =>
                        !(m is MemberRubric rubric)
                            ? m.MemberType == MemberTypes.Field
                                ? new MemberRubric((FieldInfo)m)
                                : m.MemberType == MemberTypes.Property
                                    ? new MemberRubric((PropertyInfo)m)
                                    : null
                            : rubric
                )
                .Where(p => p != null)
                .ToArray();
        }

        public MemberInfo[] GetMembers(Type modelType)
        {
            //var excludedNames = new string[] { "proxy" };
            return modelType
                .GetMembers(BindingFlags.Instance | BindingFlags.Public)
                .Where(
                    m =>
                        //!excludedNames.Contains(m.Name.ToLower())
                         (
                            m.MemberType == MemberTypes.Field
                            || (
                                m.MemberType == MemberTypes.Property
                                && (((PropertyInfo)m).CanRead && ((PropertyInfo)m).CanWrite)
                                && (((PropertyInfo)m).PropertyType != typeof(IProxy))
                            )
                        )
                )
                .ToArray();
        }
    }
}
