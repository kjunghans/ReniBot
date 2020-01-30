using System.IO;
using System.Reflection;

namespace ReniBot.Repository
{
    public static class ScriptManager
    {
        private static string GetScript(string scriptName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string script;
            using (StreamReader textStreamReader =
                new StreamReader(assembly.GetManifestResourceStream("ReniBot.Repository." + scriptName)))
            {
                script = textStreamReader.ReadToEnd();
            }
            return script;
        }

        public static string CreateIndexNciNodes()
        {
            return GetScript("create_nci_Nodes.sql");
        }


    }
}
