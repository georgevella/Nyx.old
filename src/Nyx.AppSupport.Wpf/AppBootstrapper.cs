﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Nyx.Composition;
using Nyx.Presentation;

namespace Nyx.AppSupport.Wpf
{
    public class AppBootstrapper
    {
        private readonly Application _app;
        private readonly ViewResolver _viewResolver;
        private IContainer _container;


        public AppBootstrapper(Application app)
        {
            _app = app;
            _viewResolver = new ViewResolver();
        }

        public void Start<TStartViewModel>() where TStartViewModel : IViewModel
        {
            var navigator = _container.Get<INavigator>();
            navigator.NavigateTo<TStartViewModel>();
        }

        /// <exception cref="InvalidOperationException">Configuration failed</exception>
        public void Setup(Action<INyxApplication> configAction)
        {
            if (configAction == null)
            {
                throw new ArgumentNullException(nameof(configAction));
            }

            _container = ContainerFactory.Setup(containerConfiguration =>
            {
                containerConfiguration.Register<IViewResolver>().Using(_viewResolver);
                containerConfiguration.Register<INavigator>().UsingConcreteType<DefaultNavigator>();

                configAction(new NyxApplication(_app, _viewResolver, containerConfiguration));

                foreach (var pair in _viewResolver)
                {
                    containerConfiguration.Register(pair.Key);
                    containerConfiguration.Register(pair.Value);

                }
            });
        }


    }
}