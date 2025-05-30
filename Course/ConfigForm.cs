using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Course
{
    public partial class ConfigForm : Form
    {
        public ConfigForm()
        {
            InitializeComponent();
            foreach (var c in this.Controls)
            {
                ((Control)c).KeyPress += new KeyPressEventHandler(ConfigForm_KeyPress);
                ((Control)c).MouseMove += new MouseEventHandler(ConfigForm_MouseMove);
                ((Control)c).MouseClick += new MouseEventHandler(ConfigForm_MouseClick);
                if (c is ScrollableControl)
                {
                    ((ScrollableControl)c).Scroll += new ScrollEventHandler(ConfigForm_Scroll);
                }
            }
            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            textBox1.Text = config.AppSettings.Settings["host"].Value;
            textBox2.Text = config.AppSettings.Settings["user"].Value;
            textBox4.Text = config.AppSettings.Settings["time"].Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            var connChanged = !(config.AppSettings.Settings["host"].Value == textBox1.Text &&
                config.AppSettings.Settings["user"].Value == textBox2.Text);
            if (connChanged)
            {
                config.AppSettings.Settings["host"].Value = textBox1.Text;
                config.AppSettings.Settings["user"].Value = textBox2.Text;
                config.AppSettings.Settings["pwd"].Value = textBox3.Text;
            }
            config.AppSettings.Settings["time"].Value = textBox4.Text;
            config.Save(ConfigurationSaveMode.Minimal);
            if (connChanged)
            {
                Connection.Update(config.AppSettings.Settings["host"].Value, config.AppSettings.Settings["user"].Value, config.AppSettings.Settings["pwd"].Value);
                if (Connection.Test())
                {
                    MessageBox.Show("Подключение установлено", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    try
                    {
                        Connection.ChangeDb(config.AppSettings.Settings["db"].Value);
                    }
                    catch
                    {
                        ;
                    }
                }
                else
                {
                    MessageBox.Show($"Не удалось подключиться к серверу по адресу {textBox1.Text} с пользователем {textBox2.Text}" + (textBox3.Text.Length == 0 ? "(без пароля)" : " (с паролем)"), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ConfigForm_MouseMove(object sender, MouseEventArgs e)
        {
            timer = 0;
        }

        private void ConfigForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            timer = 0;
        }

        private void ConfigForm_MouseClick(object sender, MouseEventArgs e)
        {
            timer = 0;
        }

        private int time = Convert.ToInt32(ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath).AppSettings.Settings["time"].Value);
        private int timer = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer++;
            if (timer >= time)
            {
                timer1.Stop();
                foreach (var f in Application.OpenForms)
                {
                    if (f is AuthForm)
                    {
                        ((AuthForm)f).Visible = true;
                    }
                    else
                    {
                        ((Form)f).Close();
                    }
                }
            }
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void ConfigForm_Scroll(object sender, ScrollEventArgs e)
        {
            timer = 0;
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsDigit(e.KeyChar) || e.KeyChar == ((char)Keys.Delete) || e.KeyChar == ((char)Keys.Back));

        }
    }
}
