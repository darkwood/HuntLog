using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Xamarin.Forms;
using HuntLog.Interfaces;

namespace HuntLog.Services
{
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
            if (filename.EndsWith(".json", StringComparison.CurrentCultureIgnoreCase))
            {
                var jsonString = _fileUtility.Load(filename);

                try
                {
                    localObj = JsonConvert.DeserializeObject<T>(jsonString);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                    //Utils.LogError(ex);
                }
            }
            else
            {
                //try to read from legacy xml
                var xmlFilename = filename.Replace(".json", ".xml");
                var localFileExists = Exists(xmlFilename);

                if (localFileExists)
                {
                    var xmlString = _fileUtility.Load(xmlFilename);
                    try
                    {
                        using (var reader = new StringReader(xmlString))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(T));
                            localObj = (T)serializer.Deserialize(reader);
                            SaveToLocalStorage(localObj, filename); //Save to json format
                        }
                    }
                    catch (Exception ex)
                    {
                        //Utility.LogError(ex);
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