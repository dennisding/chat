
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ProtocolGenerator;

class PropertyInfo
{
    public int index = 0;
    public string accessor = "";
    public string typeName = "";
    public string name = "";
    public string flag = "";
    public string packerName = "";
    public string unpackerName = "";

    public PropertyInfo(IFieldSymbol field)
    {

        this.typeName = field.Type.ToDisplayString();
        string name = field.Name;

        if (!name.StartsWith("_"))
        {
            throw new InvalidDataException($"Invalid name: {this.name}");
        }

        // skip the "_"
        this.name = name.Substring(1);
        this.packerName = $"_Pack_{this.name}";
        this.unpackerName = $"_Unpack_{this.name}";

        foreach (AttributeData attribute in field.GetAttributes())
        {
            if (attribute.AttributeClass!.Name == "PropertyAttribute")
            {
                GetAttributeInfo(attribute);
            }
        }
    }

    void GetAttributeInfo(AttributeData attribute)
    {
        List<TypedConstant> arguments = attribute.ConstructorArguments.ToList();
        // 第一个参数是flag
        if (arguments.Count > 0)
        {
            TypedConstant flag = arguments[0];
            this.flag = flag.ToCSharpString();
        }
        
        if (arguments.Count > 1)
        {
            // 第二个参数是index
            TypedConstant index = arguments[1];
            this.index = int.Parse(index.ToCSharpString());
//            this.typeName = index.ToCSharpString();
        }
    }
}

class ClassInfo
{
    public Dictionary<string, PropertyInfo> properties = new Dictionary<string, PropertyInfo>();

    public string containningNamespace = "";
    public string name = "";

    public ClassInfo()
    {
    }

    public static ClassInfo Build(INamedTypeSymbol symbol)
    {
        ClassInfo info = new ClassInfo();

        info.name = symbol.Name;
        info.containningNamespace = symbol.ContainingNamespace.Name;

        foreach (var member in symbol.GetMembers())
        {
            info.TryAddProperty(member);
        }

        info.BuildPropertyIndex();

        return info;
    }

    public IEnumerable<PropertyInfo> EnumeratePropertyInfo()
    {
        foreach (PropertyInfo property in properties.Values)
        {
            yield return property;
        }
    }

    void TryAddProperty(ISymbol symbol)
    {
        IFieldSymbol? field = symbol as IFieldSymbol;
        if (field == null)
        {
            return;
        }

        foreach (var attribute in field.GetAttributes())
        {
            if (attribute.AttributeClass!.Name == "PropertyAttribute")
            {
                AddProperty(field);
            }
        }

        //IPropertySymbol? property = symbol as IPropertySymbol;
        //if (property == null)
        //{
        //    return;
        //}

        //foreach (var attribute in property.GetAttributes())
        //{
        //    if (attribute.AttributeClass!.Name == "PropertyAttribute")
        //    {
        //        AddProperty(property);
        //    }
        //}
    }

    void AddProperty(IFieldSymbol field)
    {
        string name = field.Name;
        PropertyInfo info = new PropertyInfo(field);

        properties.Add(name, info);
    }

    void BuildPropertyIndex()
    {
        var propertyNames = properties.Keys.ToList();
        propertyNames.Sort();

        int nextIndex = 10;
        foreach (var name in propertyNames)
        {
            var info = properties[name];
            if (info.index == 0)
            {
                // allocate a new index
                info.index = nextIndex;
                ++nextIndex;
            }
        }
    }
}