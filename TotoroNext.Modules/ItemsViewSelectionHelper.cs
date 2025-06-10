using System.Collections;

namespace TotoroNext.Module;

public static class ItemsViewSelectionHelper
{
    public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.RegisterAttached(
            "SelectedItem",
            typeof(object),
            typeof(ItemsViewSelectionHelper),
            new PropertyMetadata(null, OnSelectedItemChanged));

    public static object GetSelectedItem(DependencyObject obj) => obj.GetValue(SelectedItemProperty);
    public static void SetSelectedItem(DependencyObject obj, object value) => obj.SetValue(SelectedItemProperty, value);

    private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ItemsView itemsView)
        {
            var items = itemsView.ItemsSource as IList;
            if (items != null && e.NewValue != null)
            {
                int index = items.IndexOf(e.NewValue);
                if (index >= 0)
                {
                    itemsView.Select(index);
                }
            }
        }
    }

    public static readonly DependencyProperty EnableSelectionTrackingProperty =
        DependencyProperty.RegisterAttached(
            "EnableSelectionTracking",
            typeof(bool),
            typeof(ItemsViewSelectionHelper),
            new PropertyMetadata(false, OnEnableSelectionTrackingChanged));

    public static bool GetEnableSelectionTracking(DependencyObject obj) => (bool)obj.GetValue(EnableSelectionTrackingProperty);
    public static void SetEnableSelectionTracking(DependencyObject obj, bool value) => obj.SetValue(EnableSelectionTrackingProperty, value);

    private static void OnEnableSelectionTrackingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ItemsView itemsView && (bool)e.NewValue)
        {
            itemsView.SelectionChanged += (sender, args) =>
            {
                SetSelectedItem(itemsView, sender.SelectedItem);
            };
        }
    }
}
