using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RoomDetection.Prototype.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomDetection.Prototype.Net.Hubs
{
    public class RoomDetectorHub : Hub
    {
        public IConfiguration Configuration { get; set; }

        public RoomDetectorHub(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public async Task ReceiveRoomUpdate(string data)
        {
            
            var dataRoom = JsonConvert.DeserializeObject<Room>(data);
            var RoomContext = new RoomDetectionContext(Configuration);
            var Room = RoomContext.Rooms.FirstOrDefault(r => r.DeviceId == dataRoom.DeviceId);
            Room.TimeStampSince = dataRoom.TimeStampSince;
            Room.RoomStatus = dataRoom.RoomStatus;
            RoomContext.Update(Room);
            await RoomContext.SaveChangesAsync();
            await Clients.All.SendAsync("UpdateData", Room);
        }
    }
}
