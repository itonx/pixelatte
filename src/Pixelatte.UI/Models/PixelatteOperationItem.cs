namespace Pixelatte.UI.Models
{
    internal class PixelatteOperationItem
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string ImagePath { get; set; }

        public PixelatteOperationItem(string title, string subtitle, string imagePath)
        {
            Title = title;
            Subtitle = subtitle;
            ImagePath = imagePath;
        }
    }
}
