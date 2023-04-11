namespace Presentation.API.Models
{
    public record Track(string Id, string Name, string PreviewUrl, string AlbumImage,
        Duration Duration, string ExternalUrl);
}
