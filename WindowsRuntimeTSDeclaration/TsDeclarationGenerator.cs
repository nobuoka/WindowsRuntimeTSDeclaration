using System;
using System.IO;
using System.Collections.Generic;

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
                procNamespaceDeclarationSyntax((NamespaceDeclarationSyntax)syntax, indentCount);
            }
            else if (syntax is EnumDeclarationSyntax)
            {
                ProcEnumDeclarationSyntax(syntax as EnumDeclarationSyntax, indentCount);
            }
            else if (syntax is EnumMemberDeclarationSyntax)
            {
                ProcEnumMemberDeclarationSyntax(syntax as EnumMemberDeclarationSyntax, indentCount);
            }
            else if (syntax is ClassDeclarationSyntax)
            {
                procClassDeclarationSyntax((ClassDeclarationSyntax)syntax, indentCount);
            }
            else if (syntax is ConstructorDeclarationSyntax)
            {
                procConstructorDeclarationSyntax(syntax as ConstructorDeclarationSyntax, indentCount);
            }
            else if (syntax is MethodDeclarationSyntax)
            {
                procMethodDeclarationSyntax(syntax as MethodDeclarationSyntax, indentCount);
            }
            else if (syntax is PropertyDeclarationSyntax)
            {
                procPropertyDeclarationSyntax(syntax as PropertyDeclarationSyntax, indentCount);
            }
            else
            {
                throw new Exception("Unknown type : " + syntax.GetType());
            }
        }

        private void procNamespaceDeclarationSyntax(NamespaceDeclarationSyntax syntax, int indentCount)
        {
            writer.Write("module ");
            writeType(syntax.Name);
            writer.Write(" {\n");
            foreach (var memberDecl in syntax.Members)
            {
                procMemberDeclarationSyntax(memberDecl, writer, indentCount + 1);
            }
            writer.Write("}\n");
        }

        private void ProcEnumDeclarationSyntax(EnumDeclarationSyntax syntax, int indentCount)
        {
            writeIndents(indentCount);
            writer.Write("enum ");
            writer.Write(syntax.Identifier);
            writer.Write(" {");

            bool isFirst = true;
            foreach (var memberDecl in syntax.Members)
            {
                if (isFirst)
                {
                    isFirst = false;
                    writer.Write("\n");
                }
                else
                {
                    writer.Write(",\n");
                }
                ProcEnumMemberDeclarationSyntax(memberDecl, indentCount + 1);
            }
            writer.Write("\n");

            writeIndents(indentCount);
            writer.Write("}\n");
        }

        private void ProcEnumMemberDeclarationSyntax(EnumMemberDeclarationSyntax syntax, int indentCount)
        {
            writeIndents(indentCount);
            writeMethodAndPropertyIdentifier(syntax.Identifier);
        }

        private void procClassDeclarationSyntax(ClassDeclarationSyntax syntax, int indentCount)
        {
            writeIndents(indentCount);
            writer.Write("class " + syntax.Identifier);
            if (syntax.BaseList != null)
            {
                var baseClasses = new List<BaseTypeSyntax>();
                var baseInterfaces = new List<BaseTypeSyntax>();
                foreach (var t in syntax.BaseList.Types)
                {
                    if (checkBaseTypeIsInterface(t))
                        baseInterfaces.Add(t);
                    else
                        baseClasses.Add(t);
                }
                if (baseClasses.Count == 1)
                {
                    writer.Write(" extends ");
                    writeType(baseClasses[0].Type);
                }
                else if (baseClasses.Count > 1)
                {
                    throw new Exception("Number of base classes is greater than one.");
                }
                if (baseInterfaces.Count > 0)
                {
                    writer.Write(" implements ");
                    bool isFirst = true;
                    foreach (var b in baseInterfaces)
                    {
                        if (isFirst)
                            isFirst = false;
                        else
                            writer.Write(", ");
                        writeType(b.Type);
                    }
                }
            }
            writer.Write(" {\n");

            foreach (var memberDecl in syntax.Members)
            {
                procMemberDeclarationSyntax(memberDecl, writer, indentCount + 1);
            }

            writeIndents(indentCount);
            writer.Write("}\n");
        }

        private void procConstructorDeclarationSyntax(ConstructorDeclarationSyntax syntax, int indentCount)
        {
            writeIndents(indentCount);
            writer.Write("constructor(");
            if (syntax.ParameterList != null)
                procParameterList(syntax.ParameterList);
            writer.Write(");\n");
        }

        private void procMethodDeclarationSyntax(MethodDeclarationSyntax syntax, int indentCount)
        {
            ParameterSyntax outParam = RemoveOutParameter(syntax, out syntax);

            writeIndents(indentCount);
            writeMethodAndPropertyIdentifier(syntax.Identifier);
            writer.Write("(");
            if (syntax.ParameterList != null)
                procParameterList(syntax.ParameterList);
            writer.Write("): ");
            if (outParam != null)
            {
                if ("bool".Equals(syntax.ReturnType.ToString()))
                {
                    writer.Write("{ ");
                    writer.Write(outParam.Identifier);
                    writer.Write(": ");
                    writeType(outParam.Type);
                    writer.Write(", suceeded: boolean; }");
                }
                else
                {
                    throw new Exception("Unknown return pattern");
                }
            } else
            {
                writeType(syntax.ReturnType);
            }
            writer.Write(";\n");
        }

        private void procPropertyDeclarationSyntax(PropertyDeclarationSyntax syntax, int indentCount)
        {
            writeIndents(indentCount);
            writeMethodAndPropertyIdentifier(syntax.Identifier);
            writer.Write(": ");
            writeType(syntax.Type);
            writer.Write(";\n");
        }

        private ParameterSyntax RemoveOutParameter(MethodDeclarationSyntax syntax, out MethodDeclarationSyntax convertedSyntax)
        {
            var list = syntax.ParameterList;
            ParameterSyntax outParam = null;
            foreach (var parameter in list.Parameters)
            {
                if (parameter.Modifiers.Count > 0)
                {
                    foreach (var mod in parameter.Modifiers)
                    {
                        if (mod.ToString().Equals("out"))
                        {
                            outParam = parameter;
                        }
                    }
                }
            }
            convertedSyntax = syntax.WithParameterList(list.WithParameters(list.Parameters.Remove(outParam)));
            return outParam;
        }

        private void procParameterList(ParameterListSyntax syntax)
        {
            bool isFirst = true;
            foreach (var parameter in syntax.Parameters)
            {
                if (isFirst)
                    isFirst = false;
                else
                    writer.Write(", ");
                if (parameter.Modifiers.Count > 0)
                {
                    writer.Write("/*");
                    foreach (var mod in parameter.Modifiers)
                    {
                        writer.Write("[" + mod + "]");
                    }
                    writer.Write("*/ ");
                }
                writer.Write(parameter.Identifier + ": ");
                writeType(parameter.Type);
            }
        }

        private void writeMethodAndPropertyIdentifier(SyntaxToken token)
        {
            var identifier = token.ToString();
            writer.Write(Char.ToLowerInvariant(identifier[0]) + identifier.Substring(1));
        }

        private void writeType(TypeSyntax syntax)
        {
            if (syntax is PredefinedTypeSyntax)
            {
                PredefinedTypeSyntax p = syntax as PredefinedTypeSyntax;
                JavaScriptTypeNameAndUniqunessPair jsType;
                if (PredefinedTypeToJavaScriptTypeConvertingMap.getInstance().TryGetValue(p.Keyword.ToString(), out jsType))
                {
                    writer.Write(jsType.Name);
                    if (!jsType.Uniquness)
                        writer.Write(" /* " + p.Keyword + " */");
                }
                else
                {
                    throw new Exception("Unknown predefined type : " + p.Keyword);
                }
            }
            else if (syntax is IdentifierNameSyntax)
            {
                IdentifierNameSyntax i = (IdentifierNameSyntax)syntax;
                writer.Write(i.Identifier);
            }
            else if (syntax is GenericNameSyntax)
            {
                GenericNameSyntax g = syntax as GenericNameSyntax;
                writer.Write(g.Identifier);
                writer.Write("<");
                bool isFirst = true;
                foreach (var arg in g.TypeArgumentList.Arguments)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        writer.Write(", ");
                    writeType(arg);
                }
                writer.Write(">");
            }
            else if (syntax is QualifiedNameSyntax)
            {
                QualifiedNameSyntax q = (QualifiedNameSyntax)syntax;
                writeType(q.Left);
                writer.Write(".");
                writeType(q.Right);
            }
            else
            {
                throw new Exception("Unknown type : " + syntax.GetType());
            }
        }

        private void writeIndents(int indentCount)
        {
            for (int i = 0; i < indentCount; i++) writer.Write("    ");
        }

        private bool checkBaseTypeIsInterface(BaseTypeSyntax syntax)
        {
            if (syntax is SimpleBaseTypeSyntax)
            {
                var s = syntax as SimpleBaseTypeSyntax;
                return checkTypeIsInterface(s.Type);
            }
            else
            {
                throw new Exception("Unknown type : " + syntax.GetType());
            }
        }

        private bool checkTypeIsInterface(TypeSyntax syntax)
        {
            if (syntax is IdentifierNameSyntax)
            {
                var i = syntax as IdentifierNameSyntax;
                return checkIdentifierIsInterface(i.Identifier);
            }
            else if (syntax is QualifiedNameSyntax)
            {
                var q = syntax as QualifiedNameSyntax;
                return checkTypeIsInterface(q.Right);
            }
            else if (syntax is GenericNameSyntax)
            {
                var g = syntax as GenericNameSyntax;
                return checkIdentifierIsInterface(g.Identifier);
            }
            else
            {
                throw new Exception("Unknown type : " + syntax.GetType());
            }
        }

        private bool checkIdentifierIsInterface(SyntaxToken token)
        {
            return token.ToString().StartsWith("I");
        }
    }
}
