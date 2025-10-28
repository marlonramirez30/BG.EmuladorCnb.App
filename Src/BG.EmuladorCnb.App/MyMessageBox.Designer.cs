namespace BG.EmuladorCnb.App
{
    partial class MyMessageBox
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private Label lblMessage;
        private Button btnYes;
        private Button btnNo;
        private Button btnOk;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblMessage = new Label();
            btnYes = new Button();
            btnNo = new Button();
            btnOk = new Button();
            SuspendLayout();
            // 
            // lblMessage
            // 
            lblMessage.Location = new Point(3, 0);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(227, 41);
            lblMessage.TabIndex = 5;
            lblMessage.Text = "Message";
            lblMessage.TextAlign = ContentAlignment.TopCenter;
            // 
            // btnYes
            // 
            btnYes.Location = new Point(34, 44);
            btnYes.Name = "btnYes";
            btnYes.Size = new Size(72, 20);
            btnYes.TabIndex = 1;
            btnYes.Text = "Sí";
            btnYes.Click += btnYes_Click;
            // 
            // btnNo
            // 
            btnNo.Location = new Point(123, 44);
            btnNo.Name = "btnNo";
            btnNo.Size = new Size(72, 20);
            btnNo.TabIndex = 2;
            btnNo.Text = "No";
            btnNo.Click += btnNo_Click;
            // 
            // btnOk
            // 
            btnOk.Location = new Point(81, 44);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(72, 20);
            btnOk.TabIndex = 4;
            btnOk.Text = "Aceptar";
            btnOk.Click += btnOk_Click;
            // 
            // MyMessageBox
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScroll = true;
            ClientSize = new Size(234, 80);
            Controls.Add(btnOk);
            Controls.Add(btnNo);
            Controls.Add(btnYes);
            Controls.Add(lblMessage);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MyMessageBox";
            Text = "Confirmar";
            Load += MyMessageBox_Load;
            ResumeLayout(false);
        }

        #endregion
    }
}