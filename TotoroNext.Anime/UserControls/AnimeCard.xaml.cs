// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

using TotoroNext.Anime.Abstractions;

namespace TotoroNext.Anime.UserControls;
public sealed partial class AnimeCard : UserControl
{
    public AnimeCard()
    {
        InitializeComponent();
    }

    public AnimeModel? Anime
    {
        get { return (AnimeModel?)GetValue(AnimeProperty); }
        set { SetValue(AnimeProperty, value); }
    }

    public static readonly DependencyProperty AnimeProperty =
        DependencyProperty.Register("Anime", typeof(AnimeModel), typeof(AnimeCard), new PropertyMetadata(null));

}
