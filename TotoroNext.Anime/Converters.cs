using Microsoft.UI.Xaml.Media.Imaging;

namespace TotoroNext.Anime;

public static class Converters
{
    public static ImageSource? UriToImage(Uri? uri)
    {
        if(uri is null)
        {
            return null;
        }

        return new BitmapImage(uri);
    }
}
