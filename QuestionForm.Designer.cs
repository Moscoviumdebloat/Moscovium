namespace Moscovium_Lite
{
    partial class QuestionForm
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
            panel1 = new Panel();
            button3 = new Button();
            label1 = new Label();
            pictureBox1 = new PictureBox();
            richTextBox1 = new RichTextBox();
            button1 = new Button();
            button4 = new Button();
            button2 = new Button();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.ControlDarkDark;
            panel1.Controls.Add(button3);
            panel1.Controls.Add(label1);
            panel1.Location = new Point(0, -4);
            panel1.Name = "panel1";
            panel1.Size = new Size(419, 47);
            panel1.TabIndex = 14;
            
            panel1.MouseDown += this.panel1_MouseDown;
            // 
            // button3
            // 
            button3.BackColor = Color.Red;
            button3.Location = new Point(329, 11);
            button3.Name = "button3";
            button3.Size = new Size(65, 29);
            button3.TabIndex = 4;
            button3.Text = "Cancel";
            button3.UseVisualStyleBackColor = false;
            button3.Click += this.button3_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.Indigo;
            label1.Location = new Point(-2, 8);
            label1.Name = "label1";
            label1.Size = new Size(125, 25);
            label1.TabIndex = 0;
            label1.Text = "Debloat Script";
            
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.LiteQuestion;
            pictureBox1.Location = new Point(12, 49);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(125, 102);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 15;
            pictureBox1.TabStop = false;
          
            // 
            // richTextBox1
            // 
            richTextBox1.BackColor = SystemColors.InactiveCaptionText;
            richTextBox1.BorderStyle = BorderStyle.None;
            richTextBox1.ForeColor = Color.Gold;
            richTextBox1.Location = new Point(143, 49);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.ReadOnly = true;
            richTextBox1.Size = new Size(251, 102);
            richTextBox1.TabIndex = 19;
            richTextBox1.Text = "Would you also like to install Explorer Patcher, OpenShell, and Nilesoft Shell with the debloat script, or StartAllBack with the debloat script, or just the debloat script?";
            richTextBox1.TextChanged += this.richTextBox1_TextChanged;
            // 
            // button1
            // 
            button1.BackColor = Color.FromArgb(0, 192, 0);
            button1.Font = new Font("Segoe UI", 16.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button1.ForeColor = SystemColors.ButtonFace;
            button1.Location = new Point(262, 157);
            button1.Name = "button1";
            button1.Size = new Size(127, 65);
            button1.TabIndex = 20;
            button1.Text = "Debloat";
            button1.UseVisualStyleBackColor = false;
            button1.Click += this.button1_Click;
            // 
            // button4
            // 
            button4.BackColor = Color.Purple;
            button4.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button4.ForeColor = SystemColors.ButtonFace;
            button4.Location = new Point(143, 157);
            button4.Name = "button4";
            button4.Size = new Size(113, 65);
            button4.TabIndex = 22;
            button4.Text = "Debloat\r\n + SAB";
            button4.UseVisualStyleBackColor = false;
            button4.Click += this.button4_Click;
            // 
            // button2
            // 
            button2.BackColor = Color.Teal;
            button2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button2.ForeColor = SystemColors.ButtonFace;
            button2.Location = new Point(12, 157);
            button2.Name = "button2";
            button2.Size = new Size(125, 65);
            button2.TabIndex = 23;
            button2.Text = "Debloat + EP, OP, NS";
            button2.UseVisualStyleBackColor = false;
            button2.Click += this.button2_Click_1;
            // 
            // QuestionForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaptionText;
            ClientSize = new Size(401, 233);
            Controls.Add(button2);
            Controls.Add(button4);
            Controls.Add(button1);
            Controls.Add(richTextBox1);
            Controls.Add(pictureBox1);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "QuestionForm";
            ShowInTaskbar = false;
            Text = "QuestionForm";
            Load += QuestionForm_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Label label1;
        private PictureBox pictureBox1;
        private RichTextBox richTextBox1;
        private Button button1;
        private Button button3;
        private Button button4;
        private Button button2;
    }
}