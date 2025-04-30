using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Pixelatte.UI
{
    public sealed class ImageVisualizer : ContentControl
    {
        public static readonly DependencyProperty ImageResultProperty =
            DependencyProperty.Register(
                nameof(ImageResult),
                typeof(BitmapImage),
                typeof(ImageVisualizer),
                new PropertyMetadata(null));

        public BitmapImage ImageResult
        {
            get { return (BitmapImage)GetValue(ImageResultProperty); }
            set { SetValue(ImageResultProperty, value); }
        }

        public static readonly DependencyProperty BaseImageProperty =
            DependencyProperty.Register(
                nameof(BaseImage),
                typeof(BitmapImage),
                typeof(ImageVisualizer),
                new PropertyMetadata(null));

        public BitmapImage BaseImage
        {
            get { return (BitmapImage)GetValue(BaseImageProperty); }
            set { SetValue(BaseImageProperty, value); }
        }

        public static readonly DependencyProperty ShowBaseImageProperty =
            DependencyProperty.Register(
                nameof(ShowBaseImage),
                typeof(bool),
                typeof(ImageVisualizer),
                new PropertyMetadata(false, OnShowBaseImageChanged));

        private static void OnShowBaseImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue)
            {
                ImageVisualizer imageVisualizer = (ImageVisualizer)d;
                if (imageVisualizer.SwitchImages)
                {
                    imageVisualizer.SwitchImages = false;
                }
            }
        }

        public bool ShowBaseImage
        {
            get { return (bool)GetValue(ShowBaseImageProperty); }
            set { SetValue(ShowBaseImageProperty, value); }
        }

        public static readonly DependencyProperty ViewerOrientationProperty =
            DependencyProperty.Register(
                nameof(ViewerOrientation),
                typeof(Orientation),
                typeof(ImageVisualizer),
                new PropertyMetadata(Orientation.Horizontal));

        public Orientation ViewerOrientation
        {
            get { return (Orientation)GetValue(ViewerOrientationProperty); }
            set { SetValue(ViewerOrientationProperty, value); }
        }

        public static readonly DependencyProperty SwitchImagesProperty =
            DependencyProperty.Register(
                nameof(SwitchImages),
                typeof(bool),
                typeof(ImageVisualizer),
                new PropertyMetadata(false, OnSwitchImagesChanged));

        private static void OnSwitchImagesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageVisualizer imageVisualizer = (ImageVisualizer)d;
            BitmapImage original = imageVisualizer.BaseImage;
            imageVisualizer.BaseImage = imageVisualizer.ImageResult;
            imageVisualizer.ImageResult = original;
        }

        public bool SwitchImages
        {
            get { return (bool)GetValue(SwitchImagesProperty); }
            set { SetValue(SwitchImagesProperty, value); }
        }

        public ImageVisualizer()
        {
            this.DefaultStyleKey = typeof(ImageVisualizer);
        }
    }
}
