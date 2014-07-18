using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace NwnxAssembly
{
    public class RegExClass
    {
        public static System.Text.RegularExpressions.Regex CreateRegEx(string Expression)
        {
            string regex = Expression;
            System.Text.RegularExpressions.RegexOptions options = System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace | System.Text.RegularExpressions.RegexOptions.Multiline | System.Text.RegularExpressions.RegexOptions.IgnoreCase;
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(regex, options);
            return reg;
        }

        public static string ExecuteSingleMatch(string RuleExpression, string Text)
        {
            if (string.IsNullOrEmpty(Text))
                return "";

            Regex objRegex = CreateRegEx(RuleExpression);
            if (objRegex.Matches(Text).Count == 0)
            {
                return "";
            }
            else
            {
                return objRegex.Matches(Text)[0].Value;
            }
        }

        public static MatchCollection ExecuteMatch(string RuleExpression, string Text)
        {
            if (string.IsNullOrEmpty(Text))
                return ExecuteMatch("ThisShould", "ReturnZero");
            Regex objRegex = CreateRegEx(RuleExpression);
            return objRegex.Matches(Text);
        }

        public static string ExecuteReplace(string RuleExpression, string Text, string Replacement)
        {
            return ExecuteReplace(RuleExpression, Text, Replacement, 0);
        }

        public static string ExecuteReplace(string RuleExpression, string Text, string Replacement, int Count)
        {
            Regex objRegex = CreateRegEx(RuleExpression);
            if (Count > 0)
            {
                return objRegex.Replace(Text, Replacement, Count);
            }
            else
            {
                return objRegex.Replace(Text, Replacement);
            }
        }
    }
}