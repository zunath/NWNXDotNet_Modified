using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Threading;

namespace NwnxAssembly
{
    public static class ExecuteReflection
    {
        private const string ClassesFolder = "CSharpClasses";
        public static object RunAssembly(string sAssembly, string sNameSpace, string sClass, string sMethod, string[] Arguments)
        {
            Assembly a;
            string ThePath = "";
            try
            {

                string sPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                if (!File.Exists(sAssembly))
                {
                    if (!File.Exists(sPath + "\\" + ClassesFolder + "\\" + sAssembly))
                    {
                        return @"Class/Path Not Found";
                    }
                    ThePath = sPath + "\\" + ClassesFolder + "\\" + sAssembly;
                }
                else
                {
                    ThePath = sAssembly;
                }
                a = Assembly.LoadFile(ThePath);
                var type = a.GetType(sNameSpace + "." + sClass);
                MethodInfo mi = type.GetMethod(sMethod,
                  BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                return mi.Invoke(null, Arguments);
            }
            catch (Exception e)
            {
                return "Exception:" + e.ToString();
            }
            finally
            {
                a = null;
            }


        }



    }
}
