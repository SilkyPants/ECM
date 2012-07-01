using System;

namespace ECMGTK
{
    public partial class ViewItemDetails : Gtk.Window
    {
        public ViewItemDetails () : 
                base(Gtk.WindowType.Toplevel)
        {
            this.Build ();
        }
    }
}

