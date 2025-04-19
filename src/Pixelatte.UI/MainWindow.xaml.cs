using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Pixelatte.UI.ViewModels;
using Pixelatte.UI.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Pixelatte.UI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.AppContainer.DataContext = new MainWindowViewModel();
            ExtendsContentIntoTitleBar = true;

            if (this.AppWindow.Presenter is OverlappedPresenter overlappedPresenter)
            {
                overlappedPresenter.Maximize();
            }
            this.RootFrame.Navigate(typeof(InitialPage));
            this.AppWindow.Changed += AppWindow_Changed;
        }

        private void AppWindow_Changed(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowChangedEventArgs args)
        {
            if (this.AppContainer.ActualWidth != this.AppWindow.Size.Width)
            {
                this.AppContainer.Arrange(new Windows.Foundation.Rect(0, 0, this.AppWindow.ClientSize.Width, this.AppWindow.ClientSize.Height));
            }
        }

        private void TitleBar_BackRequested(Microsoft.UI.Xaml.Controls.TitleBar sender, object args)
        {
            this.RootFrame.GoBack();
        }
    }
}
