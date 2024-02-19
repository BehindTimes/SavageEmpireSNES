using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SavageEmpireSNESTransTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            tbOutput.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top);
            for(int i = 1; i <= 125; ++i)
            {
                cbConv.Items.Add(i.ToString());
            }
            //cbConv.Items.AddRange(new object)
        }

        private void processFile(byte[] fileData, uint nOffset, uint nBytes)
        {
            byte[] outbyte = new byte[2];
            var japaneseEncoding = Encoding.GetEncoding("shift_jis");

            tbOutput.Clear();

            for(uint index = 0; index < nBytes; index++)
            {
                uint byte1 = fileData[nOffset];
                uint byte2 = fileData[nOffset + 1];
                uint realval = (byte2 << 8) + byte1;


                if (realval % 3 == 0)
                {
                    realval /= 3;
                    realval += 0x8140;

                    outbyte[1] = (byte)(realval & 0xFF);
                    outbyte[0] = (byte)((realval >> 8) & 0xFF);
                    string test = japaneseEncoding.GetString(outbyte);
                    tbOutput.Text += test;
                }
                nOffset += 2;
            }
        }

        private void oldTranslate(string strFile)
        {
            byte[] fileData = File.ReadAllBytes(strFile);

            string offset = tbOffset.Text;
            string numBytes = tbNumBytes.Text;
            uint nOffset = 0;
            uint nBytes = 0;
            try
            {
                CultureInfo provider;
                provider = CultureInfo.InvariantCulture;

                if (offset.StartsWith("0x"))
                {
                    offset = offset.Remove(0, 2);
                }

                uint.TryParse(offset, System.Globalization.NumberStyles.HexNumber, provider, out nOffset);
            }
            catch (FormatException)
            {
                nOffset = 0;
            }
            try
            {
                uint.TryParse(numBytes, out nBytes);
            }
            catch (FormatException)
            {
                nBytes = 0;
            }
            if (nOffset > 0 && nBytes > 0)
            {
                processFile(fileData, nOffset, nBytes);
            }
        }

        private void btnTranslate_Click(object sender, EventArgs e)
        {
            string strFile = tbFile.Text;
            if(File.Exists(strFile))
            {
                oldTranslate(strFile);
            }
        }

        // 04 - Aloron
        // 16 - Intanya
        // 31 - Triolo
        // 32 - Tristia
        // 71-80 - Generic Kurak NPC
        private void cbConv_SelectedIndexChanged(object sender, EventArgs e)
        {
            string strFile = tbFile.Text;
            if (File.Exists(strFile))
            { }
                ConversationLoader cLoader = new ConversationLoader();
                byte[] fileData = File.ReadAllBytes(strFile);
                cLoader.LoadData(fileData);
                int curIndex = cbConv.SelectedIndex;
                if(curIndex >= 0 && curIndex < 125)
                {
                    cLoader.PrepConversations((int)curIndex);

                    tbOutput.Text = cLoader.m_CurrentConversation;
                }
            }

        private uint FindPreviousAddress(byte[] data, uint start_address)
        {
            // Another case of this will never happen, unless a person
            // runs this without knowing what they're doing
            if(start_address > data.Length)
            {
                return 0;
            }
            while(start_address > 2)
            {
                // Just a quick sanity check which should never happen
                if(start_address < 100)
                {
                    return 0;
                }
                start_address -= 2;
                if(data[start_address] != 0x05)
                {
                    return start_address + 1;
                }
            }
            return 0;
        }

        private uint FindPreviousCharacterAddress(byte[] data, uint start_address, int counter)
        {
            // Another case of this will never happen, unless a person
            // runs this without knowing what they're doing
            if (start_address > data.Length)
            {
                return 0;
            }

            while (counter > 0)
            {
                // Just a quick sanity check which should never happen
                if (start_address < 100)
                {
                    return 0;
                }
                start_address -= 2;
                if (data[start_address] == 0xFF && data[start_address + 1] == 0xFF)
                {
                    counter--;
                }
            }
            return start_address + 2;
        }

        private void findInventoryItem()
        {
            uint start_address = 0x00013515;
            uint character_address = 0x0010100c;
            bool bValid = true;

            string strFile = tbFile.Text;
            if (File.Exists(strFile))
            {
                byte[] fileData = File.ReadAllBytes(strFile);
                uint nBytes = 0;
                string numBytes = tbNumBytes.Text;

                try
                {
                    uint.TryParse(numBytes, out nBytes);
                }
                catch (FormatException)
                {
                    nBytes = 0;
                }

                if (nBytes > 0)
                {
                    int num_attempts = (int)nBytes;
                    for (int a = 0; a < num_attempts; ++a)
                    {
                        start_address = FindPreviousAddress(fileData, start_address);
                        if (start_address == 0)
                        {
                            bValid = false;
                            break;
                        }
                    }
                    if (bValid)
                    {
                        // 0x000134c7 - 20 items before poison arrow
                        // 0x00100fb4 - 20 words before poison arrow

                        character_address = FindPreviousCharacterAddress(fileData, character_address, num_attempts + 1);
                        if (character_address != 0)
                        {
                            int j = 0;
                        }
                    }
                }
            }
        }

        private void btnCheckInventory_Click(object sender, EventArgs e)
        {
            //findInventoryItem();
            //return;
            
            string numBytes = tbNumBytes.Text;
            string strFile = tbFile.Text;
            uint nBytes = 0;
            if (File.Exists(strFile))
            {
                byte[] fileData = File.ReadAllBytes(strFile);
                uint nOffset = 0;
                uint nStringOffset = 0;

                // 0x13419 - Vendor: It sells for !
                // 0x100ee2 - Vendor alphabet
                // 0x1342a - Vendor: It sells for !
                // 0x100f04 - Vendor alphabet
                // 0x00013515 - box
                // 0x000135a4 - poison arrow
                // 0x001010a0
                // 0x13742 - grapes
                // 0x0010125e
                // 0x136f8 - Grenade must be at this address.
                // 0x138ca - torch
                // 0x13be8 - ring - Ring must be at this address.
                // 0x101402 - たいまつ
                try
                {
                    string offset = tbOffset.Text;
                    string alphaoffset = tbAlphaOffset.Text;

                    uint.TryParse(numBytes, out nBytes);
                    

                    CultureInfo provider;
                    provider = CultureInfo.InvariantCulture;

                    if(offset.StartsWith("0x"))
                    {
                        offset = offset.Remove(0, 2);
                    }
                    if (alphaoffset.StartsWith("0x"))
                    {
                        alphaoffset = alphaoffset.Remove(0, 2);
                    }

                    uint.TryParse(offset, System.Globalization.NumberStyles.HexNumber, provider, out nOffset);
                    uint.TryParse(alphaoffset, System.Globalization.NumberStyles.HexNumber, provider, out nStringOffset);
                }
                catch (FormatException)
                {
                    nBytes = 0;
                }

                if (nBytes > 0 && nOffset > 0 && nStringOffset > 0)
                {
                    bool worked = testInventory(fileData, nStringOffset, nOffset, nBytes);
                }
            }
        }

        // This is only for the real untranslated ROM.  It will not work with a translated/partially translated ROM
        private bool testInventory(byte[] fileData, uint inventoryAddress, uint characterAddress, uint numItems)
        {
            var japaneseEncoding = Encoding.GetEncoding("shift_jis");
            byte[] outbyte = new byte[2];
            string strDisplay = "";

            List<string> listAlphabet = new List<string>();
            List<byte[]> listBlockData = new List<byte[]>();
            List<int> listUniqueBlockData = new List<int>();
            for (uint index = 0; index < numItems; ++index)
            {
                string strOut = "";
                uint curNum = 0;
                byte b0;
                byte b1;
                // This should never happen.  This will only happen if you're messing around
                // with this program, having no clue what's going on, but better to be safe
                // than sorry.
                if (characterAddress + 4 > fileData.Length)
                {
                    return false;
                }
                b0 = fileData[characterAddress];
                b1 = fileData[characterAddress + 1];
                curNum = (uint)b0 + (((uint)b1) << 8);
                characterAddress += 2;

                while (curNum != 0xFFFF)
                {   
                    if (characterAddress + 4 > fileData.Length)
                    {
                        return false;
                    }

                    if (curNum % 3 == 0)
                    {
                        curNum /= 3;
                        curNum += 0x8140;

                        outbyte[1] = (byte)(curNum & 0xFF);
                        outbyte[0] = (byte)((curNum >> 8) & 0xFF);
                        string test = japaneseEncoding.GetString(outbyte);
                        strOut += test;
                    }
                    else
                    {
                        return false; // Anything not divisible by 3 will produce garbage.  Not sure why it was written this way
                    }

                    b0 = fileData[characterAddress];
                    b1 = fileData[characterAddress + 1];
                    curNum = (uint)b0 + (((uint)b1) << 8);
                    characterAddress += 2;
                }
                if(strOut.Length <= 0)
                {
                    return false;
                }
                listAlphabet.Add(strOut);
            }

            for (uint index = 0; index < numItems; ++index)
            {
                byte option_length = fileData[inventoryAddress];

                inventoryAddress++;
                listBlockData.Add(new byte[option_length]);

                if(option_length % 2 != 0)
                {
                    return false;
                }

                Buffer.BlockCopy(fileData, (int)inventoryAddress, listBlockData.Last(), 0, option_length);
                inventoryAddress += option_length;
                List<byte> listUniqueBytes = new List<byte>(); 
                for (int tempindex = 0; tempindex < listBlockData.Last().Length; tempindex += 2)
                {
                    if (listBlockData.Last()[tempindex] != 0x05)
                    {
                        return false;
                    }
                    listUniqueBytes.Add(listBlockData.Last()[tempindex + 1]);
                }
                listUniqueBlockData.Add(listUniqueBytes.Distinct().ToList().Count);
            }

            // Sanity check here
            for (int index = 0; index < (int)numItems; ++index)
            {
                if ((listAlphabet[index].Length) != listUniqueBlockData[index])
                {
                    return false;
                }
            }
            // Now that we passed the sanity checks, construct the words
            for (int index = 0; index < (int)numItems; ++index)
            {
                string strOut = "";
                uint startVal = listBlockData[index][1];

                for (int tempindex = 1; tempindex < listBlockData[index].Length; tempindex += 2)
                {
                    uint tempval = listBlockData[index][tempindex];
                    if(tempval < startVal) // This should not happen
                    {
                        return false;
                    }
                    uint curCharacter = tempval - startVal;
                    if(curCharacter >= listAlphabet[index].Length) // Not within the range of the alphabet
                    {
                        return false;
                    }
                    strOut += listAlphabet[index][(int)curCharacter];
                }
                strDisplay += strOut;
                strDisplay += Environment.NewLine;
                strDisplay += Environment.NewLine;
            }
            tbOutput.Text = strDisplay;
            return true;
        }
    }
}
