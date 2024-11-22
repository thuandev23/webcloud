using System.Text.Json.Serialization;

namespace QLKhachSanAPI.Models.Domains
{
    public class Service
    {
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public DateTimeOffset DateCreated { get; set; } = GetCurrentTimeInDesiredTimeZone();

        private static DateTime GetCurrentTimeInDesiredTimeZone()
        {
            TimeZoneInfo desiredTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // ((GMT+07:00) Bangkok, Hanoi, Jakarta)

            return TimeZoneInfo.ConvertTime(DateTime.Now, desiredTimeZone);
        }

      
        [JsonIgnore]
        public virtual List<GuestService>? GuestService { get; set; }
    }
}
