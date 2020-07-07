using InfluxDB.Client.Core;

namespace Renamer.Cazzar.Influx
{
    [Measurement("queues")]
    public class DataPoint
    {
        [Column("queue", IsTag = true)] public string Queue { get; set; }
        [Column("count")] public int Count { get; set; }
    }
}