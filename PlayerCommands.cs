using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using TextEngine.Models;

namespace TextEngine
{
    public static class PlayerCommands
    {
        public static void Look(string[] cmdParts)
        {
            if(cmdParts.Length == 1)
            {
                Program.character.CurrentRoom.Look();
            }
            else
            {
                var itemName = cmdParts[1].ToLower();

                // check if item exists in room
                var item = Program.character.CurrentRoom.Inventory.Items.FirstOrDefault(x => x.Name.ToLower() == itemName);
                if(item != null)
                {
                    item.Look();
                    return;
                }

                // check if item exists in character inventory
                item = Program.character.Inventory.Items.FirstOrDefault(x => x.Name.ToLower() == itemName);
                if (item != null)
                {
                    item.Look();
                    return;
                }

                // is the item a monster?
                var monster = Program.character.CurrentRoom.Monsters.FirstOrDefault(x => x.MonsterType.Name.ToLower() == itemName);
                if (monster != null)
                {
                    monster.Look();
                    return;
                }

                Utility.Print(ConsoleColor.DarkMagenta, "You don't see that item here!");
            }
        }

        public static void Help()
        {
            Utility.Print(ConsoleColor.DarkGreen, " *** Command List *** ");
            Utility.Print(ConsoleColor.DarkGreen, "Directions: n, s, e, w, ne, nw, se, sw, u, d");
            Utility.Print(ConsoleColor.DarkGreen, "Look: l [<dir>, <item>, <monster>]");
            Utility.Print(ConsoleColor.DarkGreen, "Combat: a <monster>");
            Utility.Print(ConsoleColor.DarkGreen, "Get: g <item>");
            Utility.Print(ConsoleColor.DarkGreen, "Map: map");
            Console.WriteLine();

#if TEXTENGINEADMIN
            Utility.Print(ConsoleColor.DarkGreen, " *** Admin Commands *** ");

            Utility.Print(ConsoleColor.DarkGreen, "create room <dir>");
            Utility.Print(ConsoleColor.DarkGreen, "set name <room name>");
            Utility.Print(ConsoleColor.DarkGreen, "set desc <room description>");
            Utility.Print(ConsoleColor.DarkGreen, "set <dir> item <itemid>");

            Utility.Print(ConsoleColor.DarkGreen, "create item <item name>");
            Utility.Print(ConsoleColor.DarkGreen, "set item name <itemid> <new name>");
            Utility.Print(ConsoleColor.DarkGreen, "create monster <monster name>");
            Utility.Print(ConsoleColor.DarkGreen, "list");
            Utility.Print(ConsoleColor.DarkGreen, "set monster <monster id> <stat> <value>");
            Utility.Print(ConsoleColor.DarkGreen, "spawn <monster id>");



#endif
        }

        public static void Get(string[] cmdParts)
        {
            string itemName = string.Join(" ", cmdParts.Skip(1)).ToLower();
            using (var context = new TextEngineContext())
            {
                var localItem = Program.character.CurrentRoom.Inventory.Items.FirstOrDefault(i => i.Name.ToLower() == itemName);
                if (localItem == null)
                {
                    Utility.Print(ConsoleColor.Yellow, "You don't see that item here!");
                    return;
                }

                var item = context.Items.FirstOrDefault(i => i.ItemID == localItem.ItemID);
                var character = Character.GetInstance(context);

                item.Inventory = character.Inventory;
                context.SaveChanges();
                Program.RefreshGameState(context);
                Utility.Print(ConsoleColor.DarkGreen, "Taken.");
            }
        }

        public static void Drop(string[] cmdParts)
        {
            string itemName = string.Join(" ", cmdParts.Skip(1)).ToLower();
            using (var context = new TextEngineContext())
            {
                var localItem = Program.character.Inventory.Items.FirstOrDefault(i => i.Name.ToLower() == itemName);
                if (localItem == null)
                {
                    Utility.Print(ConsoleColor.Yellow, "You don't seem to have that item!");
                    return;
                }

                var item = context.Items.FirstOrDefault(i => i.ItemID == localItem.ItemID);
                var character = Character.GetInstance(context);

                item.Inventory = character.CurrentRoom.Inventory;
                context.SaveChanges();
                Program.RefreshGameState(context);
                Utility.Print(ConsoleColor.DarkGreen, "Dropped.");
            }
        }

        public static void Attack(string[] cmdParts)
        {
            if(cmdParts.Length < 2)
            {
                Utility.Print(ConsoleColor.Yellow, "Invalid command!");
                return;
            }

            string monsterName = cmdParts[1].ToLower();
            var monster = Program.character.CurrentRoom.Monsters.FirstOrDefault(m => m.MonsterType.Name.ToLower() == monsterName);
            if(monster == null)
            {
                Utility.Print(ConsoleColor.Yellow, "You don't see that monster here!");
                return;
            }

            int attack = Program.Random.Next(1, 101);
            if (attack < 75)
            {
                int damage = Program.Random.Next(1, 10);
                Utility.Print(ConsoleColor.DarkRed, "You smash the " + monster.MonsterType.Name + " for " + damage + " points of damage!");
                monster.MonsterType.HP -= damage;
                if(monster.MonsterType.HP <= 0)
                {
                    monster.Die();
                    using (var context = new TextEngineContext())
                    {
                        context.Monsters.Remove(monster);
                        context.SaveChanges();
                        Program.RefreshGameState(context);
                    }
                }
            }
            else
            {
                Utility.Print(ConsoleColor.DarkGreen, "You miss the " + monster.MonsterType.Name + ".");
            }
        }

        public static void DrawMap()
        {
            int x_min = Program.character.CurrentRoom.X - 3;
            int x_max = Program.character.CurrentRoom.X + 3;
            int y_min = Program.character.CurrentRoom.Y - 3;
            int y_max = Program.character.CurrentRoom.Y + 3;

            using (var context = new TextEngineContext())
            {
                var rooms = context.Rooms.Where(
                    r => r.X <= x_max &&
                    r.X >= x_min &&
                    r.Y <= y_max &&
                    r.Y >= y_min &&
                    r.Z == Program.character.CurrentRoom.Z);

                for(int y = y_max; y >= y_min; y--)
                {
                    string line1 = string.Empty;
                    string line2 = string.Empty;
                    string line3 = string.Empty;

                    for (int x = x_min; x <= x_max; x++)
                    {

                        var room = rooms
                            .Where(r => r.X == x && r.Y == y)
                            .Include(r => r.Doors)
                            .FirstOrDefault();
                        if(room == null)
                        {
                            line1 += "   ";
                            line2 += "   ";
                            line3 += "   ";
                        }
                        else
                        {
                            string[] temp = room.getMap();
                            line1 += temp[0];
                            line2 += temp[1];
                            line3 += temp[2];
                        }
                    }

                    Utility.Print(ConsoleColor.DarkGreen, line1);
                    Utility.Print(ConsoleColor.DarkGreen, line2);
                    Utility.Print(ConsoleColor.DarkGreen, line3);
                }

            }
        }
    }
}
