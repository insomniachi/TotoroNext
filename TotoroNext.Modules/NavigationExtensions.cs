using Microsoft.Extensions.DependencyInjection;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Module;

public static class NavigationExtensions
{

    public static bool GetIsAttached(UIElement obj)
    {
        return (bool)obj.GetValue(IsAttachedProperty);
    }

    public static void SetIsAttached(UIElement obj, bool value)
    {
        obj.SetValue(IsAttachedProperty, value);
    }

    public static readonly DependencyProperty IsAttachedProperty =
        DependencyProperty.RegisterAttached("IsAttached", typeof(bool), typeof(NavigationExtensions), new PropertyMetadata(false, OnIsAttachedChanged));

    private static void OnIsAttachedChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not FrameworkElement element)
        {
            return;
        }

        if (args.NewValue is not true)
        {
            return;
        }

        var navigator = ActivatorUtilities.CreateInstance<ControlNavigator>(Container.Services, element);

        element.DataContextChanged += (_, args) =>
        {
            if (args.NewValue is INavigatorHost nh)
            {
                nh.Navigator = navigator;
            }
        };
    }

    public static double GetPaneWidth(UIElement obj)
    {
        return (double)obj.GetValue(PaneWidthProperty);
    }

    public static void SetPaneWidth(UIElement obj, double value)
    {
        obj.SetValue(PaneWidthProperty, value);
    }

    public static readonly DependencyProperty PaneWidthProperty =
        DependencyProperty.RegisterAttached("PaneWidth", typeof(double), typeof(NavigationExtensions), new PropertyMetadata(null));

}
