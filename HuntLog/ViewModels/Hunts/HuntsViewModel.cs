﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HuntLog.Models;
using HuntLog.Services;

namespace HuntLog.ViewModels.Hunts
{
    public class HuntsViewModel : ViewModelBase
    {
        private readonly IHuntService _huntService;
        private readonly Func<HuntListItemViewModel> _huntListItemViewModelFactory;
        private IEnumerable<HuntListItemViewModel> m_huntListItemViewModel;

        public IEnumerable<HuntListItemViewModel> HuntListItemViewModel
        {
            get { return m_huntListItemViewModel; }
            set { SetProperty(ref m_huntListItemViewModel, value); }
        }

        private bool _dataLoaded;
        public bool DataLoaded
        {
            get => _dataLoaded; set => SetProperty(ref _dataLoaded, value);
        }

        public async Task Initialize()
        {
            await FetchHuntData();
        }

        public HuntsViewModel(IHuntService huntService, Func<HuntListItemViewModel> huntListItemViewModelFactory)
        {
            _huntService = huntService;
            _huntListItemViewModelFactory = huntListItemViewModelFactory;
            Title = "Hunts";
        }

        public async Task FetchHuntData()
        {
            var hunts = await _huntService.GetHunts();
            DataLoaded = true;
            HuntListItemViewModel = hunts.Select(x =>
                {
                    var item = _huntListItemViewModelFactory();
                    item.Initialize(x);
                    return item;
                })
                .ToList();
        }
    }
}
