using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pixelatte.UI.Behaviors
{
    internal class ItemClickToCommandBehavior : Behavior<GridView>
    {
        public static readonly DependencyProperty FlyoutProperty =
            DependencyProperty.Register(
                nameof(Flyout),
                typeof(Flyout),
                typeof(ItemClickToCommandBehavior),
                new PropertyMetadata(null));

        public Flyout Flyout
        {
            get { return (Flyout)GetValue(FlyoutProperty); }
            set { SetValue(FlyoutProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                nameof(Command),
                typeof(ICommand),
                typeof(ItemClickToCommandBehavior),
                new PropertyMetadata(null));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.ItemClick += AssociatedObject_ItemClick;
        }

        private void AssociatedObject_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Command?.Execute((e.ClickedItem as FrameworkElement).Tag);
            if (this.Flyout != null)
            {
                Task.Delay(10).ContinueWith(_ => this.Flyout.Hide(), TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.ItemClick -= AssociatedObject_ItemClick;
        }
    }
}
