using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ПРОТОН
{
    public partial class SoundPage : UserControl
    {
        MainPage _main;
        public SoundPage(MainPage main)
        {
            InitializeComponent();
            _main = main;
        }

        private void ContextMenuPictureBox_Click(object sender, EventArgs e)
        {
            PictureBox btnSender = (PictureBox)sender;
            Point ptLowerLeft = new Point(0, btnSender.Height);
            ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);
          _main.playlist.Show(ptLowerLeft);
        }

        private void SoundPage_Load(object sender, EventArgs e)
        {
           
        }

        private void SoundPage_Click(object sender, EventArgs e)
        {
            string url = SoundUrlLabel.Text;
            if (_main.InvokeRequired)
            {
                _main.Invoke(new Action(() => _main.OnMusic(url)));
            }
            else
            {
                _main.OnMusic(url);
            }
        }
        bool state= false;
        private void SoundPage_DoubleClick(object sender, EventArgs e)
        {
            state=!state;
            if (_main.InvokeRequired)
            {
                _main.Invoke(new Action(() => _main.PuauseMusic(state)));
            }
            else
            {
                _main.PuauseMusic(state);
            }
        }
    }
}
