using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextEngine.Models;

namespace TextEngine
{
    class MapHelper
    {
        public static void DrawMap()
        {
            using (var context = new TextEngineContext())
            {

                int map_radius = 2;
                int z = Program.character.CurrentRoom.Z;
                int xMin = Program.character.CurrentRoom.X - map_radius;
                int xMax = Program.character.CurrentRoom.X + map_radius;
                int yMin = Program.character.CurrentRoom.Y - map_radius;
                int yMax = Program.character.CurrentRoom.Y + map_radius;

                for(int y = yMin; y <= yMax; y++)
                {
                    for (int x = xMin; x <= xMax; x++)
                    {

                    }

                    // ending a row

                }

            }
        }

        public static string GetMapRow(int xMin, int xMax, int y, int z, int padLength)
        {
            using (var context = new TextEngineContext())
            {
                var rooms = context.Rooms
                    .Where(r => r.Z == z
                        && r.Y == y
                        && r.X >= xMin
                        && r.X <= xMax)
                    .OrderBy(o => o.X);

                string padString = new string(' ', padLength);

                string[] rows = Enumerable.Repeat(string.Empty, 5).ToArray();
                for (int x = xMin; x <= xMax; ++x)
                {
                    Room room = rooms.Where(r => r.X == x).FirstOrDefault();

                    int rowLength = 15;

                    if (room == null)
                    {
                        for (int i = 0; i < rows.Length; i++)
                        {
                            rows[i] = new string(' ', padLength + rowLength++);
                        }
                    }
                    else
                    {
                        // first string
                    }
                }

            }
            return "";
        }

    }
}
