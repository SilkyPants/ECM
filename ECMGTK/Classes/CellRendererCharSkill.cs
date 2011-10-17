using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gtk;
using Cairo;

namespace ECMGTK
{
    public class CellRendererCharSkill : CellRenderer
    {
        public TimeSpan TimeToNextLevel { get; set; }
        public string SkillName { get; set; }
        public int SkillRank { get; set; }
        public int SkillLevel { get; set; }
        public long SkillPoints { get; set; }
        public long PointsAtNext { get; set; }

        public CellRendererCharSkill()
        {
        }

        protected override void Render(Gdk.Drawable window, Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, Gdk.Rectangle expose_area, CellRendererState flags)
        {
            Gdk.Rectangle pix_rect = Gdk.Rectangle.Zero;

            this.GetSize(widget, ref cell_area, out pix_rect.X, out pix_rect.Y, out pix_rect.Width, out pix_rect.Height);

            // Take care of padding
            pix_rect.X += cell_area.X + (int)this.Xpad;
            pix_rect.Y += cell_area.Y + (int)this.Ypad;
            // Remove left, right, top and buttom borders which were added to the returned width
            pix_rect.Width -= (int)this.Xpad * 2;
            pix_rect.Height -= (int)this.Ypad * 2;

            Cairo.Context context = Gdk.CairoHelper.Create(window);

            int offset = 0;

            if (SkillLevel != -1)
            {
                Gdk.CairoHelper.SetSourcePixbuf(context, new Gdk.Pixbuf(ECM.Core.SkillbookPNG), pix_rect.X, pix_rect.Y);

                context.Rectangle(pix_rect.X, pix_rect.Y, 32, 32);

                context.Paint();

                offset += 32;
            }

            Pango.Layout layout = Pango.CairoHelper.CreateLayout(context);

            context.Rectangle(pix_rect.X + offset, pix_rect.Y, 500, 32);

            if(flags.HasFlag(CellRendererState.Selected))
                context.Color = new Color(1, 1, 1);
            else
                context.Color = new Color(0, 0, 0);

            layout.FontDescription = widget.PangoContext.FontDescription;
            layout.SetMarkup(string.Format("<b>{0}</b>", SkillName));

            Pango.CairoHelper.UpdateLayout(context, layout);
            Pango.CairoHelper.ShowLayout(context, layout);


            (context.Target as System.IDisposable).Dispose();
            (context as System.IDisposable).Dispose();
        }

        public override void GetSize(Widget widget, ref Gdk.Rectangle cell_area, out int x_offset, out int y_offset, out int width, out int height)
        {
            x_offset = y_offset = width = height = 0;

            width = cell_area.Width;
            height = SkillLevel == -1 ? 16 : 32; // cell_area.Height;

            if (cell_area != Gdk.Rectangle.Zero)
            {
                if (widget.Direction == Gtk.TextDirection.Rtl)
                    x_offset = (int)((1.0 - this.Xalign) * (cell_area.Width - width));
                else
                    x_offset = (int)(this.Xalign * (cell_area.Width - width));

                x_offset = System.Math.Max(x_offset, 0);

                y_offset = (int)(this.Yalign * (cell_area.Height - height));
                y_offset = System.Math.Max(y_offset, 0);
            }
            else
            {
                x_offset = 0;
                y_offset = 0;
            }
        }
    }
}
