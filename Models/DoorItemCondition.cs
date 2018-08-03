using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TextEngine.Models
{
    public class DoorItemCondition
    {
        public long DoorItemConditionID { get; set; }

        [Required]
        public Door Door { get; set; }
        [Required]
        public Item Item { get; set; }
    }
}
