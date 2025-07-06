// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

using Microsoft.UI.Xaml.Media.Animation;

namespace TotoroNext.Module.Controls;
public sealed partial class Shimmer : UserControl
{
    private Storyboard? _shimmerStoryboard;

    public Shimmer()
    {
        InitializeComponent();
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        LoaderAnimation.Begin();
    }

    public static IEnumerable<int> ShimmerSource(int count) => Enumerable.Range(0, count);
}
