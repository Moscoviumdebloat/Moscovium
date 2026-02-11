using Microsoft.Win32;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO.Compression;
namespace MoscoviumTwo
{
    public partial class Form1 : Form
    {
        private HttpClient client = new HttpClient();
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {

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
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            pictureBox1.Image = Properties.Resources.ButtonClickToolbox;
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            if (pictureBox1.Image != Properties.Resources.ButtonClickToolbox)
            {
                pictureBox1.Image = Properties.Resources.ButtonHoverToolbox;
            }
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            pictureBox1.Image = Properties.Resources.ButtonToolbox;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            pictureBox1.Image = Properties.Resources.ButtonToolbox;
        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            pictureBox2.Image = Properties.Resources.ButtonClickDebloat;
        }

        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {
            if (pictureBox2.Image != Properties.Resources.ButtonClickDebloat)
            {
                pictureBox2.Image = Properties.Resources.ButtonHoverDebloat;
            }
        }

        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
            pictureBox2.Image = Properties.Resources.ButtonDebloat;
        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            pictureBox2.Image = Properties.Resources.ButtonDebloat;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            var messagebox = MessageBox.Show("Would you also like to install Explorer Patcher, Openshell, and Nilesoft Shell with the debloat script", "Moscovium", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (messagebox == DialogResult.Yes)
            {
                string OPL = Path.Combine(Path.GetTempPath(), "OpenShellSetup_4_4_196.exe");
                File.WriteAllBytes(OPL, MoscoviumTwo.Properties.Resources.OpenShell);
                Process.Start(OPL);
                string EPL = Path.Combine(Path.GetTempPath(), "ep_setup.exe");
                File.WriteAllBytes(EPL, MoscoviumTwo.Properties.Resources.ep_setup);
                Process.Start(EPL);

                string NSL = Path.Combine(Path.GetTempPath(), "setup-x64.msi");
                File.WriteAllBytes(NSL, MoscoviumTwo.Properties.Resources.nilesoft_shell);
                Process.Start(new ProcessStartInfo
                {
                    FileName = "msiexec.exe",
                    Arguments = "/i \"" + NSL + "\"",
                    UseShellExecute = true
                });

                string[] args = {"-Silent",
        "-RemoveApps",
        "-RemoveGamingApps",
        "-DisableTelemetry",
        "-DisableBing",
        "-DisableSuggestions",
        "-DisableLockscreenTips",
        "-RevertContextMenu",
        "-TaskbarAlignLeft",
        "-HideSearchTb",
        "-DisableWidgets",
        "-DisableCopilot",
        "-ClearStartAllUsers",
        "-DisableDVR",
        "-DisableStartRecommended",
        "-ExplorerToThisPC",
        "-DisableMouseAcceleration",
        "-DisableDesktopSpotlight",
        "-DisableSettings365Ads",
        "-DisableSettingsHome",
        "-DisablePaintAI",
        "-DisableNotepadAI",
        "-DisableStickyKeys"};
                string arguments = "&([scriptblock]::Create((irm \"https://debloat.raphi.re/\"))) -RunDefaults" + string.Join(" ", args);
                Process.Start(new ProcessStartInfo()
                {
                    FileName = Environment.ExpandEnvironmentVariables("%SystemRoot%\\System32\\WindowsPowerShell\\v1.0\\powershell.exe"),
                    Arguments = arguments,
                    RedirectStandardOutput = false,
                    UseShellExecute = true,
                    CreateNoWindow = false,
                    Verb = "runas"
                });
                //var psi = new ProcessStartInfo
                //{
                //    FileName = Environment.ExpandEnvironmentVariables("%SystemRoot%\\System32\\WindowsPowerShell\\v1.0\\powershell.exe"),
                //    Arguments = "& ([scriptblock]::Create((irm \"https://debloat.raphi.re/\")))",
                //    RedirectStandardOutput = false,
                //    RedirectStandardInput = true,
                //    UseShellExecute = false,
                //    CreateNoWindow = false,

                //};

                //var p = Process.Start(psi);


                //p.StandardInput.WriteLine("1");



                //p.StandardInput.WriteLine("1");

                //p.StandardInput.WriteLine("");

                //p.StandardInput.Close();
                var prcss = new ProcessStartInfo()
                {
                    FileName = Environment.ExpandEnvironmentVariables("%SystemRoot%\\System32\\WindowsPowerShell\\v1.0\\powershell.exe"),
                    Arguments = "iwr -useb https://christitus.com/win | iex",
                    RedirectStandardOutput = false,
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    Verb = "runas"
                };
                var proc1 = Process.Start(prcss);
                proc1.Kill();
                string str = Path.Combine(Path.GetTempPath(), "Debloat.json");
                File.WriteAllBytes(str, MoscoviumTwo.Properties.Resources.Debloat);
                this.RunProcess("irm 'https://christitus.com/win' -OutFile \"$env:TEMP\\winutil.ps1\"");
                Thread.Sleep(1500);
                this.RunProcess("& \"$env:TEMP\\winutil.ps1\" -Config '[path-to-your-config]' -Run".Replace("[path-to-your-config]", str));
            }
            else
            {
                string[] args = {"-Silent",
        "-RemoveApps",
        "-RemoveGamingApps",
        "-DisableTelemetry",
        "-DisableBing",
        "-DisableSuggestions",
        "-DisableLockscreenTips",
        "-RevertContextMenu",
        "-TaskbarAlignLeft",
        "-HideSearchTb",
        "-DisableWidgets",
        "-DisableCopilot",
        "-ClearStartAllUsers",
        "-DisableDVR",
        "-DisableStartRecommended",
        "-ExplorerToThisPC",
        "-DisableMouseAcceleration",
        "-DisableDesktopSpotlight",
        "-DisableSettings365Ads",
        "-DisableSettingsHome",
        "-DisablePaintAI",
        "-DisableNotepadAI",
        "-DisableStickyKeys"};
                string arguments = "&([scriptblock]::Create((irm \"https://debloat.raphi.re/\"))) -RunDefaults" + string.Join(" ", args);
                Process.Start(new ProcessStartInfo()
                {
                    FileName = Environment.ExpandEnvironmentVariables("%SystemRoot%\\System32\\WindowsPowerShell\\v1.0\\powershell.exe"),
                    Arguments = arguments,
                    RedirectStandardOutput = false,
                    UseShellExecute = true,
                    CreateNoWindow = false,
                    Verb = "runas"
                });
                var prcss = new ProcessStartInfo()
                {
                    FileName = Environment.ExpandEnvironmentVariables("%SystemRoot%\\System32\\WindowsPowerShell\\v1.0\\powershell.exe"),
                    Arguments = "iwr -useb https://christitus.com/win | iex",
                    RedirectStandardOutput = false,
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    Verb = "runas"
                };
                var proc1 = Process.Start(prcss);
                proc1.Kill();
                
                
                
                string str = Path.Combine(Path.GetTempPath(), "Debloat.json");
                File.WriteAllBytes(str, MoscoviumTwo.Properties.Resources.Debloat);
                this.RunProcess("irm 'https://christitus.com/win' -OutFile \"$env:TEMP\\winutil.ps1\"");
                Thread.Sleep(1500);
                this.RunProcess("& \"$env:TEMP\\winutil.ps1\" -Config '[path-to-your-config]' -Run".Replace("[path-to-your-config]", str));
            }
        }

