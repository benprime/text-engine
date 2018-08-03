using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEngine.Models
{
    public class Inventory
    {
        public long InventoryID { get; set; }
        public List<Item> Items { get; set; }
        
        public Inventory()
        {
            if(this.Items == null)
            {
                this.Items = new List<Item>();
            }
        }

        public bool Contains(string name)
        {
            var count = this.Items.Where(i => i.Name.ToLower() == name.ToLower()).Count();
            return count != 0;
        }

        public void ShowInventory()
        {
            if (this.Items.Count > 0)
            {
                Utility.Print(ConsoleColor.DarkGreen, "You are carrying: " + string.Join(", ", this.Items));
            }
            else
            {
                Utility.Print(ConsoleColor.DarkGreen, "You are carrying nothing.");
            }
        }
    }
}
