using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DetectCapsLock
{
    public partial class Form1 : Form, IMessageFilter
    {
        Icon red = Icon.FromHandle(Properties.Resources.red.GetHicon());
        Icon green = Icon.FromHandle(Properties.Resources.green.GetHicon());
        #region drag without title - varaibles
        //source: https://stackoverflow.com/questions/23966253/moving-form-without-title-bar

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private HashSet<Control> controlsToMove = new HashSet<Control>();
        #endregion

        NotifyIcon trayIcon;
        public Form1()
        {
            InitializeComponent();
            InitializeComponentsForMe();
        }

        private void TrayIcon_Click(object sender, EventArgs e)
        {
                this.Visible = !this.Visible;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            trayIcon = new NotifyIcon();
            trayIcon.Icon = red;
            trayIcon.Visible = true;  // Shows the notify icon in the system tray
            trayIcon.ContextMenuStrip = contextMenuStrip1;
            var exitItem = contextMenuStrip1.Items.Add("Exit");
            exitItem.Click += Exit;

            trayIcon.Click += TrayIcon_Click;

            Application.AddMessageFilter(this);

            controlsToMove.Add(this);
            Manager.start();

            #region Set Update
            Timer UpdateTimer;
            UpdateTimer = new Timer();
            UpdateTimer.Tick += new EventHandler(update);
            UpdateTimer.Interval = 10; // in miliseconds
            UpdateTimer.Start();
            #endregion
        }
        void Exit(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            trayIcon.Visible = false;

            Application.Exit();
        }
        #region drag without title
        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDOWN &&
                 controlsToMove.Contains(Control.FromHandle(m.HWnd)))
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                return true;
            }
            return false;
        }
        #endregion

        void update(object sender, EventArgs e)
        {
            this.TopMost = true;
            if (Control.IsKeyLocked(Keys.CapsLock))
            {
                //caps lock on
                this.BackColor = Color.Green;
                trayIcon.Icon = green;
            }
            else
            {
                //caps lock off
                this.BackColor = Color.Red;
                trayIcon.Icon = red;
            }
        }
    }
}
