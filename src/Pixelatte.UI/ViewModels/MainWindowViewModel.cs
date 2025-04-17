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
        [ObservableProperty]
        private bool _isLoading;

        public MainWindowViewModel()
        {
            _pixelatteClient = new PixelatteManager("http://127.0.0.1:8000");
            _filePickerService = new FilePickerService();
            SelectedOperation = "Add";
        }

        [RelayCommand]
        private async Task LoadImageAsync()
        {
            try
            {
                StorageFile file = await _filePickerService.PickFile();

                if (file != null)
                {
                    Tags.Clear();
                    SelectedImagePath = file.Path;
                    IsLoading = true;
                    ImageDTO image = await _pixelatteClient.GetImageAsync($"open?img_path={file.Path}");

                    if (image.Image == null) return;

                    SelectedImage = image.Image;
                    Tags.Add($"{image.Width}x{image.Height}");
                    Tags.Add(Path.GetExtension(file.Path));
                    ShowContent = true;
                }
            }
            finally
            {
                IsLoading = false;
                if (!ShowContent) SelectedImagePath = string.Empty;
            }
        }

        async partial void OnApplyGrayscaleChanged(bool value)
        {
            if (value && _grayscaleImage == null)
            {
                IsLoading = true;
                ImageDTO image = await _pixelatteClient.GetImageAsync($"grayscale?img_path={SelectedImagePath}");
                GrayscaleImage = image.Image;
                IsLoading = false;
            }
        }

        async partial void OnApplyBasicPixelOperationChanged(bool value)
        {
            if (value && _basicPixelOperationImage == null)
            {
                if (OperationValue == 0)
                    BasicPixelOperationImage = _selectedImage;
                else
                    await LoadBasicPixelOperationImage();
            }
            else
            {
                BasicPixelOperationImage = null;
            }
        }

        async partial void OnSelectedOperationChanged(string value)
        {
            if (!ApplyBasicPixelOperation) return;
            await LoadBasicPixelOperationImage();
        }

        async partial void OnOperationValueChanged(int value)
        {
            if (!ApplyBasicPixelOperation) return;
            await LoadBasicPixelOperationImage();
        }

        private async Task LoadBasicPixelOperationImage()
        {
            IsLoading = true;
            ImageDTO image = await _pixelatteClient.GetImageAsync($"image?img_path={SelectedImagePath}&operation={SelectedOperation}&value={(OperationValue < 0 ? 0 : OperationValue)}");
            BasicPixelOperationImage = image.Image;
            IsLoading = false;
        }
    }
}
