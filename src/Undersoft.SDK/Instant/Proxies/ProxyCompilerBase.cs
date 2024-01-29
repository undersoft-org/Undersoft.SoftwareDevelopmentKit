using System.ComponentModel.DataAnnotations;

namespace Undersoft.SDK.Instant.Proxies;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using Undersoft.SDK.Series;
using Uniques;
using Rubrics;
using Rubrics.Attributes;
using Attributes;

public abstract class ProxyCompilerBase : InstantCompilerConstructors
{
    protected int rubricCount;
    protected InstantType instantType;
    protected ProxyCreator proxyCreator;
    public SortedList<short, MemberRubric> Identities = new SortedList<short, MemberRubric>();

    public ISeries<RubricModel> rubricBuilders;

    public ProxyCompilerBase(ProxyCreator creator, ISeries<RubricModel> rubricBuilders)
    {
        proxyCreator = creator;
        this.rubricBuilders = rubricBuilders;
        rubricCount = this.rubricBuilders.Count;
    }

    void resolveInstantCreatorDisplayAttributes(FieldBuilder fb, MemberInfo mi, MemberRubric mr)
    {
        object[] o = mi.GetCustomAttributes(typeof(DisplayRubricAttribute), false);
        if ((o != null) && o.Any())
        {
            DisplayRubricAttribute fda = (DisplayRubricAttribute)o.First();
            ;
            mr.DisplayName = fda.Name;

            if (fb != null)
                CreateInstantCreatorDisplayAttribute(fb, fda);
        }
        else if (mr.DisplayName != null)
        {
            CreateInstantCreatorDisplayAttribute(fb, new DisplayRubricAttribute(mr.DisplayName));
        }
    }

    void resolveInstantCreatorIdentityAttributes(FieldBuilder fb, MemberInfo mi, MemberRubric mr)
    {
        if (!mr.IsKey)
        {
            object[] o = mi.GetCustomAttributes(typeof(IdentityRubricAttribute), false);
            if ((o != null) && o.Any())
            {
                IdentityRubricAttribute fia = (IdentityRubricAttribute)o.First();
                mr.IsIdentity = true;
                mr.IsAutoincrement = fia.IsAutoincrement;

                if (Identities.ContainsKey(fia.Order))
                    fia.Order = (short)(Identities.LastOrDefault().Key + 1);

                mr.IdentityOrder = fia.Order;
                Identities.Add(mr.IdentityOrder, mr);

                if (fb != null)
                    CreateInstantCreatorIdentityAttribute(fb, fia);
            }
            else if (mr.IsIdentity)
            {
                if (Identities.ContainsKey(mr.IdentityOrder))
                    mr.IdentityOrder += (short)(Identities.LastOrDefault().Key + 1);

                Identities.Add(mr.IdentityOrder, mr);

                if (fb != null)
                    CreateInstantCreatorIdentityAttribute(
                        fb,
                        new IdentityRubricAttribute
                        {
                            IsAutoincrement = mr.IsAutoincrement,
                            Order = mr.IdentityOrder
                        }
                    );
            }
        }
    }

    void resolveInstantCreatorKeyAttributes(FieldBuilder fb, MemberInfo mi, MemberRubric mr)
    {
        object[] o = mi.GetCustomAttributes(typeof(KeyAttribute), false);
        if ((o == null) || !o.Any())
            o = mi.GetCustomAttributes(typeof(KeyRubricAttribute), false);
        else
            o[0] = new KeyRubricAttribute();

        if ((o != null) && o.Any())
        {
            KeyRubricAttribute fka = (KeyRubricAttribute)o.First();
            mr.IsKey = true;
            mr.IsIdentity = true;
            mr.IsAutoincrement = fka.IsAutoincrement;

            if (Identities.ContainsKey(fka.Order))
                fka.Order = (short)(Identities.LastOrDefault().Key + 1);

            mr.IdentityOrder = fka.Order;
            Identities.Add(mr.IdentityOrder, mr);
            mr.Required = true;

            if (fb != null)
                CreateInstantCreatorKeyAttribute(fb, fka);
        }
        else if (mr.IsKey)
        {
            mr.IsIdentity = true;
            mr.Required = true;

            if (Identities.ContainsKey(mr.IdentityOrder))
                mr.IdentityOrder += (short)(Identities.LastOrDefault().Key + 1);

            Identities.Add(mr.IdentityOrder, mr);

            if (fb != null)
                CreateInstantCreatorKeyAttribute(
                    fb,
                    new KeyRubricAttribute
                    {
                        IsAutoincrement = mr.IsAutoincrement,
                        Order = mr.IdentityOrder
                    }
                );
        }
    }

