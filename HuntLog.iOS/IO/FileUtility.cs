using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HuntLog.iOS.Renderers.IO;
using HuntLog.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileUtility))]
namespace HuntLog.iOS.Renderers.IO
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
            var filePath = GetFilePath(filename);
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
            if(filename.IndexOf('/') > -1) { return filename; }

            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, filename);
            return filePath;
        }

        public void Copy(string sourceFile, string destinationFile)
        {
            var dest = GetFilePath(destinationFile);
            new FileInfo(dest).Directory.Create();
            File.Copy(GetFilePath(sourceFile), dest);
        }

        public string[] GetAllFiles()
        {
            var documentsPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return Directory.GetFiles(documentsPath);
        }
    }
}