namespace QLKhachSanAPI.Models.Domains
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;

    public class Room
    {
        [Key]
        public string RoomID { get; set; } = Guid.NewGuid().ToString();
        public string RoomNumber { get; set; }
        public string RoomTypeID { get; set; }
        public bool IsAvaiable  { get; set; } // true is Avaiable true là rỗng
        public DateTimeOffset DateCreated { get; set; } = GetCurrentTimeInDesiredTimeZone();

        // Navigation properties
        public RoomType RoomType { get; set; }
        [JsonIgnore]
        public virtual List<ReservationRoom>? ReservationRooms { get; set; }

        private static DateTime GetCurrentTimeInDesiredTimeZone()
        {
            TimeZoneInfo desiredTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // ((GMT+07:00) Bangkok, Hanoi, Jakarta)

            return TimeZoneInfo.ConvertTime(DateTime.Now, desiredTimeZone);
        }
    }

}
