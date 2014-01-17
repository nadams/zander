using System.Windows;
using System.Windows.Controls;

namespace Zander.Modules.Settings {
    public static class TreeViewSelectedItemBehavior {
        public static readonly DependencyProperty TreeViewSelectedItemProperty =
            DependencyProperty.RegisterAttached(
            "TreeViewSelectedItem",
            typeof(object),
            typeof(TreeViewSelectedItemBehavior),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                new PropertyChangedCallback(TreeViewSelectedItemChanged)));

        public static object GetTreeViewSelectedItem(DependencyObject dependencyObject) {
            return (object)dependencyObject.GetValue(TreeViewSelectedItemProperty);
        }

        public static void SetTreeViewSelectedItem(DependencyObject dependencyObject, object value) {
            dependencyObject.SetValue(TreeViewSelectedItemProperty, value);
        }

        private static void TreeViewSelectedItemChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e) {
            var tv = dependencyObject as TreeView;

            if(e.NewValue == null && e.OldValue != null) {
                tv.SelectedItemChanged -=
                    new RoutedPropertyChangedEventHandler<object>(SelectedItemChanged);
            } else if(e.NewValue != null && e.OldValue == null) {
                tv.SelectedItemChanged +=
                    new RoutedPropertyChangedEventHandler<object>(SelectedItemChanged);
            }
        }

        private static void SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
            SetTreeViewSelectedItem((DependencyObject)sender, e.NewValue);
        }
    }
}
