using BG.EmuladorCnb.App.App_Code;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static System.Windows.Forms.AxHost;
using Timer = System.Windows.Forms.Timer;

namespace BG.EmuladorCnb.App.Components
{
    public partial class ImageCarousel : Panel
    {
        private Timer timer;
        private PictureBox pictureBox;
        private XmlNode stateNode;
        private int currentIndex = 0;
        private int rotationTime = 5000;
        private string[] imagePaths;
        private string[] nextStates;
        private CustomPanelClick onClicked;
        private string nextState;
        private Timer parentStateTimer;
        private string directoryPath;
        private int imageWidth;

        public ImageCarousel(XmlNode state, string directoryPath, int imageWidth)
        {
            InitializeComponent(); stateNode = state;
            nextState = string.Empty;
            this.directoryPath = directoryPath;
            this.imageWidth = imageWidth;

            // Obtener tiempo del banner
            if (int.TryParse(stateNode["timeBanner"]?.InnerText, out int time))
                rotationTime = time;

            InitUI();
            // Cargar imágenes
            var banners = stateNode.SelectNodes("settingBanner/banner");
            if (banners != null && banners.Count > 0)
            {
                imagePaths = new string[banners.Count];
                nextStates = new string[banners.Count];
                for (int i = 0; i < banners.Count; i++)
                {
                    imagePaths[i] = banners[i]["image"]?.InnerText ?? "";
                    nextStates[i] = banners[i]["nextState"]?.InnerText ?? "";
                }

                // Mostrar primera imagen
                ShowImage(0);

                // Iniciar rotación
                timer.Interval = rotationTime;
                timer.Start();
            }
        }

        private void InitUI()
        {
            this.nextState = string.Empty;
            pictureBox = new PictureBox();
            pictureBox.Dock = DockStyle.None;
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.Width = this.imageWidth;
            pictureBox.Height = this.Height;
            pictureBox.Cursor = Cursors.Hand;

            pictureBox.Click += new EventHandler(PictureBox_Click);
            Controls.Add(pictureBox);

            timer = new Timer();
            timer.Tick += new EventHandler(Timer_Tick);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (imagePaths == null || imagePaths.Length == 0) return;
            currentIndex = (currentIndex + 1) % imagePaths.Length;
            ShowImage(currentIndex);
        }

        private void ShowImage(int index)
        {
            try
            {
                string path = imagePaths[index];
                if (!path.StartsWith("\\"))
                    path = $"{directoryPath}\\{path}";
                if (System.IO.File.Exists(path))
                { 
                    pictureBox.Image = Image.FromFile(path);
                    this.nextState = nextStates[index];
                }
                else
                {
                    pictureBox.Image = null;
                    this.nextState = string.Empty;
                }
            }
            catch
            {
                pictureBox.Image = null;
                this.nextState = string.Empty;
            }
        }

        public string NextState
        {
            get => this.nextState;
            set => this.nextState = value;
        }

        public CustomPanelClick OnClickMethod
        {
            set => this.onClicked = value;
        }

        private void PictureBox_Click(object sender, EventArgs e) => this.onClicked((Panel)this);
    }
}
