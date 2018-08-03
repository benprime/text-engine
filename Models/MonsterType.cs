using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEngine.Models
{
    public class MonsterType
    {
        public long MonsterTypeID { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public int HitChance { get; set; }

        public int DamageMinimum { get; set; }
        public int DamageMaximum { get; set; }

        public int HP { get; set; }

        public MonsterType()
        {
            this.Description = "A scary monster!";
            this.HitChance = 50;
            this.DamageMinimum = 1;
            this.DamageMaximum = 3;
            this.HP = 5;
        }
    }
}
