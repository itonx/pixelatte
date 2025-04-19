using System;
using System.Windows.Input;

namespace Pixelatte.UI.Models
{
    internal class PixelatteOperationItem
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string ImagePath { get; set; }
        public ICommand PageCommand { get; set; }
        public Type PageType { get; set; }

        public PixelatteOperationItem(string title, string subtitle, string imagePath, ICommand pageCommand, Type pageType)
        {
            Title = title;
            Subtitle = subtitle;
            ImagePath = imagePath;
            PageCommand = pageCommand;
            PageType = pageType;
        }
    }
}
