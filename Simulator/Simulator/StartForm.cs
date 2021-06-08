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
   
    public partial class StartForm : Form
    {
        public static string binaryFile;
        public StartForm()
        {
            InitializeComponent();
            Assembler assembler = new Assembler();
            
        }

        private void btnLoadFile_Click(object sender, EventArgs e)
        {
           Assembler.filePath= Assembler.loadFile();
        }

        private void btnCompile_Click(object sender, EventArgs e)
        {
            if (Assembler.filePath != null)
            {
                Assembler.assemble();
            }
            else
            {
                MessageBox.Show("No file selected!");
            }
            binaryFile = Assembler.filePath.Replace(".asm", ".bin");
            Form1 sim = new Form1();
            sim.Show();
        }

        private void StartForm_Load(object sender, EventArgs e)
        {

        }
    }
}
