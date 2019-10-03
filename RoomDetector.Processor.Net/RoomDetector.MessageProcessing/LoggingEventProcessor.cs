using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace RoomDetector.EventProcessing
{
    public class RoomStatusTelemetry
    {
        public string RoomStatus { get; set; }
        public DateTime TimeStampSince { get; set; }
        public string DeviceId { get; set; }

    }

    class RoomEventProcessor : IEventProcessor
    {
        //this method is called when the processor stops processing a partition
        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            return Task.CompletedTask;
        }

        //this method is called when our processor is attached to a new partition for the hub
        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine($"LoggingEventProcessor opened, procession partition : '{context.PartitionId}'");
            return Task.CompletedTask;
        }

        //this method will be called anytime something goes wrong with our processor
        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            return context.CheckpointAsync();
        }

        //this method is called when data enters the hub
        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            var connection = new HubConnectionBuilder().WithUrl("https://roomdetectionprototypenet.azurewebsites.net/roomdetector").Build();
            try
            {
                await connection.StartAsync();

                Console.WriteLine($"Batch of events received on partition '{context.PartitionId}'.");
                foreach (var eventData in messages)
                {
                    var deviceId = eventData.SystemProperties["iothub-connection-device-id"];
                    var payload = GetJsonFromEventData(eventData);
                    Console.WriteLine($"Message received on partition '{context.PartitionId}', device ID: {deviceId}, payload: {payload}");
                    var telemetry = JsonConvert.DeserializeObject<RoomStatusTelemetry>(payload);
                    telemetry.DeviceId = deviceId.ToString();
                    await connection.InvokeAsync("ReceiveRoomUpdate", JsonConvert.SerializeObject(telemetry));


                }
            }
            catch (Exception ex)
            {
                await connection.StopAsync();
            }

            await context.CheckpointAsync();
        }

        private void SendFirstRespondersTo(decimal latitude, decimal longitude)
        {
            Console.WriteLine($"** First responders dispatched to ({latitude}, {longitude})");
        }

        public string GetJsonFromEventData(EventData data)
        {
            return Encoding.ASCII.GetString(data.Body.Array, data.Body.Offset, data.Body.Count);
        }
    }
}
