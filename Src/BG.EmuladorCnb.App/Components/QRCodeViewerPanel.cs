using QRCoder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BG.EmuladorCnb.App.Components
{
    public partial class QRCodeViewerPanel: Panel
    {
        private PictureBox pictureBox;
        private string _codigo;

        public QRCodeViewerPanel()
        {
            InitializeComponent();
            InicializarUI();
        }

        private void InicializarUI()
        {
            pictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White
            };
            this.Controls.Add(pictureBox);
        }

        public string Codigo
        {
            get => _codigo;
            set
            {
                _codigo = value;
                GenerarCodigoQR(_codigo);
            }
        }

        private void GenerarCodigoQR(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                pictureBox.Image = null;
                return;
            }

            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(texto, QRCodeGenerator.ECCLevel.Q))
            using (QRCode qrCode = new QRCode(qrCodeData))
            {
                Bitmap qrImage = qrCode.GetGraphic(20, Color.FromArgb(163, 26, 97), Color.White, true);
                pictureBox.Image = qrImage;
            }
        }
    }
}
