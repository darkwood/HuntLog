using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Xamarin.Forms;
using HuntLog.Interfaces;
using System.Xml;

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

    public class FileManager : IFileManager
    {
        private readonly IFileUtility _fileUtility;

        public FileManager(IFileUtility fileUtility)
        {
            _fileUtility = fileUtility;
        }

        public void SaveToLocalStorage<T>(T objToSerialize, string filename)
        {
            if (filename.EndsWith(".json", StringComparison.CurrentCultureIgnoreCase))
            {
                var jsonString = JsonConvert.SerializeObject(objToSerialize);
                _fileUtility.Save(filename, jsonString);
                return;
            }
            else
            {
                XmlSerializer xmlSerializer = new XmlSerializer(objToSerialize.GetType());

                using (StringWriter textWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(textWriter, objToSerialize);
                    _fileUtility.Save(filename, textWriter.ToString());
                }
            }
        }

        public T LoadFromLocalStorage<T>(string filename, bool loadFromserver = false)
        {
            var localObj = (T)Activator.CreateInstance(typeof(T));
            // 1 read json
            if (filename.EndsWith(".json", StringComparison.CurrentCultureIgnoreCase) && Exists(filename))
            {
                localObj = JsonConvert.DeserializeObject<T>(_fileUtility.Load(filename));
            }
            else
            {
                var xmlFilename = filename.Replace(".json", ".xml");
                if (Exists(xmlFilename))
                {
                    var xmlString = _fileUtility.Load(xmlFilename);
                    xmlString = xmlString.Replace("<int>", "<string>");
                    xmlString = xmlString.Replace("</int>", "</string>");

                    using (var reader = new StringReader(xmlString))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(T));
                        localObj = (T)serializer.Deserialize(reader);
                        SaveToLocalStorage(localObj, filename); //Save to json format
                    }
                }
            }
            return localObj;
        }

        public bool Exists(string filename)
        {
            return _fileUtility.Exists(filename);
        }

        public DateTime GetLastWriteTime(string filename)
        {
            return _fileUtility.GetLastWriteTime(filename);
        }

        public void CopyToAppFolder(string file)
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".Xml." + file);

            using (var reader = new StreamReader(stream))
            {
                _fileUtility.Save(file, reader.ReadToEnd());
            }
        }
        public void Delete(string filename)
        {
            _fileUtility.Delete(filename);
        }

        public string SaveImage(string filename, byte[] imageData)
        {
            return _fileUtility.SaveImage(filename, imageData);
        }

        public void Copy(string sourceFile, string destinationFile)
        => _fileUtility.Copy(sourceFile, destinationFile);
    }
}