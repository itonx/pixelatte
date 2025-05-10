using CommunityToolkit.Mvvm.ComponentModel;

namespace Pixelatte.UI.Models
{
    internal partial class KernelModel : ObservableObject
    {
        [ObservableProperty]
        public partial decimal Value { get; set; }
        [ObservableProperty]
        public partial bool IsEnabled { get; set; }

        public KernelModel(decimal value)
        {
            Value = value;
            IsEnabled = false;
        }
    }
}
