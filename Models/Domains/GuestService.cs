using Microsoft.EntityFrameworkCore.Metadata;
using System.ComponentModel.DataAnnotations;

namespace QLKhachSanAPI.Models.Domains
{
    public class GuestService
    {
        public string GuestID { get; set; } 
        public string ServiceID { get; set; }
        public int Number {  get; set; }
        
        public DateTimeOffset DateCreated { get; set; } = GetCurrentTimeInDesiredTimeZone();

        // Navigation properties
        public Guest Guest { get; set; }
        public Service Service { get; set; }

        private static DateTime GetCurrentTimeInDesiredTimeZone()
        {
            TimeZoneInfo desiredTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // ((GMT+07:00) Bangkok, Hanoi, Jakarta)

            return TimeZoneInfo.ConvertTime(DateTime.Now, desiredTimeZone);
        }
    }
}
