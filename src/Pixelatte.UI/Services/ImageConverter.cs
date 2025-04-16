using Microsoft.UI.Xaml.Media.Imaging;
using Pixelatte.UI.Models;
using System;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Pixelatte.UI.Services
{
    internal class ImageConverter
    {
        public async Task<ImageDTO> GetImageAsync(HttpResponseMessage response)
        {
            byte[] pixelBytes = await response.Content.ReadAsByteArrayAsync();
            var bitmap = new BitmapImage();
            using (var stream = new InMemoryRandomAccessStream())
            {
                await stream.WriteAsync(pixelBytes.AsBuffer());
                stream.Seek(0);
                await bitmap.SetSourceAsync(stream);
            }
            response.Headers.TryGetValues("width", out var widthValues);
            response.Headers.TryGetValues("height", out var heightValues);
            return new ImageDTO(bitmap, widthValues?.FirstOrDefault() ?? null, heightValues?.FirstOrDefault() ?? null);
        }
    }
}
