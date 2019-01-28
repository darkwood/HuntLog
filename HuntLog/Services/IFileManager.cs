using System;

namespace HuntLog.Services
{
    public interface IFileManager
    {
        void CopyToAppFolder(string file);
        void Delete(string filename);
        bool Exists(string filename);
        DateTime GetLastWriteTime(string filename);
        T LoadFromLocalStorage<T>(string filename, bool loadFromserver = false);
        string SaveImage(string filename, byte[] imageData);
        void Copy(string sourceFile, string destinationFile);
        void SaveToLocalStorage<T>(T objToSerialize, string filename);
    }
}