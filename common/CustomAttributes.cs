

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

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
public class PropertyAttribute : Attribute
{
    public PropertyFlag flag;
    public int index;
    public PropertyAttribute(PropertyFlag flag = PropertyFlag.ServerOnly, int index = 0)
    {
        this.flag = flag;
        this.index = index;
    }
}

public enum PropertyFlag
{
    None = 0,
    OwnerClient = (2 << 1),
    OtherClient = (2 << 2),
    Client = OwnerClient | OtherClient,
    Save = (2 << 3),
    ServerOnly = (2 << 4),

    All = Client | Save | ServerOnly
}
