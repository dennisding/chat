
// 需要source generator生成的代码
// 这里是测试用

using Common;
using System.Runtime.InteropServices;
using System.Text;

namespace Protocol.Dispatcher;

public class ILoginClient // : Common.IDispatcher<Protocol.ILoginClient>
{
    public static void Dispatch(Protocol.ILoginClient ins, BinaryReader reader)
    {
        int rpcId = reader.ReadInt32();
        switch (rpcId)
        {
            case 1:
                {
                    var isOk = reader.ReadBoolean();
                    ins.LoginResult(isOk);
                    break;
                }
            case 2:
                {
                    string msg = ((Func<string>)(() => {
                        int len = reader.ReadInt32();
                        byte[] data = reader.ReadBytes(len);

                        return Encoding.Unicode.GetString(data);
                    }))();
                    ins.Echo(msg);
                    break;
                }

            case 3:
                {
                    string msg = ((Func<string>)(() => {
                        int len = reader.ReadInt32();
                        byte[] data = reader.ReadBytes(len);

                        return Encoding.Unicode.GetString(data);
                    }))();
                    ins.EchoBack(msg);
                    break;
                }
            default:
                {
                    Console.WriteLine($"invalid rpcId: {rpcId}");
                    break;
                }
        }
    }
}

public class ILoginCore_Dispatcher
{
    public static void Dispatch(ILoginCore ins, BinaryReader reader)
    {
        int rpcId = reader.ReadInt32();
        switch (rpcId)
        {
            case 1:
                {
                    string name = ((Func<string>)(() => {
                        int len = reader.ReadInt32();
                        byte[] data = reader.ReadBytes(len);

                        return Encoding.Unicode.GetString(data);
                    }))();

                    string password = ((Func<string>)(() => {
                        int len = reader.ReadInt32();
                        byte[] data = reader.ReadBytes(len);

                        return Encoding.Unicode.GetString(data);
                    }))();

                    ins.Login(name, password);
                    break;
                }
            case 2:
                {
                    string msg = ((Func<string>)(() => {
                        int len = reader.ReadInt32();
                        byte[] data = reader.ReadBytes(len);

                        return Encoding.Unicode.GetString(data);
                    }))();
                    ins.Echo(msg);
                    break;
                }

            case 3:
                {
                    string msg = ((Func<string>)(() => {
                        int len = reader.ReadInt32();
                        byte[] data = reader.ReadBytes(len);

                        return Encoding.Unicode.GetString(data);
                    }))();
                    ins.EchoBack(msg);
                    break;
                }
            default:
                {
                    Console.WriteLine($"invalid rpcId: {rpcId}");
                    break;
                }
        }
    }
}