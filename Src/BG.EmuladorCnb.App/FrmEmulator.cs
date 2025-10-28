using HeraclesNbcDevice;
using BG.EmuladorCnb.App.App_Code;
using BG.EmuladorCnb.App.Components;
using System.Collections;
using System.Configuration;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Runtime.InteropServices;
using Configuration = BG.EmuladorCnb.App.App_Code.Configuration;
using Windows.Devices.PointOfService;
using System.Xml.Linq;
using System.Media;

namespace BG.EmuladorCnb.App
{
    public partial class FrmEmulator : Form
    {
        private const int WORKING_BUFFER_SIZE = 4096 /*0x1000*/;
        private const string logFileName = "NbcClientLog";
        private string defaultDirectory;
        private string idState;
        private string initialState;
        private string sendErrorState;
        private string readErrorState;
        private string generalErrorState;
        private string syncErrorState;
        private string reprintState;
        private string handshakeState;
        private string printCopieMessage;
        private string printErrorMessage;
        private XmlState xmlState;
        private StringBuilder tranBuffer;
        private NbcBufferList bufferList;
        private int[,] pos;
        private int buttonWidth;
        private int buttonHeight;
        private int panelWidth;
        private int panelHeight;
        private int xPanel;
        private int yPanel;
        private int yOkButtonPanel;
        private int yRetryButtonPanel;
        private int yFactor;
        private MyTcpClient tcpClient;
        private int defaultTimeout;
        private int sync;
        private string nbcCard;
        private string wKey;
        private string pinpadMessage1;
        private string pinpadMessage2;
        private string setGMAC;
        private int reprintCounter;
        private MyMessageBox myMessageBox;
        private string programVersion = "2.5";
        private string stateVersion;
        private bool hasBanner = false;
        private bool hasCancelButton = false;
        private bool handshaked = false;
        private string pendingPath = Path.Combine(getCurrentDirectory(), "NBCP.txt");
        private MyLog myLog;
        private string HEAD_TEMPERATURE;
        private string PAPER_OUT;
        private string POWER_SUPPLY;
        private string BAD_PRINTER;
        private int freeSpacePercentage;
        private string freeSpacePath;
        private string FREE_SPACE;
        private string moveLogPath;
        private bool mockTarjetaCliente;
        private string MockCnbWorkingKey;
        private string MockCnbPinBuffer;
        private string MockCnbCardNumber;
        private List<string> CamposBufferRemover;
        private string currentDirectory = getCurrentDirectory();

        public FrmEmulator(string idState)
        {
            this.idState = idState;
            this.initialState = idState;
            this.defaultDirectory = getCurrentDirectory();
            this.xmlState = new XmlState(this.defaultDirectory, "States.xml");
            this.tranBuffer = new StringBuilder(4);
            this.bufferList = new NbcBufferList();
            InitializeComponent();
            Configuration configuration = new Configuration(this.defaultDirectory + "\\Configuration.xml");
            this.tcpClient = new MyTcpClient(configuration.getString("Connection", "Ip", "172.26.49.42"), configuration.getInteger("Connection", "Port", 6701));
            this.tcpClient.ReceiveTimeout = configuration.getInteger("Connection", "ReadTimeOut", 70000);
            this.myLog = new MyLog("NbcClientLog");
            this.nbcCard = configuration.getString("General", "AdminCard", "9999999999999999");
            this.wKey = configuration.getString("Security", "WorkingKey", "00000000000000000000000000000000");
            this.pinpadMessage1 = configuration.getString("Advanced", "PinpadMsg1", "1D2000000100000F50617365206C61207461726A657461D594673D");
            this.pinpadMessage2 = configuration.getString("Advanced", "PinpadMsg2", "1D21000001000010496E6772657365206C6120636C61766504452298");
            this.setGMAC = configuration.getString("Advanced", "SetGMAC", "550056CC09E7CFDC4CEF56CC09E7CFDC4CEFD5D44F");
            this.sync = 0;
            CryptClass.InitCryptClass(configuration.getString("Security", "Key", "490B39AAEEB33AEF0242E1D82D467CFF9AB3E1A745AF69CD"), configuration.getString("Security", "Iv", "760BB74A9C0920E8"));
            this.myMessageBox = new MyMessageBox();
            this.freeSpacePercentage = configuration.getInteger("FreeSpace", "Percentage", 95);
            this.freeSpacePath = configuration.getString("FreeSpace", "Path", "\\saio");
            this.moveLogPath = Path.Combine(getCurrentDirectory(), "BackupLog");
            this.mockTarjetaCliente = bool.Parse(configuration.getString("Mock", "SimularTarjetaCliente", "False"));
            this.MockCnbPinBuffer = configuration.getString("Mock", "CnbPinBuffer", "NONE");
            this.MockCnbWorkingKey = configuration.getString("Mock", "CnbWorkingKey", "NONE");
            this.MockCnbCardNumber = configuration.getString("Mock", "CnbCardNumber", "XXXXXXXXXXXXXXXX");
            this.CamposBufferRemover = new List<string>();
            this.CamposBufferRemover.Add("account");
            this.CamposBufferRemover.Add("fullname");
            this.CamposBufferRemover.Add("ammount");
            this.CamposBufferRemover.Add("auxiliar1");
        }

        private void FrmEmulator_Load(object sender, EventArgs e)
        {
            this.initializePos();
            this.initializeConfig();
            this.idState = !this.getPending() ? this.handshakeState : this.reprintState;
            this.doTimer_new.Enabled = true;
        }

