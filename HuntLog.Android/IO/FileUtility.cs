using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HuntLog.Interfaces;
using Jaktloggen.Droid.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileUtility))]
namespace Jaktloggen.Droid.IO
{
    public class FileUtility : IFileUtility
    {
        public void Save(string filename, string text)
        {
            string filePath = GetFilePath(filename);
            File.WriteAllText(filePath, text);
        }

        public string SaveImage(string filename, byte[] imageData)
        {
            string filePath = GetFilePath(filename);
            File.WriteAllBytes(filePath, imageData);
            return filePath;
        }

        public string Load(string filename)
        {
            string filePath = GetFilePath(filename);
            return File.ReadAllText(filePath);
        }

        public bool Exists(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename)) { return false; }

            string filePath = GetFilePath(filename);
            return File.Exists(filePath);
        }

        public void LogError(string error)
        {
            string filePath = GetFilePath("error.txt");
            var errorlog = File.ReadAllLines(filePath)?.ToList();
            if (errorlog == null)
            {
                errorlog = new List<string>();
            }
            errorlog.Add(error);
            File.WriteAllText(filePath, String.Join(System.Environment.NewLine, errorlog));
        }

        public void Delete(string filename)
        {
            if (Exists(filename))
            {
                string filePath = GetFilePath(filename);
                File.Delete(filePath);
            }
        }

        public DateTime GetLastWriteTime(string filename)
        {
            string filePath = GetFilePath(filename);
            return File.GetLastWriteTime(filePath);
        }

        public string GetFilePath(string filename)
        {
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, filename);
            return filePath;
        }

        public void Copy(string sourcePath, string destinationPath)
        {
            File.Copy(sourcePath, destinationPath);
        }

        public string[] GetAllFiles()
        {
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            return Directory.GetFiles(documentsPath);
        }

        public byte[] ReadAll(string filename)
        {
            string filePath = GetFilePath(filename);
            return File.ReadAllBytes(filePath);
        }
    }
}