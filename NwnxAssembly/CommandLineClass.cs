using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

/// <summary> 
/// Arguments class 
/// </summary> 
public class CommandLineClass
{
    // Variables 
    public MyDictionary<string, string> mcolParameters;

    private System.Collections.Generic.List<string> mcolKeys;
    /// <summary>
    /// Constructor 
    /// </summary>
    /// <param name="Args"></param>
    /// <remarks></remarks>
    public CommandLineClass(System.Collections.ObjectModel.ReadOnlyCollection<string> Args)
    {
        System.Collections.Generic.List<string> colArgs = new System.Collections.Generic.List<string>();
        for (int i = 0; i <= Args.Count - 1; i++)
        {
            colArgs.Add(Args[i]);
        }
        LoadArguments(colArgs.ToArray());
    }

    /// <summary>
    /// Constructor 
    /// </summary>
    /// <param name="Args"></param>
    /// <remarks></remarks>
    public CommandLineClass(string[] Args)
    {
        LoadArguments(Args);
    }

    /// <summary>
    /// Loads all arguments and saves to this class
    /// </summary>
    /// <param name="Args"></param>
    /// <remarks></remarks>
    private void LoadArguments(string[] Args)
    {
        mcolParameters = new MyDictionary<string, string>();
        Regex regexSpliter = new Regex("^-{1,2}|^/|=|:", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex regexRemover = new Regex("^['\"]?(.*?)['\"]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        string strName = null;
        string[] arrParts = null;

        // Valid parameters forms: 
        // {-,/,--}param{ ,=,:}((",')value(",')) 
        // Examples: 
        // -param1 value1 --param2 /param3:"Test-:-work" 
        // /param4=happy -param5 '--=nice=--' 
        foreach (string Txt in Args)
        {
            // Look for new parameters (-,/ or --) and a 
            // possible enclosed value (=,:) 
            arrParts = regexSpliter.Split(Txt, 3);

            switch (arrParts.Length)
            {
                case 1:
                    // Found a value (for the last parameter 
                    // found (space separator)) 
                    if (strName != null)
                    {
                        if (!mcolParameters.ContainsKey(strName.ToUpper()))
                        {
                            arrParts[0] = regexRemover.Replace(arrParts[0], "$1");

                            mcolParameters.Add(strName.ToUpper(), arrParts[0]);
                        }
                        strName = null;
                    }
                    // else Error: no parameter waiting for a value (skipped) 
                    break; // TODO: might not be correct. Was : Exit Select
                case 2:

                    // Found just a parameter 
                    // The last parameter is still waiting. 
                    // With no value, set it to true. 
                    if (strName != null)
                    {
                        if (!mcolParameters.ContainsKey(strName.ToUpper()))
                        {
                            mcolParameters.Add(strName.ToUpper(), "true");
                        }
                    }
                    strName = arrParts[1];
                    break; // TODO: might not be correct. Was : Exit Select
                case 3:

                    // Parameter with enclosed value 
                    // The last parameter is still waiting. 
                    // With no value, set it to true. 
                    if (strName != null)
                    {
                        if (!mcolParameters.ContainsKey(strName.ToUpper()))
                        {
                            mcolParameters.Add(strName.ToUpper(), "true");
                        }
                    }

                    strName = arrParts[1];

                    // Remove possible enclosing characters (",') 
                    if (!mcolParameters.ContainsKey(strName.ToUpper()))
                    {
                        arrParts[2] = regexRemover.Replace(arrParts[2], "$1");
                        mcolParameters.Add(strName.ToUpper(), arrParts[2]);
                    }

                    strName = null;
                    break; // TODO: might not be correct. Was : Exit Select
            }
        }
        // In case a parameter is still waiting 
        if (strName != null)
        {
            if (!mcolParameters.ContainsKey(strName.ToUpper()))
            {
                mcolParameters.Add(strName.ToUpper(), "true");
            }
        }
    }

    /// <summary>
    /// Retrieve a parameter value if it exists 
    /// (overriding C# indexer property) 
    /// </summary>
    /// <param name="Name"></param>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public string this[string Name]
    {
        get { return (mcolParameters[Name.ToUpper()]); }
    }

    /// <summary>
    /// Retrieve a parameter value if it exists 
    /// (overriding C# indexer property) 
    /// </summary>
    /// <param name="Index"></param>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public string this[int Index]
    {
        get {
            var enumer = mcolParameters.GetEnumerator();
            for(int i = 0; i < Index; i++) 
            {
                try
                {
                    enumer.MoveNext();
                }
                catch
                {
                    return "";
                }
            }
            return enumer.Current.Value;
        }
    }

    /// <summary>
    /// Checks if the parameter name exists
    /// </summary>
    /// <param name="Name"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public bool ContainsName(string Name)
    {
        return mcolParameters.ContainsKey(Name.ToUpper());
    }

    /// <summary>
    /// Checks if the parameter value exists
    /// </summary>
    /// <param name="Value"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public bool ContainsValue(string Value)
    {
        return mcolParameters.ContainsValue(Value.ToUpper());
    }

    /// <summary>
    /// Provides a easy way to retrieving the value from the collection, and additionally gives a default value when the parameter not found
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="DefaultValue"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public string GetParameterValue(string Name, string DefaultValue)
    {
        if (this.ContainsName(Name))
        {
            return this[Name];
        }
        else
        {
            return DefaultValue;
        }
    }

    /// <summary>
    /// Returns count of parsed items
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public int Count
    {
        get { return mcolParameters.Count; }
    }

}