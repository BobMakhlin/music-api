namespace Application.Models
{
    public record Track(string Id, string Name, string PreviewUrl, string AlbumImage,
        TimeSpan Duration, string ExternalUrl);
}
