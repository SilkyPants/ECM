using System;
using Gtk;

namespace ECMGTK
{
    public class SkillProgressCellRenderer : CellRenderer
    {
        [GLib.Property("SkillLevel")]
        public int SkillLevel { get; set; }
        public float PercentToNextLevel { get; set; }

        public SkillProgressCellRenderer ()
        {
            SkillLevel = 5;
            PercentToNextLevel = .75f;
        }

        protected override void Render (Gdk.Drawable window, Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, Gdk.Rectangle expose_area, CellRendererState flags)
        {
            Gdk.Rectangle pix_rect = Gdk.Rectangle.Zero;

            this.GetSize (widget, ref cell_area, out pix_rect.X, out pix_rect.Y, out pix_rect.Width, out pix_rect.Height);

            // Take care of padding
            pix_rect.X += cell_area.X + (int) this.Xpad;
            pix_rect.Y += cell_area.Y + (int) this.Ypad;
            // Remove left, right, top and buttom borders which were added to the returned width
            pix_rect.Width  -= (int) this.Xpad * 2;
            pix_rect.Height -= (int) this.Ypad * 2;

            Gdk.GC gc = new Gdk.GC(window);

            Gdk.Rectangle levelRect = new Gdk.Rectangle(pix_rect.X, pix_rect.Y + 6, 47, 9);

            window.DrawRectangle(gc, false, levelRect);

            levelRect.Inflate(-2, -2);

            for(int i = 0; i < SkillLevel; i++)
            {
                Gdk.Rectangle levelFill = new Gdk.Rectangle(levelRect.X + i * 9, levelRect.Y, 8, 6);

                window.DrawRectangle(gc, true, levelFill);
            }

            Gdk.Rectangle trainProgressRect = new Gdk.Rectangle(pix_rect.X, pix_rect.Y + 21, 47, 5);

            window.DrawRectangle(gc, false, trainProgressRect);

            trainProgressRect.Inflate(-2, -2);

            float w = trainProgressRect.Width;
            w *= PercentToNextLevel;
            trainProgressRect.Width = (int)w;
            trainProgressRect.Height = 2;

            window.DrawRectangle(gc, true, trainProgressRect);

            gc.Dispose();
        }

        public override void GetSize (Widget widget, ref Gdk.Rectangle cell_area, out int x_offset, out int y_offset, out int width, out int height)
        {
            width = cell_area.Width;
            height = cell_area.Height;
            
            if (cell_area != Gdk.Rectangle.Zero)
            {
                if (widget.Direction == Gtk.TextDirection.Rtl)
                    x_offset =  (int) ((1.0 - this.Xalign) * (cell_area.Width - width));
                else
                    x_offset = (int) (this.Xalign * (cell_area.Width - width));

                x_offset = System.Math.Max (x_offset, 0);
                
                y_offset = (int) (this.Yalign * (cell_area.Height - height));
                y_offset = System.Math.Max (y_offset, 0);
            }
            else
            {
                x_offset = 0;
                y_offset = 0;
            }
        }
    }
}

