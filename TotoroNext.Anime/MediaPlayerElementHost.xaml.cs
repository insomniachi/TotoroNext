using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Input;
using TotoroNext.MediaEngine.Abstractions;
using TotoroNext.Module;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace TotoroNext.Anime;
public sealed partial class MediaPlayerElementHost : UserControl
{
    public MediaPlayerElementHost()
    {
        InitializeComponent();
    }

    public IMediaPlayer? Player
    {
        get
        {
            try
            {
                return (IMediaPlayer)GetValue(PlayerProperty);
            }
            catch
            {
                return null;
            }
        }
        set { SetValue(PlayerProperty, value); }
    }

    public string EngineType
    {
        get { return (string)GetValue(EngineTypeProperty); }
        set { SetValue(EngineTypeProperty, value); }
    }

    public static readonly DependencyProperty EngineTypeProperty =
        DependencyProperty.Register("EngineType", typeof(string), typeof(MediaPlayerElementHost), new PropertyMetadata("", OnEngineTypeChanged));

    public static readonly DependencyProperty PlayerProperty =
        DependencyProperty.Register("Player", typeof(IMediaPlayer), typeof(MediaPlayerElementHost), new PropertyMetadata(null));

    private static void OnEngineTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is not string s)
        {
            return;
        }

        if(string.IsNullOrEmpty(s))
        {
            return;
        }

        var host = (MediaPlayerElementHost)d;
        var factory = Container.Services.GetKeyedService<IMediaPlayerElementFactory>(s);
        if(factory is null)
        {
            return;
        }
        var player = factory.CreatePlayer();
        var element = factory.CreateElement(player);
        host.RootGrid.Children.Insert(0, element);
        
        host.DispatcherQueue.TryEnqueue(() =>
        {
            host.Player = player;
        });
    }

    private void RootGrid_PointerMoved(object sender, PointerRoutedEventArgs e)
    {

    }

    private void RootGrid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        var x = Player;
    }
}
