
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Services
{
    using PackerInfoDict = Dictionary<Type, PackerInfo>;

    class PackerInfo
    {
        static PackerInfoDict? packers;

        public MethodInfo packer;
        public MethodInfo unpacker;

        public PackerInfo(MethodInfo packer, MethodInfo unpacker)
        {
            this.packer = packer;
            this.unpacker = unpacker;
        }

        public static PackerInfo Get(Type type)
        {
            if (packers == null) 
            {
                packers = new PackerInfoDict();
                DefinePackers();
            }
            return packers[type];
        }

        static void DefinePackers()
        {
            DefinePacker(typeof(int), "PackInt", "UnpackInt");
            DefinePacker(typeof(string), "PackString", "UnpackString");
            DefinePacker(typeof(bool), "PackBool", "UnpackBool");
        }

        static void DefinePacker(Type type, string packer, string unpacker)
        {
            MethodInfo? packMethod = typeof(Packer).GetMethod(
                    packer,
                    new Type[] { typeof(MemoryStream), type }
                );

            MethodInfo? unpackMethod = typeof(Packer).GetMethod(
                    unpacker,
                    new Type[] { typeof(BinaryReader) }
                );

            PackerInfo info = new PackerInfo(packMethod!, unpackMethod!);
            packers![type] = info;
        }
    }

    public class Packer
    {
        public static void PackInt(MemoryStream stream, int value)
        {
            byte[] data = BitConverter.GetBytes(value);
            stream.Write(data);
        }

        public static int UnpackInt(BinaryReader reader)
        {
            return reader.ReadInt32();
        }

        public static void PackBool(MemoryStream stream, bool value)
        {
            byte[] data = BitConverter.GetBytes(value);
            stream.Write(data);
        }

        public static bool UnpackBool(BinaryReader reader)
        {
            return reader.ReadBoolean();
        }

        public static void PackString(MemoryStream stream, string value)
        {
            var strData = MemoryMarshal.AsBytes(value.AsSpan());
            byte[] data = BitConverter.GetBytes((int)strData.Length);

            stream.Write(data);
            stream.Write(strData);
        }

        public static string UnpackString(BinaryReader reader)
        {
            int len = reader.ReadInt32();
            byte[] data = reader.ReadBytes(len);

            return Encoding.Unicode.GetString(data);
        }
    }
}