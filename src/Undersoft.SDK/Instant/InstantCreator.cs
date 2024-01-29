namespace Undersoft.SDK.Instant
{
    using Undersoft.SDK.Series;
    using Undersoft.SDK.Uniques;
    using Rubrics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;

    public class InstantCreator<T> : InstantCreator
    {
        public InstantCreator(InstantType modeType = InstantType.Reference)
            : base(typeof(T), modeType) { }

        public InstantCreator(string createdTypeName, InstantType modeType = InstantType.Reference)
            : base(typeof(T), createdTypeName, modeType) { }
    }

    public class InstantCreator : IInstantCreator
    {
        protected RubricBuilder rubricBuilder = new RubricBuilder();
        protected ISeries<RubricModel> rubricModels = new Registry<RubricModel>();
        private MemberRubric[] memberRubrics;
        private Type compiledType;

        public InstantCreator(
            IList<MemberInfo> instantMembers,
            InstantType modeType = InstantType.Reference
        ) : this(instantMembers.ToArray(), null, modeType) { }

        public InstantCreator(
            IList<MemberInfo> instantMembers,
            string createdTypeName,
            InstantType modeType = InstantType.Reference
        )
        {
            Name =
                (createdTypeName != null && createdTypeName != "")
                    ? createdTypeName
                    : DateTime.Now.ToBinary().ToString();

            Name += "Instant";

            mode = modeType;

            rubricBuilder = new RubricBuilder();

            rubricModels = rubricBuilder.Create(rubricBuilder.PrepareMembers(instantMembers));

            Rubrics = new MemberRubrics(rubricModels.Select(m => m.Member).ToArray());
            Rubrics.KeyRubrics = new MemberRubrics();
        }

        public InstantCreator(
            MemberRubrics instantRubrics,
            string createdTypeName,
            InstantType modeType = InstantType.Reference
        ) : this(instantRubrics.ToArray(), createdTypeName, modeType) { }

        public InstantCreator(Type baseOnType, InstantType modeType = InstantType.Reference)
            : this(baseOnType, null, modeType) { }

        public InstantCreator(
            Type baseOnType,
            string createdTypeName,
            InstantType modeType = InstantType.Reference
        )
        {
            BaseType = baseOnType;

            if (modeType == InstantType.Derived)
                IsDerived = true;

            Name = createdTypeName == null ? baseOnType.Name : createdTypeName;
            Name += "Instant";
            mode = modeType;

            rubricBuilder = new RubricBuilder();
            rubricModels = rubricBuilder.Create(baseOnType);

            Rubrics = new MemberRubrics(rubricModels.Select(m => m.Member).ToArray());
            Rubrics.KeyRubrics = new MemberRubrics();
        }

        public Type BaseType { get; set; }

        public bool IsDerived { get; set; }

        public string Name { get; set; }

        public IRubrics Rubrics { get; set; }

        public int Size { get; set; }

        public Type Type { get; set; }

        private InstantType mode { get; set; }

        private long? _seed = null;
        private long seed => _seed ??= Type.UniqueKey64();

        public IInstant Create()
        {
            if (this.Type == null)
            {
                try
                {
                    switch (mode)
                    {
                        case InstantType.Reference:
                            compileInstantType(
                                new InstantCompilerReferenceTypes(this, rubricModels)
                            );
                            break;
                        case InstantType.ValueType:
                            compileInstantType(new InstantCompilerValueTypes(this, rubricModels));
                            break;
                        case InstantType.Derived:
                            compileDerivedType(
                                new InstantCompilerDerivedTypes(this, rubricModels)
                            );
                            break;
                        default:
                            break;
                    }

                    Rubrics.Update();
                }
                catch (Exception ex)
                {
                    throw new InstantTypeCompilerException(
                        "Instant compilation at runtime failed see inner exception",
                        ex
                    );
                }
            }
            return create();
        }

        public object New()
        {
            if (this.Type == null)
                return Create();
            return this.Type.New();
        }

        private IInstant create()
        {
            if (this.Type == null)
                return Create();

            var figure = (IInstant)this.Type.New();
            figure.Id = Unique.NewId;
            figure.TypeId = seed;
            return figure;
        }

        private void compileDerivedType(InstantCompiler compiler)
        {
            var fcdt = compiler;
            compiledType = fcdt.CompileInstantType(Name);
            Rubrics.KeyRubrics.Add(fcdt.Identities.Values);
            Type = compiledType.New().GetType();

            if (!(Rubrics.AsValues().Any(m => m.Name == "code")))
            {
                var f = this.Type.GetField("code", BindingFlags.NonPublic | BindingFlags.Instance);

                if (!Rubrics.TryGet("code", out MemberRubric mr))
                {
                    mr = new MemberRubric(f);
                    mr.InstantCreatorField = f;
                    Rubrics.Insert(0, mr);
                }
                mr.RubricName = "code";
            }
            Size = Rubrics.BinarySize;
        }

        private void compileInstantType(InstantCompiler compiler)
        {
            var fcvt = compiler;
            compiledType = fcvt.CompileInstantType(Name);
            Rubrics.KeyRubrics.Add(fcvt.Identities.Values);
            Type = compiledType.New().GetType();
            if (mode == InstantType.ValueType)
            {
                try
                {
                    Size = Marshal.SizeOf(Type);
                }
                catch(Exception ex) 
                {
                    Size = Rubrics.BinarySize;
                }
            }
            else
                Size = Rubrics.BinarySize;
        }

        private MemberRubric[] createMemberRurics(IList<MemberInfo> membersInfo)
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
    }

    public class InstantTypeCompilerException : Exception
    {
        public InstantTypeCompilerException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
