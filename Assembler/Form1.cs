using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Assembler
{
    public partial class assemblerForm : Form
    {
        public assemblerForm()
        {
            InitializeComponent();
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void loadFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = @"C:\Users\popa_\Desktop\Proiect Microprocesor";
                openFileDialog.Filter = "Assembly files (*.asm)|*.asm|Text files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContent = reader.ReadToEnd();
                    }
                }

            }

            var fileContentByLines = fileContent.Split(new[] { " ","  ", "," },StringSplitOptions.None);

            foreach (String cuv in fileContentByLines)
            {
                if (cuv != "" && !cuv.Contains("\n"))
                {
                    outputTB.Text = outputTB.Text + cuv + "\n";
                }
                else if (cuv != "" && cuv.Contains("\n"))
                {
                    outputTB.Text = outputTB.Text + cuv;
                }


            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
