// --------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This software is licensed under the MIT license. See LICENSE file for details.
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TCPluginInstaller.Logic
{
    /// <summary>
    /// Ini file helper class
    /// </summary>
    internal static class IniFile
    {
        private static int _bufferSize = 1024;

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section, string key, string defaultValue, StringBuilder value, int size, string filePath);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string section, string key, string defaultValue, [In, Out] char[] value, int size, string filePath);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int GetPrivateProfileSection(string section, IntPtr keyValue, int size, string filePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool WritePrivateProfileString(string section, string key, string value, string filePath);

        /// <summary>
        /// Get key value pairs
        /// </summary>
        /// <param name="file">Ini file path</param>
        /// <param name="section">Section to read</param>
        /// <returns>KeyvaluePairs as dictionary</returns>
        public static IDictionary<string, string> GetKeyValuePairs(string file, string section)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            while (true)
            {
                IntPtr returnedString = Marshal.AllocCoTaskMem(_bufferSize * sizeof(char));
                int size = GetPrivateProfileSection(section, returnedString, _bufferSize, file);

                if (size == 0 || _bufferSize == 0)
                {
                    Marshal.FreeCoTaskMem(returnedString);
                    return ret;
                }
                if (size < _bufferSize - 2)
                {
                    var managedString = Marshal.PtrToStringAuto(returnedString, size - 1);
                    Marshal.FreeCoTaskMem(returnedString);
                    var lines = managedString.Split('\0');

                    if (lines.Length > 0)
                    {
                        foreach (var line in lines)
                        {
                            var pair = line.Split('=');
                            ret.Add(pair[0], pair[1]);
                        }

                        return ret;
                    }
                }

                Marshal.FreeCoTaskMem(returnedString);
                _bufferSize *= 2;
            }
        }

        /// <summary>
        /// Write a value to an Ini file
        /// </summary>
        /// <param name="file">File</param>
        /// <param name="section">Section</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <returns>true if succesfull, otherwise false</returns>
        public static bool WriteValue(string file, string section, string key, string value)
        {
            return WritePrivateProfileString(section, key, value, file);
        }

        /// <summary>
        /// Read a value from an Ini file
        /// </summary>
        /// <param name="file">File</param>
        /// <param name="section">Section</param>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value to return, if key not found</param>
        /// <returns>Key value or default value</returns>
        public static string ReadValue(string file, string section, string key, string defaultValue = "")
        {
            var value = new StringBuilder(_bufferSize);
            GetPrivateProfileString(section, key, defaultValue, value, value.Capacity, file);
            return value.ToString();
        }

        /// <summary>
        /// Delete a key from the Ini file
        /// </summary>
        /// <param name="file">File</param>
        /// <param name="section">Section</param>
        /// <param name="key">key</param>
        /// <returns>true if succesfull, otherwise false</returns>
        public static bool DeleteKey(string file, string section, string key)
        {
            return WritePrivateProfileString(section, key, null, file);
        }

    }
}
