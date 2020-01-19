using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
