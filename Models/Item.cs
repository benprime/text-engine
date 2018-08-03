using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace TextEngine.Models
{
    public class Item
    {
        public long ItemID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Inventory Inventory { get; set; }

        public void Look()
        {
            Utility.Print(ConsoleColor.Cyan, this.Description);
        }

        public override string ToString()
        {
#if TEXTENGINEADMIN
            return this.Name + " (" + this.ItemID + ")";
#else
            return this.Name;
#endif
        }

        public static Item FromID(TextEngineContext context, string itemID)
        {
            Item item = null;
            long itemIDLong;
            bool validID = long.TryParse(itemID, out itemIDLong);

            if (validID)
            {
                item = context.Items
                    .Include(i => i.Inventory)
                    .Where(i => i.ItemID == itemIDLong)
                    .FirstOrDefault();
            }
            return item;
        }
    }
}
