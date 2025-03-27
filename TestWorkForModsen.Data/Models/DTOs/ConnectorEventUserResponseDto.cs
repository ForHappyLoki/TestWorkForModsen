using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWorkForModsen.Data.Models.DTOs
{
    public class ConnectorEventUserResponseDto
    {
        public int EventId { get; set; }
        public int UserId { get; set; }
        public DateTime AdditionTime { get; set; }
        public EventBriefDto Event { get; set; }
        public UserBriefDto User { get; set; }
    }
}