        private void FrmEmulator_KeyPress(object sender, KeyPressEventArgs e)
        {
            int keyChar1 = (int)e.KeyChar;
            if ((keyChar1 < 48 /*0x30*/ || keyChar1 > 57) && (keyChar1 < 65 || keyChar1 > 90) && (keyChar1 < 97 || keyChar1 > 122) && keyChar1 != 8 && keyChar1 != 46 && keyChar1 != 32 /*0x20*/)
                return;
            Panel numericOrAlphaPanel = (Panel)null;
            string keyChar2 = e.KeyChar.ToString().ToUpper();
            if (keyChar1 == 8)
                keyChar2 = "<-";
            if (keyChar1 == 32 /*0x20*/)
                keyChar2 = "->";
            Button buttonByText;
            if ((buttonByText = this.findButtonByText(keyChar2, out numericOrAlphaPanel)) == null)
                return;
            if (numericOrAlphaPanel is NumericPanel)
                ((NumericPanel)numericOrAlphaPanel).numericButton_Click((object)buttonByText, (EventArgs)null);
            else
                ((AlphaPanel)numericOrAlphaPanel).alphaButton_Click((object)buttonByText, (EventArgs)null);
        }

        private Button findButtonByText(string keyChar, out Panel numericOrAlphaPanel)
        {
            Button buttonByText = (Button)null;
            numericOrAlphaPanel = (Panel)null;
            int index1 = 0;
            while (index1 < this.Controls.Count && (numericOrAlphaPanel = this.Controls[index1] is NumericPanel || this.Controls[index1] is AlphaPanel ? (Panel)this.Controls[index1] : (Panel)null) == null)
                ++index1;
            if (numericOrAlphaPanel != null)
            {
                int index2 = 0;
                while (index2 < numericOrAlphaPanel.Controls.Count && (buttonByText = !(numericOrAlphaPanel.Controls[index2] is Button) || !(numericOrAlphaPanel.Controls[index2].Text == keyChar) ? (Button)null : (Button)numericOrAlphaPanel.Controls[index2]) == null)
                    ++index2;
            }
            return buttonByText;
        }

        private void doTimer_Tick(object sender, EventArgs e)
        {
            this.doTimer_new.Enabled = false;
            this.addComponents();
        }

        private void reloadTimer_Tick(object sender, EventArgs e)
        {
            this.reloadTimer_new.Enabled = false;
            this.xmlState.reload();
            this.reloadTimer_new.Enabled = true;
        }

        private void initializePos()
        {
            var thisWidth = ((Control)this).Size.Width - 15;
            var thisHeight = ((Control)this).Size.Height - 35;
            this.pos = new int[8, 2];
            this.yFactor = (thisHeight / 8 * 7 / 4);
            this.buttonHeight =(this.yFactor - 30) / 2;
            int num1 = thisHeight / 8 + this.yFactor - this.buttonHeight;
            this.buttonWidth = thisWidth / 3;
            int num2 = thisWidth - this.buttonWidth;
            for (int index = 0; index < 4; ++index)
            {
                this.pos[index, 0] = num2;
                this.pos[index, 1] = num1;
                num1 += this.yFactor;
            }
            int num3 = 0;
            int num4 = thisHeight / 8 + this.yFactor - this.buttonHeight;
            for (int index = 4; index < 8; ++index)
            {
                this.pos[index, 0] = num3;
                this.pos[index, 1] = num4;
                num4 += this.yFactor;
            }
            this.xPanel = 0;
            this.yPanel = (thisHeight / 4);
            this.panelWidth = thisWidth;
            this.panelHeight = thisHeight / 4 * 3;
            this.yOkButtonPanel = this.pos[2, 1] - this.yPanel;
            this.yRetryButtonPanel = this.pos[3, 1] - this.yPanel;
        }

        private void initializeConfig()
        {
            XmlNode stateById = this.xmlState.getStateById("0");
            if (stateById == null)
                return;
            this.addBanner(stateById);
            this.setStateTimeout(stateById);
            this.setSpecialStates(stateById);
        }

        private void addBanner(XmlNode node)
        {
            XmlNodeList elementsByTagName = ((XmlElement)node).GetElementsByTagName("bannerPath");
            if (elementsByTagName.Count <= 0 || elementsByTagName[0].InnerXml.Trim().Length <= 0)
                return;
            string filename = elementsByTagName[0].InnerXml.Trim();
            PictureBox pictureBox = new PictureBox();
            try
            {
                if (!filename.StartsWith("\\"))
                    filename = $"{getCurrentDirectory()}\\{filename}";
                pictureBox.Image = (Image)new Bitmap(filename);
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox.Width = this.Width - 16;
                pictureBox.Height = 60;//(this.Height - 80) / 8;
                pictureBox.Top = 0;
                pictureBox.Left = 0;
                this.Controls.Add((Control)pictureBox);
                this.hasBanner = true;
            }
            catch (Exception ex)
            {
            }
        }

        private void addCarousel()
        {
            XmlNode node = this.xmlState.getStateById("0");
            // Crear y agregar el control
            ImageCarousel carouselPanel = new(node, currentDirectory, this.Width - 18);
            carouselPanel.Dock = DockStyle.None;
            carouselPanel.Width = this.Width - 16;
            carouselPanel.Height = 100;
            carouselPanel.Top = 56;
            carouselPanel.Left = 0;
            carouselPanel.BackColor = Color.FromArgb(209, 0, 127);
            carouselPanel.OnClickMethod = new CustomPanelClick(this.carouselPanel_Click);

            this.Controls.Add((Control)carouselPanel);
        }

