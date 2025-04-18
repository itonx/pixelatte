using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Pixelatte.UI.Services;
using System;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Pixelatte.UI
{
    public sealed class ImagePicker : Control
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command",
            typeof(ICommand),
            typeof(ImagePicker),
            new PropertyMetadata(null));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public ImagePicker()
        {
            this.DefaultStyleKey = typeof(ImagePicker);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            HyperlinkButton? hyperlinkButton = GetTemplateChild("imagePicker") as HyperlinkButton;
            Grid? draggableArea = GetTemplateChild("draggableArea") as Grid;

            if (hyperlinkButton != null)
                hyperlinkButton.Click += HyperlinkButton_Click;
            if (draggableArea != null)
            {
                draggableArea.DragOver += DraggableArea_DragOver;
                draggableArea.Drop += DraggableArea_Drop;
            }
        }

        private async void DraggableArea_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Count == 1)
                {
                    StorageFile? storageFile = items[0] as StorageFile;
                    if (storageFile == null) return;
                    this.ExecuteCommand(storageFile);
                }
                else
                {
                    await DialogService.ShowAsync("Can't load more than 1 image.");
                }
            }
        }

        private void DraggableArea_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        }

        private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker fileOpenPicker = new()
            {
                ViewMode = PickerViewMode.Thumbnail,
                FileTypeFilter = { ".jpg", ".jpeg", ".png", ".gif" },
            };

            nint windowHandle = WindowNative.GetWindowHandle(App.m_window);
            InitializeWithWindow.Initialize(fileOpenPicker, windowHandle);

            StorageFile? storageFile = await fileOpenPicker.PickSingleFileAsync();
            if (storageFile == null) return;
            this.ExecuteCommand(storageFile);
        }

        private void ExecuteCommand(object? param)
        {
            if (this.Command.CanExecute(param))
            {
                this.Command?.Execute(param);
            }
        }
    }
}
