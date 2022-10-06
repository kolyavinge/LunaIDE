using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Luna.IDE.Media;

public static class ImageCollection
{
    private static readonly Dictionary<string, ImageSource> _images = new();

    public static ImageSource? GetImage(string imageName)
    {
        if (_images.ContainsKey(imageName))
        {
            return _images[imageName];
        }
        else
        {
            var image = new BitmapImage(new Uri($"pack://application:,,,/Images/{imageName}"));
            _images.Add(imageName, image);
            return image;
        }
    }
}
