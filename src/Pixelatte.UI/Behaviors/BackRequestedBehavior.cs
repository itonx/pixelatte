using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;
using System;

namespace Pixelatte.UI.Behaviors
{
    internal class BackRequestedBehavior : Behavior<TitleBar>
    {
        public static readonly DependencyProperty FrameControlProperty =
            DependencyProperty.Register(
                nameof(FrameControl),
                typeof(Frame),
                typeof(BackRequestedBehavior),
                new PropertyMetadata(null));

        public Frame FrameControl
        {
            get { return (Frame)GetValue(FrameControlProperty); }
            set { SetValue(FrameControlProperty, value); }
        }

        public static readonly DependencyProperty PageProperty =
            DependencyProperty.Register(
                nameof(Page),
                typeof(Type),
                typeof(BackRequestedBehavior),
                new PropertyMetadata(null));

        public Type Page
        {
            get { return (Type)GetValue(PageProperty); }
            set { SetValue(PageProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.BackRequested += AssociatedObject_BackRequested;
        }

        private void AssociatedObject_BackRequested(TitleBar sender, object args)
        {
            if (this.FrameControl == null) return;
            if (!this.FrameControl.CanGoBack) return;
            this.Page = null;//Page is not updated on GoBack(), null will trigger the next Page
            this.FrameControl.GoBack();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.BackRequested -= AssociatedObject_BackRequested;
        }
    }
}
