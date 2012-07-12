using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;

namespace ECM.API
{
    public class ImageLoader
    {
        Gdk.PixbufAnimationIter m_AnimatedIconIter;
        Gdk.PixbufAnimation m_AnimatedIcon;

        public event EventHandler OnImageUpdated;
        
        private long m_ImageID;
        private ImageApi.ImageRequestSize m_ImageRequestSize;
        private ImageApi.ImageRequestType m_ImageRequestType;

        BackgroundWorker m_Worker = new BackgroundWorker();

        public ImageLoader()
        {
            AnimatedIcon = new Gdk.PixbufAnimation(ECM.Core.LoadingSpinnerGIF);
            m_Worker.DoWork += new DoWorkEventHandler(RetrieveImage);
        }

        void RetrieveImage(object sender, DoWorkEventArgs e)
        {
            MemoryStream ms = new MemoryStream();

            if(m_ImageRequestType == ImageApi.ImageRequestType.Alliance)
                ms = ImageApi.GetAllianceLogo(m_ImageID, m_ImageRequestSize);
            else if (m_ImageRequestType == ImageApi.ImageRequestType.Character)
                ms = ImageApi.GetCharacterPortrait(m_ImageID, m_ImageRequestSize);
            else if (m_ImageRequestType == ImageApi.ImageRequestType.Corporation)
                ms = ImageApi.GetCorporationLogo(m_ImageID, m_ImageRequestSize);
            else if (m_ImageRequestType == ImageApi.ImageRequestType.Item)
                ms = ImageApi.GetItemImage(m_ImageID, m_ImageRequestSize);

            AnimatedIcon = ImageApi.StreamToPixbufAnim(new System.Drawing.Bitmap(ms));

            if (OnImageUpdated != null)
                OnImageUpdated(this, null);
        }

        public ImageLoader(long imageID, ImageApi.ImageRequestSize imageRequestSize, ImageApi.ImageRequestType imageRequestType) : this()
        {
            this.m_ImageID = imageID;
            this.m_ImageRequestSize = imageRequestSize;
            this.m_ImageRequestType = imageRequestType;

            m_Worker.RunWorkerAsync();
        }

        public Gdk.PixbufAnimation AnimatedIcon
        {
            get { return m_AnimatedIcon; }
            set
            {
                m_AnimatedIcon = value;

                if (m_AnimatedIcon != null)
                {
                    m_AnimatedIconIter = m_AnimatedIcon.GetIter(IntPtr.Zero);

                    GLib.Timeout.Add((uint)m_AnimatedIconIter.DelayTime, new GLib.TimeoutHandler(OnDelayTimeExpire));
                }
            }
        }

        public Gdk.Pixbuf Image
        {
            get
            {
                if(m_AnimatedIconIter != null)
                    return m_AnimatedIconIter.Pixbuf;

                return null;
            }
        }

        bool OnDelayTimeExpire()
        {
            if (m_AnimatedIconIter != null)
            {
                m_AnimatedIconIter.Advance(IntPtr.Zero);

                if (OnImageUpdated != null)
                    OnImageUpdated(this, null);

                if(!AnimatedIcon.IsStaticImage)
                    GLib.Timeout.Add((uint)m_AnimatedIconIter.DelayTime, new GLib.TimeoutHandler(OnDelayTimeExpire));
            }

            return false;
        }
    }
}
