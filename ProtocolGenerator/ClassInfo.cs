
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

    public PropertyInfo(IPropertySymbol symbol)
    {

        this.typeName = symbol.Type.Name;
        string name = symbol.Name;

        if (!name.StartsWith("_"))
        {
            throw new InvalidDataException($"Invalid name: {this.name}");
        }

        // skip the "_"
        this.name = name.Substring(1);

        foreach (AttributeData attribute in symbol.GetAttributes())
        {
            if (attribute.AttributeClass!.Name == "Property")
            {
                GetAttributeInfo(attribute);
            }
        }
    }

    void GetAttributeInfo(AttributeData attribute)
    {
        foreach (var namedArgument in attribute.NamedArguments)
        {
            string name = namedArgument.Key;
            TypedConstant value = namedArgument.Value;
            // string valueString = value.ToCSharpString();
            string valueString = value.ToString();
            if (name == "index")
            {
                this.index = (int)value.Value!;
            }
            else if (name == "flag")
            {
                this.flag = value.ToString();
            }
        }
    }
}

class ClassInfo
{
    Dictionary<string, PropertyInfo> properties = new Dictionary<string, PropertyInfo>();

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

    void TryAddProperty(ISymbol symbol)
    {
        IPropertySymbol? property = symbol as IPropertySymbol;
        if (property == null)
        {
            return;
        }

        foreach (var attribute in property.GetAttributes())
        {
            if (attribute.AttributeClass!.Name == "Property")
            {
                AddProperty(property);
            }
        }
    }

    void AddProperty(IPropertySymbol property)
    {
        string name = property.Name;
        PropertyInfo info = new PropertyInfo(property);

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