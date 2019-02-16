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
    public interface IHunterService 
    {
        Task<IEnumerable<Jeger>> GetItems();
        Task<Jeger> Get(string id);
        Task Save(Jeger hunt);
        Task Delete(string id);
    }

    public class HunterService : IHunterService
    {
        private const int _delay = 0;
        private const string _dataFileName = "jegere.xml";
        private readonly IFileManager _fileManager;
        private List<Jeger> _dtos;

        public HunterService(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        public async Task Delete(string id)
        {
            _dtos.RemoveAt(_dtos.FindIndex(x => x.ID == id));
            _fileManager.SaveToLocalStorage(_dtos, _dataFileName);
            await Task.CompletedTask;
        }

        public async Task<Jeger> Get(string id)
        {
            var hunts = await GetItems();
            return hunts.SingleOrDefault(x => x.ID == id);
        }

        public async Task<IEnumerable<Jeger>> GetItems()
        {
            if (_dtos == null) 
            {
                await Task.Delay(_delay);

                _dtos = _fileManager.LoadFromLocalStorage<List<Jeger>>(_dataFileName);

            }
            return _dtos;
        }

        public async Task Save(Jeger hunt)
        {
            var itemToReplace = _dtos.SingleOrDefault(x => x.ID == hunt.ID);
            if(itemToReplace != null) 
            {
                _dtos.Remove(itemToReplace);
            }
            _dtos.Add(hunt);

            _fileManager.SaveToLocalStorage(_dtos, _dataFileName);

            await Task.CompletedTask;
        }
    }
}
