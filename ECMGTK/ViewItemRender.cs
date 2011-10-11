using System;
using System.ComponentModel;

namespace ECMGTK
{
    public partial class ViewItemRender : Gtk.Window
    {
        public ViewItemRender (ECM.EveItem item) :
                base(Gtk.WindowType.Toplevel)
        {
            this.Build ();

            this.Title = item.Name;

            imgItemRender.PixbufAnimation = new Gdk.PixbufAnimation(ECM.Core.LoadingSpinnerGIF);

            BackgroundWorker fetchRender = new BackgroundWorker();

            fetchRender.DoWork += delegate(object sender, DoWorkEventArgs e)
            {
                imgItemRender.Pixbuf = EveApi.ImageApi.GetItemRenderGTK(item.ID, EveApi.ImageApi.ImageRequestSize.Size512x512);
            };

            fetchRender.RunWorkerAsync();
        }
    }
}

