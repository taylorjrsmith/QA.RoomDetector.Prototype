using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoomDetection.Prototype.Common
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DeviceId { get; set; }
        public string RoomStatus { get; set; }
        public DateTime TimeStampSince { get; set; }
        //[NotMapped]
        //public string RoomIconClass
        //{
        //    get
        //    {
        //        return RoomType switch
        //        {
        //            RoomType.MeetingRoom => "fas fa-toilet",
        //            RoomType.Toilet => "fas fa-door-open",
        //            _ => "fas door-closed"
        //        };
        //    }
        //    }
    }

    public enum RoomType
    {
        MeetingRoom = 1,
        Toilet = 0
    }
}
