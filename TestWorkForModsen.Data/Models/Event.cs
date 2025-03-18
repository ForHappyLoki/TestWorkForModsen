namespace TestWork_Events.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
        public string Category { get; set; }
        public string MaxParticipants { get; set; }
        // По хорошему изображения должны храниться на отдельном сервере, а тут должны лежать ссылки
        // откуда их можно скачать, а потом кешировать популярные картинки в редисе.
        // Но раз не обязательно - экономим время и храним набор байтов 
        public byte[] Image { get; set; }
        public ICollection<ConnectorEventUser> ConnectorEventUser { get; set; } = new List<ConnectorEventUser>();
    }
}
