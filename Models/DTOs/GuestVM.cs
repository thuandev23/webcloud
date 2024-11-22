using System.ComponentModel.DataAnnotations;

namespace QLKhachSanAPI.Models.DTOs
{
    public class GuestVM
    {
        public string? IdToUpdate { get; set; }
       
        public string FullName { get; set; }
        public int Age { get; set; }
        public string? Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
    }
}


