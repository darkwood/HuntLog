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
    public interface IHuntService
    {
        Task<IEnumerable<Jakt>> GetItems();
        Task<Jakt> Get(string id);
        Task Save(Jakt hunt);
        Task Delete(string id);
    }

    public class HuntService : IHuntService
    {
        private const int _delay = 0;
        private const string _dataFileName = "jakt.json";
        private readonly IFileManager _fileManager;
        private List<Jakt> _dtos;

        public HuntService(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        public async Task Delete(string id)
        {
            _dtos.RemoveAt(_dtos.FindIndex(x => x.ID == id));
            _fileManager.SaveToLocalStorage(_dtos, _dataFileName);
            await Task.CompletedTask;
        }

        public async Task<Jakt> Get(string id)
        {
            var hunts = await GetItems();
            return hunts.SingleOrDefault(x => x.ID == id);
        }

        public async Task<IEnumerable<Jakt>> GetItems()
        {
            if (_dtos == null)
            {
                await Task.Delay(_delay);

                _dtos = _fileManager.LoadFromLocalStorage<List<Jakt>>(_dataFileName);

            }
            return _dtos;
        }

        public async Task Save(Jakt hunt)
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
    }
}
