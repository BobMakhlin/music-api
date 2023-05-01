namespace Application.Models
{
    public record FilterTracksQuery(string? Name,
            string? Artist,
            string? Genre,
            string? Album,
            int? YearFrom,
            int? YearTo);
}
