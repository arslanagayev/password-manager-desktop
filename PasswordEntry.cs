namespace PasswordManager
{
    public class PasswordEntry
    {
        public int Id { get; set; }
        public string Site { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
