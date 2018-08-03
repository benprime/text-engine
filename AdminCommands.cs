using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using TextEngine.Models;

namespace TextEngine
{
    public static class AdminCommands
    {
        public static void Create(string[] args)
        {
            string type = args[1].ToLower();
            string value = string.Join(" ", args.Skip(2));

            switch (type)
            {
                case "room":
                    Direction direction = Utility.StringToDirection(args[2]);
                    AdminCommands.CreateRoom(direction);
                    break;
                case "item":
                    AdminCommands.CreateItem(value);
                    break;
                case "monster":
                    AdminCommands.CreateMonsterType(value);
                    break;
                default:
                    Utility.Print(ConsoleColor.Yellow, "Invalid command.");
                    break;
            }
        }

        public static void SpawnMonster(string[] cmdParts)
        {
            string monsterName = string.Join(" ", cmdParts.Skip(1)).ToLower();
            using (var context = new TextEngineContext())
            {
                var monsterType = context.MonsterTypes.FirstOrDefault(m => m.Name.ToLower() == monsterName);
                if(monsterType == null)
                {
                    Utility.Print(ConsoleColor.Yellow, "Invalid monster type!");
                    return;
                }

                var monster = new Monster();
                monster.MonsterType = monsterType;
                //Room room = Room.ByID(context, Program.character.CurrentRoom.RoomID);
                monster.Room = Program.character.CurrentRoom;
                context.Monsters.Add(monster);
                context.Inventories.Add(monster.Inventory);
                context.SaveChanges();
                Program.RefreshGameState(context);
            }

        }

        public static void ListMonsterTypes()
        {
            using (var context = new TextEngineContext())
            {
                Utility.Print(ConsoleColor.Blue, "Name\tTypeID\tHP\tHit%\tDmgMin\tDmgMax");
                foreach (MonsterType mt in context.MonsterTypes)
                {
                    Utility.Print(ConsoleColor.Blue, mt.Name + "\t" + mt.MonsterTypeID + "\t" + mt.HP + "\t" + mt.HitChance + "\t" + mt.DamageMinimum + "\t" + mt.DamageMaximum);
                }
            }
        }

        private static void CreateMonsterType(string value)
        {
            using (var context = new TextEngineContext())
            {
                var m = context.MonsterTypes.FirstOrDefault(mon => mon.Name.ToLower() == value.ToLower());
                if (m != null)
                {
                    Utility.Print(ConsoleColor.Yellow, "A monster with that name already exists!");
                    return;
                }

                m = new MonsterType();
                m.Name = value;
                context.MonsterTypes.Add(m);
                context.SaveChanges();
            }
        }

        internal static void Give(string[] cmdParts)
        {
            if (cmdParts.Length < 3)
            {
                Utility.Print(ConsoleColor.Yellow, "Invalid command!");
                return;
            }

            string monsterName = cmdParts[1].ToLower();
            string itemName = string.Join(" ", cmdParts.Skip(2)).ToLower();

            using (var context = new TextEngineContext())
            {
                Room room = Room.ByID(context, Program.character.CurrentRoom.RoomID);
                Monster monster = room.Monsters.FirstOrDefault(m => m.MonsterType.Name.ToLower() == monsterName);
                if(monster == null)
                {
                    Utility.Print(ConsoleColor.Yellow, "You don't see that monster here!");
                    return;
                }
                Item item = Character.GetInstance(context).Inventory.Items.FirstOrDefault(i => i.Name.ToLower() == itemName);
                if (item == null)
                {
                    Utility.Print(ConsoleColor.Yellow, "You don't posses that item!");
                    return;
                }

                item.Inventory = monster.Inventory;
                context.SaveChanges();
                Program.RefreshGameState(context);
            }
        }

        private static void CreateItem(string name)
        {

            using (var context = new TextEngineContext())
            {
                var room = Room.ByID(context, Program.character.CurrentRoom.RoomID);
                //room.Inventory = Program.character.CurrentRoom.Inventory;
                var item = new Item();
                item.Name = name;
                item.Description = "default description";
                item.Inventory = room.Inventory;
                context.Items.Add(item);
                context.SaveChanges();
                Program.RefreshGameState(context);
            }
        }

