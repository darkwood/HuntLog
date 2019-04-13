using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HuntLog.Models;
using Xamarin.Essentials;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace HuntLog.Services
{
    public interface ISelectService<T>
    {
        Task<IEnumerable<string>> GetItems();
        Task Add(string item);
        Task Delete(string item);
    }

    public class SelectService<T> : ISelectService<T>
    {
        private const int _delay = 0;
        private string _dataFileName = "";
        private readonly IFileManager _fileManager;
        private List<string> _dtos;

        public SelectService(IFileManager fileManager)
        {
            _fileManager = fileManager;

            SetDataFileBasedOnType();
        }


        public async Task Delete(string id)
        {
            _dtos.RemoveAt(_dtos.FindIndex(x => x == id));
            _fileManager.SaveToLocalStorage(_dtos, _dataFileName);
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<string>> GetItems()
        {
            if (_dtos == null)
            {
                await Task.Delay(_delay);
                _dtos = _fileManager.LoadFromLocalStorage<List<string>>(_dataFileName);
            }
            return _dtos;
        }

        public async Task Add(string item)
        {
            if(_dtos.Exists(x => x == item)) { return; }

            _dtos.Add(item);
            _fileManager.SaveToLocalStorage(_dtos, _dataFileName);

            await Task.CompletedTask;
        }

        private void SetDataFileBasedOnType()
        {
            object p = typeof(T);
            if (p == typeof(Art))
            {
                _dataFileName = "selectedartids.xml";
            }
            else if (p == typeof(LoggType))
            {
                _dataFileName = "selectedloggids.json";
            }
            else
            {
                throw new NotImplementedException(typeof(T).ToString() + " sitt filnavn er ikke implementert.");
            }

        }
    }
}
