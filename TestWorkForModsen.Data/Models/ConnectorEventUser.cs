namespace TestWork_Events.Models
{
    public class ConnectorEventUser
    {
        public int EventId { get; set; }
        public Event Event { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime AdditionTime { get; set; }

    }
}
