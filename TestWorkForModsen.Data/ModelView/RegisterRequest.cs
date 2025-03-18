namespace TestWork_Events.ModelView
{
    public class RegisterRequest
    {
        public required string Email { get; init; }
        public required string Password { get; init; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateOnly Birthday { get; set; }
    }
}
