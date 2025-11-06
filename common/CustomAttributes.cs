

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

public enum AttributeFlag
{
    None = 0,
    OwnerClient = 1,
    OtherClient = 2,
    Client = OwnerClient | OtherClient
}