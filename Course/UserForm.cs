using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Course
{
    public partial class UserForm : Form
    {
        Dictionary<string, string> user = null;
        public UserForm()
        {
            InitializeComponent();
            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";
            FillCombos();
        }
        public UserForm(Dictionary<string, string> user)
        {
            InitializeComponent();
            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";
            FillCombos();
            textBox2.Text = user["UserLogin"];
            try
            {
                comboBox1.SelectedValue = user["UserWorkerId"];
                comboBox2.SelectedItem = Connection.GetRoleById(user["UserRoleId"]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            textBoxUnderline2.BackColor = Color.DarkGoldenrod;
            this.user = user;
        }
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
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
        private void FillCombos()
        {
            try
            {
                comboBox1.DataSource = Connection.GetWorkers();
                comboBox2.DataSource = Connection.GetRoles();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void SwitchButton()
        {
            button1.Enabled = label3.BackColor == Color.DarkGreen &&
                label2.BackColor == Color.DarkGreen &&
                label11.BackColor == Color.DarkGreen &&
                (textBoxUnderline2.BackColor == Color.DarkGreen ||
                textBoxUnderline2.BackColor == Color.DarkGoldenrod);
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
            SwitchButton();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex >= 0)
            {
                label11.BackColor = Color.DarkGreen;
            }
            else
            {
                label11.BackColor = Color.DarkRed;
            }
            SwitchButton();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0)
            {
                label2.BackColor = Color.DarkGreen;
            }
            else
            {
                label2.BackColor = Color.DarkRed;
            }
            SwitchButton();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox3.Text.Length > 0)
            {
                textBoxUnderline2.BackColor = Color.DarkGreen;
            }
            else
            {
                if (user == null)
                {
                    textBoxUnderline2.BackColor = Color.DarkRed;
                }
                else
                {
                    textBoxUnderline2.BackColor = Color.DarkGoldenrod;
                }
            }
            SwitchButton();
        }

        private void hidePasswordButton1_Click(object sender, EventArgs e)
        {
            textBox3.UseSystemPasswordChar = !textBox3.UseSystemPasswordChar;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var entitySelected = user != null;
            if (user == null)
            {
                user = new Dictionary<string, string>();
            }
            user["UserLogin"] = textBox2.Text;
            if (textBoxUnderline2.BackColor != Color.DarkGoldenrod)
            {
                var a = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(textBox3.Text));
                var password = BitConverter.ToString(a).Replace("-", string.Empty).ToLower();
                user["UserPassword"] = password;
            }
            try
            {
                user["UserRoleId"] = Connection.GetRoleId(comboBox2.SelectedValue.ToString());
                user["UserWorkerId"] = ((KeyValuePair<string, string>)comboBox1.SelectedItem).Key;
            }
            catch (Exception ex)
            {
                user = null;
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (entitySelected)
            {
                //update
                if (MessageBox.Show("Редактировать запись?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) != DialogResult.OK)
                {
                    return;
                }
                bool updated;
                try
                {
                    updated = Connection.UpdateObject("user", user);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (updated)
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
                bool inserted;
                try
                {
                    inserted = Connection.InsertObject("user", user);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (inserted)
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

        private void comboBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;
            using (var brush = new SolidBrush(e.ForeColor))
            {
                e.DrawBackground();
                e.Graphics.DrawString(((KeyValuePair<string, string>)comboBox1.Items[e.Index]).Value.ToString(), e.Font, brush, e.Bounds);
                e.DrawFocusRectangle();
            }
        }
    }
}
