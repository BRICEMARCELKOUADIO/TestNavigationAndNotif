using Newtonsoft.Json;
using Plugin.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kola.Infrastructure.Hepler
{
    public static class CrossSettingsHelper
    {
        public static void Remove(string pkey, string pvirtualFile = null)
        {
            CrossSettings.Current.Remove(pkey, pvirtualFile);
        }

        public static void ClearVirtualFile(string pvirtualFile)
        {
            CrossSettings.Current.Clear(pvirtualFile);
        }

        #region Methods Surcharge GetValue
        public static string GetValueOrDefault(string pkey, string defaultValue = "", string pvirtualFile = null)
        {            
            return CrossSettings.Current.GetValueOrDefault(pkey, defaultValue, pvirtualFile);
        }

        public static bool GetValueOrDefault(string pkey, bool defaultValue = false, string pvirtualFile = null)
        {
            return CrossSettings.Current.GetValueOrDefault(pkey, defaultValue, pvirtualFile);
        }

        public static decimal GetValueOrDefault(string pkey, decimal defaultValue = default, string pvirtualFile = null)
        {
            return CrossSettings.Current.GetValueOrDefault(pkey, defaultValue, pvirtualFile);
        }

        public static long GetValueOrDefault(string pkey, long defaultValue = default, string pvirtualFile = null)
        {
            return CrossSettings.Current.GetValueOrDefault(pkey, defaultValue, pvirtualFile);
        }

        public static int GetValueOrDefault(string pkey, int defaultValue = default, string pvirtualFile = null)
        {
            return CrossSettings.Current.GetValueOrDefault(pkey, defaultValue, pvirtualFile);
        }

        public static T GetValueOrDefault<T>(string pkey, string pvirtualFile = null)
        {
            T obj = default;

            var str = CrossSettings.Current.GetValueOrDefault(pkey, "", pvirtualFile);

            if (str == "")
            {
                return obj;
            }

            try
            {
                obj = JsonConvert.DeserializeObject<T>(str);
            }
            catch (Exception ex)
            {
                obj = default;
                Console.WriteLine($"Erreur survenue sur T CrossSettingsHelper.GetValueOrDefault<T> -->  param = {pkey}, {pvirtualFile} - StackTrace -> {ex.Message}");
            }
            return obj;
        }

        public static List<T> GetValueOrDefault<T>(string pkey, string defaultValueNotUsed = "", string pvirtualFile = null)
        {
            List<T> objList = default;
            var str = CrossSettings.Current.GetValueOrDefault(pkey, defaultValueNotUsed, pvirtualFile);
            if (str == "")
            {
                return objList;
            }

            try
            {
                objList = JsonConvert.DeserializeObject<List<T>>(str);
            }
            catch (Exception ex)
            {
                objList = default;
                Console.WriteLine($"Erreur survenue sur List<T> CrossSettingsHelper.GetValueOrDefault<T> -->  param = {pkey}, {pvirtualFile} - StackTrace -> {ex.Message}");
            }
            return objList;
        }

        #endregion


        #region Methods Surcharge AddOrUpdate
        public static bool AddOrUpdateValue(string pkey, string pvalue, string pvirtualFile = null)
        {
            bool ok = CrossSettings.Current.AddOrUpdateValue(pkey, pvalue, pvirtualFile);
            if (!ok)
            {
                Console.WriteLine($"Erreur survenue sur CrossSettingsHelper.AddOrUpdateValue -->  param = {pkey}, {pvalue},{pvirtualFile}");
            }
            return ok;
        }

        public static bool AddOrUpdateValue(string pkey, bool pvalue, string pvirtualFile = null)
        {
            bool ok = CrossSettings.Current.AddOrUpdateValue(pkey, pvalue, pvirtualFile);
            if (!ok)
            {
                Console.WriteLine($"Erreur survenue sur CrossSettingsHelper.AddOrUpdateValue -->  param = {pkey}, {pvalue},{pvirtualFile}");
            }
            return ok;
        }

        public static bool AddOrUpdateValue(string pkey, decimal pvalue, string pvirtualFile = null)
        {
            bool ok = CrossSettings.Current.AddOrUpdateValue(pkey, pvalue, pvirtualFile);
            if (!ok)
            {
                Console.WriteLine($"Erreur survenue sur CrossSettingsHelper.AddOrUpdateValue -->  param = {pkey}, {pvalue},{pvirtualFile}");
            }
            return ok;
        }

        public static bool AddOrUpdateValue(string pkey, long pvalue, string pvirtualFile = null)
        {
            bool ok = CrossSettings.Current.AddOrUpdateValue(pkey, pvalue, pvirtualFile);
            if (!ok)
            {
                Console.WriteLine($"Erreur survenue sur CrossSettingsHelper.AddOrUpdateValue -->  param = {pkey}, {pvalue},{pvirtualFile}");
            }
            return ok;
        }

        public static bool AddOrUpdateValue(string pkey, int pvalue, string pvirtualFile = null)
        {
            bool ok = CrossSettings.Current.AddOrUpdateValue(pkey, pvalue, pvirtualFile);
            if (!ok)
            {
                Console.WriteLine($"Erreur survenue sur CrossSettingsHelper.AddOrUpdateValue -->  param = {pkey}, {pvalue},{pvirtualFile}");
            }
            return ok;
        }
        #endregion
    }
}