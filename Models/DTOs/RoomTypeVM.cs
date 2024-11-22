using QLKhachSanAPI.Models.Domains;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace QLKhachSanAPI.Models.DTOs
{
    public class RoomTypeVM
    {
        public string? IdToUpdate { get; set; }
        public string? Name { get; set; }
        //public int Capacity { get; set; }
        public double? AreaInSquareMeters { get; set; }
        public string? Description { get; set; }
        public double DailyPrice { get; set; } // decimal is hard for future LinQ query handling
        
        public DateTimeOffset DateCreated { get; set; }

    }
}
