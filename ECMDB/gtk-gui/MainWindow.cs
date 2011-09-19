
// This file has been generated by the GUI designer. Do not modify.

public partial class MainWindow
{
	private global::Gtk.VBox vbox1;
	private global::Gtk.HBox hbox1;
	private global::Gtk.Notebook notebook1;
	private global::Gtk.Table table1;
	private global::Gtk.Label label2;
	private global::Gtk.Table table2;
	private global::Gtk.Label label3;
	private global::Gtk.FileChooserButton filechooserbutton1;
	private global::Gtk.Label label4;
	private global::Gtk.VButtonBox vbuttonbox1;
	private global::Gtk.Button button1;
	private global::Gtk.Label lblStatus;
	private global::Gtk.Label lblRecords;
	private global::Gtk.ProgressBar pgbProgress;
    
	protected virtual void Build ()
	{
		global::Stetic.Gui.Initialize (this);
		// Widget MainWindow
		this.Name = "MainWindow";
		this.Title = global::Mono.Unix.Catalog.GetString ("MainWindow");
		this.WindowPosition = ((global::Gtk.WindowPosition)(4));
		this.BorderWidth = ((uint)(3));
		// Container child MainWindow.Gtk.Container+ContainerChild
		this.vbox1 = new global::Gtk.VBox ();
		this.vbox1.Name = "vbox1";
		this.vbox1.Spacing = 6;
		// Container child vbox1.Gtk.Box+BoxChild
		this.hbox1 = new global::Gtk.HBox ();
		this.hbox1.Name = "hbox1";
		// Container child hbox1.Gtk.Box+BoxChild
		this.notebook1 = new global::Gtk.Notebook ();
		this.notebook1.CanFocus = true;
		this.notebook1.Name = "notebook1";
		this.notebook1.CurrentPage = 2;
		this.notebook1.BorderWidth = ((uint)(3));
		// Container child notebook1.Gtk.Notebook+NotebookChild
		this.table1 = new global::Gtk.Table (((uint)(3)), ((uint)(3)), false);
		this.table1.Name = "table1";
		this.table1.RowSpacing = ((uint)(6));
		this.table1.ColumnSpacing = ((uint)(6));
		this.notebook1.Add (this.table1);
		// Notebook tab
		this.label2 = new global::Gtk.Label ();
		this.label2.Name = "label2";
		this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("MS SQL");
		this.notebook1.SetTabLabel (this.table1, this.label2);
		this.label2.ShowAll ();
		// Container child notebook1.Gtk.Notebook+NotebookChild
		this.table2 = new global::Gtk.Table (((uint)(3)), ((uint)(3)), false);
		this.table2.Name = "table2";
		this.table2.RowSpacing = ((uint)(6));
		this.table2.ColumnSpacing = ((uint)(6));
		this.notebook1.Add (this.table2);
		global::Gtk.Notebook.NotebookChild w2 = ((global::Gtk.Notebook.NotebookChild)(this.notebook1 [this.table2]));
		w2.Position = 1;
		// Notebook tab
		this.label3 = new global::Gtk.Label ();
		this.label3.Name = "label3";
		this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("MySql");
		this.notebook1.SetTabLabel (this.table2, this.label3);
		this.label3.ShowAll ();
		// Container child notebook1.Gtk.Notebook+NotebookChild
		this.filechooserbutton1 = new global::Gtk.FileChooserButton (global::Mono.Unix.Catalog.GetString ("Select A File"), ((global::Gtk.FileChooserAction)(0)));
		this.filechooserbutton1.Name = "filechooserbutton1";
		this.notebook1.Add (this.filechooserbutton1);
		global::Gtk.Notebook.NotebookChild w3 = ((global::Gtk.Notebook.NotebookChild)(this.notebook1 [this.filechooserbutton1]));
		w3.Position = 2;
		// Notebook tab
		this.label4 = new global::Gtk.Label ();
		this.label4.Name = "label4";
		this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("SQLite");
		this.notebook1.SetTabLabel (this.filechooserbutton1, this.label4);
		this.label4.ShowAll ();
		this.hbox1.Add (this.notebook1);
		global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.notebook1]));
		w4.Position = 0;
		// Container child hbox1.Gtk.Box+BoxChild
		this.vbuttonbox1 = new global::Gtk.VButtonBox ();
		this.vbuttonbox1.Name = "vbuttonbox1";
		this.vbuttonbox1.BorderWidth = ((uint)(3));
		this.vbuttonbox1.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
		// Container child vbuttonbox1.Gtk.ButtonBox+ButtonBoxChild
		this.button1 = new global::Gtk.Button ();
		this.button1.CanFocus = true;
		this.button1.Name = "button1";
		this.button1.UseUnderline = true;
		this.button1.Label = global::Mono.Unix.Catalog.GetString ("GtkButton");
		this.vbuttonbox1.Add (this.button1);
		global::Gtk.ButtonBox.ButtonBoxChild w5 = ((global::Gtk.ButtonBox.ButtonBoxChild)(this.vbuttonbox1 [this.button1]));
		w5.Expand = false;
		w5.Fill = false;
		this.hbox1.Add (this.vbuttonbox1);
		global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.vbuttonbox1]));
		w6.Position = 1;
		w6.Expand = false;
		w6.Fill = false;
		this.vbox1.Add (this.hbox1);
		global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.hbox1]));
		w7.Position = 0;
		// Container child vbox1.Gtk.Box+BoxChild
		this.lblStatus = new global::Gtk.Label ();
		this.lblStatus.Name = "lblStatus";
		this.lblStatus.LabelProp = global::Mono.Unix.Catalog.GetString ("label1");
		this.vbox1.Add (this.lblStatus);
		global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.lblStatus]));
		w8.Position = 1;
		w8.Expand = false;
		w8.Fill = false;
		// Container child vbox1.Gtk.Box+BoxChild
		this.lblRecords = new global::Gtk.Label ();
		this.lblRecords.Name = "lblRecords";
		this.lblRecords.LabelProp = global::Mono.Unix.Catalog.GetString ("label4");
		this.vbox1.Add (this.lblRecords);
		global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.lblRecords]));
		w9.Position = 2;
		w9.Expand = false;
		w9.Fill = false;
		// Container child vbox1.Gtk.Box+BoxChild
		this.pgbProgress = new global::Gtk.ProgressBar ();
		this.pgbProgress.Name = "pgbProgress";
		this.vbox1.Add (this.pgbProgress);
		global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.pgbProgress]));
		w10.PackType = ((global::Gtk.PackType)(1));
		w10.Position = 3;
		w10.Expand = false;
		w10.Fill = false;
		this.Add (this.vbox1);
		if ((this.Child != null)) {
			this.Child.ShowAll ();
		}
		this.DefaultWidth = 400;
		this.DefaultHeight = 158;
		this.Show ();
		this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
	}
}
