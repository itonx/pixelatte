using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Pixelatte.UI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        HttpClient httpClient = new HttpClient();
        List<string> Tags = new List<string>();

        public MainWindow()
        {
            this.InitializeComponent();
            httpClient.BaseAddress = new Uri("http://127.0.0.1:8000");
            var iconPath = System.IO.Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Assets", "icon.ico");
            this.AppWindow.SetIcon(iconPath);
        }

        private async void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker fileOpenPicker = new()
            {
                ViewMode = PickerViewMode.Thumbnail,
                FileTypeFilter = { ".jpg", ".jpeg", ".png", ".gif" },
            };

            nint windowHandle = WindowNative.GetWindowHandle(this);
            InitializeWithWindow.Initialize(fileOpenPicker, windowHandle);

            StorageFile file = await fileOpenPicker.PickSingleFileAsync();

            if (file != null)
            {
                // Do something with the file.
                filePath.Text = file.Path;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.UriSource = new Uri(file.Path);
                using (System.Drawing.Image image = System.Drawing.Image.FromFile(file.Path))
                {
                    Tags.Add($"{image.Width.ToString()}x{image.Height.ToString()}");
                    Tags.Add(Path.GetExtension(file.Path));
                    properties.ItemsSource = Tags;
                    //imgContainer.MaxWidth = image.Width;
                    //imgContainer.MaxHeight = image.Height;
                }
                //imgContainer.Source = bitmapImage;
                var grayImg = await GetBitmapImage($"image?img_path={filePath.Text}&operation=add&value=0");
                imgContainer.Source = grayImg;
            }
        }

        private async Task<byte[]> GetPixelBytes(string endpoint)
        {
            var response = await httpClient.GetAsync(endpoint);
            return await response.Content.ReadAsByteArrayAsync();
        }

        private async Task<BitmapImage> GetBitmapImage(string endpoint)
        {
            byte[] pixelBytes = await GetPixelBytes(endpoint);
            var bitmap = new BitmapImage();
            using (var stream = new InMemoryRandomAccessStream())
            {
                await stream.WriteAsync(pixelBytes.AsBuffer());
                stream.Seek(0);
                await bitmap.SetSourceAsync(stream);
            }
            return bitmap;
        }

        private async void GrayscaleToggle_Toggled(object sender, RoutedEventArgs e)
        {
            await ProcessToggleOperation(sender as ToggleSwitch, imgContainerGrayscale, $"grayscale?img_path={filePath.Text}");
        }

        private async void BasicPixelOperationsToggle_Toggled(object sender, RoutedEventArgs e)
        {
            await ProcessToggleOperation(sender as ToggleSwitch, imgContainerBasicPixelOperations, $"image?img_path={filePath.Text}&operation={operations.SelectedValue}&value={variation.Value}");
        }

        private async Task ProcessToggleOperation(ToggleSwitch toggle, Image imageContainer, string endpoint)
        {
            if (toggle == null || imageContainer == null) return;

            if (toggle.IsOn)
            {
                var grayImg = await GetBitmapImage($"/{endpoint}");
                imageContainer.Source = grayImg;
                imageContainer.Visibility = Visibility.Visible;
            }
            else
            {
                imageContainer.Source = null;
            }
        }

        private async void variation_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            await ProcessToggleOperation(operationsToggleSwitch, imgContainerBasicPixelOperations, $"image?img_path={filePath.Text}&operation={operations.SelectedValue}&value={variation.Value}");
        }
    }
}
