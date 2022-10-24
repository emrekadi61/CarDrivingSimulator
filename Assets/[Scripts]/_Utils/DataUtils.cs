using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace Utils.Data
{
    public static class DataUtils
    {
        public static void Save(this string data, string where)
        {
            string folder = Application.persistentDataPath + "/local/";
            string filePath = folder + where + ".json";
            string[] f = where.Split('/');
            for (int i = 0; i < f.Length - 1; i++) folder += f[i] + "/";

            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            if (!File.Exists(filePath)) File.Create(filePath).Close();

            File.WriteAllText(filePath, data);
        }

        public static string Read(this string path)
        {
            string folder = Application.persistentDataPath + "/local/";
            string filePath = folder + path + ".json";
            string content = null;

            if (!File.Exists(filePath)) return null;

            StreamReader sr = new StreamReader(filePath);
            content = sr.ReadToEnd();
            sr.Close();

            return content;
        }

        public static string ReadFromResources(this string path)
        {
            TextAsset ta = Resources.Load<TextAsset>(path);
            return ta?.text;
        }

        public static string Get(this string path)
        {
            string data = path.Read();
            if (string.IsNullOrEmpty(data))
            {
                data = path.ReadFromResources();
                data.Save(path);
            }
            return data;
        }

        public static void DeleteAllData(UnityAction onComplete = null)
        {
            string folderPath = Application.persistentDataPath + "/local/";
            if (Directory.Exists(folderPath))
            {
                DeleteDatas(folderPath);
                Directory.CreateDirectory(folderPath);
            }
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            onComplete?.Invoke();
        }

        private static void DeleteDatas(string path)
        {
            string[] files = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);

            for (int i = 0; i < files.Length; i++)
            {
                File.SetAttributes(files[i], FileAttributes.Normal);
                File.Delete(files[i]);
            }

            for (int i = 0; i < dirs.Length; i++)
            {
                DeleteDatas(dirs[i]);
            }

            Directory.Delete(path, false);
        }
    }
}