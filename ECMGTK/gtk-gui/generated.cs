
// This file has been generated by the GUI designer. Do not modify.
namespace Stetic
{
	internal class Gui
	{
		private static bool initialized;
		
		internal static void Initialize (Gtk.Widget iconRenderer)
		{
			if ((Stetic.Gui.initialized == false)) {
				Stetic.Gui.initialized = true;
				global::Gtk.IconFactory w1 = new global::Gtk.IconFactory ();
				global::Gtk.IconSet w2 = new global::Gtk.IconSet (global::Gdk.Pixbuf.LoadFromResource ("ECMGTK.Resources.Icons.Home.png"));
				w1.Add ("Home", w2);
				global::Gtk.IconSet w3 = new global::Gtk.IconSet (global::Gdk.Pixbuf.LoadFromResource ("ECMGTK.Resources.Icons.CharSheet.png"));
				w1.Add ("CharSheet", w3);
				global::Gtk.IconSet w4 = new global::Gtk.IconSet (global::Gdk.Pixbuf.LoadFromResource ("ECMGTK.Resources.Icons.Contacts.png"));
				w1.Add ("Contacts", w4);
				global::Gtk.IconSet w5 = new global::Gtk.IconSet (global::Gdk.Pixbuf.LoadFromResource ("ECMGTK.Resources.Icons.Map.png"));
				w1.Add ("Map", w5);
				global::Gtk.IconSet w6 = new global::Gtk.IconSet (global::Gdk.Pixbuf.LoadFromResource ("ECMGTK.Resources.Icons.Assets.png"));
				w1.Add ("Assets", w6);
				global::Gtk.IconSet w7 = new global::Gtk.IconSet (global::Gdk.Pixbuf.LoadFromResource ("ECMGTK.Resources.Icons.Attributes.png"));
				w1.Add ("Attributes", w7);
				global::Gtk.IconSet w8 = new global::Gtk.IconSet (global::Gdk.Pixbuf.LoadFromResource ("ECMGTK.Resources.Icons.Certificates.png"));
				w1.Add ("Certificate", w8);
				global::Gtk.IconSet w9 = new global::Gtk.IconSet (global::Gdk.Pixbuf.LoadFromResource ("ECMGTK.Resources.Icons.Cog.png"));
				w1.Add ("Cog", w9);
				global::Gtk.IconSet w10 = new global::Gtk.IconSet (global::Gdk.Pixbuf.LoadFromResource ("ECMGTK.Resources.Icons.Contracts.png"));
				w1.Add ("Contracts", w10);
				global::Gtk.IconSet w11 = new global::Gtk.IconSet (global::Gdk.Pixbuf.LoadFromResource ("ECMGTK.Resources.Icons.Corporations.png"));
				w1.Add ("Corporations", w11);
				global::Gtk.IconSet w12 = new global::Gtk.IconSet (global::Gdk.Pixbuf.LoadFromResource ("ECMGTK.Resources.Icons.CustomChar.png"));
				w1.Add ("CustomChar", w12);
				global::Gtk.IconSet w13 = new global::Gtk.IconSet (global::Gdk.Pixbuf.LoadFromResource ("ECMGTK.Resources.Icons.Market.png"));
				w1.Add ("Market", w13);
				global::Gtk.IconSet w14 = new global::Gtk.IconSet (global::Gdk.Pixbuf.LoadFromResource ("ECMGTK.Resources.Icons.Mail.png"));
				w1.Add ("Mail", w14);
				global::Gtk.IconSet w15 = new global::Gtk.IconSet (global::Gdk.Pixbuf.LoadFromResource ("ECMGTK.Resources.Icons.Fitting.png"));
				w1.Add ("Fitting", w15);
				global::Gtk.IconSet w16 = new global::Gtk.IconSet (global::Gdk.Pixbuf.LoadFromResource ("ECMGTK.Resources.Icons.Research.png"));
				w1.Add ("Research", w16);
				global::Gtk.IconSet w17 = new global::Gtk.IconSet (global::Gdk.Pixbuf.LoadFromResource ("ECMGTK.Resources.Icons.Money.png"));
				w1.Add ("Money", w17);
				global::Gtk.IconSet w18 = new global::Gtk.IconSet (global::Gdk.Pixbuf.LoadFromResource ("ECMGTK.Resources.Icons.Skills.png"));
				w1.Add ("Skills", w18);
				global::Gtk.IconSet w19 = new global::Gtk.IconSet (global::Gdk.Pixbuf.LoadFromResource ("ECMGTK.Resources.Icons.Implants.png"));
				w1.Add ("Implants", w19);
				global::Gtk.IconSet w20 = new global::Gtk.IconSet (global::Gdk.Pixbuf.LoadFromResource ("ECMGTK.Resources.Icons.KillLogs.png"));
				w1.Add ("KillLog", w20);
				global::Gtk.IconSet w21 = new global::Gtk.IconSet (global::Gdk.Pixbuf.LoadFromResource ("ECMGTK.Resources.Icons.Standings.png"));
				w1.Add ("Standings", w21);
				global::Gtk.IconSet w22 = new global::Gtk.IconSet (global::Gdk.Pixbuf.LoadFromResource ("ECMGTK.Resources.Icons.Internet.png"));
				w1.Add ("Internet", w22);
				global::Gtk.IconSet w23 = new global::Gtk.IconSet (global::Gdk.Pixbuf.LoadFromResource ("ECMGTK.Resources.Icons.Medal.png"));
				w1.Add ("Medal", w23);
				w1.AddDefault ();
			}
		}
	}
	
	internal class ActionGroups
	{
		public static Gtk.ActionGroup GetActionGroup (System.Type type)
		{
			return Stetic.ActionGroups.GetActionGroup (type.FullName);
		}
		
		public static Gtk.ActionGroup GetActionGroup (string name)
		{
			return null;
		}
	}
}
