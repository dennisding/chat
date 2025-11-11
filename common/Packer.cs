using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

namespace Common;

public class Packer
{
    // pack int
    public static void PackInt(MemoryStream stream, int value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        stream.Write(bytes);
    }

    public static int UnpackInt(BinaryReader reader)
    {
        return reader.ReadInt32();
    }

    public static void PackBool(MemoryStream stream, bool value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        stream.Write(bytes);
    }

    public static bool UnpackBool(BinaryReader reader)
    {
        return reader.ReadBoolean();
    }

    // pack string
    public static void PackString(MemoryStream stream, string value)
    {
        var rawBytes = MemoryMarshal.AsBytes(value.AsSpan());
        var lenBytes = BitConverter.GetBytes((int)rawBytes.Length);

        stream.Write(lenBytes, 0, lenBytes.Length);
        stream.Write(rawBytes);
    }

    public static string UnpackString(BinaryReader reader)
    {
        int len = reader.ReadInt32();
        byte[] data = reader.ReadBytes(len);

        return Encoding.Unicode.GetString(data) ;
    }

    // pack MemoryStream
    public static void PackMemoryStream(MemoryStream stream, MemoryStream value)
    {
        stream.Write(value.GetBuffer(), 0, (int)value.Length);
    }

    public static MemoryStream UnpackMemoryStream(BinaryReader reader)
    {
        long size = reader.BaseStream.Length - reader.BaseStream.Position;
        return new MemoryStream(reader.ReadBytes((int)size));
    }

    // pack ActorId
    public static void PackActorId(MemoryStream stream, ActorId actorId)
    {
        byte[] rawData = BitConverter.GetBytes(actorId.value);
        stream.Write(rawData);
    }

    public static ActorId UnpackActorId(BinaryReader reader)
    {
        long value = reader.ReadInt64();
        return new ActorId(value);
    }

    public static void PackProperty<T>(MemoryStream stream, T value)
        where T : IProperty
    {
        value.PackTo(stream);
    }

    public static T UnpackProperty<T>(BinaryReader reader)
        where T : IProperty, new()
    {
        T value = new T();
        value.UnpackFrom(reader);
        return value;
    }
}