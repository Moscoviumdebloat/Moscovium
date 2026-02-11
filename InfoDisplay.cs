using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Moscovium_Lite
{
    public partial class InfoDisplay : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private Font GetRetroFont(float currentSize)
        {
            string[] candidates = new[] { "Fixedsys", "Terminal", "MS Sans Serif", "Lucida Console", "Courier New" };
            foreach (var name in candidates)
            {
                try
                {
                    var f = new Font(name, currentSize, FontStyle.Regular, GraphicsUnit.Point);
                    return f;
                }
                catch
                {

                }
            }

            return SystemFonts.DefaultFont;
        }

        private void ApplyRetroFont(Control parent)
        {
            if (parent == null) return;

            try
            {
                parent.Font = GetRetroFont(parent.Font.Size);
            }
            catch
            {

            }

            foreach (Control c in parent.Controls)
            {
                ApplyRetroFont(c);

            }
        }

        public InfoDisplay(string caption, string info)
        {
            InitializeComponent();
            ApplyRetroFont(this);
            label1.Text = caption;
            richTextBox1.Text = info;


        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void InfoDisplay_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
