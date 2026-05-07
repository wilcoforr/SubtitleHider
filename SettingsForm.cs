using System;
using System.Drawing;
using System.Windows.Forms;

namespace SubtitleHider
{
    internal sealed class SettingsForm : Form
    {
        private readonly SubtitleHiderForm owner;

        private readonly TrackBar opacitySlider;
        private readonly Label opacityLabel;
        private readonly ComboBox colorDropdown;
        private readonly Button closeButton;

        internal SettingsForm(SubtitleHiderForm owner)
        {
            this.owner = owner;

            Text = "Menu";
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            ShowInTaskbar = false;
            TopMost = owner.TopMost;
            MinimizeBox = false;
            MaximizeBox = false;

            Width = 260;
            Height = 180;

            opacitySlider = new TrackBar();
            opacitySlider.Minimum = 20;
            opacitySlider.Maximum = 100;
            opacitySlider.TickFrequency = 10;
            opacitySlider.SmallChange = 5;
            opacitySlider.LargeChange = 10;
            opacitySlider.Width = 220;
            opacitySlider.Location = new Point(10, 10);
            opacitySlider.Value = Math.Clamp((int)(owner.Opacity * 100), opacitySlider.Minimum, opacitySlider.Maximum);
            opacitySlider.Scroll += (_, _) => Apply();
            Controls.Add(opacitySlider);

            opacityLabel = new Label();
            opacityLabel.AutoSize = true;
            opacityLabel.Location = new Point(10, opacitySlider.Bottom + 6);
            Controls.Add(opacityLabel);

            colorDropdown = new ComboBox();
            colorDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            colorDropdown.Items.AddRange(new object[] { "Black", "Gray", "White" });
            colorDropdown.Width = 100;
            colorDropdown.Location = new Point(10, opacityLabel.Bottom + 8);
            colorDropdown.SelectedIndexChanged += (_, _) => Apply();
            Controls.Add(colorDropdown);

            closeButton = new Button();
            closeButton.Text = "Close Subtitle Hider";
            
            closeButton.Width = 200;
            closeButton.Height = 24;
            closeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            closeButton.Location = new Point((ClientSize.Width - closeButton.Width) / 2, ClientSize.Height - closeButton.Height - 10);
            closeButton.Click += (_, _) => {
                Close();
                owner.Close();
            };

            Controls.Add(closeButton);

            // Initialize selection to current owner BackColor.
            if (owner.BackColor == Color.Black) colorDropdown.SelectedItem = "Black";
            else if (owner.BackColor == Color.Gray) colorDropdown.SelectedItem = "Gray";
            else if (owner.BackColor == Color.White) colorDropdown.SelectedItem = "White";
            else colorDropdown.SelectedIndex = 1;

            UpdateOpacityLabel();
        }

        private void Apply()
        {
            UpdateOpacityLabel();

            double opacity = opacitySlider.Value / 100.0;
            Color color1 = colorDropdown.SelectedItem as string switch
            {
                "Black" => Color.Black,
                "White" => Color.White,
                _ => Color.Gray //default
            };

            //Color color = color1;

            owner.ApplySettings(opacity, color1);
        }

        private void UpdateOpacityLabel()
        {
            opacityLabel.Text = $"Opacity: {opacitySlider.Value}%";
        }
    }
}
