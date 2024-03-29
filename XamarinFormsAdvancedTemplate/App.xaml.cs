﻿using AppHosting.Abstractions.Interfaces;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using XamarinFormsAdvancedTemplate.Services.Utils.Application;
using XamarinFormsAdvancedTemplate.Views.Tabbed;
using Application = Xamarin.Forms.Application;

namespace XamarinFormsAdvancedTemplate
{
    public partial class App : Application
    {
        private readonly IApplicationService _application;
        private readonly IAppHostLifetime _appHostLifetime;

        public App(IApplicationService application,
                   IAppHostLifetime appHostLifetime)
        {

            _application = application;
            _appHostLifetime = appHostLifetime;
        }

        protected override void OnStart()
        {
            InitializeComponent();

            Current
                .On<Android>()
                .UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);

            _application.InitializeApplication<AppTabbedPage>();
        }

        protected override void OnResume()
        {
            _appHostLifetime.NotifyResuming();
        }

        protected override void OnSleep()
        {
            _appHostLifetime.NotifySleeping();
        }
    }
}