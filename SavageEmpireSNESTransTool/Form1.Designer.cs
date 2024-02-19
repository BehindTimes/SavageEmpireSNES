namespace SavageEmpireSNESTransTool
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbOffset = new System.Windows.Forms.TextBox();
            this.tbOutput = new System.Windows.Forms.TextBox();
            this.btnTranslate = new System.Windows.Forms.Button();
            this.tbNumBytes = new System.Windows.Forms.TextBox();
            this.lblOffset = new System.Windows.Forms.Label();
            this.lblNumBytes = new System.Windows.Forms.Label();
            this.lblFile = new System.Windows.Forms.Label();
            this.tbFile = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cbConv = new System.Windows.Forms.ComboBox();
            this.btnCheckInventory = new System.Windows.Forms.Button();
            this.tbAlphaOffset = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tbOffset
            // 
            this.tbOffset.Location = new System.Drawing.Point(54, 6);
            this.tbOffset.Name = "tbOffset";
            this.tbOffset.Size = new System.Drawing.Size(100, 20);
            this.tbOffset.TabIndex = 0;
            this.tbOffset.Text = "100f26";
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(12, 61);
            this.tbOutput.Multiline = true;
            this.tbOutput.Name = "tbOutput";
            this.tbOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbOutput.Size = new System.Drawing.Size(554, 286);
            this.tbOutput.TabIndex = 1;
            this.tbOutput.WordWrap = false;
            // 
            // btnTranslate
            // 
            this.btnTranslate.Location = new System.Drawing.Point(464, 3);
            this.btnTranslate.Name = "btnTranslate";
            this.btnTranslate.Size = new System.Drawing.Size(75, 23);
            this.btnTranslate.TabIndex = 2;
            this.btnTranslate.Text = "Translate";
            this.btnTranslate.UseVisualStyleBackColor = true;
            this.btnTranslate.Click += new System.EventHandler(this.btnTranslate_Click);
            // 
            // tbNumBytes
            // 
            this.tbNumBytes.Location = new System.Drawing.Point(358, 6);
            this.tbNumBytes.Name = "tbNumBytes";
            this.tbNumBytes.Size = new System.Drawing.Size(100, 20);
            this.tbNumBytes.TabIndex = 3;
            this.tbNumBytes.Text = "15";
            // 
            // lblOffset
            // 
            this.lblOffset.AutoSize = true;
            this.lblOffset.Location = new System.Drawing.Point(13, 9);
            this.lblOffset.Name = "lblOffset";
            this.lblOffset.Size = new System.Drawing.Size(35, 13);
            this.lblOffset.TabIndex = 4;
            this.lblOffset.Text = "Offset";
            // 
            // lblNumBytes
            // 
            this.lblNumBytes.AutoSize = true;
            this.lblNumBytes.Location = new System.Drawing.Point(266, 9);
            this.lblNumBytes.Name = "lblNumBytes";
            this.lblNumBytes.Size = new System.Drawing.Size(85, 13);
            this.lblNumBytes.TabIndex = 5;
            this.lblNumBytes.Text = "Number of Bytes";
            // 
            // lblFile
            // 
            this.lblFile.AutoSize = true;
            this.lblFile.Location = new System.Drawing.Point(12, 38);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(23, 13);
            this.lblFile.TabIndex = 6;
            this.lblFile.Text = "File";
            // 
            // tbFile
            // 
            this.tbFile.Location = new System.Drawing.Point(54, 35);
            this.tbFile.Name = "tbFile";
            this.tbFile.Size = new System.Drawing.Size(297, 20);
            this.tbFile.TabIndex = 7;
            this.tbFile.Text = "F:/snes/Roms/notrans.sfc";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(-47, 184);
            this.tableLayoutPanel1.MinimumSize = new System.Drawing.Size(20, 20);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(20, 20);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // cbConv
            // 
            this.cbConv.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbConv.FormattingEnabled = true;
            this.cbConv.Location = new System.Drawing.Point(358, 33);
            this.cbConv.Name = "cbConv";
            this.cbConv.Size = new System.Drawing.Size(77, 21);
            this.cbConv.TabIndex = 9;
            this.cbConv.SelectedIndexChanged += new System.EventHandler(this.cbConv_SelectedIndexChanged);
            // 
            // btnCheckInventory
            // 
            this.btnCheckInventory.Location = new System.Drawing.Point(441, 32);
            this.btnCheckInventory.Name = "btnCheckInventory";
            this.btnCheckInventory.Size = new System.Drawing.Size(108, 23);
            this.btnCheckInventory.TabIndex = 10;
            this.btnCheckInventory.Text = "Test Inventory";
            this.btnCheckInventory.UseVisualStyleBackColor = true;
            this.btnCheckInventory.Click += new System.EventHandler(this.btnCheckInventory_Click);
            // 
            // tbAlphaOffset
            // 
            this.tbAlphaOffset.Location = new System.Drawing.Point(160, 6);
            this.tbAlphaOffset.Name = "tbAlphaOffset";
            this.tbAlphaOffset.Size = new System.Drawing.Size(100, 20);
            this.tbAlphaOffset.TabIndex = 11;
            this.tbAlphaOffset.Text = "1343b";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(576, 369);
            this.Controls.Add(this.tbAlphaOffset);
            this.Controls.Add(this.btnCheckInventory);
            this.Controls.Add(this.cbConv);
            this.Controls.Add(this.tbOutput);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.tbFile);
            this.Controls.Add(this.lblFile);
            this.Controls.Add(this.lblNumBytes);
            this.Controls.Add(this.lblOffset);
            this.Controls.Add(this.tbNumBytes);
            this.Controls.Add(this.btnTranslate);
            this.Controls.Add(this.tbOffset);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbOffset;
        private System.Windows.Forms.TextBox tbOutput;
        private System.Windows.Forms.Button btnTranslate;
        private System.Windows.Forms.TextBox tbNumBytes;
        private System.Windows.Forms.Label lblOffset;
        private System.Windows.Forms.Label lblNumBytes;
        private System.Windows.Forms.Label lblFile;
        private System.Windows.Forms.TextBox tbFile;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ComboBox cbConv;
        private System.Windows.Forms.Button btnCheckInventory;
        private System.Windows.Forms.TextBox tbAlphaOffset;
    }
}

