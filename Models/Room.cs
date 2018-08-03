using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TextEngine.Models
{
    public class Room
    {
        public long RoomID { get; set; }

        // the coordinates of this room.
        // so far this is only for drawing the map
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public List<Door> Doors { get; set; }
        public List<Monster> Monsters { get; set; }
        public Inventory Inventory { get; set; }

        public Room()
        {
            if (this.Inventory == null)
            {
                this.Inventory = new Inventory();
            }

            if (this.Monsters == null)
            {
                this.Monsters = new List<Monster>();
            }

            if (this.Doors == null)
            {
                this.Doors = new List<Door>();
            }
        }

        public static Room ByID(TextEngineContext context, long ID)
        {
            return context.Rooms
                .Where(r => r.RoomID == ID)
                .Include(r => r.Doors)
                .Include(r => r.Inventory)
                .Include(r => r.Monsters).ThenInclude(m => m.MonsterType)
                .Include(r => r.Monsters).ThenInclude(m => m.Inventory)
                .Include(r => r.Monsters).ThenInclude(m => m.Inventory).ThenInclude(i => i.Items)
                .FirstOrDefault();
        }

        public bool DoorExists(Direction direction)
        {
            return this.Doors.Exists(d => d.Direction == direction);
        }

        public void Move(Direction direction)
        {
            Door door = null;
            using (var context = new TextEngineContext())
            {
                door = (from d in context.Doors
                    .Include(d => d.TargetRoom).ThenInclude(r => r.Doors)
                    .Include(d => d.DoorItemConditions).ThenInclude(c => c.Item).ThenInclude(i => i.Inventory)
                    .Where(d => d.SourceRoom.RoomID == Program.character.CurrentRoom.RoomID && d.Direction == direction)
                        select d).FirstOrDefault();
                if (door == null)
                {
                    Utility.Print(ConsoleColor.Yellow, "There doesn't appear to be an exit in that direction.");
                    return;
                }
            }

            // are there item conditions?
            bool itemConditionsMet = true;
            foreach (var cond in door.DoorItemConditions)
            {
                if (Program.character.Inventory.Items.FirstOrDefault(x => x.ItemID == cond.Item.ItemID) == null)
                {
                    Utility.Print(ConsoleColor.Yellow, "You must possess the " + cond.Item.Name + " to unlock this door!");
                    itemConditionsMet = false;
                }
                else
                {
                    Utility.Print(ConsoleColor.Yellow, "You successfully unlock the door using the " + cond.Item.Name + "!");
                    Console.WriteLine();
                }
            }

            if (!itemConditionsMet)
            {
                return;
            }

            /*
            Room TargetRoom = door == null ? null : door.TargetRoom;

            if (TargetRoom == null)
            {
                var defaultColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("There is no exit in that direction!");
                Console.ForegroundColor = defaultColor;
                return;
            }
            */

            Program.character.CurrentRoom = door.TargetRoom;
            using (var context = new TextEngineContext())
            {
                Character c = Character.GetInstance(context);
                Room room = Room.ByID(context, door.TargetRoom.RoomID);
                c.CurrentRoom = room;
                context.SaveChanges();
                Program.RefreshGameState(context);
            }
            Program.character.CurrentRoom.Look();
        }

        public void Look()
        {
            var defaultColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(this.Name);

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(this.Description);

            if (this.Inventory.Items.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("You notice " + string.Join(", ", this.Inventory.Items) + " here.");
            }

            if (this.Monsters.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("Also here: " + string.Join(", ", this.Monsters));
            }

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            string exits = this.Doors != null && this.Doors.Count > 0 ? string.Join(", ", this.Doors) : "None";
            Console.WriteLine("Obvious exits: " + exits);
            Console.ForegroundColor = defaultColor;
        }

        public string[] getMap()
        {
            string[] roomMap = new string[3];

            // build first line
            roomMap[0] = DoorExists(Direction.Northwest) ? "\\" : " ";
            roomMap[0] += DoorExists(Direction.North) ? "|" : " ";
            roomMap[0] += DoorExists(Direction.Northeast) ? "/" : " ";

            roomMap[1] = DoorExists(Direction.West) ? "-" : " ";
            roomMap[1] += this.RoomID == Program.character.CurrentRoom.RoomID ? "*" : "#";
            roomMap[1] += DoorExists(Direction.East) ? "-" : " ";

            roomMap[2] = DoorExists(Direction.Southwest) ? "/" : " ";
            roomMap[2] += DoorExists(Direction.South) ? "|" : " ";
            roomMap[2] += DoorExists(Direction.Southeast) ? "\\" : " ";

            return roomMap;
        }




    }
}
