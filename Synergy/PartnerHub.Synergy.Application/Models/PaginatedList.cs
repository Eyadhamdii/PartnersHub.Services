namespace PartnersHub.Synergy.Application.Models;

public class PaginatedList<T>
{
    public List<T> List { get; set; }
    public int TotalCount { get; set; }
}
