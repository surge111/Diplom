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
    public partial class ClientForm : Form
    {
        public ClientForm()
        {
            InitializeComponent();
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

        }
    }
}
