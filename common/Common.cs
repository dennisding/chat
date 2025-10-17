
using Services;
using System.Reflection;

namespace Common
{
    public class Initer
    {
        public static void Init()
        {
            //MethodInfo packer = typeof(Initer).GetMethod("PackActorId")!;
            //MethodInfo unpacker = typeof(Initer).GetMethod("UnpackActorId")!;
            //Services.PackerInfo.AddPacker(typeof(ActorId), packer, unpacker);
        }


        //public static void PackActorId(MemoryStream stream, ActorId aid)
        //{
        //    byte[] rawData = BitConverter.GetBytes(aid.value);
        //    stream.Write(rawData);
        //}

        //public static ActorId UnpackActorId(BinaryReader reader)
        //{
        //    long value = reader.ReadInt64();
        //    return new ActorId(value);
        //}
    }
}