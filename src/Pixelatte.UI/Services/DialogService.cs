using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace Pixelatte.UI.Services
{
    static internal class DialogService
    {
        public static async Task<ContentDialogResult> ShowAsync(string? message, string title = "Error")
        {
            ContentDialog dialog = new ContentDialog()
            {
                XamlRoot = App.m_window?.Content.XamlRoot,
                Title = "Unhandled Exception",
                Content = message ?? "Unknown error",
                CloseButtonText = "Ok"
            };

            return await dialog.ShowAsync();
        }
    }
}
