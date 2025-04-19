using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Pixelatte.UI.Models;
using Pixelatte.UI.Services;
using Pixelatte.UI.Views;
using System;
using System.Collections.Generic;
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
        private readonly Dictionary<string, string> _orientationValues = new()
        {
            {"horizontal", "\uE76F" },
            {"vertical", "\uE784" },
        };
        [ObservableProperty]
        ObservableCollection<string> _tags = new ObservableCollection<string>();
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
        [ObservableProperty]
        private int _saltAndPepperNoiseLevel;
        [ObservableProperty]
        private bool _applySaltAndPepperNoise;
        [ObservableProperty]
        private BitmapImage _saltAndPepperNoiseImage;
        [ObservableProperty]
        private bool _showOriginalInGrayscale;
        [ObservableProperty]
        private bool _showOriginalInBasicPixelOperation;
        [ObservableProperty]
        private bool _showOriginalInSaltAndPepperNoise;
        [ObservableProperty]
        private ObservableCollection<PixelatteOperationItem> _pixelatteOperationList = new ObservableCollection<PixelatteOperationItem>();
        [ObservableProperty]
        private Type _page;
        [ObservableProperty]
        private string _orientation;
        [ObservableProperty]
        private string _operationTitle;

        public MainWindowViewModel()
        {
            _pixelatteClient = new PixelatteManager("http://127.0.0.1:8000");
            _filePickerService = new FilePickerService();
            SelectedOperation = "Add";
            PixelatteOperationList.Add(new PixelatteOperationItem("Grayscale", "Convert the image to grayscale", "Assets/LargeTile.scale-100.png", OpenGrayscalePageCommand));
            Page = typeof(SelectImagePage);
            Orientation = _orientationValues["horizontal"];
        }

        [RelayCommand]
        private async Task LoadImageAsync(StorageFile file)
        {
            try
            {
                Tags.Clear();
                SelectedImagePath = file.Path;
                IsLoading = true;
                ImageDTO image = await _pixelatteClient.GetImageAsync($"open?img_path={file.Path}");

                if (image.Image == null) return;

                SelectedImage = image.Image;
                Tags.Add($"{image.Width}x{image.Height}");
                Tags.Add(Path.GetExtension(file.Path));
                Page = typeof(InitialPage);
            }
            finally
            {
                IsLoading = false;
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

        async partial void OnApplySaltAndPepperNoiseChanged(bool value)
        {
            await LoadSaltAndPepperNoiseImage();
        }

        async partial void OnSaltAndPepperNoiseLevelChanged(int value)
        {
            if (!ApplySaltAndPepperNoise) return;
            await LoadSaltAndPepperNoiseImage();
        }

        partial void OnPageChanged(Type value)
        {
            if (value == null) OperationTitle = null;
        }

        private async Task LoadSaltAndPepperNoiseImage()
        {
            IsLoading = true;
            ImageDTO image = await _pixelatteClient.GetImageAsync($"saltandpeppernoise?img_path={SelectedImagePath}&noise_level={SaltAndPepperNoiseLevel}");
            SaltAndPepperNoiseImage = image.Image;
            IsLoading = false;
        }

        [RelayCommand]
        private async Task OpenGrayscalePage()
        {
            Page = typeof(GrayscaleView);
            IsLoading = true;
            ImageDTO image = await _pixelatteClient.GetImageAsync($"grayscale?img_path={SelectedImagePath}");
            GrayscaleImage = image.Image;
            OperationTitle = "Grayscale";
            IsLoading = false;
        }

        [RelayCommand]
        private void ChangeOrientation(object? parameter)
        {
            if (_orientationValues.ContainsKey(parameter?.ToString() ?? string.Empty))
            {
                Orientation = "";
                Orientation = _orientationValues[parameter.ToString()];
            }
        }
    }
}
