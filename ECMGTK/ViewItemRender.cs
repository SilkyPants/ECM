using System;
using System.ComponentModel;

namespace ECMGTK
{
    public partial class ViewItemRender : Gtk.Window
    {
        public ViewItemRender () :
                base(Gtk.WindowType.Toplevel)
        {
            this.Build ();
        }

        public void ShowItemRender(ECM.EveItem item)
        {
            BackgroundWorker fetchRender = new BackgroundWorker();

            this.Title = item.Name;
            imgItemRender.PixbufAnimation = new Gdk.PixbufAnimation(ECM.Core.LoadingSpinnerGIF);

            fetchRender.DoWork += delegate(object sender, DoWorkEventArgs e)
            {
                imgItemRender.Pixbuf = EveApi.ImageApi.GetItemRenderGTK(item.ID, EveApi.ImageApi.ImageRequestSize.Size512x512);

                //Show();
            };

            fetchRender.RunWorkerAsync();
            ShowAll();
        }

        public void ShowCharacterRender (ECM.Character character, EveApi.ImageApi.ImageRequestSize size512x512)
        {
            BackgroundWorker fetchRender = new BackgroundWorker();

            this.Title = character.Name;
            imgItemRender.PixbufAnimation = new Gdk.PixbufAnimation(ECM.Core.LoadingSpinnerGIF);

            fetchRender.DoWork += delegate(object sender, DoWorkEventArgs e)
            {
                imgItemRender.Pixbuf = EveApi.ImageApi.GetCharacterPortraitGTK(character.ID, EveApi.ImageApi.ImageRequestSize.Size512x512);
            };

            fetchRender.RunWorkerAsync();
            ShowAll();
        }

        protected void OnDelete (object o, Gtk.DeleteEventArgs args)
        {
            // Stop the window actually closing
            args.RetVal = true;

            // Hide the window
            Hide();
        }

    }
}

