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
        [GLib.Property("SkillMinsToNext")]
        public double SkillMinsToNext { get; set; }

        [GLib.Property("SkillName")]
        public string SkillName { get; set; }

        [GLib.Property("SkillRank")]
        public int SkillRank { get; set; }

        [GLib.Property("SkillLevel")]
        public int SkillLevel { get; set; }

        [GLib.Property("SkillCurrSP")]
        public long SkillCurrSP { get; set; }

        [GLib.Property("SkillNextSP")]
        public long SkillNextSP { get; set; }

        [GLib.Property("SkillLevlSP")]
        public long SkillLevlSP { get; set; }

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

            if (SkillLevel == -1)
            {
                RenderGroupCell(context, background_area, flags, widget);
            }
            else
            {
                context.Save();
                context.Antialias = Antialias.None;
                context.LineWidth = 1;
                context.MoveTo(expose_area.Left, expose_area.Bottom);
                context.LineTo(expose_area.Right, expose_area.Bottom);
                context.Restore();

                RenderSkillCell(context, pix_rect, flags, widget);
            }

            (context.Target as System.IDisposable).Dispose();
            (context as System.IDisposable).Dispose();
        }

        private void RenderSkillCell(Context context, Gdk.Rectangle pix_rect, CellRendererState flags, Widget widget)
        {
            int osOffset = 0;

            if(ECM.Helper.CurrentPlatform == ECM.Helper.Platform.Windows)
                osOffset = 1;

            // Render Progress bars
            context.Color = new Color(0.3, 0.3, 0.3);

            context.Save();
            int startX = pix_rect.Right - 48;
            context.Antialias = Antialias.None;
            context.LineWidth = 1;

            context.Rectangle(startX, pix_rect.Y + 2, 47, 9);

            context.Rectangle(startX, pix_rect.Y + 16, 47, 5);

            context.Stroke();
            context.Restore();

            context.Save();
            for (int i = 0; i < SkillLevel; i++)
            {
                context.Rectangle(startX + 2 + i * 9, pix_rect.Y + 3 + osOffset, 8, 6);
            }

            // Time Bar
            double fullWidth = 44;
            double dist = SkillNextSP - SkillLevlSP;
            double trav = SkillCurrSP - SkillLevlSP;
            double perc = 1;

            if(dist > 0)
                perc = Math.Max(0, trav / dist);

            context.Rectangle(startX + 2, pix_rect.Y + 17 + osOffset, fullWidth * perc, 2);

            context.Fill();
            context.Restore();

            // Render Skillbook Icon
            context.Save();
            Gdk.CairoHelper.SetSourcePixbuf(context, new Gdk.Pixbuf(ECM.Core.SkillbookPNG), pix_rect.X, pix_rect.Y);

            context.Rectangle(pix_rect.X, pix_rect.Y, 32, 32);

            context.Paint();
            context.Restore();

            // Render Text
            if (flags.HasFlag(CellRendererState.Selected))
                context.Color = new Color(1, 1, 1);
            else
                context.Color = new Color(0, 0, 0);

            Pango.Layout layout = Pango.CairoHelper.CreateLayout(context);

            context.Rectangle(pix_rect.X + 32, pix_rect.Y, 500, 32);

            layout.FontDescription = widget.PangoContext.FontDescription;
            layout.SetMarkup(string.Format("<span size=\"smaller\">{0} ({1}x)</span>\n<span size=\"smaller\" weight=\"bold\">SP {2:0,0}/{3:0,0}</span>", SkillName, SkillRank, SkillCurrSP, SkillNextSP));

            Pango.CairoHelper.UpdateLayout(context, layout);
            Pango.CairoHelper.ShowLayout(context, layout);

            layout = Pango.CairoHelper.CreateLayout(context);

            layout.FontDescription = widget.PangoContext.FontDescription;
            layout.Alignment = Pango.Alignment.Right;
            layout.SetMarkup(string.Format("<span size=\"small\">Level {0}\n{1}</span>", SkillLevel, ECM.Helper.GetDurationInWordsShort(TimeSpan.FromMinutes(SkillMinsToNext))));

            int w, h;
            layout.GetPixelSize(out w, out h);

            context.Rectangle(startX - (w + 6), pix_rect.Y, 500, 32);

            Pango.CairoHelper.UpdateLayout(context, layout);
            Pango.CairoHelper.ShowLayout(context, layout);
        }

        private void RenderGroupCell(Context context, Gdk.Rectangle pix_rect, CellRendererState flags, Widget widget)
        {
            // Draw backdrop
            context.Save();

            Gradient pat = new LinearGradient(pix_rect.Left, pix_rect.Top, pix_rect.Left, pix_rect.Bottom);
            pat.AddColorStop(0, new Color(0.4, 0.4, 0.4, 1));
            pat.AddColorStop(1, new Color(0.15, 0.15, 0.15, 1));
            context.Pattern = pat;

            context.Rectangle(pix_rect.X, pix_rect.Y, pix_rect.Width, pix_rect.Height);

            // Fill the path with pattern
            context.Fill();

            // We "undo" the pattern setting here
            context.Restore();

            context.Save();
            context.Antialias = Antialias.None;
            context.LineWidth = 1;
            context.Color = new Color(0.5, 0.5, 0.5);
            context.MoveTo(pix_rect.Left, pix_rect.Top);
            context.LineTo(pix_rect.Right, pix_rect.Top); 
            context.Stroke();
            context.Restore();

            Pango.Layout layout = Pango.CairoHelper.CreateLayout(context);

            context.Rectangle(pix_rect.X + 3, pix_rect.Y + 2, 500, 32);

            context.Color = new Color(1, 1, 1);

            string queueString = string.Empty;
            if(SkillRank != 0)
            {
                string colour = SkillRank < 0 ? "foreground=\"yellow\"" : string.Empty;
                queueString = string.Format("<span size=\"smaller\" {0}>    ({1} in queue)</span>", colour, Math.Abs(SkillRank));
            }

            layout.FontDescription = widget.PangoContext.FontDescription;
            layout.SetMarkup(string.Format("<span size=\"smaller\">{0} - {1} skills, {2:0,0} points</span>{3}", SkillName, SkillCurrSP, SkillNextSP, queueString));

            Pango.CairoHelper.UpdateLayout(context, layout);
            Pango.CairoHelper.ShowLayout(context, layout);
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
