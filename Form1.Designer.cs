namespace Moscovium_Lite
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            panel1 = new Panel();
            button2 = new Button();
            button1 = new Button();
            label1 = new Label();
            button3 = new Button();
            button4 = new Button();
            button5 = new Button();
            button6 = new Button();
            panel2 = new Panel();
            label2 = new Label();
            pictureBox1 = new PictureBox();
            button7 = new Button();
            button8 = new Button();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.ControlDarkDark;
            panel1.Controls.Add(button2);
            panel1.Controls.Add(button1);
            panel1.Controls.Add(label1);
            panel1.Location = new Point(-5, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(845, 34);
            panel1.TabIndex = 0;
            panel1.MouseDown += panel1_MouseDown;
            // 
            // button2
            // 
            button2.BackColor = Color.Moccasin;
            button2.Location = new Point(750, 2);
            button2.Name = "button2";
            button2.Size = new Size(43, 29);
            button2.TabIndex = 2;
            button2.Text = "-";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // button1
            // 
            button1.BackColor = Color.Red;
            button1.Location = new Point(799, 2);
            button1.Name = "button1";
            button1.Size = new Size(43, 29);
            button1.TabIndex = 1;
            button1.Text = "X";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 16.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.Indigo;
            label1.Location = new Point(17, 0);
            label1.Name = "label1";
            label1.Size = new Size(211, 38);
            label1.TabIndex = 0;
            label1.Text = "Moscovium Lite";
            label1.Click += label1_Click;
            // 
            // button3
            // 
            button3.BackColor = Color.Indigo;
            button3.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button3.ForeColor = Color.SlateBlue;
            button3.Location = new Point(12, 63);
            button3.Name = "button3";
            button3.Size = new Size(207, 67);
            button3.TabIndex = 1;
            button3.Text = "Toolbox";
            button3.UseVisualStyleBackColor = false;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.BackColor = Color.Indigo;
            button4.Font = new Font("Segoe UI", 16.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button4.ForeColor = Color.SlateBlue;
            button4.Location = new Point(12, 136);
            button4.Name = "button4";
            button4.Size = new Size(207, 67);
            button4.TabIndex = 2;
            button4.Text = "Debloat Script";
            button4.UseVisualStyleBackColor = false;
            button4.Click += button4_Click;
            // 
            // button5
            // 
            button5.BackColor = Color.Indigo;
            button5.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button5.ForeColor = Color.SlateBlue;
            button5.Location = new Point(12, 209);
            button5.Name = "button5";
            button5.Size = new Size(207, 67);
            button5.TabIndex = 3;
            button5.Text = "Change Wallpaper";
            button5.UseVisualStyleBackColor = false;
            button5.Click += button5_Click;
            // 
            // button6
            // 
            button6.BackColor = Color.Indigo;
            button6.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button6.ForeColor = Color.SlateBlue;
            button6.Location = new Point(12, 282);
            button6.Name = "button6";
            button6.Size = new Size(207, 67);
            button6.TabIndex = 4;
            button6.Text = "Yabosen.cfg";
            button6.UseVisualStyleBackColor = false;
            button6.Click += button6_Click;
            // 
            // panel2
            // 
            panel2.AllowDrop = true;
            panel2.BackColor = Color.Indigo;
            panel2.Controls.Add(label2);
            panel2.Location = new Point(12, 382);
            panel2.Name = "panel2";
            panel2.Size = new Size(396, 87);
            panel2.TabIndex = 5;
            panel2.DragDrop += panel2_DragDrop;
            panel2.DragEnter += panel2_DragEnter;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 16.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.ForeColor = SystemColors.ActiveCaption;
            label2.Location = new Point(44, 26);
            label2.Name = "label2";
            label2.Size = new Size(347, 38);
            label2.TabIndex = 0;
            label2.Text = "Drop your CS2 config here";
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(665, 382);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(162, 88);
            pictureBox1.TabIndex = 6;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // button7
            // 
            button7.BackColor = Color.Indigo;
            button7.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button7.ForeColor = Color.SlateBlue;
            button7.Location = new Point(653, 63);
            button7.Name = "button7";
            button7.Size = new Size(174, 67);
            button7.TabIndex = 7;
            button7.Text = "Restart Explorer";
            button7.UseVisualStyleBackColor = false;
            button7.Click += button7_Click;
            // 
            // button8
            // 
            button8.BackColor = Color.Indigo;
            button8.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button8.ForeColor = Color.SlateBlue;
            button8.Location = new Point(473, 63);
            button8.Name = "button8";
            button8.Size = new Size(174, 67);
            button8.TabIndex = 8;
            button8.Text = "VisualC Runtime";
            button8.UseVisualStyleBackColor = false;
            button8.Click += button8_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Desktop;
            ClientSize = new Size(839, 489);
            Controls.Add(button8);
            Controls.Add(button7);
            Controls.Add(pictureBox1);
            Controls.Add(panel2);
            Controls.Add(button6);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Form1";
            Text = "Moscovium Lite";
            Load += Form1_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Label label1;
        private Button button2;
        private Button button1;
        private Button button3;
        private Button button4;
        private Button button5;
        private Button button6;
        private Panel panel2;
        private Label label2;
        private PictureBox pictureBox1;
        private Button button7;
        private Button button8;
    }
}
