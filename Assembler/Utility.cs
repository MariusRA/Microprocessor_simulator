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


            List<List<String>> asmMatrix = new List<List<String>>();


            TextFieldParser parser = new TextFieldParser(filePath);

            rtb.Text = "";

            String[] delimiters = { ":", ",", " " };

            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(delimiters);

            while (!parser.EndOfData)
            {
                string[] asmFields = parser.ReadFields();
                List<String> asmRow = new List<String>();

                foreach (string s in asmFields)
                {
                    if (s.Contains(";"))
                    {
                        break;
                    }
                    if (!s.Equals(""))
                    {
                        asmRow.Add(s);
                    }
                }
                if (asmRow.Count != 0)
                {
                    asmMatrix.Add(asmRow);
                }

                //Counting the number of lines stored in ASM file 
                lineCounter++;
            }

            parser.Close();
            return asmMatrix;
        }

        public static String getAdressingMode(string operand)
        {
            string result = "";

            if (operand.Contains("R"))
            {
                if (operand.Contains("("))
                {
                    if (operand[0].Equals('(') && operand[operand.Length - 1].Equals(')'))
                    {
                        result = "10";
                    }
                    else
                    {
                        result = "11";
                    }
                }
                else
                {
                    result = "01";
                }
            }
            else
            {
                result = "00";
            }


            return result;
        }

        public static void displayCodifiedInstructions(List<List<String>> binaryMatrix, RichTextBox outT)
        {
            for (int i = 0; i < binaryMatrix.Count(); i++)
            {
                for (int j = 0; j < binaryMatrix[i].Count(); j++)
                {
                    outT.Text += binaryMatrix[i][j] + " ";
                }
                outT.Text += Environment.NewLine;
            }
        }

        public static void displayASMFileContents(List<List<String>> matrix, RichTextBox outT)
        {
            for (int i = 0; i < matrix.Count(); i++)
            {
                for (int j = 0; j < matrix[i].Count(); j++)
                {
                    outT.Text += matrix[i][j] + " ";
                }
                outT.Text += Environment.NewLine;
            }
            outT.Text += Environment.NewLine;
        }

        public static void putConstantValues(List<List<String>> matrix, List<string> values, List<int> indexes)
        {
            for (int i = 0; i < indexes.Count(); i++)
            {
                List<string> val = new List<string>();
                val.Add(values[i]);
                matrix.Insert(indexes[i], val);
                for (int j = i + 1; j < indexes.Count(); j++)
                {
                    indexes[j]++;
                }
            }
        }

        public static bool checkCorrectRegisterNumber(string numberAsString, int line)
        {
            int number = Int32.Parse(numberAsString);
            if (number < 0 || number > 15)
            {
                string errText = "Register with number " + numberAsString + " does not exist (line: " + (line + 1) + " )";
                MessageBox.Show(errText, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        public static List<string> matrixToList(List<List<String>> matrix)
        {
            List<string> mToL = new List<string>();

            foreach (var v in matrix)
            {
                for (int i = 1; i < v.Count(); i++)
                {
                    v[0] += v[i];
                }
                mToL.Add(v[0]);
            }
            return mToL;
        }

        public static void binaryFileWriter(List<string> matrix, string fileName)
        {
            fileName = fileName.Replace(".asm", ".bin");
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
            {
                foreach (var line in matrix)
                {
                    short x;
                    x = System.Convert.ToInt16(line, 2);
                    writer.Write(x);                    
                }
            }
        }

    }
}
