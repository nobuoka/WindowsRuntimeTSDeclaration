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

            generated = generateDeclaration("class HelloWorld : ParentWorld, IParentInterface {}");
            Assert.AreEqual("class HelloWorld extends ParentWorld implements IParentInterface {\n}\n", generated);

            generated = generateDeclaration("class HelloWorld : IParent1, IParent2, IParent3 {}");
            Assert.AreEqual("class HelloWorld implements IParent1, IParent2, IParent3 {\n}\n", generated);
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
