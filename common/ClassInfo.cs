
namespace Common;

public class ClassInfo
{
    public string name;
    public Dictionary<string, PropertyInfo> propertyInfos = new Dictionary<string, PropertyInfo>();
    public Dictionary<int, PropertyInfo> indexProperties = new Dictionary<int, PropertyInfo>();

    public List<PropertyInfo> sortedInfos = new List<PropertyInfo>();

    public ClassInfo(string name)
    {
        this.name = name;
    }

    public void AddPropertyInfo(PropertyInfo info)
    {
        propertyInfos.Add(info.name, info);
        indexProperties.Add(info.index, info);
    }

    public void Build()
    {
        sortedInfos = propertyInfos.Values.ToList();
        sortedInfos.Sort((x, y) => string.Compare(x.name, y.name));
    }

    public PropertyInfo GetPropertyInfo(string name)
    {
        return propertyInfos[name];
    }

    public PropertyInfo? GetPropertyInfo(int index)
    {
        if (indexProperties.TryGetValue(index, out var propertyInfo))
        {
            return propertyInfo;
        }

        return null;
    }
}