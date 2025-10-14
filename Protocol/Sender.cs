
//// 需要source generator生成的代码
//// 这里是测试用

//using Common;
//using System.IO;
//using System.Runtime.InteropServices;

//namespace Protocol.Sender;

//public class ILoginClient : Protocol.ILoginClient
//{
//    ISender sender;

//    public ILoginClient(ISender sender)
//    {
//        this.sender = sender;
//    }

//    public void LoginResult(bool isOk)
//    {
//        MemoryStream stream = new MemoryStream();

//        int rpcId = 1;
//        var npcIdData = BitConverter.GetBytes(rpcId);
//        stream.Write(npcIdData);

//        {
//            byte[] rawData = BitConverter.GetBytes(isOk);
//            stream.Write(rawData);
//        }

//        sender.Send(stream);
//    }

//    public void Echo(string msg)
//    {
//        MemoryStream stream = new MemoryStream();
//        int rpcId = 2;
//        var npcIdData = BitConverter.GetBytes(rpcId);
//        stream.Write(npcIdData);
//        // pack stream
//        {
//            var strData = MemoryMarshal.AsBytes(msg.AsSpan());
//            byte[] data = BitConverter.GetBytes((int)strData.Length);

//            stream.Write(data);
//            stream.Write(strData);
//        }

//        sender.Send(stream);
//    }

//    public void EchoBack(string msg)
//    {
//        MemoryStream stream = new MemoryStream();

//        int rpcId = 3;
//        var npcIdData = BitConverter.GetBytes(rpcId);
//        stream.Write(npcIdData);

//        {
//            var rawData = MemoryMarshal.AsBytes(msg.AsSpan());
//            byte[] lenData = BitConverter.GetBytes((int)rawData.Length);
//            stream.Write(lenData);
//            stream.Write(rawData);
//        }

//        sender.Send(stream);
//    }
//}

//public class ILoginCore_Sender : ILoginCore
//{
//    ISender sender;
//    public ILoginCore_Sender(ISender sender)
//    {
//        this.sender = sender;
//    }

//    public void Login(string name, string password)
//    {
//        MemoryStream stream = new MemoryStream();

//        int rpcId = 1;
//        var npcIdData = BitConverter.GetBytes(rpcId);
//        stream.Write(npcIdData);

//        {
//            var strData = MemoryMarshal.AsBytes(name.AsSpan());
//            byte[] data = BitConverter.GetBytes((int)strData.Length);

//            stream.Write(data);
//            stream.Write(strData);
//        }

//        {
//            var strData = MemoryMarshal.AsBytes(password.AsSpan());
//            byte[] data = BitConverter.GetBytes((int)strData.Length);

//            stream.Write(data);
//            stream.Write(strData);
//        }

//        sender.Send(stream);
//    }

//    public void Echo(string msg)
//    {

//        MemoryStream stream = new MemoryStream();

//        int rpcId = 2;
//        var npcIdData = BitConverter.GetBytes(rpcId);
//        stream.Write(npcIdData);

//        {
//            var strData = MemoryMarshal.AsBytes(msg.AsSpan());
//            byte[] data = BitConverter.GetBytes((int)strData.Length);

//            stream.Write(data);
//            stream.Write(strData);
//        }

//        sender.Send(stream);
//    }

//    public void EchoBack(string msg)
//    {
//        MemoryStream stream = new MemoryStream();

//        int rpcId = 3;
//        var npcIdData = BitConverter.GetBytes(rpcId);
//        stream.Write(npcIdData);

//        {
//            var strData = MemoryMarshal.AsBytes(msg.AsSpan());
//            byte[] data = BitConverter.GetBytes((int)strData.Length);

//            stream.Write(data);
//            stream.Write(strData);
//        }

//        sender.Send(stream);
//    }
//}