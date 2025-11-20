
namespace Common;

public class ClassInfo
{
    public string name;
    //public Dictionary<string, PropertyInfo> propertyInfos = new Dictionary<string, PropertyInfo>();
    //public Dictionary<int, PropertyInfo> indexProperties = new Dictionary<int, PropertyInfo>();

    public List<PropertyInfomation> sortedInfos = new List<PropertyInfomation>();

    public Dictionary<string, PropertyInfomation> propertyInfos = new Dictionary<string, PropertyInfomation>();
    public Dictionary<int, PropertyInfomation> indexProperties = new Dictionary<int, PropertyInfomation>();

    public Dictionary<string, MethodInfomation> methodInfos = new Dictionary<string, MethodInfomation>();
    public Dictionary<int, MethodInfomation> indexMethods = new Dictionary<int, MethodInfomation>();

    public static List<PropertyInfomation> builtinInfos = CreateBuiltinPropertyInfo();
    public static PropertyInfomation methodIndexInfo = new PropertyInfomation(-1, "_index");

    public ClassInfo(string name)
    {
        this.name = name;
    }

    // 属性数量, 暂定64个, 后续出异常了再增加数量
    static List<PropertyInfomation> CreateBuiltinPropertyInfo(int count = 64)
    {
        var infos = new List<PropertyInfomation>();

        for (int i = 0; i < count; ++i)
        {
            infos.Add(new PropertyInfomation(i, i.ToString()));
        }

        return infos;
    }

    //public void AddPropertyInfo(PropertyInfo info)
    //{
    //    propertyInfos.Add(info.name, info);
    //    indexProperties.Add(info.index, info);
    //}
    public void AddPropertyInfo(PropertyInfomation info)
    {
        propertyInfos.Add(info.name, info);
        indexProperties.Add(info.index, info);
    }

    public void AddMethodInfo(MethodInfomation info)
    {
        methodInfos.Add(info.name, info);
        indexMethods.Add(info.index, info);
    }

    public void Build()
    {
        sortedInfos = propertyInfos.Values.ToList();
        sortedInfos.Sort((x, y) => string.Compare(x.name, y.name));
    }

    //public PropertyInfo GetPropertyInfo(string name)
    //{
    //    return propertyInfos[name];
    //}

    public PropertyInfomation GetPropertyInfo(string name)
    {
        return propertyInfos[name];
    }

    //public PropertyInfo? GetPropertyInfo(int index)
    //{
    //    if (indexProperties.TryGetValue(index, out var propertyInfo))
    //    {
    //        return propertyInfo;
    //    }

    //    return null;
    //}

    public MethodInfomation GetMethodInfo(string name)
    {
        return methodInfos[name];
    }

    public MethodInfomation GetMethodInfo(int index)
    {
        return indexMethods[index];
    }
}

public class MethodInfomation
{
    public int index;
    public string name;
    public Action<IDataStreamReader, object> caller;

    public PropertyInfomation indexProperty;

    public MethodInfomation(int index, string name, Action<IDataStreamReader, object> caller)
    {
        this.index = index;
        this.name = name;
        this.caller = caller;
        this.indexProperty = new PropertyInfomation(-1, "_index");
    }
}

public class PropertyInfomation
{
    public int index;
    public string name;
    public string notifierName;
    public PropertyFlag flag;

    public PropertyInfomation(int index, string name, PropertyFlag flag = PropertyFlag.All)
    {
        this.index = index;
        this.name = name;

        this.notifierName = $"_{this.name}_Changed";
        this.flag = flag;
    }
}