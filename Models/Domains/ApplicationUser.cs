namespace QLKhachSanAPI.Models
{
    using Microsoft.AspNetCore.Identity;
    using QLKhachSanAPI.Models.Domains;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text.Json.Serialization;

    public class ApplicationUser : IdentityUser
    {
        // additional properties will go here

        // https://www.entityframeworktutorial.net/efcore/one-to-many-conventions-entity-framework-core.aspx

        [Column(TypeName = "nvarchar(150)")]
        [MaxLength(100)]
        public string FullName { get; set; }    // not null
        public DateTimeOffset? DateJoined { get; set; } = GetCurrentTimeInDesiredTimeZone();  // DateTime.Now;
      
        [MaxLength(260)]
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }


        private static DateTime GetCurrentTimeInDesiredTimeZone()
        {
            TimeZoneInfo desiredTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // ((GMT+07:00) Bangkok, Hanoi, Jakarta)

            return TimeZoneInfo.ConvertTime(DateTime.Now, desiredTimeZone);
        }
    }
}
