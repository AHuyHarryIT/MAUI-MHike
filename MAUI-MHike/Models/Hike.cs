using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUI_MHike.Models;


[Table("Hikes")]
public class Hike
{
    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [NotNull, Indexed]
    public string Name { get; set; } = string.Empty;

    [NotNull, Indexed]
    public string Location { get; set; } = string.Empty;

    [NotNull, Indexed]
    public string DateIso { get; set; } = DateTime.Today.ToString("yyyy-MM-dd");

    public bool Parking { get; set; }

    public double LengthKm { get; set; }

    public int Difficulty { get; set; } = 3; // 1..5

    public string Description { get; set; } = string.Empty;

    [Ignore]
    public DateTime Date
    {
        get => DateTime.TryParse(DateIso, out var d) ? d : DateTime.Today;
        set => DateIso = value.ToString("yyyy-MM-dd");
    }

    [Ignore]
    public string DifficultyLabel => Difficulty switch
    {
        1 => "Very Easy",
        2 => "Easy",
        3 => "Medium",
        4 => "Hard",
        5 => "Very Hard",
        _ => "Unknown"
    };

    [Ignore]
    public string Meta => $"{(Parking ? "Parking • " : "No parking • ")}{LengthKm:0.##} km • {DifficultyLabel}";
}

