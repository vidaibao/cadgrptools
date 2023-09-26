using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Dapper;

namespace cadgrptools
{
    public class SQLiteDapper
    {
        User u = new User();

        public static IEnumerable<User> GetAll()
        {
            var config = new SQLiteConnectionStringBuilder()
            {
                DataSource = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\cadgrp1.db"
            };
            using (var connection = new SQLiteConnection(config.ToString()))
            {
                connection.Open();
                return connection.Query<User>(@"select * from user", null);
            }
        }


        public override string ToString()
        {
            return string.Format("ID={0}, Name={1}, Age={2}", u.ID, u.Name, u.Age);
        }



    }


}
