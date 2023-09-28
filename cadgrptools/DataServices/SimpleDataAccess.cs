using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cadgrptools.DataServices
{
    public class SimpleDataAccess
    {
        public List<User> Users { get; set; }

        public void Load()
        {
            Users = new List<User>() 
            {
                new User {Id=1, Name="Nguyen Van Nhat", Age=30},
                new User {Id=2, Name="Nguyen Van Nhi", Age=22},
                new User {Id=3, Name="Nguyen Van Tam", Age=23},
                new User {Id=4, Name="Nguyen Van Tu", Age=24},
                new User {Id=5, Name="Nguyen Van Ngu", Age=25},
                new User {Id=6, Name="Nguyen Van Luc", Age=26},
                new User {Id=7, Name="Nguyen Van That", Age=27},
                new User {Id=8, Name="Nguyen Van Bat", Age=28},
                new User {Id=9, Name="Nguyen Van Cuu", Age=29},
            };
        }

        public void SaveChanges() { }
    }
}
