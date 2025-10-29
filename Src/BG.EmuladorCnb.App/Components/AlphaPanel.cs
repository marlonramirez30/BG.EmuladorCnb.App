using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BG.EmuladorCnb.App.App_Code;
using Timer = System.Windows.Forms.Timer;

namespace BG.EmuladorCnb.App.Components
{
    public partial class AlphaPanel : Panel
    {
        private Button okButton;
        private Button retryButton;
        private Label labelBuffer;
        private Timer parentStateTimer;
        private string bufferName;
        private int maxLength;
        private CustomPanelClick onClicked;
        private string nextState;
        private char[] letter = new char[38]
        {
            '1',
            '2',
            '3',
            '4',
            '5',
            '6',
            '7',
            '8',
            '9',
            '0',
            'A',
            'B',
            'C',
            'D',
            'E',
            'F',
            'G',
            'H',
            'I',
            'J',
            'K',
            'L',
            'M',
            'N',
            'O',
            'P',
            'Q',
            'R',
            'S',
            'T',
            'U',
            'V',
            'W',
            'X',
            'Y',
            'Z',
            '>',
            '<'
        };
        public AlphaPanel(
          int width,
          int height,
          int widthButton,
          int heightButton,
          int yOkButton,
          int yRetryButton,
          string okButtonText,
          string retryButtonText)
        {
            this.Width = width;
            this.Height = height;
            this.bufferName = "buffer";
            this.maxLength = 40;
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
            this.okButton.Cursor = Cursors.Hand;
            this.okButton.Font = new Font("Segoe UI", 14);
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
            this.addAlphaButtons();
        }

        private void addAlphaButtons()
        {
            Button buttonEnter = new Button();
            buttonEnter.Text = "Enter";
            buttonEnter.Visible = false;
            this.Controls.Add((Control)buttonEnter);
            int num1 = this.Width / 3 * 2;
            int num2 = this.Height / 3 * 2;
            int num3 = num1 / 10;
            int num4 = num2 / 4;
            int num5 = 0;
            int num6 = this.Height / 3;
            int num7 = num5;
            int num8 = num6;
            for (int index = 1; index <= this.letter.Length; ++index)
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
                if (this.letter[index - 1] == '>')
                {
                    button.BackColor = Color.DarkGreen;
                    button.ForeColor = Color.White;
                    button.Text = "->"; 
                }
                else if (this.letter[index - 1] == '<')
                {
                    button.BackColor = Color.DarkGreen;
                    button.ForeColor = Color.White;
                    button.Text = "<-"; 
                }
                else if (isNumeric(this.letter[index - 1].ToString() ?? ""))
                {
                    button.BackColor = Color.DarkGreen;
                    button.ForeColor = Color.White;
                    button.Text = this.letter[index - 1].ToString() ?? "";
                }
                else
                    button.Text = this.letter[index - 1].ToString() ?? ""; 
                button.Click += new EventHandler(this.alphaButton_Click);
                button.KeyPress += new KeyPressEventHandler(this.button_KeyPress);
                this.Controls.Add((Control)button);
                num7 += num3;
                if (index % 10 == 0)
                {
                    num7 = num5;
                    num8 += num4;
                }
            }
        }

        private bool isNumeric(string alphaText)
        {
            foreach (char c in alphaText)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }

        private void button_KeyPress(object sender, KeyPressEventArgs e)
        {
            int keyChar = (int)e.KeyChar;
            if ((keyChar < 48 || keyChar > 57) && (keyChar < 65 || keyChar > 90) && (keyChar < 97 || keyChar > 122) && keyChar != 8 && keyChar != 46 && keyChar != 32 && keyChar != 13)
                return;
            string str = e.KeyChar.ToString().ToUpper();
            Button sender1 = (Button)null;
            if (keyChar == 13)
                str = "Enter";
            if (keyChar == 8)
                str = "<-";
            if (keyChar == 32)
                str = "->";
            int index = 0;
            while (index < this.Controls.Count && (sender1 = !(this.Controls[index] is Button) || !(this.Controls[index].Text == str) ? (Button)null : (Button)this.Controls[index]) == null)
                ++index;
            if (sender1 == null)
                return;
            this.alphaButton_Click((object)sender1, (EventArgs)null);
        }

        public string BufferName
        {
            get => this.bufferName;
            set => this.bufferName = value;
        }

        public void alphaButton_Click(object sender, EventArgs e)
        {
            this.parentStateTimer.Enabled = false;
            string text = ((Control)sender).Text;
            if (text.ToLower() == "enter")
            {
                okButton_Click(this.okButton, e);
            }
            else if (text[0] == '<')
            {
                if (this.labelBuffer.Text.Length > 0)
                    this.labelBuffer.Text = this.labelBuffer.Text.Substring(0, this.labelBuffer.Text.Length - 1);
            }
            else if (this.labelBuffer.Text.Length < this.maxLength)
            {
                if (text[0] == '-')
                    this.labelBuffer.Text += ' ';
                else
                    this.labelBuffer.Text += text;
            }
            this.parentStateTimer.Enabled = true;
        }

        private void retryButton_Click(object sender, EventArgs e)
        {
            this.parentStateTimer.Enabled = true;
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
            get => this.labelBuffer.Text;
            set => this.labelBuffer.Text = value;
        }

        public string NextState
        {
            get => this.nextState;
            set => this.nextState = value;
        }

        public int MaxLength
        {
            set => this.maxLength = value;
        }

        public Timer ParentStateTimer
        {
            get => this.parentStateTimer;
            set => this.parentStateTimer = value;
        }
    }
}