        public static void Set(string[] args)
        {
            if (args.Length < 3)
            {
                Utility.Print(ConsoleColor.Yellow, "Set command incorrect!\nExample: set title This is a title.");
                return;
            }

            string type = args[1].ToLower();
            string value = string.Join(" ", args.Skip(2));
            switch (type)
            {
                case "name":
                    Program.character.CurrentRoom.Name = value;
                    using (var context = new TextEngineContext())
                    {
                        Room room = context.Rooms.Where(x => x.RoomID == Program.character.CurrentRoom.RoomID).First();
                        room.Name = value;
                        context.SaveChanges();
                    }
                    break;
                case "desc":
                    Program.character.CurrentRoom.Description = value;
                    using (var context = new TextEngineContext())
                    {
                        Room room = context.Rooms.Where(x => x.RoomID == Program.character.CurrentRoom.RoomID).First();
                        room.Description = value;
                        context.SaveChanges();
                    }
                    break;
                case "n":
                case "s":
                case "e":
                case "w":
                case "nw":
                case "ne":
                case "sw":
                case "se":
                case "u":
                case "d":
                    AdminCommands.SetDoorCondition(args);
                    break;
                case "item":
                    AdminCommands.SetItem(args);
                    break;
                default:
                    Utility.Print(ConsoleColor.Yellow, "Invalid command.");
                    break;
            }

        }

        private static void SetItem(string[] args)
        {
            using (var context = new TextEngineContext())
            {
                Item item = Item.FromID(context, args[3]);
                if (item == null)
                {
                    Utility.Print(ConsoleColor.Yellow, "Invalid item!");
                    return;
                }

                // todo: validate these
                var attribute = args[2];
                var value = string.Join(" ", args.Skip(4));
                switch (attribute)
                {
                    case "name":
                        item.Name = value;
                        break;
                    case "desc":
                        item.Description = value;
                        break;
                }
                context.SaveChanges();
                Program.RefreshGameState(context);
            }
        }

        private static void SetDoorCondition(string[] args)
        {
            if (args.Length < 3)
            {
                Utility.Print(ConsoleColor.Yellow, "Invalid command.");
                return;
            }

            string dir = args[1];
            string type = args[2];
            string itemId = args[3];

            using (var context = new TextEngineContext())
            {

                switch (args[2])
                {
                    case "item":
                        Item item = Item.FromID(context, args[3]);
                        if (item == null)
                        {
                            Utility.Print(ConsoleColor.Yellow, "Invalid item!");
                            return;
                        }
                        Door door = Door.FromDir(context, dir);
                        if (item == null)
                        {
                            Utility.Print(ConsoleColor.Yellow, "Invalid direction!");
                            return;
                        }

                        var doorItemCondition = new DoorItemCondition();
                        doorItemCondition.Item = item;
                        doorItemCondition.Door = door;
                        context.DoorItemConditions.Add(doorItemCondition);
                        context.SaveChanges();
                        break;
                    default:
                        Utility.Print(ConsoleColor.Yellow, "Invalid command!");
                        break;
                }
            }
        }

        private static void CreateRoom(Direction direction)
        {
            if (direction == Direction.None)
            {
                Utility.Print(ConsoleColor.Yellow, "Invalid direction.");
                return;
            }

            // check for existing room
            var door = Program.character.CurrentRoom.Doors.Where(d => d.Direction == direction).FirstOrDefault();
            if (door != null)
            {
                Utility.Print(ConsoleColor.Yellow, "A room already exists in that location.");
                return;
            }

            // figure out the coordinates of the new room.
            // this seems a little clunky to me, but it will do for now.
            int x = Program.character.CurrentRoom.X;
            int y = Program.character.CurrentRoom.Y;
            int z = Program.character.CurrentRoom.Z;

            // autofill is REALLY handy here...
            switch (direction)
            {
                case Direction.North:
                    ++y;
                    break;
                case Direction.South:
                    --y;
                    break;
                case Direction.East:
                    ++x;
                    break;
                case Direction.West:
                    --x;
                    break;
                case Direction.Northwest:
                    ++y;
                    --x;
                    break;
                case Direction.Southeast:
                    --y;
                    ++x;
                    break;
                case Direction.Northeast:
                    ++y;
                    ++x;
                    break;
                case Direction.Southwest:
                    --y;
                    --x;
                    break;
                case Direction.Up:
                    --z;
                    break;
                case Direction.Down:
                    ++z;
                    break;
            }

            // create the room and doors
            using (var context = new TextEngineContext())
            {
                Room r = new Room();
                r.Name = "New Room";
                r.Description = "New Room Description";
                r.X = x;
                r.Y = y;
                r.Z = z;
                context.Rooms.Add(r);

                context.Inventories.Add(r.Inventory);

                // initial door
                Door d = new Door();
                d.Direction = direction;
                d.SourceRoom = Program.character.CurrentRoom;
                d.TargetRoom = r;
                context.Doors.Add(d);

                // and back again
                Door d2 = new Door();
                d2.Direction = Utility.DirectionOpposite(direction);
                d2.SourceRoom = r;
                d2.TargetRoom = Program.character.CurrentRoom;
                context.Doors.Add(d2);

                context.SaveChanges();
            }

            Program.character.CurrentRoom.Look();
        }
    }
}
