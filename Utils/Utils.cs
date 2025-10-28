
namespace Utils;

public class Utils
{
    public static Uuid GenUuid()
    {
        // 64 位, 高32为为当前秒数, 低32位是一个随机数
        long timeStamp = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
        long rand = (int)Random.Shared.Next();
        return new Uuid((timeStamp << 32) | rand);
    }
}

public readonly record struct Uuid(long value = 0);