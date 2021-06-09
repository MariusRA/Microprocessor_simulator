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
        generalRegisters genRegs;
        memory mem;

        public Form1()
        {
            simulator = new Simulator();
            genRegs = new generalRegisters();
            mem = new memory();
            InitializeComponent();
            CenterToScreen();
            genRegPB.Image = Properties.Resources.genReg;
            memPB.Image = Properties.Resources.mem;
            aluPB.Image = Properties.Resources.alu;
            mux1PB.Image = Properties.Resources.mux;
            mux2PB.Image = Properties.Resources.mux;
            bgcPB.Image = Properties.Resources.bgc;

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
            genRegs.updateRegisisterText(simulator.registers);
            if (mem != null)
            {
                mem.displayMemory(simulator.memoryLocations);
            }
               
        }

        private void genRegPB_Click(object sender, EventArgs e)
        {
            genRegs.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void memPB_Click(object sender, EventArgs e)
        {
            mem.Show();
        }
    }
}
