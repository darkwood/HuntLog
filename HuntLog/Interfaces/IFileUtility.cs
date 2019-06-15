using System;

namespace HuntLog.Interfaces
{
    public interface IFileUtility
    {
        void Save(string filename, string text);
        string SaveImage(string filename, byte[] imageData);
        string Load(string filename);
        string GetFilePath(string filename);
        DateTime GetLastWriteTime(string filename);
        bool Exists(string filename);
        void LogError(string error);
        void Delete(string filename);
        void Copy(string sourceFile, string destinationFile);
        string[] GetAllFiles();
    }
}
