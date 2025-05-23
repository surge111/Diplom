﻿using System;
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
    public partial class MenuForm : Form
    {
        public MenuForm()
        {
            InitializeComponent();
            if (User.Role == "Администратор")
            {
                button1.Visible = false;
                button2.Visible = false;
                button3.Visible = false;
                button4.Visible = false;
                button8.Visible = false;
            }
            else if (User.Role == "Менеджер")
            {
                button2.Visible = false;
                button5.Visible = false;
                button6.Visible = false;
                button7.Visible = false;
                button8.Visible = false;
            }
            else if (User.Role == "Сотрудник")
            {
                button3.Visible = false;
                button4.Visible = false;
                button5.Visible = false;
                button6.Visible = false;
                button7.Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var f = new ListForm("product");
            this.Visible = false;
            f.ShowDialog();
            this.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var f = new ListForm("order");
            this.Visible = false;
            f.ShowDialog();
            this.Visible = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var f = new ListForm("category");
            this.Visible = false;
            f.ShowDialog();
            this.Visible = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var f = new ListForm("supplier");
            this.Visible = false;
            f.ShowDialog();
            this.Visible = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var f = new ListForm("worker");
            this.Visible = false;
            f.ShowDialog();
            this.Visible = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var f = new ListForm("user");
            this.Visible = false;
            f.ShowDialog();
            this.Visible = true;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var f = new RecoveryForm();
            this.Visible = false;
            f.ShowDialog();
            this.Visible = true;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            var f = new ListForm("client");
            this.Visible = false;
            f.ShowDialog();
            this.Visible = true;
        }
    }
}
