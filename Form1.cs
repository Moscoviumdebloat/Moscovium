using Microsoft.Win32;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO.Compression;
using System.Runtime.InteropServices;

namespace Moscovium_Lite
{
    public partial class Form1 : Form
    {
        private HttpClient client = new HttpClient();


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
        public Form1()
        {
            InitializeComponent();
            label3.Text = DateTime.Now.ToString("hh:mm");
            label4.Text = DateTime.Now.ToString("D");
            ApplyRetroFont(this);
        }



        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        public void RunProcess(string command)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = Environment.ExpandEnvironmentVariables("%SystemRoot%\\System32\\WindowsPowerShell\\v1.0\\powershell.exe"),
                Arguments = "-NoProfile -ExecutionPolicy Bypass -NoExit -Command \"" + command + "\"",
                RedirectStandardOutput = false,
                UseShellExecute = true,
                CreateNoWindow = false,
                Verb = "runas"
            });
        }
        private void button3_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            try
            {
                string[] debloatArgs =
                {
                "-Silent", "-RemoveApps", "-RemoveGamingApps", "-DisableTelemetry",
                "-DisableBing", "-DisableSuggestions", "-DisableLockscreenTips",
                "-RevertContextMenu", "-TaskbarAlignLeft", "-HideSearchTb",
                "-DisableWidgets", "-DisableCopilot", "-ClearStartAllUsers",
                "-DisableDVR", "-DisableStartRecommended", "-ExplorerToThisPC",
                "-DisableMouseAcceleration", "-DisableDesktopSpotlight",
                "-DisableSettings365Ads", "-DisableSettingsHome",
                "-DisablePaintAI", "-DisableNotepadAI", "-DisableStickyKeys"
            };

                var arguments = "&([scriptblock]::Create((irm \"https://debloat.raphi.re/\"))) -RunDefaults " +
                                string.Join(" ", debloatArgs);

                ProcessHelper.RunElevatedPowerShellRaw(arguments);

                MessageDisplay msd = new MessageDisplay("Moscovium Lite", "Raphi debloat script has been launched.");
                msd.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageDisplay msd = new MessageDisplay("Moscovium Lite Error", $"Failed to run Raphi debloat: {ex.Message}");
                msd.ShowDialog();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {

            WallpaperStyleSelect dlg = new WallpaperStyleSelect();
            dlg.ShowDialog();

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            var selected = dlg.SelectedWPStyle;
            if (dlg.HasChanged)
            {
                openFileDialog1.Filter = "Image Files (*.bmp;*.jpg;*.jpeg;*.png)|*.bmp;*.jpg;*.jpeg;*.png";
                if (openFileDialog1.ShowDialog() != DialogResult.OK)
                    return;
                Wallpaper.Set(openFileDialog1.FileName, selected);
            }
            else
            {
                MessageDisplay msg = new MessageDisplay("Error!", "Wallpaper fitting option not selected. Please choose");
                msg.ShowDialog();
            }
        }

        private async void button6_Click(object sender, EventArgs e)
        {

            List<string> folders = new List<string>();
            string str1 = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Counter-Strike Global Offensive\\game\\csgo\\cfg";
            if (Directory.Exists(str1))
                folders.Add(str1);
            foreach (DriveInfo driveInfo in Enumerable.Where<DriveInfo>((IEnumerable<DriveInfo>)DriveInfo.GetDrives(), (Func<DriveInfo, bool>)(x => x.IsReady)))
            {
                try
                {
                    foreach (string directory in Directory.GetDirectories(((FileSystemInfo)driveInfo.RootDirectory).FullName, "Steam", (SearchOption)1))
                    {
                        string str2 = Path.Combine(directory, "steamapps\\common\\Counter-Strike Global Offensive\\game\\csgo\\cfg");
                        if (Directory.Exists(str2))
                            folders.Add(str2);
                    }
                }
                catch
                {
                }
            }
            if (folders.Count == 0)
            {
                MessageDisplay msg = new MessageDisplay("YabosenCFG Fail", "Could not find CS:GO cfg folder.");
                msg.ShowDialog();

            }
            else
                await this.GetFile(folders);
        }
        public async Task GetFile(List<string> folders)
        {
            try
            {
                using (HttpResponseMessage response = await this.client.GetAsync("https://raw.githubusercontent.com/Yabosen/YabosenCFG/main/yabosen.cfg"))
                {
                    response.EnsureSuccessStatusCode();
                    byte[] numArray = await response.Content.ReadAsByteArrayAsync();
                    foreach (string folder in folders)
                    {
                        string str = Path.Combine(folder, "yabosen.cfg");
                        File.WriteAllBytes(str, numArray);
                        InfoDisplay infoDisplay = new InfoDisplay("YabosenCFG Succesful", "yabosen.cfg has been downloaded to " + str);
                        infoDisplay.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageDisplay messageDisplay = new MessageDisplay("YabosenCFG Fail", "Error: " + ex.Message);
                messageDisplay.ShowDialog();
            }
        }

        private async void panel2_DragDrop(object sender, DragEventArgs e)
        {
            string[] strArray = (string[])e.Data.GetData(DataFormats.FileDrop);
            for (int index = 0; index < strArray.Length; ++index)
            {
                string str1 = strArray[index];
                if (str1.Contains(".cfg"))
                {
                    InfoDisplay infoDisplay = new InfoDisplay("Moscovium CFG Downloader", "Downloading " + Path.GetFileName(str1) + "...");
                    infoDisplay.ShowDialog();
                    List<string> folders = new List<string>();
                    string str2 = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Counter-Strike Global Offensive\\game\\csgo\\cfg";
                    if (Directory.Exists(str2))
                        folders.Add(str2);
                    foreach (DriveInfo driveInfo in Enumerable.Where<DriveInfo>((IEnumerable<DriveInfo>)DriveInfo.GetDrives(), (Func<DriveInfo, bool>)(x => x.IsReady)))
                    {
                        try
                        {
                            foreach (string directory in Directory.GetDirectories(((FileSystemInfo)driveInfo.RootDirectory).FullName, "Steam", (SearchOption)1))
                            {
                                string str3 = Path.Combine(directory, "steamapps\\common\\Counter-Strike Global Offensive\\game\\csgo\\cfg");
                                if (Directory.Exists(str3))
                                    folders.Add(str3);
                            }
                        }
                        catch
                        {
                        }
                    }
                    if (folders.Count == 0)
                    {
                        MessageDisplay msg = new MessageDisplay("Moscovium CFG Downloader", "Could not find CS:GO cfg folder.");
                        msg.ShowDialog();

                        return;
                    }
                    await this.GetFile(folders, str1);
                }
                else
                {
                    MessageDisplay messageDisplay = new MessageDisplay("Moscovium CFG Downloader", "Not a .cfg file");
                    messageDisplay.ShowDialog();
                    return;
                }
            }
            strArray = (string[])null;
        }

        private void panel2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        public async Task GetFile(List<string> folders, string filename)
        {
            try
            {
                foreach (string folder in folders)
                {
                    string str = Path.Combine(folder, Path.GetFileName(filename));
                    File.Copy(filename, str, true);
                    InfoDisplay infoDisplay = new InfoDisplay("Moscovium CFG Downloader", Path.GetFileName(filename) + " has been downloaded to " + str);
                    infoDisplay.ShowDialog();

                }
            }
            catch (Exception ex)
            {
                MessageDisplay messageDisplay = new MessageDisplay("Moscovium CFG Downloader", "Error: " + ex.Message);
                messageDisplay.ShowDialog();

            }
        }



        private void pictureBox1_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutBox1 = new AboutBox1();
            aboutBox1.ShowDialog();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            foreach (Process process in Process.GetProcessesByName("explorer"))
            {
                process.Kill();
                Process.Start("C:\\Windows\\explorer.exe");
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string ZIP = Path.Combine(Path.GetTempPath(), "VisualCResometing.zip");
            File.WriteAllBytes(ZIP, Moscovium_Lite.Properties.Resources.VisualC);
            string Paeth = Path.Combine(Path.GetTempPath(), "VisualCDstributables");
            ZipFile.ExtractToDirectory(ZIP, Paeth);
            Process.Start(Path.Combine(Paeth, "install_all.bat"));

        }

        private void button9_Click(object sender, EventArgs e)
        {


        }

        private async void button9_Click_1(object sender, EventArgs e)
        {
            try
            {
                var debloatJson = Path.Combine(Path.GetTempPath(), "Debloat.json");
                File.WriteAllBytes(debloatJson, Properties.Resources.DebloatScript);

                ProcessHelper.RunPowerShellCommand("irm 'https://christitus.com/win' -OutFile \"$env:TEMP\\winutil.ps1\"");

                // Short delay to ensure download
                await Task.Delay(2000);

                ProcessHelper.RunPowerShellCommand($"& \"$env:TEMP\\winutil.ps1\" -Config '{debloatJson}' -Run");
                InfoDisplay infoDisplay = new InfoDisplay("Chris Titus WinUtil", "Chris Titus WinUtil (Automated) has been launched.");
                infoDisplay.ShowDialog();

            }
            catch (Exception ex)
            {
                MessageDisplay msd = new MessageDisplay("Moscovium Lite Error", $"Failed to run Chris Titus WinUtil: {ex.Message}");
                msd.ShowDialog();
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessHelper.RunElevated("bcdedit", "/set disabledynamictick yes");

                InfoDisplay infoDisplay = new InfoDisplay("Dynamic Tick Disabled", "The command 'bcdedit /set disabledynamictick yes' has been executed.\nA system restart is recommended for changes to take full effect.");
                infoDisplay.ShowDialog();

            }
            catch (Exception ex)
            {
                MessageDisplay msd = new MessageDisplay("Moscovium Lite Error", $"Failed to execute command: {ex.Message}");
                msd.ShowDialog();

            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            try
            {
                RegistryHelper.SetWin32PrioritySeparation(22);
                InfoDisplay infoDisplay = new InfoDisplay("Win32PrioritySeparation Set", "Win32PrioritySeparation has been set to 22 (Decimal). \nA restart might be required for changes to take full effect.");
                infoDisplay.ShowDialog();


            }
            catch (Exception ex)
            {
                MessageDisplay msd = new MessageDisplay("Moscovium Lite Error", $"Failed to set Win32PrioritySeparation: {ex.Message}");
                msd.ShowDialog();

            }
        }

        private async void button12_Click(object sender, EventArgs e)
        {
            try
            {
                var SAB = Path.Combine(Path.GetTempPath(), "StartAllBack.ps1");
                File.WriteAllBytes(SAB, Properties.Resources.StartAllBackPs1);
                if (System.IO.File.Exists(SAB))
                {
                    // Run hidden/separate process to verify
                    var proc = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = $"-ExecutionPolicy Bypass -File \"{SAB}\"",
                        UseShellExecute = true,
                        Verb = "runas"
                    });
                    if (proc != null)
                    {
                        await System.Threading.Tasks.Task.Run(() => proc.WaitForExit());
                        RestartStuff restartStuff = new RestartStuff();
                        restartStuff.ShowDialog();
                        if (restartStuff.Answer == "restart")
                        {
                            System.Diagnostics.Process.Start("shutdown", "/r /t 0");
                        }
                    }


                }
                else
                {
                    MessageDisplay msd = new MessageDisplay("Moscovium Lite Error", "StartAllBack.ps1 not found.");
                    msd.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageDisplay msd = new MessageDisplay("Moscovium Lite Error", $"Failed to run script: {ex.Message}");
                msd.ShowDialog();
                return;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label3.Text = DateTime.Now.ToString("hh:mm");
            label4.Text = DateTime.Now.ToString("D");
        }

        private void button13_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = Environment.ExpandEnvironmentVariables("%SystemRoot%\\System32\\WindowsPowerShell\\v1.0\\powershell.exe"),
                Arguments = "start control",
                RedirectStandardOutput = false,
                UseShellExecute = true,
                CreateNoWindow = true,
                Verb = "runas"
            });

        }
    }
    public sealed class Wallpaper
    {
        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 1;
        private const int SPIF_SENDWININICHANGE = 2;

        private Wallpaper()
        {
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(
          int uAction,
          int uParam,
          string lpvParam,
          int fuWinIni);

        public static void Set(string file, string style)
        {
            using (Stream stream = (Stream)new MemoryStream(File.ReadAllBytes(file)))
            {
                Image image = Image.FromStream(stream);
                string lpvParam = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
                string filename = lpvParam;
                ImageFormat bmp = ImageFormat.Bmp;
                image.Save(filename, bmp);
                RegistryKey registryKey1 = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", true);
                if (style == "Fill")
                {
                    RegistryKey registryKey2 = registryKey1;
                    int num = 10;
                    string str1 = num.ToString();
                    registryKey2.SetValue("WallpaperStyle", (object)str1);
                    RegistryKey registryKey3 = registryKey1;
                    num = 0;
                    string str2 = num.ToString();
                    registryKey3.SetValue("TileWallpaper", (object)str2);
                }
                else if (style == "Fit")
                {
                    RegistryKey registryKey4 = registryKey1;
                    int num = 6;
                    string str3 = num.ToString();
                    registryKey4.SetValue("WallpaperStyle", (object)str3);
                    RegistryKey registryKey5 = registryKey1;
                    num = 0;
                    string str4 = num.ToString();
                    registryKey5.SetValue("TileWallpaper", (object)str4);
                }
                else if (style == "Strech")
                {
                    RegistryKey registryKey6 = registryKey1;
                    int num = 2;
                    string str5 = num.ToString();
                    registryKey6.SetValue("WallpaperStyle", (object)str5);
                    RegistryKey registryKey7 = registryKey1;
                    num = 0;
                    string str6 = num.ToString();
                    registryKey7.SetValue("TileWallpaper", (object)str6);
                }
                else if (style == "Tile")
                {
                    RegistryKey registryKey8 = registryKey1;
                    int num = 0;
                    string str7 = num.ToString();
                    registryKey8.SetValue("WallpaperStyle", (object)str7);
                    RegistryKey registryKey9 = registryKey1;
                    num = 1;
                    string str8 = num.ToString();
                    registryKey9.SetValue("TileWallpaper", (object)str8);
                }
                else if (style == "Center")
                {
                    RegistryKey registryKey10 = registryKey1;
                    int num = 0;
                    string str9 = num.ToString();
                    registryKey10.SetValue("WallpaperStyle", (object)str9);
                    RegistryKey registryKey11 = registryKey1;
                    num = 0;
                    string str10 = num.ToString();
                    registryKey11.SetValue("TileWallpaper", (object)str10);
                }
                else if (style == "Span")
                {
                    RegistryKey registryKey12 = registryKey1;
                    int num = 22;
                    string str11 = num.ToString();
                    registryKey12.SetValue("WallpaperStyle", (object)str11);
                    RegistryKey registryKey13 = registryKey1;
                    num = 0;
                    string str12 = num.ToString();
                    registryKey13.SetValue("TileWallpaper", (object)str12);
                }
                else
                {
                    MessageDisplay msg = new MessageDisplay("Error!", "Wallpaper fitting option not recognized. Please choose another.");
                    msg.ShowDialog();
                }
                    Wallpaper.SystemParametersInfo(20, 0, lpvParam, 3);
            }
        }

        public enum Style
        {
            Tiled,
            Centered,
            Stretched,
        }
    }

}
