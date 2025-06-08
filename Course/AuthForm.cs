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
using System.Threading;
using System.IO;

namespace Course
{
    public partial class AuthForm : Form
    {
        private string captcha = "";
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
            return;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var hash = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(textBox2.Text));
            var password = BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
            if (textBox1.Text == ConfigurationManager.AppSettings["initLogin"] && 
                password == ConfigurationManager.AppSettings["initPwd"])
            {
                var f = new RecoveryForm();
                this.Visible = false;
                f.ShowDialog();
                try
                {
                    this.Visible = true;
                }
                catch
                {
                    ;
                }
                return;
            }
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
                Connection.ChangeDb(config.AppSettings.Settings["db"].Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DataTable userData;
            try
            {
                userData = Connection.GetUser(textBox1.Text, textBox2.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (userData.Rows.Count == 1 && (!textBox3.Visible || textBox3.Text == captcha))
            {
                var role = userData.Rows[0].ItemArray[userData.Columns["RoleName"].Ordinal].ToString();
                var id = userData.Rows[0].ItemArray[userData.Columns["RoleId"].Ordinal].ToString();
                var workerId = userData.Rows[0].ItemArray[userData.Columns["WorkerId"].Ordinal].ToString();
                User.Role = role;
                User.Id = id;
                User.WorkerId = workerId;
                var f = new MenuForm();
                this.Visible = false;
                f.ShowDialog();
                try
                {
                    this.Visible = true;
                    ClearForm(true);
                }
                catch
                {
                    ;
                }
            }
            else
            {
                var captchaVisible = textBox3.Visible;
                MessageBox.Show(textBox3.Visible ? "Неверно введены логин, пароль или Captcha" : "Неверно введены логин или пароль","",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                ClearForm(false);
                if (captchaVisible)
                {
                    ChangeCaptcha();
                    this.Enabled = false;
                    Thread.Sleep(10000);
                    this.Enabled = true;
                }
                else
                {
                    SetCaptchaVisibility(true);
                }
            }
        }
        private void ClearForm(bool hideCaptcha)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBoxUnderline1.BackColor = Color.DarkRed;
            textBoxUnderline2.BackColor = Color.DarkRed;
            SwitchButton();
            if (textBox3.Visible && hideCaptcha)
            {
                SetCaptchaVisibility(false);
            }
        }
        private void SetCaptchaVisibility(bool isVisible)
        {
            if (isVisible)
            {
                ChangeCaptcha();
            }
            this.Height = isVisible ? this.Height + 170 : this.Height - 170;
            label3.Visible = isVisible;
            textBox3.Visible = isVisible;
            button3.Visible = isVisible;
            pictureBox1.Visible = isVisible;
        }
        private void SwitchButton()
        {
            button1.Enabled = textBox1.Text.Length > 0 && textBox2.Text.Length > 0 && (textBox3.Text.Length == 4 || !textBox3.Visible);
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
            SwitchButton();
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
            SwitchButton();
        }

        private void hidePasswordButton1_Click(object sender, EventArgs e)
        {
            textBox2.UseSystemPasswordChar = !textBox2.UseSystemPasswordChar;
        }

        private void AuthForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.Visible && MessageBox.Show("Выйти?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) != DialogResult.OK)
            {
                e.Cancel = true;
                return;
            }
            var dumpDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"AIS\\Saves\\save_{DateTime.Now.Date.ToString("dd_MM_yyyy")}");
            var i = 1;
            while (Directory.Exists(dumpDir))
            {
                dumpDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"AIS\\Saves\\save_{DateTime.Now.Date.ToString("dd_MM_yyyy")}_{i}");
                i++;
            }
            Directory.CreateDirectory(dumpDir);
            string[] tables;
            try
            {
                tables = Connection.GetTables();
            }
            catch (Exception ex)
            {
                return;
            }
            foreach (var table in tables)
            {
                FileStream file = File.Create(Path.Combine(dumpDir, $"{table}_data.csv"));
                try
                {
                    Connection.ExportData(file, table);
                }
                catch (Exception ex)
                {
                    ;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var f = new ConfigForm();
            this.Visible = false;
            f.ShowDialog();
            this.Visible = true;
        }
        private void ChangeCaptcha()
        {
            var rnd = new Random();
            captcha = "";
            var alpha = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz1234567890";
            for (int i = 0; i < 4; i++)
            {
                captcha += alpha[rnd.Next(alpha.Length)];
            }
            var captchaImg = new Bitmap(150, 75);
            var g = Graphics.FromImage(captchaImg);
            g.Clear(Color.Tan);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    g.DrawLine(Pens.Peru, new Point(0, rnd.Next(i * 25, (i + 1) * 25)), new Point(150, rnd.Next(i * 25, (i + 1) * 25)));
                }

            }
            for (int i = 0; i < 10; i++)
            {
                g.DrawLine(Pens.Peru, new Point(rnd.Next(150), 0), new Point(rnd.Next(150), 75));
            }
            var font = new Font("Comic Sans MS", 14.25F, FontStyle.Strikeout, GraphicsUnit.Point);
            for (int i = 0; i < 4; i++)
            {
                g.DrawString(captcha[i].ToString(), font, Brushes.Black, new Point(rnd.Next(10 + 36 * i, 25 + 36 * i), rnd.Next(10, 50)));
            }
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    g.DrawLine(Pens.Peru, new Point(rnd.Next(i * 30, (i + 1) * 30), 0), new Point(rnd.Next(i * 30, (i + 1) * 30), 75));
                }
                
            }
            g.Save();
            pictureBox1.Image = captchaImg;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ChangeCaptcha();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            label4.BackColor = textBox3.Text.Length == 4 ? Color.DarkGreen : Color.DarkRed;
            SwitchButton();
        }

        private void AuthForm_Load(object sender, EventArgs e)
        {
            var forms = Application.OpenForms;
            for (int i = 0; i < forms.Count; i++)
            {
                if (!(forms[i] == this))
                {
                    forms[i].Close();
                }
            }
        }
    }
}
