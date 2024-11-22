    namespace QLKhachSanAPI.Models.Domains
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;

    public class Reservation
    {
        [Key]
        public string ReservationID { get; set; } = Guid.NewGuid().ToString();
        public string GuestID { get; set; }
      
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
       
        public bool? IsConfirmed { get; set; }
        public DateTime? ConfirmationTime { get; set; }

        public DateTimeOffset DateCreated { get; set; } = GetCurrentTimeInDesiredTimeZone();

        // Navigation properties
        public Guest? Guest { get; set; }
        [JsonIgnore]
        public virtual List<ReservationRoom>? ReservationRooms { get; set; }

        private static DateTime GetCurrentTimeInDesiredTimeZone()
        {
            TimeZoneInfo desiredTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // ((GMT+07:00) Bangkok, Hanoi, Jakarta)

            return TimeZoneInfo.ConvertTime(DateTime.Now, desiredTimeZone);
        }
    }
}
