
using Common;
using System.IO.Pipelines;
using System.Security.Cryptography.X509Certificates;

namespace Protocol;

[Common.Protocol]
public interface ILoginClient
{
    void LoginResult(bool isOk);
    void Echo(string msg);
    void EchoBack(string msg);
}

[Common.Protocol]
public interface ILoginServer
{
    void Login(string name, string password);
    void Echo(string msg);
    void EchoBack(string msg);

    void CheckUsernameResult(bool isOk);
}

[Common.Protocol]
public interface IChatClient
{
    void ShowMessage(string msg);

    void SendData(ChatData data);
}

[Common.Protocol]
public interface IChatServer
{
    void ShowMessage(string msg);

    void NewRoom(string name);
    void NewRoomResult(bool isOk, ActorId roomId);
    void EnterRoom(string name);
    void OnEnterRoom(ActorId roomId, bool isLobby);

    void LeaveRoom();

    void ClientMessage(string msg);
    void ChatMessage(string msg);

    void MessageTo(string userName, string msg);
}

public interface IChatShadow
{
    void ShowMessage(string msg);
}

[Common.PropertyAttribute]
public partial class ChatData : Common.Property
{
    [Common.PropertyAttribute(PropertyFlag.ClientOnly, 2000)]
    int _hp;

    [Common.PropertyAttribute(PropertyFlag.ClientOnly, 100)]
    string _name = "无";

    [Common.PropertyAttribute(PropertyFlag.ClientOnly)]
    ActorId _friend;
}

//public class ProtocolCreator
//{
//    public static T CreatePacker<T>(ISender sender, PropertyFlag flag = PropertyFlag.All)
//    {
//        if (typeof(T) == typeof(ILoginClient))
//        {
//            return (T)(object)new ILoginClient_Packer(sender, flag);
//        }
//        else if (typeof(T) == typeof(ILoginServer))
//        {
//            return (T)(object)new ILoginServer_Packer(sender, flag);
//        }

//        throw new NotImplementedException();
//    }

//    public static IDispatcher CreateDispatcher<T>()
//    {
//        throw new NotImplementedException();
//    }
//}

//public partial class ChatData
//{
//    public int hp
//    {
//        get { return this._hp; }
//        set
//        {
//            this._hp = value;
//            OnPropertyChanged(ChatData_ClassInfo._hp_Info);
//        }
//    }

//    public string name
//    {
//        get { return this._name; }
//        set
//        {
//            this._name = value;
//            OnPropertyChanged(ChatData_ClassInfo._name_Info);
//        }
//    }

//    public ActorId friend
//    {
//        get { return this._friend; }
//        set
//        {
//            this._friend = value;
//            OnPropertyChanged(ChatData_ClassInfo._friend_Info);
//        }
//    }
//}

//public class ChatData_ClassInfo
//{
//    public static ClassInfo classInfo = CreateClassInfo();

//    static ClassInfo CreateClassInfo()
//    {
//        ClassInfo info = new ClassInfo("ChatData");

//        info.AddPropertyInfo(new PropertyInfomation(10, "hp", PropertyFlag.ClientOnly));
//        info.AddPropertyInfo(new PropertyInfomation(11, "name", PropertyFlag.ClientOnly));
//        info.AddPropertyInfo(new PropertyInfomation(12, "friend", PropertyFlag.ClientOnly));

//        return info;
//    }

//    public static PropertyInfomation _hp_Info = classInfo.GetPropertyInfo("hp");
//    public static void _Pack_hp(IDataStreamWriter datas, object ins)
//    {
//        ChatData self = (ChatData)ins;
//        Common.Packer.Pack(datas, _hp_Info, self.hp);
//    }

//    public static void _Unpack_hp(IDataStreamReader reader, object ins)
//    {
//        ChatData self = (ChatData)ins;
//        self.hp = Common.Packer.UnpackInt(reader, _hp_Info);
//    }

//    public static PropertyInfomation _name_Info = classInfo.GetPropertyInfo("name");
//    public static void _Pack_name(IDataStreamWriter datas, object ins)
//    {
//        ChatData self = (ChatData)ins;
//        Common.Packer.Pack(datas, _name_Info, self.name);
//    }

//    public static void _Unpack_name(IDataStreamReader reader, object ins)
//    {
//        ChatData self = (ChatData)ins;
//        self.name = Common.Packer.UnpackString(reader, _name_Info);
//    }

//    public static PropertyInfomation _friend_Info = classInfo.GetPropertyInfo("friend");
//    public static void _Pack_friend(IDataStreamWriter datas, object ins)
//    {
//        ChatData self = (ChatData)ins;
//        Common.Packer.Pack(datas, _friend_Info, self.friend);
//    }

//    public static void _Unpack_friend(IDataStreamReader reader, object ins)
//    {
//        ChatData self = (ChatData)ins;
//        self.friend = Common.Packer.UnpackActorId(reader, _friend_Info);
//    }
//}