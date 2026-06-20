using SQLite;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using tracktask.Models;
using tracktask.Data;

namespace tracktask.Services
{
    public class AuthService
    {
        private readonly SQLiteConnection _db;

        public AuthService()
        {
            _db = AppDatabase.GetDb();
        }

        public bool Register(string email, string password, string fullName = "")
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(password))
                return false;

            email = email.Trim().ToLower();

            if (_db.Table<User>().Any(u => u.Email == email))
                return false;

            var user = new User
            {
                Name = fullName,
                Email = email,
                PasswordHash = HashPassword(password),
                Role = UserRole.User
            };

            _db.Insert(user);
            return true;
        }

        public User Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || password == null)
                return null;

            email = email.Trim().ToLower();
            var hashed = HashPassword(password);

            return _db.Table<User>().FirstOrDefault(u =>
                u.Email == email &&
                u.PasswordHash == hashed);
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}