namespace ContactManager.WebSite.ViewModels.Shared;

public interface IPaginated {
    int PageIndex { get; }
    int TotalPages { get; }
    bool HasPreviousPage { get; }
    bool HasNextPage { get; }
}

public class PaginatedList<T> : IPaginated {
    public IReadOnlyList<T> Items { get; }
    public int PageIndex { get; }
    public int TotalPages { get; }
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    public PaginatedList(IReadOnlyList<T> items, int totalCount, int pageIndex, int pageSize) {
        Items = items;
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
    }
}
