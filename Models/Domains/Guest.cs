namespace QLKhachSanAPI.Models.Domains
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;

    public class Guest
    {
        [Key]
        public string GuestID { get; set; } = Guid.NewGuid().ToString();
       
        public string FullName { get; set; }
        public int Age { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTimeOffset DateCreated { get; set; } = GetCurrentTimeInDesiredTimeZone();

        // Navigation properties
        [JsonIgnore]
        public ICollection<Reservation>? Reservations { get; set; }
        public ICollection<GuestService>? GuestService { get; set; }
        public ICollection<Bill>? Bills { get; set; }

        private static DateTime GetCurrentTimeInDesiredTimeZone()
        {
            TimeZoneInfo desiredTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // ((GMT+07:00) Bangkok, Hanoi, Jakarta)

            return TimeZoneInfo.ConvertTime(DateTime.Now, desiredTimeZone);
        }
    }
}
