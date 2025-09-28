
using System.ComponentModel;
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace services
{
    /// <summary>
    ///  class Remote
    ///  {
    ///     TcpClient client;
    ///     NetworkStream stream;
    ///     public Remote(TcpClient client) {
    ///         this.client = client;
    ///         this.stream = stream;
    ///     }
    ///     public Close() {
    ///     
    ///     }
    ///     public void Echo(string msg)
    ///     {
    ///         MemoryStream stream = new MemoryStream();
    ///         stream.write(ToStream(msg));
    ///     }
    ///  }
    /// </summary>
    public class RemoteBuilder
    {
        public static Dictionary<Type, Type> types = new Dictionary<Type, Type>();
        public static ModuleBuilder moduleBuilder = CreateModuleBuilder();

        public static object Build(Type type)
        {
            if (types.TryGetValue(type, out var value))
            {
                return Activator.CreateInstance(value)!;
            }

            Type newType = CreateRemoteType(type);
            types.Add(type, newType);

            return Activator.CreateInstance(newType)!;
        }

        static ModuleBuilder CreateModuleBuilder()
        {
            var name = new AssemblyName("Remote");
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

//            builder.AddInterfaceImplementation(type);

            foreach (var methodInfo in type.GetMethods())
            {
                AddRemoteMethod(builder, methodInfo);
            }
            
            return builder.CreateType();
        }

        static void AddRemoteMethod(TypeBuilder builder, MethodInfo methodInfo)
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

            ConstructorInfo memoryStreamCon = typeof(MemoryStream).GetConstructor(Type.EmptyTypes)!;
            code.Emit(OpCodes.Newobj, memoryStreamCon);

            //            code.Emit(OpCodes.Pop);
            //code.Emit(OpCodes.Ldind_I4, 1024);

            //MethodInfo methodPack = typeof(RemoteBuilder).GetMethod(
            //        "Pack", 
            //        new Type[] {typeof(MemoryStream), typeof(int)}
            //    )!;
            //code.EmitCall(OpCodes.Call, methodPack, null);
            MethodInfo methodPack = typeof(RemoteBuilder).GetMethod(
                    "Pack", 
                    new Type[] {typeof(MemoryStream), typeof(int)}
                )!;
            code.Emit(OpCodes.Ldc_I4, 1025);
            code.EmitCall(OpCodes.Call, methodPack, null);

            code.Emit(OpCodes.Ret);
            //// void methodName(arg1, arg2, arg3, arg3)
            //// MemoryStream memory = new MemoryStream();
            //// byte[] buff = new byte[4];
            //// memory.Write(buff);
            //// memory.Flush();
            //code.EmitWriteLine("hello!!!");
            ////ConstructorInfo ctor = typeof(MemoryStream).GetConstructor(Type.EmptyTypes)!;
            ////code.Emit(OpCodes.Ldarg_1);
            ////code.EmitWriteLine("hello!!!");

            //MethodInfo newStream = typeof(RemoteBuilder).GetMethod("NewStream", Type.EmptyTypes)!;

            //code.EmitCall(OpCodes.Call, newStream, null);
            //code.Emit(OpCodes.Pop);

            ////MethodInfo method = typeof(RemoteBuilder).GetMethod("Pack", new Type[] {typeof(MemoryStream), typeof(int)})!;
            //MethodInfo method = typeof(RemoteBuilder).GetMethod("Test", 
            //    Type.EmptyTypes)!;
            //code.EmitCall(OpCodes.Call, method, Type.EmptyTypes);

            ////code.EmitCall(OpCodes.Call, method, null);
            //code.Emit(OpCodes.Ret);
        }

        public static MemoryStream NewStream()
        {
            return new MemoryStream();
        }

        public static void Test()
        {
            Console.WriteLine("test call!!!");
        }

        public static void Pack2(int i1)
        {
            Console.WriteLine($"call by generate code: {i1}");
        }

        public static void Pack2(int i1, int i2)
        {
            Console.WriteLine($"call by generate code: {i1}, {i2}");
        }

        public static void Pack(MemoryStream stream, int value)
        {
            Console.WriteLine($"Call by generated code: {stream}, {value}");
        }

        public static void Pack(int i1)
        {
            Console.WriteLine($"pack i1: {i1}");
        }
    }
}