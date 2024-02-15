using System;
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
            m_knownBits.Add(0x15, "DISKIKI_SOMETHING");
            m_knownBits.Add(0x81, "ASKED_DOKREI_ABOUT_UNION");
            m_knownBits.Add(0x85, "KNOWS_ARORON");
            m_knownBits.Add(0x86, "KNOWS_KURAK_NEED");
            m_knownBits.Add(0x87, "AIERA_SAVED_KURAK_JOINED_UNION");
            m_knownBits.Add(0x88, "KNOWS_APATON");
            m_knownBits.Add(0x8A, "KNOWS_YOLARU_NEED");
            m_knownBits.Add(0x8B, "YOLARU_JOINED_UNION");
            m_knownBits.Add(0x90, "KNOWS_BARRAB_NEED");
            m_knownBits.Add(0x91, "BARRAB_JOINED_UNION");
            m_knownBits.Add(0x94, "KNOWS_DISKIKI_NEED");
            m_knownBits.Add(0x95, "DISKIKI_JOINED_UNION");
            m_knownBits.Add(0x9A, "KNOWS_DOKREY");
            m_knownBits.Add(0x9E, "KNOWS_FRITZ");
            m_knownBits.Add(0x9F, "FRITZ_TALKED_ABOUT_BRAIN");
            m_knownBits.Add(0xA0, "FRITZ_ALREADY_TOLD_LONG_STORY");
            m_knownBits.Add(0xAA, "KNOWS_INARA");
            m_knownBits.Add(0xAB, "INARA_UNUSED?");
            m_knownBits.Add(0xAC, "INARA_EXPLAINED_ALREADY_WHY_PINDIRO_JOINS_UNION");
            m_knownBits.Add(0xAD, "INTANYA_CONVERSATION_RESET");
            m_knownBits.Add(0xB1, "KNOWS_ABOUT_TOPURU");
            m_knownBits.Add(0xC5, "KNOWS_MOSAGAN");
            m_knownBits.Add(0xCA, "KNOWS_Pakusakutamaku");
        }

        void LoadOptions()
        {
            uint PC_Address = DIALOG_OPTIONS;

            byte option_length = m_data[PC_Address];

            while (option_length != 0x82)
            {  
                PC_Address++;
                m_optionSet.Add(new byte[option_length]);
                if(m_optionSet.Count == 310)
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
            m_waitForInputAddress = curAddress;
            curAddress++;
            byte1 = m_data[curAddress];
            curAddress++;
            strRet += byte1.ToString("X2");
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
            string strRet = "???_END";
            curAddress++;

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
            string strRet = "IF NOT ";
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

        // ??? Something to do with profile loading
        private string Process0xA1(ref uint curAddress)
        {
            byte curByte = 0;
            string strRet = "LOAD_PROFILE ";
            curAddress++;
            curByte = m_data[curAddress];
            curAddress++;
            strRet += curByte.ToString("X2");
            curByte = m_data[curAddress];
            curAddress++;
            strRet += curByte.ToString("X2");
            curByte = m_data[curAddress];
            curAddress++;
            strRet += curByte.ToString("X2");
            curByte = m_data[curAddress];
            curAddress++;
            strRet += curByte.ToString("X2");

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
            string strRet = "IF NOT SELECTED DIALOG ";
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
                    if(m_dialogOptions.Count > val)
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
            GetWord(ref curAddress);
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

        private string Process0xCA(ref uint curAddress)
        {
            string strRet = "IF NOT PROMPT THEN";
            m_waitForInputAddress = curAddress;
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
            if(id + 1 < m_conv_addresses_pc.Length)
            {
                conversation_length = m_conv_addresses_pc[id + 1] - curAddress;
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
                    case 0x88:
                        m_CurrentConversation += Process0x88(ref curAddress);
                        break;
                    case 0x89:
                        m_CurrentConversation += Process0x89(ref curAddress);
                        break;
                    case 0x8C:
                        m_CurrentConversation += Process0x8C(ref curAddress);
                        break;
                    case 0x8D:
                        m_CurrentConversation += Process0x8D(ref curAddress);
                        break;
                    case 0x94:
                        m_CurrentConversation += Process0x94(ref curAddress);
                        break;
                    case 0x96:
                        m_CurrentConversation += Process0x96(ref curAddress);
                        break;
                    case 0x9A:
                        m_CurrentConversation += Process0x9A(ref curAddress);
                        break;
                    case 0x9B:
                        m_CurrentConversation += Process0x9B(ref curAddress);
                        break;
                    case 0xA1:
                        m_CurrentConversation += Process0xA1(ref curAddress);
                        break;
                    case 0xAB:
                        m_CurrentConversation += Process0xAB(ref curAddress);
                        break;
                    case 0xB6:
                        m_CurrentConversation += Process0xB6(ref curAddress);
                        break;
                    case 0xB8:
                        m_CurrentConversation += Process0xB8(ref curAddress);
                        break;
                    case 0xB9:
                        m_CurrentConversation += Process0xB9(ref curAddress);
                        break;
                    case 0xBE:
                        m_CurrentConversation += Process0xBE(ref curAddress);
                        break;
                    case 0xC0:
                        m_CurrentConversation += Process0xC0(ref curAddress);
                        break;
                    case 0xC2:
                        m_CurrentConversation += Process0xC2(ref curAddress);
                        break;
                    case 0xC4:
                        m_CurrentConversation += Process0xC4(ref curAddress);
                        break;
                    case 0xC9:
                        m_CurrentConversation += Process0xC9(ref curAddress);
                        break;
                    case 0xCA:
                        m_CurrentConversation += Process0xCA(ref curAddress);
                        break;
                    case 0xFF:
                        done = true;
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
