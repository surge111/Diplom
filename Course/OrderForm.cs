using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Course
{
    public partial class OrderForm : Form
    {
        private Dictionary<string, string> order = new Dictionary<string, string>();
        
        public OrderForm()
        {
            InitializeComponent();
            comboBox2.DisplayMember = "Value";
            comboBox2.ValueMember = "Key";
            FillCombos();
            comboBox2.SelectedValue = User.WorkerId;
            dateTimePicker1.Value = DateTime.Now;
            order.Add("OrderWorkerId", comboBox2.SelectedValue.ToString());
            order.Add("OrderDate", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
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
        }
        public OrderForm(string orderId)
        {
            InitializeComponent();
            comboBox2.DisplayMember = "Value";
            comboBox2.ValueMember = "Key";
            FillCombos();
            comboBox2.SelectedValue = User.WorkerId;
            dateTimePicker1.Value = DateTime.Now;
            order.Add("OrderId", orderId);
            order.Add("OrderWorkerId", comboBox2.SelectedValue.ToString());
            order.Add("OrderDate", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
            order.Add("OrderStatus", "Новый");
            if (User.Role == "Сотрудник")
            {
                comboBox2.Enabled = false;
                dateTimePicker1.Enabled = false;
            }
            FillDGV();
        }
        public OrderForm(Dictionary<string, string> order)
        {
            InitializeComponent();
            comboBox2.DisplayMember = "Value";
            comboBox2.ValueMember = "Key";
            FillCombos();
            try
            {
                comboBox2.SelectedValue = order["OrderWorkerId"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            dateTimePicker1.Value = DateTime.Parse(order["OrderDate"]).Date;
            if (order["OrderStatus"] == "Проведён")
            {
                button1.Text = "Сформировать чек";
                dataGridView1.ContextMenuStrip = null;
                comboBox2.Enabled = false;
                dateTimePicker1.Enabled = false;
            }
            this.order = order;
            FillDGV();
        }
        private void FillCombos()
        {
            try
            {
                comboBox2.DataSource = Connection.GetWorkers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void FillDGV()
        {
            dataGridView1.DataSource = Connection.GetOrderItems(order["OrderId"]);
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].Visible = false;
            }
            dataGridView1.Columns["OrderItemCost"].Visible = true;
            dataGridView1.Columns["OrderItemQuantity"].Visible = true;
            dataGridView1.Columns["ProductName"].Visible = true;
            dataGridView1.Columns["ProductName"].HeaderText = "Товар";
            dataGridView1.Columns["OrderItemQuantity"].HeaderText = "Количество";
            dataGridView1.Columns["OrderItemCost"].HeaderText = "Стоимость";
            SwitchButton();
        }
        private void SwitchButton()
        {
            button1.Enabled = label11.BackColor == Color.DarkGreen && label14.BackColor == Color.DarkGreen && dataGridView1.Rows.Count > 0;
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

        private void comboBox2_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;
            using (var brush = new SolidBrush(e.ForeColor))
            {
                e.DrawBackground();
                e.Graphics.DrawString(((KeyValuePair<string, string>)comboBox2.Items[e.Index]).Value.ToString(), e.Font, brush, e.Bounds);
                e.DrawFocusRectangle();
            }
        }

        private void OrderForm_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            switch (button1.Text)
            {
                case "Провести":
                    {
                        order["OrderWorkerId"] = comboBox2.SelectedValue.ToString();
                        order["OrderDate"] = dateTimePicker1.Value.Date.ToString("yyyy-MM-dd");
                        order["OrderStatus"] = "Проведён";
                        bool confirmed;
                        try
                        {
                            confirmed = Connection.ConfirmOrder(order);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        if (confirmed)
                        {
                            button1.Text = "Сформировать чек";
                            dataGridView1.ContextMenuStrip = null;
                            comboBox2.Enabled = false;
                            dateTimePicker1.Enabled = false;
                        }
                        else
                        {
                            order["OrderStatus"] = "Новый";
                        }
                        break;
                    }
                case "Сформировать чек":
                    {
                        //make report
                        ReportBuilder.OrderItemsReport(order["OrderId"], dateTimePicker1.Value.Date, ((KeyValuePair<string, string>)comboBox2.SelectedItem).Value);
                        break;
                    }
            }
            button1.Enabled = true;
        }

        private void добавитьТоварыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = new ListForm("product", order["OrderId"]);
            this.Visible = false;
            f.ShowDialog();
            this.Visible = true;
            FillDGV();
        }

        private void удалитьТоварToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (order["OrderStatus"] == "Проведён")
            {
                return;
            }
            if (dataGridView1.SelectedRows.Count < 1)
            {
                return;
            }
            if (MessageBox.Show("Удалить запись?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) != DialogResult.OK)
            {
                return;
            }
            var rowIndex = dataGridView1.SelectedRows[0].Index;
            Connection.DeleteOrderItem(dataGridView1["OrderItemOrderId", rowIndex].Value.ToString(), dataGridView1["OrderItemProductId", rowIndex].Value.ToString());
            FillDGV();
        }

        private void OrderForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (order["OrderStatus"] == "Проведён")
            {
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}