        private void carouselPanel_Click(Panel p)
        {
            TouchSound.PlayTouch();
            ImageCarousel carouselPanel = (ImageCarousel)p;
            this.idState = carouselPanel.NextState;
            this.addComponents();
        }

        private void setSpecialStates(XmlNode node)
        {
            this.sendErrorState = "999";
            this.readErrorState = "999";
            this.generalErrorState = "999";
            this.syncErrorState = "999";
            this.reprintState = "200";
            this.handshakeState = "201";
            this.BAD_PRINTER = "995";
            this.HEAD_TEMPERATURE = "996";
            this.POWER_SUPPLY = "997";
            this.PAPER_OUT = "998";
            this.FREE_SPACE = "994";
            this.printCopieMessage = "Press yes for print next copie";
            this.printErrorMessage = "ALERTA: Impresora con problemas";
            this.stateVersion = "";
            XmlNodeList elementsByTagName1 = ((XmlElement)node).GetElementsByTagName("sendErrorState");
            if (elementsByTagName1.Count > 0)
                this.sendErrorState = elementsByTagName1[0].InnerXml;
            XmlNodeList elementsByTagName2 = ((XmlElement)node).GetElementsByTagName("readErrorState");
            if (elementsByTagName2.Count > 0)
                this.readErrorState = elementsByTagName2[0].InnerXml;
            XmlNodeList elementsByTagName3 = ((XmlElement)node).GetElementsByTagName("generalErrorState");
            if (elementsByTagName3.Count > 0)
                this.generalErrorState = elementsByTagName3[0].InnerXml;
            XmlNodeList elementsByTagName4 = ((XmlElement)node).GetElementsByTagName("syncErrorState");
            if (elementsByTagName4.Count > 0)
                this.syncErrorState = elementsByTagName4[0].InnerXml;
            XmlNodeList elementsByTagName5 = ((XmlElement)node).GetElementsByTagName("reprintState");
            if (elementsByTagName5.Count > 0)
                this.reprintState = elementsByTagName5[0].InnerXml;
            XmlNodeList elementsByTagName6 = ((XmlElement)node).GetElementsByTagName("handshakeState");
            if (elementsByTagName6.Count > 0)
                this.handshakeState = elementsByTagName6[0].InnerXml;
            XmlNodeList elementsByTagName7 = ((XmlElement)node).GetElementsByTagName("badprinter");
            if (elementsByTagName7.Count > 0)
                this.BAD_PRINTER = elementsByTagName7[0].InnerXml;
            XmlNodeList elementsByTagName8 = ((XmlElement)node).GetElementsByTagName("headtemperature");
            if (elementsByTagName8.Count > 0)
                this.HEAD_TEMPERATURE = elementsByTagName8[0].InnerXml;
            XmlNodeList elementsByTagName9 = ((XmlElement)node).GetElementsByTagName("powersupply");
            if (elementsByTagName9.Count > 0)
                this.POWER_SUPPLY = elementsByTagName9[0].InnerXml;
            XmlNodeList elementsByTagName10 = ((XmlElement)node).GetElementsByTagName("paperout");
            if (elementsByTagName10.Count > 0)
                this.PAPER_OUT = elementsByTagName10[0].InnerXml;
            XmlNodeList elementsByTagName11 = ((XmlElement)node).GetElementsByTagName("printErrorMessage");
            if (elementsByTagName11.Count > 0)
                this.printErrorMessage = elementsByTagName11[0].InnerXml;
            XmlNodeList elementsByTagName12 = ((XmlElement)node).GetElementsByTagName("freespace");
            if (elementsByTagName12.Count > 0)
                this.FREE_SPACE = elementsByTagName12[0].InnerXml;
            XmlNodeList elementsByTagName13 = ((XmlElement)node).GetElementsByTagName("version");
            if (elementsByTagName13.Count > 0)
                this.stateVersion = elementsByTagName13[0].InnerXml;
            XmlNodeList elementsByTagName14 = ((XmlElement)node).GetElementsByTagName("printCopieMessage");
            if (elementsByTagName14.Count <= 0)
                return;
            this.printCopieMessage = elementsByTagName14[0].InnerXml;
        }

        private void setStateTimeout(XmlNode node)
        {
            XmlNodeList elementsByTagName = ((XmlElement)node).GetElementsByTagName("stateTimeout");
            this.defaultTimeout = 10000;
            if (elementsByTagName.Count > 0)
            {
                try
                {
                    this.defaultTimeout = int.Parse(elementsByTagName[0].InnerXml);
                }
                catch (Exception ex)
                {
                }
            }
            this.stateTimer_new.Interval = this.defaultTimeout;
        }

