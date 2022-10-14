using System.Windows.Media;

namespace Luna.IDE.App.Media;

public interface IImageCollection
{
    ImageSource? GetImage(string imageName);
}
