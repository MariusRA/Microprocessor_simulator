using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator
{
    class Simulator
    {
        public List<ushort> registers;
        public byte FLAG;
        public ushort PC, DBUS, T, IVR, ADR, MDR, IR, RBUS, SBUS, ALU;
        public ushort SP = 65535;

        public byte[] memoryLocations = new byte[1 << 16];

        // current instruction details
        public ushort instrClass, opcode, addressingModeSource, sourceReg, addressingModeDestination, destReg;
        public short offset;

        // phases
        public bool TINT, TIF, TOF, TEX;
        public int currentPhase = 1, currentImpulse = 1;

        public string binaryFilePath;

        public Simulator()
        {
            registers = new List<ushort>();
            for (short i = 0; i < 16; i++)
            {
                registers.Add(0);
            }
           
            loadFileToMemory(StartForm.binaryFile);

            for (int i = 0; i < memoryLocations.Length; i += 2)
            {
                var x = memoryLocations[i];
                memoryLocations[i] = memoryLocations[i + 1];
                memoryLocations[i + 1] = x;
            }
        }

        public void decodeInstruction(ushort instruction)
        {
            if (instruction >> 15 == 0) // check the instruction class 0 = > 2 operand instruction
            {
                instrClass = 0;

                opcode = (ushort)(instruction >> 12);
                addressingModeDestination = (ushort)((instruction >> 10) & 3);
                destReg = (ushort)((instruction >> 6) & 15);
                addressingModeSource = (ushort)((instruction >> 4) & 3);
                sourceReg = (ushort)(instruction & 15);
            }

        }

        public void loadFileToMemory(string filePath)
        {
            var fileContent = File.ReadAllBytes(filePath);
            int i = 0;
            foreach (var octet in fileContent)
            {
                memoryLocations[i] = octet;
                i++;
            }
        }

        public void phaseGen()
        {

            if (TINT == true && TIF == false && TOF == false && TEX == false)
            {
                currentPhase = 4;
                currentImpulse = 1;
                TINT = false;
            }
            else if (TIF == true && TOF == false && TEX == false && TINT == false)
            {
                currentPhase = 1;
                currentImpulse = 1;
                TIF = false;

            }
            else if (TOF == true && TEX == false && TINT == false && TIF == false)
            {
                currentPhase = 2;
                currentImpulse = 1;
                TOF = false;
            }
            else if (TEX == true && TIF == false && TOF == false && TINT == false)
            {
                currentPhase = 3;
                currentImpulse = 1;
                TEX = false;
            }

        }

        public void impulseGen()
        {
            switch (currentPhase)
            {
                case (1): // instruction fetch

                    // PC->ADR in order to access memory
                    if (currentImpulse.Equals(1))
                    {
                        DBUS = PC;
                        ALU = DBUS;
                        RBUS = ALU;
                        ADR = RBUS;
                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(2))
                    {
                        // create instruction from the first 2 bytes at the memory location
                        byte firstByte = memoryLocations[PC];
                        byte secondByte = memoryLocations[PC + 1];
                        ushort instruction = (ushort)(firstByte << 8);
                        instruction = (ushort)(instruction | (ushort)(secondByte));
                        IR = instruction;
                        decodeInstruction(IR);
                        PC = (ushort)(PC + 2);
                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(3))
                    {
                        if (instrClass.Equals(0)) // 2 operand instr => we need operand fetch
                        {

                            TOF = true;

                        }

                        phaseGen();
                    }

                    break;

                case (2): // operand fetch


                    switch (addressingModeDestination)
                    {
                        case (1):
                            operandFetchDirect();
                            break;
                        case (2):
                            operandFetchIndirect();
                            break;
                        case (3):
                            operandFetchIndexed();
                            break;
                    }

                    break;

                case (3): // execution
                    switch (opcode)
                    {
                        case 0:
                            executionMOV();
                            break;
                        case 1:
                            executionADD();
                            break;
                        case 2:
                            executionSUB();
                            break;
                        case 3:
                            executionCMP();
                            break;
                        case 4:
                            executionAND();
                            break;
                        case 5:
                            executionOR();
                            break;
                        case 6:
                            executionXOR();
                            break;

                        default:
                            if (currentImpulse.Equals(1))
                            {
                                TIF = true;
                                phaseGen();
                            }
                            break;
                    }
                    break;



            }

        }

        private void setFLAGS(ushort RBUS, ushort DBUS, ushort SBUS)
        {

            if (RBUS == 0)
            {
                FLAG = (byte)(FLAG | (1 << 2));
            }
            else
            {
                FLAG = (byte)(FLAG & ~(1 << 2));
            }

            if (RBUS < DBUS)
            {
                FLAG = (byte)(FLAG | (1 << 3));
            }
            else
            {
                FLAG = (byte)(FLAG & ~(1 << 3));
            }

            if (RBUS >> 15 == 1)
            {
                FLAG = (byte)(FLAG | 1);
            }
            else
            {
                FLAG = (byte)(FLAG & ~1);
            }

            if (SBUS + DBUS > 65535)
            {
                FLAG = (byte)(FLAG | (1 << 1));
            }
            else
            {
                FLAG = (byte)(FLAG & ~(1 << 1));
            }

        }
        public void operandFetchDirect()
        {
            switch (addressingModeSource)
            {
                case 0: // immediate operand
                    if (currentImpulse.Equals(1))
                    {
                        // PC->ADR in order to access the memory location from which to take the immediate value
                        DBUS = PC;
                        ALU = DBUS;
                        RBUS = ALU;
                        ADR = RBUS;

                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(2))
                    {
                        byte firstByte = memoryLocations[ADR];
                        byte secondByte = memoryLocations[ADR + 1];
                        ushort immValue = (ushort)(firstByte << 8);
                        immValue = (ushort)(immValue | (ushort)(secondByte));
                        RBUS = immValue;
                        T = RBUS;
                        PC = (ushort)(PC + 2);

                        TEX = true;
                        phaseGen();
                    }
                    break;


                case 1: // direct operand => register => decoded in the decodeInstr method
                    if (currentImpulse.Equals(1))
                    {
                        // selecct the register and move it to the T reg
                        DBUS = registers[sourceReg];
                        ALU = DBUS;
                        RBUS = ALU;
                        T = RBUS;

                        TEX = true;
                        phaseGen();
                    }
                    break;

                case 2: // indirect operand => we have to access the memory based on the address found in the register
                    if (currentImpulse.Equals(1))
                    {
                        //select the register and move it to ADR in order to acc the memory
                        DBUS = registers[sourceReg];
                        ALU = DBUS;
                        RBUS = ALU;
                        ADR = RBUS;

                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(2))
                    {
                        RBUS = memoryLocations[25000 + ADR];
                        T = RBUS;

                        TEX = true;
                        phaseGen();
                    }
                    break;


                case 3: // indexed operand
                    if (currentImpulse.Equals(1))
                    {
                        DBUS = registers[sourceReg];
                        ALU = DBUS;
                        RBUS = ALU;
                        ADR = RBUS;

                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(2))
                    {
                        byte firstByte = memoryLocations[ADR];
                        byte secondByte = memoryLocations[ADR + 1];
                        ushort index = (ushort)(firstByte << 8);
                        index = (ushort)(index | (ushort)(secondByte));
                        RBUS = index;
                        T = RBUS;
                        PC = (ushort)(PC + 2);


                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(3))
                    {
                        DBUS = T;
                        SBUS = registers[sourceReg];
                        ALU = (ushort)(DBUS + SBUS);
                        RBUS = ALU;
                        ADR = RBUS;

                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(4))
                    {
                        RBUS = memoryLocations[25000 + ADR];
                        T = RBUS;

                        TEX = true;
                        phaseGen();
                    }

                    break;
            }
        }

        public void operandFetchIndirect()
        {
            switch (addressingModeSource)
            {
                case 0: // immediate operand
                    if (currentImpulse.Equals(1))
                    {
                        SBUS = registers[destReg];
                        ALU = SBUS;
                        RBUS = ALU;
                        MDR = RBUS;

                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(2))
                    {
                        DBUS = PC;
                        ALU = DBUS;
                        RBUS = ALU;
                        ADR = RBUS;

                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(3))
                    {
                        byte firstByte = memoryLocations[ADR];
                        byte secondByte = memoryLocations[ADR + 1];
                        ushort immValue = (ushort)(firstByte << 8);
                        immValue = (ushort)(immValue | (ushort)(secondByte));

                        RBUS = immValue;
                        T = RBUS;
                        PC = (ushort)(PC + 2);

                        TEX = true;
                        phaseGen();
                    }
                    break;
                case 1:
                    if (currentImpulse.Equals(1))
                    {
                        SBUS = registers[destReg];
                        ALU = SBUS;
                        RBUS = ALU;
                        MDR = RBUS;

                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(2))
                    {
                        DBUS = registers[sourceReg];
                        ALU = DBUS;
                        RBUS = ALU;
                        T = RBUS;

                        TEX = true;
                        phaseGen();
                    }
                    break;

                case 2:
                    if (currentImpulse.Equals(1))
                    {
                        SBUS = registers[destReg];
                        ALU = SBUS;
                        RBUS = ALU;
                        MDR = RBUS;

                        currentImpulse++;

                    }
                    else if (currentImpulse.Equals(2))
                    {
                        DBUS = registers[sourceReg];
                        ALU = DBUS;
                        RBUS = ALU;
                        ADR = RBUS;

                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(3))
                    {
                        RBUS = memoryLocations[25000 + ADR];
                        T = RBUS;

                        TEX = true;
                        phaseGen();
                    }
                    break;

                case 3:
                    if (currentImpulse.Equals(1))
                    {
                        SBUS = registers[destReg];
                        ALU = SBUS;
                        RBUS = ALU;
                        MDR = RBUS;

                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(2))
                    {
                        DBUS = PC;
                        ALU = DBUS;
                        RBUS = ALU;
                        ADR = RBUS;

                        currentImpulse++;
                    }

                    else if (currentImpulse.Equals(3))
                    {
                        byte firstByte = memoryLocations[ADR];
                        byte secondByte = memoryLocations[ADR + 1];
                        ushort index = (ushort)(firstByte << 8);
                        index = (ushort)(index | (ushort)(secondByte));

                        RBUS = index;
                        T = RBUS;
                        PC = (ushort)(PC + 2);

                        currentImpulse++;

                    }
                    else if (currentImpulse.Equals(4))
                    {
                        DBUS = T;
                        SBUS = registers[sourceReg];
                        ALU = (ushort)(DBUS + SBUS);
                        RBUS = ALU;
                        ADR = RBUS;

                        currentImpulse++;

                    }
                    else if (currentImpulse.Equals(5))
                    {
                        RBUS = memoryLocations[25000 + ADR];
                        T = RBUS;

                        TEX = true;
                        phaseGen();
                    }
                    break;
            }
        }
        public void operandFetchIndexed()
        {
            switch (addressingModeSource)
            {
                case 0: // immediate operand
                    if (currentImpulse.Equals(1))
                    {
                        DBUS = PC;
                        ALU = DBUS;
                        RBUS = ALU;
                        ADR = RBUS;

                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(2))
                    {
                        byte firstByte = memoryLocations[ADR];
                        byte secondByte = memoryLocations[ADR + 1];
                        ushort index = (ushort)(firstByte << 8);
                        index = (ushort)(index | (ushort)(secondByte));

                        RBUS = index;
                        T = RBUS;
                        PC = (ushort)(PC + 2);

                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(3))
                    {
                        DBUS = T;
                        SBUS = registers[destReg];
                        ALU = (ushort)(SBUS + DBUS);
                        RBUS = ALU;
                        MDR = RBUS;

                        currentImpulse++;

                    }

                    else if (currentImpulse.Equals(4))
                    {
                        DBUS = PC;
                        ALU = DBUS;
                        RBUS = ALU;
                        ADR = RBUS;

                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(5))
                    {
                        byte firstByte = memoryLocations[ADR];
                        byte secondByte = memoryLocations[ADR + 1];
                        ushort immValue = (ushort)(firstByte << 8);
                        immValue = (ushort)(immValue | (ushort)(secondByte));

                        RBUS = immValue;
                        T = RBUS;
                        PC = (ushort)(PC + 2); 

                        TEX = true;
                        phaseGen();

                    }
                    break;
                case 1:
                    if (currentImpulse.Equals(1))
                    {
                        DBUS = PC;
                        ALU = DBUS;
                        RBUS = ALU;
                        ADR = RBUS;

                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(2))
                    {
                        byte firstByte = memoryLocations[ADR];
                        byte secondByte = memoryLocations[ADR + 1];
                        ushort index = (ushort)(firstByte << 8);
                        index = (ushort)(index | (ushort)(secondByte));

                        RBUS = index;
                        T = RBUS;
                        PC = (ushort)(PC + 2);

                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(3))
                    {
                        DBUS = T;
                        SBUS = registers[destReg];
                        ALU = (ushort)(SBUS + RBUS);
                        RBUS = ALU;
                        MDR = RBUS;

                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(4))
                    {
                        DBUS =  registers[sourceReg];
                        ALU = DBUS;
                        RBUS = ALU;
                        T = RBUS;

                        TEX = true;
                        phaseGen();
                    }
                    break;

                case 2:
                    if (currentImpulse.Equals(1))
                    {
                        DBUS = PC;
                        ALU = DBUS;
                        RBUS = ALU;
                        ADR = RBUS;

                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(2))
                    {
                        byte firstByte = memoryLocations[ADR];
                        byte secondByte = memoryLocations[ADR + 1];
                        ushort index = (ushort)(firstByte << 8);
                        index = (ushort)(index | (ushort)(secondByte));

                        RBUS = index;
                        T = RBUS;
                        PC = (ushort)(PC + 2);

                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(3))
                    {
                        DBUS = T;
                        SBUS = registers[destReg];
                        ALU = (ushort)(SBUS + RBUS);
                        RBUS = ALU;
                        MDR = RBUS;

                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(4))
                    {
                        DBUS = registers[sourceReg];
                        ALU = DBUS;
                        RBUS = ALU;
                        ADR = RBUS;

                       
                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(5))
                    {
                        RBUS = memoryLocations[25000 + ADR];
                        T = RBUS;
                        

                        TEX = true;
                        phaseGen();
                    }
                    break;

                case 3:
                    if (currentImpulse.Equals(1))
                    {
                        DBUS = PC;
                        ALU = DBUS;
                        RBUS = ALU;
                        ADR = RBUS;

                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(2))
                    {
                        byte firstByte = memoryLocations[ADR];
                        byte secondByte = memoryLocations[ADR + 1];
                        ushort index = (ushort)(firstByte << 8);
                        index = (ushort)(index | (ushort)(secondByte));

                        RBUS = index;
                        T = RBUS;
                        PC = (ushort)(PC + 2);

                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(3))
                    {
                        DBUS = T;
                        SBUS = registers[sourceReg];
                        ALU = (ushort)(SBUS + RBUS);
                        RBUS = ALU;
                        ADR = RBUS;

                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(4))
                    {
                        RBUS = memoryLocations[25000 + ADR];
                        T = RBUS;

                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(5))
                    {
                        DBUS = PC;
                        ALU = DBUS;
                        RBUS = ALU;
                        ADR = RBUS;

                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(6))
                    {
                        byte firstByte = memoryLocations[ADR];
                        byte secondByte = memoryLocations[ADR + 1];
                        ushort index = (ushort)(firstByte << 8);
                        index = (ushort)(index | (ushort)(secondByte));

                        RBUS = index;
                        T = RBUS;
                        PC = (ushort)(PC + 2);

                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(7))
                    {
                        DBUS = T;
                        SBUS = registers[destReg];
                        ALU = (ushort)(SBUS + RBUS);
                        RBUS = ALU;
                        MDR = RBUS;

                        TEX=true;

                        phaseGen();
                    }
                    break;

            }
        }

        public void executionMOV()
        {
            switch (addressingModeDestination)
            {
                case 1:
                    if (currentImpulse.Equals(1))
                    {
                        DBUS = T;
                        ALU = DBUS;
                        RBUS = ALU;
                        registers[destReg] = RBUS;

                        TIF = true;
                        phaseGen();
                    }
                    break;
                case 2:
                    if (currentImpulse.Equals(1))
                    {
                        DBUS = T;
                        ALU = DBUS;
                        RBUS = ALU;
                        memoryLocations[25000 + MDR] = (byte)(RBUS >> 8);
                        memoryLocations[25000 + MDR + 1] = (byte)(RBUS);

                        TIF = true;
                        phaseGen();
                    }
                    break;
                case 3:
                    if (currentImpulse.Equals(1))
                    {
                        DBUS = T;
                        ALU = DBUS;
                        RBUS = ALU;
                        memoryLocations[25000 + MDR] = (byte)(RBUS >> 8);
                        memoryLocations[25000 + MDR + 1] = (byte)(RBUS);

                        TIF = true;
                        phaseGen();
                    }
                    break;
            }
        }

        public void executionADD()
        {
            switch (addressingModeDestination)
            {
                case 1:
                    if (currentImpulse == 1)
                    {
                        DBUS = registers[destReg];
                        SBUS = T;
                        ALU = (ushort)(DBUS + SBUS);
                        RBUS = ALU;
                        registers[destReg] = RBUS;
                        setFLAGS(RBUS, DBUS, SBUS);

                        TIF = true;
                        phaseGen();
                    }
                    break;
                case 2:
                    if (currentImpulse == 1)
                    {
                        DBUS = MDR; 
                        ALU = DBUS; 
                        RBUS = ALU; 
                        ADR = RBUS;

                        currentImpulse = 2;
                    }
                    else if (currentImpulse == 2)
                    {
                        RBUS = (ushort)((ushort)((ushort)memoryLocations[ADR] << 8) | (ushort)memoryLocations[ADR + 1]); 
                        IVR = RBUS;
                        
                        currentImpulse = 3;
                    }
                    else if (currentImpulse == 3)
                    {
                        DBUS = IVR;
                        SBUS = T; 
                        ALU = (ushort)(DBUS + SBUS); 
                        RBUS = ALU; 

                        setFLAGS(RBUS, DBUS, SBUS);

                        memoryLocations[25000 + MDR] = (byte)(RBUS >> 8); 
                        memoryLocations[25000 + MDR + 1] = (byte)(RBUS);

                        TIF = true;
                        phaseGen();
                            
                    }
                    break;

                case 3:
                    if (currentImpulse == 1)
                    {
                        DBUS = MDR; 
                        ALU = DBUS; 
                        RBUS = ALU; 
                        ADR = RBUS; 

                        currentImpulse = 2;
                    }
                    else if (currentImpulse == 2)
                    {
                        RBUS = (ushort)((ushort)((ushort)memoryLocations[ADR] << 8) | (ushort)memoryLocations[ADR + 1]); 
                        IVR = RBUS; 
                 
                        currentImpulse = 3;
                    }
                    else if (currentImpulse == 3)
                    {
                        DBUS = IVR;
                        SBUS = T; 
                        ALU = (ushort)(DBUS + SBUS); 
                        RBUS = ALU;

                        setFLAGS(RBUS, DBUS, SBUS);

                        memoryLocations[25000 + MDR] = (byte)(RBUS >> 8); 
                        memoryLocations[25000 + MDR + 1] = (byte)(RBUS);

                        TIF = true;
                        phaseGen();
                    }
                    break;


            }
        }

        public void executionSUB()
        {
            switch (addressingModeDestination)
            {
                case 1:
                    if (currentImpulse == 1)
                    {
                        DBUS = registers[destReg]; 
                        SBUS = T; 
                        ALU = (ushort)(DBUS - SBUS); 
                        RBUS = ALU; 
                        registers[destReg] = RBUS; 

                        setFLAGS(RBUS, DBUS, SBUS);

                        if (RBUS > DBUS)
                        {
                            FLAG = (byte)(FLAG | (1 << 3));
                        }
                        else
                        {
                            FLAG = (byte)(FLAG & ~(1 << 3));
                        }

                        TIF = true;
                        phaseGen();
                    }
                    break;

                case 2:
                    if (currentImpulse == 1)
                    {
                        DBUS = MDR; 
                        ALU = DBUS; 
                        RBUS = ALU; 
                        ADR = RBUS; 

                        currentImpulse = 2;
                    }
                    else if (currentImpulse == 2)
                    {
                        RBUS = (ushort)((ushort)((ushort)memoryLocations[ADR] << 8) | (ushort)memoryLocations[ADR + 1]); 
                        IVR = RBUS; 

                        currentImpulse = 3;
                    }
                    else if (currentImpulse == 3)
                    {
                        DBUS = IVR; 
                        SBUS = T; 
                        ALU = (ushort)(DBUS - SBUS); 
                        RBUS = ALU;

                        setFLAGS(RBUS, DBUS, SBUS);

                        if (RBUS > DBUS)
                        {
                            FLAG = (byte)(FLAG | (1 << 3));
                        }
                        else
                        {
                            FLAG = (byte)(FLAG & ~(1 << 3));
                        }

                        memoryLocations[25000 + MDR] = (byte)(RBUS >> 8); 
                        memoryLocations[25000 + MDR + 1] = (byte)(RBUS);

                        TIF = true;
                        phaseGen();
                    }
                    break;

                case 3:
                    if (currentImpulse == 1)
                    {
                        DBUS = MDR; 
                        ALU = DBUS; 
                        RBUS = ALU; 
                        ADR = RBUS; 

                        currentImpulse = 2;
                    }
                    else if (currentImpulse == 2)
                    {
                        RBUS = (ushort)((ushort)((ushort)memoryLocations[ADR] << 8) | (ushort)memoryLocations[ADR + 1]); 
                        IVR = RBUS; 

                        currentImpulse = 3;
                    }
                    else if (currentImpulse == 3)
                    {
                        DBUS = IVR; 
                        SBUS = T; 
                        ALU = (ushort)(DBUS + SBUS); 
                        RBUS = ALU;

                        setFLAGS(RBUS, DBUS, SBUS);

                        if (RBUS > DBUS)
                        {
                            FLAG = (byte)(FLAG | (1 << 3));
                        }
                        else
                        {
                            FLAG = (byte)(FLAG & ~(1 << 3));
                        }

                        memoryLocations[25000 + MDR] = (byte)(RBUS >> 8); 
                        memoryLocations[25000 + MDR + 1] = (byte)(RBUS);

                        TIF = true;
                        phaseGen();
                    }

                    break;
            }
        }

        public void executionAND()
        {
            switch (addressingModeDestination)
            {
                case 1:
                    if (currentImpulse.Equals(1))
                    {
                        DBUS = registers[destReg];
                        SBUS = T;
                        ALU = (ushort)(DBUS & SBUS);
                        RBUS = ALU;
                        registers[destReg] = RBUS;
                        setFLAGS(RBUS, DBUS, SBUS);
                        TIF = true;
                        phaseGen();
                    }
                    break;

                case 2:
                    if (currentImpulse.Equals(1))
                    {
                        DBUS = MDR;
                        ALU = DBUS;
                        RBUS = ALU;
                        ADR = RBUS;

                        currentImpulse++;
                    }

                    else if (currentImpulse.Equals(2))
                    {

                        RBUS = (ushort)((ushort)((ushort)memoryLocations[ADR] << 8) | (ushort)memoryLocations[ADR + 1]);
                        IVR = RBUS;
                        currentImpulse++;

                    }
                    else if (currentImpulse.Equals(3))
                    {
                        DBUS = IVR;
                        SBUS = T;
                        ALU = (ushort)(DBUS & SBUS);
                        RBUS = ALU;
                        setFLAGS(RBUS, DBUS, SBUS);

                        memoryLocations[25000 + MDR] = (byte)(RBUS >> 8);
                        memoryLocations[25000 + MDR + 1] = (byte)(RBUS);

                        TIF = true;

                        phaseGen();
                    }
                    break;
                case 3:
                    if (currentImpulse.Equals(1))
                    {
                        DBUS = MDR;
                        ALU = DBUS;
                        RBUS = ALU;
                        ADR = RBUS;

                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(2))
                    {
                        RBUS = (ushort)((ushort)((ushort)memoryLocations[ADR] << 8) | (ushort)memoryLocations[ADR + 1]);
                        IVR = RBUS;

                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(3))
                    {
                        DBUS = IVR;
                        SBUS = T;
                        ALU = (ushort)(DBUS & SBUS);
                        RBUS = ALU;

                        setFLAGS(RBUS, DBUS, SBUS);

                        memoryLocations[25000 + MDR] = (byte)(RBUS >> 8);
                        memoryLocations[25000 + MDR + 1] = (byte)(RBUS);

                        TIF = true;
                        phaseGen();
                    }
                    break;
            }
        }

        public void executionOR()
        {
            switch (addressingModeDestination)
            {
                case 1:
                    if (currentImpulse.Equals(1))
                    {
                        DBUS = registers[destReg];
                        SBUS = T;
                        ALU = (ushort)(DBUS | SBUS);
                        RBUS = ALU;
                        registers[destReg] = RBUS;
                        setFLAGS(RBUS, DBUS, SBUS);
                        TIF = true;
                        phaseGen();
                    }
                    break;

                case 2:
                    if (currentImpulse.Equals(1))
                    {
                        DBUS = MDR;
                        ALU = DBUS;
                        RBUS = ALU;
                        ADR = RBUS;

                        currentImpulse++;
                    }

                    else if (currentImpulse.Equals(2))
                    {

                        RBUS = (ushort)((ushort)((ushort)memoryLocations[ADR] << 8) | (ushort)memoryLocations[ADR + 1]);
                        IVR = RBUS;
                        currentImpulse++;

                    }
                    else if (currentImpulse.Equals(3))
                    {
                        DBUS = IVR;
                        SBUS = T;
                        ALU = (ushort)(DBUS | SBUS);
                        RBUS = ALU;
                        setFLAGS(RBUS, DBUS, SBUS);

                        memoryLocations[25000 + MDR] = (byte)(RBUS >> 8);
                        memoryLocations[25000 + MDR + 1] = (byte)(RBUS);

                        TIF = true;

                        phaseGen();
                    }
                    break;
                case 3:
                    if (currentImpulse.Equals(1))
                    {
                        DBUS = MDR;
                        ALU = DBUS;
                        RBUS = ALU;
                        ADR = RBUS;

                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(2))
                    {
                        RBUS = (ushort)((ushort)((ushort)memoryLocations[ADR] << 8) | (ushort)memoryLocations[ADR + 1]);
                        IVR = RBUS;

                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(3))
                    {
                        DBUS = IVR;
                        SBUS = T;
                        ALU = (ushort)(DBUS | SBUS);
                        RBUS = ALU;

                        setFLAGS(RBUS, DBUS, SBUS);

                        memoryLocations[25000 + MDR] = (byte)(RBUS >> 8);
                        memoryLocations[25000 + MDR + 1] = (byte)(RBUS);

                        TIF = true;
                        phaseGen();
                    }
                    break;
            }
        }

        public void executionXOR()
        {
            switch (addressingModeDestination)
            {
                case 1:
                    if (currentImpulse.Equals(1))
                    {
                        DBUS = registers[destReg];
                        SBUS = T;
                        ALU = (ushort)(DBUS ^ SBUS);
                        RBUS = ALU;
                        registers[destReg] = RBUS;
                        setFLAGS(RBUS, DBUS, SBUS);
                        TIF = true;
                        phaseGen();
                    }
                    break;

                case 2:
                    if (currentImpulse.Equals(1))
                    {
                        DBUS = MDR;
                        ALU = DBUS;
                        RBUS = ALU;
                        ADR = RBUS;

                        currentImpulse++;
                    }

                    else if (currentImpulse.Equals(2))
                    {

                        RBUS = (ushort)((ushort)((ushort)memoryLocations[ADR] << 8) | (ushort)memoryLocations[ADR + 1]);
                        IVR = RBUS;
                        currentImpulse++;

                    }
                    else if (currentImpulse.Equals(3))
                    {
                        DBUS = IVR;
                        SBUS = T;
                        ALU = (ushort)(DBUS ^ SBUS);
                        RBUS = ALU;
                        setFLAGS(RBUS, DBUS, SBUS);

                        memoryLocations[25000 + MDR] = (byte)(RBUS >> 8);
                        memoryLocations[25000 + MDR + 1] = (byte)(RBUS);

                        TIF = true;

                        phaseGen();
                    }
                    break;
                case 3:
                    if (currentImpulse.Equals(1))
                    {
                        DBUS = MDR;
                        ALU = DBUS;
                        RBUS = ALU;
                        ADR = RBUS;

                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(2))
                    {
                        RBUS = (ushort)((ushort)((ushort)memoryLocations[ADR] << 8) | (ushort)memoryLocations[ADR + 1]);
                        IVR = RBUS;

                        currentImpulse++;
                    }
                    else if (currentImpulse.Equals(3))
                    {
                        DBUS = IVR;
                        SBUS = T;
                        ALU = (ushort)(DBUS ^ SBUS);
                        RBUS = ALU;

                        setFLAGS(RBUS, DBUS, SBUS);

                        memoryLocations[25000 + MDR] = (byte)(RBUS >> 8);
                        memoryLocations[25000 + MDR + 1] = (byte)(RBUS);

                        TIF = true;
                        phaseGen();
                    }
                    break;
            }
        }
        public void executionCMP()
        {

            switch (addressingModeDestination)
            {
                case 1:
                    if (currentImpulse == 1)
                    {
                        DBUS = registers[destReg]; 
                        SBUS = T; 
                        ALU = (ushort)(DBUS - SBUS); 
                        RBUS = ALU;

                        setFLAGS(RBUS, DBUS, SBUS);

                        TIF = true;
                        phaseGen();
                    }
                    break;

                case 2:
                    if (currentImpulse == 1)
                    {
                        DBUS = MDR; 
                        ALU = DBUS; 
                        RBUS = ALU; 
                        ADR = RBUS; 

                        currentImpulse = 2;
                    }
                    else if (currentImpulse == 2)
                    {
                        RBUS = (ushort)((ushort)((ushort)memoryLocations[ADR] << 8) | (ushort)memoryLocations[ADR + 1]); 
                        IVR = RBUS;

                        currentImpulse = 3;
                    }
                    else if (currentImpulse == 3)
                    {
                        DBUS = IVR; 
                        SBUS = T; 
                        ALU = (ushort)(DBUS - SBUS); 
                        RBUS = ALU;

                        setFLAGS(RBUS, DBUS, SBUS);

                        TIF = true;
                        phaseGen();
                    }
                    break;

                case 3:
                    if (currentImpulse == 1)
                    {
                        DBUS = MDR; 
                        ALU = DBUS; 
                        RBUS = ALU; 
                        ADR = RBUS; 

                        currentImpulse = 2;
                    }
                    else if (currentImpulse == 2)
                    {
                        RBUS = (ushort)((ushort)((ushort)memoryLocations[ADR] << 8) | (ushort)memoryLocations[ADR + 1]);
                        IVR = RBUS; 

                        currentImpulse = 3;
                    }
                    else if (currentImpulse == 3)
                    {
                        DBUS = IVR; 
                        SBUS = T; 
                        ALU = (ushort)(DBUS ^ SBUS); 
                        RBUS = ALU;

                        setFLAGS(RBUS, DBUS, SBUS);

                        TIF = true;

                        phaseGen();
                    }

                    break;
            }
        }

    }
}

