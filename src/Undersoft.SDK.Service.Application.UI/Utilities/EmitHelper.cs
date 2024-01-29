using System.Reflection;
using System.Reflection.Emit;

namespace Undersoft.SDK.Service.Application.Components;

public static class EmitHelper
{
    public static Type? CreateTypeByName(string typeName, IEnumerable<ITableColumn> cols, Type? parent = null, Func<ITableColumn, IEnumerable<CustomAttributeBuilder>>? creatingCallback = null)
    {
        var typeBuilder = CreateTypeBuilderByName(typeName, parent);

        foreach (var col in cols)
        {
            var attributeBuilds = creatingCallback?.Invoke(col);
            typeBuilder.CreateProperty(col, attributeBuilds);
        }
        return typeBuilder.CreateType();
    }

    private static TypeBuilder CreateTypeBuilderByName(string typeName, Type? parent = null)
    {
        var assemblyName = new AssemblyName("BootstrapBlazor_DynamicAssembly");
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule("BootstrapBlazor_DynamicAssembly_Module");
        var typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Public, parent);
        return typeBuilder;
    }

    private static void CreateProperty(this TypeBuilder typeBuilder, IEditorItem col, IEnumerable<CustomAttributeBuilder>? attributeBuilds = null) => CreateProperty(typeBuilder, col.GetFieldName(), col.PropertyType, attributeBuilds);

    private static void CreateProperty(this TypeBuilder typeBuilder, string propertyName, Type propertyType, IEnumerable<CustomAttributeBuilder>? attributeBuilds = null)
    {
        var fieldName = propertyName;
        var field = typeBuilder.DefineField($"_{fieldName}", propertyType, FieldAttributes.Private);

        var methodGetField = typeBuilder.DefineMethod($"Get{fieldName}", MethodAttributes.Public, propertyType, null);
        var methodSetField = typeBuilder.DefineMethod($"EntitySet{fieldName}", MethodAttributes.Public, null, new Type[] { propertyType });

        var ilOfGetId = methodGetField.GetILGenerator();
        ilOfGetId.Emit(OpCodes.Ldarg_0);
        ilOfGetId.Emit(OpCodes.Ldfld, field);
        ilOfGetId.Emit(OpCodes.Ret);

        var ilOfSetId = methodSetField.GetILGenerator();
        ilOfSetId.Emit(OpCodes.Ldarg_0);
        ilOfSetId.Emit(OpCodes.Ldarg_1);
        ilOfSetId.Emit(OpCodes.Stfld, field);
        ilOfSetId.Emit(OpCodes.Ret);

        var propertyId = typeBuilder.DefineProperty(fieldName, PropertyAttributes.None, propertyType, null);
        propertyId.SetGetMethod(methodGetField);
        propertyId.SetSetMethod(methodSetField);

        foreach (var cab in attributeBuilds ?? Enumerable.Empty<CustomAttributeBuilder>())
        {
            propertyId.SetCustomAttribute(cab);
        }
    }
}
