using System;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using SpotlightGetterApp.Services.SettingsServices;
using Windows.ApplicationModel.Activation;
using Template10.Mvvm;
using Template10.Common;
using System.Linq;

namespace SpotlightGetterApp
{
    sealed partial class App : Template10.Common.BootStrapper
    {
        public App()
        {
            InitializeComponent();
            SplashFactory = (e) => new Views.Splash(e);

            #region App settings

            var _settings = SettingsService.Instance;
            RequestedTheme = _settings.AppTheme;
            CacheMaxDuration = _settings.CacheMaxDuration;
            ShowShellBackButton = _settings.UseShellBackButton;

            #endregion
        }

        public override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            await Task.CompletedTask;
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            var isSetup = await Task.Run(() => SettingsService.Instance.SpotlightFolder == null || SettingsService.Instance.SaveFolder == null);
            // long-running startup tasks go here
            NavigationService.Navigate(isSetup
                ? typeof (Views.IntroductionPage)
                : typeof (Views.MainPage));
            await Task.CompletedTask;
        }
    }
}

