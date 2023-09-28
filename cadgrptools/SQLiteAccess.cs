using System;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Windows;

namespace cadgrptools
{
    public class SQLiteAccess
    {
        //string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
        string pathDb = "cadgrp1.db";
        string cs;

        SQLiteDataReader reader;


        public SQLiteAccess(string path)
        {
            this.pathDb = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path);
            this.cs = @"URI=file:" + pathDb;
        }

        //string cs = @"URI=file:" + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\cadgrp1.db"; //db create debug
        //string cs = @"URI=file:" + pathDb; //db create debug   ERROR
        //string cs = @"URI=file: C:\\cadgrp1.db";


        public void ReadData()
        {
            var conn = new SQLiteConnection(cs);
            conn.Open();

            string sql = "select * from project";
            var cmd = new SQLiteCommand(sql, conn);
            reader = cmd.ExecuteReader();

            while (reader.Read())
            {//20230926
                //dataGridView1.Rows.Insert(0, reader.GetString(0), reader.GetString(1));
                string s = string.Concat(reader.GetInt32(0).ToString(), reader.GetString(1), reader.GetString(2));
                MessageBox.Show(s);
            }
        }


        // create database and table
        public void CreateDB()
        {
            if (!System.IO.File.Exists(pathDb))
            {
                SQLiteConnection.CreateFile(pathDb);
                using (var sqlite = new SQLiteConnection(@"Data Source=" + pathDb))
                {
                    sqlite.Open();
                    string sql = "create table project (id integer not null unique, shortname varchar(5), fullname text)";
                    SQLiteCommand cmd = new SQLiteCommand(sql, sqlite);
                    cmd.ExecuteNonQuery();

                    sql = "CREATE TABLE BeamProfile (" +
                        "id INTEGER NOT NULL UNIQUE," +
                        "ProjectId INTEGER NOT NULL," +
                        "Name TEXT NOT NULL," +
                        "Branch INTEGER NOT NULL DEFAULT 1," +
                        "Profile TEXT NOT NULL, " +
                        "RollWelded TEXT NOT NULL DEFAULT 'H', " +
                        "Material TEXT NOT NULL, Joinb TEXT NOT NULL, " +
                        "Created TEXT NOT NULL, Modified TEXT, Deleted TEXT," +
                        "Note TEXT, PRIMARY KEY(id AUTOINCREMENT))";
                    cmd = new SQLiteCommand(sql, sqlite);
                    cmd.ExecuteNonQuery();
                    return;
                }
            }
            Console.WriteLine("Database can not create!");
        }



        public void InsertRecord(string sql)
        {
            var conn = new SQLiteConnection(cs);
            conn.Open();
            var cmd = new SQLiteCommand(conn);

            try
            {

                cmd.CommandText = sql;// "insert into test (name,id) values (@name, @id)";
                //cmd.Parameters.AddWithValue("@name", name_txt.Text);
                //cmd.Parameters.AddWithValue("@id", id_txt.Text);
                /*
                dataGridView1.ColumnCount = 2;
                dataGridView1.Columns[0].Name = "Name";
                dataGridView1.Columns[1].Name = "Id";
                string[] row = new string[] { name_txt.Text, id_txt.Text };
                dataGridView1.Rows.Add(row);
                */
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        /*
        private void InsertBtn_Click(object sender, EventArgs e)
        {
            var conn = new SQLiteConnection(cs);
            conn.Open();
            var cmd = new SQLiteCommand(conn);

            try
            {

                cmd.CommandText = "insert into test (name,id) values (@name, @id)";
                cmd.Parameters.AddWithValue("@name", name_txt.Text);
                cmd.Parameters.AddWithValue("@id", id_txt.Text);

                dataGridView1.ColumnCount = 2;
                dataGridView1.Columns[0].Name = "Name";
                dataGridView1.Columns[1].Name = "Id";
                string[] row = new string[] { name_txt.Text, id_txt.Text };
                dataGridView1.Rows.Add(row);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void UpdateBtn_Click(object sender, EventArgs e)
        {
            var conn = new SQLiteConnection(cs);
            conn.Open();
            var cmd = new SQLiteCommand(conn);

            try
            {
                cmd.CommandText = "update test set id=@id where name=@name";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@name", name_txt.Text);
                cmd.Parameters.AddWithValue("@id", id_txt.Text);

                cmd.ExecuteNonQuery();
                dataGridView1.Rows.Clear();
                DataShow();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void DeleteBtn_Click(object sender, EventArgs e)
        {
            var conn = new SQLiteConnection(cs);
            conn.Open();
            var cmd = new SQLiteCommand(conn);

            try
            {

                cmd.CommandText = "delete from test where name=@name";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@name", name_txt.Text);

                cmd.ExecuteNonQuery();
                dataGridView1.Rows.Clear();
                DataShow();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            {
                dataGridView1.CurrentRow.Selected = true;
                name_txt.Text = dataGridView1.Rows[e.RowIndex].Cells["Name"].FormattedValue.ToString();
                id_txt.Text = dataGridView1.Rows[e.RowIndex].Cells["Id"].FormattedValue.ToString();
            }
        }
        */
    }
}
