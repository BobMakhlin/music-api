using System.Text.Json.Serialization;

namespace Presentation.API.Models
{
    public class Track
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PreviewUrl { get; set; }
        public string AlbumImage { get; set; }
        // duration?
        // thirdparty url (to spotify)?
    }
}
