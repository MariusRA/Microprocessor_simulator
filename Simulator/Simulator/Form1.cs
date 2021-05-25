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
            
        }

        private void loadfileBTN_Click(object sender, EventArgs e)
        {
            Assembler.filePath = Assembler.loadFile();
            List<List<string>> asmmatrix = Assembler.parseFile(Assembler.filePath);

            Assembler.displayContentsOnTextbox(asmmatrix, textBox1);

            assembleBTN.Enabled = true;
        }

        private void assembleBTN_Click(object sender, EventArgs e)
        {
            Assembler.assemble();
        }

    }
}
