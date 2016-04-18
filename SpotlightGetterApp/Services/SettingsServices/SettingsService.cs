using System;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Template10.Common;
using Template10.Utils;
using Windows.UI.Xaml;

namespace SpotlightGetterApp.Services.SettingsServices
{
    public class SettingsService
    {
        public static SettingsService Instance { get; }

        private readonly ApplicationDataContainer _appSettings = ApplicationData.Current.LocalSettings;
        private readonly IStorageItemAccessList _accessList = StorageApplicationPermissions.FutureAccessList;
        static SettingsService()
        {
            // implement singleton pattern
            Instance = Instance ?? new SettingsService();
        }

        readonly Template10.Services.SettingsService.ISettingsHelper _helper;
        private SettingsService()
        {
            _helper = new Template10.Services.SettingsService.SettingsHelper();
        }

        public StorageFolder SpotlightFolder
        {
            get { return _accessList.GetFolderAsync("SpotlightFolder").GetResults(); }
            set { _accessList.AddOrReplace("SpotlightFolder", value); }
        }

        public StorageFolder SaveFolder
        {
            get { return _accessList.GetFolderAsync("SaveFolder").GetResults(); }
            set { _accessList.AddOrReplace("SaveFolder", value); }
        }

        public bool UseShellBackButton
        {
            get { return _helper.Read<bool>(nameof(UseShellBackButton), true); }
            set
            {
                _helper.Write(nameof(UseShellBackButton), value);
                BootStrapper.Current.NavigationService.Dispatcher.Dispatch(() =>
                {
                    BootStrapper.Current.ShowShellBackButton = value;
                    BootStrapper.Current.UpdateShellBackButton();
                    BootStrapper.Current.NavigationService.Refresh();
                });
            }
        }

        public ApplicationTheme AppTheme
        {
            get
            {
                var theme = ApplicationTheme.Light;
                var value = _helper.Read<string>(nameof(AppTheme), theme.ToString());
                return Enum.TryParse<ApplicationTheme>(value, out theme) ? theme : ApplicationTheme.Dark;
            }
            set
            {
                _helper.Write(nameof(AppTheme), value.ToString());
                (Window.Current.Content as FrameworkElement).RequestedTheme = value.ToElementTheme();
            }
        }

        public TimeSpan CacheMaxDuration
        {
            get { return _helper.Read<TimeSpan>(nameof(CacheMaxDuration), TimeSpan.FromDays(2)); }
            set
            {
                _helper.Write(nameof(CacheMaxDuration), value);
                BootStrapper.Current.CacheMaxDuration = value;
            }
        }
    }
}

