using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ECM
{
    public partial class frmItemRenderView : Form
    {
        long m_TypeID;
        public frmItemRenderView(long typeID, EveApi.ImageApi.ImageRequestSize size)
        {
            InitializeComponent();

            long m_TypeID = typeID;

            int newSize = 256;

            switch (size)
            {
                case EveApi.ImageApi.ImageRequestSize.Size30x30:
                case EveApi.ImageApi.ImageRequestSize.Size32x32:
                    newSize = 32;
                    break;
                case EveApi.ImageApi.ImageRequestSize.Size64x64:
                    newSize = 64;
                    break;
                case EveApi.ImageApi.ImageRequestSize.Size128x128:
                    newSize = 128;
                    break;
                case EveApi.ImageApi.ImageRequestSize.Size200x200:
                case EveApi.ImageApi.ImageRequestSize.Size256x256:
                    newSize = 256;
                    break;
                case EveApi.ImageApi.ImageRequestSize.Size512x512:
                case EveApi.ImageApi.ImageRequestSize.Size1024x1024:
                    newSize = 512;
                    break;
            }

            this.Size = new System.Drawing.Size(newSize + 16, newSize + 38);


            pictureBox1.Image = EveApi.ImageApi.GetItemRenderNET(m_TypeID, size);
        }
    }
}
