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
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Pixelatte.UI.ViewModels
{
    internal partial class MainWindowViewModel : ObservableObject
    {
        private readonly PixelatteManager _pixelatteClient;

        [ObservableProperty]
        public partial bool IsLoading { get; set; }
        [ObservableProperty]
        public partial string IsLoadingText { get; set; }
        [ObservableProperty]
        public partial string? OperationTitle { get; set; }
        [ObservableProperty]
        public partial Type Page { get; set; }
        [ObservableProperty]
        public partial bool ShowServerConfiguration { get; set; }

        [ObservableProperty]
        public partial bool ShowOriginal { get; set; }
        [ObservableProperty]
        public partial bool IsHorizontal { get; set; }
        [ObservableProperty]
        public partial bool SwitchImages { get; set; }
        [ObservableProperty]
        public partial bool IsLocalServerRunning { get; set; }
        [ObservableProperty]
        public partial bool IsLocalServerLoading { get; set; }
        [ObservableProperty]
        public partial ObservableCollection<string> Tags { get; set; }
        [ObservableProperty]
        public partial string SelectedImagePath { get; set; }
        [ObservableProperty]
        public partial BitmapImage? SelectedImage { get; set; }
        [ObservableProperty]
        public partial ObservableCollection<PixelatteOperationItem> PixelatteOperationList { get; set; }

        [ObservableProperty]
        public partial BitmapImage? GrayscaleImage { get; set; }

        [ObservableProperty]
        public partial BitmapImage? BasicPixelOperationImage { get; set; }
        [ObservableProperty]
        public partial string SelectedOperation { get; set; }
        [ObservableProperty]
        public partial int OperationValue { get; set; }

        [ObservableProperty]
        public partial BitmapImage? SaltAndPepperNoiseImage { get; set; }
        [ObservableProperty]
        public partial int SaltAndPepperNoiseLevel { get; set; }

        [ObservableProperty]
        public partial BitmapImage? ConvolutionImage { get; set; }
        [ObservableProperty]
        public partial int KernelSize { get; set; }
        [ObservableProperty]
        public partial ObservableCollection<KernelModel> KernelValues { get; set; }
        [ObservableProperty]
        public partial ObservableCollection<FilterType> FilterTypes { get; set; }
        [ObservableProperty]
        public partial FilterType SelectedFilterType { get; set; }
        [ObservableProperty]
        public partial ObservableCollection<FilterOperation> OperationList { get; set; }
        [ObservableProperty]
        public partial FilterOperation SelectedFilterOperation { get; set; }
        [ObservableProperty]
        public partial bool IsCustomKernel { get; set; }

        public MainWindowViewModel()
        {
            _pixelatteClient = new PixelatteManager(AppSettings.Configuration["server"] ?? string.Empty);
            SelectedOperation = "Add";
            PixelatteOperationList = new ObservableCollection<PixelatteOperationItem>()
            {
                new PixelatteOperationItem("Grayscale", "Convert the image to grayscale", "/Assets/grayscale.svg", OpenGrayscalePageCommand, typeof(GrayscaleView)),
                new PixelatteOperationItem("Pixel Operations", "Add, substract, multiply, or divide the value of each pixel", "/Assets/basicPixelOperation.svg", OpenBasicPixelOperationPageCommand, typeof(BasicPixelOperationView)),
                new PixelatteOperationItem("Salt & Pepper Noise Gen", "Add salt and pepper noise to the image", "/Assets/saltAndPepperNoise.png", OpenSaltAndPepperNoisePageCommand, typeof(SaltAndPepperNoiseView)),
                new PixelatteOperationItem("Convolution", "Apply a kernel (filter) to the image", "/Assets/saltAndPepperNoise.png", OpenConvolutionPageCommand, typeof(ConvolutionView)),//TODO: Update icon
            };
            Page = typeof(SelectImagePage);
            Tags = new ObservableCollection<string>();
            SelectedImagePath = string.Empty;
            IsLoadingText = "Loading";
            KernelValues = new ObservableCollection<KernelModel>();
            FilterTypes = new ObservableCollection<FilterType>()
            {
                new FilterType("Low-Pass", new List<FilterOperation>()
                {
                    new FilterOperation("Gaussian"),
                    new FilterOperation("Median"),
                    new FilterOperation("Mean"),
                    new FilterOperation("Mode"),
                }),
                new FilterType("High-Pass", new List<FilterOperation>()
                {
                    new FilterOperation("Laplacian", [0, 1, 0, 1, -4, 1, 0, 1, 0]),
                    new FilterOperation("Prewitt", [1, 1, 1, 0, 0, 0, 1, 1, 1]), //Horizontal
                    new FilterOperation("Sobel", [1, 2, 1, 0, 0, 0, -1, -2, -1]), //Horizontal
                }),
                new FilterType("Algorithms", new List<FilterOperation>()
                {
                    new FilterOperation("Canny"),//TODO: Review Canny filter
                }),
            };
            SelectedFilterType = FilterTypes[0];
            KernelSize = 3;
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

        partial void OnIsCustomKernelChanged(bool value)
        {
            if (SelectedFilterOperation.UseKernel)
            {
                foreach (KernelModel kernelValue in KernelValues)
                {
                    kernelValue.IsEnabled = value;
                }
            }
        }

        partial void OnSelectedFilterTypeChanged(FilterType value)
        {
            OperationList = new ObservableCollection<FilterOperation>(value.Operations);
            SelectedFilterOperation = OperationList[0];
        }

        partial void OnSelectedFilterOperationChanged(FilterOperation value)
        {
            if (value == null) return;
            KernelSize = value.DefaultKernel != null ? (int)Math.Sqrt(value.DefaultKernel.Length) : KernelSize;
            CreateKernel();
            IsCustomKernel = false;
        }

        private void CreateKernel()
        {
            if (SelectedFilterOperation != null && SelectedFilterOperation.UseKernel && SelectedFilterOperation.DefaultKernel != null)
            {
                KernelValues.Clear();
                for (int i = 0; i < SelectedFilterOperation.DefaultKernel.Length; i++)
                {
                    KernelValues.Add(new KernelModel(SelectedFilterOperation.DefaultKernel[i]));
                }
            }
        }

        async partial void OnSelectedOperationChanged(string value)
        {
            if (BasicPixelOperationImage == null) return;
            await LoadBasicPixelOperationImage();
        }

        async partial void OnOperationValueChanged(int value)
        {
            if (BasicPixelOperationImage == null) return;
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
            ImageDTO image = await _pixelatteClient.GetImageAsync($"convolution?img_path={SelectedImagePath}");
            SaltAndPepperNoiseImage = image.Image;
            IsLoading = false;
        }

        private async Task LoadConvolutionImage()
        {
            IsLoading = true;
            ImageDTO image = await _pixelatteClient.GetImageAsync($"saltandpeppernoise?img_path={SelectedImagePath}&noise_level={SaltAndPepperNoiseLevel}");
            ConvolutionImage = image.Image;
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

        [RelayCommand]
        private async Task OpenConvolutionPage()
        {
            LoadPage(typeof(ConvolutionView));
            await LoadConvolutionImage();
        }

        private void LoadPage(Type pageType)
        {
            Page = pageType;
            OperationTitle = PixelatteOperationList.FirstOrDefault(o => o.PageType == pageType)?.Title ?? string.Empty;
        }

        partial void OnIsLocalServerLoadingChanged(bool value)
        {
            IsLoading = value;
            IsLoadingText = value ? "Starting local server..." : "Loading";
        }
    }
}
