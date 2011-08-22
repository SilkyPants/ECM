using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ApiTestBed
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void GetImage(object sender, EventArgs e)
        {
            Image img = null;

            if (sender == btnGetCharPortrait)
                img = EveApi.ImageApi.GetCharacterPortrait((int)nudCharID.Value, EveApi.ImageApi.ImageRequestSize.Size256x256);

            if (img != null)
            {
                picImage.Image = img;
            }
        }
    }
}
