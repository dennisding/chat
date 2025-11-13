
using Common;
using System.Runtime.CompilerServices;

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
    [Common.PropertyAttribute(PropertyFlag.ServerOnly, 2000)]
    int _hp;

    [Common.PropertyAttribute(PropertyFlag.Client, 100)]
    string _name = "无";

    [Common.PropertyAttribute(PropertyFlag.Client)]
    ActorId _friend;
}

//// 这个类将由ProtocolGenerator自动生成
//public partial class ChatData
//{
//    public static ClassInfo classInfo = CreateClassInfo();

//    public static ClassInfo CreateClassInfo()
//    {
//        ClassInfo info = new ClassInfo("ChatData");

//        info.AddPropertyInfo(new Common.PropertyInfo(10, PropertyFlag.Client, "hp", _Pack_hp, _Unpack_hp));
//        info.AddPropertyInfo(new Common.PropertyInfo(11, PropertyFlag.Client, "name", _Pack_name, _Unpack_name));

//        info.Build();
//        return info;
//    }

//    public override ClassInfo GetClassInfo()
//    {
//        return classInfo;
//    }

//    public static void _Pack_hp(object obj, MemoryStream stream)
//    {
//        ChatData self = (ChatData)obj;
//        //int index = 10;
//        //Common.Packer.PackInt(stream, index);
//        Common.Packer.PackInt(stream, self._hp);
//    }

//    public static void _Unpack_hp(object obj, BinaryReader reader)
//    {
//        ChatData self = (ChatData)obj;
//        int value = reader.ReadInt32();
//        self.hp = value;
//    }

//    public static Common.PropertyInfo _hp_Info = classInfo.GetPropertyInfo("hp");
//    public int hp
//    {
//        get { return this._hp; }
//        set
//        {
//            this._hp = value;

//            OnPropertyChanged(_hp_Info);
//        }
//    }

//    // name
//    public static void _Pack_name(object obj, MemoryStream stream)
//    {
//        ChatData self = (ChatData)obj;
//        //int index = 11;
//        //Common.Packer.PackInt(stream, index);
//        Common.Packer.PackString(stream, self._name);
//    }

//    public static void _Unpack_name(object obj, BinaryReader reader)
//    {
//        ChatData self = (ChatData)obj;

//        string name = Common.Packer.UnpackString(reader);
//        self.name = name;
//    }

//    public static Common.PropertyInfo _name_Info = classInfo.GetPropertyInfo("name");
//    public string name
//    {
//        get { return this._name; }
//        set
//        {
//            this._name = value;
//            OnPropertyChanged(_name_Info);
//        }
//    }
//}