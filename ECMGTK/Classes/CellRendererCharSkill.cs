using System;
using System.Collections.Generic;
using System.Text;
using Gtk;
using Cairo;

namespace ECMGTK
{
    public class CellRendererCharSkill : CellRendererEveTree
    {
        [GLib.Property("SkillMinsToNext")]
        public double SkillMinsToNext { get; set; }

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

        protected override int CellHeight
        {
            get { return IsHeading ? 16 : 32; }
        }

        public CellRendererCharSkill()
        {
        }

        protected override void RenderCell(Context context, Gdk.Rectangle cell_rect, bool isSelected)
        {
            if (IsHeading)
            {
                RenderGroupCell(context, cell_rect);
            }
            else
            {
                RenderSkillCell(context, cell_rect, isSelected);
            }
        }

        private void RenderSkillCell(Context context, Gdk.Rectangle pix_rect, bool isSelected)
        {
            int osOffset = 0;
            int startX = pix_rect.Right - 64;

            if(ECM.Helper.CurrentPlatform == ECM.Helper.Platform.Windows)
                osOffset = 1;

            // Render Text
            context.Save();
            
            Color col = Black;

            if (isSelected)
                col = White;

            string s = string.Format("{0} ({1}x)", Text, SkillRank);
            TextExtents te = context.TextExtents(s);
            RenderText(context, s, pix_rect.X + 32, pix_rect.Y + 2, col);

            s = string.Format("Level {0}", SkillLevel);
            te = context.TextExtents(s);
            RenderText(context, s, startX - (te.Width + 6), pix_rect.Y + 2, col);

            s = ECM.Helper.GetDurationInWordsShort(TimeSpan.FromMinutes(SkillMinsToNext));
            te = context.TextExtents(s);
            RenderText(context, s, startX - (te.Width + 6), pix_rect.Y + te.Height + 6, col);

            s = string.Format("SP {0:0,0}/{1:0,0}", SkillCurrSP, SkillNextSP);
            context.SelectFontFace(m_FontDesc.Family, FontSlant.Normal, FontWeight.Bold);
            te = context.TextExtents(s);

            RenderText(context, s, pix_rect.X + 32, pix_rect.Y + te.Height + 6, col);

            context.Restore();

            // Render Progress bars
            context.Save();

            context.Color = DarkGray;
            context.Antialias = Antialias.None;
            context.LineWidth = 1;

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
            
            context.Rectangle(startX, pix_rect.Y + 2, 47, 9);

            context.Rectangle(startX, pix_rect.Y + 16, 47, 5);

            context.Stroke();

            context.Restore();

            // Render Skillbook Icon
            context.Save();
            Gdk.CairoHelper.SetSourcePixbuf(context, new Gdk.Pixbuf(ECM.Core.SkillbookPNG), pix_rect.X, pix_rect.Y);

            context.Paint();
            context.Restore();
        }

        private void RenderGroupCell(Context context, Gdk.Rectangle pix_rect)
        {
            string text = string.Format("{0} - {1} skills, {2:0,0} points", Text, SkillCurrSP, SkillNextSP);
            
            TextExtents te = RenderText(context, text, pix_rect.X, pix_rect.Y + 2, White);
            
            if(SkillRank != 0)
            {
                text = string.Format("    ({0} in queue)", Math.Abs(SkillRank));

                RenderText(context, text, pix_rect.X + te.Width, pix_rect.Y + 2, Yellow);
            }
        }
    }
}
