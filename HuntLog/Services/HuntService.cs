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
        Task<IEnumerable<Jakt>> GetHunts();
        Task<Jakt> GetHunt(string id);
        Task Save(Jakt hunt);
    }


    public class HuntService : IHuntService
    {

        private const int _delay = 500;
        private List<Jakt> _hunts;
        public async Task<Jakt> GetHunt(string id)
        {
            var hunts = await GetHunts();
            return hunts.SingleOrDefault(x => x.ID == id);
        }

        public async Task<IEnumerable<Jakt>> GetHunts()
        {
            if (_hunts == null) 
            {
                await Task.Delay(_delay);

                _hunts = FileManager.LoadFromLocalStorage<List<Jakt>>("jakt.xml");

                //_hunts = new List<Hunt>
                //{
                //    new Hunt { ID = "1", Sted = "Jonsvatnet", DatoFra = DateTime.Now },
                //    new Hunt { ID = "2",  Sted = "Bymarka", DatoFra = DateTime.Now.AddDays(-10) },
                //    new Hunt { ID = "3",  Sted = "Levanger", DatoFra = DateTime.Now.AddMonths(-3) }
                //};
            }
            return _hunts.OrderByDescending(x => x.DatoFra);
        }

        public async Task Save(Jakt hunt)
        {
            var itemToReplace = _hunts.SingleOrDefault(x => x.ID == hunt.ID);
            if(itemToReplace != null) 
            {
                _hunts.Remove(itemToReplace);
            }
            _hunts.Add(hunt);

            await Task.CompletedTask;
        }
    }
}
