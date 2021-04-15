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

        public static Dictionary<string, string> register = new Dictionary<string, string>()
        {
            {"R0", "0000" },
            {"R1", "0001" },
            {"R2", "0010" },
            {"R3", "0011" },
            {"R4", "0100" },
            {"R5", "0101" },
            {"R6", "0110" },
            {"R7", "0111" },
            {"R8", "1000" },
            {"R9", "1001" },
            {"R10", "1010" },
            {"R11", "1011" },
            {"R12", "1100" },
            {"R13", "1101" },
            {"R14", "1110" },
            {"R15", "1111" }
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

            //first column=tag name, second column=where we find it, third column=where we use it
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
                            binaryMatrix[i].Insert(1, Utility.getAdressingMode(firstOperand));

                            switch (binaryMatrix[i][1])
                            {
                                case "10":
                                    //indirect
                                    char[] delimiters = { '(', ')', 'R' };
                                    var reg = asmMatrix[i][1].Split(delimiters);
                                    if (!Utility.checkCorrectRegisterNumber(reg[2], i))
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
                                    if (!Utility.checkCorrectRegisterNumber(reg1[2], i))
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
                                    if (!Utility.checkCorrectRegisterNumber(reg2[1], i))
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
                            binaryMatrix[i].Insert(3, Utility.getAdressingMode(secondOperand));

                            switch (binaryMatrix[i][3])
                            {
                                case "10":
                                    //indirect
                                    char[] delimiters = { '(', ')', 'R' };
                                    var reg = secondOperand.Split(delimiters);
                                    if (!Utility.checkCorrectRegisterNumber(reg[2], i))
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
                                    if (!Utility.checkCorrectRegisterNumber(reg1[2], i))
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
                                    if (!Utility.checkCorrectRegisterNumber(reg2[1], i))
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
                                    binaryMatrix[i].Insert(1, Utility.getAdressingMode(operand));

                                    switch (binaryMatrix[i][1])
                                    {
                                        case "10":
                                            //indirect
                                            char[] delimiters = { '(', ')', 'R' };
                                            var reg = asmMatrix[i][1].Split(delimiters);
                                            if (!Utility.checkCorrectRegisterNumber(reg[2], i))
                                            {
                                                binaryMatrix[i][2] = System.Convert.ToString(Convert.ToInt32(reg[2]), 2);
                                                while (binaryMatrix[i][2].Length < 4)
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
                                            if (!Utility.checkCorrectRegisterNumber(reg1[1], i))
                                            {
                                                binaryMatrix[i][2] = System.Convert.ToString(Convert.ToInt32(reg1[1]), 2);
                                                while (binaryMatrix[i][2].Length < 4)
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
                                                if (!Utility.checkCorrectRegisterNumber(reg[2], i))
                                                {
                                                    binaryMatrix[i][2] = System.Convert.ToString(Convert.ToInt32(reg[2]), 2);
                                                    while (binaryMatrix[i][2].Length < 4)
                                                    {
                                                        // 'binary' value must be represented on 4 bits
                                                        binaryMatrix[i][2] = "0" + binaryMatrix[i][2];
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
                                                if (!Utility.checkCorrectRegisterNumber(reg[2], i))
                                                {
                                                    binaryMatrix[i][2] = System.Convert.ToString(Convert.ToInt32(reg[2]), 2);
                                                    while (binaryMatrix[i][2].Length < 4)
                                                    {
                                                        // 'binary' value must be represented on 4 bits
                                                        binaryMatrix[i][2] = "0" + binaryMatrix[i][2];
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
                                                    while (con.Length < 16)
                                                    {
                                                        // constant value must be represented on 16 bits
                                                        con = "0" + con;
                                                    }
                                                    immValuesI.Add(i + 1);
                                                    immValuesV.Add(con);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // tag
                                        }


                                        if (asmMatrix[i][0] == "CALL") ;

                                    }


                                }
                            }
                            else if (binaryMatrix[i][0].Length == 8)
                            {

                            }

                            break;
                        case 1:
                            break;
                    }

                }

            }

            //put the constant values at their places
            Utility.putConstantValues(binaryMatrix, immValuesV, immValuesI);

            tagsMatrix = Utility.findTags(binaryMatrix);
            Utility.displayContentsOnTextbox(asmMatrix, outputTB);
            outputTB.Text += Environment.NewLine;

            Utility.displayContentsOnTextbox(binaryMatrix, outputTB);
            outputTB.Text += Environment.NewLine;

            Utility.displayContentsOnTextbox(tagsMatrix, outputTB);
            outputTB.Text += Environment.NewLine;


            //if (!hasApplicationRestarted)
            //{

            //    //display the contents of our 2 matrices
            //    Utility.displayContentsOnTextbox(asmMatrix, outputTB);
            //    Utility.displayContentsOnTextbox(binaryMatrix, outputTB);

            //    //generate and write .bin file
            //    List<string> linesToWrite = Utility.matrixToList(binaryMatrix);
            //    // Utility.binaryFileWriter(linesToWrite, filePath);
            //}
            //else
            //{

            //};
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

    }
}
