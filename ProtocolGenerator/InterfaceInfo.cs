
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Reflection;

namespace ProtocolGenerator;

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
    public int rpcId = 0;

    public List<ParameterInfo> parameters = new List<ParameterInfo>();

    public MethodInfo(IMethodSymbol method)
    {
        name = method.Name;

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
            this.rpcId = rpcId;
        }
    }

}

public class InterfaceInfo
{
    public string name = "";
    public string senderName = "";
    public string dispatcherName = "";
    public string containingNamespace = "";
    public string fullName = "";
    public List<MethodInfo> methods = new List<MethodInfo>();

    public static InterfaceInfo Build(INamedTypeSymbol inter)
    {
        InterfaceInfo info = new InterfaceInfo();

        // info.name = node.Identifier.Text;
        info.name = inter.Name;

        info.senderName = $"{info.name}_Sender";
        info.dispatcherName = $"{info.name}_Dispatcher";

        // add members
        foreach (var member in inter.GetMembers())
        {
            info.AddMember(member);
        }

        info.GenerateRpcId();

        return info;
    }

    void AddMember(ISymbol member)
    {
        var method = member as IMethodSymbol;
        if (method == null)
        {
            return;
        }

        MethodInfo info = new MethodInfo(method);

        methods.Add(info);
    }

    void GenerateRpcId()
    {
        Dictionary<string, int> rpcIdDict = new Dictionary<string, int>();
        HashSet<int> idSet = new HashSet<int>();

        foreach (var method in methods)
        {
            rpcIdDict.Add(method.name, method.rpcId);
            idSet.Add(method.rpcId);
        }

        var nameList = rpcIdDict.Keys.ToList();
        nameList.Sort();

        //int rpcIdStart = 10;
        //for (int index = 0; index < nameList.Count; ++index)
        //{
        //    int oldId = rpcIdDict[nameList[index]];
        //    int rpcId = rpcIdStart + index;

        //    if (oldId != 0)
        //    {
        //        rpcId = oldId;
        //    }
        //    rpcIdDict[nameList[index]] = rpcId;
        //}
        int nextRpcId = 10;
        for (int index = 0; index < nameList.Count; ++index)
        {
            int rpcId = rpcIdDict[nameList[index]];
            if (rpcId == 0)
            { // get nextRpcId
                while (idSet.Contains(nextRpcId))
                {
                    ++nextRpcId;
                }
                rpcId = nextRpcId;
                ++nextRpcId;
            }
            rpcIdDict[nameList[index]] = rpcId;
        }

        // set the rpcid
        foreach (var method in methods)
        {
            method.rpcId = rpcIdDict[method.name];
        }
    }
}