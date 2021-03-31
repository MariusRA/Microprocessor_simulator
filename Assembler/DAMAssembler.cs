using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.VisualBasic.FileIO;


namespace Assembler
{
    public partial class assemblerForm : Form
    {
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
            string filePath = Utility.loadFile();
            List<List<String>> asmMatrix = Utility.parseFile(outputTB, filePath);
            List<List<String>> binaryMatrix = new List<List<string>>();

            List<int> immValuesI = new List<int>();
            List<string> immValuesV = new List<string>();

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
                string instructionName = asmMatrix[i][0].ToUpper();
                if (instructions.ContainsKey(instructionName))
                {
                    binaryMatrix[i][0] = instructions[instructionName];

                    switch (asmMatrix[i].Count())
                    {
                        case 3: // if the instruction has 2 operands
                            var firstOperand = asmMatrix[i][1].ToUpper();
                            binaryMatrix[i].Insert(1, Utility.getAdressingMode(firstOperand));
                            if (asmMatrix[i][1].Contains("R")) // if the first operand is a register
                            {
                                if (asmMatrix[i][1].Contains("("))
                                {
                                    if (asmMatrix[i][1][0].Equals('(') && asmMatrix[i][1][asmMatrix[i][1].Length - 1].Equals(')'))//indirect
                                    {
                                        char[] delimiters = { '(', ')', 'R' };
                                        var reg = asmMatrix[i][1].Split(delimiters);
                                        if (!Utility.checkCorrectRegisterNumber(reg[2],i))
                                        {
                                            binaryMatrix[i][2] = System.Convert.ToString(Convert.ToInt32(reg[2]), 2);
                                            while (binaryMatrix[i][2].Length < 4) // 'binary' value must be represented on 4 bits
                                            {
                                                binaryMatrix[i][2] = "0" + binaryMatrix[i][2];
                                            }
                                        }                                             
                                    }
                                    else //indexat
                                    {
                                        char[] delimiters = { '(', ')', 'R' };
                                        var reg = asmMatrix[i][1].Split(delimiters);//reg[2] we have the register NO.

                                        if (!Utility.checkCorrectRegisterNumber(reg[2], i))
                                        {
                                            binaryMatrix[i][2] = System.Convert.ToString(Convert.ToInt32(reg[2]), 2);
                                            while (binaryMatrix[i][2].Length < 4) // 'binary' value must be represented on 4 bits
                                            {
                                                binaryMatrix[i][2] = "0" + binaryMatrix[i][2];
                                            }
                                            string con = "";
                                            if (reg[0] != "") //const value in front of (
                                            {
                                                con = System.Convert.ToString(Convert.ToInt32(reg[0]), 2);
                                            }
                                            else //const value after )
                                            {
                                                con = System.Convert.ToString(Convert.ToInt32(reg[reg.Length - 1]), 2);
                                            }
                                            while (con.Length < 16) // 'binary' value must be represented on 4 bits
                                            {
                                                con = "0" + con;
                                            }
                                            immValuesI.Add(i + 1);
                                            immValuesV.Add(con);
                                        }
                                       
                                    }
                                }
                                else //direct
                                {
                                    var reg = asmMatrix[i][1].Split('R');
                                    if (!Utility.checkCorrectRegisterNumber(reg[1], i))
                                    {
                                        binaryMatrix[i][2] = System.Convert.ToString(Convert.ToInt32(reg[1]), 2);
                                        while (binaryMatrix[i][2].Length < 4) // 'binary' value must be represented on 4 bits
                                        {
                                            binaryMatrix[i][2] = "0" + binaryMatrix[i][2];
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //error: first operand must not be a imm value
                                string errText = "First operand must not be a imm value (line: " + (i + 1) + " )";
                                MessageBox.Show(errText, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                            var secondOperand = asmMatrix[i][2].ToUpper();
                            binaryMatrix[i].Insert(3, Utility.getAdressingMode(secondOperand));
                            if (asmMatrix[i][2].Contains("R")) // if the first operand is a register
                            {
                                if (asmMatrix[i][2].Contains("("))
                                {
                                    if (asmMatrix[i][2][0].Equals('(') && asmMatrix[i][2][asmMatrix[i][2].Length - 1].Equals(')'))//indirect
                                    {
                                        char[] delimiters = { '(', ')', 'R' };
                                        var reg = asmMatrix[i][2].Split(delimiters);
                                        if (!Utility.checkCorrectRegisterNumber(reg[2], i))
                                        {
                                            binaryMatrix[i][4] = System.Convert.ToString(Convert.ToInt32(reg[2]), 2);
                                            while (binaryMatrix[i][4].Length < 4) // 'binary' value must be represented on 4 bits
                                            {
                                                binaryMatrix[i][4] = "0" + binaryMatrix[i][4];
                                            }
                                        }
                                    }
                                    else //indexat
                                    {
                                        char[] delimiters = { '(', ')', 'R' };
                                        var reg = asmMatrix[i][2].Split(delimiters);//reg[2] we have the register NO.

                                        if (!Utility.checkCorrectRegisterNumber(reg[2], i))
                                        {
                                            binaryMatrix[i][4] = System.Convert.ToString(Convert.ToInt32(reg[2]), 2);
                                            while (binaryMatrix[i][4].Length < 4) // 'binary' value must be represented on 4 bits
                                            {
                                                binaryMatrix[i][4] = "0" + binaryMatrix[i][4];
                                            }
                                            string con = "";
                                            if (reg[0] != "") //const value in front of (
                                            {
                                                con = System.Convert.ToString(Convert.ToInt32(reg[0]), 2);
                                            }
                                            else //const value after )
                                            {
                                                con = System.Convert.ToString(Convert.ToInt32(reg[reg.Length - 1]), 2);
                                            }
                                            while (con.Length < 16) // 'binary' value must be represented on 4 bits
                                            {
                                                con = "0" + con;
                                            }
                                            immValuesI.Add(i + 1);
                                            immValuesV.Add(con);
                                        }
                                    }
                                }
                                else //direct
                                {
                                    var reg = asmMatrix[i][2].Split('R');

                                    if (!Utility.checkCorrectRegisterNumber(reg[1], i))
                                    {
                                        binaryMatrix[i][4] = System.Convert.ToString(Convert.ToInt32(reg[1]), 2);
                                        while (binaryMatrix[i][4].Length < 4) // 'binary' value must be represented on 4 bits
                                        {
                                            binaryMatrix[i][4] = "0" + binaryMatrix[i][4];
                                        }
                                    }                     
                                }
                            }
                            else
                            {
                                //second operand can be a constant value
                                binaryMatrix[i][4] = "0000"; //those bits can have any value since the adressing mode is an immediate one

                                string con = asmMatrix[i][2];
                                con = System.Convert.ToString(Convert.ToInt32(asmMatrix[i][2]), 2);
                                while (con.Length < 16) // 'binary' value must be represented on 4 bits
                                {
                                    con = "0" + con;
                                }

                                immValuesI.Add(i + 1);
                                immValuesV.Add(con);
                            }
                            break;

                        case 2:
                            while (binaryMatrix[i][0].Length < 16)
                            {
                                binaryMatrix[i][0] = binaryMatrix[i][0] + "0";
                            }
                            break;
                        case 1:
                            break;
                    }

                }

            }
            //put the constant values at their places
            Utility.putConstantValues(binaryMatrix, immValuesV, immValuesI);
            //display the contents of our 2 matrices
            Utility.displayASMFileContents(asmMatrix, outputTB);
            Utility.displayCodifiedInstructions(binaryMatrix, outputTB);

            List<string> linesToWrite = Utility.matrixToList(binaryMatrix);

            Utility.binaryFileWriter(linesToWrite, filePath);

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

    }
}
