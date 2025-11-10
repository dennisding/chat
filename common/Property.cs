
using System.Reflection;

namespace Common;

public interface IPropertyOwner
{
    void OnPropertyChanged(PropertyInfo info);
}

public class PropertyInfo
{
    public int index;
    public PropertyFlag flag = PropertyFlag.None;
    public string name;
    public string notifierName;
    public Action<object, MemoryStream> packer;
    public Action<object, BinaryReader> unpacker;

    public PropertyInfo(int index, PropertyFlag flag, 
        string name, 
        Action<object, MemoryStream> packer,
        Action<object, BinaryReader> unpacker)
    {
        this.index = index;
        this.flag = flag;
        this.name = name;
        this.notifierName = $"_{name}_Changed";
        this.packer = packer;
        this.unpacker = unpacker;
    }
}

public class Property
{
    IPropertyOwner? owner;
    bool notify = true;

    public Property()
    {
    }

    public void SetOwner(IPropertyOwner owner)
    {
        this.owner = owner;
    }

//    public void OnPropertyChanged(string notifierName, Action<MemoryStream> packer, Action notifier)
    public void OnPropertyChanged(PropertyInfo info)
    {
        if (!notify)
        {
            return;
        }

        MethodInfo? method = this.GetType().GetMethod(info.notifierName);
        method?.Invoke(this, null);

        this.owner?.OnPropertyChanged(info);
    }

    public void SetNotify(bool notify)
    {
        this.notify = notify;
    }

    public void PackTo(MemoryStream stream)
    {
    }

    public void UnpackFrom(BinaryReader reader)
    {
        SetNotify(false);

        SetNotify(true);
    }

    public virtual void UnpackProperty(BinaryReader reader)
    {
    }

}
