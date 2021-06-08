using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simulator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CenterToScreen();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int i = 0;
            while (i < 100)
            {
                dataGridView1.Rows.Add();

                // Grab the new row!
               dataGridView1.Rows[i].Cells["memoryLocation"].Value = i.ToString();
                dataGridView1.Rows[i].Cells["valueAtLocation"].Value = i.ToString();
                // Add the data
                
            }
        }


    }
}
