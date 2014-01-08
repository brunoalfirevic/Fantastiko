using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Text.RegularExpressions;
using System.IO;
using JezicniProcesor;


namespace UserInterface
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem nemuNew;
		private System.Windows.Forms.MenuItem menuLoad;
		private System.Windows.Forms.MenuItem menuSave;
		private System.Windows.Forms.MenuItem menuExit;
		private System.ComponentModel.IContainer components;

		private bool populating = true;
		private Bojac TheBojac;
		private int razmak=95,razv=1;
		private int _brojacDokumenata=0;
		private FlickerFreeRichEditTextBox textbox;
		private Color _bojaPozadine;
		private Color _bojaSlova;
		private Color _bojaKljucnihRijeci;
		private Color _bojaIdentifikatora;
		private ArrayList Boje = new ArrayList();

		private System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		private System.Windows.Forms.ToolBarButton toolBarButton2;
		private System.Windows.Forms.ToolBarButton toolBarButton3;
		private System.Windows.Forms.ToolBarButton toolBarButton4;
		private System.Windows.Forms.ToolBarButton toolBarButton5;
		private System.Windows.Forms.ToolBarButton toolBarButton6;
		private System.Windows.Forms.ToolBarButton toolBarButton7;
		private System.Windows.Forms.ImageList slicice;
		private System.Windows.Forms.ToolBarButton toolBarButton8;
		private System.Windows.Forms.ToolBarButton toolBarButton9;
		private System.Windows.Forms.TabControl tab1;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.ToolBarButton toolBarButton10;
		private System.Windows.Forms.ToolBarButton toolBarButton11;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem menuItem8;
		private System.Windows.Forms.MenuItem menuItem9;
		private System.Windows.Forms.OpenFileDialog openFileDialog2;
		private System.Windows.Forms.MenuItem menuItem11;
		private System.Windows.Forms.ToolBarButton toolBarButton13;
		private System.Windows.Forms.ToolBarButton toolBarButton12;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			TheBojac = new Bojac(/*"pascal.txt"*/);  // pascal.txt se nalazi u debug direktoriju
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Form1));
			this.button1 = new System.Windows.Forms.Button();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.button3 = new System.Windows.Forms.Button();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.nemuNew = new System.Windows.Forms.MenuItem();
			this.menuLoad = new System.Windows.Forms.MenuItem();
			this.menuSave = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuExit = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItem7 = new System.Windows.Forms.MenuItem();
			this.menuItem11 = new System.Windows.Forms.MenuItem();
			this.menuItem9 = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.menuItem8 = new System.Windows.Forms.MenuItem();
			this.toolBar1 = new System.Windows.Forms.ToolBar();
			this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton2 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton3 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton4 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton5 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton6 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton7 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton12 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton8 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton9 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton13 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton10 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton11 = new System.Windows.Forms.ToolBarButton();
			this.slicice = new System.Windows.Forms.ImageList(this.components);
			this.tab1 = new System.Windows.Forms.TabControl();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(0, 0);
			this.button1.Name = "button1";
			this.button1.TabIndex = 9;
			// 
			// listBox1
			// 
			this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.listBox1.Location = new System.Drawing.Point(0, 352);
			this.listBox1.Name = "listBox1";
			this.listBox1.ScrollAlwaysVisible = true;
			this.listBox1.Size = new System.Drawing.Size(664, 95);
			this.listBox1.TabIndex = 3;
			this.listBox1.Visible = false;
			this.listBox1.DoubleClick += new System.EventHandler(this.listBox1_SelectedIndexChanged);
			this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(0, 0);
			this.button3.Name = "button3";
			this.button3.TabIndex = 9;
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem1,
																					  this.menuItem2,
																					  this.menuItem7,
																					  this.menuItem6});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.nemuNew,
																					  this.menuLoad,
																					  this.menuSave,
																					  this.menuItem4,
																					  this.menuExit});
			this.menuItem1.Text = "File";
			// 
			// nemuNew
			// 
			this.nemuNew.Index = 0;
			this.nemuNew.Text = "New";
			this.nemuNew.Click += new System.EventHandler(this.menuItem2_Click);
			// 
			// menuLoad
			// 
			this.menuLoad.Index = 1;
			this.menuLoad.Text = "Load...";
			this.menuLoad.Click += new System.EventHandler(this.menuItem3_Click);
			// 
			// menuSave
			// 
			this.menuSave.Index = 2;
			this.menuSave.Text = "Save...";
			this.menuSave.Click += new System.EventHandler(this.menuItem4_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 3;
			this.menuItem4.Text = "-";
			// 
			// menuExit
			// 
			this.menuExit.Index = 4;
			this.menuExit.Text = "Exit";
			this.menuExit.Click += new System.EventHandler(this.menuItem5_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 1;
			this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem3});
			this.menuItem2.Text = "Edit";
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 0;
			this.menuItem3.Text = "Refresh";
			this.menuItem3.Click += new System.EventHandler(this.menuItem3_Click_1);
			// 
			// menuItem7
			// 
			this.menuItem7.Index = 2;
			this.menuItem7.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem11,
																					  this.menuItem9});
			this.menuItem7.Text = "&Compile";
			// 
			// menuItem11
			// 
			this.menuItem11.Index = 0;
			this.menuItem11.Text = "Compile";
			this.menuItem11.Click += new System.EventHandler(this.menuItem11_Click);
			// 
			// menuItem9
			// 
			this.menuItem9.Index = 1;
			this.menuItem9.MdiList = true;
			this.menuItem9.Text = "Run";
			this.menuItem9.Click += new System.EventHandler(this.menuItem9_Click);
			// 
			// menuItem6
			// 
			this.menuItem6.Index = 3;
			this.menuItem6.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem5});
			this.menuItem6.Text = "Help";
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 0;
			this.menuItem5.Text = "About";
			this.menuItem5.Click += new System.EventHandler(this.menuItem5_Click_1);
			// 
			// menuItem8
			// 
			this.menuItem8.Index = -1;
			this.menuItem8.Text = "";
			// 
			// toolBar1
			// 
			this.toolBar1.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
			this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																						this.toolBarButton1,
																						this.toolBarButton2,
																						this.toolBarButton3,
																						this.toolBarButton4,
																						this.toolBarButton5,
																						this.toolBarButton6,
																						this.toolBarButton7,
																						this.toolBarButton12,
																						this.toolBarButton8,
																						this.toolBarButton9,
																						this.toolBarButton13,
																						this.toolBarButton10,
																						this.toolBarButton11});
			this.toolBar1.DropDownArrows = true;
			this.toolBar1.ImageList = this.slicice;
			this.toolBar1.Location = new System.Drawing.Point(0, 0);
			this.toolBar1.Name = "toolBar1";
			this.toolBar1.ShowToolTips = true;
			this.toolBar1.Size = new System.Drawing.Size(664, 28);
			this.toolBar1.TabIndex = 8;
			this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
			// 
			// toolBarButton1
			// 
			this.toolBarButton1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// toolBarButton2
			// 
			this.toolBarButton2.ImageIndex = 0;
			this.toolBarButton2.ToolTipText = "New";
			// 
			// toolBarButton3
			// 
			this.toolBarButton3.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// toolBarButton4
			// 
			this.toolBarButton4.ImageIndex = 1;
			this.toolBarButton4.ToolTipText = "Open";
			// 
			// toolBarButton5
			// 
			this.toolBarButton5.Enabled = false;
			this.toolBarButton5.ImageIndex = 2;
			this.toolBarButton5.ToolTipText = "Save";
			// 
			// toolBarButton6
			// 
			this.toolBarButton6.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// toolBarButton7
			// 
			this.toolBarButton7.Enabled = false;
			this.toolBarButton7.ImageIndex = 4;
			this.toolBarButton7.ToolTipText = "Refresh";
			// 
			// toolBarButton12
			// 
			this.toolBarButton12.ImageIndex = 6;
			this.toolBarButton12.ToolTipText = "Properties";
			// 
			// toolBarButton8
			// 
			this.toolBarButton8.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// toolBarButton9
			// 
			this.toolBarButton9.Enabled = false;
			this.toolBarButton9.ImageIndex = 3;
			this.toolBarButton9.ToolTipText = "Compile";
			// 
			// toolBarButton13
			// 
			this.toolBarButton13.Enabled = false;
			this.toolBarButton13.ImageIndex = 7;
			this.toolBarButton13.ToolTipText = "Run";
			// 
			// toolBarButton10
			// 
			this.toolBarButton10.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// toolBarButton11
			// 
			this.toolBarButton11.Enabled = false;
			this.toolBarButton11.ImageIndex = 5;
			this.toolBarButton11.ToolTipText = "Close";
			// 
			// slicice
			// 
			this.slicice.ImageSize = new System.Drawing.Size(16, 16);
			this.slicice.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("slicice.ImageStream")));
			this.slicice.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// tab1
			// 
			this.tab1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tab1.ItemSize = new System.Drawing.Size(58, 18);
			this.tab1.Location = new System.Drawing.Point(0, 32);
			this.tab1.Name = "tab1";
			this.tab1.SelectedIndex = 0;
			this.tab1.Size = new System.Drawing.Size(664, 416);
			this.tab1.TabIndex = 7;
			this.tab1.SelectedIndexChanged += new System.EventHandler(this.tab1_SelectedIndexChanged);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(664, 449);
			this.Controls.Add(this.toolBar1);
			this.Controls.Add(this.tab1);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.button1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Menu = this.mainMenu1;
			this.Name = "Form1";
			this.Text = "AFJJP2 - Fantastiko";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		
		#region Bojanje koda
		struct WordAndPosition
		{
			public string Word;
			public int Position;
			public int Length;
			public override string ToString()
			{
				string s = "Word = " + Word + ", Position = " + Position + ", Length = " + Length + "\n";
				return s;
			}
		};

		ArrayList TheBuffer = new ArrayList();

		private int ParseLine(string s)
		{

			TheBuffer.Clear();
			WordAndPosition buffer;
			int count = 0;
			Regex r = new Regex(@"\w+|[^A-Za-z0-9_ \f\t\v]", RegexOptions.IgnoreCase|RegexOptions.Compiled);
			
			Match m;
			for (m = r.Match(s); m.Success ; m = m.NextMatch()) 
			{
				buffer.Word = m.Value;
				buffer.Position = m.Index;
				buffer.Length = m.Length;
				TheBuffer.Add(buffer);
				count++;
			}


			return count;
		}

		private void Lookup(string s, out Color color, out FontStyle fontStyle)
		{
			color = _bojaSlova;
			fontStyle = FontStyle.Regular;

			if (TheBojac.IsIdentifikator(s))
			{
				color = _bojaIdentifikatora;
				fontStyle = FontStyle.Regular;
			}

			if (TheBojac.IsKeyword(s))
			{
				color = _bojaKljucnihRijeci;
				fontStyle = FontStyle.Bold;
			}
		}

		private void MakeColorSyntaxForCurrentLine()
		{
			int CurrentSelectionStart = textbox.SelectionStart;
			int CurrentSelectionLength = textbox.SelectionLength;

			// naði poèetak linije
			int pos = CurrentSelectionStart;
			while ( (pos > 0) && (textbox.Text[pos-1] != '\n'))
				pos--;

			// naði kraj linije
			int pos2 = CurrentSelectionStart;
			while ( (pos2 < textbox.Text.Length) && 
				(textbox.Text[pos2] != '\n') )
				pos2++;

			string s = textbox.Text.Substring(pos, pos2 - pos);
			string previousWord = "";
			int count = ParseLine(s);
			for  (int i = 0; i < count; i++)
			{
				WordAndPosition wp = (WordAndPosition)TheBuffer[i];

				Color c;
				FontStyle style;

				Lookup(wp.Word, out c, out style);
				textbox.Select(wp.Position + pos, wp.Length);
				if (c!=textbox.SelectionColor || style!=textbox.SelectionFont.Style)
				{
					textbox.SelectionColor = c;
					textbox.SelectionFont = new Font(textbox.Font.Name, textbox.Font.Size, style);
				}

				previousWord = wp.Word;
			}

			if (CurrentSelectionStart >=0)
				textbox.Select(CurrentSelectionStart, CurrentSelectionLength);
		}

		private void MakeColorSyntaxForAllText(string s, bool resetSelection)
		{
			populating = true;
			int CurrentSelectionStart = textbox.SelectionStart;
			int CurrentSelectionLength = textbox.SelectionLength;

			int count = ParseLine(s);
			string previousWord = "";
			for (int i = 0; i < count; i++)
			{
				WordAndPosition wp = (WordAndPosition)TheBuffer[i];
				Color c;
				FontStyle style;

				Lookup(wp.Word, out c, out style);
				textbox.Select(wp.Position, wp.Length);
				if (c!=textbox.SelectionColor || style!=textbox.SelectionFont.Style)
				{
					textbox.SelectionColor = c;
					textbox.SelectionFont = new Font(textbox.Font.Name, textbox.Font.Size, style);
				}

				previousWord = wp.Word;
			}

			if (CurrentSelectionStart >=0)
				textbox.Select(CurrentSelectionStart, CurrentSelectionLength);

			populating = false;
			if (resetSelection)
			{
				textbox.Select(0,0);
			}
			else
			{
				textbox.Select(CurrentSelectionStart, CurrentSelectionLength);
			}

		}

		#endregion
		private void textbox_BojaPromjenjena(object sender, System.EventArgs e)
		{
			
			_bojaPozadine=(Color)Boje[0];
			_bojaKljucnihRijeci=(Color)Boje[1];
			_bojaSlova=(Color)Boje[2];
			_bojaIdentifikatora=(Color)Boje[3];
			
			
			for(int i=0;i<_brojacDokumenata;i++)
			{
				textbox=(FlickerFreeRichEditTextBox)tab1.TabPages[i].Controls[0];
				textbox.BackColor=_bojaPozadine;	
				MakeColorSyntaxForAllText(textbox.Text,true);
			}

		}

		private void textbox_TextChanged(object sender, System.EventArgs e)
		{
			if (populating)
				return;
			
			UserInterface.FlickerFreeRichEditTextBox._Paint = false;
			if (textbox.Lines.Length!=textbox._LastNumberOfLines)
			{
				if (textbox.Lines.Length - textbox._LastNumberOfLines==1 &&
					textbox.SelectionLength==0)
				{
					// naði poèetak linije
					int selStart = textbox.SelectionStart;
					int pos = selStart;
					while ( (pos > 0) && (textbox.Text[pos-1] != '\n'))
						pos--;
					if (pos==selStart)
					{
						//find previous line
						if (pos!=0)
						{
							string leadingBlanks = "";
							pos--;
							while ( (pos > 0) && (textbox.Text[pos-1] != '\n'))
							{
								pos--;
							}
							while (pos<textbox.Text.Length && textbox.Text[pos]!='\n' &&
								textbox.Text[pos]!='\r' && char.IsWhiteSpace(textbox.Text[pos]))
							{
								leadingBlanks = leadingBlanks + textbox.Text[pos];
								pos++;
							}

							if (leadingBlanks.Length!=0)
							{
								populating = true;
								textbox.SelectedText = leadingBlanks;
								textbox.Select(selStart+leadingBlanks.Length, 0);
								populating = false;
							}
						}
					}
				}
				MakeColorSyntaxForAllText(textbox.Text, false);
				textbox._LastNumberOfLines = textbox.Lines.Length;
				textbox._LastLength = textbox.Text.Length;
			}
			else
			{
				MakeColorSyntaxForCurrentLine();
			}
			UserInterface.FlickerFreeRichEditTextBox._Paint = true; 
		}

		private void listBox1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			string pozicija,greska;
			int pocetak,kraj;
			textbox = (FlickerFreeRichEditTextBox)tab1.TabPages[tab1.SelectedIndex].Controls[0];
			textbox.Focus();	
			greska=listBox1.SelectedItem.ToString();
			pocetak=greska.IndexOf("(pozicija ")+10;
			kraj=greska.IndexOf(")");
			pozicija=greska.Substring(pocetak,kraj-pocetak);
			pocetak=greska.IndexOf(":  ")+3;
			kraj=greska.Length;
			greska=greska.Substring(pocetak,kraj-pocetak);
			if (textbox.Text.Length>0)
			{
				textbox.Select(Convert.ToInt32(pozicija), 1);
				textbox.ScrollToCaret();
			}
		}

		private void menuItem5_Click(object sender, System.EventArgs e)
		{
			Application.Exit();
		}

		private void menuItem2_Click(object sender, System.EventArgs e)
		{
			NoviDokument();	
		}

		private void menuItem3_Click(object sender, System.EventArgs e)
		{
			OtvoriDokument();
		}

		private void menuItem4_Click(object sender, System.EventArgs e)
		{
			SpremiDokument();
		}

		private void menuItem3_Click_1(object sender, System.EventArgs e)
		{
			Osvjezi();
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{	
			string s;		
			ColorConverter c = new ColorConverter();
			FileInfo fi = new FileInfo("Postavke.dat");

			if (fi.Exists) {
				StreamReader citaj = new StreamReader("Postavke.dat");

				s=citaj.ReadLine();
				_bojaPozadine=(Color)c.ConvertFromString(s);
				s=citaj.ReadLine();
				_bojaKljucnihRijeci=(Color)c.ConvertFromString(s);
				s=citaj.ReadLine();
				_bojaSlova=(Color)c.ConvertFromString(s);
				s=citaj.ReadLine();
				_bojaIdentifikatora = (Color)c.ConvertFromString(s);
				citaj.Close();

			} else {

				_bojaPozadine = (Color)c.ConvertFromString("White");
				_bojaKljucnihRijeci = (Color)c.ConvertFromString("Blue");
				_bojaSlova = (Color)c.ConvertFromString("Black");
				_bojaIdentifikatora = (Color)c.ConvertFromString("Green");
				
				StreamWriter strPostavke = new StreamWriter("Postavke.dat");
				strPostavke.Write("White\nBlue\nBlack\nGreen\n");
				strPostavke.Close();
			}
			NoviDokument();
		}

		private void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			switch(toolBar1.Buttons.IndexOf(e.Button))
			{
				case 1:
					NoviDokument();
					break; 
				case 3:
					OtvoriDokument();
					break; 
				case 4:
					SpremiDokument();   
					break; 
				case 6:
					Osvjezi();  
					break;
				case 7:
					Postavke();
					break;
				case 9:
					Kompajliraj();  
					break;
				case 10:
					Pokreni();
					break;
				case 12:
					Brisi();
					break;
			}
		
		}


		#region ToolBar akcije
		private void NoviDokument()
		{
			FlickerFreeRichEditTextBox novi = new FlickerFreeRichEditTextBox();	
			novi.ScrollBars=System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
			novi.Anchor=tab1.Anchor;
			novi.Dock=DockStyle.Fill;
			novi.AcceptsTab = true;
			novi.AutoWordSelection = true;
			novi.Font = new Font("Courier New", 10);
			novi.BackColor=_bojaPozadine;

			novi.TextChanged+= new EventHandler(textbox_TextChanged);
			novi.BackColorChanged+= new EventHandler(textbox_BojaPromjenjena);
			tab1.TabPages.Add(new TabPage("novi"+_brojacDokumenata.ToString()+".pas"));
			tab1.TabPages[_brojacDokumenata].Controls.Add(novi);
			tab1.SelectedTab=tab1.TabPages[_brojacDokumenata];
			tab1.SelectedTab.Focus();
			textbox = novi;	
			_brojacDokumenata++;
			Razvuci();
			this.MakeColorSyntaxForAllText(textbox.Text, true);
			
			if (_brojacDokumenata==1)
				textbox.Text = 
					@"program ime;
var
	a:integer;
begin
	for a:=1 to 10 do begin
		WriteLineInteger(a);
	end;
end.";
		}

		private void NoviDokument(string kod, string datoteka)
		{
			FlickerFreeRichEditTextBox novi = new FlickerFreeRichEditTextBox();	
			novi.ScrollBars=System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
			novi.Anchor=tab1.Anchor;
			novi.Dock=DockStyle.Fill;
			novi.AcceptsTab = true;
			novi.AutoWordSelection = true;
			novi.Font = new Font("Courier New", 10);
			novi.BackColor=_bojaPozadine;
			novi.Text=kod;
			novi.AcceptsTab = true;
			novi.AutoWordSelection = true;
			novi.Font = new Font("Courier New", 10);
			novi.BackColor=_bojaPozadine;
		//	novi.TextChanged+= new EventHandler(textbox_TextChanged);
		//	novi.BackColorChanged+= new EventHandler(textbox_BojaPromjenjena);
			
			tab1.TabPages.Add(new TabPage(datoteka));
			tab1.TabPages[_brojacDokumenata].Controls.Add(novi);
			tab1.SelectedTab=tab1.TabPages[_brojacDokumenata];
			tab1.SelectedTab.Focus();
			_brojacDokumenata++;
			novi.TextChanged+= new EventHandler(textbox_TextChanged);
			novi.BackColorChanged+= new EventHandler(textbox_BojaPromjenjena);
			textbox = novi;
			Razvuci();
		}

		private void Pokreni()
		{
			if (tab1.TabCount==0)
			{
				return;
			}
			listBox1.Items.Clear();
			listBox1.Visible=false;
			LanguageProcessorService processor = new LanguageProcessorService();
			textbox = (FlickerFreeRichEditTextBox)tab1.TabPages[tab1.SelectedIndex].Controls[0];
			
			processor.Process(textbox.Text, true);

			if (processor.ListaGresaka.Count==0)
			{
				StreamWriter sw = File.CreateText("_temp.p");
				sw.Write(processor.GeneriraniProgram);
				sw.Close();

				System.Diagnostics.Process.Start("PMachine.exe", "_temp.p");
			}
			else
			{
				listBox1.Visible=true;
				if (razv==1)
				{
					tab1.Height=listBox1.Top-35;
					textbox.Height=tab1.Height-5;
					razv=0;
				}
			}

			foreach(GreskaJezicnogProcesora greska in processor.ListaGresaka)
			{
				if (greska.LinijaIzvornogKoda==-1 && greska.PozicijaGreske==-1)
				{
					if (textbox.Lines.Length > 0 && 
						textbox.Lines[textbox.Lines.Length-1].Length > 0)
					{
						int linija = textbox.Lines.Length - 1;
						int pozicija = textbox.Lines[textbox.Lines.Length-1].Length - 1;
						listBox1.Items.Add("Linija " + linija.ToString() + 
							" (pozicija " + pozicija.ToString() + ") " + greska.Opis);
					}
					else
					{
						listBox1.Items.Add("Linija 0 (pozicija 0) " + greska.Opis);
					}
				}
				else
				{
					listBox1.Items.Add("Linija " + greska.LinijaIzvornogKoda.ToString() + 
						" (pozicija " + greska.PozicijaGreske+ ") " + greska.Opis);
				}
			}
		}

		private void SpremiDokument()
		{
			string kod="";
			textbox = (FlickerFreeRichEditTextBox)tab1.TabPages[tab1.SelectedIndex].Controls[0];
			kod=textbox.Text;
			
			Stream file;
					
			saveFileDialog1.Filter = "Pascal files (*.pas)|*.pas|P-Code files (*.p)|*.p|ARM files (*.arm)|*.arm"  ;
			saveFileDialog1.FilterIndex = 1 ;
			saveFileDialog1.RestoreDirectory = true ;
			saveFileDialog1.FileName="novi"+tab1.SelectedIndex.ToString();
		
			if(saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				if((file = saveFileDialog1.OpenFile()) != null)
				{
					StreamWriter pisi = new StreamWriter(file);
					pisi.WriteLine(kod);
					tab1.TabPages[tab1.SelectedIndex].Text=SkratiIme(saveFileDialog1.FileName);
					pisi.Close();
					file.Close();
				}
			}
			Osvjezi();
			textbox.Select(0,0);
		}

		private void OtvoriDokument()
		{
			string kod="";
		
			Stream file;
					
			openFileDialog1.Filter = "Pascal files (*.pas)|*.pas|P-Code files (*.p)|*.p|ARM files (*.arm)|*.arm"  ;
			openFileDialog1.FilterIndex = 1 ;
			openFileDialog1.RestoreDirectory = true ;
		
			if(openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				if((file = openFileDialog1.OpenFile()) != null)
				{
					StreamReader citaj = new StreamReader(file);
					kod=citaj.ReadToEnd();
					NoviDokument(kod,SkratiIme(openFileDialog1.FileName));
					citaj.Close();
					file.Close();
				}
				Osvjezi();
				textbox.Select(0,0);
			}			
		}

		private void Kompajliraj()
		{
			listBox1.Items.Clear();
			listBox1.Visible=false;
			LanguageProcessorService processor = new LanguageProcessorService();
			textbox = (FlickerFreeRichEditTextBox)tab1.TabPages[tab1.SelectedIndex].Controls[0];
			
			processor.Process(textbox.Text, true);

			if (processor.ListaGresaka.Count==0)
			{
				string ime=tab1.TabPages[tab1.SelectedIndex].Text;
				ime=ime.Substring(0,ime.LastIndexOf("."));
				NoviDokument(processor.GeneriraniProgram,ime+".p");

				if (!processor.GreskaPriGeneriranjuArmKoda)
				{
					NoviDokument(processor.GeneriraniArmKod,ime+".arm");
				}
				else
				{
					MessageBox.Show(String.Format("Doslo je do greske pri generiranju ARM koda. Opis greske: {0}",processor.OpisArmGreske), "Greska");
				}
			}
			else
			{
				listBox1.Visible=true;
				if (razv==1)
				{
					tab1.Height-=razmak;
					razv=0;
				}
			}

			foreach(GreskaJezicnogProcesora greska in processor.ListaGresaka)
			{
				if (greska.LinijaIzvornogKoda==-1 && greska.PozicijaGreske==-1)
				{
					if (textbox.Lines.Length > 0 && 
						textbox.Lines[textbox.Lines.Length-1].Length > 0)
					{
						int linija = textbox.Lines.Length;
						int pozicija = textbox.Lines[textbox.Lines.Length-1].Length - 1;
						listBox1.Items.Add("Linija " + linija.ToString() + 
							" (pozicija " + (textbox.Text.Length-1).ToString() + ") " + greska.Opis);
					}
					else
					{
						listBox1.Items.Add("Linija 0 (pozicija 0) " + greska.Opis);
					}
				}
				else
				{
					listBox1.Items.Add("Linija " + greska.LinijaIzvornogKoda.ToString() + 
						" (pozicija " + greska.PozicijaGreske+ ") " + greska.Opis);
				}
			}
		}

		private void Osvjezi()
		{			
			textbox = (FlickerFreeRichEditTextBox)tab1.TabPages[tab1.SelectedIndex].Controls[0];
			textbox.SelectAll();
			textbox.Refresh();
			UserInterface.FlickerFreeRichEditTextBox._Paint = false;			
			MakeColorSyntaxForAllText(textbox.Text, false);
			UserInterface.FlickerFreeRichEditTextBox._Paint = true;	
		}

		private string SkratiIme(string imeDatoteke)
		{
			if (imeDatoteke.IndexOf("\\")>0)
			{
				string ime="";
				ime=imeDatoteke.Substring(imeDatoteke.LastIndexOf("\\")+1);
				return ime;
			}
			else
				return imeDatoteke;
		}
		private void Brisi()
		{
			if (tab1.TabPages.Count>1)
			{
				tab1.TabPages[tab1.SelectedIndex].Dispose();
				_brojacDokumenata--;
			}
		}
		private void Postavke()
		{
			Boje.Clear();
			frmPostavke p = new frmPostavke(Boje,textbox);
			p.ShowDialog();
		}
		#endregion;

		private void tab1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			textbox = (FlickerFreeRichEditTextBox)tab1.TabPages[tab1.SelectedIndex].Controls[0];
			textbox.ForeColor=_bojaSlova;
			Razvuci();
		}
		private void Razvuci()
		{	 
			if (tab1.TabPages[tab1.SelectedIndex].Text.LastIndexOf(".p")==tab1.TabPages[tab1.SelectedIndex].Text.Length-2 || 
				tab1.TabPages[tab1.SelectedIndex].Text.LastIndexOf(".arm")==tab1.TabPages[tab1.SelectedIndex].Text.Length-4)
			{
				mainMenu1.MenuItems[2].Enabled=false;
				toolBar1.Buttons[9].Enabled=false;
				toolBar1.Buttons[10].Enabled=false;
			}
			else 
			{
				mainMenu1.MenuItems[2].Enabled=true;
				toolBar1.Buttons[9].Enabled=true;
				toolBar1.Buttons[10].Enabled=true;
			}

			toolBar1.Buttons[4].Enabled=true;
			toolBar1.Buttons[6].Enabled=true;
			toolBar1.Buttons[12].Enabled=true;

			if (razv==0)
			{
				listBox1.Visible=false;
				tab1.Height+=razmak;
				razv=1;
			}
			textbox.Focus();
		}

		private void menuItem5_Click_1(object sender, System.EventArgs e)
		{
			About about = new About();
			about.ShowDialog();
		}

		private void menuItem9_Click(object sender, System.EventArgs e)
		{
			Pokreni();
		}

		private void menuItem11_Click(object sender, System.EventArgs e)
		{
			Kompajliraj();
		}

	}
}
