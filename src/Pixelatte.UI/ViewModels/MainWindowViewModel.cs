using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Pixelatte.UI.Models;
using Pixelatte.UI.Services;
using Pixelatte.UI.Views;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Pixelatte.UI.ViewModels
{
    internal partial class MainWindowViewModel : ObservableObject
    {
        private readonly PixelatteManager _pixelatteClient;

        [ObservableProperty]
        private bool _isLoading;
        [ObservableProperty]
        private string _operationTitle;
        [ObservableProperty]
        private Type _page;
        [ObservableProperty]
        private bool _showServerConfiguration;
        [ObservableProperty]
        private bool _showOriginal;
        [ObservableProperty]
        private bool _isHorizontal;
        [ObservableProperty]
        private bool _switchImages;
        [ObservableProperty]
        private bool _isLocalServerRunning;
        [ObservableProperty]
        private bool _isLocalServerLoading;
        [ObservableProperty]
        ObservableCollection<string> _tags = new ObservableCollection<string>();
        [ObservableProperty]
        private string _selectedImagePath;
        [ObservableProperty]
        private BitmapImage _selectedImage;
        [ObservableProperty]
        private ObservableCollection<PixelatteOperationItem> _pixelatteOperationList = new ObservableCollection<PixelatteOperationItem>();

        [ObservableProperty]
        private BitmapImage _grayscaleImage;

        [ObservableProperty]
        private BitmapImage _basicPixelOperationImage;
        [ObservableProperty]
        private string _selectedOperation;
        [ObservableProperty]
        private int _operationValue;

        [ObservableProperty]
        private BitmapImage _saltAndPepperNoiseImage;
        [ObservableProperty]
        private int _saltAndPepperNoiseLevel;

        public MainWindowViewModel()
        {
            _pixelatteClient = new PixelatteManager("http://127.0.0.1:8000");
            SelectedOperation = "Add";
            PixelatteOperationList.Add(new PixelatteOperationItem("Grayscale", "Convert the image to grayscale", "/Assets/grayscale.svg", OpenGrayscalePageCommand, typeof(GrayscaleView)));
            PixelatteOperationList.Add(new PixelatteOperationItem("Pixel Operations", "Add, substract, multiply, or divide the value of each pixel", "/Assets/basicPixelOperation.svg", OpenBasicPixelOperationPageCommand, typeof(BasicPixelOperationView)));
            PixelatteOperationList.Add(new PixelatteOperationItem("Salt & Pepper Noise Gen", "Add salt and pepper noise to the image", "/Assets/saltAndPepperNoise.png", OpenSaltAndPepperNoisePageCommand, typeof(SaltAndPepperNoiseView)));
            Page = typeof(SelectImagePage);
            ShowServerConfiguration = true;
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

        async partial void OnSelectedOperationChanged(string value)
        {
            if (_basicPixelOperationImage == null) return;
            await LoadBasicPixelOperationImage();
        }

        async partial void OnOperationValueChanged(int value)
        {
            if (_basicPixelOperationImage == null) return;
            await LoadBasicPixelOperationImage();
        }

        private async Task LoadBasicPixelOperationImage()
        {
            IsLoading = true;
            ImageDTO image = await _pixelatteClient.GetImageAsync($"image?img_path={SelectedImagePath}&operation={SelectedOperation}&value={(OperationValue < 0 ? 0 : OperationValue)}");
            BasicPixelOperationImage = image.Image;
            IsLoading = false;
        }

        async partial void OnSaltAndPepperNoiseLevelChanged(int value)
        {
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
            LoadPage(typeof(GrayscaleView));
            IsLoading = true;
            ImageDTO image = await _pixelatteClient.GetImageAsync($"grayscale?img_path={SelectedImagePath}");
            GrayscaleImage = image.Image;
            IsLoading = false;
        }

        [RelayCommand]
        private async Task OpenBasicPixelOperationPage()
        {
            LoadPage(typeof(BasicPixelOperationView));
            await LoadBasicPixelOperationImage();
        }

        [RelayCommand]
        private async Task OpenSaltAndPepperNoisePage()
        {
            LoadPage(typeof(SaltAndPepperNoiseView));
            await LoadSaltAndPepperNoiseImage();
        }

        private void LoadPage(Type pageType)
        {
            Page = pageType;
            OperationTitle = PixelatteOperationList.FirstOrDefault(o => o.PageType == pageType)?.Title ?? string.Empty;
        }
    }
}
