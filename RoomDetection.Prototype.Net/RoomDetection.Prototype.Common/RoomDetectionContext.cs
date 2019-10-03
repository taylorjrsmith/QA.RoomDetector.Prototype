using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomDetection.Prototype.Common
{
    public class RoomDetectionContext : DbContext
    {
        private readonly IConfiguration Config;
        public DbSet<Room> Rooms { get; set; }

        public RoomDetectionContext()
        {
            Config = new ConfigurationBuilder().AddJsonFile("appsettings.db.json").Build();
        }

        public RoomDetectionContext(IConfiguration configuration)
        {
            Config = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Config.GetConnectionString("RoomDatabase"));
        }

        public object FirstOrdefault()
        {
            throw new NotImplementedException();
        }
    }
}