        private void addComponents()
        {
            XmlNode stateById = this.xmlState.getStateById(this.idState);
            if (stateById == null)
            {
                this.idState = this.generalErrorState;
                this.xmlState.getStateById(this.idState);
            }
            else
            {
                this.SuspendLayout();
                int count = this.Controls.Count;
                for (int index = 1; index < count; ++index)
                    this.Controls[1].Dispose();
                if (!this.hasBanner && this.Controls.Count > 0)
                    this.Controls[0].Dispose();
                string stringAttribute1 = this.xmlState.getStringAttribute(stateById, "type", "invalid");
                string stringAttribute2 = this.xmlState.getStringAttribute(stateById, "title", "");
                string stringAttribute3 = this.xmlState.getStringAttribute(stateById, "vAlignTitle", "top");
                this.addCarousel();
                if (stringAttribute2 != string.Empty)
                {
                    Label label = new Label();
                    int num = (stringAttribute3 == "middle" ? this.Height / 3 : this.Height / 7) + 40;
                    label.Left = 0;
                    label.Top = num;
                    label.Width = this.Width;
                    label.TextAlign = ContentAlignment.TopCenter;
                    label.Text = stringAttribute2;
                    label.Height = 50;
                    label.Font = new Font("Segoe UI", 14, FontStyle.Bold);
                    label.BackColor = Color.Transparent;
                    this.Controls.Add((Control)label);
                }
                this.stateTimer_new.Enabled = false;
                switch (stringAttribute1)
                {
                    case "alpha":
                        this.addAlphaPanel(stateById);
                        this.stateTimer_new.Enabled = true;
                        break;
                    case "break":
                        this.moveLog();
                        this.idState = this.xmlState.getStringAttribute(stateById, "nextState", this.idState);
                        this.doTimer_new.Enabled = true;
                        break;
                    case "card":
                        this.readCard(stateById);
                        break;
                    case "face":
                        this.facePhi(stateById);
                        break;
                    case "handshake":
                        this.handshake(stateById);
                        break;
                    case "menu":
                        this.addMenuButtons(stateById);
                        this.stateTimer_new.Enabled = true;
                        break;
                    case "numeric":
                        this.addNumericPanel(stateById);
                        this.stateTimer_new.Enabled = true;
                        break;
                    case "pin":
                        this.pinEntry(stateById);
                        break;
                    case "prepare":
                        this.idState = this.xmlState.getStringAttribute(stateById, "nextState", this.idState);
                        this.doTimer_new.Enabled = true;
                        break;
                    case "reprint":
                        this.reprint(stateById);
                        break;
                    case "send":
                        this.reprintCounter = 0;
                        this.sendTransaction(this.xmlState.getBooleanAttribute(stateById, "reprint", false), true, false);
                        break;
                }
                this.ResumeLayout();
            }
            this.Focus();
        }

        private void addMenuButtons(XmlNode node)
        {
            XmlNodeList elementsByTagName1 = ((XmlElement)node).GetElementsByTagName("button");
            for (int i = 0; i < elementsByTagName1.Count && i < 8; ++i)
            {
                NbcButton nbcButton = new NbcButton();
                nbcButton.Text = this.xmlState.getStringAttribute(elementsByTagName1[i], "text", "Undefined");
                nbcButton.Left = this.pos[this.xmlState.getIntAttribute(elementsByTagName1[i], "pos", 0), 0];
                nbcButton.Top = this.pos[this.xmlState.getIntAttribute(elementsByTagName1[i], "pos", 0), 1];
                nbcButton.Height = this.buttonHeight;
                nbcButton.Width = this.buttonWidth;
                nbcButton.Tag = (object)elementsByTagName1[i];
                nbcButton.Click += new EventHandler(this.bNextState_Click);
                this.Controls.Add((Control)nbcButton);
            }
            XmlNodeList elementsByTagName2 = ((XmlElement)node).GetElementsByTagName("label");
            for (int i = 0; i < elementsByTagName2.Count; ++i)
            {
                string stringAttribute = this.xmlState.getStringAttribute(elementsByTagName2[i], "value", "");
                string str = !(stringAttribute.Trim() != string.Empty) ? this.xmlState.getStringAttribute(elementsByTagName2[i], "text", "") : this.bufferList.getBufferValue(stringAttribute);
                if (str.Trim() != string.Empty)
                {
                    Label label = new Label();
                    switch (this.xmlState.getStringAttribute(elementsByTagName2[i], "type", "default"))
                    {
                        case "left":
                            label.Left = 0;
                            label.Width = this.Width / 2;
                            label.TextAlign = ContentAlignment.TopRight;
                            break;
                        case "right":
                            label.Left = this.Width / 2;
                            label.Width = this.Width / 2;
                            label.TextAlign = ContentAlignment.TopLeft;
                            break;
                        default:
                            label.Left = 0;
                            label.Width = this.Width;
                            label.TextAlign = ContentAlignment.TopCenter;
                            break;
                    }
                    label.Height = 50;
                    label.Font = new Font("Segoe UI", 14);
                    label.Top = this.Height / 12 * this.xmlState.getIntAttribute(elementsByTagName2[i], "pos", 5);
                    label.Text = str;
                    this.Controls.Add((Control)label);
                }
            }
        }

        private void addNumericPanel(XmlNode node)
        {
            XmlNodeList elementsByTagName = ((XmlElement)node).GetElementsByTagName("numericPanel");
            if (elementsByTagName.Count <= 0)
                return;
            NumericPanel numericPanel = new NumericPanel(this.panelWidth, this.panelHeight, this.xmlState.getBooleanAttribute(elementsByTagName[0], "decimalButton", false), this.buttonWidth, this.buttonHeight, this.yOkButtonPanel, this.yRetryButtonPanel, this.xmlState.getStringAttribute(elementsByTagName[0], "okButtonText", "Continue"), this.xmlState.getStringAttribute(elementsByTagName[0], "retryButtonText", "Retry"));
            numericPanel.Left = this.xPanel;
            numericPanel.Top = this.yPanel;
            numericPanel.BufferName = this.xmlState.getStringAttribute(elementsByTagName[0], "bufferName", numericPanel.BufferName);
            numericPanel.NextState = this.xmlState.getStringAttribute(elementsByTagName[0], "nextState", this.idState);
            numericPanel.MinValue = this.xmlState.getFloatAttribute(elementsByTagName[0], "minValue", numericPanel.MinValue);
            numericPanel.MaxValue = this.xmlState.getFloatAttribute(elementsByTagName[0], "maxValue", numericPanel.MaxValue);
            numericPanel.OnClickMethod = new CustomPanelClick(this.numericPanel_Click);
            numericPanel.ParentStateTimer = this.stateTimer_new;
            this.Controls.Add((Control)numericPanel);
        }

