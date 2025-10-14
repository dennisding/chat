

using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;

namespace ProtocolGenerator;

class Indent
{
    static Dictionary<int, string> idents = new Dictionary<int, string>();

    public string value = "";
    int ident = 0;

    public Indent(int ident = 0)
    {
        this.ident = ident;

        if (idents.TryGetValue(ident, out string? value))
        {
            this.value = value;
        } else
        {
            MakeIdentString(ident);
        }
    }

    void MakeIdentString(int ident)
    {
        string value = string.Concat(Enumerable.Repeat("    ", ident));
        idents[ident] = value;
        this.value = value;
    }

    public Indent Next()
    {
        return new Indent(ident + 1);
    }
}