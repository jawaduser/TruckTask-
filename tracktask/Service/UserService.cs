using System.Collections.Generic;
using SQLite;
using tracktask.Data;
using tracktask.Models;
using tracktask.Service.Interface;

namespace tracktask.Service
{
    public class UserService : IUserService
    {
        private readonly SQLiteConnection _db;

        public UserService()
        {
            _db = AppDatabase.GetDb();
        }

        public int Insert(User item)
        {
            return _db.Insert(item); // returns number of rows inserted
        }

        public int Update(User item)
        {
            return _db.Update(item); // returns number of rows updated
        }

        public int Delete(User item)
        {
            return _db.Delete(item); // returns number of rows deleted
        }

        public User GetById(int id)
        {
            return _db.Find<User>(id);
        }

        public List<User> GetAll()
        {
            return _db.Table<User>().ToList();
        }
    }
}
