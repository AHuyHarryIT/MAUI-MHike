using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUI_MHike.Models
{
    public class Hike
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Today;
        public bool Parking { get; set; }
        public double LengthKm { get; set; }
        public int Difficulty { get; set; } = 3; // 1..5
        public string Description { get; set; } = string.Empty;

        public string DifficultyLabel => Difficulty switch
        {
            1 => "Very Easy",
            2 => "Easy",
            3 => "Medium",
            4 => "Hard",
            5 => "Very Hard",
            _ => "Unknown"
        };

        public string Meta => $"{(Parking ? "Parking • " : "No parking • ")}{LengthKm:0.##} km • {DifficultyLabel}";
    }
}