        private void pictureBox6_MouseDown(object sender, MouseEventArgs e)
        {
            pictureBox6.Image = Properties.Resources.ButtonClickWallpaper;
        }

        private void pictureBox6_MouseEnter(object sender, EventArgs e)
        {
            if (pictureBox6.Image != Properties.Resources.ButtonClickWallpaper)
            {
                pictureBox6.Image = Properties.Resources.ButtonHoverWallpaper;
            }
        }

        private void pictureBox6_MouseLeave(object sender, EventArgs e)
        {
            pictureBox6.Image = Properties.Resources.ButtonWallpaper;
        }

        private void pictureBox6_MouseUp(object sender, MouseEventArgs e)
        {
            pictureBox6.Image = Properties.Resources.ButtonWallpaper;
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            TaskDialogRadioButton taskDialogRadio1 = new TaskDialogRadioButton();
            taskDialogRadio1.Text = "Fill";
            TaskDialogRadioButton taskDialogRadio2 = new TaskDialogRadioButton();
            taskDialogRadio2.Text = "Fit";
            TaskDialogRadioButton taskDialogRadio3 = new TaskDialogRadioButton();
            taskDialogRadio3.Text = "Strech";
            TaskDialogRadioButton taskDialogRadio4 = new TaskDialogRadioButton();
            taskDialogRadio4.Text = "Tile";
            TaskDialogRadioButton taskDialogRadio5 = new TaskDialogRadioButton();
            taskDialogRadio5.Text = "Center";
            TaskDialogRadioButton taskDialogRadio6 = new TaskDialogRadioButton();
            taskDialogRadio6.Text = "Span";
            TaskDialogPage page = new TaskDialogPage();
            page.RadioButtons.Add(taskDialogRadio1);
            page.RadioButtons.Add(taskDialogRadio2);
            page.RadioButtons.Add(taskDialogRadio3);
            page.RadioButtons.Add(taskDialogRadio4);
            page.RadioButtons.Add(taskDialogRadio5);
            page.RadioButtons.Add(taskDialogRadio6);
            TaskDialog.ShowDialog(this, page);

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            var selected = page.RadioButtons.FirstOrDefault(r => r.Checked);
            if (selected != null)
            {
                openFileDialog1.Filter = "Image Files (*.bmp;*.jpg;*.jpeg;*.png)|*.bmp;*.jpg;*.jpeg;*.png";
                if (openFileDialog1.ShowDialog() != DialogResult.OK)
                    return;
                Wallpaper.Set(openFileDialog1.FileName, selected.Text);
            }
            else
            {
                MessageBox.Show("Choose a fitting option");
            }
        }

