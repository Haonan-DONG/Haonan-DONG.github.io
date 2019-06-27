using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Data.OleDb;

namespace Evacuation_Routine_Planning_System
{
    public partial class Load_Interface : Form
    {
        public string path;

        public Load_Interface()
        {
            MessageBox.Show("Please choose the users' database.");
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.ShowDialog();

            path = fileDialog.FileName;
            InitializeComponent();
        }

        // -------------------- Log Interface --------------------------
        // using the access database to create the users' data.
        private void logButton_Click(object sender, EventArgs e)
        {
            Global.Login = true;
            AccessDb userData = new AccessDb(path);
            string userName = userNameTB.Text;
            string passWord = passWordTB.Text;
            int flag;

            string rightPassword = userData.dataBaseSelectPassword(userName, out flag);
            bool rightAuthority = userData.dataBaseSelectAutority(userName);

            try
            {
                if (flag == 0)
                {
                    MessageBox.Show("No Register!");
                }
                else
                {
                    if (passWord == rightPassword)
                    {
                        if (rightAuthority)
                        {
                            this.Close();
                            
                        }
                        else
                        {
                            MessageBox.Show("No Authority");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Password Error, Please check again!");
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("error");
            }
        }

        // ------------------- Register Interface ----------------------
        // register form. not edited.
        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
