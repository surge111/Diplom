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
            FillCombo();
        }
        private void FillCombo()
        {
            try
            {
                comboBox2.DataSource = Connection.GetTables();
                comboBox2.SelectedIndex = -1;
            }
            catch
            {
                ;
            }
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == -1)
            {
                button2.Enabled = false;
            }
            else
            {
                button2.Enabled = true;
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
            FillCombo();
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
            var data = new string[] { };
            using (var stream = d.OpenFile())
            {
                using (var rdr = new StreamReader(stream))
                {
                    var str = rdr.ReadLine();
                    
                    while (!rdr.EndOfStream)
                    {
                        str = rdr.ReadLine();
                        data = data.Append(str).ToArray();
                    }
                }
            }
            try
            {
                var res = Connection.ImportData(data, comboBox2.SelectedValue.ToString());
                if (res == -1)
                {
                    MessageBox.Show($"Импорт в таблицу {comboBox2.SelectedValue} успешно завершён. Добавлено {data.Length} записей", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                {
                    MessageBox.Show($"Не удалось импортировать данные. При импорте строки {res + 2} произошла ошибка", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBox2_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;
            using (var brush = new SolidBrush(e.ForeColor))
            {
                e.DrawBackground();
                e.Graphics.DrawString(comboBox2.Items[e.Index].ToString(), e.Font, brush, e.Bounds);
                e.DrawFocusRectangle();
            }
        }
    }
}
