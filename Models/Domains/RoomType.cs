namespace QLKhachSanAPI.Models.Domains
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;

    public class RoomType
    {
        [Key]
        public string RoomTypeID { get; set; } = Guid.NewGuid().ToString();
        public string? Name { get; set; }
        //public int? Capacity { get; set; } // no need for capacity
        public double? AreaInSquareMeters { get; set; }
        public string? Description { get; set; }
       
        public double DailyPrice { get; set; } // decimal is hard for future LinQ query handling
       

        public DateTimeOffset DateCreated { get; set; } = GetCurrentTimeInDesiredTimeZone();

        // Navigation property for rooms of this room type
        [JsonIgnore]
        public ICollection<Room>? Rooms { get; set; }


        private static DateTime GetCurrentTimeInDesiredTimeZone()
        {
            TimeZoneInfo desiredTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // ((GMT+07:00) Bangkok, Hanoi, Jakarta)

            return TimeZoneInfo.ConvertTime(DateTime.Now, desiredTimeZone);
        }
    }

}
