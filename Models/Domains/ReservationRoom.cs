namespace QLKhachSanAPI.Models.Domains
{
    using System.ComponentModel.DataAnnotations;


    public class ReservationRoom
    {   
        // No need for [Key] here !!!
        //public string Id { get; set; } = Guid.NewGuid().ToString();
         // Must have room id and reservation ID for reservation
        public string RoomID { get; set; }
        public string ReservationID { get; set; }
        public DateTimeOffset DateCreated { get; set; } = GetCurrentTimeInDesiredTimeZone();


        // Navigation properties
        public Room Room { get; set; }
        public Reservation Reservation { get; set; }


        private static DateTime GetCurrentTimeInDesiredTimeZone()
        {
            TimeZoneInfo desiredTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // ((GMT+07:00) Bangkok, Hanoi, Jakarta)

            return TimeZoneInfo.ConvertTime(DateTime.Now, desiredTimeZone);
        }
    }

}
