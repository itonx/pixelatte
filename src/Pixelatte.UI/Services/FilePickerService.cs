using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace Pixelatte.UI.Services
{
    internal class FilePickerService
    {
        public async Task<StorageFile> PickFile()
        {
            FileOpenPicker fileOpenPicker = new()
            {
                ViewMode = PickerViewMode.Thumbnail,
                FileTypeFilter = { ".jpg", ".jpeg", ".png", ".gif" },
            };

            nint windowHandle = WindowNative.GetWindowHandle(App.m_window);
            InitializeWithWindow.Initialize(fileOpenPicker, windowHandle);

            return await fileOpenPicker.PickSingleFileAsync();
        }
    }
}
