using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Security.Cryptography;

namespace Course
{
    public partial class AuthForm : Form
    {
        public AuthForm()
        {
            InitializeComponent();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetterOrDigit(e.KeyChar) || 
                char.IsPunctuation(e.KeyChar) || 
                char.IsSeparator(e.KeyChar) ||
                e.KeyChar == (char)Keys.Back || 
                e.KeyChar == (char)Keys.Delete))
            {
                e.Handled = true; 
                return;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (!(char.IsLetterOrDigit(e.KeyChar) ||
            //    char.IsPunctuation(e.KeyChar) ||
            //    char.IsSeparator(e.KeyChar) ||
            //    e.KeyChar == (char)Keys.Back ||
            //    e.KeyChar == (char)Keys.Delete))
            //{
            //    e.Handled = true;
            //    return;
            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var hash = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(textBox2.Text));
            var password = BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
            if (textBox1.Text == ConfigurationManager.AppSettings["initLogin"] && 
                textBox2.Text == ConfigurationManager.AppSettings["initPwd"])
            {
                // navigate to db recovery
                return;
            }
            var userData = Connection.GetUser(textBox1.Text, textBox2.Text);
            if (userData.Rows.Count == 1)
            {
                var role = userData.Rows[0].ItemArray[userData.Columns["RoleName"].Ordinal].ToString();
                var id = userData.Rows[0].ItemArray[userData.Columns["RoleId"].Ordinal].ToString();
                var workerId = userData.Rows[0].ItemArray[userData.Columns["WorkerId"].Ordinal].ToString();
                User.Role = role;
                User.Id = id;
                User.WorkerId = workerId;
                if (User.Role == "Сотрудник")
                {
                    var f = new ListForm("product");
                    this.Visible = false;
                    f.ShowDialog();
                    ClearForm();
                    this.Visible = true;
                }
                else
                {
                    var f = new MenuForm();
                    this.Visible = false;
                    f.ShowDialog();
                    ClearForm();
                    this.Visible = true;
                }
            }
            else
            {
                MessageBox.Show("Неверно введены логин или пароль","",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                ClearForm();
            }
        }
        private void ClearForm()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBoxUnderline1.BackColor = Color.DarkRed;
            textBoxUnderline2.BackColor = Color.DarkRed;
            button1.Enabled = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
            {
                textBoxUnderline1.BackColor = Color.DarkGreen;
            }
            else if (textBox1.Text.Length == 0)
            {
                textBoxUnderline1.BackColor = Color.DarkRed;
            }
            button1.Enabled = textBox1.Text.Length > 0 && textBox2.Text.Length > 0;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text.Length > 0)
            {
                textBoxUnderline2.BackColor = Color.DarkGreen;
            }
            else if (textBox2.Text.Length == 0)
            {
                textBoxUnderline2.BackColor = Color.DarkRed;
            }
            button1.Enabled = textBox1.Text.Length > 0 && textBox2.Text.Length > 0;
        }

        private void hidePasswordButton1_Click(object sender, EventArgs e)
        {
            textBox2.UseSystemPasswordChar = !textBox2.UseSystemPasswordChar;
        }

        private void AuthForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Выйти?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) != DialogResult.OK)
            {
                e.Cancel = true;
            }
        }
    }
}
