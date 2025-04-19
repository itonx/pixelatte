using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;
using Pixelatte.UI.Models;

namespace Pixelatte.UI.Behaviors
{
    internal class GridViewItemClickBehavior : Behavior<GridView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.ItemClick += AssociatedObject_ItemClick;
        }

        private void AssociatedObject_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is PixelatteOperationItem operation)
            {
                operation.PageCommand?.Execute(null);
            }
        }

        protected override void OnDetaching()
        {
            AssociatedObject.ItemClick -= AssociatedObject_ItemClick;
            base.OnDetaching();
        }
    }
}
