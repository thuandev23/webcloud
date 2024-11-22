namespace QLKhachSanAPI.Models.DTOs
{
    using QLKhachSanAPI.Models.Domains;
    using System.ComponentModel.DataAnnotations;

    public class ReservationViewModel
    {
        [Required(ErrorMessage = "Please provide the guest's full name.")]
        [MaxLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string GuestFullName { get; set; }

        [Required(ErrorMessage = "Please provide a valid phone number.")]
        [RegularExpression(@"^\+[0-9]{1,15}$", ErrorMessage = "Invalid phone number format.")]
        public string GuestPhoneNumber { get; set; }

        [Required(ErrorMessage = "Please provide  type email.")]
        public string GuestEmail { get; set; }
       

        [Required(ErrorMessage = "Please provide a room type id.")]
        public string RoomTypeId { get; set; }

        [Required(ErrorMessage = "Please specify the reservation start time.")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Please specify the reservation end time.")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime EndTime { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Please specify the number of rooms.")]
        [Range(1, int.MaxValue, ErrorMessage = "Number of rooms must be at least 1.")]
        public int NumberOfRooms { get; set; } = 1;

        public string? SpecialNote { get; set; }


    }

}
