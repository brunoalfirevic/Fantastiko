using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

namespace UserInterface
{
	/// <summary>
	/// Summary description for Form2.
	/// </summary>
	public class frmPostavke : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private bool _mijenjano=false;
		private Color _bojaPozadine;
		private Color _bojaSlova;
		private Color _bojaKljucnihRijeci;
		private Color _bojaIdentifikatora;
		ColorConverter c = new ColorConverter();
		private ArrayList NoveBoje = new ArrayList();
		FlickerFreeRichEditTextBox text;

		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.RadioButton rbPozadina;
		private System.Windows.Forms.RadioButton rbSlova;
		private System.Windows.Forms.RadioButton rbKljucne;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RichTextBox richTextBox1;
		private System.Windows.Forms.RadioButton radioButton1;
		private System.Windows.Forms.ColorDialog colorDialog1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Button button5;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button button6;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.ComponentModel.Container components = null;

		public frmPostavke(ArrayList StareBoje,FlickerFreeRichEditTextBox t)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			NoveBoje=StareBoje;
			text=t;
			
			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
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
			this.button1 = new System.Windows.Forms.Button();
			this.rbPozadina = new System.Windows.Forms.RadioButton();
			this.rbSlova = new System.Windows.Forms.RadioButton();
			this.rbKljucne = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.radioButton1 = new System.Windows.Forms.RadioButton();
			this.colorDialog1 = new System.Windows.Forms.ColorDialog();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.button4 = new System.Windows.Forms.Button();
			this.button5 = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.button6 = new System.Windows.Forms.Button();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(80, 160);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(80, 24);
			this.button1.TabIndex = 1;
			this.button1.Text = "Postavi";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// rbPozadina
			// 
			this.rbPozadina.Checked = true;
			this.rbPozadina.Location = new System.Drawing.Point(160, 32);
			this.rbPozadina.Name = "rbPozadina";
			this.rbPozadina.Size = new System.Drawing.Size(72, 16);
			this.rbPozadina.TabIndex = 3;
			this.rbPozadina.TabStop = true;
			this.rbPozadina.Text = "Pozadina";
			this.rbPozadina.CheckedChanged += new System.EventHandler(this.rbPozadina_CheckedChanged);
			// 
			// rbSlova
			// 
			this.rbSlova.Location = new System.Drawing.Point(160, 64);
			this.rbSlova.Name = "rbSlova";
			this.rbSlova.Size = new System.Drawing.Size(56, 16);
			this.rbSlova.TabIndex = 4;
			this.rbSlova.Text = "Slova";
			this.rbSlova.CheckedChanged += new System.EventHandler(this.rbSlova_CheckedChanged);
			// 
			// rbKljucne
			// 
			this.rbKljucne.Location = new System.Drawing.Point(160, 96);
			this.rbKljucne.Name = "rbKljucne";
			this.rbKljucne.Size = new System.Drawing.Size(88, 16);
			this.rbKljucne.TabIndex = 5;
			this.rbKljucne.Text = "Kljucne rijeci";
			this.rbKljucne.CheckedChanged += new System.EventHandler(this.rbKljucne_CheckedChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 16);
			this.label1.TabIndex = 6;
			this.label1.Text = "Primjer:";
			// 
			// richTextBox1
			// 
			this.richTextBox1.Location = new System.Drawing.Point(16, 32);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this.richTextBox1.ShowSelectionMargin = true;
			this.richTextBox1.Size = new System.Drawing.Size(128, 24);
			this.richTextBox1.TabIndex = 7;
			this.richTextBox1.Text = "program ime;  Writeln;";
			this.richTextBox1.WordWrap = false;
			// 
			// radioButton1
			// 
			this.radioButton1.Location = new System.Drawing.Point(160, 128);
			this.radioButton1.Name = "radioButton1";
			this.radioButton1.Size = new System.Drawing.Size(88, 16);
			this.radioButton1.TabIndex = 9;
			this.radioButton1.Text = "Identifikatori";
			this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
			// 
			// colorDialog1
			// 
			this.colorDialog1.AllowFullOpen = false;
			this.colorDialog1.ShowHelp = true;
			// 
			// button2
			// 
			this.button2.Enabled = false;
			this.button2.Location = new System.Drawing.Point(248, 24);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(72, 24);
			this.button2.TabIndex = 10;
			this.button2.Text = "Promijeni...";
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button3
			// 
			this.button3.Enabled = false;
			this.button3.Location = new System.Drawing.Point(248, 56);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(72, 24);
			this.button3.TabIndex = 11;
			this.button3.Text = "Promijeni...";
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// button4
			// 
			this.button4.Enabled = false;
			this.button4.Location = new System.Drawing.Point(248, 88);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(72, 24);
			this.button4.TabIndex = 12;
			this.button4.Text = "Promijeni...";
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// button5
			// 
			this.button5.Enabled = false;
			this.button5.Location = new System.Drawing.Point(248, 120);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(72, 24);
			this.button5.TabIndex = 13;
			this.button5.Text = "Promijeni...";
			this.button5.Click += new System.EventHandler(this.button5_Click);
			// 
			// panel1
			// 
			this.panel1.Location = new System.Drawing.Point(16, 64);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(128, 16);
			this.panel1.TabIndex = 14;
			// 
			// button6
			// 
			this.button6.Location = new System.Drawing.Point(176, 160);
			this.button6.Name = "button6";
			this.button6.Size = new System.Drawing.Size(80, 24);
			this.button6.TabIndex = 15;
			this.button6.Text = "Odustani";
			this.button6.Click += new System.EventHandler(this.button6_Click);
			// 
			// comboBox1
			// 
			this.comboBox1.Location = new System.Drawing.Point(16, 96);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(128, 21);
			this.comboBox1.TabIndex = 16;
			this.comboBox1.Text = "(preset)";
			this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
			// 
			// frmPostavke
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.ControlLight;
			this.ClientSize = new System.Drawing.Size(330, 194);
			this.Controls.Add(this.comboBox1);
			this.Controls.Add(this.button6);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.button5);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.radioButton1);
			this.Controls.Add(this.richTextBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.rbKljucne);
			this.Controls.Add(this.rbSlova);
			this.Controls.Add(this.rbPozadina);
			this.Controls.Add(this.button1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "frmPostavke";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Postavke";
			this.Load += new System.EventHandler(this.frmPostavke_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void frmPostavke_Load(object sender, System.EventArgs e)
		{
			string starePostavke;

			StreamReader citaj = new StreamReader("Postavke.dat");	

			starePostavke=citaj.ReadLine();
			_bojaPozadine=(Color)c.ConvertFromString(starePostavke);
			richTextBox1.BackColor=_bojaPozadine;

			
				starePostavke=citaj.ReadLine();
				_bojaKljucnihRijeci=(Color)c.ConvertFromString(starePostavke);
				Oboji(_bojaKljucnihRijeci,1);
			
				starePostavke=citaj.ReadLine();
				_bojaSlova=(Color)c.ConvertFromString(starePostavke);
				Oboji(_bojaSlova,2);
			
				starePostavke=citaj.ReadLine();
				_bojaIdentifikatora=(Color)c.ConvertFromString(starePostavke);
				Oboji(_bojaIdentifikatora,3);

				citaj.Close();


			panel1.BackColor=_bojaPozadine;
			comboBox1.Items.Add("Pascal");
			comboBox1.Items.Add("C");
		

			citaj.Close();
						
			button2.Enabled=true;
			button3.Enabled=false;
			button4.Enabled=false;
			button5.Enabled=false;
			panel1.BackColor=_bojaPozadine;

		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			_mijenjano=true;
			if (_mijenjano)
			{
				StreamWriter pisi = new StreamWriter("Postavke.dat");
				
				pisi.WriteLine(c.ConvertToString(_bojaPozadine)); 
				pisi.WriteLine(c.ConvertToString(_bojaKljucnihRijeci));	
				pisi.WriteLine(c.ConvertToString(_bojaSlova));	
				pisi.WriteLine(c.ConvertToString(_bojaIdentifikatora));
			
				pisi.Close();

				NoveBoje.Clear();
				NoveBoje.Add(_bojaPozadine);
				NoveBoje.Add(_bojaKljucnihRijeci);
				NoveBoje.Add(_bojaSlova);
				NoveBoje.Add(_bojaIdentifikatora);


				text.BackColor=_bojaPozadine;
			}
			frmPostavke.ActiveForm.Dispose();
		}

		private void Oboji(Color boja, int i)
		{
			if (i==1)
			{
				richTextBox1.Select(0,7);
				richTextBox1.SelectionColor=boja;
			}
			if (i==2)
			{
				richTextBox1.Select(8,4);
				richTextBox1.SelectionColor=boja;
				
			}
			if (i==3)
			{
				richTextBox1.Select(11,16);
				richTextBox1.SelectionColor=boja;

			}
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
			colorDialog1.Color = _bojaPozadine;
			if (colorDialog1.ShowDialog() == DialogResult.OK)
			{	
				_mijenjano=true;
				_bojaPozadine=colorDialog1.Color;
				richTextBox1.BackColor =  colorDialog1.Color;
				if (rbPozadina.Checked) panel1.BackColor=_bojaPozadine;
			}		
		}

		private void button3_Click(object sender, System.EventArgs e)
		{
			colorDialog1.Color = _bojaSlova;
			if (colorDialog1.ShowDialog() == DialogResult.OK)
			{	
				_mijenjano=true;
				_bojaSlova=colorDialog1.Color;
				Oboji(_bojaSlova,2);
				if (rbSlova.Checked) panel1.BackColor=_bojaSlova;
			}		
		}

		private void button4_Click(object sender, System.EventArgs e)
		{
			colorDialog1.Color = _bojaKljucnihRijeci;
			if (colorDialog1.ShowDialog() == DialogResult.OK)
			{	
				_mijenjano=true;
				_bojaKljucnihRijeci=colorDialog1.Color;
				Oboji(_bojaKljucnihRijeci,1);
				if (rbKljucne.Checked) panel1.BackColor=_bojaKljucnihRijeci;
			}	
		}

		private void button5_Click(object sender, System.EventArgs e)
		{
			colorDialog1.Color = _bojaIdentifikatora;
			if (colorDialog1.ShowDialog() == DialogResult.OK)
			{	
				_mijenjano=true;
				_bojaIdentifikatora=colorDialog1.Color;
				Oboji(_bojaIdentifikatora,3);
				if (radioButton1.Checked) panel1.BackColor=_bojaIdentifikatora;
			}
		}

		private void rbPozadina_CheckedChanged(object sender, System.EventArgs e)
		{
			if (rbPozadina.Checked)
			{
				button2.Enabled=true;
				button3.Enabled=false;
				button4.Enabled=false;
				button5.Enabled=false;
				panel1.BackColor=_bojaPozadine;
			}
		}

		private void rbSlova_CheckedChanged(object sender, System.EventArgs e)
		{
			if (rbSlova.Checked)
			{
				button2.Enabled=false;
				button3.Enabled=true;
				button4.Enabled=false;
				button5.Enabled=false;
				panel1.BackColor=_bojaSlova;
			}
		}

		private void rbKljucne_CheckedChanged(object sender, System.EventArgs e)
		{
			if (rbKljucne.Checked)
			{
				button2.Enabled=false;
				button3.Enabled=false;
				button4.Enabled=true;
				button5.Enabled=false;
				panel1.BackColor=_bojaKljucnihRijeci;
			}
		}

		private void radioButton1_CheckedChanged(object sender, System.EventArgs e)
		{
			if (radioButton1.Checked)
			{
				button2.Enabled=false;
				button3.Enabled=false;
				button4.Enabled=false;
				button5.Enabled=true;
				panel1.BackColor=_bojaIdentifikatora;
			}
		}

		private void button6_Click(object sender, System.EventArgs e)
		{
			frmPostavke.ActiveForm.Dispose();
		}

		private void comboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (comboBox1.SelectedIndex==0)
			{
				_bojaPozadine=Color.DarkBlue;
				_bojaSlova=Color.White;
				_bojaKljucnihRijeci=Color.Yellow;
				_bojaIdentifikatora=Color.DeepPink;
				_mijenjano=true;

			}
			if (comboBox1.SelectedIndex==1)
			{
				_bojaPozadine=Color.White;
				_bojaSlova=Color.Black;
				_bojaKljucnihRijeci=Color.Blue;
				_bojaIdentifikatora=Color.Green;
				_mijenjano=true;

			}
			panel1.BackColor=_bojaPozadine;
			richTextBox1.BackColor=_bojaPozadine;
			Oboji(_bojaSlova,2);
			Oboji(_bojaKljucnihRijeci,1);
			Oboji(_bojaIdentifikatora,3);


		}

	}
}
