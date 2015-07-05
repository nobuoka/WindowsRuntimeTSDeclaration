using System.Collections.Generic;

namespace WindowsRuntimeTSDeclaration
{
    public class JavaScriptTypeNameAndUniqunessPair
    {
        public string Name { get; }
        public bool Uniquness { get; }
        public JavaScriptTypeNameAndUniqunessPair(string name, bool uniquness)
        {
            Name = name;
            Uniquness = uniquness;
        }
    }

    public class PredefinedTypeToJavaScriptTypeConvertingMap : Dictionary<string, JavaScriptTypeNameAndUniqunessPair>
    {
        private static PredefinedTypeToJavaScriptTypeConvertingMap Instance;

        public static PredefinedTypeToJavaScriptTypeConvertingMap getInstance()
        {
            if (Instance != null)
            {
                return Instance;
            }

            Instance = new PredefinedTypeToJavaScriptTypeConvertingMap();

            // See: https://msdn.microsoft.com/en-us/library/br205768%28v=vs.85%29.aspx
            Instance.Add("bool", new JavaScriptTypeNameAndUniqunessPair("boolean", true));
            Instance.Add("byte", new JavaScriptTypeNameAndUniqunessPair("number", false));
            Instance.Add("char", new JavaScriptTypeNameAndUniqunessPair("string", false));
            Instance.Add("double", new JavaScriptTypeNameAndUniqunessPair("number", false));
            Instance.Add("short", new JavaScriptTypeNameAndUniqunessPair("number", false));
            Instance.Add("int", new JavaScriptTypeNameAndUniqunessPair("number", false));
            Instance.Add("long", new JavaScriptTypeNameAndUniqunessPair("number", false));
            Instance.Add("object", new JavaScriptTypeNameAndUniqunessPair("Object", true));
            Instance.Add("float", new JavaScriptTypeNameAndUniqunessPair("number", false));
            Instance.Add("string", new JavaScriptTypeNameAndUniqunessPair("string", true));
            Instance.Add("ushort", new JavaScriptTypeNameAndUniqunessPair("number", false));
            Instance.Add("uint", new JavaScriptTypeNameAndUniqunessPair("number", false));
            Instance.Add("ulong", new JavaScriptTypeNameAndUniqunessPair("number", false));
            Instance.Add("void", new JavaScriptTypeNameAndUniqunessPair("void", true));
            return Instance;
        }
    }
}
