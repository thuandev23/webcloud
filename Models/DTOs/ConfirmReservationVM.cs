namespace QLKhachSanAPI.Models.DTOs
{
    using System.ComponentModel.DataAnnotations;

    public class ConfirmReservationVM
    {
        [Required]
        public string ReservationId { get; set; }
        [Required]
        public bool Confirm { get; set; }
    }
}
