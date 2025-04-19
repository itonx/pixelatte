using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.Xaml.Interactivity;

namespace Pixelatte.UI.Behaviors
{
    internal class MaximizedAtStartupBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty AppWindowProperty =
        DependencyProperty.Register(
            nameof(AppWindow),
            typeof(AppWindow),
            typeof(MaximizedAtStartupBehavior),
            new PropertyMetadata(null));

        public AppWindow AppWindow
        {
            get { return (AppWindow)GetValue(AppWindowProperty); }
            set { SetValue(AppWindowProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            if (this.AppWindow.Presenter is OverlappedPresenter overlappedPresenter)
            {
                overlappedPresenter.Maximize();
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }
    }
}