        private void addAlphaPanel(XmlNode node)
        {
            XmlNodeList elementsByTagName = ((XmlElement)node).GetElementsByTagName("alphaPanel");
            if (elementsByTagName.Count <= 0)
                return;
            AlphaPanel alphaPanel = new AlphaPanel(this.panelWidth, this.panelHeight, this.buttonWidth, this.buttonHeight, this.yOkButtonPanel, this.yRetryButtonPanel, this.xmlState.getStringAttribute(elementsByTagName[0], "okButtonText", "Continue"), this.xmlState.getStringAttribute(elementsByTagName[0], "retryButtonText", "Retry"));
            alphaPanel.Left = this.xPanel;
            alphaPanel.Top = this.yPanel;
            alphaPanel.BufferName = this.xmlState.getStringAttribute(elementsByTagName[0], "bufferName", alphaPanel.BufferName);
            alphaPanel.NextState = this.xmlState.getStringAttribute(elementsByTagName[0], "nextState", this.idState);
            alphaPanel.MaxLength = this.xmlState.getIntAttribute(elementsByTagName[0], "maxLength", 40);
            alphaPanel.OnClickMethod = new CustomPanelClick(this.alphaPanel_Click);
            alphaPanel.ParentStateTimer = this.stateTimer_new;
            this.Controls.Add((Control)alphaPanel);
        }

        private string getPrinterState(uint printerStatus)
        {
            if ((int)printerStatus == (int)Printer.PTR_NO_STATUS)
                return this.BAD_PRINTER;
            if (((int)printerStatus & (int)Printer.PTR_PAPER_OUT) == (int)Printer.PTR_PAPER_OUT)
                return this.PAPER_OUT;
            if (((int)printerStatus & (int)Printer.PTR_HEAD_TEMPERATURE) == (int)Printer.PTR_HEAD_TEMPERATURE)
                return this.HEAD_TEMPERATURE;
            return ((int)printerStatus & (int)Printer.PTR_POWER_SUPPLY) == (int)Printer.PTR_POWER_SUPPLY ? this.POWER_SUPPLY : this.BAD_PRINTER;
        }

        private void bNextState_Click(object sender, EventArgs e)
        {
            uint printerStatus = Printer.getPrinterStatus();
            if ((int)printerStatus != (int)Printer.PTR_OK)
                this.idState = this.getPrinterState(printerStatus);
            else if (this.getPercentageDiskFreeSpace() >= this.freeSpacePercentage)
            {
                this.idState = this.FREE_SPACE;
            }
            else
            {
                XmlNode tag = (XmlNode)((Control)sender).Tag;
                if (this.xmlState.getStringAttribute(tag, "code", " ") != " ")
                    this.tranBuffer.Append(this.xmlState.getStringAttribute(tag, "code", " "));
                this.idState = this.xmlState.getStringAttribute(tag, "nextState", this.idState);
            }
            this.addComponents();
        }

        private void numericPanel_Click(Panel p)
        {
            NumericPanel numericPanel = (NumericPanel)p;
            if (numericPanel.validate())
            {
                this.bufferList.add(numericPanel.BufferName, numericPanel.BufferValue);
                this.idState = numericPanel.NextState;
                this.addComponents();
            }
            else
                numericPanel.BufferValue = "";
        }

        private void clearBuffers()
        {
            this.tranBuffer = new StringBuilder(4);
            this.bufferList.clear();
        }

        private void alphaPanel_Click(Panel p)
        {
            AlphaPanel alphaPanel = (AlphaPanel)p;
            this.bufferList.add(alphaPanel.BufferName, alphaPanel.BufferValue);
            this.idState = alphaPanel.NextState;
            this.addComponents();
        }

        private void readCard(XmlNode node)
        {
            XmlNodeList elementsByTagName = ((XmlElement)node).GetElementsByTagName("cardReader");
            if (elementsByTagName.Count > 0)
            {
                string stringAttribute1 = this.xmlState.getStringAttribute(elementsByTagName[0], "displayMessage", "[642000010100]");
                string stringAttribute2 = this.xmlState.getStringAttribute(elementsByTagName[0], "failedText", "Error reading card");
                string stringAttribute3 = this.xmlState.getStringAttribute(elementsByTagName[0], "retryMessage", "Error reading card, do you want retry?");
                PinPad pinPad = new PinPad(this.xmlState.getIntAttribute(elementsByTagName[0], "timeout", 10000), this.pinpadMessage1, this.pinpadMessage2, this.setGMAC);
                this.myMessageBox.TextMessage = stringAttribute3;
                this.myMessageBox.IsButtonYesAndNo = true;
                string track1 = string.Empty;
                string track2 = string.Empty;
                if (this.mockTarjetaCliente)
                {
                    Configuration configuration = new Configuration(this.defaultDirectory + "\\Configuration.xml");
                    string bufferValue1 = configuration.getString("Mock", "TjTrack1", "");
                    string bufferValue2 = configuration.getString("Mock", "TjTrack1", "");
                    if (!string.IsNullOrEmpty(bufferValue1))
                        this.bufferList.add("track1", bufferValue1);
                    this.bufferList.add("track2", bufferValue2);
                    this.idState = this.xmlState.getStringAttribute(elementsByTagName[0], "nextState", "");
                }
                else
                {
                    bool flag;
                    do
                    {
                        Application.DoEvents();
                        flag = pinPad.readCardData(stringAttribute1, out track1, out track2);
                        pinPad.clearScreen();
                    }
                    while (!flag && this.myMessageBox.ShowDialog() == DialogResult.Yes);
                    pinPad.close();
                    if (flag)
                    {
                        if (track1 != null)
                            this.bufferList.add("track1", track1);
                        this.bufferList.add("track2", track2);
                        this.idState = this.xmlState.getStringAttribute(elementsByTagName[0], "nextState", "");
                    }
                    else
                    {
                        this.idState = this.initialState;
                        Label label = new Label();
                        label.Width = this.Width;
                        label.Top = this.Height / 4 * 3;
                        label.Left = 0;
                        label.TextAlign = ContentAlignment.TopCenter;
                        label.Text = stringAttribute2;
                        label.ForeColor = Color.Red;
                        this.Controls.Add((Control)label);
                        Application.DoEvents();
                        this.clearBuffers();
                        Thread.Sleep(5000);
                    }
                }
            }
            else
                this.idState = this.generalErrorState;
            this.doTimer_new.Enabled = true;
        }

