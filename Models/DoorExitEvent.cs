﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEngine.Models
{
    public class DoorExitEvent
    {
        public long DoorExitEventID { get; set; }
        public Room Room { get; set; }
        public string Message { get; set; }
    }
}
