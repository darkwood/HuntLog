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
    public interface IBaseService<T>
    {
        Task<IEnumerable<T>> GetItems();
        Task<T> Get(string id);
        Task Save(T hunt);
        Task Delete(string id);
    }

    public class BaseService<T> : IBaseService<T> where T : BaseDto
    {
        private const int _delay = 0;
        private string _dataFileName = "logg.json";
        private readonly IFileManager _fileManager;
        private List<T> _dtos;

        public BaseService(IFileManager fileManager)
        {
            _fileManager = fileManager;

            SetDataFileBasedOnType();
        }


        public async Task Delete(string id)
        {
            _dtos.RemoveAt(_dtos.FindIndex(x => x.ID == id));
            _fileManager.SaveToLocalStorage(_dtos, _dataFileName);
            await Task.CompletedTask;
        }

        public async Task<T> Get(string id)
        {
            var hunts = await GetItems();
            return hunts.SingleOrDefault(x => x.ID == id);
        }

        public async Task<IEnumerable<T>> GetItems()
        {
            if (_dtos == null)
            {
                await Task.Delay(_delay);

                _dtos = _fileManager.LoadFromLocalStorage<List<T>>(_dataFileName);

            }
            return _dtos;
        }

        public async Task Save(T hunt)
        {
            var itemToReplace = _dtos.SingleOrDefault(x => x.ID == hunt.ID);
            if (itemToReplace != null)
            {
                _dtos.Remove(itemToReplace);
            }
            _dtos.Add(hunt);

            _fileManager.SaveToLocalStorage(_dtos, _dataFileName);

            await Task.CompletedTask;
        }

        private void SetDataFileBasedOnType()
        {
            object p = typeof(T);
            if (p == typeof(Art))
            {
                _dataFileName = "arter.xml";
            }
            else if (p == typeof(Jakt))
            {
                _dataFileName = "jakt.json";
            }
            else if (p == typeof(Logg))
            {
                _dataFileName = "logger.json";
            }
            else if (p == typeof(Jeger))
            {
                _dataFileName = "jegere.json";
            }
            else if (p == typeof(Dog))
            {
                _dataFileName = "dogs.json";
            }
            else if (p == typeof(ArtGroup))
            {
                _dataFileName = "artgroup.xml";
            }
            else if (p == typeof(LoggType))
            {
                _dataFileName = "loggtyper.xml";
            }
            else
            {
                throw new NotImplementedException(typeof(T).ToString() + " sitt filnavn er ikke implementert.");
            }
        }
    }
}
