
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.Emit;

namespace Services
{
    public class RemoteBuilder
    {
        public static Dictionary<Type, Type> types = new Dictionary<Type, Type>();
        public static ModuleBuilder moduleBuilder = CreateModuleBuilder();

        public static T Build<T>(TcpClient client)
        {
            Type type = typeof(T);
            if (!types.ContainsKey(type))
            {
                Type newType = CreateRemoteType(type);
                types.Add(type, newType);
            }

            Type remoteType = types[type];

            T obj = (T)Activator.CreateInstance(remoteType)!;
            FieldInfo clientInfo = remoteType.GetField("client")!;
            clientInfo.SetValue(obj, client);
            return obj;
        }

        static ModuleBuilder CreateModuleBuilder()
        {
            var name = new AssemblyName("ServicesRemote");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);

            return assemblyBuilder.DefineDynamicModule("Remote");
        }

        static Type CreateRemoteType(Type type)
        {
            TypeBuilder builder = moduleBuilder.DefineType(
                $"{type.Name}_Remote", 
                TypeAttributes.Public | TypeAttributes.Class, 
                typeof(object), 
                new Type[] {type});

            // define the attribute
            FieldInfo clientInfo = builder.DefineField("client", typeof(TcpClient), FieldAttributes.Public);

            MethodInfo[] methods = type.GetMethods();
            int[] methodIds = GenMethodIds(methods);

            for (int index = 0; index < methodIds.Length; ++index)
            {
                int methodId = methodIds[index];
                AddRemoteMethod(builder, clientInfo, methodId, methods[index]);
            }
            
            return builder.CreateType();
        }

        public static int[] GenMethodIds(MethodInfo[] infos)
        {
            Dictionary<string, int> idDict = new Dictionary<string, int>();

            foreach (MethodInfo info in infos)
            {
                idDict.Add(info.Name, 0);
            }

            var orderedIds = idDict.Keys.ToArray().Order().ToArray();
            int idBegin = 10; // 前面0~9 共10个编号留着做特殊用途
            for (int index = 0; index < orderedIds.Length; ++index)
            {
                int id = idBegin + index;
                string name = orderedIds[index];
                idDict[name] = id;
            }
            
            
            int[] result = new int[infos.Length];
            for (int index = 0; index < result.Length; ++index)
            {
                MethodInfo info = infos[index];
                result[index] = idDict[info.Name];
            }

            return result;
        }

        static void EmitMethodCode(ILGenerator code, FieldInfo client, int methodId, Type[] parameters)
        {
            // 调用 remote.Name(arg1, arg2, arg3);
            // MemoryStream stream = new MemoryStream();
            ConstructorInfo con = typeof(MemoryStream).GetConstructor(Type.EmptyTypes)!;
            code.Emit(OpCodes.Newobj, con); // memoryStream

            // Packer.PackInt(stream, methodId)
            PackerInfo intInfo = PackerInfo.Get(typeof(int));
            code.Emit(OpCodes.Dup); // memoryStream, memoryStream
            code.Emit(OpCodes.Ldc_I4, methodId); // memoryStream, memoryStream, methodId
            code.EmitCall(OpCodes.Call, intInfo.packer, Type.EmptyTypes); // memoryStream

            for (int index = 0; index < parameters.Length; ++index)
            {
                // Packer.Pack{Int, String..}(stream, arg1);
                Type param = parameters[index];
                code.Emit(OpCodes.Dup); // memoryStream, memoryStream
                code.Emit(OpCodes.Ldarg, index + 1); // memorystream, memoryStream, arg_index
                PackerInfo info = PackerInfo.Get(param);
                code.EmitCall(OpCodes.Call, info.packer, Type.EmptyTypes); // memoryStream
            }
            
            // RemoteBuilder.SendStream(meoryStream, client)
            code.Emit(OpCodes.Ldarg_0); // memoryStream, this
            code.Emit(OpCodes.Ldfld, client); // memoryStream, TcpClient
            MethodInfo packStream = typeof(RemoteBuilder).GetMethod(
                "SendStream",
                BindingFlags.Static | BindingFlags.Public)!;
            code.EmitCall(OpCodes.Call, packStream, Type.EmptyTypes); // (empty)

            code.Emit(OpCodes.Ret);
        }

        public static void SendStream(MemoryStream stream, TcpClient client)
        {
            byte[] lenBuff = BitConverter.GetBytes((int)stream.Length);
            NetworkStream netStream = client.GetStream();
            // send the package
            netStream.Write(lenBuff);
            netStream.Write(stream.GetBuffer(), 0, (int)stream.Length);
            netStream.Flush();
        }

        static void AddRemoteMethod(TypeBuilder builder, FieldInfo client, int methodId, MethodInfo methodInfo)
        {
            Type[] parameterTypes = new Type[methodInfo.GetParameters().Length];
            for (int index = 0; index < parameterTypes.Length; ++index)
            {
                parameterTypes[index] = methodInfo.GetParameters()[index].ParameterType;
            }

            MethodBuilder methodBuilder = builder.DefineMethod(
                methodInfo.Name, 
                MethodAttributes.Public | MethodAttributes.Virtual,
                null,
                parameterTypes
                );

            ILGenerator code = methodBuilder.GetILGenerator();

            EmitMethodCode(code, client, methodId, parameterTypes);
        }
    }
}
