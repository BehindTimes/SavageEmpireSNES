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

        // 15 - Intanya
        // 30 - Triolo
        // 71 - Generic Kurak NPC
        // 73 - Generic Kurak NPC
        // 75 - Generic Kurak NPC
        // 77 - Generic Kurak NPC
        // 122 - Generic Kurak NPC
        private void cbConv_SelectedIndexChanged(object sender, EventArgs e)
        {
            string strFile = tbFile.Text;
            if (File.Exists(strFile))
            {
                oldTranslate(strFile);

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
        }
    }
}
