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
    }
}
