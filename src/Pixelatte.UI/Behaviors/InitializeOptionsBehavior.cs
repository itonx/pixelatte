using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.Xaml.Interactivity;

namespace Pixelatte.UI.Behaviors
{
    internal class InitializeOptionsBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty MainWindowProperty =
            DependencyProperty.Register(
                nameof(MainWindow),
                typeof(Window),
                typeof(InitializeOptionsBehavior),
                new PropertyMetadata(null));

        public Window MainWindow
        {
            get { return (Window)GetValue(MainWindowProperty); }
            set { SetValue(MainWindowProperty, value); }
        }

        public static readonly DependencyProperty MaximizeAtStartupProperty =
            DependencyProperty.Register(
                nameof(MaximizeAtStartup),
                typeof(bool),
                typeof(InitializeOptionsBehavior),
                new PropertyMetadata(false));

        public bool MaximizeAtStartup
        {
            get { return (bool)GetValue(MaximizeAtStartupProperty); }
            set { SetValue(MaximizeAtStartupProperty, value); }
        }

        public static readonly DependencyProperty ExtendsContentIntoTitleBarProperty =
            DependencyProperty.Register(
                nameof(ExtendsContentIntoTitleBar),
                typeof(bool),
                typeof(InitializeOptionsBehavior),
                new PropertyMetadata(false));

        public bool ExtendsContentIntoTitleBar
        {
            get { return (bool)GetValue(ExtendsContentIntoTitleBarProperty); }
            set { SetValue(ExtendsContentIntoTitleBarProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            if (this.MainWindow == null || this.MainWindow.AppWindow == null) return;

            if (this.MaximizeAtStartup && this.MainWindow.AppWindow.Presenter is OverlappedPresenter overlappedPresenter)
            {
                overlappedPresenter.Maximize();
            }

            this.MainWindow.ExtendsContentIntoTitleBar = this.ExtendsContentIntoTitleBar;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }
    }
}
