using System;
using System.Collections.Generic;
using System.Text;
using Gtk;
using Cairo;
using System.IO;


namespace ECMGTK
{
    public class CellRendererCertificate : CellRendererEveTree
    {
        [GLib.Property("CertGrade")]
        public int CertGrade { get; set; }

        protected override int CellHeight
        {
            get { return IsHeading ? 16 : 32; }
        }

        protected override void RenderCell(Context context, Gdk.Rectangle pix_rect, bool isSelected)
        {
            if (!IsHeading)
            {
                Stream certIconStream = ECM.Core.CertGrade0PNG;
                string certGradeString = "None";

                if (CertGrade == 1)
                {
                    certIconStream = ECM.Core.CertGrade1PNG;
                    certGradeString = "Basic";
                }
                else if (CertGrade == 2)
                {
                    certIconStream = ECM.Core.CertGrade2PNG;
                    certGradeString = "Standard";
                }
                else if (CertGrade == 3)
                {
                    certIconStream = ECM.Core.CertGrade3PNG;
                    certGradeString = "Improved";
                }
                else if (CertGrade == 4)
                {
                    certIconStream = ECM.Core.CertGrade4PNG;
                    certGradeString = "None";
                }
                else if (CertGrade == 5)
                {
                    certIconStream = ECM.Core.CertGrade5PNG;
                    certGradeString = "Elite";
                }

                int osOffset = 0;
                int startX = pix_rect.Right - 64;

                if (ECM.Helper.CurrentPlatform == ECM.Helper.Platform.Windows)
                    osOffset = 1;

                // Render Text
                context.Save();

                Color col = Black;

                if (isSelected)
                    col = White;

                TextExtents te = context.TextExtents(certGradeString);
                RenderText(context, certGradeString, startX - te.Width - 5, pix_rect.Y + 2, col);

                RenderText(context, Text, pix_rect.X + 32, pix_rect.Y + 2, col);

                context.Restore();

                // Render Progress bars
                context.Save();

                context.Color = DarkGray;
                context.Antialias = Antialias.None;
                context.LineWidth = 1;

                for (int i = 0; i < CertGrade; i++)
                {
                    context.Rectangle(startX + 2 + i * 9, pix_rect.Y + 3 + osOffset, 8, 6);
                }

                context.Fill();

                context.Rectangle(startX, pix_rect.Y + 2, 47, 9);

                context.Stroke();

                context.Restore();

                // Render Certificate Icon
                context.Save();

                Gdk.CairoHelper.SetSourcePixbuf(context, new Gdk.Pixbuf(certIconStream), pix_rect.X, pix_rect.Y);

                context.Paint();
                context.Restore();
            }
            else
                base.RenderCell(context, pix_rect, isSelected);
        }
    }
}
