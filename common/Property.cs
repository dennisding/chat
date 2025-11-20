
using System.Reflection;

namespace Common;

public interface IPropertyOwner
{
    void OnPropertyChanged(PropertyInfomation info);
}

public interface IProperty
{
    ClassInfo GetClassInfo();

    // 以下接口可以直接用默认实现
    void SetOwner(IPropertyOwner owner) { }

    void OnPropertyChanged(PropertyInfomation info)
    {
        MethodInfo? notifier = this.GetType().GetMethod(info.notifierName);
        notifier?.Invoke(this, null);
    }

    void PackTo(IDataStreamWriter writer, PropertyInfomation info)
    {
    }

    void UnpackFrom(IDataStreamReader reader, PropertyInfomation info)
    {
    }

    void UnpackProperty(int index, IDataStreamReader reader)
    {
    }
}

public class Property : IProperty
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

    public void OnPropertyChanged(PropertyInfomation info)
    {
        if (!notify)
        {
            return;
        }

        MethodInfo? notifier = this.GetType().GetMethod(info.notifierName);
        notifier?.Invoke(this, null);

        this.owner?.OnPropertyChanged(info);
    }

    public void SetNotify(bool notify)
    {
        this.notify = notify;
    }

    public virtual ClassInfo GetClassInfo()
    {
        return new ClassInfo("Null");
    }
}
