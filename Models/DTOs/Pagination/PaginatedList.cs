namespace QLKhachSanAPI.Models.DTOs.Pagination
{
    using Microsoft.EntityFrameworkCore;


    public class PaginatedList<T> : List<T>
    {
        public int PageNumber { get; private set; }
        public int TotalPages { get; private set; }
        public int TotalCount { get; private set; }

        public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;

            AddRange(items);
        }

        public bool HasPreviousPage => PageNumber > 1;

        public bool HasNextPage => PageNumber < TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int itemsPerPage)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * itemsPerPage).Take(itemsPerPage).ToListAsync();
            return new PaginatedList<T>(items, count, pageNumber, itemsPerPage);
        }
        public static Task<PaginatedList<T>> CreateAsync(List<T> source, int pageNumber, int itemsPerPage)
        {
            var count = source.Count;
            var items = source.Skip((pageNumber - 1) * itemsPerPage).Take(itemsPerPage).ToList();
            return Task.FromResult(new PaginatedList<T>(items, count, pageNumber, itemsPerPage));
        }


    }
}
