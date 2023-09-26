using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace cadgrptools
{
    public class SQLiteAccess
    {
        //string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
        string pathDb = "cadgrp1.db";
        string cs = @"URI=file:" + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\cadgrp1.db"; //db create debug

        //SQLiteConnection conn;
        //SQLiteCommand cmd;
        SQLiteDataReader reader;


        public void ReadData(string sql)
        {
            var conn = new SQLiteConnection(cs);
            conn.Open();

            sql = "select * from test";
            var cmd = new SQLiteCommand(sql, conn);
            reader = cmd.ExecuteReader();

            while (reader.Read())
            {//20230926
                //dataGridView1.Rows.Insert(0, reader.GetString(0), reader.GetString(1));
            }
        }


        // create database and table
        private void CreateDB()
        {
            if (!System.IO.File.Exists(pathDb))
            {
                SQLiteConnection.CreateFile(pathDb);
                using (var sqlite = new SQLiteConnection(@"Data Source=" + pathDb))
                {
                    sqlite.Open();
                    string sql = "create table test (name varchar(20), id varchar(12))";
                    SQLiteCommand cmd = new SQLiteCommand(sql, sqlite);
                    cmd.ExecuteNonQuery();

                    sql = "CREATE TABLE \"BeamProfile\" (\r\n\t\"id\"\tINTEGER NOT NULL UNIQUE,\r\n\t\"ProjectId\"\tINTEGER NOT NULL,\r\n\t\"Name\"\tTEXT NOT NULL,\r\n\t\"Branch\"\tINTEGER NOT NULL DEFAULT 1,\r\n\t\"Profile\"\tTEXT NOT NULL,\r\n\t\"RollWelded\"\tTEXT NOT NULL DEFAULT 'H',\r\n\t\"Material\"\tTEXT NOT NULL,\r\n\t\"Joinb\"\tTEXT NOT NULL,\r\n\t\"Created\"\tTEXT NOT NULL,\r\n\t\"Modified\"\tTEXT,\r\n\t\"Deleted\"\tTEXT,\r\n\t\"Note\"\tTEXT,\r\n\tPRIMARY KEY(\"id\" AUTOINCREMENT)\r\n)";
                    cmd = new SQLiteCommand(sql, sqlite);
                    cmd.ExecuteNonQuery();
                    return;
                }
            }
            Console.WriteLine("Database can not create!");
        }





    }
}
