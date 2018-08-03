using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TextEngine.Models;
using Microsoft.Data.Entity;

namespace TextEngine
{
    class Program
    {

        public static Character character;

        public static Random Random;

        public static void RefreshGameState(TextEngineContext context)
        {
            character = Character.GetInstance(context);

            if (character == null)
            {
                character = new Character();
                character.SetInitialRoom(context);
                context.Add(character);
                context.SaveChanges();
            }
        }

        static void Main(string[] args)
        {
            Program.Random = new Random();
            // initialize database
            using (var context = new TextEngineContext())
            {
                // TODO: remove this
                //context.Database.EnsureDeleted();

                if (context.Database.EnsureCreated())
                {
                    context.Seed(context);
                }

                RefreshGameState(context);
            }

            Program.character.CurrentRoom.Look();
            for (var play = true; play;)
            {
                Console.WriteLine();
                Console.Write("[HP=" + Program.character.HP + "]> ");
                string cmd = Console.ReadLine().Trim();
                Console.WriteLine();

                // split on one or more whitespace characters
                var cmdParts = Regex.Split(cmd, @"\s+");
                var action = cmdParts[0].ToLower();
                var cmdArgs = cmdParts.Skip(1);

                // on empty input, default to "look"
                if (string.IsNullOrEmpty(action))
                {
                    action = "l";
                }

                switch (action)
                {
#if TEXTENGINEADMIN
                    case "create":
                        AdminCommands.Create(cmdParts);
                        break;
                    case "set":
                        AdminCommands.Set(cmdParts);
                        break;
                    case "list":
                        AdminCommands.ListMonsterTypes();
                        break;
                    case "spawn":
                        AdminCommands.SpawnMonster(cmdParts);
                        break;
                    case "give":
                        AdminCommands.Give(cmdParts);
                        break;
#endif
                    case "n":
                    case "north":
                    case "s":
                    case "south":
                    case "e":
                    case "east":
                    case "w":
                    case "west":
                    case "nw":
                    case "northwest":
                    case "ne":
                    case "northeast":
                    case "sw":
                    case "southwest":
                    case "se":
                    case "southeast":
                    case "u":
                    case "up":
                    case "d":
                    case "down":
                        character.CurrentRoom.Move(Utility.StringToDirection(action));
                        break;
                    case "l":
                    case "look":
                        PlayerCommands.Look(cmdParts);
                        break;
                    case "i":
                    case "inventory":
                        Program.character.Inventory.ShowInventory();
                        break;
                    case "g":
                    case "get":
                    case "t":
                    case "take":
                        PlayerCommands.Get(cmdParts);
                        break;
                    case "drop":
                        PlayerCommands.Drop(cmdParts);
                        break;
                    case "a":
                    case "attack":
                        PlayerCommands.Attack(cmdParts);
                        break;
                    case "q":
                    case "quit":
                        play = false;
                        break;
                    case "h":
                    case "help":
                        PlayerCommands.Help();
                        break;
                    case "m":
                    case "map":
                        PlayerCommands.DrawMap();
                        break;
                    default:
                        Console.WriteLine("I beg your pardon?");
                        break;
                }

#if !TEXTENGINEADMIN
                Utility.ProcessCombat();
#endif

                if (Program.character.HP < 0)
                {
                    Utility.Print(ConsoleColor.Red, "YOU HAVE DIED.");
                    play = false;
                }
            }

            Console.WriteLine("Thanks for playing!");
        }
    }
}
