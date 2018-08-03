using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEngine.Models
{
    public class Monster
    {
        public long MonsterID { get; set; }
        public MonsterType MonsterType { get; set; }
        public Room Room { get; set; }
        public Inventory Inventory { get; set; }

        public Monster()
        {
            if(this.Inventory == null)
            {
                this.Inventory = new Inventory();
            }
        }

        public void Look()
        {
            Utility.Print(ConsoleColor.DarkGreen, this.MonsterType.Description);
            if (this.Inventory.Items.Count > 0)
            {
                Utility.Print(ConsoleColor.DarkGreen, "It appears to be carrying: " + string.Join(", ", this.Inventory.Items));
            }
            else
            {
                Utility.Print(ConsoleColor.DarkGreen, "It appears to be carrying nothing.");
            }
        }

        public void Attack()
        {
            int attack = Program.Random.Next(1, 101);
            if (attack <= this.MonsterType.HitChance)
            {
                int damage = Program.Random.Next(this.MonsterType.DamageMinimum, this.MonsterType.DamageMaximum + 1);
                Utility.Print(ConsoleColor.DarkRed, this.MonsterType.Name + " hits you for " + damage + " damage!");
                Program.character.HP -= damage;
            }
            else
            {
                Utility.Print(ConsoleColor.DarkGreen, this.MonsterType.Name + " swings at you and misses!");
            }
        }

        public void Die()
        {
            Utility.Print(ConsoleColor.DarkRed, "The " + this.MonsterType.Name + " collapses.");
            Utility.Print(ConsoleColor.Yellow, "The " + this.MonsterType.Name + " drops " + string.Join(", ", this.Inventory.Items));
            using (var context = new TextEngineContext())
            {
                foreach(Item localItem in this.Inventory.Items)
                {
                    var item = context.Items.First(i => i.ItemID == localItem.ItemID);
                    item.Inventory = Room.ByID(context, Program.character.CurrentRoom.RoomID).Inventory;
                }
                context.SaveChanges();
                Program.RefreshGameState(context);
            }
        }

        public override string ToString()
        {
            return this.MonsterType.Name;
        }
    }
}
