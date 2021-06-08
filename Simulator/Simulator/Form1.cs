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
        
        Simulator simulator;

        public Form1()
        {
            simulator = new Simulator();

            InitializeComponent();
            CenterToScreen();

            foreach (var tb in this.Controls)
            {
                if (tb is TextBox)
                {
                    ((TextBox)tb).Text = "0";
                }
            }

        }

        private void updateRegContents()
        {

            Reg1.Text = simulator.registers[0].ToString();
            Reg2.Text = simulator.registers[1].ToString();
            Reg3.Text = simulator.registers[2].ToString();
            Reg4.Text = simulator.registers[3].ToString();
            Reg5.Text = simulator.registers[4].ToString();
            Reg6.Text = simulator.registers[5].ToString();
            Reg7.Text = simulator.registers[6].ToString();
            Reg8.Text = simulator.registers[7].ToString();
            Reg9.Text = simulator.registers[8].ToString();
            Reg10.Text = simulator.registers[9].ToString();
            Reg11.Text = simulator.registers[10].ToString();
            Reg12.Text = simulator.registers[11].ToString();
            Reg13.Text = simulator.registers[12].ToString();
            Reg14.Text = simulator.registers[13].ToString();
            Reg15.Text = simulator.registers[14].ToString();
            Reg16.Text = simulator.registers[15].ToString();

            RegFlag.Text = simulator.FLAG.ToString();
            RegMDR.Text = simulator.MDR.ToString();
            RegIVR.Text = simulator.IVR.ToString();
            RegPC.Text = simulator.PC.ToString();
            RegSP.Text = simulator.SP.ToString();
            RegT.Text = simulator.T.ToString();
            RegIR.Text = simulator.IR.ToString();
            RegADR.Text = simulator.ADR.ToString();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            simulator.impulseGen();
            updateRegContents();
        }

    }
}
