using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DelayDemoForms
{
    public partial class Form2 : Form
    {
        int cikissifre = 112233;
        public Form2()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (cikissifre == Convert.ToInt32(textBox1.Text))
                
            Application.Exit();
           else
            {
                MessageBox.Show("Şifre Yanlış");
            }
        }
    }
}
