using System.Windows;
using System.Windows.Media;

namespace Regular.Utilities
{
    public static class WpfUtils

    {
        // Method from Brian Lagunas to find an element's parent of a particular type
        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            // We've reached the end of the tree
            if (parentObject == null) return null;

            // Check if the parent matches the type we're looking for
            if (parentObject is T parent)
                return parent;
            return FindParent<T>(parentObject);
        }
    }
}