        private void facePhi(XmlNode node)
        {
            XmlNodeList elementsByTagName = ((XmlElement)node).GetElementsByTagName("faceReader");
            if (elementsByTagName.Count <= 0)
                return;
            Application.DoEvents();
            this.bufferList.add("resultneov", "true");
            this.bufferList.add("secneov", DateTime.Now.ToString("dHHmmss"));
            this.idState = this.xmlState.getStringAttribute(elementsByTagName[0], "nextState", "");
            this.doTimer_new.Enabled = true;
        }

        private void pinEntry(XmlNode node)
        {
            XmlNodeList elementsByTagName = ((XmlElement)node).GetElementsByTagName(nameof(pinEntry));
            if (elementsByTagName.Count > 0)
            {
                string stringAttribute1 = this.xmlState.getStringAttribute(elementsByTagName[0], "displayMessage", "[642100010100]");
                string stringAttribute2 = this.xmlState.getStringAttribute(elementsByTagName[0], "failedText", "Pinpad Error");
                PinPad pinPad = new PinPad(this.xmlState.getIntAttribute(elementsByTagName[0], "timeout", 10000), this.pinpadMessage1, this.pinpadMessage2, this.setGMAC);
                Application.DoEvents();
                string bufferValue = this.bufferList.getBufferValue("track2");
                string nbcCard;
                if (!(bufferValue != string.Empty))
                    nbcCard = this.nbcCard;
                else
                    nbcCard = bufferValue.Split('=')[0];
                string cardNumber = nbcCard;
                string pinBuffer = string.Empty;
                if (pinPad.readPinBuffer(stringAttribute1, cardNumber, this.wKey, out pinBuffer))
                {
                    this.bufferList.add("pinBuffer", this.MockCnbPinBuffer);
                    this.idState = this.xmlState.getStringAttribute(elementsByTagName[0], "nextState", "");
                    pinPad.clearScreen();
                    pinPad.close();
                }
                else
                {
                    this.idState = this.initialState;
                    Label label = new Label();
                    label.Text = stringAttribute2;
                    label.ForeColor = Color.Red;
                    label.Width = this.Width;
                    label.Top = this.Height / 4 * 3;
                    label.Left = 0;
                    label.TextAlign = ContentAlignment.TopCenter;
                    this.Controls.Add((Control)label);
                    Application.DoEvents();
                    pinPad.clearScreen();
                    pinPad.close();
                    this.clearBuffers();
                    Thread.Sleep(5000);
                }
            }
            else
                this.idState = this.generalErrorState;
            this.doTimer_new.Enabled = true;
        }

