

namespace Common;

[AttributeUsage(AttributeTargets.Interface)]
public class Protocol : Attribute
{
}

[AttributeUsage(AttributeTargets.Method)]
public class Rpc : Attribute
{
    public int rpcId { get; set; }

    public Rpc(int rpcId = 0)
    {
        this.rpcId = rpcId;
    }
}