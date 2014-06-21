using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace GraphDLL
{
    internal sealed partial class GForm : Form
    {
        public const string by = "Matin Lotfali";
        public const string name = "Graphic Box";
        public string About
        {
            get
            {
                FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo("GraphDLL.dll");
                return name + " v" + myFileVersionInfo.FileVersion;
            }
        }
        public static bool fullScreen = false;
        static bool _fullScreen = false, active = false;
        public static int MouseX, MouseY;
        public static bool LeftClick, RightClick, MiddleClick;
        public static char ch;
        public static Keys key;
        public static System.Drawing.Bitmap showform;
        public static bool refreshNeeded = true;
        public static bool activateNeeded = true;

        public static FuncTypes type;

        public static bool answered = false;

        public static void SetFunc(FuncTypes functype)
        {
            answered = false;
            type = functype;
        }

        public KeyMessageFilter myKeys;
        public GForm()
        {
            InitializeComponent();

            myKeys = new KeyMessageFilter();
            if (fullScreen)
                this.fullscreen();
            else
                ClientSize = new Size(Graph.width, Graph.height);
            this.Text = About;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (active && fullScreen != _fullScreen)
            {
                if (fullScreen)
                    fullscreen();
                else
                    windowScreen();
            }


            if (!answered)
            {
                switch (type)
                {
                    case FuncTypes.refresh: RefreshBitmap(); break;
                    case FuncTypes.showmouse: Cursor.Show(); break;
                    case FuncTypes.hidemouse: Cursor.Hide(); break;
                }
                answered = true;
            }
            if (type == FuncTypes.close)
                Close();
            if (refreshNeeded)
                RefreshBitmap();

            if (activateNeeded || Graph.imediateDrawing)
            {
                Activate();
                activateNeeded = false;
            }
        }

        private void GForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Graph.mySocket != null)
                Graph.mySocket.Close();
            if (Graph.mySocket2 != null)
                Graph.mySocket2.Close();
            if (Graph.myClient != null)
                Graph.myClient.Close();
            Hide();
            windowScreen();
            Environment.Exit(0);
        }

        void RefreshBitmap()
        {
            pictureBox1.Image = showform;
            refreshNeeded = false;
            pictureBox1.Update();
        }

        void fullscreen()
        {
            if (!_fullScreen)
            {                
                if (User32.CResolution(Graph.width, Graph.height, out devmod))
                {                    
                    FormBorderStyle = FormBorderStyle.None;
                    WindowState = FormWindowState.Maximized;
                    _fullScreen = true;
                }
                else
                    fullScreen = false;
            }
        }
        
        static DEVMODE1 devmod;
        void windowScreen()
        {
            if (_fullScreen)
            {                
                if (User32.CResolution(ref devmod))
                {
                    _fullScreen = false;
                    FormBorderStyle = FormBorderStyle.Sizable;
                    WindowState = FormWindowState.Normal;
                    ClientSize = new Size(Graph.width, Graph.height);
                }
                else
                    fullScreen = true;
            }
        }

        #region Mouse
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            double c = (double)pictureBox1.Height / Graph.height;
            int l = (int)(pictureBox1.Width - c * Graph.width) / 2;
            GForm.MouseX = (int)((e.X - l) / c);
            GForm.MouseY = (int)(e.Y / c);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left: GForm.LeftClick = true; break;
                case MouseButtons.Right: GForm.RightClick = true; break;
                case MouseButtons.Middle: GForm.MiddleClick = true; break;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left: GForm.LeftClick = false; break;
                case MouseButtons.Right: GForm.RightClick = false; break;
                case MouseButtons.Middle: GForm.MiddleClick = false; break;
            }
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            if (!Graph.showmouse)
                Cursor.Show();
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            if (!Graph.showmouse)
                Cursor.Hide();
        }
        #endregion
        #region Keyboard
        private void GForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            GForm.ch = e.KeyChar;
        }

        private void GForm_KeyDown(object sender, KeyEventArgs e)
        {
            GForm.key = e.KeyCode;

            if (e.Alt && e.KeyCode == Keys.Enter)
            {
                fullScreen = !fullScreen;
                e.SuppressKeyPress = true;
            }
        }
        #endregion

        private void GForm_Deactivate(object sender, EventArgs e)
        {
            active = false;
            windowScreen();
        }

        private void GForm_Activated(object sender, EventArgs e)
        {
            active = true;
        }


    }
}
