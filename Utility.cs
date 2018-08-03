using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextEngine.Models;

namespace TextEngine
{
    public static class Utility
    {
        public static Direction StringToDirection(string dir)
        {
            switch(dir)
            {
                case "n":
                case "north":
                    return Direction.North;
                case "s":
                case "south":
                    return Direction.South;
                case "e":
                case "east":
                    return Direction.East;
                case "w":
                case "west":
                    return Direction.West;
                case "nw":
                case "northwest":
                    return Direction.Northwest;
                case "ne":
                case "northeast":
                    return Direction.Northeast;
                case "sw":
                case "southwest":
                    return Direction.Southwest;
                case "se":
                case "southeast":
                    return Direction.Southeast;
                case "u":
                case "up":
                    return Direction.Up;
                case "d":
                case "down":
                    return Direction.Down;
                default:
                    return Direction.None;
            }
        }

        public static Direction DirectionOpposite(Direction dir)
        {
            switch (dir)
            {
                case Direction.South:
                    return Direction.North;
                case Direction.North:
                    return Direction.South;
                case Direction.West:
                    return Direction.East;
                case Direction.East:
                    return Direction.West;
                case Direction.Southeast:
                    return Direction.Northwest;
                case Direction.Southwest:
                    return Direction.Northeast;
                case Direction.Northeast:
                    return Direction.Southwest;
                case Direction.Northwest:
                    return Direction.Southeast;
                case Direction.Down:
                    return Direction.Up;
                case Direction.Up:
                    return Direction.Down;
                default:
                    return Direction.None;
            }
        }

        public static void Print(ConsoleColor color, string message)
        {
            ConsoleColor c = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = c;
        }

        public static void ProcessCombat()
        {
            foreach(Monster m in Program.character.CurrentRoom.Monsters)
            {
                m.Attack();
            }
        }
    }
}
