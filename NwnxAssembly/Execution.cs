using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace NwnxAssembly
{
    public class Execution
    {

        //Arguments must be delimitated via ¬ characters
        public string ExecuteClassCode(string sClassFile, string sMethodName, string sArgs)
        {
            string[] sSplitter = { "¬" };
            string[] Arguments = sArgs.Split(sSplitter, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                return CompilerServices.ExecuteCode(sClassFile + ".cs", "NwnxAssembly", sClassFile, sMethodName, true, Arguments).ToString();
            }
            catch (Exception e)
            {
                return "Exception:" + e.ToString();
            }
        }

        public string ExecuteAssembly(string sAssembly, string sNameSpace, string sClass, string sMethod, string sArgs)
        {
            string[] sSplitter = { "¬" };
            string[] Arguments = sArgs.Split(sSplitter, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                return ExecuteReflection.RunAssembly(sAssembly, sNameSpace, sClass, sMethod, Arguments).ToString();
            }
            catch (Exception e)
            {
                return "Exception:" + e.ToString();
            }


        }
        public string SendMail(string SMTP, string port, string sAddress, string sSubject, string sContent, string sFromAddress, string sPass, string sFromName)
        {
            return EMailServices.SendMail(SMTP, port, sAddress, sSubject, sContent, sFromAddress, sPass, sFromName);
        }

        /// <summary>
        /// This is the generic function that will execute any method you create on this class
        /// </summary>
        /// <param name="Commands"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string Execute(string Commands)
        {
            try
            {
                //transform the commands into an array
                MatchCollection colMatches = RegExClass.ExecuteMatch("(-\\w+:\\w+)|(-\\w+:'.*?'\\s)", Commands);
                System.Collections.Generic.List<string> colArguments = new System.Collections.Generic.List<string>();
                for (int i = 0; i <= colMatches.Count - 1; i++)
                {
                    string strParameter = Strings.Replace(colMatches[i].Value.Trim(), "{&qt}", "'");
                    colArguments.Add(strParameter);
                }

                //parse the arguments
                CommandLineClass cml = new CommandLineClass(colArguments.ToArray());

                //get method name
                string strMethod = cml["m"];

                //get parameter count
                int intParamCount = Convert.ToInt32(cml ["pc"]);

                //retrieve the method
                Execution cls = new Execution();
                try
                {
                    System.Reflection.MethodInfo mi = cls.GetType().GetMethod(strMethod);
                    if (mi == null)
                        return "(error)Method was not found";
                    System.Reflection.ParameterInfo[] pi = mi.GetParameters();
                    if (pi.Length != intParamCount)
                        return "(error)The method has different parameter count against the one you haved informed";

                    //get parameters 
                    object[] arrParameters = new object[intParamCount];
                    int intParamIndex = -1;
                    for (int i = 1; i <= intParamCount; i++)
                    {
                        string strValue = cml["p" + i.ToString()];
                        intParamIndex += 1;
                        System.Reflection.ParameterInfo p = pi[intParamIndex];
                        arrParameters[intParamIndex] = Convert.ChangeType(strValue, p.ParameterType);
                    }
                    return cls.GetType().InvokeMember(strMethod, System.Reflection.BindingFlags.InvokeMethod, null, cls, arrParameters).ToString();
                }
                catch (System.Reflection.AmbiguousMatchException ex)
                {
                    //when ambiguos method found, use the VB function 
                    //  CallByName to resolve this problem
                    //this part maybe you won't port to C#
                    try
                    {
                        //get parameters 
                        object[] arrParameters = new object[intParamCount];
                        int intParamIndex = -1;
                        for (int i = 1; i <= intParamCount; i++)
                        {
                            string strValue = cml["p" + i.ToString()];
                            intParamIndex += 1;
                            arrParameters[intParamIndex] = strValue;
                        }
                        return Interaction.CallByName(cls, strMethod, CallType.Method, arrParameters).ToString();
                    }
                    catch
                    {
                        throw;
                    }
                }
                catch
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return "(error)" + ex.Message;
            }
        }

        /// <summary>
        /// This is a test method
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetGuid()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Returns the current date, giving the ability to format the result
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetDate()
        {
            return DateTime.Now.ToString();
        }

        /// <summary>
        /// Returns the current date, giving the ability to format the result
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetDate(string Format)
        {
            return string.Format("{0:" + Format + "}", DateTime.Now);
        }

        /// <summary>
        /// This function is used to test purposes
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Test(string Text)
        {
            //MsgBox(Text)
            return Text;
        }

        /// <summary>
        /// This function is used to test purposes
        /// </summary>
        /// <param name="Text1"></param>
        /// <param name="Text2"></param>
        /// <param name="Text3"></param>
        /// <param name="Text4"></param>
        /// <param name="Text5"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Test(string Text1, string Text2, string Text3, string Text4, string Text5)
        {
            return Text1 + ", " + Text2 + ", " + Text3 + ", " + Text4 + ", " + Text5;
        }

        public string DeleteLastServerVault(string PlayerCaller, string DeleteFromPlayer, string RenameToExtension)
        {
            IniReader clsIni = new IniReader(System.Windows.Forms.Application.StartupPath + System.IO.Path.DirectorySeparatorChar + "NWNX.ini");
            string strServerVault = clsIni.ReadString("DOTNET", "ServerVaultPath", "C:\\NeverwinterNights\\NWN\\servervault") + System.IO.Path.DirectorySeparatorChar + DeleteFromPlayer;
            if (!System.IO.Directory.Exists(strServerVault))
                return "(error)Server vault doesn't exists";

            DataTable dtb = new DataTable();
            dtb.Columns.Add("path", typeof(string));
            dtb.Columns.Add("lastchange", typeof(DateTime));

            string[] arrFiles = System.IO.Directory.GetFiles(strServerVault);
            if (arrFiles.Length > 0)
            {
                for (int i = 0; i <= arrFiles.Length - 1; i++)
                {
                    dtb.Rows.Add(arrFiles[i], System.IO.File.GetLastWriteTime(arrFiles[i]));
                }
                DataView dtv = new DataView(dtb);
                dtv.Sort = "lastchange DESC";
                string strFilePath = dtv[0]["path"].ToString();
                if (!string.IsNullOrEmpty(RenameToExtension))
                {
                    if (System.IO.File.Exists(strFilePath + "." + RenameToExtension))
                    {
                        System.IO.File.Delete(strFilePath + "." + RenameToExtension);
                    }
                    System.IO.File.Move(strFilePath, strFilePath + "." + RenameToExtension);
                }
                else
                {
                    System.IO.File.Delete(strFilePath);
                }
                return "";
            }
            else
            {
                return "(error)There is no files in this directory: '" + strServerVault + "'";
            }

        }

    }
}