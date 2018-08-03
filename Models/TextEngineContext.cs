using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Sqlite;
using System.ComponentModel.DataAnnotations;

namespace TextEngine.Models
{
    public class TextEngineContext : DbContext
    {
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Door> Doors { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<DoorItemCondition> DoorItemConditions { get; set; }
        public DbSet<DoorExitEvent> DoorExitEvent { get; set; }
        public DbSet<DoorEntryEvent> DoorEntryEvent { get; set; }
        public DbSet<MonsterType> MonsterTypes { get; set; }
        public DbSet<Monster> Monsters { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = "TextEngineData.sqlite" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);

            optionsBuilder.UseSqlite(connection);
        }

        /*
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Door>()
                .HasOne(x => x.TargetRoom);
        }
    */

        public void Seed(TextEngineContext context)
        {
            Room r = new Room();
            r.Name = "default";
            r.Description = "default";
            context.Rooms.Add(r);
            context.Inventories.Add(r.Inventory);
            context.SaveChanges();
        }
    }
}
