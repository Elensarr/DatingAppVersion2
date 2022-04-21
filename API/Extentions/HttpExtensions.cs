using API.Helpers;
using System.Text.Json;

namespace API.Extentions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, int currentPage, int itemPerPage,
            int totalItems, int totalPages)
        {
            var paginationHeader = new PaginationHeader(currentPage, itemPerPage, totalItems, totalPages);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            //adding info to headers
            response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader, options));

            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}
