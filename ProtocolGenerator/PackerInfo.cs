
namespace ProtocolGenerator;

class PackerInfo
{
    static Dictionary<string, string> packerInfos = new Dictionary<string, string>();
    static Dictionary<string, string> unpackInfos = new Dictionary<string, string>();

    public static void InitPacker()
    {
        AddPackerInfos();
    }

    static void AddPackerInfos()
    {
        AddPacker("int", "", "UnpackInt");
        AddPacker("bool", "", "UnpackBool");
        AddPacker("Boolean", "", "UnpackBool");
        AddPacker("string", "", "UnpackString");
        AddPacker("Common.ActorId", "", "UnpackActorId");
        AddPacker("Common.Mailbox", "", "UnpackMailbox");
        AddPacker("System.IO.MemoryStream", "", "UnpackMemoryStream");
    }

    static void AddPacker(string type, string packer, string unpacker)
    {
        if (packer == "")
        {
            packer = "Pack";
        }

        packerInfos[type] = packer;
        unpackInfos[type] = unpacker;
    }

    public static string GetPackerName(string typeName)
    {
        if (packerInfos.TryGetValue(typeName, out string? value))
        {
            return value;
        }

        return "Pack";
    }

    public static string GetUnpackerName(string typeName)
    {
        if (unpackInfos.TryGetValue(typeName, out string? value))
        {
            return value;
        }

        return $"UnpackProperty<{typeName}>";
    }
}