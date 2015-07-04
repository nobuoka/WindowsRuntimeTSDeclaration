using System.IO;
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
