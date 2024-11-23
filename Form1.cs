using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using memorywriterx.Properties;
using memorywriterx;
using Microsoft.Web.WebView2.WinForms;
using System.Diagnostics;
using ForlornApi;

namespace memorywriter_winforms
{
    public partial class Form1 : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
            (
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthElipise,
            int nHeightElipise
            );

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void SetButtonImagePadding(Button button, Image resourceImage, int padding)
        {
            int newWidth = button.Width - (padding * 2);
            int newHeight = button.Height - (padding * 2);

            if (newWidth <= 0 || newHeight <= 0) return;

            Image resizedImage = new Bitmap(resourceImage, new Size(newWidth, newHeight));

            button.Image = resizedImage;
            button.ImageAlign = ContentAlignment.MiddleCenter;
        }
        public string api;

        public Form1()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 16, 16));
            api = "CX";
        }
        private void MonitorProcess(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length > 0)
            {
                var process = processes[0];
                process.EnableRaisingEvents = true;

                process.Exited += (s, e) =>
                {
                    this.Invoke((MethodInvoker)(() =>
                    {
                        label2.Text = "Not Injected";
                    }));
                };
            }
            else
            {
                label2.Text = "Not Injected";
            }
        }

        Point lastPoint;

        private void Form1_Load(object sender, EventArgs e)
        {
            AllocConsole();
            this.monacoEditor.Source = new Uri(Path.Combine(Application.StartupPath, @"Monaco\index.html"));
            Console.WriteLine("ForlornAPI Version:    " + ForlornApi.Forlorn.ForlornVersion);
            Console.WriteLine("cxapi Version:         " + cxapi.cxapi.cxapiVersion);
            Console.WriteLine("moon Version:          1.0b.3");
            Console.WriteLine("Current API:           " + api);
            // execute button
            SetButtonImagePadding(button1, Resources.bxs_right_arrow, 8);
            // clear button
            SetButtonImagePadding(button4, Resources.bxs_eraser, 8);
            // open file button
            SetButtonImagePadding(button5, Resources.bx_folder_open, 8);
            // save file button
            SetButtonImagePadding(button6, Resources.bxs_save, 8);
            // kill roblox button
            SetButtonImagePadding(button8, Resources.bx_x, 4);
            // inject button
            SetButtonImagePadding(button7, Resources.bx_injection, 6);
            // close button
            SetButtonImagePadding(button2, Resources.bx_x, 4);
            // minimize button
            SetButtonImagePadding(button3, Resources.bx_minus, 6);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        private async void button1_Click(object sender, EventArgs e)
        {
            if (api == "Forlorn")
            {
                if (ForlornApi.Api.IsInjected())
                {
                    var result = await monacoEditor.ExecuteScriptAsync("window.editor.getValue();");
                    string realResult = JsonConvert.DeserializeObject<string>(result);
                    if (realResult == null)
                    {
                        realResult = string.Empty;
                    }
                    Console.WriteLine("INFO: Executing \" " + realResult + " \"");
                    ForlornApi.Api.ExecuteScript(realResult);
                    Console.WriteLine("INFO-SUCCESS: Script executed");
                }
                else
                {
                    MessageBox.Show("Please press Inject before executing.");
                }
            }
            else if (api == "CX")
            {
                if (cxapi.CoreFunctions.IsInjected())
                {
                    var result = await monacoEditor.ExecuteScriptAsync("window.editor.getValue();");
                    string realResult = JsonConvert.DeserializeObject<string>(result);
                    if (realResult == null)
                    {
                        realResult = string.Empty;
                    }
                    Console.WriteLine("INFO: Executing \" " + realResult + " \"");
                    cxapi.CoreFunctions.ExecuteScript(realResult);
                    Console.WriteLine("INFO-SUCCESS: Script executed");
                }
                else
                {
                    MessageBox.Show("Please press Inject before executing.");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e) => Application.Exit();

        private void button3_Click(object sender, EventArgs e) => WindowState = FormWindowState.Minimized;

        private void button7_Click(object sender, EventArgs e)
        {
            if (api == "Forlorn")
            {
                if (ForlornApi.Api.IsInjected())
                {
                    Console.WriteLine("INFO: Injecting");
                    if (ForlornApi.Api.IsRobloxOpen())
                    {
                        label2.Text = "Injecting...";
                        ForlornApi.Api.Inject();
                        Console.WriteLine("INFO-SUCCESS: Injected successfully");
                        MonitorProcess("RobloxPlayerBeta");
                        label2.Text = "Injected";
                        Thread.Sleep(5000);
                        ForlornApi.Api.ExecuteScript("print(\"Loaded ForlornAPI with moon\")");
                    }
                    else
                    {
                        Console.WriteLine("INFO-ERROR: Roblox is not open");
                        MessageBox.Show("Roblox is not open, open roblox before injecting.");
                    }
                }
                else
                {
                    MessageBox.Show("Forlorn already injected.");
                }
            }
            else if (api == "CX")
            {
                if (!cxapi.CoreFunctions.IsInjected())
                {
                    Console.WriteLine("INFO: Injecting");
                    if (cxapi.CoreFunctions.IsRobloxOpen())
                    {
                        label2.Text = "Injecting...";
                        cxapi.CoreFunctions.Inject(true);
                        Console.WriteLine("INFO-SUCCESS: Injected successfully");
                        MonitorProcess("RobloxPlayerBeta");
                        label2.Text = "Injected";
                        Thread.Sleep(5000);
                        cxapi.CoreFunctions.ExecuteScript("print(\"Loaded CXAPI with moon\")");
                    }
                    else
                    {
                        Console.WriteLine("INFO-ERROR: Roblox is not open");
                        MessageBox.Show("Roblox is not open, open roblox before injecting.");
                    }
                }
                else
                {
                    MessageBox.Show("cxapi already injected.");
                }
            }
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            Console.WriteLine("INFO: Trying to clear Monaco...");
            await monacoEditor.ExecuteScriptAsync("window.editor.setValue(\"\");");
            Console.WriteLine("WARNING: Monaco should be cleared.");
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = new Point(e.X, e.Y);
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastPoint.X;
                this.Top += e.Y - lastPoint.Y;
            }
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                openFileDialog1.Title = "Open";
                var value = File.ReadAllText(openFileDialog1.FileName);
                await monacoEditor.ExecuteScriptAsync("window.editor.setValue(\"" + value + "\");");
            }
        }

        private async void button6_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (Stream s = File.Open(saveFileDialog1.FileName, FileMode.CreateNew))
                using (StreamWriter sw = new StreamWriter(s))
                {
                    var value = await monacoEditor.ExecuteScriptAsync("window.editor.getValue();");
                    string realValue = JsonConvert.DeserializeObject<string>(value);
                    sw.Write(realValue);
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (ForlornApi.Api.IsRobloxOpen())
            {
                ForlornApi.Api.KillRoblox();
            }
            else
            {
                MessageBox.Show("Roblox is not open.");
            }
        }
    }
}
