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
    public partial class NbcButton: Button
    {    
        public NbcButton()
        {
            this.BackColor = Color.FromArgb(163, 26, 97);
            this.ForeColor = Color.White;
            this.Cursor = Cursors.Hand;
        }
    }
}
