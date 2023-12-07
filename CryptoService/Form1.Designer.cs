namespace CryptoService
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            initTextRTB = new RichTextBox();
            ciphroTextRTB = new RichTextBox();
            decryptedTextRTB = new RichTextBox();
            encryptBtn = new Button();
            decryptBtn = new Button();
            keyRTB = new RichTextBox();
            LeyLabel = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(20, 9);
            label1.Name = "label1";
            label1.Size = new Size(36, 15);
            label1.TabIndex = 0;
            label1.Text = "Initial";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(20, 126);
            label2.Name = "label2";
            label2.Size = new Size(63, 15);
            label2.TabIndex = 1;
            label2.Text = "Ciphrotext";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(20, 243);
            label3.Name = "label3";
            label3.Size = new Size(61, 15);
            label3.TabIndex = 2;
            label3.Text = "Decrypted";
            // 
            // initTextRTB
            // 
            initTextRTB.Location = new Point(20, 27);
            initTextRTB.Name = "initTextRTB";
            initTextRTB.Size = new Size(453, 96);
            initTextRTB.TabIndex = 3;
            initTextRTB.Text = "";
            // 
            // ciphroTextRTB
            // 
            ciphroTextRTB.Location = new Point(20, 144);
            ciphroTextRTB.Name = "ciphroTextRTB";
            ciphroTextRTB.Size = new Size(453, 96);
            ciphroTextRTB.TabIndex = 4;
            ciphroTextRTB.Text = "";
            // 
            // decryptedTextRTB
            // 
            decryptedTextRTB.Location = new Point(20, 261);
            decryptedTextRTB.Name = "decryptedTextRTB";
            decryptedTextRTB.Size = new Size(453, 96);
            decryptedTextRTB.TabIndex = 5;
            decryptedTextRTB.Text = "";
            // 
            // encryptBtn
            // 
            encryptBtn.Location = new Point(479, 27);
            encryptBtn.Name = "encryptBtn";
            encryptBtn.Size = new Size(75, 50);
            encryptBtn.TabIndex = 6;
            encryptBtn.Text = "Encrypt";
            encryptBtn.UseVisualStyleBackColor = true;
            encryptBtn.Click += encryptBtnClick;
            // 
            // decryptBtn
            // 
            decryptBtn.Location = new Point(479, 144);
            decryptBtn.Name = "decryptBtn";
            decryptBtn.Size = new Size(75, 50);
            decryptBtn.TabIndex = 7;
            decryptBtn.Text = "Decrypt";
            decryptBtn.UseVisualStyleBackColor = true;
            decryptBtn.Click += decryptBtnClick;
            // 
            // keyRTB
            // 
            keyRTB.Location = new Point(560, 48);
            keyRTB.Name = "keyRTB";
            keyRTB.Size = new Size(296, 146);
            keyRTB.TabIndex = 8;
            keyRTB.Text = "";
            // 
            // LeyLabel
            // 
            LeyLabel.AutoSize = true;
            LeyLabel.Location = new Point(560, 30);
            LeyLabel.Name = "LeyLabel";
            LeyLabel.Size = new Size(26, 15);
            LeyLabel.TabIndex = 9;
            LeyLabel.Text = "Key";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(868, 375);
            Controls.Add(LeyLabel);
            Controls.Add(keyRTB);
            Controls.Add(decryptBtn);
            Controls.Add(encryptBtn);
            Controls.Add(decryptedTextRTB);
            Controls.Add(ciphroTextRTB);
            Controls.Add(initTextRTB);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "MainForm";
            Text = "DES";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private RichTextBox initTextRTB;
        private RichTextBox ciphroTextRTB;
        private RichTextBox decryptedTextRTB;
        private Button encryptBtn;
        private Button decryptBtn;
        private Label KeyLabel;
        private RichTextBox keyRTB;
        private Label LeyLabel;
    }
}
