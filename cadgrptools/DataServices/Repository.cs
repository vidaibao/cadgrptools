using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cadgrptools.DataServices
{
    public class Repository
    {
        protected readonly SimpleDataAccess _context;
        public Repository(SimpleDataAccess context)
        {
            _context = context;
            _context.Load();
        }

        public void SaveChanges() => _context.SaveChanges();
        public List<User> Users => _context.Users;
        public User[] Select() => _context.Users.ToArray();
        public User Select(int id)
        {
            foreach (var user in _context.Users)
            {
                if (user.Id == id) return user;
            }
            return null;
        }

        public User[] Select(string key)
        {
            var temp = new List<User>();
            var k = key.ToLower();
            foreach (var item in _context.Users)
            {
                var logic =
                    item.Name.ToLower().Contains(k) //||
                    //item.Name.ToLower().Contains(k)
                    ;
                if (logic) temp.Add(item);
            }
            return temp.ToArray();
        }


        public void Insert(User user)
        {
            var lastIndex = _context.Users.Count - 1;
            var id = lastIndex < 0 ? 1 : _context.Users[lastIndex].Id + 1;
            user.Id = id;
            _context.Users.Add(user);
        }


        public bool Update(int id, User user)
        {
            var u = Select(id);
            if (u == null) return false;
            u.Name = user.Name;
            return true;
        }


        public bool Delete(int id)
        {
            var u = Select(id);
            if (u == null) return false;
            _context.Users.Remove(u);
            return true;
        }


    }
}
