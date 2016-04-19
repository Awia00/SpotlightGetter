using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using SpotlightGetterApp.Services.SettingsServices;
using Template10.Mvvm;

namespace SpotlightGetterApp.ViewModels
{
    public class IntroductionPageViewModel : ViewModelBase
    {
        public IntroductionPageViewModel()
        {

        }

        private string _status = "";

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                base.RaisePropertyChanged();
            }
        }

        #region Actions

        public async void PickSpotlightFolder()
        {
            var folderPicker = new FolderPicker()
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            folderPicker.FileTypeFilter.Add(".txt");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null && folder.Path.Contains(@"AppData\Local\Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets"))
            {
                // Application now has read/write access to all contents in the picked folder
                // (including other sub-folder contents)
                SettingsService.Instance.SpotlightFolder = folder;
                Status = "Everything went fine - we'll take you to the frontpage";
                await Task.Delay(1500);
                NavigationService.Navigate(typeof(Views.MainPage));
            }
            else
            {
                Status = "Something went wrong.";
            }
        }

        public async void CopyFilesCommand()
        {
            var folder = await SettingsService.Instance.GetSpotlightFolder();
            var files = await folder.GetFilesAsync();
            int i = 1;
            foreach (var storageFile in files)
            {
                try
                {

                    var newFile = await storageFile.CopyAsync(ApplicationData.Current.TemporaryFolder, "picture.png", NameCollisionOption.GenerateUniqueName);

                    var properties = await newFile.Properties.GetImagePropertiesAsync();

                    if (properties.Width < 1920 || properties.Height < 1080)
                    {
                        await newFile.DeleteAsync();
                    }
                    else
                    {
                        var title = properties.Title;
                        if (string.IsNullOrEmpty(title))
                        {
                            title = $"picture{i}";
                            i++;
                        }
                        await newFile.RenameAsync($"{title}.png", NameCollisionOption.GenerateUniqueName);
                    }
                }
                catch (Exception)
                {

                }
            }
            SaveFilesCommand.Invoke();
        }

        public Action SaveFilesCommand => new Action(async () =>
        {
            var folderPicker = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            folderPicker.FileTypeFilter.Add(".txt");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                // Application now has read/write access to all contents in the picked folder
                // (including other sub-folder contents)
                SettingsService.Instance.SaveFolder = folder;
            }
            else
            {
            }

            var files = await ApplicationData.Current.TemporaryFolder.GetFilesAsync();
            foreach (var storageFile in files)
            {
                try
                {
                    await storageFile.CopyAsync(folder, storageFile.Name, NameCollisionOption.FailIfExists);
                }
                catch (Exception)
                {

                }
                finally
                {
                    await storageFile.DeleteAsync();
                }
            }
        });
        #endregion Actions
    }
}
