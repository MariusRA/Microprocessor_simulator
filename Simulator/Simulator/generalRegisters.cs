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
    public partial class generalRegisters : Form
    {
        public generalRegisters()
        {
            InitializeComponent();
            CenterToScreen();
        }

        private void generalRegisters_Load(object sender, EventArgs e)
        {

        }


        public void updateRegisisterText(List<ushort> regs)
        {
            Reg1.Text = regs[0].ToString();
            Reg2.Text = regs[1].ToString();
            Reg3.Text = regs[2].ToString();
            Reg4.Text = regs[3].ToString();
            Reg5.Text = regs[4].ToString();
            Reg6.Text = regs[5].ToString();
            Reg7.Text = regs[6].ToString();
            Reg8.Text = regs[7].ToString();
            Reg9.Text = regs[8].ToString();
            Reg10.Text = regs[9].ToString();
            Reg11.Text = regs[10].ToString();
            Reg12.Text = regs[11].ToString();
            Reg13.Text = regs[12].ToString();
            Reg14.Text = regs[13].ToString();
            Reg15.Text = regs[14].ToString();
            Reg16.Text = regs[15].ToString();
        }
    }
}