    void resolveInstantCreatorRquiredAttributes(FieldBuilder fb, MemberInfo mi, MemberRubric mr)
    {
        object[] o = mi.GetCustomAttributes(typeof(RequiredRubricAttribute), false);
        if ((o != null) && o.Any())
        {
            mr.Required = true;

            if (fb != null)
                CreateInstantCreatorRequiredAttribute(fb);
        }
        else if (mr.Required)
        {
            if (fb != null)
                CreateInstantCreatorRequiredAttribute(fb);
        }
    }

    void resolveInstantCreatorTreatmentAttributes(FieldBuilder fb, MemberInfo mi, MemberRubric mr)
    {
        object[] o = mi.GetCustomAttributes(typeof(RubricAggregateAttribute), false);
        if ((o != null) && o.Any())
        {
            RubricAggregateAttribute fta = (RubricAggregateAttribute)o.First();
            ;

            mr.AggregationOperand = fta.SummaryOperand;

            if (fb != null)
                CreateInstantCreatorTreatmentAttribute(fb, fta);
        }
        else if (mr.AggregationOperand != AggregationOperand.None)
        {
            CreateInstantCreatorTreatmentAttribute(
                fb,
                new RubricAggregateAttribute { SummaryOperand = mr.AggregationOperand }
            );
        }
    }

    public abstract Type CompileProxyType(string typeName);

    public abstract void CreateCompareToMethod(TypeBuilder tb);

    public abstract void CreateEqualsMethod(TypeBuilder tb);

    public abstract void CreateFieldsAndProperties(TypeBuilder tb);

    public void CreateInstantCreatorAsAttribute(FieldBuilder field, InstantAsAttribute attrib)
    {
        field.SetCustomAttribute(
            new CustomAttributeBuilder(
                marshalAsCtor,
                new object[] { attrib.Value },
                new FieldInfo[] { typeof(MarshalAsAttribute).GetField("SizeConst") },
                new object[] { attrib.SizeConst }
            )
        );
    }

    public void CreateInstantCreatorDisplayAttribute(FieldBuilder field, DisplayRubricAttribute attrib)
    {
        field.SetCustomAttribute(
            new CustomAttributeBuilder(figureDisplayCtor, new object[] { attrib.Name })
        );
    }

    public void CreateInstantCreatorIdentityAttribute(
        FieldBuilder field,
        IdentityRubricAttribute attrib
    )
    {
        field.SetCustomAttribute(
            new CustomAttributeBuilder(
                figureIdentityCtor,
                Type.EmptyTypes,
                new FieldInfo[]
                {
                    typeof(IdentityRubricAttribute).GetField("Order"),
                    typeof(IdentityRubricAttribute).GetField("IsAutoincrement")
                },
                new object[] { attrib.Order, attrib.IsAutoincrement }
            )
        );
    }

    public void CreateInstantCreatorKeyAttribute(FieldBuilder field, KeyRubricAttribute attrib)
    {
        field.SetCustomAttribute(
            new CustomAttributeBuilder(
                figureKeyCtor,
                Type.EmptyTypes,
                new FieldInfo[]
                {
                    typeof(KeyRubricAttribute).GetField("Order"),
                    typeof(KeyRubricAttribute).GetField("IsAutoincrement")
                },
                new object[] { attrib.Order, attrib.IsAutoincrement }
            )
        );
    }

