﻿using System;
using System.Collections.Generic;
using Autofac;
using HuntLog.ViewModels;
using Xamarin.Forms;
using LightInject;

namespace HuntLog.Factories
{
    public class ViewFactory : IViewFactory
    {
        private readonly IDictionary<Type, Type> _map = new Dictionary<Type, Type>();
        private readonly IServiceFactory _serviceFactory;

        public ViewFactory(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        public void Register<TViewModel, TView>()
            where TViewModel : class, IViewModel
            where TView : Page
        {
            _map[typeof(TViewModel)] = typeof(TView);
        }

        public Page Resolve<TViewModel>(Action<TViewModel> setStateAction = null) where TViewModel : class, IViewModel
        {
            TViewModel viewModel;
            return Resolve<TViewModel>(out viewModel, setStateAction);
        }

        public Page Resolve<TViewModel>(out TViewModel viewModel, Action<TViewModel> setStateAction = null)
            where TViewModel : class, IViewModel
        {
            viewModel = _serviceFactory.GetInstance<TViewModel>();

            var viewType = _map[typeof(TViewModel)];
            var view = _serviceFactory.GetInstance(viewType) as Page;

            view.BindingContext = viewModel;
            return view;
        }

        public Page Resolve<TViewModel>(TViewModel viewModel)
            where TViewModel : class, IViewModel
        {
            var viewType = _map[typeof(TViewModel)];
            var view = _serviceFactory.GetInstance(viewType) as Page;
            view.BindingContext = viewModel;
            return view;
        }

    }
}
