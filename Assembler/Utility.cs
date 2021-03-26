using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Assembler
{
    class Utility
    {
       
        public static string loadFile()
        {
            //selecting the file we want to parse
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = @"C:\";
                openFileDialog.Filter = "Assembly files (*.asm)|*.asm|Text files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
                }
            }
            return filePath;
        }

        public static List<String> parseFile(RichTextBox rtb, string filePath)
        {
            //start parsing the.asm file
            int lineCounter = 0;

            List<String> asmElements = new List<String>();
            TextFieldParser parser = new TextFieldParser(filePath);

            rtb.Text = "";

            String[] delimiters = { ":", ",", " ", "(", ")" };

            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(delimiters);

            while (!parser.EndOfData)
            {
                string[] asmFields = parser.ReadFields();

                foreach (string s in asmFields)
                {
                    if (!s.Equals(""))
                    {
                        asmElements.Add(s);
                    }
                }
                //Counting the number of lines stored in ASM file 
                lineCounter++;
            }

            parser.Close();

            foreach (String s in asmElements)
            {
                rtb.Text += s + Environment.NewLine;
            }

            return asmElements;
        }

        //for future 
        public static Dictionary<String, String> instructionsOpcode()
        {
            
            Dictionary<String, String> instructions = new Dictionary<String, String>();

            FileInfo fileInfo = new FileInfo("instruction_opcodes.txt");

            return instructions;
        }
    }
}
