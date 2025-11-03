namespace PartnersHub.InfraBase.Application.Common.Models;

/// <summary>
/// Represents a paginated list of items
/// </summary>
/// <typeparam name="T">The type of items in the list</typeparam>
public class PaginatedList<T>
{
    /// <summary>
    /// The items in the current page
    /// </summary>
    public List<T> Items { get; init; } = new();

    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public int PageNumber { get; init; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Total number of items across all pages
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// Indicates if there is a previous page
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Indicates if there is a next page
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Creates an empty paginated list
    /// </summary>
    public static PaginatedList<T> Empty(int pageNumber = 1, int pageSize = 10)
    {
        return new PaginatedList<T>
        {
            Items = new List<T>(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = 0
        };
    }

    /// <summary>
    /// Creates a paginated list from items and total count
    /// </summary>
    public static PaginatedList<T> Create(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
    {
        return new PaginatedList<T>
        {
            Items = items.ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
