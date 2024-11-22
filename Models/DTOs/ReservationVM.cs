using QLKhachSanAPI.Models.Domains;

namespace QLKhachSanAPI.Models.DTOs
{
    public class ReservationVM
    {
        public string? IdToUpdate { get; set; }
        public string GuestID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool? IsConfirmed { get; set; }
        public DateTime? ConfirmationTime { get; set; }
    }
}
