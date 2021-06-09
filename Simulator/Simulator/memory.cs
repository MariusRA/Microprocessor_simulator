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
    public partial class memory : Form
    {
        public memory()
        {
            InitializeComponent();
        }

        public  void displayMemory(byte[] memory)
        {
            memDGV.Rows.Clear();
            for (int i = 0; i < 65000; i++)
            {
                if (memory[i] != 0)
                {
                    DataGridViewRow row = (DataGridViewRow)memDGV.Rows[0].Clone();
                    row.Cells[0].Value = i;
                    row.Cells[1].Value = memory[i];
                    memDGV.Rows.Add(row);
                }
                
            }
           
        }
    }
}
