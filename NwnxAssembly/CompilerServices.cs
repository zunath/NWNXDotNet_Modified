using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;
using System.CodeDom;
using System.Reflection;
using Microsoft.CSharp;
using System.IO;
using NwnxAssembly;

namespace NwnxAssembly
{
    public static class CompilerServices
    {
        public static object ExecuteCode(string codeSourceFile, string namespacename, string classname, string functionname, bool isstatic, params object[] args)
        {
            object returnval = "Null";
            try
            {
                IniReader clsIni = new IniReader(System.Windows.Forms.Application.StartupPath + System.IO.Path.DirectorySeparatorChar + "NWNX.ini");
                string sPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string ClassesFolder = clsIni.ReadString("DOTNET", "CSharpPath", "CSharpClasses");
                if (!System.IO.Directory.Exists(sPath + "\\" + ClassesFolder))
                    return "(error)CSharp Folder doesn't exist";

                if (!File.Exists(sPath + "\\" + ClassesFolder + "\\" + codeSourceFile))
                {
                    return @"Class/Path Not Found";
                }

                TextReader tReader = new StreamReader(sPath + "//" + ClassesFolder + "//" + codeSourceFile);
                string sAllText = tReader.ReadToEnd();
                tReader.Close();
                tReader = null;



                Assembly asm = BuildAssembly(sAllText);
                object instance = null;
                Type type = null;
                if (isstatic)
                {
                    type = asm.GetType(namespacename + "." + classname);
                }
                else
                {
                    instance = asm.CreateInstance(namespacename + "." + classname);
                    type = instance.GetType();
                }
                MethodInfo method = type.GetMethod(functionname);
                returnval = method.Invoke(instance, args);
                instance = null;
                return "Returned: "+returnval;
            }
            catch (Exception e)
            {
                return "Exception:" + e.ToString();
            }
        }





        private static Assembly BuildAssembly(string code)
        {
            //Microsoft.CSharp.CSharpCodeProvider provider = new CSharpCodeProvider();
            CodeDomProvider compiler = CodeDomProvider.CreateProvider("C#");
            //ICodeCompiler compiler = provider.CreateCompiler();
            CompilerParameters compilerparams = new CompilerParameters();
            compilerparams.GenerateExecutable = false;
            compilerparams.GenerateInMemory = true;
            Assembly[] refAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            string[] sSplit = { "," };
            foreach (Assembly a in refAssemblies)
            {
                compilerparams.ReferencedAssemblies.Add(a.Location);
            }
            CompilerResults results =
               compiler.CompileAssemblyFromSource(compilerparams, code);
            if (results.Errors.HasErrors)
            {
                StringBuilder errors = new StringBuilder("Compiler Errors :\r\n");
                foreach (CompilerError error in results.Errors)
                {
                    errors.AppendFormat("Line {0},{1}\t: {2}\n",
                           error.Line, error.Column, error.ErrorText);
                }
                throw new Exception(errors.ToString());
            }
            else
            {
                return results.CompiledAssembly;
            }
        }



    }
}