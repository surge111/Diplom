using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Course
{
    public partial class ProductForm : Form
    {
        Dictionary<string, string> product = null;
        private string imagePath = "";
        public ProductForm()
        {
            InitializeComponent();
            FillCombos();
            pictureBox1.Image = Properties.Resources.picture;
        }
        public ProductForm(Dictionary<string, string> product)
        {
            InitializeComponent();
            FillCombos();
            textBox2.Text = product["ProductName"];
            textBox3.Text = product["ProductQuantity"];
            textBox4.Text = product["ProductCost"];
            textBox5.Text = product["ProductDiscount"];
            dateTimePicker1.Value = DateTime.Parse(product["ProductExpirationDate"]);
            comboBox1.SelectedItem = Connection.GetCategoryById(product["ProductCategoryId"]);
            comboBox2.SelectedItem = Connection.GetSupplierById(product["ProductSupplierId"]);
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\SetupInstaller\\Resources\\ProductImages\\");
            try
            {
                pictureBox1.Image = new Bitmap(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\SetupInstaller\\Resources\\ProductImages\\" + product["ProductImage"]);
                imagePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\SetupInstaller\\Resources\\ProductImages\\" + product["ProductImage"];
            }
            catch (Exception)
            {
                pictureBox1.Image = Properties.Resources.picture;
            }
            this.product = product;
        }
        private void FillCombos()
        {
            comboBox1.DataSource = Connection.GetCategories();
            comboBox2.DataSource = Connection.GetSuppliers();
        }
        private void SwitchButton()
        {
            button1.Enabled = label3.BackColor == Color.DarkGreen &&
                label5.BackColor == Color.DarkGreen &&
                label7.BackColor == Color.DarkGreen &&
                label9.BackColor == Color.DarkGreen &&
                label14.BackColor == Color.DarkGreen &&
                label11.BackColor == Color.DarkGreen &&
                comboBoxUnderline1.BackColor == Color.DarkGreen;
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
            SwitchButton();
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!("1234567890".Contains(e.KeyChar.ToString()) ||
                e.KeyChar == (char)Keys.Back ||
                e.KeyChar == (char)Keys.Delete))
            {
                e.Handled = true;
                return;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox3.Text.Length > 0)
            {
                label5.BackColor = Color.DarkGreen;
            }
            else
            {
                label5.BackColor = Color.DarkRed;
            }
            SwitchButton();
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!("1234567890.".Contains(e.KeyChar.ToString()) ||
                e.KeyChar == (char)Keys.Back ||
                e.KeyChar == (char)Keys.Delete))
            {
                e.Handled = true;
                return;
            }
            if ((textBox4.Text.Length == 0 || textBox4.Text.Contains('.')) && e.KeyChar == '.')
            {
                e.Handled = true;
                return;
            }
            if (textBox4.Text.Length > 3 && textBox4.Text[textBox4.Text.Length - 3] == '.')
            {
                e.Handled = true;
                return;
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (textBox4.Text.Length > 0)
            {
                label7.BackColor = Color.DarkGreen;
            }
            else
            {
                label7.BackColor = Color.DarkRed;
            }
            SwitchButton();
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!("1234567890".Contains(e.KeyChar.ToString()) ||
                e.KeyChar == (char)Keys.Back ||
                e.KeyChar == (char)Keys.Delete))
            {
                e.Handled = true;
                return;
            }
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
                comboBoxUnderline1.BackColor = Color.DarkGreen;
            }
            else
            {
                comboBoxUnderline1.BackColor = Color.DarkRed;
            }
            SwitchButton();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (textBox5.Text.Length > 0)
            {
                label9.BackColor = Color.DarkGreen;
            }
            else
            {
                label9.BackColor = Color.DarkRed;
            }
            SwitchButton();
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
                e.Graphics.DrawString(comboBox1.Items[e.Index].ToString(), e.Font, brush, e.Bounds);
                e.DrawFocusRectangle();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            var file = new OpenFileDialog();
            file.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\SetupInstaller\\Resources\\ProductImages\\";
            file.Filter = "jpg images|*.jpg";
            file.Multiselect = false;
            var res = file.ShowDialog();
            if (res == DialogResult.OK)
            {
                pictureBox1.Image.Dispose();
                imagePath = file.FileName;
                pictureBox1.Image = new Bitmap(file.FileName);
            }
            file.Dispose();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            var productSelected = product != null;
            //copy image to res
            int i = 1;
            var newImg = imagePath.Substring(imagePath.LastIndexOf("\\") + 1);
            while (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\SetupInstaller\\Resources\\ProductImages\\" + newImg))
            {
                newImg = imagePath.Substring(imagePath.LastIndexOf("\\") + 1, imagePath.Substring(imagePath.LastIndexOf("\\") + 1).Length - 4) + $" ({i}).jpg";
                i++;
            }
                
            try
            {
                File.Copy(imagePath,
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\SetupInstaller\\Resources\\ProductImages\\" + newImg,
                    true);
                imagePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\SetupInstaller\\Resources\\ProductImages\\" + newImg;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось установить изображение:\n" + imagePath + "\n" + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (imagePath != "")
            {
                imagePath = imagePath.Substring(imagePath.LastIndexOf("\\") + 1);
            }
            if (product == null)
            {
                product = new Dictionary<string, string>();
            }
            product["ProductName"] = textBox2.Text;
            product["ProductQuantity"] = textBox3.Text;
            product["ProductCost"] = textBox4.Text;
            product["ProductDiscount"] = textBox5.Text;
            product["ProductExpirationDate"] = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            product["ProductCategoryId"] = Connection.GetCategoryId(comboBox1.SelectedItem.ToString());
            product["ProductSupplierId"] = Connection.GetSupplierId(comboBox2.SelectedItem.ToString());
            product["ProductImage"] = imagePath;
            if (productSelected)
            {
                //update
                if (MessageBox.Show("Редактировать запись?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) != DialogResult.OK)
                {
                    return;
                }
                if (Connection.UpdateObject("product", product))
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
                if (Connection.InsertObject("product", product))
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
