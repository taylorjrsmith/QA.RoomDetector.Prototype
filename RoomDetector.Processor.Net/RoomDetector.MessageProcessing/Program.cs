using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using RoomDetector.EventProcessing;
using System;
using System.Threading.Tasks;

namespace Globomantics.MessageProcessor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var hubName = "iothub-ehub-tayloriot-2072611-c8e858b539";
            var iotHubConnectionString = "Endpoint=sb://ihsuproddmres007dednamespace.servicebus.windows.net/;SharedAccessKeyName=iothubowner;SharedAccessKey=CXRnMpKJe8lTLdx4SVZdcxgs2CRjCAEGJX9RgHnx0z8=;EntityPath=iothub-ehub-tayloriot-2072611-c8e858b539";
            var storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=iotstoragetjrs;AccountKey=yD8DeVqKqqEQ7xKAyU02CJ6dJQvFdOC4OzrwM7IcVsqn6k8e6z3fCaqrE7u3YeCUf78ytNfd62oypuySp3Bu0Q==;EndpointSuffix=core.windows.net";
            var storageContainerName = "message-processor-host";
            var consumerGroupName = PartitionReceiver.DefaultConsumerGroupName;

            var processor = new EventProcessorHost(hubName, consumerGroupName, iotHubConnectionString, storageConnectionString, storageContainerName);

            await processor.RegisterEventProcessorAsync<RoomEventProcessor>();

            Console.WriteLine("Event processor started, press enter to exit...");
            Console.ReadLine();
            await processor.UnregisterEventProcessorAsync();
        }

    }
}
