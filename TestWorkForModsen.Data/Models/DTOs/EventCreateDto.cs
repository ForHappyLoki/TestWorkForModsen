﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWorkForModsen.Data.Models.DTOs
{
    public class EventCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
        public string Category { get; set; }
        public string MaxParticipants { get; set; }
        public byte[] Image { get; set; }
    }
}
