using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HuntLog.Models;
using HuntLog.Services;

namespace HuntLog.Factories
{
    public interface IHunterFactory
    {

    }

    public class HunterFactory : IHunterFactory
    {
        private readonly IHunterService _hunterService;

        public HunterFactory(IHunterService hunterService)
        {
            _hunterService = hunterService;
        }


    }
}
