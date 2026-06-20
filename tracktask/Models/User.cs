using SQLite;

namespace tracktask.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int UserId { get; set; }

        [NotNull]
        public string Name { get; set; }

        [Unique, NotNull]
        public string Email { get; set; }

        [NotNull]
        public string PasswordHash { get; set; }

        public UserRole Role { get; set; } = UserRole.User; // default role
    }

    public enum UserRole
    {
        User = 0,
        Admin = 1
    }
}