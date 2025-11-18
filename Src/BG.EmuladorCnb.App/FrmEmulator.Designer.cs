namespace BG.EmuladorCnb.App
{
    partial class FrmEmulator
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Timer doTimer_new;
        private System.Windows.Forms.Timer stateTimer_new;
        private System.Windows.Forms.Timer reloadTimer_new;
        

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            var currentScreen = Screen.FromPoint(Cursor.Position);

            int x = currentScreen.WorkingArea.Right - (this.Width + 200);
            int y = currentScreen.WorkingArea.Top;

            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmEmulator));
            doTimer_new = new System.Windows.Forms.Timer(components);
            stateTimer_new = new System.Windows.Forms.Timer(components);
            reloadTimer_new = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // doTimer_new
            // 
            doTimer_new.Interval = 1;
            doTimer_new.Tick += doTimer_Tick;
            // 
            // stateTimer_new
            // 
            stateTimer_new.Tick += stateTimer_Tick;
            // 
            // reloadTimer_new
            // 
            reloadTimer_new.Enabled = true;
            reloadTimer_new.Interval = 60000;
            reloadTimer_new.Tick += reloadTimer_Tick;
            // 
            // FrmEmulator
            // 
            BackColor = Color.White;
            ClientSize = new Size(475, 810);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "FrmEmulator";
            StartPosition = FormStartPosition.Manual;
            Location = new Point(x, y);
            Text = "Banco del Barrio | Banco de Guayaquil";
            Load += FrmEmulator_Load;
            KeyPress += FrmEmulator_KeyPress;
            ResumeLayout(false);
        }

        #endregion
    }
}
