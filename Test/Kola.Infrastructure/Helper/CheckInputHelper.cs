using System;
using System.Text.RegularExpressions;

namespace Kola.Infrastructure.Hepler
{
    public static class CheckInputHelper
    {
        public static bool IsValidEmail(string pemail)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(pemail);
                return addr.Address == pemail;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool VerifyInputLength(string str, int max)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            string s = Regex.Replace(str, "[^a-zA-Z0-9_@]+", "", RegexOptions.Compiled);

            return s.Length >= max;
        }

        public static string MakeFirstCharToUpper(this string str)
        {
            string result = "";
            if (string.IsNullOrEmpty(str) || str?.Length == 0)
            {
                result = "";
            }
            else if (str?.Length == 1)
            {
                result = str.Substring(0, 1).ToUpperInvariant().ToCharArray()[0].ToString();
            }
            else
            {
                string[] list = str.Split(' ');

                foreach (var line in list)
                {
                    if (line.Length == 0)
                        continue;
                    if (line.Length == 1)
                        result += $" {line.Substring(0, 1).ToUpperInvariant().ToCharArray()[0]}";
                    else
                        result += $" {line.Substring(0, 1).ToUpperInvariant().ToCharArray()[0].ToString() + line.Substring(1)}";
                }
            }

            return result;
        }

        public static string MakeToUpper(this string str)
        {
            if (string.IsNullOrEmpty(str) || str?.Length == 0)
                return "";

            return str.ToUpperInvariant();
        }

        public static bool CompareDateGreaterThan(this DateTime d1, DateTime d2)
        {
            // d1 > d2
            if (d1.Year < d2.Year)
                return false;
            if (d1.Month < d2.Month)
                return false;
            if (d1.Day < d2.Day)
                return false;

            return true;
        }

        public static bool CompareDateLessThan(this DateTime d1, DateTime d2)
        {
            //d1 < d2
            if (d1.Year > d2.Year)
                return false;
            if (d1.Month > d2.Month)
                return false;
            if (d1.Day > d2.Day)
                return false;

            return true;
        }

        public static bool CompareDateEqualThan(this DateTime d1, DateTime d2)
        {
            int ok = 0;
            if (d1.Year == d2.Year)
                ok += 1 ;
            if (d1.Month == d2.Month)
                ok += 1;
            if (d1.Day == d2.Day)
                ok += 1;

            return ok == 3;
        }

    }
}