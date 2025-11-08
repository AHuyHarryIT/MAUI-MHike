using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MAUI_MHike.Models;

[Table("Observations")]
public class Observation
{
    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [Indexed]               // index for hike filter
    public string HikeId { get; set; } = string.Empty;

    [NotNull]
    public string Note { get; set; } = string.Empty;

    // store as epoch seconds to mirror Android approach
    [NotNull, Indexed]
    public long TimeSec { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();

    public string? Comments { get; set; }

    [Ignore]
    public DateTimeOffset Time => DateTimeOffset.FromUnixTimeSeconds(TimeSec);
}
