﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SavageEmpireSNESTransTool
{
    internal class ConversationLoader
    {
        private byte[] m_data;
        private uint[] m_conv_addresses_snes;
        private uint[] m_conv_addresses_pc;
        private uint[] m_conv_charset_addresses_snes;
        private uint[] m_conv_charset_addresses_pc;
        const int NUM_CONVERSATIONS = 125;
        const uint CONV_ADDRESS = 0xc0000;
        const uint CONV_CHARSET_ADDRESS = 0x118000;
        const uint BANK_SIZE = 0x8000;
        const uint DIALOG_OPTIONS = 0x147AE;
        //const uint DIALOG_OPTIONS = 0x147C6;
        uint m_waitForInputAddress = 0;
        const uint NAME_IN_OPTION_OFFSET = 0x1c4;

        public string m_CurrentConversation;

        private Dictionary<uint, string> m_knownFlags;
        private Dictionary<uint, string> m_knownBits;
        private List<string> m_charSet;
        private List<byte[]> m_optionSet;
        private List<string> m_dialogOptions;

        public ConversationLoader()
        {
            m_conv_addresses_snes = new uint[NUM_CONVERSATIONS];
            m_conv_addresses_pc = new uint[NUM_CONVERSATIONS];
            m_conv_charset_addresses_snes = new uint[NUM_CONVERSATIONS];
            m_conv_charset_addresses_pc = new uint[NUM_CONVERSATIONS];
            m_charSet = new List<string>();
            m_optionSet = new List<byte[]>();
            m_dialogOptions = new List<string>();

            m_knownFlags = new Dictionary<uint, string>();

            m_knownFlags.Add(0x10A9, "INTANYA_INTRO_DONE");

            m_knownBits = new Dictionary<uint, string>();
            m_knownBits.Add(0x07, "KURAK_SOMETHING");
            m_knownBits.Add(0x0B, "YOLARU_SOMETHING");
            m_knownBits.Add(0x11, "BARRAB_SOMETHING");
            m_knownBits.Add(0x15, "DISQUIQUI_SOMETHING");
            m_knownBits.Add(0x2F, "HAS_JIMMYS_NOTEBOOK_1");
            m_knownBits.Add(0x3F, "ALREADY_HAVE_PLACHTA");
            m_knownBits.Add(0x40, "INVENTORY_FULL");
            m_knownBits.Add(0x5B, "KNOWS_TRIOLO");
            m_knownBits.Add(0x81, "ASKED_DOKREI_ABOUT_UNION");
            m_knownBits.Add(0x85, "KNOWS_ALORON");
            m_knownBits.Add(0x86, "KNOWS_KURAK_NEED");
            m_knownBits.Add(0x87, "AIERA_SAVED_KURAK_JOINED_UNION");
            m_knownBits.Add(0x88, "KNOWS_APATON");
            m_knownBits.Add(0x8A, "KNOWS_YOLARU_NEED");
            m_knownBits.Add(0x8B, "YOLARU_JOINED_UNION");
            m_knownBits.Add(0x8D, "MOCTAPOTL_RESTORED_1???");
            m_knownBits.Add(0x90, "KNOWS_BARRAB_NEED");
            m_knownBits.Add(0x91, "BARRAB_JOINED_UNION");
            m_knownBits.Add(0x92, "KNOWS_CHAFBLUM");
            m_knownBits.Add(0x93, "CHAFBLUM_DRANK_PACHTA");
            m_knownBits.Add(0x94, "KNOWS_DISQUIQUI_NEED");
            m_knownBits.Add(0x95, "DISQUIQUI_JOINED_UNION");
            m_knownBits.Add(0x9A, "KNOWS_DOKRAY");
            m_knownBits.Add(0x9E, "KNOWS_FRITZ");
            m_knownBits.Add(0x9F, "FRITZ_TALKED_ABOUT_BRAIN");
            m_knownBits.Add(0xA0, "FRITZ_ALREADY_TOLD_LONG_STORY");
            m_knownBits.Add(0xA1, "TALKED_WITH_GRUGORR");
            m_knownBits.Add(0xA3, "HAAKUR_SHIELD_RETURNED");
            m_knownBits.Add(0xA4, "KNOWS_GUOBLUM");
            m_knownBits.Add(0xA7, "KNOWS_HALISA");
            m_knownBits.Add(0xA8, "GORILLA_DEAD");
            m_knownBits.Add(0xAA, "KNOWS_INARA");
            m_knownBits.Add(0xAB, "INARA_UNUSED?");
            m_knownBits.Add(0xAC, "INARA_EXPLAINED_ALREADY_WHY_PINDIRO_JOINS_UNION");
            m_knownBits.Add(0xAD, "INTANYA_CONVERSATION_RESET");
            m_knownBits.Add(0xAF, "HAS_JIMMYS_NOTEBOOK_2");
            m_knownBits.Add(0xB1, "KNOWS_ABOUT_TOPURU");
            m_knownBits.Add(0xB2, "HAS_JIMMYS_CAMERA");
            m_knownBits.Add(0xB3, "KNOWS_SANE_SPECTOR");
            m_knownBits.Add(0xB4, "SPECTOR_TALKED_PROBLEM");
            m_knownBits.Add(0xB7, "RETURNED_SACRED_HIDE");
            m_knownBits.Add(0xB9, "KOTL_DOOR_OPEN");
            m_knownBits.Add(0xBA, "KNOWS_KSSSINDRA");
            m_knownBits.Add(0xBB, "KNOWS_KUNAWO");
            m_knownBits.Add(0xBC, "ASKED_KUNAWO_ABOUT_PINDIRO");
            m_knownBits.Add(0xBF, "CAN_GET_PLACHTA");
            m_knownBits.Add(0xC1, "KNOWS_LEREI");
            m_knownBits.Add(0xC2, "KNOWS_MOCTAPOTL");
            m_knownBits.Add(0xC4, "MOCTAPOTL_RESTORED_2???");
            m_knownBits.Add(0xC5, "KNOWS_MOSAGAN");
            m_knownBits.Add(0xC6, "KNOWS_NAKAI"); // This is wrong, find out why.  It appears to be if you've talked with him, but was set elsewhere as well, though not sure where
            m_knownBits.Add(0xC7, "KNOWS_NAWL");
            m_knownBits.Add(0xCA, "KNOWS_PAXAPTAMAC");
            m_knownBits.Add(0xCC, "KNOWS_RAFKIN");
            m_knownBits.Add(0xCF, "KNOWS_SHAMURU");
            m_knownBits.Add(0xD0, "KNOWS_SPECTOR");
            m_knownBits.Add(0xD1, "KNOWS_SYSSKARR");
            m_knownBits.Add(0xD2, "SYSSKARR_TOLD_HOWTO_UNITE");
            m_knownBits.Add(0xD3, "KILLED_THUNDERER");
            m_knownBits.Add(0xD4, "KNOWS_TIAPATLA");
            m_knownBits.Add(0xD9, "TOPURU_MENTIONED_BLUE_STONE");
            m_knownBits.Add(0xDB, "KNOWS_TRIOLO");
            m_knownBits.Add(0xDC, "KNOWS_TRISTIA");
            m_knownBits.Add(0xE6, "DARDEN_KILLED");
            m_knownBits.Add(0xE7, "WAMAP_TELLS_ABOUT_FABOZZ_NOT_SAVED");
            m_knownBits.Add(0xE8, "FABOZZ_RETURNED");
            m_knownBits.Add(0xEB, "YUNAPOTLI_HAS_BRAIN");
        }

        void LoadOptions()
        {
            uint PC_Address = DIALOG_OPTIONS;

            byte option_length = m_data[PC_Address];

            while (option_length != 0x82)
            {
                if (PC_Address == 0x15b51) // エノコー Option 645/0x285
                {
                    int j = 9;
                }
                PC_Address++;
                m_optionSet.Add(new byte[option_length]);
                if(m_optionSet.Count == 0x4c)
                {
                    int j = 9;
                }
                Buffer.BlockCopy(m_data, (int)PC_Address, m_optionSet.Last(), 0, option_length);
                PC_Address += option_length;
                option_length = m_data[PC_Address];
            }
        }

        uint CalculateSNESAddress(uint PC_Address)
        {
            uint SNES_Address = PC_Address / 0x8000;
            SNES_Address *= 0x10000;
            SNES_Address += PC_Address % 0x8000;
            SNES_Address += 0x8000;
            SNES_Address |= 0x800000;

            return SNES_Address;
        }

        uint CalculatePCAddress(uint SNES_Address)
        {
            uint PC_Address = (SNES_Address - 0x8000) & 0x7FFFFF;
            PC_Address /= 0x10000;
            PC_Address *= BANK_SIZE;
            PC_Address += ((SNES_Address - 0x8000) % 0x10000);
            return PC_Address;
        }

        uint CalculateROMAddress(uint SNES_Address)
        {
            uint PC_Address = 0;
            return PC_Address;
        }

        public void LoadData(byte[] data)
        {
            m_data = data;
            LoadOptions();
        }

        public void PrepConversations(int id)
        {
            for(int index = 0; index < NUM_CONVERSATIONS; ++index)
            {
                m_conv_charset_addresses_snes[index] = ((uint)m_data[CONV_CHARSET_ADDRESS + (index * 4)]) +
                    (((uint)m_data[CONV_CHARSET_ADDRESS + (index * 4 + 1)]) << 8) +
                    (((uint)m_data[CONV_CHARSET_ADDRESS + (index * 4 + 2)]) << 16) +
                    (((uint)m_data[CONV_CHARSET_ADDRESS + (index * 4 + 3)]) << 24);
                m_conv_charset_addresses_pc[index] = CalculatePCAddress(m_conv_charset_addresses_snes[index]);

                m_conv_addresses_snes[index] = ((uint)m_data[CONV_ADDRESS + (index * 2)]) +
                    (((uint)m_data[CONV_ADDRESS + (index * 2) + 1]) << 8) +
                    ((((uint)m_data[CONV_ADDRESS + index + (2 * NUM_CONVERSATIONS)])) << 16);
                m_conv_addresses_pc[index] = CalculatePCAddress(m_conv_addresses_snes[index]);
            }
            if(id >= 0 && id < NUM_CONVERSATIONS)
            {
                m_CurrentConversation = "";
                LoadConversation(id);
            }
        }

        private uint LoadCharset(int id, out string strCharset)
        {
            strCharset = "-- CHARSET";
            uint address = m_conv_charset_addresses_pc[id];
            uint retval = 0;
            string strChars = "";

            byte[] outbyte = new byte[2];
            var japaneseEncoding = Encoding.GetEncoding("shift_jis");

            for (uint nOffset = 0; nOffset < 512; ++nOffset)
            {
                uint byte1 = m_data[address + (nOffset * 2)];
                uint byte2 = m_data[address + (nOffset * 2)+ 1];
                uint realval = (byte2 << 8) + byte1;
                if(realval == 0xFFFF)
                {
                    retval = nOffset * 2;
                    break;
                }

                if (realval % 3 == 0)
                {
                    realval /= 3;
                    realval += 0x8140;

                    outbyte[1] = (byte)(realval & 0xFF);
                    outbyte[0] = (byte)((realval >> 8) & 0xFF);
                    string curChar = japaneseEncoding.GetString(outbyte);
                    strChars += curChar;
                    m_charSet.Add(curChar);
                }
            }
            strCharset += "(" + (retval / 2) + "): " + strChars;
            strCharset += Environment.NewLine;
            return retval;
        }
        
        private uint GetWord(ref uint curAddress)
        {
            uint byte1 = m_data[curAddress];
            uint byte2 = m_data[curAddress + 1];
            uint realval = (byte2 << 8) + byte1;

            curAddress += 2;

            return realval;
        }

        private string GetGlobalAddress(ref uint curAddress)
        {
            string strRet = "";

            uint byte1 = m_data[curAddress];
            uint byte2 = m_data[curAddress + 1];
            uint byte3 = m_data[curAddress + 2];
            uint realval = (byte3 << 16) + (byte2 << 8) + byte1;
            uint realAddress = CalculatePCAddress(realval);
            strRet += realAddress.ToString("X");

            curAddress +=3;

            return strRet;
        }

        private string GetLocalAddress(ref uint curAddress)
        {
            string strRet = "";

            uint byte1 = m_data[curAddress];
            uint byte2 = m_data[curAddress + 1];
            uint realval = (byte2 << 8) + byte1;
            realval &= 0x7FFF;
            uint tempaddress = curAddress & 0xFFFF8000;
            tempaddress += realval;
            if(tempaddress == m_waitForInputAddress)
            {
                strRet = "WAIT_FOR_INPUT";
            }
            else
            {
                strRet = tempaddress.ToString("X");
            }
            

            curAddress += 2;

            return strRet;
        }

        // Load Portrait - Seems to always be proceeded by 0x02 and ends with 0x00.
        // Perhaps 0x00 is tied to the byte, and it's reading a word?
        private string Process0x07(ref uint curAddress)
        {
            byte byte1;
            string strRet = "LOAD_PORTRAIT ";
            curAddress++;
            byte1 = m_data[curAddress];
            curAddress++;
            strRet += byte1.ToString("X2");
            return strRet;
        }

        // Load option from memory (Used with Generic NPCs saving names)
        private string Process0x13(ref uint curAddress)
        {
            uint address = 0;
            string strRet;
            curAddress++;
            address = GetWord(ref curAddress);

            strRet = String.Format("LOAD_OPTION_FROM_MEMORY ADDRESS {0:X4}", address);

            return strRet;
        }

        // Blank RAM
        private string Process0x88(ref uint curAddress)
        {
            string strRet = "BLANK";
            curAddress++;

            return strRet;
        }

        private string Process0x89(ref uint curAddress)
        {
            string strRet = "RETURN";
            curAddress++;

            return strRet;
        }

        // Local Jump
        private string Process0x8A(ref uint curAddress)
        {
            string strRet = "GOTO_8A ";
            curAddress++;
            strRet += GetLocalAddress(ref curAddress);

            return strRet;
        }

        // Local Jump
        private string Process0x8C(ref uint curAddress)
        {
            string strRet = "GOTO ";
            curAddress++;
            strRet += GetLocalAddress(ref curAddress);

            return strRet;
        }

        // Long Jump
        private string Process0x8D(ref uint curAddress)
        {
            string strRet = "XJUMP ";
            curAddress++;
            strRet += GetGlobalAddress(ref curAddress);

            return strRet;
        }

        // Jump
        private string Process0x94(ref uint curAddress)
        {
            string strRet = "IF ";
            curAddress++;
            strRet += GetWord(ref curAddress).ToString("X4");
            strRet += " JUMP ";
            strRet += GetLocalAddress(ref curAddress);

            return strRet;
        }

        // Local Jump - Close Conditinal
        private string Process0x96(ref uint curAddress)
        {
            string strRet = "JUMP_END ";
            curAddress++;
            strRet += GetLocalAddress(ref curAddress);

            return strRet;
        }
        // Local Jump - Close Conditinal
        private string Process0x97(ref uint curAddress)
        {
            string strRet = "JUMP_97 ";
            curAddress++;
            strRet += GetLocalAddress(ref curAddress);

            return strRet;
        }

        // Set Bit
        private string Process0x9A(ref uint curAddress)
        {
            byte byte1;
            byte byte2;
            string strRet = "SET BIT ";
            curAddress++;

            byte1 = m_data[curAddress];
            byte2 = m_data[curAddress + 1];
            curAddress += 2;
            strRet += byte1.ToString("X2");
            strRet += ("(");
            if (m_knownBits.ContainsKey(byte1))
            {
                strRet += m_knownBits[byte1];
            }
            else
            {
                strRet += "?";
            }
            strRet += ") VALUE " + byte2;

            return strRet;
        }

        // If not bit
        private string Process0x9B(ref uint curAddress)
        {
            byte byte1;
            byte byte2;
            string strRet = "IF_NOT ";
            curAddress++;

            byte1 = m_data[curAddress];
            byte2 = m_data[curAddress + 1]; // comparison?
            curAddress += 2;
            strRet += byte1.ToString("X2");
            strRet += ("(");
            if (m_knownBits.ContainsKey(byte1))
            {
                strRet += m_knownBits[byte1];
            }
            else
            {
                strRet += "?";
            }
            strRet += ") THEN ";

            return strRet;
        }

        private string Process0xA0(ref uint curAddress)
        {
            string strRet = "A0??? ";
            uint value;
            curAddress++;
            value = GetWord(ref curAddress);

            strRet += value.ToString("X4");

            return strRet;
        }

        // Saves a value for callback
        // First 2 bytes Memory Address
        // Second 2 bytes Option Value
        private string Process0xA1(ref uint curAddress)
        {
            uint address = 0;
            uint option = 0;
            string strRet;
            curAddress++;
            address = GetWord(ref curAddress);
            option = GetWord(ref curAddress);
            string strOption = GetDialogOption(option);

            strRet = String.Format("STORE_OPTION_IN_MEMORY ADDRESS: {0:X4} VALUE: {1:X4}({2})", address, option, strOption);

            return strRet;
        }

        // Conditional Dialog
        private string Process0xAB(ref uint curAddress)
        {
            byte curByte = 0;
            string strRet = "SKIP  ";
            curAddress++;
            curByte = m_data[curAddress];
            curAddress++;
            strRet += curByte.ToString("X2");

            return strRet;
        }

        // Conditional Dialog
        private string Process0xB6(ref uint curAddress)
        {
            uint curWord = 0;
            string strRet = "IF_NOT_SELECTED_DIALOG ";
            curAddress++;
            curWord = GetWord(ref curAddress);
            curWord &= 0x7FFF;
            strRet += " " + curWord.ToString("X4") + "(";
            // Output the string here
            string strOption = GetDialogOption(curWord);
            m_dialogOptions.Add(strOption);
            strRet += strOption;
            strRet += ") ";

            strRet += "THEN:";

            return strRet;
        }

        // Load dialog in memory
        private string Process0xB8(ref uint curAddress)
        {
            uint curWord = 0;
            string strRet = "LOAD_DIALOG_OPTIONS";
            curAddress++;
            do
            {
                curWord = GetWord(ref curAddress);
                uint tempWord = (curWord & 0xBFFF);
                strRet += " " + tempWord.ToString("X4") + "(";
                // Output the string here
                string strOption = GetDialogOption(tempWord);
                m_dialogOptions.Add(strOption);
                strRet += strOption;
                strRet += ")";

            }
            while (0 == (curWord & 0x4000));

            return strRet;
        }

        // Continue loading dialog into memory
        private string Process0xB9(ref uint curAddress)
        {
            uint curWord = 0;
            string strRet = "LOAD_MORE_DIALOG_OPTIONS";
            curAddress++;
            do
            {
                curWord = GetWord(ref curAddress);
                uint tempWord = (curWord & 0xBFFF);
                strRet += " " + tempWord.ToString("X4") + "(";
                // Output the string here
                string strOption = GetDialogOption(tempWord);
                m_dialogOptions.Add(strOption);
                strRet += strOption;
                strRet += ")";
            }
            while (0 == (curWord & 0x4000));
            return strRet;
        }

        // Display dialog options
        private string Process0xBE(ref uint curAddress)
        {
            /*
			 * Display or hide dialogue options. If the highest bit is set (8x), then display it. 
			 * If not set, then hide the option. These are indexed relative to the order they were 
			 * loaded in by a B8 or B9 command.
			*/
            byte curByte = 0;
            string strRet = "SET_DIALOG_OPTIONS";
            curAddress++;

            do
            {
                curByte = m_data[curAddress];
                
                if(curByte != 0)
                {
                    if(0 != (0x80 & curByte))
                    {
                        strRet += " SHOW";
                    }
                    else
                    {
                        strRet += " HIDE";
                    }
                    int val = (curByte & 0x7F);
                    strRet += " " + val.ToString() + "(";
                    // Output the string here
                    val--;
                    if (m_dialogOptions.Count > val)
                    {
                        strRet += m_dialogOptions[val];
                    }
                    else
                    {
                        strRet += "ERROR_EXCEEDED_OPTIONS";
                    }
                    
                    strRet += ")";
                }
                
                curAddress++;
            }
            while (0 != curByte);

            return strRet;
        }

        // Set flag in RAM
        private string Process0xC0(ref uint curAddress)
        {
            uint curWord = 0;
            string strRet = "SET";
            curAddress++;
            curWord = GetWord(ref curAddress);
            strRet += " " + curWord.ToString("X4") + "(";
            // Output the string here
            if (m_knownFlags.ContainsKey(curWord))
            {
                strRet += m_knownFlags[curWord];
            }
            else
            {
                strRet += "?";
            }
            strRet += ")";
            return strRet;
        }

        private string Process0xC2(ref uint curAddress)
        {
            uint curWord = 0;
            string strRet = "FLAGS_GREATER_THAN ";
            curAddress++;
            curWord = m_data[curAddress];
            curAddress++;
            strRet += curWord.ToString("X2");
            return strRet;
        }

        // Conditional
        private string Process0xC4(ref uint curAddress)
        {
            uint curWord = 0;
            string strRet = "IF_NOT_FLAG ";
            curAddress++;
            curWord = GetWord(ref curAddress);
            strRet += curWord.ToString("X4");
            strRet += ("(");
            if (m_knownFlags.ContainsKey(curWord))
            {
                strRet += m_knownFlags[curWord];
            }
            else
            {
                strRet += "?";
            }
            strRet += ") THEN:";
            return strRet;
        }

        private string Process0xC9(ref uint curAddress)
        {
            string strRet = "WAIT_FOR_INPUT";
            m_waitForInputAddress = curAddress;
            curAddress++;
            return strRet;
        }

        // Not entirely sure exactly how this works, only that this is the correct break down of the instruction
        private string Process0xCA(ref uint curAddress)
        {
            string strRet = "YES_NO_PROMPT ";
            curAddress++;
            //strRet += GetLocalAddress(ref curAddress);
            return strRet;
        }

        private string Process0xCB(ref uint curAddress)
        {
            string strRet = "YES_NO_PROMPT_IF_NO_CONTINUE_TO ";
            curAddress++;
            strRet += GetLocalAddress(ref curAddress);
            return strRet;
        }

        private string Process0xCC(ref uint curAddress)
        {
            string strRet = "YES_NO_PROMPT_END_NO_JUMP ";
            curAddress++;
            strRet += GetLocalAddress(ref curAddress);
            return strRet;
        }

        private string Process0x01(ref uint curAddress, ref bool textmode)
        {
            string strRet = "LINE_BREAK";
            curAddress++;
            return strRet;
        }

            private string Process0x05(ref uint curAddress, ref bool textmode)
        {
            string strRet = "";
            if(!textmode)
            {
                strRet += "PRINT: ";
            }
            textmode = true;
            curAddress++;
            byte codeByte = m_data[curAddress];
            curAddress++;
            if(codeByte >= 0xc0) // A0-BF seem to be bank info
            {
                codeByte -= 0x20;
            }
            if(codeByte >= m_charSet.Count)
            {
                // There's an error here
                strRet += "?";
            }
            else
            {
                strRet += m_charSet[codeByte];
            }

            return strRet;
        }

        private void LoadConversation(int id)
        {
            bool textmode = false;
            uint curAddress = m_conv_addresses_pc[id];
            uint conversation_length = 4000;
            bool hasEndAddress = false;
            if(id + 1 < m_conv_addresses_pc.Length)
            {
                conversation_length = m_conv_addresses_pc[id + 1] - curAddress;
                hasEndAddress = true;
            }
            uint endAddress = curAddress + conversation_length;
            m_CurrentConversation = "-- DIALOG " + (id + 1) + Environment.NewLine;
            string strCharset;
            uint numchars = LoadCharset(id, out strCharset);
            m_CurrentConversation += strCharset;
            while (curAddress < endAddress)
            {
                bool done = false;
                byte codeByte = m_data[curAddress];

                if (codeByte != 0x05 && codeByte < 65 || codeByte > 122)
                {
                    if(textmode)
                    {
                        m_CurrentConversation += Environment.NewLine;
                    }
                    textmode = false;
                }
                if (!textmode && codeByte != 0xFF)
                {
                    m_CurrentConversation += curAddress.ToString("X") + ":\t";
                }
                
                switch (codeByte)
                {
                    case 0x01:
                        m_CurrentConversation += Process0x01(ref curAddress, ref textmode);
                        break;
                    case 0x05:
                        m_CurrentConversation += Process0x05(ref curAddress, ref textmode);
                        break;
                    case 0x07:
                        m_CurrentConversation += Process0x07(ref curAddress);
                        break;
                    case 0x013:
                        m_CurrentConversation += Process0x13(ref curAddress);
                        break;
                    case 0x8A: // Jumps further down in the script, and stores the current address to return to. Should return here via 0x89
                        m_CurrentConversation += Process0x8A(ref curAddress);
                        break;
                    case 0x88: // Blank/Return to where the conversation was called, close conversation
                        m_CurrentConversation += Process0x88(ref curAddress);
                        break;
                    case 0x89: // Return to what called this address, keep conversation open
                        m_CurrentConversation += Process0x89(ref curAddress);
                        break;
                    case 0x8C: // GOTO
                        m_CurrentConversation += Process0x8C(ref curAddress);
                        break;
                    case 0x8D:
                        m_CurrentConversation += Process0x8D(ref curAddress);
                        break;
                    case 0x94:
                        m_CurrentConversation += Process0x94(ref curAddress);
                        break;
                    case 0x96: // JUMP_END
                        m_CurrentConversation += Process0x96(ref curAddress);
                        break;
                    case 0x97: // JUMP???
                        m_CurrentConversation += Process0x97(ref curAddress);
                        break;
                    case 0x9A:
                        m_CurrentConversation += Process0x9A(ref curAddress);
                        break;
                    case 0x9B: // IF_NOT
                        m_CurrentConversation += Process0x9B(ref curAddress);
                        break;
                    //case 0x9F // Seems like some sort of randomizer
                    //    break;
                    case 0xA0:
                        m_CurrentConversation += Process0xA0(ref curAddress);
                        break;
                    case 0xA1:
                        m_CurrentConversation += Process0xA1(ref curAddress);
                        break;
                    case 0xAB:
                        m_CurrentConversation += Process0xAB(ref curAddress);
                        break;
                    case 0xB6: // IF_NOT_SELECTED_DIALOG
                        m_CurrentConversation += Process0xB6(ref curAddress);
                        break;
                    case 0xB8: // LOAD_DIALOG_OPTIONS - Also ends current dialog
                        m_CurrentConversation += Process0xB8(ref curAddress);
                        break;
                    case 0xB9:
                        m_CurrentConversation += Process0xB9(ref curAddress);
                        break;
                    case 0xBE: // SET_DIALOG_OPTIONS
                        m_CurrentConversation += Process0xBE(ref curAddress);
                        break;
                    case 0xC0: // Set flag in RAM
                        m_CurrentConversation += Process0xC0(ref curAddress);
                        break;
                    case 0xC2:
                        m_CurrentConversation += Process0xC2(ref curAddress);
                        break;
                    case 0xC4:
                        m_CurrentConversation += Process0xC4(ref curAddress);
                        break;
                    case 0xC9: // WAIT_FOR_INPUT
                        m_CurrentConversation += Process0xC9(ref curAddress);
                        break;
                    case 0xCA: // IF_NOT_PROMPT_THEN
                        m_CurrentConversation += Process0xCA(ref curAddress);
                        break;
                    case 0xCB: // IF_NOT_PROMPT_THEN GOTO
                        m_CurrentConversation += Process0xCB(ref curAddress);
                        break;
                    case 0xCC: // IF_NOT_PROMPT_THEN GOTO
                        m_CurrentConversation += Process0xCC(ref curAddress);
                        break;
                    case 0xFF:
                        if (!hasEndAddress)
                        {
                            done = true;
                        }
                        else
                        {
                            // Probably part of another code that's not figured out
                            //m_CurrentConversation += curAddress.ToString("X") + ":\t";
                            //m_CurrentConversation += "*" + codeByte.ToString("X2") + "*";
                            curAddress++;
                            //m_CurrentConversation += Environment.NewLine;
                        }
                        break;
                    default:
                        m_CurrentConversation += "*" + codeByte.ToString("X2") + "*";
                        curAddress++;
                        break;
                }
                if(!textmode && codeByte != 0xFF)
                {
                    m_CurrentConversation += Environment.NewLine;
                }
                
                if(done)
                {
                    break;
                }
            }
        }

        private string GetDialogOption(uint address)
        {
            string strRet = "";
            bool readbyte = false;
            uint tempaddress = (address & 0x7FF);
            if (tempaddress < 0x200)
            {
                return "???_1";
            }
            tempaddress -= 0x200;
            if(tempaddress < 59 || tempaddress > m_optionSet.Count + 59)
            {
                return "???_2";
            }
            uint dialogindex = tempaddress - 59;
            byte[] curOption = m_optionSet[(int)dialogindex];
            for(int index = 0; index < curOption.Length; ++index)
            {
                byte curByte = curOption[index];
                if(curByte == 0x05 && readbyte == false)
                {
                    readbyte = true;
                    continue;
                }
                if(readbyte)
                {
                    if (curByte >= 0xc0) // A0-BF seem to be bank info
                    {
                        curByte -= 0x20;
                    }
                    if (curByte >= m_charSet.Count)
                    {
                        // There's an error here
                        strRet += "?";
                    }
                    else
                    {
                        strRet += m_charSet[curByte];
                    }
                }
                else
                {
                    // Most likely Latin alphabet investigate
                    int j = 9;
                }
                readbyte = false;
            }
            return strRet;
        }
    }
}
