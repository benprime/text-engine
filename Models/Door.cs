using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TextEngine.Models
{
    public class Door
    {
        public long DoorID { get; set; }

        [InverseProperty("Doors")]
        public Room SourceRoom { get; set; }

        public Room TargetRoom { get; set; }

        public Direction Direction { get; set; }

        public DoorEntryEvent DoorEntryEvent { get; set; }
        public DoorExitEvent DoorExitEvent { get; set; }

        public List<DoorItemCondition> DoorItemConditions { get; set; }

        public override string ToString()
        {
            return this.Direction.ToString();
        }

        public static Door FromDir(TextEngineContext context, string dir)
        {
            Direction direction = Utility.StringToDirection(dir);
            return context.Doors.FirstOrDefault(x => x.Direction == direction && x.SourceRoom.RoomID == Program.character.CurrentRoom.RoomID);
        }
    }
}
