using System.Windows;
using System.Windows.Media;

namespace Regular.Utilities
{
    public static class VisualTreeUtils
    {
        // Method from Brian Lagunas to find an element's parent of a particular type
        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            // We've reached the end of the tree
            if (parentObject == null) return null;

            // Check if the parent matches the type we're looking for
            if (parentObject is T parent) return parent;
            return FindParent<T>(parentObject);
        }

        // Recursively searched the visual tree for a child element of a particular type
        public static T FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;
            
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent);)
            {
                var childObject = VisualTreeHelper.GetChild(parent, i);
                if (childObject is T child) return child;
                return FindChild<T>(childObject);
            }
            return null;
        }
    }
}
