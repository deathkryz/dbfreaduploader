using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DbfUploader
{
    public partial class Form1 : Form
    {
        public frmWaitForm frm;
        private int numberToCompute = 0;
        private int highestPercentageReached = 0;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;

        public Form1()
        {
            InitializeComponent();
            
        }

        private void btnAbrir_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
                checkedListBox1.Items.Clear();

                string[] files = System.IO.Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*.dbf");

                foreach (string file in files) {
                    checkedListBox1.Items.Add(file);
                }
                if (checkedListBox1.Items.Count != 0) {
                    ((Control)this.tabPage3).Enabled = true;
                }
                

            }
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            if (btnSelectAll.Text == "Select All")
            {
                btnSelectAll.Text = "DeSelect All";
                SelectDeselectAll(true); // passing <strong>true </strong>so that all items will be checked
            }
            else
            {
                btnSelectAll.Text = "Select All";
                SelectDeselectAll(false); // passing false so that all items will be unchecked
            }
        }
        void SelectDeselectAll(bool Selected)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++) // loop to set all items checked or unchecked
            {
                checkedListBox1.SetItemChecked(i, Selected);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Testing");
            

        }

        private void CallCreateDataBase() {
            DbConnect dbConnect;
            dbConnect = new DbConnect();
            Console.WriteLine(txtDbName.Text.ToString());

            if (!txtDbName.Text.ToString().Equals(""))
            {
                dbConnect.CreateDb(txtDbName.Text);
            }

            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {

                if (checkedListBox1.GetItemChecked(i))
                {
                    string str = (string)checkedListBox1.Items[i];
                    //Console.WriteLine(str);
                    dbConnect.CreateTables(txtDbName.Text, str);

                }
            }
           

            
         
        }

        public static bool ValidateIPv4(string ipString)
        {
            if (ipString.ToLower().Equals("localhost")) return true;
            if (ipString.Count(c => c == '.') != 3) return false;
            IPAddress address;
            return IPAddress.TryParse(ipString, out address);
        }

        public bool IsCheckedEmpty() {
            bool bandera = false;
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {

                if (checkedListBox1.GetItemChecked(i))
                {

                    bandera = true;
                }
            }
            return bandera;
        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (IsCheckedEmpty())
            {
                using (frm = new frmWaitForm(CallCreateDataBase))
                {
                    frm.ShowDialog(this);



                }

                /*if (string.IsNullOrEmpty(txtDbName.Text))
                {
                    MessageBox.Show("You can't leave the input text 'Database name' empty ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }*/
                /* if (string.IsNullOrEmpty(txtIp.Text))
                 {
                     MessageBox.Show("You can't leave the input text 'Ip' empty ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                 }
                 if (string.IsNullOrEmpty(txtUser.Text))
                 {
                     MessageBox.Show("You can't leave the input text 'User' empty ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                 }
                 if (string.IsNullOrEmpty(txtPass.Text))
                 {
                     MessageBox.Show("You can't leave the input text 'Password' empty ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                 }
                 if (string.IsNullOrEmpty(txtPort.Text))
                 {
                     MessageBox.Show("You can't leave the input text 'Port' empty ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                 }*/
                /* else
                 {
                     if (!ValidateIPv4(txtIp.Text))
                     {
                         MessageBox.Show("Ip is not valid ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                     }
                     else
                     {
                         using (frm = new frmWaitForm(CallCreateDataBase))
                         {
                             frm.ShowDialog(this);



                         }
                     }

                 }*/
            }
            else {
                MessageBox.Show("You must select a dbf ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            


            

        }


        public void CallInsertData() {
            DbConnect dbConnect;
            dbConnect = new DbConnect();
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {

                    if (checkedListBox1.GetItemChecked(i))
                    {
                        string str = (string)checkedListBox1.Items[i];
                        //Console.WriteLine(str);
                        dbConnect.InsertData(txtDbName.Text, str);
                        
                    }
                }
             
        }

        void SaveData() {
            for (int i = 0; i <= 500; i++) {
                System.Threading.Thread.Sleep(10);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

            if (IsCheckedEmpty())
            {

                /*if (string.IsNullOrEmpty(txtDbName.Text))
                {
                    MessageBox.Show("You can't leave the input text 'Database name' empty ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (string.IsNullOrEmpty(txtIp.Text))
                {
                    MessageBox.Show("You can't leave the input text 'Ip' empty ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (string.IsNullOrEmpty(txtUser.Text))
                {
                    MessageBox.Show("You can't leave the input text 'User' empty ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (string.IsNullOrEmpty(txtPass.Text))
                {
                    MessageBox.Show("You can't leave the input text 'Password' empty ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (string.IsNullOrEmpty(txtPort.Text))
                {
                    MessageBox.Show("You can't leave the input text 'Port' empty ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!ValidateIPv4(txtIp.Text))
                    {
                        MessageBox.Show("Ip is not valid ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {*/
                        using (frm = new frmWaitForm(CallInsertData))
                        {
                            frm.ShowDialog(this);



                        }
                /*            }

                        }*/
            }
            else
                {
                MessageBox.Show("You must select a dbf ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }
    }
}
