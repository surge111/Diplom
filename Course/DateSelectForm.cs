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
    public partial class DateSelectForm : Form
    {
        public DateSelectForm()
        {
            InitializeComponent();
            dateTimePicker1.MaxDate = DateTime.Now.Date;
            dateTimePicker2.MaxDate = DateTime.Now.Date;
            dateTimePicker2.Value = DateTime.Now.Date;
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePicker1.Value.Date > dateTimePicker2.Value.Date)
                dateTimePicker1.Value = dateTimePicker2.Value.Date;
            dateTimePicker1.MaxDate = dateTimePicker2.Value.Date;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePicker2.Value.Date < dateTimePicker1.Value.Date)
                dateTimePicker2.Value = dateTimePicker1.Value.Date;
            dateTimePicker2.MinDate = dateTimePicker1.Value.Date;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
