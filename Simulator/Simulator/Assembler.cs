using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simulator
{
    class Assembler
    {
        public static bool hasApplicationRestarted = false;
        public static Dictionary<String, String> instructions = new Dictionary<String, String>()
            {
                {"MOV","0000"},
                {"ADD","0001"},
                {"SUB","0010"},
                {"CMP","0011"},
                {"AND","0100"},
                {"OR","0101"},
                {"XOR","0110"},
                {"CLR","1000000000"},
                {"NEG","1000000001"},
                {"INC","1000000010"},
                {"DEC","1000000011"},
                {"ASL","1000000100"},
                {"ASR","1000000101"},
                {"LSR","1000000110"},
                {"RLC","1000000111"},
                {"JMP","1000001000"},
                {"CALL","1000001001"},
                {"PUSH","1000001010"},
                {"POP","1000001011"},
                {"BR","10100000"},
                {"BNE","10100001"},
                {"BEQ","10100010"},
                {"BPL","10100011"},
                {"BMI","10100100"},
                {"BCS","10100101"},
                {"BCC","10100110"},
                {"BVS","10100111"},
                {"BVC","10101000"},
                {"CLC","1110000000000000"},
                {"CLV","1110000000000001"},
                {"CLZ","1110000000000010"},
                {"CLS","1110000000000011"},
                {"CCC","1110000000000100"},
                {"SEC","1110000000000101"},
                {"SEV","1110000000000110"},
                {"SEZ","1110000000000111"},
                {"SES","1110000000001000"},
                {"SCC","1110000000001001"},
                {"NOP","1110000000001010"},
                {"RET","1110000000001011"},
                {"RETI","1110000000001100"},
                {"HALT","1110000000001101"},
                {"WAIT","1110000000001110"},
                {"PUSHPC","1110000000001111"},
                {"POPPC","1110000000010000"},
                {"PUSHFLAG","1110000000010001"},
                {"POPFLAG","1110000000010010"}
            };
        public static string filePath;
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

        public static List<List<String>> parseFile(string filePath)
        {

            List<List<String>> asmMatrix = new List<List<String>>();

            TextFieldParser parser = new TextFieldParser(filePath);

            String[] delimiters = { ",", " " };

            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(delimiters);

            while (!parser.EndOfData)
            {
                string[] asmFields = parser.ReadFields();
                List<String> asmRow = new List<String>();

                foreach (string s in asmFields)
                {
                    if (s.Contains("$$"))
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

        public static void displayContentsOnTextbox(List<List<String>> matrix, TextBox outT)
        {
            for (int i = 0; i < matrix.Count(); i++)
            {
                for (int j = 0; j < matrix[i].Count(); j++)
                {
                    outT.Text += matrix[i][j] + " ";
                }
                outT.Text += Environment.NewLine;
            }
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
                string temp = v[0];
                for (int i = 1; i < v.Count(); i++)
                {
                    v[0] += v[i];
                }
                mToL.Add(v[0]);
                v[0] = temp;
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

        public static List<List<string>> findTags(List<List<string>> matrix)
        {
            //first column=tag name, second column=where we find it, rest of columns=where we use it
            List<List<string>> tagsMatrix = new List<List<string>>();

            for (int k = 0; k < matrix.Count(); k++)
            {
                {
                    for (int i = 0; i < matrix[k].Count(); i++)
                    {
                        if (matrix[k][i].Contains(":"))
                        {
                            var isTag = matrix[k][i].Split(':');
                            List<string> t = new List<string>();
                            t.Add(isTag[0]);
                            t.Add((k * 2).ToString());
                            tagsMatrix.Add(t);
                            matrix.RemoveAt(k);
                        }
                    }
                }
            }
            return tagsMatrix;

        }

        public static void assemble()
        {
            List<List<String>> asmMatrix = Assembler.parseFile(filePath);
            List<List<String>> binaryMatrix = new List<List<string>>();

            List<List<string>> tagsMatrix = new List<List<string>>();

            List<int> immValuesI = new List<int>();
            List<string> immValuesV = new List<string>();

            for (int j = 0; j < asmMatrix.Count(); j++)
            {
                for (int i = 0; i < asmMatrix[j].Count(); i++)
                {
                    asmMatrix[j][i] = asmMatrix[j][i].ToUpper();
                }
            }

            foreach (var line in asmMatrix)
            {
                List<String> row = new List<string>();

                foreach (var s in line)
                {
                    string temp = null;
                    temp = String.Copy(s);
                    row.Add(temp);
                }
                binaryMatrix.Add(row);
            }

            for (int i = 0; i < asmMatrix.Count; i++)
            {
                string instructionName = asmMatrix[i][0];
                if (instructions.ContainsKey(instructionName))
                {
                    binaryMatrix[i][0] = instructions[instructionName];

                    switch (asmMatrix[i].Count())
                    {
                        case 3:
                            // if the instruction has 2 operands
                            var firstOperand = asmMatrix[i][1];

                            //we get the address mode of the operand
                            binaryMatrix[i].Insert(1, getAdressingMode(firstOperand));

                            switch (binaryMatrix[i][1])
                            {
                                case "10":
                                    //indirect
                                    char[] delimiters = { '(', ')', 'R' };
                                    var reg = asmMatrix[i][1].Split(delimiters);
                                    if (!checkCorrectRegisterNumber(reg[2], i))
                                    {
                                        binaryMatrix[i][2] = System.Convert.ToString(Convert.ToInt32(reg[2]), 2);
                                        if (binaryMatrix[i][2].Length < 4)
                                        {
                                            // 'binary' value must be represented on 4 bits
                                            binaryMatrix[i][2] = binaryMatrix[i][2].PadLeft(4, '0');
                                        }
                                    }
                                    break;

                                case "11":
                                    //indexed
                                    char[] delimiters1 = { '(', ')', 'R' };
                                    var reg1 = asmMatrix[i][1].Split(delimiters1);

                                    //reg[2] we have the register NO.
                                    if (!checkCorrectRegisterNumber(reg1[2], i))
                                    {
                                        binaryMatrix[i][2] = System.Convert.ToString(Convert.ToInt32(reg1[2]), 2);

                                        // 'binary' value must be represented on 4 bits
                                        if (binaryMatrix[i][2].Length < 4)
                                        {
                                            binaryMatrix[i][2] = binaryMatrix[i][2].PadLeft(4, '0');
                                        }
                                        string con = "";
                                        if (reg1[0] != "")
                                        {
                                            //const value in front of (
                                            con = System.Convert.ToString(Convert.ToInt32(reg1[0]), 2);
                                        }
                                        else
                                        {
                                            //const value after )
                                            con = System.Convert.ToString(Convert.ToInt32(reg1[reg1.Length - 1]), 2);
                                        }
                                        if (con.Length < 16)
                                        {
                                            // constant value must be represented on 16 bits
                                            con = con.PadLeft(16, '0');
                                        }
                                        immValuesI.Add(i + 1);
                                        immValuesV.Add(con);

                                    }

                                    break;

                                case "00":
                                    //error: first operand must not be a imm value
                                    string errText = "First operand must not be a imm value (line: " + (i + 1) + " )";
                                    DialogResult result = MessageBox.Show(errText, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                    if (result == DialogResult.OK)
                                    {
                                        Application.Restart();
                                        hasApplicationRestarted = true;

                                    }
                                    break;

                                case "01":
                                    //direct
                                    var reg2 = asmMatrix[i][1].Split('R');
                                    if (!checkCorrectRegisterNumber(reg2[1], i))
                                    {
                                        binaryMatrix[i][2] = System.Convert.ToString(Convert.ToInt32(reg2[1]), 2);
                                        if (binaryMatrix[i][2].Length < 4)
                                        {
                                            // 'binary' value must be represented on 4 bits
                                            binaryMatrix[i][2] = binaryMatrix[i][2].PadLeft(4, '0');
                                        }
                                    }
                                    break;

                            }

                            var secondOperand = asmMatrix[i][2];

                            //we get the address mode of the operand
                            binaryMatrix[i].Insert(3, getAdressingMode(secondOperand));

                            switch (binaryMatrix[i][3])
                            {
                                case "10":
                                    //indirect
                                    char[] delimiters = { '(', ')', 'R' };
                                    var reg = secondOperand.Split(delimiters);
                                    if (!checkCorrectRegisterNumber(reg[2], i))
                                    {
                                        binaryMatrix[i][4] = System.Convert.ToString(Convert.ToInt32(reg[2]), 2);
                                        if (binaryMatrix[i][4].Length < 4)
                                        {
                                            // 'binary' value must be represented on 4 bits
                                            binaryMatrix[i][4] = binaryMatrix[i][4].PadLeft(4, '0');
                                        }
                                    }
                                    break;

                                case "11":
                                    //indexed
                                    char[] delimiters1 = { '(', ')', 'R' };
                                    var reg1 = secondOperand.Split(delimiters1);

                                    //reg[2] we have the register NO.
                                    if (!checkCorrectRegisterNumber(reg1[2], i))
                                    {
                                        binaryMatrix[i][4] = System.Convert.ToString(Convert.ToInt32(reg1[2]), 2);

                                        // 'binary' value must be represented on 4 bits
                                        if (binaryMatrix[i][4].Length < 4)
                                        {
                                            binaryMatrix[i][4] = binaryMatrix[i][4].PadLeft(4, '0');
                                        }
                                        string con1 = "";
                                        if (reg1[0] != "")
                                        {
                                            //const value in front of (
                                            con1 = System.Convert.ToString(Convert.ToInt32(reg1[0]), 2);
                                        }
                                        else
                                        {
                                            //const value after )
                                            con1 = System.Convert.ToString(Convert.ToInt32(reg1[reg1.Length - 1]), 2);
                                        }
                                        if (con1.Length < 16)
                                        {
                                            // constant value must be represented on 16 bits
                                            con1 = con1.PadLeft(16, '0');
                                        }
                                        immValuesI.Add(i + 1);
                                        immValuesV.Add(con1);

                                    }

                                    break;

                                case "01":
                                    //direct
                                    var reg2 = secondOperand.Split('R');
                                    if (!checkCorrectRegisterNumber(reg2[1], i))
                                    {
                                        binaryMatrix[i][4] = System.Convert.ToString(Convert.ToInt32(reg2[1]), 2);
                                        if (binaryMatrix[i][4].Length < 4)
                                        {
                                            binaryMatrix[i][4] = binaryMatrix[i][4].PadLeft(4, '0');
                                        }
                                    }
                                    break;

                                case "00":
                                    //second operand can be a constant value
                                    //those bits can have any value since the adressing mode is an immediate one
                                    binaryMatrix[i][4] = "0000";

                                    string con = asmMatrix[i][2];
                                    con = System.Convert.ToString(Convert.ToInt32(asmMatrix[i][2]), 2);
                                    if (con.Length < 16)
                                    {
                                        // constant value must be represented on 16 bits
                                        con = con.PadLeft(16, '0');
                                    }

                                    immValuesI.Add(i + 1);
                                    immValuesV.Add(con);
                                    break;

                            }
                            break;


                        case 2:

                            if (binaryMatrix[i][0].Length == 10) //instructions that do not permit a imm value as operand
                            {
                                if (asmMatrix[i][0] != "JMP" && asmMatrix[i][0] != "CALL")
                                {
                                    string operand = asmMatrix[i][1];
                                    binaryMatrix[i].Insert(1, getAdressingMode(operand));

                                    switch (binaryMatrix[i][1])
                                    {
                                        case "10":
                                            //indirect
                                            char[] delimiters = { '(', ')', 'R' };
                                            var reg = asmMatrix[i][1].Split(delimiters);
                                            if (!checkCorrectRegisterNumber(reg[2], i))
                                            {
                                                binaryMatrix[i][2] = System.Convert.ToString(Convert.ToInt32(reg[2]), 2);
                                                if (binaryMatrix[i][2].Length < 4)
                                                {
                                                    // 'binary' value must be represented on 4 bits
                                                    binaryMatrix[i][2] = binaryMatrix[i][2].PadLeft(4, '0');
                                                }
                                            }

                                            break;

                                        case "11":

                                            //indexed
                                            string errText = "Operand's addressing mode must not be indexed (line: " + (i + 1) + " )";
                                            DialogResult result = MessageBox.Show(errText, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                            if (result == DialogResult.OK)
                                            {
                                                Application.Restart();
                                                hasApplicationRestarted = true;
                                            }
                                            break;

                                        case "01":
                                            //direct
                                            var reg1 = asmMatrix[i][1].Split('R');
                                            if (!checkCorrectRegisterNumber(reg1[1], i))
                                            {
                                                binaryMatrix[i][2] = System.Convert.ToString(Convert.ToInt32(reg1[1]), 2);
                                                if (binaryMatrix[i][2].Length < 4)
                                                {
                                                    // 'binary' value must be represented on 4 bits
                                                    binaryMatrix[i][2] = binaryMatrix[i][2].PadLeft(4, '0');
                                                }
                                            }
                                            break;

                                        case "00":
                                            //error: operand must not be a imm value
                                            string errText1 = "Operand must not be a imm value (line: " + (i + 1) + " )";
                                            DialogResult result1 = MessageBox.Show(errText1, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                            if (result1 == DialogResult.OK)
                                            {
                                                Application.Restart();
                                                hasApplicationRestarted = true;
                                            }
                                            break;

                                    }

                                }
                                else
                                {
                                    //the instruction is a CALL or a JMP
                                    if (asmMatrix[i][0] == "JMP")
                                    {
                                        string operand = asmMatrix[i][1];
                                        if (operand.Contains("("))
                                        {
                                            if (operand[0].Equals('(') && operand[operand.Length - 1].Equals(')'))
                                            {
                                                //indirect
                                                binaryMatrix[i].Insert(1, "10");

                                                char[] delimiters = { '(', ')', 'R' };
                                                var reg = operand.Split(delimiters);
                                                if (!checkCorrectRegisterNumber(reg[2], i))
                                                {
                                                    binaryMatrix[i][2] = System.Convert.ToString(Convert.ToInt32(reg[2]), 2);
                                                    if (binaryMatrix[i][2].Length < 4)
                                                    {
                                                        // 'binary' value must be represented on 4 bits
                                                        binaryMatrix[i][2] = binaryMatrix[i][2].PadLeft(4, '0');
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //indexed
                                                binaryMatrix[i].Insert(1, "11");

                                                char[] delimiters = { '(', ')', 'R' };
                                                var reg = operand.Split(delimiters);

                                                //reg[2] we have the register NO.
                                                if (!checkCorrectRegisterNumber(reg[2], i))
                                                {
                                                    binaryMatrix[i][2] = System.Convert.ToString(Convert.ToInt32(reg[2]), 2);
                                                    if (binaryMatrix[i][2].Length < 4)
                                                    {
                                                        // 'binary' value must be represented on 4 bits
                                                        binaryMatrix[i][2] = binaryMatrix[i][2].PadLeft(4, '0');
                                                    }
                                                    string con = "";
                                                    if (reg[0] != "")
                                                    {
                                                        //const value in front of (
                                                        con = System.Convert.ToString(Convert.ToInt32(reg[0]), 2);
                                                    }
                                                    else
                                                    {
                                                        //const value after )
                                                        con = System.Convert.ToString(Convert.ToInt32(reg[reg.Length - 1]), 2);
                                                    }
                                                    if (con.Length < 16)
                                                    {
                                                        // constant value must be represented on 16 bits
                                                        con = con.PadLeft(16, '0');
                                                    }
                                                    immValuesI.Add(i + 1);
                                                    immValuesV.Add(con);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //tags
                                            immValuesI.Add(i + 1);
                                            immValuesV.Add("constant value");
                                        }


                                        if (asmMatrix[i][0] == "CALL") ;

                                    }


                                }
                            }

                            break;
                        case 1:
                            break;
                    }

                }

            }

            putConstantValues(binaryMatrix, immValuesV, immValuesI);
            tagsMatrix = findTags(binaryMatrix);

            //here we handle tags for branches and jumps

            for (int i = 0; i < binaryMatrix.Count(); i++)
            {
                if (binaryMatrix[i][0].Length > 8 && binaryMatrix[i][0].Length != 16)
                {

                    for (int j = 0; j < tagsMatrix.Count(); j++)
                    {
                        if (binaryMatrix[i][1] == tagsMatrix[j][0])
                        {
                            binaryMatrix[i][1] = "000000";
                            List<string> x = new List<string>();
                            x.Add(System.Convert.ToString(Convert.ToInt32(tagsMatrix[j][1]), 2).PadLeft(16, '0'));
                            binaryMatrix[i + 1] = x;
                        }
                    }
                }
                else if (binaryMatrix[i][0].Length == 8) //branches
                {
                    for (int k = 0; k < tagsMatrix.Count(); k++)
                    {
                        if (binaryMatrix[i][1] == tagsMatrix[k][0])
                        {
                            int currentPC = i * 2;
                            int lineOfTag = Int32.Parse(tagsMatrix[k][1]);
                            int codif = currentPC - lineOfTag;
                            string value = null;
                            if (codif >= 0)
                            {
                                value = System.Convert.ToString(codif, 2).PadLeft(8, '0');
                            }
                            else
                            {
                                value = System.Convert.ToString(codif, 2);
                                if (value.Length > 8)
                                {
                                    value = value.Substring(value.Length - 8, 8);
                                }
                            }

                            binaryMatrix[i][1] = value;

                        }
                        else //branch with imm value
                        {
                        };
                    }
                }
            }

            if (!hasApplicationRestarted)
            {

                //generate and write.bin file
                List<string> linesToWrite = matrixToList(binaryMatrix);
                binaryFileWriter(linesToWrite, filePath);
                MessageBox.Show("Assembly done!");
            }
            else
            {

            };
        }

    }
}
