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

        List<TextBox> registers;

        public Form1()
        {
            simulator = new Simulator();
            registers = new List<TextBox>();
            InitializeComponent();
            CenterToScreen();

           foreach(var tb in this.Controls)
            {
                if(tb is TextBox)
                {
                    ((TextBox)tb).Text = "0";
                }
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click_1(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void RegPC_TextChanged(object sender, EventArgs e)
        {

        }

        private void RegMDR_TextChanged(object sender, EventArgs e)
        {

        }

        private void RegFlag_TextChanged(object sender, EventArgs e)
        {

        }

        private void label21_Click(object sender, EventArgs e)
        {

        }

        private void RegSP_TextChanged(object sender, EventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void RegIVR_TextChanged(object sender, EventArgs e)
        {

        }

        private void RegIR_TextChanged(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void RegT_TextChanged(object sender, EventArgs e)
        {

        }

        private void label22_Click(object sender, EventArgs e)
        {

        }

        private void RegADR_TextChanged(object sender, EventArgs e)
        {

        }

        private void RegDec_TextChanged(object sender, EventArgs e)
        {

        }

        private void label20_Click(object sender, EventArgs e)
        {

        }

        private void label23_Click(object sender, EventArgs e)
        {

        }



        //private void loadfileBTN_Click(object sender, EventArgs e)
        //{
        //    Assembler.filePath = Assembler.loadFile();
        //    List<List<string>> asmmatrix = Assembler.parseFile(Assembler.filePath);

        //    Assembler.displayContentsOnTextbox(asmmatrix, textBox1);

        //    assembleBTN.Enabled = true;
        //}

        //private void assembleBTN_Click(object sender, EventArgs e)
        //{
        //    Assembler.assemble();
        //}

        //private void textBox1_TextChanged(object sender, EventArgs e)
        //{

        //}

    }
}
