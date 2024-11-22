using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace QLKhachSanAPI.Models.Domains
{
    public class Bill
    {
        [Key]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public double Sum { get; set; }
        public bool Status { get; set; }
        public string IDGuest { get; set; }
        
        public DateTimeOffset DateCreated { get; set; } = GetCurrentTimeInDesiredTimeZone();

        private static DateTime GetCurrentTimeInDesiredTimeZone()
        {
            TimeZoneInfo desiredTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // ((GMT+07:00) Bangkok, Hanoi, Jakarta)

            return TimeZoneInfo.ConvertTime(DateTime.Now, desiredTimeZone);
        }

        public Guest Guest { get; set; }
       

    }
}
