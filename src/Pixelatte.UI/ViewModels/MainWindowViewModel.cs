using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Pixelatte.UI.Models;
using Pixelatte.UI.Services;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace Pixelatte.UI.ViewModels
{
    internal partial class MainWindowViewModel : ObservableObject
    {
        private readonly PixelatteManager _pixelatteClient;
        private readonly FilePickerService _filePickerService;

        [ObservableProperty]
        ObservableCollection<string> _tags = new ObservableCollection<string>();
        [ObservableProperty]
        private bool _showContent;
        [ObservableProperty]
        private string _selectedImagePath;
        [ObservableProperty]
        private BitmapImage _selectedImage;
        [ObservableProperty]
        private bool _applyGrayscale;
        [ObservableProperty]
        private BitmapImage _grayscaleImage;
        [ObservableProperty]
        private bool _applyBasicPixelOperation;
        [ObservableProperty]
        private BitmapImage _basicPixelOperationImage;
        [ObservableProperty]
        private string _selectedOperation;
        [ObservableProperty]
        private int _operationValue;

        public MainWindowViewModel()
        {
            _pixelatteClient = new PixelatteManager("http://127.0.0.1:8000");
            _filePickerService = new FilePickerService();
            SelectedOperation = "Add";
        }

        [RelayCommand]
        private async Task LoadImageAsync()
        {
            StorageFile file = await _filePickerService.PickFile();

            if (file != null)
            {
                Tags.Clear();
                SelectedImagePath = file.Path;
                ImageDTO image = await _pixelatteClient.GetImageAsync($"open?img_path={file.Path}");
                SelectedImage = image.Image;
                Tags.Add($"{image.Width}x{image.Height}");
                Tags.Add(Path.GetExtension(file.Path));
                ShowContent = true;
            }
        }

        async partial void OnApplyGrayscaleChanged(bool value)
        {
            if (value && _grayscaleImage == null)
            {
                ImageDTO image = await _pixelatteClient.GetImageAsync($"grayscale?img_path={SelectedImagePath}");
                GrayscaleImage = image.Image;
            }
        }

        partial void OnApplyBasicPixelOperationChanged(bool value)
        {
            if (value && _basicPixelOperationImage == null)
            {
                BasicPixelOperationImage = _selectedImage;
            }
            else
            {
                BasicPixelOperationImage = null;
            }
        }

        async partial void OnSelectedOperationChanged(string value)
        {
            if (!ApplyBasicPixelOperation) return;
            ImageDTO image = await _pixelatteClient.GetImageAsync($"image?img_path={SelectedImagePath}&operation={SelectedOperation}&value={OperationValue}");
            BasicPixelOperationImage = image.Image;
        }

        async partial void OnOperationValueChanged(int value)
        {
            if (!ApplyBasicPixelOperation) return;
            ImageDTO image = await _pixelatteClient.GetImageAsync($"image?img_path={SelectedImagePath}&operation={SelectedOperation}&value={OperationValue}");
            BasicPixelOperationImage = image.Image;
        }
    }
}