    public void CreateInstantCreatorRequiredAttribute(FieldBuilder field)
    {
        field.SetCustomAttribute(
            new CustomAttributeBuilder(figureRequiredCtor, Type.EmptyTypes)
        );
    }

    public void CreateInstantCreatorTreatmentAttribute(
        FieldBuilder field,
        RubricAggregateAttribute attrib
    )
    {
        field.SetCustomAttribute(
            new CustomAttributeBuilder(
                seriesTreatmentCtor,
                Type.EmptyTypes,
                new FieldInfo[]
                {
                    typeof(RubricAggregateAttribute).GetField("AggregationOperand"),
                    typeof(RubricAggregateAttribute).GetField("AggregationOperand")
                },
                new object[] { attrib.SummaryOperand }
            )
        );
    }

    public abstract void CreateGetBytesMethod(TypeBuilder tb);

    public virtual void CreateGetEmptyProperty(TypeBuilder tb)
    {
        PropertyBuilder prop = tb.DefineProperty(
            "Empty",
            PropertyAttributes.HasDefault,
            typeof(IUnique),
            Type.EmptyTypes
        );

        PropertyInfo iprop = typeof(IUnique).GetProperty("Empty");

        MethodInfo accessor = iprop.GetGetMethod();

        ParameterInfo[] args = accessor.GetParameters();
        Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

        MethodBuilder getter = tb.DefineMethod(
            accessor.Name,
            accessor.Attributes & (~MethodAttributes.Abstract),
            accessor.CallingConvention,
            accessor.ReturnType,
            argTypes
        );
        tb.DefineMethodOverride(getter, accessor);

        prop.SetGetMethod(getter);
        ILGenerator il = getter.GetILGenerator();

        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldflda, rubricBuilders[0].Member.InstantCreatorField);
        il.EmitCall(OpCodes.Call, typeof(Uscn).GetMethod("get_Empty"), null);
        il.Emit(OpCodes.Ret);
    }

    public virtual void CreateGetGenericByIntMethod(TypeBuilder tb)
    {
        string[] typeParameterNames = { "V" };
        GenericTypeParameterBuilder[] typeParameters = tb.DefineGenericParameters(
            typeParameterNames
        );

        MethodInfo mi = typeof(IInstant)
            .GetMethod("Get", new Type[] { typeof(int) })
            .GetGenericMethodDefinition();

        ParameterInfo[] args = mi.GetParameters();
        Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

        MethodBuilder method = tb.DefineMethod(
            mi.Name,
            mi.Attributes & (~MethodAttributes.Abstract),
            mi.CallingConvention,
            mi.ReturnType,
            argTypes
        );

        tb.DefineMethodOverride(method, mi);

        ILGenerator il = method.GetILGenerator();

        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldflda, rubricBuilders[0].Member.InstantCreatorField);
        il.Emit(OpCodes.Ldarg_1);
        il.EmitCall(
            OpCodes.Call,
            typeof(Uscn).GetMethod("CompareTo", new Type[] { typeof(IUnique) }),
            null
        );
        il.Emit(OpCodes.Ret);
    }

    public virtual void CreateGetUniqueBytesMethod(TypeBuilder tb)
    {
        MethodInfo createArray = typeof(IUnique).GetMethod("GetIdBytes");

        ParameterInfo[] args = createArray.GetParameters();
        Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

        MethodBuilder method = tb.DefineMethod(
            createArray.Name,
            createArray.Attributes & (~MethodAttributes.Abstract),
            createArray.CallingConvention,
            createArray.ReturnType,
            argTypes
        );
        tb.DefineMethodOverride(method, createArray);

        ILGenerator il = method.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldflda, rubricBuilders[0].Member.InstantCreatorField);
        il.EmitCall(OpCodes.Call, typeof(Uscn).GetMethod("GetIdBytes"), null);
        il.Emit(OpCodes.Ret);
    }

    public virtual void CreateGetUniqueKeyMethod(TypeBuilder tb)
    {
        MethodInfo createArray = typeof(IUnique).GetMethod("GetId");

        ParameterInfo[] args = createArray.GetParameters();
        Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

        MethodBuilder method = tb.DefineMethod(
            createArray.Name,
            createArray.Attributes & (~MethodAttributes.Abstract),
            createArray.CallingConvention,
            createArray.ReturnType,
            argTypes
        );
        tb.DefineMethodOverride(method, createArray);

        ILGenerator il = method.GetILGenerator();

        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldflda, rubricBuilders[0].Member.InstantCreatorField);
        il.EmitCall(OpCodes.Call, typeof(Uscn).GetMethod("GetId"), null);
        il.Emit(OpCodes.Ret);
    }

    public virtual void CreateGetUniqueSeedMethod(TypeBuilder tb)
    {
        MethodInfo createArray = typeof(IUnique).GetMethod("GetTypeId");

        ParameterInfo[] args = createArray.GetParameters();
        Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

        MethodBuilder method = tb.DefineMethod(
            createArray.Name,
            createArray.Attributes & (~MethodAttributes.Abstract),
            createArray.CallingConvention,
            createArray.ReturnType,
            argTypes
        );
        tb.DefineMethodOverride(method, createArray);

        ILGenerator il = method.GetILGenerator();

        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldflda, rubricBuilders[0].Member.InstantCreatorField);
        il.EmitCall(OpCodes.Call, typeof(Uscn).GetMethod("GetTypeId"), null);
        il.Emit(OpCodes.Ret);
    }

    public abstract void CreateTargetProperty(TypeBuilder tb, Type type, string name);

    public abstract void CreateItemByIntProperty(TypeBuilder tb);

    public abstract void CreateItemByStringProperty(TypeBuilder tb);

    public void CreateMarshaAslAttribute(FieldBuilder field, MarshalAsAttribute attrib)
    {
        field.SetCustomAttribute(
            new CustomAttributeBuilder(
                marshalAsCtor,
                new object[] { attrib.Value },
                new FieldInfo[] { typeof(MarshalAsAttribute).GetField("SizeConst") },
                new object[] { attrib.SizeConst }
            )
        );
    }

    public abstract void CreateRubricsProperty(TypeBuilder tb, Type type, string name);

    public abstract void CreateSerialCodeProperty(TypeBuilder tb, Type type, string name);

    public virtual void CreateSetUniqueKeyMethod(TypeBuilder tb)
    {
        MethodInfo createArray = typeof(IUnique).GetMethod("SetId");

        ParameterInfo[] args = createArray.GetParameters();
        Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

        MethodBuilder method = tb.DefineMethod(
            createArray.Name,
            createArray.Attributes & (~MethodAttributes.Abstract),
            createArray.CallingConvention,
            createArray.ReturnType,
            argTypes
        );
        tb.DefineMethodOverride(method, createArray);

        ILGenerator il = method.GetILGenerator();

        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Ldflda, rubricBuilders[0].Member.InstantCreatorField);
        il.EmitCall(OpCodes.Call, typeof(Uscn).GetMethod("SetId"), null);
        il.Emit(OpCodes.Ret);
    }

    public virtual void CreateSetUniqueSeedMethod(TypeBuilder tb)
    {
        MethodInfo createArray = typeof(IUnique).GetMethod("SetTypeId");

        ParameterInfo[] args = createArray.GetParameters();
        Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

        MethodBuilder method = tb.DefineMethod(
            createArray.Name,
            createArray.Attributes & (~MethodAttributes.Abstract),
            createArray.CallingConvention,
            createArray.ReturnType,
            argTypes
        );
        tb.DefineMethodOverride(method, createArray);

        ILGenerator il = method.GetILGenerator();

        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Ldflda, rubricBuilders[0].Member.InstantCreatorField);
        il.EmitCall(OpCodes.Call, typeof(Uscn).GetMethod("SetTypeId"), null);
        il.Emit(OpCodes.Ret);
    }

    public virtual void CreateIdProperty(TypeBuilder tb)
    {
        PropertyBuilder prop = tb.DefineProperty(
            "Id",
            PropertyAttributes.HasDefault,
            typeof(ulong),
            new Type[] { typeof(ulong) }
        );

        PropertyInfo iprop = typeof(IUnique).GetProperty("Id");

        MethodInfo accessor = iprop.GetGetMethod();

        ParameterInfo[] args = accessor.GetParameters();
        Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

        MethodBuilder getter = tb.DefineMethod(
            accessor.Name,
            accessor.Attributes & (~MethodAttributes.Abstract),
            accessor.CallingConvention,
            accessor.ReturnType,
            argTypes
        );
        tb.DefineMethodOverride(getter, accessor);

        prop.SetGetMethod(getter);
        ILGenerator il = getter.GetILGenerator();

        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldflda, rubricBuilders[0].Member.InstantCreatorField);
        il.EmitCall(OpCodes.Call, typeof(Uscn).GetProperty("Id").GetGetMethod(), null);
        il.Emit(OpCodes.Ret);

        MethodInfo mutator = iprop.GetSetMethod();

        args = mutator.GetParameters();
        argTypes = Array.ConvertAll(args, a => a.ParameterType);

        MethodBuilder setter = tb.DefineMethod(
            mutator.Name,
            mutator.Attributes & (~MethodAttributes.Abstract),
            mutator.CallingConvention,
            mutator.ReturnType,
            argTypes
        );
        tb.DefineMethodOverride(setter, mutator);

        prop.SetSetMethod(setter);
        il = setter.GetILGenerator();

        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldflda, rubricBuilders[0].Member.InstantCreatorField);
        il.Emit(OpCodes.Ldarg_1);
        il.EmitCall(OpCodes.Call, typeof(Uscn).GetProperty("Id").GetSetMethod(), null);
        il.Emit(OpCodes.Ret);
    }

    public virtual void CreateTypeIdProperty(TypeBuilder tb)
    {
        PropertyBuilder prop = tb.DefineProperty(
            "TypeId",
            PropertyAttributes.HasDefault,
            typeof(ulong),
            new Type[] { typeof(ulong) }
        );

        PropertyInfo iprop = typeof(IUnique).GetProperty("TypeId");

        MethodInfo accessor = iprop.GetGetMethod();

        ParameterInfo[] args = accessor.GetParameters();
        Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

        MethodBuilder getter = tb.DefineMethod(
            accessor.Name,
            accessor.Attributes & (~MethodAttributes.Abstract),
            accessor.CallingConvention,
            accessor.ReturnType,
            argTypes
        );
        tb.DefineMethodOverride(getter, accessor);

        prop.SetGetMethod(getter);
        ILGenerator il = getter.GetILGenerator();

        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldflda, rubricBuilders[0].Member.InstantCreatorField);
        il.EmitCall(OpCodes.Call, typeof(Uscn).GetProperty("TypeId").GetGetMethod(), null);
        il.Emit(OpCodes.Ret);

        MethodInfo mutator = iprop.GetSetMethod();

        args = mutator.GetParameters();
        argTypes = Array.ConvertAll(args, a => a.ParameterType);

        MethodBuilder setter = tb.DefineMethod(
            mutator.Name,
            mutator.Attributes & (~MethodAttributes.Abstract),
            mutator.CallingConvention,
            mutator.ReturnType,
            argTypes
        );
        tb.DefineMethodOverride(setter, mutator);

        prop.SetSetMethod(setter);
        il = setter.GetILGenerator();

        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldflda, rubricBuilders[0].Member.InstantCreatorField);
        il.Emit(OpCodes.Ldarg_1);
        il.EmitCall(OpCodes.Call, typeof(Uscn).GetProperty("TypeId").GetSetMethod(), null);
        il.Emit(OpCodes.Ret);
    }

    public abstract void CreateValueArrayProperty(TypeBuilder tb);

    public abstract TypeBuilder GetTypeBuilder(string typeName);

    public void ResolveMemberAttributes(FieldBuilder fb, MemberRubric mr)
    {
        MemberInfo mi = mr.RubricInfo;
        if (
            !(((IMemberRubric)mi).MemberInfo is FieldBuilder)
            && !(((IMemberRubric)mi).MemberInfo is PropertyBuilder)
        )
        {
            resolveInstantCreatorKeyAttributes(fb, mi, mr);

            resolveInstantCreatorIdentityAttributes(fb, mi, mr);

            resolveInstantCreatorRquiredAttributes(fb, mi, mr);

            resolveInstantCreatorDisplayAttributes(fb, mi, mr);

            resolveInstantCreatorTreatmentAttributes(fb, mi, mr);
        }
    }

    public void ResolveMarshalAsAttributeForArray(
        FieldBuilder field,
        MemberRubric member,
        Type type
    )
    {
        MemberInfo _member = member.RubricInfo;
        if ((member is MemberRubric) && (member.InstantCreatorField != null))
        {
            _member = member.InstantCreatorField;
        }

        object[] o = _member.GetCustomAttributes(typeof(MarshalAsAttribute), false);
        if ((o == null) || !o.Any())
        {
            o = _member.GetCustomAttributes(typeof(InstantAsAttribute), false);
            if ((o != null) && o.Any())
            {
                InstantAsAttribute faa = (InstantAsAttribute)o.First();
                CreateInstantCreatorAsAttribute(
                    field,
                    new InstantAsAttribute(UnmanagedType.ByValArray)
                    {
                        SizeConst = (faa.SizeConst < 1) ? 64 : faa.SizeConst
                    }
                );
            }
            else
            {
                int size = 64;
                if (member.RubricSize > 0)
                    size = member.RubricSize;
                CreateInstantCreatorAsAttribute(
                    field,
                    new InstantAsAttribute(UnmanagedType.ByValArray) { SizeConst = size }
                );
            }
        }
        else
        {
            MarshalAsAttribute maa = (MarshalAsAttribute)o.First();
            CreateMarshaAslAttribute(
                field,
                new MarshalAsAttribute(UnmanagedType.ByValArray)
                {
                    SizeConst = (maa.SizeConst < 1) ? 64 : maa.SizeConst
                }
            );
        }
    }

    public void ResolveMarshalAsAttributeForString(
        FieldBuilder field,
        MemberRubric member,
        Type type
    )
    {
        MemberInfo _member = member.RubricInfo;
        if ((member is MemberRubric) && (member.InstantCreatorField != null))
        {
            _member = member.InstantCreatorField;
        }

        object[] o = _member.GetCustomAttributes(typeof(MarshalAsAttribute), false);
        if ((o == null) || !o.Any())
        {
            o = _member.GetCustomAttributes(typeof(InstantAsAttribute), false);
            if ((o != null) && o.Any())
            {
                InstantAsAttribute maa = (InstantAsAttribute)o.First();
                CreateInstantCreatorAsAttribute(
                    field,
                    new InstantAsAttribute(UnmanagedType.ByValTStr)
                    {
                        SizeConst = (maa.SizeConst < 1) ? 64 : maa.SizeConst
                    }
                );
            }
            else
            {
                int size = 64;
                if (member.RubricSize > 0)
                    size = member.RubricSize;
                CreateInstantCreatorAsAttribute(
                    field,
                    new InstantAsAttribute(UnmanagedType.ByValTStr) { SizeConst = size }
                );
            }
        }
        else
        {
            MarshalAsAttribute maa = (MarshalAsAttribute)o.First();
            CreateMarshaAslAttribute(
                field,
                new MarshalAsAttribute(UnmanagedType.ByValTStr)
                {
                    SizeConst = (maa.SizeConst < 1) ? 64 : maa.SizeConst
                }
            );
        }
    }
}
