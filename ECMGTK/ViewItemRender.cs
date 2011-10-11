using System;
using System.ComponentModel;

namespace ECMGTK
{
    public partial class ViewItemRender : Gtk.Window
    {
        public ViewItemRender (long itemID) :
                base(Gtk.WindowType.Toplevel)
        {
            this.Build ();
            imgItemRender.PixbufAnimation = new Gdk.PixbufAnimation(ECM.Core.LoadingSpinnerGIF);

            BackgroundWorker fetchRender = new BackgroundWorker();

            fetchRender.DoWork += delegate(object sender, DoWorkEventArgs e)
            {
                imgItemRender.Pixbuf = EveApi.ImageApi.GetItemRenderGTK(itemID, EveApi.ImageApi.ImageRequestSize.Size512x512);
            };

            fetchRender.RunWorkerAsync();
        }
    }
}

