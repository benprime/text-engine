using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace TextEngine.Models
{
    public class Character
    {
        public long CharacterID { get; set; }
        public Room CurrentRoom { get; set; }

        public Inventory Inventory { get; set; }
        public int HP { get; set; }

        public Character()
        {
            this.HP = 50;
            if (this.Inventory == null)
            {
                this.Inventory = new Inventory();
            }
        }

        public static Character GetInstance(TextEngineContext context)
        {
            return context.Characters
            .Include(c => c.Inventory).ThenInclude(i => i.Items)
            .Include(c => c.CurrentRoom).ThenInclude(r => r.Doors)
            .Include(c => c.CurrentRoom).ThenInclude(r => r.Inventory).ThenInclude(i => i.Items)
            .Include(c => c.CurrentRoom).ThenInclude(r => r.Monsters).ThenInclude(m => m.MonsterType)
            .Include(c => c.CurrentRoom).ThenInclude(r => r.Monsters).ThenInclude(m => m.Inventory)
            .Include(c => c.CurrentRoom).ThenInclude(r => r.Monsters).ThenInclude(m => m.Inventory).ThenInclude(i => i.Items)
            .FirstOrDefault();
        }

        // this method only does anything when a new game is started
        public void SetInitialRoom(TextEngineContext context)
        {
            // set the default room to the first room in the list
            // TODO: maybe change this to 0, 0
            if (this.CurrentRoom == null)
            {
                // TODO: how to make the model do this automatically?
                // ThenInclude: https://github.com/aspnet/EntityFramework/wiki/Design-Meeting-Notes:-January-8,-2015
                var rooms = from r in context.Rooms
                            .Include(r => r.Doors).ThenInclude(d => d.TargetRoom) // this syntax was found in a bug fix
                            .Include(r => r.Inventory)
                            select r;

                this.CurrentRoom = rooms.First();
            }
        }
    }
}
