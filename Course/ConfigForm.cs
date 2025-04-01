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
            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            textBox1.Text = config.AppSettings.Settings["host"].Value;
            textBox2.Text = config.AppSettings.Settings["user"].Value;
            textBox3.Text = config.AppSettings.Settings["pwd"].Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            config.AppSettings.Settings["host"].Value = textBox1.Text;
            config.AppSettings.Settings["user"].Value = textBox2.Text;
            config.AppSettings.Settings["pwd"].Value = textBox3.Text;
            config.Save(ConfigurationSaveMode.Minimal);
            Connection.Update();
            if (Connection.Test())
            {
                MessageBox.Show("Подключение установлено", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                try
                {
                    Connection.ChangeDb();
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
}
