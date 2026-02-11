using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Moscovium_Lite
{
    public partial class Form2 : Form
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
        public Form2()
        {
            InitializeComponent();
            ApplyRetroFont(this);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = Environment.ExpandEnvironmentVariables("%SystemRoot%\\System32\\WindowsPowerShell\\v1.0\\powershell.exe"),
                Arguments = "iwr -useb https://christitus.com/win | iex",
                RedirectStandardOutput = false,
                UseShellExecute = true,
                CreateNoWindow = false,
                Verb = "runas"
            });
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = Environment.ExpandEnvironmentVariables("%SystemRoot%\\System32\\WindowsPowerShell\\v1.0\\powershell.exe"),
                Arguments = "& ([scriptblock]::Create((irm \"https://debloat.raphi.re/\")))",
                RedirectStandardOutput = false,
                UseShellExecute = true,
                CreateNoWindow = false,
                Verb = "runas"
            });
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = Environment.ExpandEnvironmentVariables("%SystemRoot%\\System32\\WindowsPowerShell\\v1.0\\powershell.exe"),
                Arguments = "irm https://get.activated.win | iex",
                RedirectStandardOutput = false,
                UseShellExecute = true,
                CreateNoWindow = false,
                Verb = "runas"
            });
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form3 f3 = new Form3();
            f3.Show();
        }
    }
}
