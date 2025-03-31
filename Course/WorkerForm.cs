using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Course
{
    public partial class WorkerForm : Form
    {
        private Dictionary<string, string> worker = null;
        public WorkerForm()
        {
            InitializeComponent();
        }
        public WorkerForm(Dictionary<string, string> worker)
        {
            InitializeComponent();
            textBox1.Text = worker["WorkerSurname"];
            textBox2.Text = worker["WorkerName"];
            textBox3.Text = worker["WorkerPatronymic"];
            maskedTextBox1.Text = worker["WorkerPhone"].Substring(1);
            this.worker = worker;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!("ёйцукенгшщзхъфывапролджэячсмитьбю- ".Contains(e.KeyChar.ToString().ToLower()) ||
                e.KeyChar == (char)Keys.Back ||
                e.KeyChar == (char)Keys.Delete))
            {
                e.Handled = true;
                return;
            }
            if ("ёйцукенгшщзхъфывапролджэячсмитьбю".Contains(e.KeyChar.ToString().ToLower()) && textBox1.Text.Length == 0)
            {
                e.KeyChar = e.KeyChar.ToString().ToUpper()[0];
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!("ёйцукенгшщзхъфывапролджэячсмитьбю- ".Contains(e.KeyChar.ToString().ToLower()) ||
                e.KeyChar == (char)Keys.Back ||
                e.KeyChar == (char)Keys.Delete))
            {
                e.Handled = true;
                return;
            }
            if ("ёйцукенгшщзхъфывапролджэячсмитьбю".Contains(e.KeyChar.ToString().ToLower()) && textBox1.Text.Length == 0)
            {
                e.KeyChar = e.KeyChar.ToString().ToUpper()[0];
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!("ёйцукенгшщзхъфывапролджэячсмитьбю- ".Contains(e.KeyChar.ToString().ToLower()) ||
                e.KeyChar == (char)Keys.Back ||
                e.KeyChar == (char)Keys.Delete))
            {
                e.Handled = true;
                return;
            }
            if ("ёйцукенгшщзхъфывапролджэячсмитьбю".Contains(e.KeyChar.ToString().ToLower()) && textBox1.Text.Length == 0)
            {
                e.KeyChar = e.KeyChar.ToString().ToUpper()[0];
            }
        }

        private void maskedTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!("1234567890".Contains(e.KeyChar.ToString()) ||
                e.KeyChar == (char)Keys.Back ||
                e.KeyChar == (char)Keys.Delete))
            {
                e.Handled = true;
                return;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
            {
                textBoxUnderline1.BackColor = Color.DarkGreen;
            }
            else
            {
                textBoxUnderline1.BackColor = Color.DarkRed;
            }
            button1.Enabled = textBoxUnderline1.BackColor == Color.DarkGreen && label3.BackColor == Color.DarkGreen && label7.BackColor == Color.DarkGreen;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text.Length > 0)
            {
                label3.BackColor = Color.DarkGreen;
            }
            else
            {
                label3.BackColor = Color.DarkRed;
            }
            button1.Enabled = textBoxUnderline1.BackColor == Color.DarkGreen && label3.BackColor == Color.DarkGreen && label7.BackColor == Color.DarkGreen;
        }

        private void maskedTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (maskedTextBox1.MaskCompleted)
            {
                label7.BackColor = Color.DarkGreen;
            }
            else
            {
                label7.BackColor = Color.DarkRed;
            }
            button1.Enabled = textBoxUnderline1.BackColor == Color.DarkGreen && label3.BackColor == Color.DarkGreen && label7.BackColor == Color.DarkGreen;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var workerSelected = worker != null;
            if (worker == null)
            {
                worker = new Dictionary<string, string>();
            }
            worker["WorkerSurname"] = textBox1.Text;
            worker["WorkerName"] = textBox2.Text;
            worker["WorkerPatronymic"] = textBox3.Text;
            worker["WorkerPhone"] = maskedTextBox1.Text
                .Replace("+7", "8")
                .Replace("(", "")
                .Replace(")", "")
                .Replace("-", "");
            if (workerSelected)
            {
                //update
                if (MessageBox.Show("Редактировать запись?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) != DialogResult.OK)
                {
                    return;
                }
                if (Connection.UpdateObject("worker", worker))
                {
                    MessageBox.Show("Запись редактирована", "", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Не удалось редактировать запись", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                //insert
                if (MessageBox.Show("Добавить запись?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) != DialogResult.OK)
                {
                    return;
                }
                if (Connection.InsertObject("worker", worker))
                {
                    MessageBox.Show("Запись добавлена", "", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Не удалось добавить запись", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }
    }
}
