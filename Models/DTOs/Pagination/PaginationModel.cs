namespace QLKhachSanAPI.Models.DTOs.Pagination
{
    using System.ComponentModel.DataAnnotations;

    public class PaginationModel
    {
        [Range(1, Int16.MaxValue, ErrorMessage = "The field {0} must be greater than {1}.")]
        public int pageNumber { get; set; }
        public int itemsPerPage { get; set; }
        public int totalCount { get; set; }
    }
}
