
using System.Reflection;

namespace Common;

public interface IPropertyOwner
{
    //    void OnPropertyChanged(PropertyInfo info);
    void OnPropertyChanged(PropertyInfomation info);
}

public interface IProperty
{
    ClassInfo GetClassInfo();

    // 以下接口可以直接用默认实现
    void SetOwner(IPropertyOwner owner) { }
//    void OnPropertyChanged(PropertyInfo info)
    void OnPropertyChanged(PropertyInfomation info)
    {
        MethodInfo? notifier = this.GetType().GetMethod(info.notifierName);
        notifier?.Invoke(this, null);
    }

    //void PackTo(MemoryStream stream)
    //{
    //    //foreach (var property in GetClassInfo().sortedInfos)
    //    //{
    //    //    property.packer(this, stream);
    //    //}
    //}
    void PackTo(IDataStreamWriter writer, PropertyInfomation info)
    {
    }

    void UnpackFrom(IDataStreamReader reader, PropertyInfomation info)
    {
    }

    //void UnpackFrom(BinaryReader reader)
    //{
    //    //foreach (var property in GetClassInfo().sortedInfos)
    //    //{
    //    //    property.unpacker(this, reader);
    //    //}
    //}

    void UnpackProperty(int index, IDataStreamReader reader)
    {
        //PropertyInfo? info = GetClassInfo().GetPropertyInfo(index);
        //info?.unpacker(this, reader);
    }
}

//public class PropertyInfo
//{
//    public int index;
//    public PropertyFlag flag = PropertyFlag.None;
//    public string name;
//    public string notifierName;
//    public Action<object, MemoryStream> packer;
//    public Action<object, BinaryReader> unpacker;

//    public PropertyInfo(int index, PropertyFlag flag, 
//        string name, 
//        Action<object, MemoryStream> packer,
//        Action<object, BinaryReader> unpacker)
//    {
//        this.index = index;
//        this.flag = flag;
//        this.name = name;
//        this.notifierName = $"_{name}_Changed";
//        this.packer = packer;
//        this.unpacker = unpacker;
//    }
//}

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

    // public void OnPropertyChanged(PropertyInfo info)
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
