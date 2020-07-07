using System;
using System.Threading.Tasks;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using Renamer.Cazzar.Influx;
using Shoko.Server;
using Shoko.Server.Commands;
using Shoko.Server.Plugin;

namespace Renamer.Cazzar
{
    public class InfluxDbPlugin : IPlugin
    {

        private InfluxDBClient _influxDb = null;

        public string Name => "InfluxDB exporter";

        public void Load()
        {
            CheckInflux().ConfigureAwait(false).GetAwaiter().GetResult();
            ShokoService.CmdProcessorGeneral.OnQueueCountChangedEvent +=  e => QueueUpdate(e, "general");
            ShokoService.CmdProcessorHasher.OnQueueCountChangedEvent += e => QueueUpdate(e, "hasher");
            ShokoService.CmdProcessorImages.OnQueueCountChangedEvent += e => QueueUpdate(e, "images");
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
        }
    }
}