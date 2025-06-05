using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
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
        private int time = Convert.ToInt32(ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath).AppSettings.Settings["time"].Value);
        private int timer = 0;
        private int page = 1;
        private int totalPages = 1;
        private string tableName;
        private string orderId = null;
        private bool fillingOrder = false;
        public ListForm(string tableName, string orderId = null)
        {
            InitializeComponent();
            foreach (var c in this.Controls)
            {
                ((Control)c).KeyPress += new KeyPressEventHandler(ListForm_KeyPress);
                ((Control)c).MouseMove += new MouseEventHandler(ListForm_MouseMove);
                ((Control)c).MouseClick += new MouseEventHandler(ListForm_MouseClick);
                if (c is ScrollableControl)
                {
                    ((ScrollableControl)c).Scroll += new ScrollEventHandler(ListForm_Scroll);
                }
            }
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
                if (tableName == "product")
                {
                    button3.Visible = false;
                    button4.Visible = false;
                    contextMenuStrip1.Items.Remove(contextMenuStrip1.Items["добавитьToolStripMenuItem"]);
                    contextMenuStrip1.Items.Remove(contextMenuStrip1.Items["редактироватьToolStripMenuItem"]);
                    contextMenuStrip1.Items.Remove(contextMenuStrip1.Items["удалитьToolStripMenuItem"]);
                }
            }
            if (User.Role == "Менеджер")
            {
                if (tableName == "product")
                {
                    contextMenuStrip1.Items.Remove(contextMenuStrip1.Items["добавитьВЗаказToolStripMenuItem"]);
                }
            }
            if (tableName == "order")
            {
                panel1.Visible = false;
                contextMenuStrip1.Items.Remove(contextMenuStrip1.Items["добавитьToolStripMenuItem"]);
            }
            if (tableName != "product")
            {
                panel2.Visible = false;
                panel3.Visible = false;
                flowLayoutPanel2.Visible = false;
                contextMenuStrip1.Items.Remove(contextMenuStrip1.Items["добавитьВЗаказToolStripMenuItem"]);
            }
            FillCombos();
            GetTotalPages();
            InsertPages();
            FillDGVData();
            label1.Text = $"Всего записей: {dataGridView1.RowCount}/{entries}";
            timer1.Start();
        }
        private void InsertPages()
        {
            var btn1 = button1;
            var btn2 = button2;
            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel1.Controls.Add(btn1);
            for (int i = 1; i <= totalPages; i++)
            {
                var linkLabel = new LinkLabel();
                linkLabel.AutoSize = true;
                linkLabel.LinkColor = Color.Chocolate;
                linkLabel.Location = new Point(41, 8);
                linkLabel.Margin = new Padding(5, 8, 5, 0);
                linkLabel.Size = new Size(14, 18);
                linkLabel.TabIndex = 4;
                linkLabel.TabStop = true;
                linkLabel.VisitedLinkColor = Color.Chocolate;
                linkLabel.Click += new EventHandler(this.linkLabel_Click);
                linkLabel.Name = $"ll{i}";
                linkLabel.Text = i.ToString();
                if (linkLabel.Text == page.ToString())
                {
                    linkLabel.LinkBehavior = LinkBehavior.NeverUnderline;
                    linkLabel.LinkColor = Color.Sienna;
                }
                flowLayoutPanel1.Controls.Add(linkLabel);
            }
            flowLayoutPanel1.Controls.Add(btn2);
        }
        private void FillCombos()
        {
            if (panel2.Visible)
            {
                comboBox1.DataSource = new string[] { "Не сортировать", "Названию", "Стоимости" };
                comboBox1.SelectedIndex = 0;
                string[] categories;
                string[] suppliers;
                try
                {
                    categories = Connection.GetCategories();
                    suppliers = Connection.GetSuppliers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                for (var i = 0; i < categories.Length; i++)
                {
                    categories[i] = "Категория: " + categories[i];
                }
                for (var i = 0; i < suppliers.Length; i++)
                {
                    suppliers[i] = "Поставщик: " + suppliers[i];
                }
                comboBox2.DataSource = new string[] { "Не фильтровать" }.Concat(categories.Concat(suppliers).ToArray()).ToArray();
                comboBox2.SelectedIndex = 0;
            }
        }
        private void FillDGVData()
        {
            try
            {
                dataGridView1.DataSource = Connection.SelectTable(tableName,
                    textBox1.Text,
                    comboBox2.SelectedIndex > 0 ? comboBox2.SelectedValue.ToString() : "",
                    comboBox1.SelectedIndex > 0 ? comboBox1.SelectedValue.ToString() : "",
                    button5.Text == "↓" ? "desc" : "asc",
                    page);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            for (var i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns[i].Visible = false;
            }
            switch (tableName)
            {
                case "category":
                    {
                        dataGridView1.Columns["CategoryName"].Visible = true;
                        dataGridView1.Columns["CategoryName"].HeaderText = "Наименование";
                        break;
                    }
                case "supplier":
                    {
                        dataGridView1.Columns["SupplierName"].Visible = true;
                        dataGridView1.Columns["SupplierName"].HeaderText = "Наименование";
                        break;
                    }
                case "order":
                    {
                        dataGridView1.Columns["OrderId"].Visible = true;
                        dataGridView1.Columns["OrderDate"].Visible = true;
                        dataGridView1.Columns["OrderStatus"].Visible = true;
                        dataGridView1.Columns["OrderId"].HeaderText = "Номер заказа";
                        dataGridView1.Columns["OrderDate"].HeaderText = "Дата заказа";
                        dataGridView1.Columns["OrderStatus"].HeaderText = "Статус";
                        SetFioColumn(1);
                        dataGridView1.Columns["Fio"].HeaderText = "Сотрудник";
                        break;
                    }
                case "worker":
                    {
                        SetFioColumn(0);
                        SetHiddenPhoneColumn(1);
                        break;
                    }
                case "client":
                    {
                        SetFioColumn(0);
                        SetHiddenPhoneColumn(1);
                        break;
                    }
                case "user":
                    {
                        dataGridView1.Columns["UserLogin"].Visible = true;
                        dataGridView1.Columns["RoleName"].Visible = true;
                        dataGridView1.Columns["UserLogin"].HeaderText = "Логин";
                        dataGridView1.Columns["RoleName"].HeaderText = "Роль";
                        SetFioColumn(1);
                        dataGridView1.Columns["Fio"].HeaderText = "Работник";
                        break;
                    }
                case "product":
                    {
                        dataGridView1.Columns["ProductName"].Visible = true;
                        dataGridView1.Columns["ProductQuantity"].Visible = true;
                        dataGridView1.Columns["CategoryName"].Visible = true;
                        dataGridView1.Columns["SupplierName"].Visible = true;
                        dataGridView1.Columns["ProductName"].HeaderText = "Наименование";
                        dataGridView1.Columns["ProductQuantity"].HeaderText = "Кол-во";
                        dataGridView1.Columns["CategoryName"].HeaderText = "Категория";
                        dataGridView1.Columns["SupplierName"].HeaderText = "Поставщик";
                        SetImageColumn(1);
                        SetTotalCostColumn(5);
                        SetProductStatusColumn(6);
                        MarkProducts();
                        break;
                    }
            }

        }
        private void MarkProducts()
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (Convert.ToInt32(dataGridView1["ProductDiscount", i].Value.ToString()) >= 5)
                {
                    dataGridView1["TotalCost", i].Style.BackColor = Color.LightGreen;
                }
            }
        }
        private void SetHiddenPhoneColumn(int columnIndex)
        {
            DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
            col.Name = "HiddenPhone";
            col.HeaderText = "Телефон";
            try
            {
                dataGridView1.Columns.Remove("HiddenPhone");
            }
            catch
            {
                ;
            }
            dataGridView1.Columns.Insert(columnIndex, col);
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1[columnIndex, i].Value = $"{dataGridView1[tableName[0].ToString().ToUpper() + tableName.Substring(1) + "Phone", i].Value.ToString().Substring(0, 4) + "***" + dataGridView1[tableName[0].ToString().ToUpper() + tableName.Substring(1) + "Phone", i].Value.ToString().Substring(7)}";
            }
        }
        private void SetProductStatusColumn(int columnIndex)
        {
            DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
            col.Name = "Status";
            col.HeaderText = "Статус";
            try
            {
                dataGridView1.Columns.Remove("Status");
            }
            catch
            {
                ;
            }
            dataGridView1.Columns.Insert(columnIndex, col);
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1[columnIndex, i].Value = "В наличии";
                if (DateTime.Parse(dataGridView1["ProductExpirationDate", i].Value.ToString()).Date < DateTime.Now.Date)
                {
                    dataGridView1[columnIndex, i].Value = "Требуется замена";
                }
                if (dataGridView1["ProductQuantity", i].Value.ToString() == "0")
                {
                    dataGridView1[columnIndex, i].Value = "Нет в наличии";
                }
            }
        }
        private void SetFioColumn(int columnIndex)
        {
            DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
            col.Name = "Fio";
            col.HeaderText = "ФИО";
            try
            {
                dataGridView1.Columns.Remove("Fio");
            }
            catch
            {
                ;
            }
            dataGridView1.Columns.Insert(columnIndex, col);
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (tableName == "worker" || tableName == "client")
                {
                    var fio = $"{dataGridView1[tableName[0].ToString().ToUpper() + tableName.Substring(1) + "Surname", i].Value.ToString()}" + (dataGridView1[tableName[0].ToString().ToUpper() + tableName.Substring(1) + "Name", i].Value.ToString() != "" ? $" {dataGridView1[tableName[0].ToString().ToUpper() + tableName.Substring(1) + "Name", i].Value.ToString()[0]}." : "") + (dataGridView1[tableName[0].ToString().ToUpper() + tableName.Substring(1) + "Patronymic", i].Value.ToString() != "" ? $" {dataGridView1[tableName[0].ToString().ToUpper() + tableName.Substring(1) + "Patronymic", i].Value.ToString()[0]}." : "");
                    dataGridView1[columnIndex, i].Value = $"{dataGridView1[tableName[0].ToString().ToUpper() + tableName.Substring(1) + "Surname", i].Value} ..";
                }
                else
                {
                    dataGridView1[columnIndex, i].Value = $"{dataGridView1["WorkerSurname", i].Value} {dataGridView1["WorkerName", i].Value.ToString()[0]}.{dataGridView1["WorkerPatronymic", i].Value.ToString()[0]}.";
                }
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
            timer1.Stop();
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
                case "client":
                    {
                        var f = new ClientForm();
                        res = f.ShowDialog();
                        break;
                    }
            }
            if (res == DialogResult.OK)
            {
                FillDGVData();
                GetTotalPages();
            }
            timer1.Start();
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
                GetTotalPages();
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
                GetTotalPages();
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
            timer1.Stop();
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
                        entity.Add("OrderClientId", dataGridView1["OrderClientId", rowIndex].Value.ToString());
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
                case "client":
                    {
                        entity.Add("ClientId", dataGridView1["ClientId", rowIndex].Value.ToString());
                        entity.Add("ClientSurname", dataGridView1["ClientSurname", rowIndex].Value.ToString());
                        entity.Add("ClientName", dataGridView1["ClientName", rowIndex].Value.ToString());
                        entity.Add("ClientPatronymic", dataGridView1["ClientPatronymic", rowIndex].Value.ToString());
                        entity.Add("ClientPhone", dataGridView1["ClientPhone", rowIndex].Value.ToString());
                        var f = new ClientForm(entity);
                        res = f.ShowDialog();
                        break;
                    }
            }
            if (res == DialogResult.OK)
            {
                FillDGVData();
            }
            timer1.Start();
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count <= 0 || dataGridView1.SelectedRows[0].Index < 0)
            {
                return;
            }
            if (tableName == "order" && dataGridView1["OrderStatus", dataGridView1.SelectedRows[0].Index].Value.ToString() == "Проведён")
            {
                MessageBox.Show("Невозможно удалить проведённый заказ", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                return;
            }
            DialogResult res = DialogResult.None;
            var rowIndex = dataGridView1.SelectedRows[0].Index;
            var entity = new Dictionary<string, string>();
            res = MessageBox.Show("Удалить запись?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            if (res == DialogResult.OK)
            {
                bool deleted = false;
                try
                {
                    deleted = Connection.DeleteObject(tableName, dataGridView1[tableName[0].ToString().ToUpper() + tableName.Substring(1) + "Id", rowIndex].Value.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Не удалось удалить запись", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (deleted)
                {
                    MessageBox.Show("Запись успешно удалена", "", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    FillDGVData();
                    GetTotalPages();
                }
                else
                {
                    MessageBox.Show("Не удалось удалить запись", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }
        
        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            try
            {
                ReportBuilder.ProductQuantityReport(Connection.SelectTable(tableName,
                    textBox1.Text,
                    comboBox2.SelectedIndex > 0 ? comboBox2.SelectedValue.ToString() : "",
                    comboBox1.SelectedIndex > 0 ? comboBox1.SelectedValue.ToString() : "",
                    button5.Text == "↓" ? "desc" : "asc"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            timer1.Start();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            var f = new OrderForm(orderId);
            var res = f.ShowDialog();
            if (res == DialogResult.OK)
            {
                orderId = null;
            }
            timer1.Start();
        }

        private void добавитьВЗаказToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count <= 0)
            {
                return;
            }
            var rowIndex = dataGridView1.SelectedRows[0].Index;
            if (rowIndex < 0)
            {
                return;
            }
            if (orderId == null)
            {
                var order = new Dictionary<string, string>();
                order.Add("OrderWorkerId", User.WorkerId);
                order.Add("OrderClientId", "1");
                order.Add("OrderDate", DateTime.Now.Date.ToString("yyyy-MM-dd"));
                order.Add("OrderStatus", "Новый");
                try
                {
                    Connection.InsertObject("order", order);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                this.orderId = Connection.GetLastOrderId();
            }
            if (!this.fillingOrder)
            {
                button6.Visible = true;
            }
            var orderItem = new Dictionary<string, string>();
            orderItem.Add("OrderItemProductId", dataGridView1["ProductId", rowIndex].Value.ToString());
            orderItem.Add("OrderItemQuantity", "1");
            orderItem.Add("OrderItemCost", dataGridView1["TotalCost", rowIndex].Value.ToString().Replace(",", "."));
            Connection.AddItemToOrder(orderId, orderItem);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            var f = new DateSelectForm();
            var res = f.ShowDialog();
            if (res != DialogResult.OK)
            {
                timer1.Start();
                return;
            }
            try
            {
                ReportBuilder.ProductRevenueReport(Connection.SelectTable(tableName,
                    textBox1.Text,
                    comboBox2.SelectedIndex > 0 ? comboBox2.SelectedValue.ToString() : "",
                    comboBox1.SelectedIndex > 0 ? comboBox1.SelectedValue.ToString() : "",
                    button5.Text == "↓" ? "desc" : "asc"),
                    f.dateTimePicker1.Value.Date,
                    f.dateTimePicker2.Value.Date);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            timer1.Start();
        }

        private void linkLabel_Click(object sender, EventArgs e)
        {
            foreach (var ctrl in flowLayoutPanel1.Controls)
            {
                if (ctrl is LinkLabel)
                {
                    (ctrl as LinkLabel).LinkBehavior = LinkBehavior.AlwaysUnderline;
                    (ctrl as LinkLabel).LinkColor = Color.Chocolate;
                }
            }
            var ll = (sender as LinkLabel);
            ll.LinkBehavior = LinkBehavior.NeverUnderline;
            ll.LinkColor = Color.Sienna;
            page = Convert.ToInt32(ll.Text);
            FillDGVData();
            label1.Text = $"Всего записей: {dataGridView1.RowCount}/{entries}";
        }
        private int entries;
        private void GetTotalPages()
        {
            
            try
            {
                entries = Connection.SelectTableLength(tableName,
                    textBox1.Text,
                    comboBox2.SelectedIndex > 0 ? comboBox2.SelectedValue.ToString() : "",
                    comboBox1.SelectedIndex > 0 ? comboBox1.SelectedValue.ToString() : "",
                    button5.Text == "↓" ? "desc" : "asc");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            totalPages = entries / 20 + ((entries % 20 == 0 && entries != 0) ? 0 : 1);
            page = 1;
            label1.Text = $"Всего записей: {dataGridView1.RowCount}/{entries}";
            InsertPages();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (page == 1)
            {
                return;
            }
            page--;
            InsertPages();
            FillDGVData();
            label1.Text = $"Всего записей: {dataGridView1.RowCount}/{entries}";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            panel3.Visible = false;
        }

        private void условноеФорматированиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel3.Visible = true;
        }

        private void ListForm_SizeChanged(object sender, EventArgs e)
        {
            dataGridView1.Height = this.Height - 250;
        }

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

        private void ListForm_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                timer1.Start();
            }
            else
            {
                timer1.Stop();
            }
        }

        private void ListForm_MouseMove(object sender, MouseEventArgs e)
        {
            timer = 0;
        }

        private void ListForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            timer = 0;
        }

        private void ListForm_MouseClick(object sender, MouseEventArgs e)
        {
            timer = 0;
        }

        private void ListForm_Scroll(object sender, ScrollEventArgs e)
        {
            timer = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (page == totalPages)
            {
                return;
            }
            page++;
            InsertPages();
            FillDGVData();
            label1.Text = $"Всего записей: {dataGridView1.RowCount}/{entries}";
        }

        private void ListForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
        }
    }
}