        private void pictureBox4_MouseDown(object sender, MouseEventArgs e)
        {
            pictureBox4.Image = Properties.Resources.ButtonClickYabo;
        }

        private void pictureBox4_MouseEnter(object sender, EventArgs e)
        {
            if (pictureBox4.Image != Properties.Resources.ButtonClickYabo)
            {
                pictureBox4.Image = Properties.Resources.ButtonHoverYabo;
            }
        }

        private void pictureBox4_MouseLeave(object sender, EventArgs e)
        {
            pictureBox4.Image = Properties.Resources.ButtonYabo;
        }

        private void pictureBox4_MouseUp(object sender, MouseEventArgs e)
        {
            pictureBox4.Image = Properties.Resources.ButtonYabo;
        }

        private async void pictureBox4_Click(object sender, EventArgs e)
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
                int num = (int)MessageBox.Show("Could not find CS:GO cfg folder.");
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
                        int num = (int)MessageBox.Show("yabosen.cfg has been downloaded to " + str);
                    }
                }
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show("Error: " + ex.Message);
            }
        }

        private async void panel1_DragDrop(object sender, DragEventArgs e)
        {
            string[] strArray = (string[])e.Data.GetData(DataFormats.FileDrop);
            for (int index = 0; index < strArray.Length; ++index)
            {
                string str1 = strArray[index];
                if (str1.Contains(".cfg"))
                {
                    int num1 = (int)MessageBox.Show(str1);
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
                        int num2 = (int)MessageBox.Show("Could not find CS:GO cfg folder.");
                        return;
                    }
                    await this.GetFile(folders, str1);
                }
                else
                {
                    int num = (int)MessageBox.Show("Not a .cfg file");
                    return;
                }
            }
            strArray = (string[])null;
        }

        private async void panel1_DragEnter(object sender, DragEventArgs e)
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
                    int num = (int)MessageBox.Show(filename + " has been downloaded to " + str);
                }
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void pictureBox5_MouseDown(object sender, MouseEventArgs e)
        {
            pictureBox5.Image = Properties.Resources.ButtonClickRetard;
        }

        private void pictureBox5_MouseEnter(object sender, EventArgs e)
        {
            if (pictureBox5.Image != Properties.Resources.ButtonClickRetard)
            {
                pictureBox5.Image = Properties.Resources.ButtonHoverRetard;
            }
        }

        private void pictureBox5_MouseLeave(object sender, EventArgs e)
        {
            pictureBox5.Image = Properties.Resources.ButtonRetard;
        }

        private void pictureBox5_MouseUp(object sender, MouseEventArgs e)
        {
            pictureBox5.Image = Properties.Resources.ButtonRetard;
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            foreach (Process process in Process.GetProcessesByName("explorer"))
            {
                process.Kill();
                Process.Start("C:\\Windows\\explorer.exe");
            }
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Product of Unknown Cyberia, \nIdea by Yabosen, \nCode by Akumarin/Bendy(Megamer Studios)", "Moscovium", MessageBoxButtons.OK, MessageBoxIcon.Information);
            AboutBox1 aboutBox1 = new AboutBox1();
            aboutBox1.ShowDialog();
        }

        private void pictureBox8_MouseDown(object sender, MouseEventArgs e)
        {
            pictureBox8.Image = Properties.Resources.ButtonClickRuntime;
        }

        private void pictureBox8_MouseEnter(object sender, EventArgs e)
        {

            if (pictureBox8.Image != Properties.Resources.ButtonClickRuntime)
            {
                pictureBox8.Image = Properties.Resources.ButtonHoverRuntime;
            }
        }

        private void pictureBox8_MouseLeave(object sender, EventArgs e)
        {
            pictureBox8.Image = Properties.Resources.ButtonRuntime;
        }

        private void pictureBox8_MouseUp(object sender, MouseEventArgs e)
        {
            pictureBox8.Image = Properties.Resources.ButtonRuntime;
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            string ZIP = Path.Combine(Path.GetTempPath(), "VisualCResometing.zip");
            File.WriteAllBytes(ZIP, MoscoviumTwo.Properties.Resources.VisualC);
            string Paeth = Path.Combine(Path.GetTempPath(), "VisualCDstributables");
            ZipFile.ExtractToDirectory(ZIP, Paeth);
            Process.Start(Path.Combine(Paeth, "install_all.bat"));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string DRT = Path.Combine(Path.GetTempPath(), "dry_run_test.ps1");
            File.WriteAllBytes(DRT, MoscoviumTwo.Properties.Resources.dry_run_test);
            Process.Start(new ProcessStartInfo()
            {
                FileName = Environment.ExpandEnvironmentVariables("%SystemRoot%\\System32\\WindowsPowerShell\\v1.0\\powershell.exe"),
                Arguments = "-NoProfile -ExecutionPolicy Bypass -NoExit -File \"" + DRT + "\"",
                RedirectStandardOutput = false,
                UseShellExecute = true,
                CreateNoWindow = false,
             
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
                if (style == "Fit")
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
                if (style == "Strech")
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
                if (style == "Tile")
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
                if (style == "Center")
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
                if (style == "Span")
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



