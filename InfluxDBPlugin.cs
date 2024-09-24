/*

using System;
using System.Threading.Tasks;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using Renamer.Cazzar.Influx;
using Shoko.Plugin.Abstractions;

namespace Renamer.Cazzar;
public class InfluxDbPlugin : IPlugin
{
    private readonly IShokoEventHandler _eventHandler;

    private InfluxDBClient _influxDb = null;

    public InfluxDbPlugin(IShokoEventHandler eventHandler)
    {
        _eventHandler = eventHandler;
    }

    public void OnSettingsLoaded(IPluginSettings settings)
    {
    }

    public string Name => "InfluxDB exporter";

    public void Load()
    {
        CheckInflux().ConfigureAwait(false).GetAwaiter().GetResult();
        // TODO Add events and redesign graphs for new queue
    }

    private async Task CheckInflux()
    {
        if (this._influxDb != null)
        {
            var health = await this._influxDb.HealthAsync();
            if (health.Status == HealthCheck.StatusEnum.Pass) return;
        }
        this._influxDb = InfluxDBClientFactory.CreateV1("http://192.168.1.3:8086", "", new char[0], "shoko", "autogen");
    }

    private async void QueueUpdate(QueueCountEventArgs e, string queue)
    {
        await CheckInflux();
        using var db = _influxDb.GetWriteApi();
        var datapoint = new DataPoint()
        {
            Count = e.QueueCount,
            Queue =  queue
        };
        db.WriteMeasurement("shoko", "default", WritePrecision.Ms, datapoint);
    }* /
}
*/