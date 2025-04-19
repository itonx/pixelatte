using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;
using System;

namespace Pixelatte.UI.Behaviors
{
    internal class MvvmNavigationBehavior : Behavior<Frame>
    {
        public static readonly DependencyProperty PageProperty =
            DependencyProperty.Register(
                nameof(Page),
                typeof(Type),
                typeof(MvvmNavigationBehavior),
                new PropertyMetadata(null, OnPageChanged));

        private static void OnPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MvvmNavigationBehavior behavior && e.NewValue != null && behavior.IsLoaded)
            {
                Type? pageType = e.NewValue as Type;
                behavior.AssociatedObject.Navigate(pageType, null, new Microsoft.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());
            }
        }

        public Type Page
        {
            get { return (Type)GetValue(PageProperty); }
            set { SetValue(PageProperty, value); }
        }

        public static readonly DependencyProperty IsLoadedProperty =
            DependencyProperty.Register(
                nameof(IsLoaded),
                typeof(bool),
                typeof(MvvmNavigationBehavior),
                new PropertyMetadata(false));

        public bool IsLoaded
        {
            get { return (bool)GetValue(IsLoadedProperty); }
            set { SetValue(IsLoadedProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            this.IsLoaded = true;
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            if (Page == null) return;
            AssociatedObject.Navigate(Page);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }
    }
}
