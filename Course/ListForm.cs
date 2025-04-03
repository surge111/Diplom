using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Course
{
    public partial class ListForm : Form
    {
        private int totalPages = 1;
        private string tableName;
        private string orderId = null;
        private bool fillingOrder = false;
        public ListForm(string tableName, string orderId = null)
        {
            InitializeComponent();
            this.tableName = tableName;
            if (tableName == "product" && orderId != null)
            {
                this.orderId = orderId;
                fillingOrder = true;
            }
        }
        private void ListForm_Load(object sender, EventArgs e)
        {
            if (User.Role == "Сотрудник")
            {
                flowLayoutPanel2.Visible = false;
                contextMenuStrip1.Items.Remove(contextMenuStrip1.Items["добавитьToolStripMenuItem"]);
                contextMenuStrip1.Items.Remove(contextMenuStrip1.Items["редактироватьToolStripMenuItem"]);
                contextMenuStrip1.Items.Remove(contextMenuStrip1.Items["удалитьToolStripMenuItem"]);
            }
            if (User.Role == "Менеджер")
            {
                contextMenuStrip1.Items.Remove(contextMenuStrip1.Items["удалитьToolStripMenuItem"]);
            }
            if (tableName != "product")
            {
                panel2.Visible = false;
                flowLayoutPanel2.Visible = false;
                contextMenuStrip1.Items.Remove(contextMenuStrip1.Items["добавитьВЗаказToolStripMenuItem"]);
            }
            if (tableName == "order")
            {
                panel1.Visible = false;
                contextMenuStrip1.Items.Remove(contextMenuStrip1.Items["добавитьToolStripMenuItem"]);
            }
            FillCombos();
            FillDGVData();
            InsertPages();
        }
        private void InsertPages()
        {
            var btn = button2;
            var linkLable = new LinkLabel();
            linkLable.AutoSize = true;
            linkLable.LinkColor = System.Drawing.Color.Chocolate;
            linkLable.Location = new System.Drawing.Point(41, 8);
            linkLable.Margin = new System.Windows.Forms.Padding(5, 8, 5, 0);
            linkLable.Size = new System.Drawing.Size(14, 18);
            linkLable.TabIndex = 4;
            linkLable.TabStop = true;
            linkLable.Text = "1";
            linkLable.VisitedLinkColor = System.Drawing.Color.Sienna;
            flowLayoutPanel1.Controls.Remove(btn);
            for (int i = 1; i <= totalPages; i++)
            {
                linkLable.Text = i.ToString();
                flowLayoutPanel1.Controls.Add(linkLabel1);
            }
            flowLayoutPanel1.Controls.Add(btn);
        }
        private void FillCombos()
        {
            if (panel2.Visible)
            {
                var categories = Connection.GetCategories();
                for (var i = 0; i < categories.Length; i++)
                {
                    categories[i] = "Категория: " + categories[i];
                }
                var suppliers = Connection.GetSuppliers();
                for (var i = 0; i < suppliers.Length; i++)
                {
                    suppliers[i] = "Поставщик: " + suppliers[i];
                }
                comboBox2.DataSource = new string[] { "Не фильтровать" }.Concat(categories.Concat(suppliers).ToArray()).ToArray();
                comboBox1.DataSource = new string[] { "Не сортировать", "Названию", "Стоимости" };
                comboBox2.SelectedIndex = 0;
                comboBox1.SelectedIndex = 0;
            }
        }
        private void FillDGVData()
        {
            dataGridView1.DataSource = Connection.SelectTable(tableName, 
                textBox1.Text, 
                comboBox2.SelectedIndex > 0 ? comboBox2.SelectedValue.ToString():"",
                comboBox1.SelectedIndex > 0 ? comboBox1.SelectedValue.ToString():"", 
                button5.Text == "↓"?"desc":"asc");
            for (var i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns[i].Visible = false;
            }
            switch (tableName)
            {
                case "category":
                    {
                        dataGridView1.Columns["CategoryId"].Visible = true;
                        dataGridView1.Columns["CategoryName"].Visible = true;
                        dataGridView1.Columns["CategoryId"].HeaderText = "#";
                        dataGridView1.Columns["CategoryName"].HeaderText = "Наименование";
                        break;
                    }
                case "supplier":
                    {
                        dataGridView1.Columns["SupplierId"].Visible = true;
                        dataGridView1.Columns["SupplierName"].Visible = true;
                        dataGridView1.Columns["SupplierId"].HeaderText = "#";
                        dataGridView1.Columns["SupplierName"].HeaderText = "Наименование";
                        break;
                    }
                case "order":
                    {
                        dataGridView1.Columns["OrderId"].Visible = true;
                        dataGridView1.Columns["OrderDate"].Visible = true;
                        dataGridView1.Columns["OrderStatus"].Visible = true;
                        dataGridView1.Columns["OrderId"].HeaderText = "#";
                        dataGridView1.Columns["OrderDate"].HeaderText = "Дата заказа";
                        dataGridView1.Columns["OrderStatus"].HeaderText = "Статус";
                        SetFioColumn(1);
                        break;
                    }
                case "worker":
                    {
                        dataGridView1.Columns["WorkerId"].Visible = true;
                        dataGridView1.Columns["WorkerPhone"].Visible = true;
                        dataGridView1.Columns["WorkerId"].HeaderText = "#";
                        dataGridView1.Columns["WorkerPhone"].HeaderText = "Телефон";
                        SetFioColumn(1);
                        break;
                    }
                case "user":
                    {
                        dataGridView1.Columns["UserId"].Visible = true;
                        dataGridView1.Columns["UserLogin"].Visible = true;
                        dataGridView1.Columns["RoleName"].Visible = true;
                        dataGridView1.Columns["UserId"].HeaderText = "#";
                        dataGridView1.Columns["UserLogin"].HeaderText = "Логин";
                        dataGridView1.Columns["RoleName"].HeaderText = "Роль";
                        SetFioColumn(1);
                        break;
                    }
                case "product":
                    {
                        dataGridView1.Columns["ProductId"].Visible = true;
                        dataGridView1.Columns["ProductName"].Visible = true;
                        dataGridView1.Columns["ProductQuantity"].Visible = true;
                        dataGridView1.Columns["CategoryName"].Visible = true;
                        dataGridView1.Columns["SupplierName"].Visible = true;
                        dataGridView1.Columns["ProductId"].HeaderText = "#";
                        dataGridView1.Columns["ProductName"].HeaderText = "Наименование";
                        dataGridView1.Columns["ProductQuantity"].HeaderText = "Кол-во";
                        dataGridView1.Columns["CategoryName"].HeaderText = "Категория";
                        dataGridView1.Columns["SupplierName"].HeaderText = "Поставщик";
                        SetImageColumn(1);
                        SetTotalCostColumn(5);
                        break;
                    }
            }

        }

        private void SetFioColumn(int columnIndex)
        {
            DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
            col.Name = "WorkerFio";
            col.HeaderText = "ФИО";
            try
            {
                dataGridView1.Columns.Remove("WorkerFio");
            }
            catch
            {
                ;
            }
            dataGridView1.Columns.Insert(columnIndex, col);
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1[columnIndex, i].Value = $"{dataGridView1["WorkerSurname", i].Value} {dataGridView1["WorkerName", i].Value.ToString()[0]}.{dataGridView1["WorkerPatronymic", i].Value.ToString()[0]}.";
            }
        }
        private void SetImageColumn(int columnIndex)
        {
            var col = new DataGridViewImageColumn();
            col.Name = "Image";
            col.HeaderText = "Изображение";
            col.ImageLayout = DataGridViewImageCellLayout.Stretch;
            try
            {
                dataGridView1.Columns.Remove("Image");
            }
            catch
            {
                ;
            }
            dataGridView1.Columns.Insert(columnIndex, col);
            for (var i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Height = 80;
                if (dataGridView1["ProductImage", i].Value.ToString() != "")
                {
                    try
                    {
                        var imgPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\SetupInstaller\\Resources\\ProductImages\\" + dataGridView1["ProductImage", i].Value.ToString();
                        dataGridView1.Rows[i].Cells[columnIndex].Value = new Bitmap(imgPath);
                    }
                    catch (Exception)
                    {
                        dataGridView1.Rows[i].Cells[columnIndex].Value = Properties.Resources.picture;
                    }
                }
                else
                {
                    dataGridView1.Rows[i].Cells[columnIndex].Value = Properties.Resources.picture;
                }
            }
        }
        private void SetTotalCostColumn(int columnIndex)
        {
            DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
            col.Name = "TotalCost";
            col.HeaderText = "Стоимость";
            col.ValueType = typeof(float);
            try
            {
                dataGridView1.Columns.Remove("TotalCost");
            }
            catch
            {
                ;
            }
            dataGridView1.Columns.Insert(columnIndex, col);
            for (var i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1["ProductDiscount", i].Value.ToString() != "0")
                {
                    dataGridView1[columnIndex, i].Value = Math.Round(Convert.ToSingle(dataGridView1["ProductCost", i].Value.ToString()) * (Convert.ToSingle((100 - Convert.ToInt32(dataGridView1["ProductDiscount", i].Value.ToString()))) / 100), 2, MidpointRounding.AwayFromZero).ToString();
                }
                else
                {
                    dataGridView1[columnIndex, i].Value = dataGridView1["ProductCost", i].Value.ToString();
                }
            }
        }
        private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult res = DialogResult.None;
            switch (tableName)
            {
                case "product":
                    {
                        var f = new ProductForm();
                        res = f.ShowDialog();
                        break;
                    }
                case "user":
                    {
                        var f = new UserForm();
                        res = f.ShowDialog();
                        break;
                    }
                case "worker":
                    {
                        var f = new WorkerForm();
                        res = f.ShowDialog();
                        break;
                    }
                case "supplier":
                    {
                        var f = new DictionaryForm(tableName);
                        res = f.ShowDialog();
                        break;
                    }
                case "category":
                    {
                        var f = new DictionaryForm(tableName);
                        res = f.ShowDialog();
                        break;
                    }
            }
            if (res == DialogResult.OK)
            {
                FillDGVData();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (button5.Text == "↑")
            {
                button5.Text = "↓";
            }
            else
            {
                button5.Text = "↑";
            }
            if (comboBox1.SelectedIndex > 0)
            {
                FillDGVData();
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length >= 2 || textBox1.Text == "")
            {
                FillDGVData();
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!(("ёйцукенгшщзхъфывапролджэячсмитьбю1234567890- ".Contains(e.KeyChar.ToString().ToLower()) ||
                char.IsPunctuation(e.KeyChar) ||
                e.KeyChar == (char)Keys.Back ||
                e.KeyChar == (char)Keys.Delete)) && tableName != "user") ||
                (!((char.IsLetterOrDigit(e.KeyChar) ||
                char.IsPunctuation(e.KeyChar) ||
                "- ".Contains(e.KeyChar.ToString()) ||
                e.KeyChar == (char)Keys.Back ||
                e.KeyChar == (char)Keys.Delete)) && tableName == "user"))
            {
                e.Handled = true;
                return;
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

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex >= 0)
            {
                FillDGVData();
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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0)
            {
                FillDGVData();
            }
        }

        private void редактироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count <= 0)
            {
                return;
            }
            DialogResult res = DialogResult.None;
            var rowIndex = dataGridView1.SelectedRows[0].Index;
            var entity = new Dictionary<string, string>();
            switch (tableName)
            {
                case "product":
                    {
                        entity.Add("ProductId", dataGridView1["ProductId", rowIndex].Value.ToString());
                        entity.Add("ProductName", dataGridView1["ProductName", rowIndex].Value.ToString());
                        entity.Add("ProductQuantity", dataGridView1["ProductQuantity", rowIndex].Value.ToString());
                        entity.Add("ProductCost", dataGridView1["ProductCost", rowIndex].Value.ToString());
                        entity.Add("ProductDiscount", dataGridView1["ProductDiscount", rowIndex].Value.ToString());
                        entity.Add("ProductCategoryId", dataGridView1["ProductCategoryId", rowIndex].Value.ToString());
                        entity.Add("ProductSupplierId", dataGridView1["ProductSupplierId", rowIndex].Value.ToString());
                        entity.Add("ProductExpirationDate", dataGridView1["ProductExpirationDate", rowIndex].Value.ToString());
                        entity.Add("ProductImage", dataGridView1["ProductImage", rowIndex].Value.ToString());
                        var f = new ProductForm(entity);
                        res = f.ShowDialog();
                        break;
                    }
                case "order":
                    {
                        entity.Add("OrderId", dataGridView1["OrderId", rowIndex].Value.ToString());
                        entity.Add("OrderWorkerId", dataGridView1["OrderWorkerId", rowIndex].Value.ToString());
                        entity.Add("OrderDate", dataGridView1["OrderDate", rowIndex].Value.ToString());
                        entity.Add("OrderStatus", dataGridView1["OrderStatus", rowIndex].Value.ToString());
                        var f = new OrderForm(entity);
                        res = f.ShowDialog();
                        break;
                    }
                case "user":
                    {
                        entity.Add("UserId", dataGridView1["UserId", rowIndex].Value.ToString());
                        entity.Add("UserLogin", dataGridView1["UserLogin", rowIndex].Value.ToString());
                        entity.Add("UserRoleId", dataGridView1["UserRoleId", rowIndex].Value.ToString());
                        entity.Add("UserWorkerId", dataGridView1["UserWorkerId", rowIndex].Value.ToString());
                        var f = new UserForm(entity);
                        res = f.ShowDialog();
                        break;
                    }
                case "worker":
                    {
                        entity.Add("WorkerId", dataGridView1["WorkerId", rowIndex].Value.ToString());
                        entity.Add("WorkerSurname", dataGridView1["WorkerSurname", rowIndex].Value.ToString());
                        entity.Add("WorkerName", dataGridView1["WorkerName", rowIndex].Value.ToString());
                        entity.Add("WorkerPatronymic", dataGridView1["WorkerPatronymic", rowIndex].Value.ToString());
                        entity.Add("WorkerPhone", dataGridView1["WorkerPhone", rowIndex].Value.ToString());
                        var f = new WorkerForm(entity);
                        res = f.ShowDialog();
                        break;
                    }
                case "supplier":
                    {
                        entity.Add("SupplierId", dataGridView1["SupplierId", rowIndex].Value.ToString());
                        entity.Add("SupplierName", dataGridView1["SupplierName", rowIndex].Value.ToString());
                        var f = new DictionaryForm(tableName, entity);
                        res = f.ShowDialog();
                        break;
                    }
                case "category":
                    {
                        entity.Add("CategoryId", dataGridView1["CategoryId", rowIndex].Value.ToString());
                        entity.Add("CategoryName", dataGridView1["CategoryName", rowIndex].Value.ToString());
                        var f = new DictionaryForm(tableName, entity);
                        res = f.ShowDialog();
                        break;
                    }
            }
            if (res == DialogResult.OK)
            {
                FillDGVData();
            }
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count <= 0)
            {
                return;
            }
            DialogResult res = DialogResult.None;
            var rowIndex = dataGridView1.SelectedRows[0].Index;
            var entity = new Dictionary<string, string>();
            res = MessageBox.Show("Удалить запись?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            if (res == DialogResult.OK)
            {
                if (Connection.DeleteObject(tableName, dataGridView1[tableName[0].ToString().ToUpper() + tableName.Substring(1) + "Id", rowIndex].Value.ToString()))
                {
                    MessageBox.Show("Запись успешно удалена", "", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    FillDGVData();
                }
                else
                {
                    MessageBox.Show("Не удалось удалить запись", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }
        
        private void button3_Click(object sender, EventArgs e)
        {
            ReportBuilder.ProductQuantityReport(Connection.SelectTable(tableName,
                textBox1.Text,
                comboBox2.SelectedIndex > 0 ? comboBox2.SelectedValue.ToString() : "",
                comboBox1.SelectedIndex > 0 ? comboBox1.SelectedValue.ToString() : "",
                button5.Text == "↓" ? "desc" : "asc"));
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var f = new OrderForm(orderId);
            var res = f.ShowDialog();
            if (res == DialogResult.OK)
            {
                orderId = null;
            }
        }

        private void добавитьВЗаказToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count <= 0)
            {
                return;
            }
            if (orderId == null)
            {
                var order = new Dictionary<string, string>();
                order.Add("OrderWorkerId", User.WorkerId);
                order.Add("OrderDate", DateTime.Now.Date.ToString("yyyy-MM-dd"));
                order.Add("OrderStatus", "Новый");
                Connection.InsertObject("order", order);
                this.orderId = Connection.GetLastOrderId();
            }
            if (!this.fillingOrder)
            {
                button6.Visible = true;
            }
            var rowIndex = dataGridView1.SelectedRows[0].Index;
            var orderItem = new Dictionary<string, string>();
            orderItem.Add("OrderItemProductId", dataGridView1["ProductId", rowIndex].Value.ToString());
            orderItem.Add("OrderItemQuantity", "1");
            orderItem.Add("OrderItemCost", dataGridView1["TotalCost", rowIndex].Value.ToString().Replace(",", "."));
            Connection.AddItemToOrder(orderId, orderItem);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var f = new DateSelectForm();
            var res = f.ShowDialog();
            if (res != DialogResult.OK)
            {
                return;
            }
            ReportBuilder.ProductRevenueReport(Connection.SelectTable(tableName,
                textBox1.Text,
                comboBox2.SelectedIndex > 0 ? comboBox2.SelectedValue.ToString() : "",
                comboBox1.SelectedIndex > 0 ? comboBox1.SelectedValue.ToString() : "",
                button5.Text == "↓" ? "desc" : "asc"),
                f.dateTimePicker1.Value.Date,
                f.dateTimePicker2.Value.Date);
        }
    }
}
