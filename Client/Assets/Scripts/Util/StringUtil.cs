using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class StringUtil
{
    private static StringBuilder m_stringBuilder = new StringBuilder();

    public static string[] Split(string input, string separator, StringSplitOptions options = System.StringSplitOptions.None)
    {

        if (string.IsNullOrEmpty(input))
        {
            return null;
        }

        if (string.IsNullOrEmpty(separator))
        {
            return null;
        }

        char[] charSeparator = separator.ToCharArray();
        if (charSeparator == null || charSeparator.Length == 0)
        {
            return null;
        }

        return input.Split(charSeparator, options);
    }

    public static string[] Split(string input, string[] separator, StringSplitOptions options = System.StringSplitOptions.None)
    {

        if (string.IsNullOrEmpty(input))
        {
            return null;
        }

        if (separator == null || separator.Length == 0)
        {
            return null;
        }

        return input.Split(separator, options);
    }

    public static int Split(string input, string separator, ref List<string> output)
    {
        string[] split = Split(input, separator);
        if (split == null)
        {
            return 0;
        }

        foreach (string s in split)
        {
            output.Add(s);
        }

        return output.Count;
    }

    public static int Explode(string input, string separator, ref List<string> output)
    {
        string[] split = Split(input, separator, StringSplitOptions.RemoveEmptyEntries);
        if (split == null)
        {
            return 0;
        }

        foreach (string s in split)
        {
            output.Add(s);
        }

        return output.Count;
    }

    public static string ToUTF8String(string input)
    {

        byte[] unicodeBytes = Encoding.UTF8.GetBytes(input);
        return Encoding.UTF8.GetString(unicodeBytes);
    }

    public static string Format(string format, params object[] args)
    {
        string output = format;
        if (args != null && args.Length != 0)
        {
            m_stringBuilder.Length = 0;
            output = m_stringBuilder.AppendFormat(format, args).ToString();
        }

        return output;
    }

    public static string ToUTF8Format(string format, params object[] args)
    {

        return ToUTF8String(Format(format, args));
    }

    public static bool MatchRegex(string input, string regex)
    {
        return Regex.IsMatch(input, regex);
    }

    public static string ReplaceRegex(string input, string replacement, string regex)
    {
        return Regex.Replace(input, replacement, replacement);
    }

    public static bool HasEmail(string email)
    {
        string emailChecker = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.([a-zA-Z]{2}|com|org|net|edu|gov|biz|info|mil|mobi|name|aero|asia|jobs|museum)$";
        return MatchRegex(email, emailChecker);
    }

    public static bool HasID(string id)
    {
        string idChecker = "([^ .,'\";:`]+)";
        return MatchRegex(id, idChecker);
    }

    public static string EscapeURL(string input, System.Text.Encoding e)
    {
        return UnityEngine.WWW.EscapeURL(input, e);
    }

    public static string UTF8EscapeURL(string input)
    {
        return EscapeURL(input, System.Text.Encoding.UTF8);
    }

    public static string StringFromStackTrace(System.DateTime dt, int frameDepth = 1)
    {
        System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);
        System.Diagnostics.StackFrame sf = st.GetFrame(frameDepth + 1);

        return StringUtil.Format("time={0}, method={1}, file={2}, line={3}", dt.ToString(), sf.GetMethod().Name, System.IO.Path.GetFileName(sf.GetFileName()), sf.GetFileLineNumber()).ToString();
    }

    public static string GetPath(string fullPath, uint cuttingDepth = 1)
    {
        string output = string.Empty;
        string[] folders = StringUtil.Split(fullPath, "/");
        if (cuttingDepth > folders.Length) return output;
        for (int i = 0; i < folders.Length - cuttingDepth; ++i)
        {
            output = (output == string.Empty) ? folders[i] : Format("{0}/{1}", output, folders[i]);
        }
        return output;
    }

    public static string RandomString(int length, bool lowerCase = false)
    {

        System.Random random = new System.Random();

        m_stringBuilder.Length = 0;
        char ch;
        for (int i = 0; i < length; i++)
        {
            ch = System.Convert.ToChar(System.Convert.ToInt32(System.Math.Floor(26 * random.NextDouble() + 65)));
            m_stringBuilder.Append(ch);
        }

        if (lowerCase)
        {
            return m_stringBuilder.ToString().ToLower();
        }

        return m_stringBuilder.ToString();
    }

    public static string RandomPassword()
    {

        System.Random random = new System.Random();

        m_stringBuilder.Length = 0;
        m_stringBuilder.Append(RandomString(4, true));
        m_stringBuilder.Append(random.Next(1000, 9999));
        m_stringBuilder.Append(RandomString(2, false));
        return m_stringBuilder.ToString();
    }

    public static string StripCleanString(string src)
    {
        if (!string.IsNullOrEmpty(src))
        {
            string[] dst = src.Split('\n');
            return dst[0].Trim();
        }

        return string.Empty;
    }

    public static string IntToCommaString(int value)
    {
        return string.Format("{0:#,0}", value);
    }

    public static string FloatTimeToStringTime(float time)
    {
        //System.Text.StringBuilder sb = new System.Text.StringBuilder();
        m_stringBuilder.Length = 0;
        int min = (int)Mathf.Floor(time * 0.016666667f);
        if (min < 10)
        {
            m_stringBuilder.Append("0");
            m_stringBuilder.Append(min);
        }
        else
        {
            m_stringBuilder.Append(min);
        }

        m_stringBuilder.Append(":");

        int sec = (int)Mathf.Floor(time % 60.0f);
        if (sec < 10)
        {
            m_stringBuilder.Append("0");
            m_stringBuilder.Append(sec);
        }
        else
        {
            m_stringBuilder.Append(sec);
        }
        return m_stringBuilder.ToString();
    }

    public static string FormatNumberString(int number)
    {
        string numberString = number.ToString();
        string returnString = numberString;

        if (numberString.Length > 3)
        {
            int totalCommas = Mathf.CeilToInt(numberString.Length / 3.0F) - 1;

            returnString = "";
            int counter = 0;
            for (int charIdx = numberString.Length - 1; charIdx >= 0; charIdx--)
            {
                returnString = returnString.Insert(0, numberString[charIdx].ToString());
                if (((counter % 3) == 2) && (totalCommas > 0))
                {
                    returnString = returnString.Insert(0, ",");
                    totalCommas--;
                }
                counter++;
            }
        }

        return returnString;
    }

    /// <summary>
    /// byte size를 KB, MB, GB로 변환
    /// </summary>
    /// <param name="fileSize">byte 단위의 파일 크기</param>
    /// <returns></returns>
    public static string HumaneFileSizeString(int fileSize)
    {
        string[] suffix = { "bytes", "kB", "MB", "GB", "TB" };
        if (fileSize == 0)
            return "0 " + suffix[0];

        int sufIndex = (int)Math.Floor(Math.Log(fileSize, 1024));
        double normalizedSize = fileSize / Math.Pow(1024, sufIndex);
        return string.Format("{0:0.##} {1}", normalizedSize, suffix[sufIndex]);
    }
}
