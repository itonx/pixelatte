using Pixelatte.UI.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pixelatte.UI.Services
{
    internal class PixelatteManager
    {
        private HttpClient _httpClient = new HttpClient();
        private ImageConverter _imageConverter = new ImageConverter();

        public PixelatteManager(string baseUrl)
        {
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        public async Task<ImageDTO> GetImageAsync(string endpoint)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
                return await _imageConverter.GetImageAsync(response);
            }
            catch (Exception e)
            {
                await DialogService.ShowAsync(e?.Message);
            }

            return new();
        }
    }
}
