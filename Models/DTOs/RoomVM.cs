using QLKhachSanAPI.Models.Domains;
using System.Text.Json.Serialization;

namespace QLKhachSanAPI.Models.DTOs
{
    public class RoomVM
    {
        public string? IdToUpdate { get; set; }
        public string RoomNumber { get; set; }
        public string RoomTypeID { get; set; }
        public bool IsAvaiable { get; set; } 
        public DateTimeOffset DateCreated { get; set; }

    }
}
