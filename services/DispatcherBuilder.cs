
namespace Services
{
    using System.Reflection;
    using System.Reflection.Emit;
    using DispatcherDict = Dictionary<int, DispatcherDelegate>;

    public delegate void DispatcherDelegate(object ins, BinaryReader reader);

    public class DispatcherBuilder
    {
        public static ModuleBuilder moduleBuilder = CreateModuleBuilder();
        public static Dictionary<Type, DispatcherDict> dispatchers = new Dictionary<Type, DispatcherDict>();

        static ModuleBuilder CreateModuleBuilder()
        {
            var name = new AssemblyName("ServicesDispatcher");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);

            return assemblyBuilder.DefineDynamicModule("Dispatcher");
        }

        // 返回了一个实现了inter 接口的 dispatcher
        public static Dispatcher Build(Type inter)
        {
            DispatcherDict dispatcherDict = GetDispatcherDict(inter);
            Dispatcher dispatcher = new Dispatcher(dispatcherDict);

            return dispatcher;
        }

        static DispatcherDict GetDispatcherDict(Type inter)
        {
            if (dispatchers.ContainsKey(inter))
            {
                return dispatchers[inter];
            }

            DispatcherDict result = new DispatcherDict();
            Type dispatcherType = CreateDispatchType(inter);

            // gether dispatch method
            MethodInfo[] methodInfos = inter.GetMethods();
            int[] ids = RemoteBuilder.GenMethodIds(methodInfos);
            for (int index = 0; index < ids.Length; ++index)
            {
                int methodId = ids[index];
                MethodInfo method = dispatcherType.GetMethod(methodInfos[index].Name)!;
                DispatcherDelegate dispatcher = method.CreateDelegate<DispatcherDelegate>();

                result[methodId] = dispatcher;
            }

            return result;
        }

        static Type CreateDispatchType(Type inter)
        {
            TypeBuilder builder = moduleBuilder.DefineType(
                $"{inter.Name}_Dispatcher",
                TypeAttributes.Public | TypeAttributes.Class,
                typeof(object),
                new Type[] { inter });

            MethodInfo[] methods = inter.GetMethods();
            foreach (MethodInfo method in methods)
            {
                AddDispatchMethod(builder, inter, method);
            }

            return builder.CreateType();
        }

        static void AddDispatchMethod(TypeBuilder builder, Type inter, MethodInfo method)
        {
            Type[] parameterTypes = new Type[] { typeof(object), typeof(BinaryReader)};

            MethodBuilder methodBuilder = builder.DefineMethod(
                method.Name,
                MethodAttributes.Public | MethodAttributes.Static,
                null,
                parameterTypes
                );

            ILGenerator code = methodBuilder.GetILGenerator();
            EmitMethodCode(code, inter, method);
        }

        static void EmitMethodCode(ILGenerator code, Type inter, MethodInfo method)
        {
            // Dispatch(object ins, BinaryReader reader)
            ParameterInfo[] parameterInfos = method.GetParameters();
            Type[] parameterTypes = new Type[parameterInfos.Length];

            // this = (InterfaceType)ins;
            code.Emit(OpCodes.Ldarg_0); /// this, args...
            code.Emit(OpCodes.Castclass, inter); // this, args...

            // unpack the parameter
            // ValueType value = packer.Unpack{Int, String}(binaryReader);
            for (int index = 0; index < parameterInfos.Length; index++)
            {
                ParameterInfo parameter = parameterInfos[index];
                parameterTypes[index] = parameter.ParameterType;

                PackerInfo packInfo = PackerInfo.Get(parameter.ParameterType);
                code.Emit(OpCodes.Ldarg_1);
                code.Emit(OpCodes.Call, packInfo.unpacker);
            }

            // call the ins.Name(args...)
            MethodInfo instanceMethod = inter.GetMethod(method.Name)!;
            code.EmitCall(OpCodes.Callvirt, instanceMethod, parameterTypes);

            code.Emit(OpCodes.Ret);          
        }
    }

    public class Dispatcher
    {
        // 用一个字典来存dispatcher
        public DispatcherDict dispatchers;

        public Dispatcher(DispatcherDict dispatchers)
        {
            this.dispatchers = dispatchers;
        }

        public void Dispatch(object ins, BinaryReader reader)
        {
            int rpcId = reader.ReadInt32();
            if (dispatchers!.TryGetValue(rpcId, out DispatcherDelegate? dispatcher))
            {
                dispatcher.Invoke(ins, reader);
            }
            else
            {
                Console.WriteLine($"invalid rpcId: {rpcId}");
            }
        }
    }
}