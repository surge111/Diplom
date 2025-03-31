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
    public partial class DictionaryForm : Form
    {
        private string tableName;
        private Dictionary<string, string> entity = null;
        public DictionaryForm(string tableName)
        {
            InitializeComponent();
            this.tableName = tableName;
        }
        public DictionaryForm(string tableName, Dictionary<string, string> entity)
        {
            InitializeComponent();
            this.tableName = tableName;
            textBox2.Text = entity[$"{tableName[0].ToString().ToUpper() + tableName.Substring(1)}Name"];
            this.entity = entity;
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!("ёйцукенгшщзхъфывапролджэячсмитьбю1234567890- ".Contains(e.KeyChar.ToString().ToLower()) ||
                char.IsPunctuation(e.KeyChar) ||
                e.KeyChar == (char)Keys.Back ||
                e.KeyChar == (char)Keys.Delete))
            {
                e.Handled = true;
                return;
            }
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
            button1.Enabled = label3.BackColor == Color.DarkGreen;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var entitySelected = entity != null;
            if (entity == null)
            {
                entity = new Dictionary<string, string>();
            }
            entity[tableName[0].ToString().ToUpper() + tableName.Substring(1) + "Name"] = textBox2.Text;
            if (entitySelected)
            {
                //update
                if (MessageBox.Show("Редактировать запись?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) != DialogResult.OK)
                {
                    return;
                }
                if (Connection.UpdateObject(tableName, entity))
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
                if (Connection.InsertObject(tableName, entity))
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
