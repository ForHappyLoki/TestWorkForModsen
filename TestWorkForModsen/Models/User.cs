    using System.ComponentModel.DataAnnotations;

    namespace TestWork_Events.Models
    {
        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public DateOnly Birthday { get; set; }
            public string Email { get; set; }
            public ICollection<ConnectorEventUser> ConnectorEventUser { get; set; } = new List<ConnectorEventUser>();
        }
    }
