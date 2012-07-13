using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gtk;
using Cairo;
using System.IO;

namespace ECMGTK
{
    class CellRendererAsset : CellRendererEveTree
    {
        protected override Stream ExpandedIcon
        {
            get { return ECM.Core.SolidDownPNG; }
        }

        protected override Stream CollapsedIcon
        {
            get { return ECM.Core.SolidRightPNG; }
        }

        public override bool TruncateText 
        {
            get { return !IsHeading; } 
        }

        public CellRendererAsset(IntPtr raw)
            : base(raw)
        {
            RenderInfo = false;
        }

        public CellRendererAsset()
        {
            RenderInfo = false;
        }

        protected override void RenderExpander(Context context, Gdk.Rectangle pix_rect)
        {
            Gdk.Pixbuf expander;

            if (IsExpanded)
                expander = new Gdk.Pixbuf(ExpandedIcon);
            else
                expander = new Gdk.Pixbuf(CollapsedIcon);

            context.Save();

            Gdk.CairoHelper.SetSourcePixbuf(context, expander, pix_rect.Left, pix_rect.Y);

            m_TextXOffset = expander.Width;

            context.Paint();
            context.Restore();
        }
    }
}
