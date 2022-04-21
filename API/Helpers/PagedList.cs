using Microsoft.EntityFrameworkCore;

namespace API.Helpers
{
    public class PagedList<T> : List<T> // inherit from list, T - generic type, can take any type of entity
    {
        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize) // items - get from query
        {
            // why capital letter?
            CurrentPage = pageNumber;
            //???      
            // why need (double)
            TotalPages = (int)Math.Ceiling(count / (double) pageSize);
            PageSize = pageSize;
            TotalCount = count;
            
            // ??? 
            // where does it add 
            AddRange(items);
        }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            //1st query to get count
            var count = await source.CountAsync();
            //2nd query
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}