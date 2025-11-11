
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
        AddPacker("int", "Common.Packer.PackInt", "Common.Packer.UnpackInt");
        AddPacker("bool", "Common.Packer.PackBool", "Common.Packer.UnpackBool");
        AddPacker("Boolean", "Common.Packer.PackBool", "Common.Packer.UnpackBool");
        AddPacker("string", "Common.Packer.PackString", "Common.Packer.UnpackString");
        AddPacker("Common.ActorId", "Common.Packer.PackActorId", "Common.Packer.UnpackActorId");
        AddPacker("System.IO.MemoryStream", "Common.Packer.PackMemoryStream", "Common.Packer.UnpackMemoryStream");
    }

    static void AddPacker(string type, string packer, string unpacker)
    {
        packerInfos[type] = packer;
        unpackInfos[type] = unpacker;
    }

    public static string GetPackerName(string typeName)
    {
        if (packerInfos.TryGetValue(typeName, out string? value))
        {
            return value;
        }

        return "Common.Packer.PackProperty";
//        return "Invalid Type:" + typeName;
    }

    public static string GetUnpackerName(string typeName)
    {
        if (unpackInfos.TryGetValue(typeName, out string? value))
        {
            return value;
        }

        return $"Common.Packer.UnpackProperty<{typeName}>";
//        return "Invalid Type:" + typeName;
    }
}