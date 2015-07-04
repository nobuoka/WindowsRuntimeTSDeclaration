using System;
using System.IO;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WindowsRuntimeTSDeclaration
{
    public class TsDeclarationGenerator
    {
        private SyntaxTree tree;
        private TextWriter writer;

        public TsDeclarationGenerator(SyntaxTree tree, TextWriter writer)
        {
            this.tree = tree;
            this.writer = writer;
        }

        public void Generate()
        {
            var root = (CompilationUnitSyntax)tree.GetRoot();
            foreach (var memberDecl in root.Members)
            {
                procMemberDeclarationSyntax(memberDecl, Console.Out, 0);
            }
        }

        private void procMemberDeclarationSyntax(MemberDeclarationSyntax syntax, TextWriter writer, int indentCount)
        {
            if (syntax is NamespaceDeclarationSyntax)
            {
                procNamespaceDeclarationSyntax((NamespaceDeclarationSyntax)syntax);
            }
        }

        private void procNamespaceDeclarationSyntax(NamespaceDeclarationSyntax syntax)
        {
            writer.Write("module " + syntax.Name + " {\n");
            writer.Write("}\n");
        }
    }
}
