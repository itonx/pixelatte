using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Pixelatte.UI.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InitialPage : Page
    {
        public InitialPage()
        {
            this.InitializeComponent();
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            (App.m_window as MainWindow).RootFrame.Navigate(typeof(GrayscaleView), null, new Microsoft.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());
        }
    }
}
