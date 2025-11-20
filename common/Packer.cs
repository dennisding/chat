using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

namespace Common;

public class Packer
{
    static Dictionary<Type, Type> propertyTypes = new Dictionary<Type, Type>();

    public static void RegisterType(Type dataType, Type implType)
    {
        propertyTypes.Add(dataType, implType);
    }

    public static void Pack(IDataStreamWriter writer, PropertyInfomation info, bool value)
    {
        writer.Write(info, value);
    }

    public static bool UnpackBool(IDataStreamReader reader, PropertyInfomation info)
    {
        return reader.ReadBool(info);
    }

    public static void Pack(IDataStreamWriter datas, PropertyInfomation info, int value)
    {
        datas.Write(info, value);
    }

    public static int UnpackInt(IDataStreamReader reader, PropertyInfomation info)
    {
        return reader.ReadInt(info);
    }

    public static void Pack(IDataStreamWriter datas, PropertyInfomation info, string value)
    {
        datas.Write(info, value);
    }

    public static string UnpackString(IDataStreamReader reader, PropertyInfomation info)
    {
        return reader.ReadString(info);
    }

    public static void Pack(IDataStreamWriter datas, PropertyInfomation info, ActorId aid)
    {
        datas.Write(info, aid.value);
    }

    public static ActorId UnpackActorId(IDataStreamReader reader, PropertyInfomation info)
    {
        return new ActorId(reader.ReadLong(info));
    }

    public static void Pack(IDataStreamWriter writer, PropertyInfomation info, MemoryStream stream)
    {
        writer.Write(info, stream);
    }

    public static MemoryStream UnpackMemoryStream(IDataStreamReader reader, PropertyInfomation info)
    {
        return reader.ReadMemoryStream(info);
    }

    public static void Pack(IDataStreamWriter datas, PropertyInfomation info, Mailbox mailbox)
    {
        // no pack!
        if (!datas.Begin(info))
        {
            return;
        }

        Pack(datas, ClassInfo.builtinInfos[0], mailbox.office);
        Pack(datas, ClassInfo.builtinInfos[1], mailbox.actor);

        datas.End();
    }

    public static Mailbox UnpackMailbox(IDataStreamReader reader, PropertyInfomation info)
    {
        ActorId office = UnpackActorId(reader, ClassInfo.builtinInfos[0]);
        ActorId actor = UnpackActorId(reader, ClassInfo.builtinInfos[1]);

        return new Mailbox(office, actor);
    }

    public static void Pack(IDataStreamWriter writer, PropertyInfomation info, IProperty prop)
    {
        //        value.PackTo(stream);
        prop.PackTo(writer, info);
    }

    public static T UnpackProperty<T>(IDataStreamReader reader, PropertyInfomation info)
    where T : IProperty, new()
    {
        T? value;

        if (propertyTypes.TryGetValue(typeof(T), out Type? implType))
        {
            value = (T?)Activator.CreateInstance(implType);
        }
        else
        {
            value = new T();
        }

        value!.UnpackFrom(reader, info);
        return value;
    }

    //// pack int
    //public static void PackInt(MemoryStream stream, int value)
    //{
    //    byte[] bytes = BitConverter.GetBytes(value);
    //    stream.Write(bytes);
    //}

    //public static int UnpackInt(BinaryReader reader)
    //{
    //    return reader.ReadInt32();
    //}

    //public static void PackBool(MemoryStream stream, bool value)
    //{
    //    byte[] bytes = BitConverter.GetBytes(value);
    //    stream.Write(bytes);
    //}

    //public static bool UnpackBool(BinaryReader reader)
    //{
    //    return reader.ReadBoolean();
    //}

    //// pack string
    //public static void PackString(MemoryStream stream, string value)
    //{
    //    var rawBytes = MemoryMarshal.AsBytes(value.AsSpan());
    //    var lenBytes = BitConverter.GetBytes((int)rawBytes.Length);

    //    stream.Write(lenBytes, 0, lenBytes.Length);
    //    stream.Write(rawBytes);
    //}

    //public static string UnpackString(BinaryReader reader)
    //{
    //    int len = reader.ReadInt32();
    //    byte[] data = reader.ReadBytes(len);

    //    return Encoding.Unicode.GetString(data) ;
    //}

    //// pack MemoryStream
    //public static void PackMemoryStream(MemoryStream stream, MemoryStream value)
    //{
    //    stream.Write(value.GetBuffer(), 0, (int)value.Length);
    //}

    //public static MemoryStream UnpackMemoryStream(BinaryReader reader)
    //{
    //    long size = reader.BaseStream.Length - reader.BaseStream.Position;
    //    return new MemoryStream(reader.ReadBytes((int)size));
    //}

    //// pack ActorId
    //public static void PackActorId(MemoryStream stream, ActorId actorId)
    //{
    //    byte[] rawData = BitConverter.GetBytes(actorId.value);
    //    stream.Write(rawData);
    //}

    //public static ActorId UnpackActorId(BinaryReader reader)
    //{
    //    long value = reader.ReadInt64();
    //    return new ActorId(value);
    //}

    //public static void PackMailbox(MemoryStream stream, Mailbox mailbox)
    //{
    //    stream.Write(BitConverter.GetBytes(mailbox.office.value));
    //    stream.Write(BitConverter.GetBytes(mailbox.actor.value));
    //}

    //public static Mailbox UnpackMailbox(BinaryReader reader)
    //{
    //    long postOffice = reader.ReadInt64();
    //    long actor = reader.ReadInt64();

    //    return new Mailbox(new ActorId(postOffice), new ActorId(actor));
    //}

    //public static void PackProperty<T>(MemoryStream stream, T value)
    //    where T : IProperty
    //{
    //    value.PackTo(stream);
    //}

    //public static T UnpackProperty<T>(BinaryReader reader)
    //    where T : IProperty, new()
    //{
    //    T? value;

    //    if (propertyTypes.TryGetValue(typeof(T), out Type? implType))
    //    {
    //        value = (T?)Activator.CreateInstance(implType);
    //    }
    //    else
    //    {
    //        value = new T();
    //    }

    //    value!.UnpackFrom(reader);
    //    return value;
    //}
}