﻿using System.IO;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WindowsRuntimeTSDeclaration.Test
{
    [TestClass]
    public class TsDeclarationGeneratorTest
    {
        [TestMethod]
        public void generateOnlyNamespaceWithSimpleNameDeclaration()
        {
            var generated = generateDeclaration("namespace HelloWorld {}");
            Assert.AreEqual("module HelloWorld {\n}\n", generated);
        }

        [TestMethod]
        public void generateOnlyNamespaceWithQualifiedNameDeclaration()
        {
            var generated = generateDeclaration("namespace HelloWorld.Qualified.Name {}");
            Assert.AreEqual("module HelloWorld.Qualified.Name {\n}\n", generated);
        }

        [TestMethod]
        public void generateNamespaceDeclarationWithMembers()
        {
            var generated = generateDeclaration("namespace HelloWorld.Qualified.Name { class Test {} }");
            Assert.AreEqual("module HelloWorld.Qualified.Name {\n    class Test {\n    }\n}\n", generated);
        }

        [TestMethod]
        public void generateOnlyEmptyClassDeclaration()
        {
            var generated = generateDeclaration("class HelloWorld {}");
            Assert.AreEqual("class HelloWorld {\n}\n", generated);
        }

        [TestMethod]
        public void generateClassDeclarationWithInheritance()
        {
            var generated = generateDeclaration("class HelloWorld : ParentWorld {}");
            Assert.AreEqual("class HelloWorld extends ParentWorld {\n}\n", generated);

            generated = generateDeclaration("class HelloWorld : Qualified.Type.ParentWorld {}");
            Assert.AreEqual("class HelloWorld extends Qualified.Type.ParentWorld {\n}\n", generated);

            generated = generateDeclaration("class HelloWorld : Qualified.Type.ParentWorld<string> {}");
            Assert.AreEqual("class HelloWorld extends Qualified.Type.ParentWorld<string> {\n}\n", generated);

            generated = generateDeclaration("class HelloWorld : ParentWorld, IParentInterface {}");
            Assert.AreEqual("class HelloWorld extends ParentWorld implements IParentInterface {\n}\n", generated);

            generated = generateDeclaration("class HelloWorld : IParent1, Qualified.Type.IParent2, Qualified.Type.IParent3<string> {}");
            Assert.AreEqual("class HelloWorld implements IParent1, Qualified.Type.IParent2, Qualified.Type.IParent3<string> {\n}\n", generated);
        }

        [TestMethod]
        public void generateConstructorDeclaration()
        {
            var generated = generateDeclaration("class Test { Test(); }");
            Assert.AreEqual("class Test {\n    constructor();\n}\n", generated);

            generated = generateDeclaration("class Test { Test(string name, Test2 test2); }");
            Assert.AreEqual("class Test {\n    constructor(name: string, test2: Test2);\n}\n", generated);
        }

        [TestMethod]
        public void generatePropertyDeclaration()
        {
            var generated = generateDeclaration("class Test { string name { get; } }");
            Assert.AreEqual("class Test {\n    name: string;\n}\n", generated);

            generated = generateDeclaration("class Test { Test2 name { get; } }");
            Assert.AreEqual("class Test {\n    name: Test2;\n}\n", generated);

            generated = generateDeclaration("class Test { Test2<Test3<uint, Test4>> name { get; } }");
            Assert.AreEqual("class Test {\n    name: Test2<Test3<number /* uint */, Test4>>;\n}\n", generated);

            generated = generateDeclaration("class Test { Qualified.Test2<Test3> name { get; } }");
            Assert.AreEqual("class Test {\n    name: Qualified.Test2<Test3>;\n}\n", generated);
        }

        [TestMethod]
        public void convertCSharpPredefinedTypeToJavaScriptType()
        {
            var generated = generateDeclaration("class Test { uint name { get; }; }");
            Assert.AreEqual("class Test {\n    name: number /* uint */;\n}\n", generated);

            generated = generateDeclaration("class Test { bool isOk { get; }; }");
            Assert.AreEqual("class Test {\n    isOk: boolean;\n}\n", generated);
        }

        private string generateDeclaration(string csStr)
        {
            var writer = new StringWriter();
            try
            {
                var tree = CSharpSyntaxTree.ParseText(csStr);
                var g = new TsDeclarationGenerator(tree, writer);
                g.Generate();
            }
            finally
            {
                writer.Close();
            }
            return writer.ToString();
        }
    }
}
