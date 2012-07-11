using System;
using System.Collections.Generic;
using System.Text;
using Gtk;
using Cairo;

namespace ECMGTK
{
    public class CellRendererEveTree : CellRenderer
    {
        [GLib.Property("Icon")]
        public Gdk.Pixbuf Icon { get; set; }

        [GLib.Property("Text")]
        public string Text { get; set; }

        [GLib.Property("IsHeading")]
        public bool IsHeading { get; set; }


        public bool RenderInfo { get; set; }

        int m_TextXOffset = 0;
        protected TreeView m_Tree = null;

        protected Pango.FontDescription m_FontDesc = null;

        static readonly Color m_BackdropHeadingColour = new Color(0.09, 0.09, 0.09);
        static readonly Color m_BackdropItemColour = new Color(0.15, 0.15, 0.15);

        protected static readonly Color Black = new Color(0, 0, 0);
        protected static readonly Color White = new Color(1, 1, 1);
        protected static readonly Color Gray = new Color(0.57, 0.57, 0.57);
        protected static readonly Color DarkGray = new Color(0.36, 0.36, 0.36);
        protected static readonly Color Yellow = new Color(0.83, 0.70, 0.01);

        protected virtual int CellHeight
        {
            get 
            {
                if (Icon != null)
                    return Icon.Height;

                return 16; 
            }
        }

        public CellRendererEveTree()
        {
            RenderInfo = true;
        }

        protected override void Render (Gdk.Drawable window, Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, Gdk.Rectangle expose_area, CellRendererState flags)
        {
            this.
            m_Tree = widget as TreeView;
            m_TextXOffset = 0;

            Gdk.Rectangle pix_rect = Gdk.Rectangle.Zero;
            bool isSelected = flags.HasFlag(CellRendererState.Selected);
            m_FontDesc = widget.PangoContext.FontDescription;

            this.GetSize (widget, ref cell_area, out pix_rect.X, out pix_rect.Y, out pix_rect.Width, out pix_rect.Height);

            // Take care of padding
            pix_rect.X += cell_area.X + (int)this.Xpad;
            pix_rect.Y += cell_area.Y + (int)this.Ypad;
            // Remove left, right, top and buttom borders which were added to the returned width
            pix_rect.Width -= (int)this.Xpad * 2;
            pix_rect.Height -= (int)this.Ypad * 2;

            // Create Context
            Cairo.Context context = Gdk.CairoHelper.Create(window);
            context.SelectFontFace(m_FontDesc.Family, FontSlant.Normal, FontWeight.Normal);

            // Draw backdrop
            //if (IsHeading) 
            {
                context.Save ();

                Color col = IsHeading ? m_BackdropHeadingColour : m_BackdropItemColour;

                context.Color = col;
                context.SetSourceRGB (col.R, col.G, col.B);

                context.Rectangle (background_area.X, background_area.Y, background_area.Width, background_area.Height);

                // Fill the path with pattern
                context.Fill ();

                // We "undo" the pattern setting here
                context.Restore ();
            }

            RenderCell(context, pix_rect, isSelected);

            // Draw expander/Info
            System.IO.Stream pixBuf;

            if (!IsHeading && RenderInfo)
                pixBuf = ECM.Core.Info16PNG;
            else if (IsExpanded)
                pixBuf = ECM.Core.Up16PNG;
            else
                pixBuf = ECM.Core.Down16PNG;

            context.Save();
            Gdk.CairoHelper.SetSourcePixbuf(context, new Gdk.Pixbuf(pixBuf), pix_rect.Right - 16, pix_rect.Y);

            context.Paint();
            context.Restore();

            // Draw line under cell
            int bottom = background_area.Bottom;

            if (ECM.Helper.CurrentPlatform != ECM.Helper.Platform.Windows)
                bottom += 1;
            else
                bottom -= 1;

            context.Save();
            context.Antialias = Antialias.None;
            context.LineWidth = 1;
            context.Color = DarkGray;
            context.MoveTo(background_area.Left, bottom);
            context.LineTo(background_area.Right, bottom);
            context.Stroke();
            context.Restore();

            (context.Target as System.IDisposable).Dispose();
            (context as System.IDisposable).Dispose();
        }

        protected virtual void RenderCell(Context context, Gdk.Rectangle pix_rect, bool isSelected)
        {
            // Draw Icon if needed
            if (Icon != null)
            {
                context.Save();
                Gdk.CairoHelper.SetSourcePixbuf(context, Icon, pix_rect.Left, pix_rect.Y);

                context.Paint();
                context.Restore();

                m_TextXOffset = Icon.Width;
            }

            // Render Text
            Color colour = White;

            //if (!isSelected && !IsHeading)
            //    colour = Black;

            string text = Text;
            TextExtents te = context.TextExtents(text);
            int subIdx = Text.Length;

            while (te.Width > pix_rect.Width - 16 - m_TextXOffset + 2)
            {
                text = string.Format("{0}...", text.Substring(0, --subIdx).TrimEnd());
                te = context.TextExtents(text);
            }

            RenderText(context, text, pix_rect.X + m_TextXOffset + 2, pix_rect.Y + 2, colour);
        }

        protected TextExtents RenderText(Context context, string text, double x, double y, Color colour)
        {
            TextExtents extents = context.TextExtents(text);

            context.Save();

            context.Color = colour;

            context.MoveTo(x, y + extents.Height); // we add the height as it seems to be top left, not bottom left
            context.ShowText(text);

            context.Restore();

            return extents;
        }

        public override void GetSize(Widget widget, ref Gdk.Rectangle cell_area, out int x_offset, out int y_offset, out int width, out int height)
        {
            x_offset = y_offset = width = height = 0;

            width = cell_area.Width;
            height = CellHeight;

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

