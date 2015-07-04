using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WindowsRuntimeTSDeclaration
{
    class Program
    {
        static void Main(string[] args)
        {
            var tree = CSharpSyntaxTree.ParseText("namespace HelloWorld {}");
            var root = (CompilationUnitSyntax)tree.GetRoot();
        }
    }
}
