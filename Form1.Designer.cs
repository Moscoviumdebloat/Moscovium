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
            components = new System.ComponentModel.Container();
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
            button9 = new Button();
            groupBox1 = new GroupBox();
            button10 = new Button();
            button11 = new Button();
            button12 = new Button();
            label3 = new Label();
            label4 = new Label();
            timer1 = new System.Windows.Forms.Timer(components);
            button13 = new Button();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            groupBox1.SuspendLayout();
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
            panel1.Size = new Size(924, 34);
            panel1.TabIndex = 0;
            panel1.MouseDown += panel1_MouseDown;
            // 
            // button2
            // 
            button2.BackColor = Color.Moccasin;
            button2.Location = new Point(824, 2);
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
            button1.Location = new Point(873, 2);
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
            button3.Location = new Point(12, 41);
            button3.Name = "button3";
            button3.Size = new Size(207, 74);
            button3.TabIndex = 1;
            button3.Text = "Toolbox";
            button3.UseVisualStyleBackColor = false;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.BackColor = Color.Indigo;
            button4.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button4.ForeColor = Color.SlateBlue;
            button4.Location = new Point(8, 23);
            button4.Name = "button4";
            button4.Size = new Size(129, 67);
            button4.TabIndex = 2;
            button4.Text = "Step 1 Win11 Debloat Raphi";
            button4.UseVisualStyleBackColor = false;
            button4.Click += button4_Click;
            // 
            // button5
            // 
            button5.BackColor = Color.Indigo;
            button5.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button5.ForeColor = Color.SlateBlue;
            button5.Location = new Point(12, 227);
            button5.Name = "button5";
            button5.Size = new Size(207, 49);
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
            button6.Size = new Size(207, 49);
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
            panel2.Location = new Point(12, 470);
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
            label2.Location = new Point(46, 27);
            label2.Name = "label2";
            label2.Size = new Size(347, 38);
            label2.TabIndex = 0;
            label2.Text = "Drop your CS2 config here";
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(757, 479);
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
            button7.Location = new Point(731, 41);
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
            button8.Location = new Point(551, 41);
            button8.Name = "button8";
            button8.Size = new Size(174, 67);
            button8.TabIndex = 8;
            button8.Text = "VisualC Runtime";
            button8.UseVisualStyleBackColor = false;
            button8.Click += button8_Click;
            // 
            // button9
            // 
            button9.BackColor = Color.Indigo;
            button9.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button9.ForeColor = Color.SlateBlue;
            button9.Location = new Point(143, 23);
            button9.Name = "button9";
            button9.Size = new Size(157, 67);
            button9.TabIndex = 9;
            button9.Text = "Step 2 Chris Titus WinUtil (Automated)";
            button9.UseVisualStyleBackColor = false;
            button9.Click += button9_Click_1;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(button9);
            groupBox1.Controls.Add(button4);
            groupBox1.ForeColor = SystemColors.ButtonFace;
            groupBox1.Location = new Point(12, 118);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(310, 100);
            groupBox1.TabIndex = 10;
            groupBox1.TabStop = false;
            groupBox1.Text = "Debloat Script";
            // 
            // button10
            // 
            button10.BackColor = Color.Indigo;
            button10.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button10.ForeColor = Color.SlateBlue;
            button10.Location = new Point(12, 337);
            button10.Name = "button10";
            button10.Size = new Size(207, 49);
            button10.TabIndex = 11;
            button10.Text = "Disable Dynamic Tick";
            button10.UseVisualStyleBackColor = false;
            button10.Click += button10_Click;
            // 
            // button11
            // 
            button11.BackColor = Color.Indigo;
            button11.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button11.ForeColor = Color.SlateBlue;
            button11.Location = new Point(12, 392);
            button11.Name = "button11";
            button11.Size = new Size(207, 72);
            button11.TabIndex = 12;
            button11.Text = "Set Win32PrioritySeperation";
            button11.UseVisualStyleBackColor = false;
            button11.Click += button11_Click;
            // 
            // button12
            // 
            button12.BackColor = Color.Indigo;
            button12.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button12.ForeColor = Color.SlateBlue;
            button12.Location = new Point(225, 40);
            button12.Name = "button12";
            button12.Size = new Size(207, 78);
            button12.TabIndex = 13;
            button12.Text = "Reset Trial StartAllBack";
            button12.UseVisualStyleBackColor = false;
            button12.Click += button12_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.ForeColor = SystemColors.ButtonHighlight;
            label3.Location = new Point(414, 470);
            label3.Name = "label3";
            label3.Size = new Size(89, 41);
            label3.TabIndex = 14;
            label3.Text = "00:00";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.ForeColor = SystemColors.ControlLight;
            label4.Location = new Point(414, 514);
            label4.Name = "label4";
            label4.Size = new Size(56, 25);
            label4.TabIndex = 15;
            label4.Text = "00:00";
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 15000;
            timer1.Tick += timer1_Tick;
            // 
            // button13
            // 
            button13.BackColor = Color.Indigo;
            button13.Font = new Font("Segoe UI", 16.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button13.ForeColor = Color.LightSteelBlue;
            button13.Location = new Point(731, 114);
            button13.Name = "button13";
            button13.Size = new Size(174, 94);
            button13.TabIndex = 16;
            button13.Text = "Control Panel";
            button13.UseVisualStyleBackColor = false;
            button13.Click += button13_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Desktop;
            ClientSize = new Size(917, 569);
            Controls.Add(button13);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(button12);
            Controls.Add(button11);
            Controls.Add(button10);
            Controls.Add(button8);
            Controls.Add(button7);
            Controls.Add(pictureBox1);
            Controls.Add(panel2);
            Controls.Add(button6);
            Controls.Add(button5);
            Controls.Add(button3);
            Controls.Add(panel1);
            Controls.Add(groupBox1);
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
            groupBox1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
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
        private Button button9;
        private GroupBox groupBox1;
        private Button button10;
        private Button button11;
        private Button button12;
        private Label label3;
        private Label label4;
        private System.Windows.Forms.Timer timer1;
        private Button button13;
    }
}
