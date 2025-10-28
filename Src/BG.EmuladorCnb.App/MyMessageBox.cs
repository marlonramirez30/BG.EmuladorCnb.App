using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BG.EmuladorCnb.App
{
    public partial class MyMessageBox: Form
    {
        public MyMessageBox()
        {
            InitializeComponent();
        }
        public string TextMessage
        {
            get => this.lblMessage.Text;
            set => this.lblMessage.Text = value;
        }

        public bool IsButtonYesAndNo
        {
            set
            {
                this.btnYes.Visible = value;
                this.btnNo.Visible = value;
                this.btnOk.Visible = !value;
            }
        }

        private void btnYes_Click(object sender, EventArgs e) => this.DialogResult = DialogResult.Yes;

        private void btnNo_Click(object sender, EventArgs e) => this.DialogResult = DialogResult.No;

        private void MyMessageBox_Load(object sender, EventArgs e)
        {
            this.Top = (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2;
            this.Left = (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2;
        }

        private void btnOk_Click(object sender, EventArgs e) => this.DialogResult = DialogResult.OK;
    }
}