        private void sendTransaction(bool withReprint, bool firstTime, bool isHandshake)
        {
            try
            {
                XmlDocument xmlDocument1 = new XmlDocument();
                XmlElement element1 = xmlDocument1.CreateElement("request");
                xmlDocument1.AppendChild((XmlNode)element1);
                XmlElement element2 = xmlDocument1.CreateElement("buffers");
                element1.AppendChild((XmlNode)element2);
                XmlElement element3 = xmlDocument1.CreateElement("tranCode");
                element3.InnerXml = this.tranBuffer.ToString();
                element2.AppendChild((XmlNode)element3);
                XmlElement element4 = xmlDocument1.CreateElement("sync");
                element4.InnerXml = this.sync.ToString();
                element2.AppendChild((XmlNode)element4);
                string str1 = this.sync.ToString();
                this.sync = this.sync < 9 ? this.sync + 1 : 0;
                for (int idx = 0; idx < this.bufferList.Count; ++idx)
                {
                    NbcBuffer bufferAt = this.bufferList.getBufferAt(idx);
                    if (element2.InnerXml.Contains("<tranCode>YA</tranCode>"))
                    {
                        if (!this.CamposBufferRemover.Exists((Predicate<string>)(x => x.Equals(bufferAt.BufferName.ToLower().Trim()))))
                        {
                            XmlElement element5 = xmlDocument1.CreateElement(bufferAt.BufferName);
                            element5.InnerXml = bufferAt.BufferValue;
                            element2.AppendChild((XmlNode)element5);
                        }
                    }
                    else
                    {
                        XmlElement element6 = xmlDocument1.CreateElement(bufferAt.BufferName);
                        element6.InnerXml = bufferAt.BufferValue;
                        element2.AppendChild((XmlNode)element6);
                    }
                }
                this.clearBuffers();
                Application.DoEvents();
                if ((!isHandshake ? (uint)this.tcpClient.connect() : 0U) > 0U)
                    this.idState = !isHandshake ? (!withReprint | firstTime ? this.sendErrorState : this.reprintState) : this.handshakeState;
                else if ((!isHandshake ? (uint)this.tcpClient.send(CryptClass.encrypt(xmlDocument1.OuterXml), true) : 0U) > 0U)
                {
                    this.idState = !isHandshake ? (!withReprint | firstTime ? this.sendErrorState : this.reprintState) : this.handshakeState;
                }
                else
                {
                    if (withReprint && !isHandshake)
                        this.setPending(true);
                    DateTime now = DateTime.Now;
                    string str2;
                    if (isHandshake)
                        str2 = $"<response><workingKey>{this.MockCnbWorkingKey}</workingKey><nbcCardNumber>{this.MockCnbCardNumber}</nbcCardNumber><sync>0</sync><nextState>83</nextState></response>";
                    else
                        str2 = this.tcpClient.read();
                    string str3 = str2;
                    if (str3 == null)
                    {
                        this.idState = !isHandshake ? (withReprint ? this.reprintState : this.readErrorState) : this.handshakeState;
                        ++this.reprintCounter;
                        int totalMilliseconds = (int)DateTime.Now.Subtract(now).TotalMilliseconds;
                        if (totalMilliseconds < this.tcpClient.ReceiveTimeout)
                            Thread.Sleep(this.tcpClient.ReceiveTimeout - totalMilliseconds);
                    }
                    else
                    {
                        if (!isHandshake)
                            str3 = CryptClass.decrypt(str3);
                        if (str3 == null)
                        {
                            this.idState = this.generalErrorState;
                        }
                        else
                        {
                            XmlDocument xmlDocument2 = new XmlDocument();
                            xmlDocument2.LoadXml(str3);
                            if (xmlDocument2.GetElementsByTagName("sync")[0].InnerXml == str1)
                            {
                                XmlNode xmlNode = xmlDocument2.GetElementsByTagName("nextState")[0];
                                this.idState = xmlNode.InnerXml;
                                if (this.idState == this.handshakeState)
                                    Thread.Sleep(5000);
                                this.stateTimer_new.Interval = xmlNode.Attributes.Count <= 0 ? this.defaultTimeout : int.Parse(xmlNode.Attributes[0].Value);
                                if (isHandshake)
                                {
                                    try
                                    {
                                        this.wKey = xmlDocument2.GetElementsByTagName("workingKey")[0].InnerXml;
                                        this.nbcCard = xmlDocument2.GetElementsByTagName("nbcCardNumber")[0].InnerXml;
                                        this.handshaked = true;
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }
                                XmlNodeList elementsByTagName1 = xmlDocument2.GetElementsByTagName("buffers");
                                if (elementsByTagName1.Count > 0)
                                {
                                    foreach (XmlNode childNode in elementsByTagName1[0].ChildNodes)
                                        this.bufferList.add(childNode.Name, childNode.InnerXml);
                                }
                                string str4 = (string)null;
                                XmlNodeList elementsByTagName2 = xmlDocument2.GetElementsByTagName("transactionKeyBuffer");
                                if (elementsByTagName2.Count > 0)
                                    str4 = DataFormat.getTrxLog(DataFormat.TransactionType.ResponseServer, (short)0, ((XmlElement)elementsByTagName2[0]).GetElementsByTagName("datetime")[0].InnerXml, ((XmlElement)elementsByTagName2[0]).GetElementsByTagName("terminalId")[0].InnerXml, ((XmlElement)elementsByTagName2[0]).GetElementsByTagName("sequential")[0].InnerXml, DataFormat.TransactionStateType.Ok);
                                XmlNodeList elementsByTagName3 = xmlDocument2.GetElementsByTagName("printBuffer");
                                if (elementsByTagName3.Count > 0)
                                {
                                    while ((int)Printer.getPrinterStatus() != (int)Printer.PTR_OK)
                                    {
                                        this.myMessageBox.TextMessage = "ALERTA: Impresora con problemas";
                                        this.myMessageBox.IsButtonYesAndNo = false;
                                        int num = (int)this.myMessageBox.ShowDialog();
                                    }
                                    this.idState = xmlNode.InnerXml;
                                    int num1 = int.Parse(elementsByTagName3[0].Attributes["numCopies"].Value);
                                    this.myMessageBox.TextMessage = this.printCopieMessage;
                                    this.myMessageBox.IsButtonYesAndNo = true;
                                    for (int index = 0; index < num1; ++index)
                                    {
                                        Printer.print(this.getPrintData(elementsByTagName3[0].ChildNodes, index + 1));
                                        foreach (string str5 in this.getPrintData(elementsByTagName3[0].ChildNodes, index + 1))
                                            this.myLog.writeLine("", str5);
                                        if (index + 1 < num1 && this.myMessageBox.ShowDialog() == DialogResult.No)
                                            break;
                                    }
                                }
                                if (str4 != null)
                                    this.myLog.writeLine(str4);
                                this.setPending(false);
                                if (!this.handshaked)
                                    this.idState = this.handshakeState;
                            }
                            else
                                this.idState = this.syncErrorState;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.idState = this.generalErrorState;
            }
            finally
            {
                this.tcpClient.close();
            }
            this.doTimer_new.Enabled = true;
        }

        private void reprint(XmlNode aNode)
        {
            this.tranBuffer.Append(this.xmlState.getStringAttribute(aNode, "bufferValue", ""));
            Label label = new Label();
            label.Text = $"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} {this.reprintCounter.ToString()}";
            label.Width = this.Width;
            label.Top = this.Height / 4 * 3;
            label.Left = 0;
            label.TextAlign = ContentAlignment.TopCenter;
            this.Controls.Add((Control)label);
            this.sendTransaction(true, false, false);
        }

        private void handshake(XmlNode aNode)
        {
            this.tranBuffer.Append(this.xmlState.getStringAttribute(aNode, "bufferValue", ""));
            this.bufferList.add("programVersion", this.programVersion);
            this.bufferList.add("stateVersion", this.stateVersion);
            Label label = new Label();
            label.Text = $"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} {this.reprintCounter.ToString()}";
            label.Width = this.Width;
            label.Top = this.Height / 4 * 3;
            label.Left = 0;
            label.TextAlign = ContentAlignment.TopCenter;
            this.Controls.Add((Control)label);
            this.sendTransaction(true, false, true);
        }

        private ArrayList getPrintData(XmlNodeList printBuffer, int copieIndex)
        {
            ArrayList printData = new ArrayList();
            foreach (XmlNode xmlNode in printBuffer)
            {
                string str = (!(xmlNode.Name == "n") ? (!(xmlNode.Name == "b") ? (!(xmlNode.Name == "l") ? "N" : "L") : "B") : "N") + xmlNode.InnerXml;
                int i = 0;
                while (i < xmlNode.Attributes.Count && xmlNode.Attributes[i].Name != "hwc")
                    ++i;
                int num = i == xmlNode.Attributes.Count ? 0 : int.Parse(xmlNode.Attributes["hwc"].Value);
                if (copieIndex != num)
                    printData.Add((object)str);
            }
            return printData;
        }

        private static string getCurrentDirectory()
        {
            try
            {
                StringBuilder lpFilename = new StringBuilder((int)byte.MaxValue);
                int moduleFileName = (int)GetModuleFileName(IntPtr.Zero, lpFilename, lpFilename.Capacity);
                return lpFilename.ToString().Substring(0, lpFilename.ToString().LastIndexOf('\\'));
            }
            catch (Exception ex)
            {
                try
                {
                    return Directory.GetCurrentDirectory();
                }
                catch
                {
                    return "\\";
                }
            }
        }

        [DllImport("coredll.dll", SetLastError = true)]
        private static extern uint GetModuleFileName([In] IntPtr hModule, [Out] StringBuilder lpFilename, [In] int nSize);

        [DllImport("coredll.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetDiskFreeSpaceEx(
          string lpDirectoryName,
          out ulong lpFreeBytesAvailable,
          out ulong lpTotalNumberOfBytes,
          out ulong lpTotalNumberOfFreeBytes);

        private int getPercentageDiskFreeSpace() => 50;

        private void stateTimer_Tick(object sender, EventArgs e)
        {
            this.stateTimer_new.Enabled = false;
            this.clearBuffers();
            this.idState = this.initialState;
            this.stateTimer_new.Interval = this.defaultTimeout;
            this.addComponents();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.stateTimer_new.Enabled = false;
            this.clearBuffers();
            this.idState = this.initialState;
            this.stateTimer_new.Interval = this.defaultTimeout;
            this.addComponents();
        }

        private void setPending(bool isPending)
        {
            StreamWriter streamWriter = (StreamWriter)null;
            try
            {
                streamWriter = new StreamWriter(this.pendingPath, false);
                streamWriter.WriteLine(isPending ? "1" : "0");
                streamWriter.Flush();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                streamWriter?.Close();
            }
        }

        private bool getPending()
        {
            StreamReader streamReader = (StreamReader)null;
            string empty = string.Empty;
            bool pending = false;
            try
            {
                if (File.Exists(this.pendingPath))
                {
                    streamReader = new StreamReader(this.pendingPath);
                    if (streamReader.ReadLine().Trim() == "1")
                        pending = true;
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                streamReader?.Close();
            }
            return pending;
        }

        private void moveLog()
        {
            if (!File.Exists(this.myLog.FullFileName))
                return;
            try
            {
                if (!Directory.Exists(this.moveLogPath))
                    Directory.CreateDirectory(this.moveLogPath);
                string str = Path.Combine(this.moveLogPath, "NbcClientLog" + DateTime.Now.ToString("yyyyMMdd-HHmmss"));
                File.Move(this.myLog.FullFileName, str);
                this.compressionExecute(str, true);
            }
            catch (Exception ex)
            {
                this.myLog.writeLine("movelog: " + ex.Message);
            }
        }

        public void compressionExecute(string source, bool deleteOnSuccess)
        {
            using (FileStream fileStream = File.Create(source + ".zlib"))
            {
                using (Stream stream = (Stream)File.OpenRead(source))
                {
                    using (ZlibStream zlibStream = new ZlibStream((Stream)fileStream, CompressionMode.Compress, CompressionLevel.BestCompression, false))
                    {
                        byte[] buffer = new byte[4096 /*0x1000*/];
                        int count;
                        while ((count = stream.Read(buffer, 0, buffer.Length)) != 0)
                            ((Stream)zlibStream).Write(buffer, 0, count);
                    }
                }
            }
            if (!deleteOnSuccess)
                return;
            try
            {
                File.Delete(source);
            }
            catch
            {
            }
        }

    }

    public static class TouchSound
    {
        [DllImport("user32.dll")]
        public static extern bool MessageBeep(uint uType);

        public static void PlayTouch()
        {
            // 0xFFFFFFFF = sonido predeterminado
            MessageBeep(0xFFFFFFFF);
        }
    }
}
