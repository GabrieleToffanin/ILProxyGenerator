using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

public class UtilGen<TObject>
{
    private static readonly PropertyInfo prop = typeof(TObject).GetProperty("Name");
    private static readonly PropertyInfo[] props = typeof(TObject).GetProperties();
    private static readonly MethodInfo baseGetGet = prop.GetGetMethod();
    private static readonly MethodInfo baseSetSet = prop.GetSetMethod();
    private readonly Converter<string, object> _expression;
    private FieldBuilder fb;
    private PropertyBuilder pb;
    private ConstructorBuilder ctor;

    public UtilGen()
    {
        AssemblyName myAsm = new AssemblyName("NewAsm");
        AssemblyBuilder ab = AssemblyBuilder.DefineDynamicAssembly(myAsm, AssemblyBuilderAccess.RunAndCollect);


        Assembly abBuilder = AssemblyBuilder.GetCallingAssembly();
        

        ModuleBuilder mb = ab.DefineDynamicModule(myAsm.Name);

        TypeBuilder tb = mb.DefineType("AProxy", TypeAttributes.Public, typeof(A));

        foreach(var item in props)
        {
            fb = BuildField(tb, "m_" + item.Name, item.PropertyType, FieldAttributes.Private);
            pb = BuildProp(tb, item.Name, item.Attributes, item.PropertyType, null);
        }

        FieldBuilder fbString = BuildField(tb, "m_name", typeof(string), FieldAttributes.Private);

        //pb = BuildProp(tb, "Name", PropertyAttributes.None, typeof(string), null);
        AddGetMethod(pb, tb, baseGetGet.Name, MethodAttributes.Public | MethodAttributes.HideBySig, prop.PropertyType, null);

        ctor = GenerateCtor(tb, MethodAttributes.Public, CallingConventions.HasThis, new Type[] { typeof(string) }, fbString);

        Type t = tb.CreateType();

        DynamicMethod dm = new DynamicMethod("Create", t.GetType(), new Type[] { typeof(string) });

        var ilDm = dm.GetILGenerator();
        ilDm.Emit(OpCodes.Ldarg_0);
        ilDm.Emit(OpCodes.Newobj, t.GetConstructor(new[] {typeof(string)}));
        ilDm.Emit(OpCodes.Ret);

        _expression = (Converter<string, object>)dm.CreateDelegate(typeof(Converter<string, object>));
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private PropertyBuilder BuildProp(TypeBuilder tb, string name, PropertyAttributes attr, Type propType, Type[] paramTypes)
    {
        return tb.DefineProperty(name,
            attr,
            propType,
            paramTypes);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private FieldBuilder BuildField(TypeBuilder tb, string name, Type fieldType, FieldAttributes fieldAttributes)
    {
        return tb.DefineField(
           name,
           fieldType,
            fieldAttributes);
    }

    private void AddGetMethod(PropertyBuilder pb,TypeBuilder tb, string name,MethodAttributes ma, Type returnType, Type[] possibleParams)
    {
        MethodBuilder getSet = tb.DefineMethod(baseGetGet.Name,
            MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
            prop.PropertyType,
            null);

        ILGenerator custNameGetIl = getSet.GetILGenerator();

        custNameGetIl.Emit(OpCodes.Ldarg_0);
        custNameGetIl.EmitCall(OpCodes.Call, baseGetGet, null);
        custNameGetIl.Emit(OpCodes.Ret);
    }

    private ConstructorBuilder GenerateCtor(TypeBuilder tb, MethodAttributes attributes, CallingConventions cc, Type[] ctorParams, FieldBuilder toInit)
    {
        Type objType = Type.GetType("System.Object");
        ConstructorInfo objCtor = objType.GetConstructor(new Type[0]);

        ConstructorBuilder ctor1 = tb.DefineConstructor(
            attributes,
            cc,
            ctorParams);

        ILGenerator il = ctor1.GetILGenerator();

        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Call, objCtor);

        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Stfld, toInit);

        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldarg_1);
        il.EmitCall(OpCodes.Call, baseSetSet, null);

        il.Emit(OpCodes.Ret);

        return ctor1;
    }

    public object Direct() => _expression("Ciao");
}

