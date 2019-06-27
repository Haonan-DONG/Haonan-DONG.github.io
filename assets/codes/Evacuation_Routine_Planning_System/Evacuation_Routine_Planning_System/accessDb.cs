using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Windows.Forms;

namespace Evacuation_Routine_Planning_System
{
    class AccessDb
    {
        private OleDbConnection myCon;

        public AccessDb(string dataSource)
        {
            string strCon = @"Provider=Microsoft.Jet.OLEDB.4.0;";
            string strSour = @"Data Source="+dataSource;
            myCon = new OleDbConnection(strCon+strSour);
        }

        void dataBaseOpen()
        {
            myCon.Open();
        }

        void dataBaseClose()
        {
            myCon.Close();
        }

        // insert into the user's data( user's name, user's password, user's authority(BOOL).)
        public void dataBaseInsert(string userName, string passWord,bool authority)
        {
            var sql = "insert into userData(ID,userName,passWord,authority) values";
            sql += string.Format("('{0}','{1}','{2}')", userName, passWord, authority);
            OleDbCommand cmd = myCon.CreateCommand();   // create a command
            cmd.CommandText = sql;
            dataBaseOpen();
            cmd.ExecuteNonQuery();
            dataBaseClose();
        }
        
        // get the password
        public string dataBaseSelectPassword(string userName,out int flag)
        {
            string passWord=null;
            flag = 0;

            try
            {
                string sql = "select * from userData where userName = '" + userName + "'";
                OleDbCommand cmd = myCon.CreateCommand();
                cmd.CommandText = sql;
                dataBaseOpen();

                OleDbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.ToString() != null)
                    {
                        string line = string.Format("{0}", reader["passWord"]);
                        passWord = line;
                    }
                    flag++;
                }
                reader.Close();
                dataBaseClose();
            }
            catch(Exception error)
            {
                MessageBox.Show("error!");
                throw error;
            }

            return passWord;
        }

        public bool dataBaseSelectAutority(string userName)
        {
            bool authority = false ;
            int flag = 0;
            try
            {
                string sql = "select * from userData where userName = '" + userName + "'";
                OleDbCommand cmd = myCon.CreateCommand();
                cmd.CommandText = sql;
                dataBaseOpen();

                OleDbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.ToString() != null)
                    {
                        string line = string.Format("{0}", reader["authority"]);
                        authority = bool.Parse(line);
                    }
                    flag++;
                }
                reader.Close();
                dataBaseClose();
            }
            catch (Exception error)
            {
                throw error;
            }

            return authority;
        }
    }
}
