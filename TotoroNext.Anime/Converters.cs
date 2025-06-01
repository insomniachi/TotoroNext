using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;

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

    public static ImageSource? StreamToImage(Stream? stream)
    {
        if(stream is null)
        {
            return null;
        }

        var bitmapImage = new BitmapImage();
        bitmapImage.SetSource(stream.AsRandomAccessStream());
        return bitmapImage;
    }

    public static ImageSource? StringToImage(string? uri)
    {
        if (uri is null)
        {
            return null;
        }

        return new BitmapImage(new Uri(uri));
    }

    public static Guid ToGuid(string guid) => Guid.Parse(guid);
}
