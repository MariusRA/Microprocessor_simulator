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
                openFileDialog.InitialDirectory = @"F:\";
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

        public static List<List<String>> parseFile(RichTextBox rtb, string filePath)
        {
            //start parsing the.asm file
            int lineCounter = 0;

            
            List <List<String>> asmMatrix = new List<List<String>>();
         

            TextFieldParser parser = new TextFieldParser(filePath);

            rtb.Text = "";

            String[] delimiters = { ":", ",", " ", "(", ")" };

            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(delimiters);

            while (!parser.EndOfData)
            {
                string[] asmFields = parser.ReadFields();
                List<String> asmRow = new List<String>();

                foreach (string s in asmFields)
                {
                    if (!s.Equals(""))
                    {
                        asmRow.Add(s);
                    }
                }
                asmMatrix.Add(asmRow);
                //Counting the number of lines stored in ASM file 
                lineCounter++;
            }

            parser.Close();

            for(int i = 0; i < asmMatrix.Count(); i++)
            {
                for(int j=0; j < asmMatrix[i].Count(); j++)
                {
                    rtb.Text += asmMatrix[i][j] + " ";
                }
                
                rtb.Text += Environment.NewLine;
            }
          
            return asmMatrix;
        }

         
       
    }
}
