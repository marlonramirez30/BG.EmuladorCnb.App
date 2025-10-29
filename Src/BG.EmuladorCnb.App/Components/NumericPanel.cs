using BG.EmuladorCnb.App.App_Code;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timer = System.Windows.Forms.Timer;

namespace BG.EmuladorCnb.App.Components
{
    public partial class NumericPanel: Panel
    {
        private float minValue;
        private float maxValue;
        private string bufferName;
        private Label labelBuffer;
        private Button okButton;
        private Button retryButton;
        private CustomPanelClick onClicked;
        private string nextState;
        private Timer parentStateTimer;

        public NumericPanel(
          int width,
          int height,
          bool withDecimalButton,
          int widthButton,
          int heightButton,
          int yOkButton,
          int yRetryButton,
          string okButtonText,
          string retryButtonText)
        {
            this.Width = width;
            this.Height = height;
            this.minValue = 0.0f;
            this.maxValue = 0.0f;
            this.bufferName = "ammount";
            this.okButton = new Button();
            this.okButton.Width = widthButton;
            this.okButton.Height = heightButton;
            this.okButton.Top = yOkButton;
            this.okButton.Left = this.Right - widthButton;
            this.okButton.Text = okButtonText;
            this.okButton.Click += new EventHandler(this.okButton_Click);
            this.okButton.KeyPress += new KeyPressEventHandler(this.button_KeyPress);
            this.okButton.BackColor = Color.FromArgb(163, 26, 97);
            this.okButton.ForeColor = Color.White;
            this.okButton.Font = new Font("Segoe UI", 14);
            this.okButton.Cursor = Cursors.Hand;
            this.Controls.Add((Control)this.okButton);
            this.retryButton = new Button();
            this.retryButton.Width = widthButton;
            this.retryButton.Height = heightButton;
            this.retryButton.Top = yRetryButton;
            this.retryButton.Left = this.Right - widthButton;
            this.retryButton.Text = retryButtonText;
            this.retryButton.Click += new EventHandler(this.retryButton_Click);
            this.retryButton.KeyPress += new KeyPressEventHandler(this.button_KeyPress);
            this.retryButton.BackColor = Color.Silver;
            this.retryButton.ForeColor = Color.Black;
            this.retryButton.Cursor = Cursors.Hand;
            this.retryButton.Font = new Font("Segoe UI", 14);
            this.Controls.Add((Control)this.retryButton);
            this.labelBuffer = new Label();

            this.labelBuffer.BackColor = Color.White;
            this.labelBuffer.ForeColor = Color.Black;
            this.labelBuffer.Cursor = Cursors.IBeam;
            this.labelBuffer.Width = this.Width - 80;
            this.labelBuffer.Height = 50;
            this.labelBuffer.Left = (this.ClientSize.Width - this.labelBuffer.Width) / 2;
            this.labelBuffer.Top = 6;
            this.labelBuffer.Font = new Font("Segoe UI", 16);
            // Borde color magenta personalizado
            labelBuffer.Paint += (s, e) =>
            {
                using (Pen pen = new Pen(Color.FromArgb(163, 26, 97), 2))
                {
                    Rectangle rect = new Rectangle(0, 2, labelBuffer.Width - 2, labelBuffer.Height - 2);
                    e.Graphics.DrawRectangle(pen, rect);
                }
            };

            this.labelBuffer.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add((Control)this.labelBuffer);
            this.addNumericButtons(withDecimalButton);
        }

        private void addNumericButtons(bool withDecimalButton)
        {
            int num1 = this.Width / 3 * 2;
            int num2 = this.Height / 4 * 3;
            int num3 = num1 / 3;
            int num4 = num2 / 4;
            int num5 = 0;
            int num6 = this.Height / 4;
            int num7 = num5;
            int num8 = num6;
            for (int index = 1; index <= 9; ++index)
            {
                Button button = new Button();
                button.Left = num7;
                button.Top = num8;
                button.Width = num3;
                button.Height = num4;
                button.BackColor = Color.DarkBlue;
                button.ForeColor = Color.White;
                button.Cursor = Cursors.Hand;
                button.Font = new Font("Segoe UI", 14);
                button.Text = index.ToString();
                button.Click += new EventHandler(this.numericButton_Click);
                button.KeyPress += new KeyPressEventHandler(this.button_KeyPress);
                this.Controls.Add((Control)button);
                num7 += num3;
                if (index % 3 == 0)
                {
                    num7 = num5;
                    num8 += num4;
                }
            }
            Button button1 = new Button();
            button1.Left = num7;
            button1.Top = num8;
            button1.Width = num3;
            button1.Height = num4;
            button1.BackColor = Color.DarkGreen;
            button1.ForeColor = Color.White;
            button1.Cursor = Cursors.Hand;
            button1.Font = new Font("Segoe UI", 14);
            button1.Text = "<-";
            button1.Click += new EventHandler(this.numericButton_Click);
            button1.KeyPress += new KeyPressEventHandler(this.button_KeyPress);
            this.Controls.Add((Control)button1);
            Button button2 = new Button();
            button2.Left = num7 + num3;
            button2.Top = num8;
            button2.Width = num3;
            button2.Height = num4;
            button2.BackColor = Color.DarkBlue;
            button2.ForeColor = Color.White;
            button2.Cursor = Cursors.Hand;
            button2.Font = new Font("Segoe UI", 14);
            button2.Text = "0";
            button2.Click += new EventHandler(this.numericButton_Click);
            button2.KeyPress += new KeyPressEventHandler(this.button_KeyPress);
            this.Controls.Add((Control)button2);
            if (withDecimalButton)
            {
                Button button3 = new Button();
                button3.Top = num8;
                button3.Left = num7 + 2 * num3;
                button3.Width = num3;
                button3.Height = num4;
                button3.BackColor = Color.DarkGreen;
                button3.ForeColor = Color.White;
                button3.Font = new Font("Segoe UI", 14);
                button3.Cursor = Cursors.Hand;
                button3.Text = ".";
                button3.Click += new EventHandler(this.numericButton_Click);
                button3.KeyPress += new KeyPressEventHandler(this.button_KeyPress);
                this.Controls.Add((Control)button3);
            }
        }

