using System.Collections;
using System.Windows.Input;
using Microsoft.UI.Xaml.Controls;

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
            if (itemsView.ItemsSource is IList items && e.NewValue != null)
            {
                int index = items.IndexOf(e.NewValue);
                if (index >= 0)
                {
                    itemsView.Select(index);
                    if (index == items.Count - 1)
                    {
                        itemsView.StartBringItemIntoView(index, new BringIntoViewOptions { VerticalAlignmentRatio = 1f });
                    }
                    else
                    {
                        itemsView.StartBringItemIntoView(index, new BringIntoViewOptions { VerticalAlignmentRatio = 0f });
                    }
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

	public static readonly DependencyProperty CommandProperty =
		DependencyProperty.RegisterAttached(
			"Command",
			typeof(ICommand),
			typeof(ItemsViewSelectionHelper),
			new PropertyMetadata(null, OnCommandChanged));

	public static ICommand GetCommand(DependencyObject obj) => (ICommand)obj.GetValue(CommandProperty);
	public static void SetCommand(DependencyObject obj, ICommand value) => obj.SetValue(CommandProperty, value);

	private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is ItemsView itemsView)
		{
			itemsView.ItemInvoked -= ItemInvoked;

			if (e.NewValue is ICommand)
			{
				itemsView.ItemInvoked += ItemInvoked;
			}
		}
	}

	private static void ItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args)
	{
		var command = GetCommand(sender);
		if (command != null && command.CanExecute(args.InvokedItem))
		{
			command.Execute(args.InvokedItem);
		}
	}
}
