using Microsoft.UI.Xaml.Media.Imaging;

namespace Pixelatte.UI.Models
{
    internal class ImageDTO
    {
        public BitmapImage Image { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }

        public ImageDTO()
        {
            Image = null;
            Width = "NaN";
            Height = "NaN";
        }

        public ImageDTO(BitmapImage image, string width, string height)
        {
            Image = image;
            Width = width ?? "NaN";
            Height = height ?? "NaN";
        }
    }
}