        private void button_KeyPress(object sender, KeyPressEventArgs e)
        {
            int keyChar = (int)e.KeyChar;
            if ((keyChar < 48 /*0x30*/ || keyChar > 57) && keyChar != 46 && keyChar != 8)
                return;
            string upper = e.KeyChar.ToString().ToUpper();
            Button sender1 = (Button)null;
            if (keyChar == 8)
                upper = "<-";
            int index = 0;
            while (index < this.Controls.Count && (sender1 = !(this.Controls[index] is Button) || !(this.Controls[index].Text == upper) ? (Button)null : (Button)this.Controls[index]) == null)
                ++index;
            if (sender1 == null)
                return;
            this.numericButton_Click((object)sender1, (EventArgs)null);
        }

        public string BufferName
        {
            get => this.bufferName;
            set => this.bufferName = value;
        }

        public float MinValue
        {
            get => this.minValue;
            set => this.minValue = value;
        }

        public float MaxValue
        {
            get => this.maxValue;
            set => this.maxValue = value;
        }

        public void numericButton_Click(object sender, EventArgs e)
        {
            this.parentStateTimer.Enabled = false;
            string text = ((Control)sender).Text;
            if (text[0] == '<')
            {
                if (this.labelBuffer.Text.Length > 0)
                    this.labelBuffer.Text = this.labelBuffer.Text.Substring(0, this.labelBuffer.Text.Length - 1);
            }
            else if (text == ".")
            {
                if (this.labelBuffer.Text.IndexOf(".") < 0)
                {
                    if (this.labelBuffer.Text.Length > 0)
                        this.labelBuffer.Text += text;
                    else
                        this.labelBuffer.Text = "0.";
                }
            }
            else if (this.labelBuffer.Text.IndexOf(".") >= 0)
            {
                if (this.labelBuffer.Text.Length - this.labelBuffer.Text.IndexOf(".") <= 2)
                    this.labelBuffer.Text += text;
            }
            else
                this.labelBuffer.Text += text;
            this.parentStateTimer.Enabled = true;
        }

        private void retryButton_Click(object sender, EventArgs e)
        {
            this.parentStateTimer.Enabled = false;
            this.labelBuffer.Text = "";
            this.parentStateTimer.Enabled = true;
        }

        public CustomPanelClick OnClickMethod
        {
            set => this.onClicked = value;
        }

        private void okButton_Click(object sender, EventArgs e) => this.onClicked((Panel)this);

        public string BufferValue
        {
            get
            {
                string bufferValue;
                if (this.labelBuffer.Text.Length <= 0)
                    bufferValue = "0";
                else
                    bufferValue = this.labelBuffer.Text.TrimEnd('.');
                return bufferValue;
            }
            set => this.labelBuffer.Text = value;
        }

        public bool validate()
        {
            string str;
            if (this.labelBuffer.Text.Length <= 0)
                str = "0";
            else
                str = this.labelBuffer.Text.TrimEnd('.').Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            string s = str;
            bool flag = false;
            try
            {
                if ((double)float.Parse(s) >= (double)this.minValue)
                {
                    if ((double)this.maxValue > 0.0)
                    {
                        if ((double)float.Parse(s) <= (double)this.maxValue)
                            flag = true;
                    }
                    else
                        flag = true;
                }
            }
            catch (Exception ex)
            {
                flag = false;
            }
            return flag;
        }

        public string NextState
        {
            get => this.nextState;
            set => this.nextState = value;
        }

        public Timer ParentStateTimer
        {
            get => this.parentStateTimer;
            set => this.parentStateTimer = value;
        }
    }
}
