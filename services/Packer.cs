
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Services
{
    using PackerInfoDict = Dictionary<Type, PackerInfo>;

    public class PackerInfo
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
            //if (packers == null) 
            //{
            //    packers = new PackerInfoDict();
            //    DefinePackers();
            //}
            PreparePackers();
            return packers![type];
        }

        static void PreparePackers()
        {
            if (packers != null)
            {
                return;
            }

            packers = new PackerInfoDict();
            DefinePackers();
        }

        static void DefinePackers()
        {
            DefinePacker(typeof(int), "PackInt", "UnpackInt");
            DefinePacker(typeof(string), "PackString", "UnpackString");
            DefinePacker(typeof(bool), "PackBool", "UnpackBool");
            DefinePacker(typeof(long), "PackLong", "UnpackLong");
            DefinePacker(typeof(MemoryStream), "PackMemoryStream", "UnpackMemoryStream");
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
            AddPacker(type, packMethod, unpackMethod);
            //PackerInfo info = new PackerInfo(packMethod!, unpackMethod!);
            //packers![type] = info;
        }

        public static void AddPacker(Type type, MethodInfo? packer, MethodInfo? unpacker)
        {
            PreparePackers();

            PackerInfo info = new PackerInfo(packer!, unpacker!);
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

        public static void PackLong(MemoryStream stream, long value)
        {
            byte[] data = BitConverter.GetBytes(value);
            stream.Write(data);
        }

        public static long UnpackLong(BinaryReader reader)
        {
            return reader.ReadInt64();
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

        // stream 只能作为最后一个参数
        public static void PackMemoryStream(MemoryStream stream, MemoryStream value)
        {
            stream.Write(value.ToArray());
        }

        public static MemoryStream UnpackMemoryStream(BinaryReader reader)
        {
            long size = reader.BaseStream.Length - reader.BaseStream.Position;
            MemoryStream stream = new MemoryStream(reader.ReadBytes((int)size));
            return stream;
        }
    }
}