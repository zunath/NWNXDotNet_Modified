//  Programmer: Ludvik Jerabek
//        Date: 08\23\2010
//     Purpose: Allow INI manipulation in .NET

using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Diagnostics;
using System;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using NwnxAssembly;

namespace NwnxAssembly
{
    // IniFile class used to read and write ini files by loading the file into memory
    public class IniReader
    {
        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileIntA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        ///// <summary>
        ///// The GetPrivateProfileInt function retrieves an integer associated with a key in the specified section of an initialization file.
        ///// </summary>
        ///// <param name="lpApplicationName">Pointer to a null-terminated string specifying the name of the section in the initialization file.</param>
        ///// <param name="lpKeyName">Pointer to the null-terminated string specifying the name of the key whose value is to be retrieved. This value is in the form of a string; the GetPrivateProfileInt function converts the string into an integer and returns the integer.</param>
        ///// <param name="nDefault">Specifies the default value to return if the key name cannot be found in the initialization file.</param>
        ///// <param name="lpFileName">Pointer to a null-terminated string that specifies the name of the initialization file. If this parameter does not contain a full path to the file, the system searches for the file in the Windows directory.</param>
        ///// <returns>The return value is the integer equivalent of the string following the specified key name in the specified initialization file. If the key is not found, the return value is the specified default value. If the value of the key is less than zero, the return value is zero.</returns>
        private static extern int GetPrivateProfileInt(string lpApplicationName, string lpKeyName, int nDefault, string lpFileName);
        [DllImport("kernel32.dll", EntryPoint = "WritePrivateProfileStringA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        ///// <summary>
        ///// The WritePrivateProfileString function copies a string into the specified section of an initialization file.
        ///// </summary>
        ///// <param name="lpApplicationName">Pointer to a null-terminated string containing the name of the section to which the string will be copied. If the section does not exist, it is created. The name of the section is case-independent; the string can be any combination of uppercase and lowercase letters.</param>
        ///// <param name="lpKeyName">Pointer to the null-terminated string containing the name of the key to be associated with a string. If the key does not exist in the specified section, it is created. If this parameter is NULL, the entire section, including all entries within the section, is deleted.</param>
        ///// <param name="lpString">Pointer to a null-terminated string to be written to the file. If this parameter is NULL, the key pointed to by the lpKeyName parameter is deleted.</param>
        ///// <param name="lpFileName">Pointer to a null-terminated string that specifies the name of the initialization file.</param>
        ///// <returns>If the function successfully copies the string to the initialization file, the return value is nonzero; if the function fails, or if it flushes the cached version of the most recently accessed initialization file, the return value is zero.</returns>
        private static extern int WritePrivateProfileString(string lpApplicationName, string lpKeyName, string lpString, string lpFileName);
        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileStringA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        ///// <summary>
        ///// The GetPrivateProfileString function retrieves a string from the specified section in an initialization file.
        ///// </summary>
        ///// <param name="lpApplicationName">Pointer to a null-terminated string that specifies the name of the section containing the key name. If this parameter is NULL, the GetPrivateProfileString function copies all section names in the file to the supplied buffer.</param>
        ///// <param name="lpKeyName">Pointer to the null-terminated string specifying the name of the key whose associated string is to be retrieved. If this parameter is NULL, all key names in the section specified by the lpAppName parameter are copied to the buffer specified by the lpReturnedString parameter.</param>
        ///// <param name="lpDefault">Pointer to a null-terminated default string. If the lpKeyName key cannot be found in the initialization file, GetPrivateProfileString copies the default string to the lpReturnedString buffer. This parameter cannot be NULL. <br>Avoid specifying a default string with trailing blank characters. The function inserts a null character in the lpReturnedString buffer to strip any trailing blanks.</br></param>
        ///// <param name="lpReturnedString">Pointer to the buffer that receives the retrieved string.</param>
        ///// <param name="nSize">Specifies the size, in TCHARs, of the buffer pointed to by the lpReturnedString parameter.</param>
        ///// <param name="lpFileName">Pointer to a null-terminated string that specifies the name of the initialization file. If this parameter does not contain a full path to the file, the system searches for the file in the Windows directory.</param>
        ///// <returns>The return value is the number of characters copied to the buffer, not including the terminating null character.</returns>
        private static extern int GetPrivateProfileString(string lpApplicationName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);
        [DllImport("kernel32", EntryPoint = "GetPrivateProfileSectionNamesA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        ///// <summary>
        ///// The GetPrivateProfileSectionNames function retrieves the names of all sections in an initialization file.
        ///// </summary>
        ///// <param name="lpszReturnBuffer">Pointer to a buffer that receives the section names associated with the named file. The buffer is filled with one or more null-terminated strings; the last string is followed by a second null character.</param>
        ///// <param name="nSize">Specifies the size, in TCHARs, of the buffer pointed to by the lpszReturnBuffer parameter.</param>
        ///// <param name="lpFileName">Pointer to a null-terminated string that specifies the name of the initialization file. If this parameter is NULL, the function searches the Win.ini file. If this parameter does not contain a full path to the file, the system searches for the file in the Windows directory.</param>
        ///// <returns>The return value specifies the number of characters copied to the specified buffer, not including the terminating null character. If the buffer is not large enough to contain all the section names associated with the specified initialization file, the return value is equal to the length specified by nSize minus two.</returns>
        private static extern int GetPrivateProfileSectionNames(byte[] lpszReturnBuffer, int nSize, string lpFileName);
        [DllImport("kernel32.dll", EntryPoint = "WritePrivateProfileSectionA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        ///// <summary>
        ///// The WritePrivateProfileSection function replaces the keys and values for the specified section in an initialization file.
        ///// </summary>
        ///// <param name="lpAppName">Pointer to a null-terminated string specifying the name of the section in which data is written. This section name is typically the name of the calling application.</param>
        ///// <param name="lpString">Pointer to a buffer containing the new key names and associated values that are to be written to the named section.</param>
        ///// <param name="lpFileName">Pointer to a null-terminated string containing the name of the initialization file. If this parameter does not contain a full path for the file, the function searches the Windows directory for the file. If the file does not exist and lpFileName does not contain a full path, the function creates the file in the Windows directory. The function does not create a file if lpFileName contains the full path and file name of a file that does not exist.</param>
        ///// <returns>If the function succeeds, the return value is nonzero.<br>If the function fails, the return value is zero.</br></returns>
        private static extern int WritePrivateProfileSection(string lpAppName, string lpString, string lpFileName);

        //Private variables and constants
        ///// <summary>
        ///// Holds the full path to the INI file.
        ///// </summary>
        private string m_Filename;
        ///// <summary>Gets or sets the section you're working in. (aka 'the active section')</summary>
        ///// <value>A String representing the section you're working in.</value>
        public string Section
        {
            get { return m_Section; }
            set { m_Section = value; }
        }
        ///// <summary>
        ///// The maximum number of bytes in a section buffer.
        ///// </summary>
        private const int MAX_ENTRY = 32768;

        // List of IniSection objects keeps track of all the sections in the INI file
        private Hashtable m_sections;

        ///// <summary>
        ///// Holds the active section name
        ///// </summary>
        private string m_Section;

        // Public constructor
        public IniReader(string file)
        {
            Filename = file;
            m_sections = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
        }

        ///// <summary>Gets or sets the full path to the INI file.</summary>
        ///// <value>A String representing the full path to the INI file.</value>
        public string Filename
        {
            get { return m_Filename; }
            set { m_Filename = value; }
        }

        // Loads the Reads the data in the ini file into the IniFile object
        public void Load(string sFileName)
        {
            Load(sFileName, false);
        }

        // Loads the Reads the data in the ini file into the IniFile object
        public void Load(string sFileName, bool bMerge)
        {
            if (!bMerge)
            {
                RemoveAllSections();
            }
            //  Clear the object... 
            IniSection tempsection = null;
            StreamReader oReader = new StreamReader(sFileName);
            Regex regexcomment = new Regex("^([\\s]*#.*)", (RegexOptions.Singleline | RegexOptions.IgnoreCase));
            // ^[\\s]*\\[[\\s]*([^\\[\\s].*[^\\s\\]])[\\s]*\\][\\s]*$
            Regex regexsection = new Regex("^[\\s]*\\[[\\s]*([^\\[\\s].*[^\\s\\]])[\\s]*\\][\\s]*$", (RegexOptions.Singleline | RegexOptions.IgnoreCase));
            //Regex regexsection = new Regex("\\[[\\s]*([^\\[\\s].*[^\\s\\]])[\\s]*\\]", (RegexOptions.Singleline | RegexOptions.IgnoreCase));
            Regex regexkey = new Regex("^\\s*([^=\\s]*)[^=]*=(.*)", (RegexOptions.Singleline | RegexOptions.IgnoreCase));
            while (!oReader.EndOfStream)
            {
                string line = oReader.ReadLine();
                if (line != string.Empty)
                {
                    Match m = null;
                    if (regexcomment.Match(line).Success)
                    {
                        m = regexcomment.Match(line);
                        Trace.WriteLine(string.Format("Skipping Comment: {0}", m.Groups[0].Value));
                    }
                    else if (regexsection.Match(line).Success)
                    {
                        m = regexsection.Match(line);
                        Trace.WriteLine(string.Format("Adding section [{0}]", m.Groups[1].Value));
                        tempsection = AddSection(m.Groups[1].Value);
                    }
                    else if (regexkey.Match(line).Success && tempsection != null)
                    {
                        m = regexkey.Match(line);
                        Trace.WriteLine(string.Format("Adding Key [{0}]=[{1}]", m.Groups[1].Value, m.Groups[2].Value));
                        tempsection.AddKey(m.Groups[1].Value).Value = m.Groups[2].Value;
                    }
                    else if (tempsection != null)
                    {
                        //  Handle Key without value
                        Trace.WriteLine(string.Format("Adding Key [{0}]", line));
                        tempsection.AddKey(line);
                    }
                    else
                    {
                        //  This should not occur unless the tempsection is not created yet...
                        Trace.WriteLine(string.Format("Skipping unknown type of data: {0}", line));
                    }
                }
            }
            oReader.Close();
        }

        // Used to save the data back to the file or your choice
        public void Save(string sFileName)
        {
            StreamWriter oWriter = new StreamWriter(sFileName, false);
            foreach (IniSection s in Sections)
            {
                Trace.WriteLine(string.Format("Writing Section: [{0}]", s.Name));
                oWriter.WriteLine(string.Format("[{0}]", s.Name));
                foreach (IniSection.IniKey k in s.Keys)
                {
                    if (k.Value != string.Empty)
                    {
                        Trace.WriteLine(string.Format("Writing Key: {0}={1}", k.Name, k.Value));
                        oWriter.WriteLine(string.Format("{0}={1}", k.Name, k.Value));
                    }
                    else
                    {
                        Trace.WriteLine(string.Format("Writing Key: {0}", k.Name));
                        oWriter.WriteLine(string.Format("{0}", k.Name));
                    }
                }
            }
            oWriter.Close();
        }

        // Gets all the sections names
        public System.Collections.ICollection Sections
        {
            get
            {
                return m_sections.Values;
            }
        }

        // Adds a section to the IniFile object, returns a IniSection object to the new or existing object
        public IniSection AddSection(string sSection)
        {
            IniSection s = null;
            sSection = sSection.Trim();
            // Trim spaces
            if (m_sections.ContainsKey(sSection))
            {
                s = (IniSection)m_sections[sSection];
            }
            else
            {
                s = new IniSection(this, sSection);
                m_sections[sSection] = s;
            }
            return s;
        }

        // Removes a section by its name sSection, returns trus on success
        public bool RemoveSection(string sSection)
        {
            sSection = sSection.Trim();
            return RemoveSection(GetSection(sSection));
        }

        // Removes section by object, returns trus on success
        public bool RemoveSection(IniSection Section)
        {
            if (Section != null)
            {
                try
                {
                    m_sections.Remove(Section.Name);
                    return true;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
            }
            return false;
        }

        //  Removes all existing sections, returns trus on success
        public bool RemoveAllSections()
        {
            m_sections.Clear();
            return (m_sections.Count == 0);
        }

        public string ReadString(string section, string key, string defVal)
        {
            StringBuilder sb = new StringBuilder(MAX_ENTRY);
            int Ret = GetPrivateProfileString(section, key, defVal, sb, MAX_ENTRY, Filename);
            return sb.ToString();
        }
        ///// <summary>Reads a String from the specified key of the specified section.</summary>
        ///// <param name="section">The section to search in.</param>
        ///// <param name="key">The key from which to return the value.</param>
        ///// <returns>Returns the value of the specified section/key pair, or returns an empty String if the specified section/key pair isn't found in the INI file.</returns>
        public string ReadString(string section, string key)
        {
            return ReadString(section, key, "");
        }
        ///// <summary>Reads a String from the specified key of the active section.</summary>
        ///// <param name="key">The key from which to return the value.</param>
        ///// <returns>Returns the value of the specified key, or returns an empty String if the specified key isn't found in the active section of the INI file.</returns>
        public string ReadString(string key)
        {
            return ReadString(Section, key);
        }
        ///// <summary>Reads a Long from the specified key of the specified section.</summary>
        ///// <param name="section">The section to search in.</param>
        ///// <param name="key">The key from which to return the value.</param>
        ///// <param name="defVal">The value to return if the specified key isn't found.</param>
        ///// <returns>Returns the value of the specified section/key pair, or returns the default value if the specified section/key pair isn't found in the INI file.</returns>
        public long ReadLong(string section, string key, long defVal)
        {
            return long.Parse(ReadString(section, key, defVal.ToString()));
        }
        ///// <summary>Reads a Long from the specified key of the specified section.</summary>
        ///// <param name="section">The section to search in.</param>
        ///// <param name="key">The key from which to return the value.</param>
        ///// <returns>Returns the value of the specified section/key pair, or returns 0 if the specified section/key pair isn't found in the INI file.</returns>
        public long ReadLong(string section, string key)
        {
            return ReadLong(section, key, 0);
        }
        ///// <summary>Reads a Long from the specified key of the active section.</summary>
        ///// <param name="key">The key from which to return the value.</param>
        ///// <param name="defVal">The section to search in.</param>
        ///// <returns>Returns the value of the specified key, or returns the default value if the specified key isn't found in the active section of the INI file.</returns>
        public long ReadLong(string key, long defVal)
        {
            return ReadLong(Section, key, defVal);
        }
        ///// <summary>Reads a Long from the specified key of the active section.</summary>
        ///// <param name="key">The key from which to return the value.</param>
        ///// <returns>Returns the value of the specified Key, or returns 0 if the specified Key isn't found in the active section of the INI file.</returns>
        public long ReadLong(string key)
        {
            return ReadLong(key, 0);
        }
        ///// <summary>Reads a Byte array from the specified key of the specified section.</summary>
        ///// <param name="section">The section to search in.</param>
        ///// <param name="key">The key from which to return the value.</param>
        ///// <returns>Returns the value of the specified section/key pair, or returns null (Nothing in VB.NET) if the specified section/key pair isn't found in the INI file.</returns>
        public byte[] ReadByteArray(string section, string key)
        {
            try
            {
                return Convert.FromBase64String(ReadString(section, key));
            }
            catch
            {
            }
            return null;
        }
        ///// <summary>Reads a Byte array from the specified key of the active section.</summary>
        ///// <param name="key">The key from which to return the value.</param>
        ///// <returns>Returns the value of the specified key, or returns null (Nothing in VB.NET) if the specified key pair isn't found in the active section of the INI file.</returns>
        public byte[] ReadByteArray(string key)
        {
            return ReadByteArray(Section, key);
        }
        ///// <summary>Reads a Boolean from the specified key of the specified section.</summary>
        ///// <param name="section">The section to search in.</param>
        ///// <param name="key">The key from which to return the value.</param>
        ///// <param name="defVal">The value to return if the specified key isn't found.</param>
        ///// <returns>Returns the value of the specified section/key pair, or returns the default value if the specified section/key pair isn't found in the INI file.</returns>
        public bool ReadBoolean(string section, string key, bool defVal)
        {
            return bool.Parse(ReadString(section, key, defVal.ToString()));
        }
        ///// <summary>Reads a Boolean from the specified key of the specified section.</summary>
        ///// <param name="section">The section to search in.</param>
        ///// <param name="key">The key from which to return the value.</param>
        ///// <returns>Returns the value of the specified section/key pair, or returns false if the specified section/key pair isn't found in the INI file.</returns>
        public bool ReadBoolean(string section, string key)
        {
            return ReadBoolean(section, key, false);
        }
        ///// <summary>Reads a Boolean from the specified key of the specified section.</summary>
        ///// <param name="key">The key from which to return the value.</param>
        ///// <param name="defVal">The value to return if the specified key isn't found.</param>
        ///// <returns>Returns the value of the specified key pair, or returns the default value if the specified key isn't found in the active section of the INI file.</returns>
        public bool ReadBoolean(string key, bool defVal)
        {
            return ReadBoolean(Section, key, defVal);
        }
        ///// <summary>Reads a Boolean from the specified key of the specified section.</summary>
        ///// <param name="key">The key from which to return the value.</param>
        ///// <returns>Returns the value of the specified key, or returns false if the specified key isn't found in the active section of the INI file.</returns>
        public bool ReadBoolean(string key)
        {
            return ReadBoolean(Section, key);
        }
        ///// <summary>Writes an Integer to the specified key in the specified section.</summary>
        ///// <param name="section">The section to write in.</param>
        ///// <param name="key">The key to write to.</param>
        ///// <param name="value">The value to write.</param>
        ///// <returns>Returns true if the function succeeds, false otherwise.</returns>
        public bool Write(string section, string key, int value)
        {
            //return FileSystem.Write(section, key, value.ToString());
            return (WritePrivateProfileString(section, key, value.ToString(), Filename) != 0);
        }
        ///// <summary>Writes an Integer to the specified key in the active section.</summary>
        ///// <param name="key">The key to write to.</param>
        ///// <param name="value">The value to write.</param>
        ///// <returns>Returns true if the function succeeds, false otherwise.</returns>
        public bool Write(string key, int value)
        {
            //return FileSystem.Write(Section, key, value);
            return (WritePrivateProfileString(Section, key, value.ToString(), Filename) != 0);
        }
        ///// <summary>Writes a String to the specified key in the specified section.</summary>
        ///// <param name="section">Specifies the section to write in.</param>
        ///// <param name="key">Specifies the key to write to.</param>
        ///// <param name="value">Specifies the value to write.</param>
        ///// <returns>Returns true if the function succeeds, false otherwise.</returns>
        public bool Write(string section, string key, string value)
        {
            return (WritePrivateProfileString(section, key, value, Filename) != 0);
        }
        ///// <summary>Writes a String to the specified key in the active section.</summary>
        /////	<param name="key">The key to write to.</param>
        ///// <param name="value">The value to write.</param>
        ///// <returns>Returns true if the function succeeds, false otherwise.</returns>
        public bool Write(string key, string value)
        {
            return (WritePrivateProfileString(Section, key, value, Filename) != 0);
            //FileSystem.Write(Section, key, value);
        }
        ///// <summary>Writes a Long to the specified key in the specified section.</summary>
        ///// <param name="section">The section to write in.</param>
        ///// <param name="key">The key to write to.</param>
        ///// <param name="value">The value to write.</param>
        ///// <returns>Returns true if the function succeeds, false otherwise.</returns>
        public bool Write(string section, string key, long value)
        {
            //return FileSystem.Write(section, key, value.ToString());
            return (WritePrivateProfileString(section, key, value.ToString(), Filename) != 0);
        }
        ///// <summary>Writes a Long to the specified key in the active section.</summary>
        ///// <param name="key">The key to write to.</param>
        ///// <param name="value">The value to write.</param>
        ///// <returns>Returns true if the function succeeds, false otherwise.</returns>
        public bool Write(string key, long value)
        {
            //return FileSystem.Write(Section, key, value);
            return (WritePrivateProfileString(Section, key, value.ToString(), Filename) != 0);
        }
        ///// <summary>Writes a Byte array to the specified key in the specified section.</summary>
        ///// <param name="section">The section to write in.</param>
        ///// <param name="key">The key to write to.</param>
        ///// <param name="value">The value to write.</param>
        ///// <param name="offset">An offset in <i>value</i>.</param>
        ///// <param name="length">The number of elements of <i>value</i> to convert.</param>
        ///// <returns>Returns true if the function succeeds, false otherwise.</returns>
        public bool Write(string section, string key, byte[] value, int offset, int length)
        {
            if (value == null)
            {
                //return FileSystem.Write(section, key, Convert.ToString(null));
                return Write(section, key, Convert.ToString(null));
            }
            else
            {
                //return FileSystem.Write(section, key, Convert.ToBase64String(value, offset, length));
                return Write(section, key, Convert.ToBase64String(value, offset, length));
            }
        }
        ///// <summary>Writes a Boolean to the specified key in the specified section.</summary>
        ///// <param name="section">The section to write in.</param>
        ///// <param name="key">The key to write to.</param>
        ///// <param name="value">The value to write.</param>
        ///// <returns>Returns true if the function succeeds, false otherwise.</returns>
        public bool Write(string section, string key, bool value)
        {
            //return FileSystem.Write(section, key, value.ToString());
            return (WritePrivateProfileString(section, key, value.ToString(), Filename) != 0);
        }
        ///// <summary>Writes a Byte array to the specified key in the specified section.</summary>
        ///// <param name="section">The section to write in.</param>
        ///// <param name="key">The key to write to.</param>
        ///// <param name="value">The value to write.</param>
        ///// <returns>Returns true if the function succeeds, false otherwise.</returns>
        public bool Write(string section, string key, byte[] value)
        {
            if (value == null)
            {
                //return FileSystem.Write(section, key, Convert.ToString(null));
                return Write(section, key, Convert.ToString(null));
            }
            else
            {
                //return FileSystem.Write(section, key, value, 0, value.Length);
                return Write(section, key, value, 0, value.Length);
            }
        }
        ///// <summary>Writes a Byte array to the specified key in the active section.</summary>
        ///// <param name="key">The key to write to.</param>
        ///// <param name="value">The value to write.</param>
        ///// <returns>Returns true if the function succeeds, false otherwise.</returns>
        public bool Write(string key, byte[] value)
        {
            //return FileSystem.Write(Section, key, value);
            return Write(Section, key, value.ToString());
        }
        ///// <summary>Writes a Boolean to the specified key in the active section.</summary>
        ///// <param name="key">The key to write to.</param>
        ///// <param name="value">The value to write.</param>
        ///// <returns>Returns true if the function succeeds, false otherwise.</returns>
        public bool Write(string key, bool value)
        {
            //return FileSystem.Write(Section, key, value);
            return Write(Section, key, value.ToString());
        }
        ///// <summary>Deletes a key from the specified section.</summary>
        ///// <param name="section">The section to delete from.</param>
        ///// <param name="key">The key to delete.</param>
        ///// <returns>Returns true if the function succeeds, false otherwise.</returns>
        public bool DeleteKey(string section, string key)
        {
            return (WritePrivateProfileString(section, key, null, Filename) != 0);
        }
        ///// <summary>Deletes a key from the active section.</summary>
        ///// <param name="key">The key to delete.</param>
        ///// <returns>Returns true if the function succeeds, false otherwise.</returns>
        public bool DeleteKey(string key)
        {
            return (WritePrivateProfileString(Section, key, null, Filename) != 0);
        }
        ///// <summary>Deletes a section from an INI file.</summary>
        ///// <param name="section">The section to delete.</param>
        ///// <returns>Returns true if the function succeeds, false otherwise.</returns>
        public bool DeleteSection(string section)
        {
            return WritePrivateProfileSection(section, null, Filename) != 0;
        }
        ///// <summary>Retrieves a list of all available sections in the INI file.</summary>
        ///// <returns>Returns an ArrayList with all available sections.</returns>
        public ArrayList GetSectionNames()
        {
            try
            {
                byte[] buffer = new byte[MAX_ENTRY + 1];
                GetPrivateProfileSectionNames(buffer, MAX_ENTRY, Filename);
                string[] parts = Encoding.ASCII.GetString(buffer).Trim(ControlChars.NullChar).Split(ControlChars.NullChar);
                return new ArrayList(parts);
            }
            catch
            {
            }
            return null;
        }
        // Returns an IniSection to the section by name, NULL if it was not found
        public IniSection GetSection(string sSection)
        {
            sSection = sSection.Trim();
            // Trim spaces
            if (m_sections.ContainsKey(sSection))
            {
                return (IniSection)m_sections[sSection];
            }
            return null;
        }

        //  Returns a KeyValue in a certain section
        public string GetKeyValue(string sSection, string sKey)
        {
            IniSection s = GetSection(sSection);
            if (s != null)
            {
                IniSection.IniKey k = s.GetKey(sKey);
                if (k != null)
                {
                    return k.Value;
                }
            }
            return string.Empty;
        }

        // Sets a KeyValuePair in a certain section
        public bool SetKeyValue(string sSection, string sKey, string sValue)
        {
            IniSection s = AddSection(sSection);
            if (s != null)
            {
                IniSection.IniKey k = s.AddKey(sKey);
                if (k != null)
                {
                    k.Value = sValue;
                    return true;
                }
            }
            return false;
        }

        // Renames an existing section returns true on success, false if the section didn't exist or there was another section with the same sNewSection
        public bool RenameSection(string sSection, string sNewSection)
        {
            //  Note string trims are done in lower calls.
            bool bRval = false;
            IniSection s = GetSection(sSection);
            if (s != null)
            {
                bRval = s.SetName(sNewSection);
            }
            return bRval;
        }

        // Renames an existing key returns true on success, false if the key didn't exist or there was another section with the same sNewKey
        public bool RenameKey(string sSection, string sKey, string sNewKey)
        {
            //  Note string trims are done in lower calls.
            IniSection s = GetSection(sSection);
            if (s != null)
            {
                IniSection.IniKey k = s.GetKey(sKey);
                if (k != null)
                {
                    return k.SetName(sNewKey);
                }
            }
            return false;
        }

        // IniSection class 
        public class IniSection
        {
            //  IniFile IniFile object instance
            private IniReader m_pIniFile;
            //  Name of the section
            private string m_sSection;
            //  List of IniKeys in the section
            private Hashtable m_keys;

            // Constuctor so objects are internally managed
            protected internal IniSection(IniReader parent, string sSection)
            {
                m_pIniFile = parent;
                m_sSection = sSection;
                m_keys = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
            }

            // Returns and hashtable of keys associated with the section
            public System.Collections.ICollection Keys
            {
                get
                {
                    return m_keys.Values;
                }
            }

            // Returns the section name
            public string Name
            {
                get
                {
                    return m_sSection;
                }
            }

            // Adds a key to the IniSection object, returns a IniKey object to the new or existing object
            public IniKey AddKey(string sKey)
            {
                sKey = sKey.Trim();
                IniSection.IniKey k = null;
                if (sKey.Length != 0)
                {
                    if (m_keys.ContainsKey(sKey))
                    {
                        k = (IniKey)m_keys[sKey];
                    }
                    else
                    {
                        k = new IniSection.IniKey(this, sKey);
                        m_keys[sKey] = k;
                    }
                }
                return k;
            }

            // Removes a single key by string
            public bool RemoveKey(string sKey)
            {
                return RemoveKey(GetKey(sKey));
            }

            // Removes a single key by IniKey object
            public bool RemoveKey(IniKey Key)
            {
                if (Key != null)
                {
                    try
                    {
                        m_keys.Remove(Key.Name);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.Message);
                    }
                }
                return false;
            }

            // Removes all the keys in the section
            public bool RemoveAllKeys()
            {
                m_keys.Clear();
                return (m_keys.Count == 0);
            }

            // Returns a IniKey object to the key by name, NULL if it was not found
            public IniKey GetKey(string sKey)
            {
                sKey = sKey.Trim();
                if (m_keys.ContainsKey(sKey))
                {
                    return (IniKey)m_keys[sKey];
                }
                return null;
            }

            // Sets the section name, returns true on success, fails if the section
            // name sSection already exists
            public bool SetName(string sSection)
            {
                sSection = sSection.Trim();
                if (sSection.Length != 0)
                {
                    // Get existing section if it even exists...
                    IniSection s = m_pIniFile.GetSection(sSection);
                    if (s != this && s != null) return false;
                    try
                    {
                        // Remove the current section
                        m_pIniFile.m_sections.Remove(m_sSection);
                        // Set the new section name to this object
                        m_pIniFile.m_sections[sSection] = this;
                        // Set the new section name
                        m_sSection = sSection;
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.Message);
                    }
                }
                return false;
            }

            // Returns the section name
            public string GetName()
            {
                return m_sSection;
            }

            // IniKey class
            public class IniKey
            {
                //  Name of the Key
                private string m_sKey;
                //  Value associated
                private string m_sValue;
                //  Pointer to the parent CIniSection
                private IniSection m_section;

                // Constuctor so objects are internally managed
                protected internal IniKey(IniSection parent, string sKey)
                {
                    m_section = parent;
                    m_sKey = sKey;
                }

                // Returns the name of the Key
                public string Name
                {
                    get
                    {
                        return m_sKey;
                    }
                }

                // Sets or Gets the value of the key
                public string Value
                {
                    get
                    {
                        return m_sValue;
                    }
                    set
                    {
                        m_sValue = value;
                    }
                }

                // Sets the value of the key
                public void SetValue(string sValue)
                {
                    m_sValue = sValue;
                }
                // Returns the value of the Key
                public string GetValue()
                {
                    return m_sValue;
                }

                // Sets the key name
                // Returns true on success, fails if the section name sKey already exists
                public bool SetName(string sKey)
                {
                    sKey = sKey.Trim();
                    if (sKey.Length != 0)
                    {
                        IniKey k = m_section.GetKey(sKey);
                        if (k != this && k != null) return false;
                        try
                        {
                            // Remove the current key
                            m_section.m_keys.Remove(m_sKey);
                            // Set the new key name to this object
                            m_section.m_keys[sKey] = this;
                            // Set the new key name
                            m_sKey = sKey;
                            return true;
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(ex.Message);
                        }
                    }
                    return false;
                }

                // Returns the name of the Key
                public string GetName()
                {
                    return m_sKey;
                }
            } // End of IniKey class
        } // End of IniSection class
    } // End of IniFile class


}