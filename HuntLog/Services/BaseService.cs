using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HuntLog.Models;
using Microsoft.AppCenter.Analytics;

namespace HuntLog.Services
{
    public interface IBaseService<T>
    {
        Task<IEnumerable<T>> GetItems(bool forceRefresh = false);
        Task<T> Get(string id);
        Task Save(T item);
        Task Delete(string id);
        Task DeleteAll();
    }

    public class BaseService<T> : IBaseService<T> where T : BaseDto
    {
        private const int _delay = 0;
        private string _dataFileName = "";
        private readonly IFileManager _fileManager;
        private List<T> _dtos;

        public BaseService(IFileManager fileManager)
        {
            _fileManager = fileManager;

            SetDataFileBasedOnType();
        }


        public async Task Delete(string id)
        {
            await GetItems();
            _dtos.RemoveAt(_dtos.FindIndex(x => x.ID == id));
            _fileManager.SaveToLocalStorage(_dtos, _dataFileName);
            await Task.CompletedTask;
        }


        public async Task DeleteAll()
        {
            _fileManager.Delete(_dataFileName);
            _fileManager.Delete(_dataFileName.Replace(".json", ".xml"));
            _dtos = null;
            await Task.CompletedTask;
        }

        public async Task<T> Get(string id)
        {
            var hunts = await GetItems();
            return hunts.SingleOrDefault(x => x.ID == id);
        }

        public async Task<IEnumerable<T>> GetItems(bool forceRefresh = false)
        {
            if (_dtos == null || forceRefresh)
            {
                await Task.Delay(_delay);

                _dtos = _fileManager.LoadFromLocalStorage<List<T>>(_dataFileName);

            }
            return _dtos.OrderByDescending(o => o.Created);
        }

        public async Task Save(T item)
        {
            await GetItems();
            var itemToReplace = _dtos.SingleOrDefault(x => x.ID == item.ID);
            if (itemToReplace != null)
            {
                _dtos.Remove(itemToReplace);
                item.Changed = DateTime.Now;
                Analytics.TrackEvent("Item changed", new Dictionary<string, string> {
                    { "Type", item.GetType().Name }
                });
            }
            else 
            {
                item.Created = DateTime.Now;
                Analytics.TrackEvent("Item created", new Dictionary<string, string> {
                    { "Type", item.GetType().Name }
                });
            }
            _dtos.Add(item);

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
            else if (p == typeof(LoggTypeGroup))
            {
                _dataFileName = "loggtypegroup.xml";
            }
            else
            {
                throw new NotImplementedException(typeof(T).ToString() + " sitt filnavn er ikke implementert.");
            }

        }

    }
}
