using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Course
{
    public partial class RecoveryForm : Form
    {
        public RecoveryForm()
        {
            InitializeComponent();
        }

        private void RecoveryForm_Load(object sender, EventArgs e)
        {
            if (Connection.Test())
            {
                var arr = new string[]
                {
                    "category",
                    "order",
                    "orderitem",
                    "product",
                    "role",
                    "supplier",
                    "user",
                    "worker"
                };
                comboBox2.DataSource = arr;
            }
            else
            {
                MessageBox.Show("Не удалось подключиться к БД", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == -1)
            {
                button2.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Connection.RecoverDb();
                MessageBox.Show("БД восстановлена", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var d = new OpenFileDialog();
            d.Filter = "Comma Separated Values (*.csv) | *.csv";
            d.Multiselect = false;
            if (d.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            using (var stream = d.OpenFile())
            {
                using (var rdr = new StreamReader(stream))
                {
                    var str = "";
                    while (!rdr.EndOfStream)
                    {
                        str = rdr.ReadLine();
                        var values = str.Split(';');
                        
                    }
                }
            }
            
        }
    }
}
