namespace QLKhachSanAPI.Models.DTOs.Pagination
{
    // generic paginated response structure:
    public class PaginatedResponse<T>
    {
        public int Pages { get; set; }
        public int Page { get; set; }
        public int Count { get; set; }
        public int Total { get; set; }
        public List<T>? Data { get; set; }
    }
}
