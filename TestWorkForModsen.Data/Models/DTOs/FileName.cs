using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWorkForModsen.Data.Models.DTOs
{
    // Обновляем ConnectorEventUserDto
    public class ConnectorEventUserDto
    {
        public int EventId { get; set; }
        public int UserId { get; set; }
        public EventResponseDto Event { get; set; } 
    }
}
