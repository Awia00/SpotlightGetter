using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
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
            var folderPicker = new Windows.Storage.Pickers.FolderPicker
            {
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
            };
            folderPicker.FileTypeFilter.Add(".txt");

            Windows.Storage.StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null && folder.Path.Contains(@"Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets"))
            {
                // Application now has read/write access to all contents in the picked folder
                // (including other sub-folder contents)
                Windows.Storage.AccessCache.StorageApplicationPermissions.
                FutureAccessList.AddOrReplace("SpotlightFolder", folder);
                Status = "Everything went fine - we'll take you to the frontpage";
                await Task.Delay(1500);
                NavigationService.Navigate(typeof(Views.MainPage), null);
            }
            else
            {
                Status = "Something went wrong.";
            }
        }

        public Action CopyFilesCommand => new Action(async () =>
        {
            var folder = await Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.GetFolderAsync("PickedFolderToken");
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
        });
        public Action SaveFilesCommand => new Action(async () =>
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker
            {
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
            };
            folderPicker.FileTypeFilter.Add(".txt");

            Windows.Storage.StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                // Application now has read/write access to all contents in the picked folder
                // (including other sub-folder contents)
                Windows.Storage.AccessCache.StorageApplicationPermissions.
                FutureAccessList.AddOrReplace("Savefolder", folder);
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
