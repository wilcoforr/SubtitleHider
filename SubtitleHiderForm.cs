
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace SubtitleHider
{

    public partial class SubtitleHiderForm : Form
    {
        // UI controls
        private Button menuButton;
        private SettingsForm? settingsForm;

        // Constants for resizing
        private const int HTLEFT = 10;
        private const int HTRIGHT = 11;
        private const int HTTOP = 12;
        private const int HTTOPLEFT = 13;
        private const int HTTOPRIGHT = 14;
        private const int HTBOTTOM = 15;
        private const int HTBOTTOMLEFT = 16;
        private const int HTBOTTOMRIGHT = 17;
        private const int HTCAPTION = 2;

        private const int WM_NCHITTEST = 0x84;

        public SubtitleHiderForm()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.None;
            //this.BackColor = Color.Lime;            // Key color
            //this.TransparencyKey = Color.Lime;      // Make lime transparent
            this.Opacity = 0.8;                     // Adjust bar transparency
            this.TopMost = true;                    // Optional: always on top
            this.StartPosition = FormStartPosition.CenterScreen;

            this.BackColor = Color.Gray;


            // Resize to current screen width when the form is shown
            //this.Width = 1080;
            this.Load += Form1_Load;
            this.SizeChanged += (_, _) => PositionMenuButton();
            this.Height = 80;

            // Menu button (opens settings form)
            menuButton = new Button();
            menuButton.Text = "Menu";
            menuButton.Width = 60;
            menuButton.Height = 24;
            menuButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            menuButton.Click += MenuButton_Click;
            this.Controls.Add(menuButton);

            PositionMenuButton();
        }

        // Hit test override for resizing
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_NCHITTEST)
            {
                int grip = 10; // Resize border thickness
                Point cursor = PointToClient(Cursor.Position);

                bool left = cursor.X <= grip;
                bool right = cursor.X >= Width - grip;
                bool top = cursor.Y <= grip;
                bool bottom = cursor.Y >= Height - grip;

                if (left && top) m.Result = (IntPtr)HTTOPLEFT;
                else if (right && top) m.Result = (IntPtr)HTTOPRIGHT;
                else if (left && bottom) m.Result = (IntPtr)HTBOTTOMLEFT;
                else if (right && bottom) m.Result = (IntPtr)HTBOTTOMRIGHT;

                else if (left) m.Result = (IntPtr)HTLEFT;
                else if (right) m.Result = (IntPtr)HTRIGHT;
                else if (top) m.Result = (IntPtr)HTTOP;
                else if (bottom) m.Result = (IntPtr)HTBOTTOM;

                // Drag form by clicking anywhere
                else
                {
                    // Don't turn clicks on child controls (eg, the Menu button) into window drags.
                    var child = GetChildAtPoint(cursor);
                    if (child == null)
                    {
                        m.Result = (IntPtr)HTCAPTION;
                    }
                }
            }
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
            try
            {
                var scr = Screen.FromControl(this);
                // Use the working area to avoid covering taskbar
                this.Left = scr.WorkingArea.Left;
                this.Width = scr.WorkingArea.Width;
                PositionMenuButton();
            }
            catch
            {
                // ignore failures and keep default size
            }
        }

        private void PositionMenuButton()
        {
            if (menuButton == null) return;
            int margin = 10;
            menuButton.Location = new Point(Math.Max(margin, ClientSize.Width - menuButton.Width - margin), 10);
        }

        private void MenuButton_Click(object? sender, EventArgs e)
        {
            if (settingsForm == null || settingsForm.IsDisposed)
            {
                settingsForm = new SettingsForm(this);
            }

            // Show as a tool window and keep it in front of the bar.
            settingsForm.StartPosition = FormStartPosition.Manual;
            settingsForm.Location = new Point(Left + Width - settingsForm.Width - 10, Top + Height + 10);
            settingsForm.Show();
            settingsForm.BringToFront();
        }

        internal void ApplySettings(double opacity, Color color)
        {
            if (opacity >= 0.0 && opacity <= 1.0) this.Opacity = opacity;
            this.BackColor = color;
        }
    }

}


