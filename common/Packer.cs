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
}