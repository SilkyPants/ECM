using System;
using System.Collections.Generic;
using System.Text;
using Gtk;
using Cairo;

namespace ECMGTK
{
    public class CellRendererMarketItem : CellRenderer
    {
        [GLib.Property("Icon")]
        public Gdk.Pixbuf Icon { get; set; }

        [GLib.Property("ItemName")]
        public string ItemName { get; set; }

        [GLib.Property("ID")]
        public long ID { get; set; }

        [GLib.Property("HasItems")]
        public bool HasItems { get; set; }

        [GLib.Property("IsItem")]
        public bool IsItem { get; set; }

        static readonly Color m_BackdropColour = new Color(.19, .18, .17);

        public CellRendererMarketItem()
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

            // Draw backdrop
            if(!IsItem)
            {
                context.Save();

                //Gradient pat = new LinearGradient(background_area.Left, background_area.Top, background_area.Left, background_area.Bottom);
                //pat.AddColorStop(0, new Color(0.4, 0.4, 0.4, 1));
                //pat.AddColorStop(1, new Color(0.15, 0.15, 0.15, 1));
                //context.Pattern = pat;

                context.Color = m_BackdropColour;
                context.SetSourceRGB(m_BackdropColour.R, m_BackdropColour.G, m_BackdropColour.B);

                context.Rectangle(background_area.X, background_area.Y, background_area.Width, background_area.Height);

                // Fill the path with pattern
                context.Fill();

                // We "undo" the pattern setting here
                context.Restore();
            }

            // Draw Icon if needed
            int textXOffset = 0;
            if (Icon != null)
            {
                context.Save();
                Gdk.CairoHelper.SetSourcePixbuf(context, Icon, cell_area.Left, cell_area.Y);

                context.Paint();
                context.Restore();

                textXOffset = Icon.Width;
            }

            // Render Text
            context.Save();

            context.Color = new Color(1, 1, 1);

            if (!flags.HasFlag(CellRendererState.Selected) && IsItem)
                context.Color = new Color(0, 0, 0);

            context.SelectFontFace(widget.PangoContext.FontDescription.Family, FontSlant.Normal, FontWeight.Normal);

            string text = ItemName;
            TextExtents te = context.TextExtents(text);
            int subIdx = ItemName.Length;

            while (te.Width > cell_area.Width - 16 - textXOffset)
            {
                text = string.Format("{0}...", text.Substring(0, --subIdx).TrimEnd());
                te = context.TextExtents(text);
            }

            context.MoveTo(cell_area.X + textXOffset, cell_area.Y + te.Height + 2);
            context.ShowText(text);

            context.Restore();

            // Draw expander/Info
            System.IO.Stream pixBuf;

            if(IsItem)
                pixBuf = ECM.Core.Info16PNG;
            else if(IsExpanded)
                pixBuf = ECM.Core.Up16PNG;
            else
                pixBuf = ECM.Core.Down16PNG;

            context.Save();
            Gdk.CairoHelper.SetSourcePixbuf(context, new Gdk.Pixbuf(pixBuf), background_area.Right - 16, background_area.Y);

            context.Paint();
            context.Restore();

            // Draw line under cell
            context.Save();
            context.Antialias = Antialias.None;
            context.LineWidth = 1;
            context.Color = new Color(0.5, 0.5, 0.5);
            context.MoveTo(background_area.Left, background_area.Bottom - 1);
            context.LineTo(background_area.Right, background_area.Bottom - 1);
            context.Stroke();
            context.Restore();

            (context.Target as System.IDisposable).Dispose();
            (context as System.IDisposable).Dispose();
        }

        public override void GetSize(Widget widget, ref Gdk.Rectangle cell_area, out int x_offset, out int y_offset, out int width, out int height)
        {
            x_offset = y_offset = width = height = 0;

            width = cell_area.Width;
            height = 16;

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

