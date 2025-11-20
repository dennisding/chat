
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

namespace ProtocolGenerator;

class PropertyInfo
{
    public int index = 0;
    public string accessor = "";
    public string typeName = "";
    public string originName = "";
    public string name = "";
    public string flag = "";
    public string packerName = "";
    public string unpackerName = "";
    public string infoName = "";

    public PropertyInfo(IFieldSymbol field)
    {

        this.typeName = field.Type.ToDisplayString();
        string name = field.Name;

        if (!name.StartsWith("_"))
        {
            throw new InvalidDataException($"Invalid name: {this.name}");
        }

        this.originName = name;
        // skip the "_"
        this.name = name.Substring(1);
        this.packerName = $"_Pack_{this.name}";
        this.unpackerName = $"_Unpack_{this.name}";
        this.infoName = $"_{this.name}_Info";

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
        }
    }
}

public class ParameterInfo
{
    public string name = "";
    public string typeName = "";

    public ParameterInfo(string name, string typeName)
    {
        this.name = name;
        this.typeName = typeName;
    }
}

// name, argNames, argTypeName
public class MethodInfo
{
    public string name = "";
    public int index = 0;

    public List<ParameterInfo> parameters = new List<ParameterInfo>();
    public string parameterTypeNameList = "";
    public string parameterNameList = "";

    public string infoName = "";
    public string packerName = "";
    public string unpackerName = "";

    public MethodInfo(IMethodSymbol method)
    {
        this.name = method.Name;
        this.infoName = $"_{name}_Info";
        this.packerName = $"_Pack_{name}";
        this.unpackerName = $"_Unpack_{name}";

        BuildParameters(method);
        ParseAttributes(method);
    }

    void BuildParameters(IMethodSymbol method)
    {
        foreach (var parameter in method.Parameters)
        {
            string name = parameter.Name;
            string typeName = parameter.Type.ToDisplayString();

            ParameterInfo info = new ParameterInfo(name, typeName);
            parameters.Add(info);

            if (parameterTypeNameList != "")
            {
                parameterTypeNameList = parameterTypeNameList + ", ";
                parameterNameList = parameterNameList + ", ";
            }
            parameterTypeNameList = parameterTypeNameList + $"{typeName} {name}";
            parameterNameList = parameterNameList + name;
        }
    }

    void ParseAttributes(IMethodSymbol method)
    {
        foreach (var attr in method.GetAttributes())
        {
            if (attr.AttributeClass!.Name != "Rpc")
            {
                continue;
            }

            int rpcId = 0;
            foreach (var argument in attr.ConstructorArguments)
            {
                if (argument.Value is int id)
                {
                    rpcId = id;
                }
            }
            this.index = rpcId;
        }
    }

}

class ClassInfo
{
    public Dictionary<string, PropertyInfo> properties = new Dictionary<string, PropertyInfo>();
    public Dictionary<string, MethodInfo> methods = new Dictionary<string, MethodInfo>();

    public string containningNamespace = "";
    public string name = "";
    public string packerName = "";
    public string dispatcherName = "";
    public string classInfoName = "";
    public string partialName = "";

    public int maxPropertyIndex = 0;
    public int maxMethodIndex = 0;

    public bool isInterface = false;

    public ClassInfo(string name, string containningNamespace, bool isInterface)
    {
        this.name = name;
        this.containningNamespace = containningNamespace;
        this.isInterface = isInterface;

        this.packerName = $"{this.name}_Packer";
        this.dispatcherName = $"{this.name}_Dispatcher";
        this.classInfoName = $"{this.name}_ClassInfo";
        this.partialName = $"{this.name}_Partial";
    }

    public static ClassInfo Build(INamedTypeSymbol symbol, bool isInterface = false)
    {
        ClassInfo info = new ClassInfo(symbol.Name, symbol.ContainingNamespace.Name, isInterface);

        if (isInterface)
        {
            info.BuildInterfaceInfo(symbol);
        }
        else
        {
            info.BuildPropertyInfo(symbol);
        }

        return info;
    }

    public void BuildInterfaceInfo(INamedTypeSymbol symbol)
    {
        foreach (var member in symbol.GetMembers())
        {
            AddMember(member);
        }

        GenerateRpcIndex();
    }

    public void BuildPropertyInfo(INamedTypeSymbol symbol)
    {
        foreach (var member in symbol.GetMembers())
        {
            TryAddProperty(member);
        }

        BuildPropertyIndex();
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
    }

    void AddProperty(IFieldSymbol field)
    {
        PropertyInfo info = new PropertyInfo(field);

        properties.Add(info.name, info);
    }

    void BuildPropertyIndex()
    {
        HashSet<int> ids = new HashSet<int>();

        foreach (var property in properties.Values)
        {
            if (property.index != 0)
            {
                ids.Add(property.index);
                maxPropertyIndex = Math.Max(property.index, maxPropertyIndex);
            }
        }

        var propertyNames = properties.Keys.ToList();
        propertyNames.Sort();

        int nextIndex = 10;
        foreach (var name in propertyNames)
        {
            var info = properties[name];
            if (info.index == 0)
            {
                while (ids.Contains(nextIndex)) { ++nextIndex; }

                // allocate a new index
                info.index = nextIndex;
                maxPropertyIndex = Math.Max(nextIndex, maxPropertyIndex);
                ++nextIndex;
            }
        }
    }

    // build interface
    void AddMember(ISymbol member)
    {
        var method = member as IMethodSymbol;
        if (method == null)
        {
            return;
        }

        MethodInfo info = new MethodInfo(method);
        methods.Add(info.name, info);
    }

    void GenerateRpcIndex()
    {
        HashSet<int> ids = new HashSet<int>();

        foreach (var property in methods.Values)
        {
            if (property.index != 0)
            {
                ids.Add(property.index);
                maxMethodIndex = Math.Max(property.index, maxMethodIndex);
            }
        }

        var names = methods.Keys.ToList();
        names.Sort();

        int nextIndex = 10;
        foreach (var name in names)
        {
            var info = methods[name];
            if (info.index == 0)
            {
                while (ids.Contains(nextIndex)) { ++nextIndex; }

                // allocate a new index
                info.index = nextIndex;
                maxMethodIndex = Math.Max(nextIndex, maxMethodIndex);
                ++nextIndex;
            }
        }
    }
}