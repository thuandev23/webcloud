namespace QLKhachSanAPI.Models.Domains
{
    using System.ComponentModel.DataAnnotations;

    public class FeedbackToken
    {
        [Key]
        public string TokenID { get; set; } = Guid.NewGuid().ToString();
        public string GuestID { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryTime { get; set; }
        public DateTimeOffset DateCreated { get; set; } = GetCurrentTimeInDesiredTimeZone();

        // Navigation property for Guest
        public Guest Guest { get; set; }


        private static DateTime GetCurrentTimeInDesiredTimeZone()
        {
            TimeZoneInfo desiredTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // ((GMT+07:00) Bangkok, Hanoi, Jakarta)

            return TimeZoneInfo.ConvertTime(DateTime.Now, desiredTimeZone);
        }
    }
}
