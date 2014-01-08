using System;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace JezicniProcesor
{
	/// <summary>
	/// Izvozi funkcijonalnost jezicnog procesora u druge dijelove aplikacije
	/// </summary>
	public class LanguageProcessorService
	{
		public LanguageProcessorService()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public void Process(string sourceProgram, bool builtInProcedures)
		{
			LeksickiAnalizator Lex = new LeksickiAnalizator();
			Lex.Analiziraj(sourceProgram);
			_listaUniformnihZnakova = Lex.VratiListuUniformnihZnakova();
			
			SintaksniAnalizator sintaksniAnalizator = new SintaksniAnalizator();
			sintaksniAnalizator.Analiziraj(_listaUniformnihZnakova,
				Lex.VratiTablicuZnakova(), Lex.VratiListuGresaka(),builtInProcedures);
			
			_sintaksnoStablo = sintaksniAnalizator.SintaksnoStablo;
			_listaGresaka = sintaksniAnalizator.VratiListuGresaka();
			_generiraniProgram = sintaksniAnalizator.GeneriraniProgram;

			if (_listaGresaka.Count==0)
			{
				GeneratorARMKoda armGenerator = new GeneratorARMKoda();
				armGenerator.GenerirajARMKod(_generiraniProgram, 8000);

				_generiraniArmKod = armGenerator.VratiARMKod;
				_greskaPriGeneriranjuArmKoda = armGenerator.DosloDoGreske;
				_opisArmGreske = armGenerator.OpisGreske;
			}
			else
			{
				_generiraniArmKod = "";
				_greskaPriGeneriranjuArmKoda = false;
				_opisArmGreske = "";
			}
			
			//sortiraj greske po mjestu pojavljivanja u izvornom programu
			for(int i=0; i<_listaGresaka.Count; i++)
			{
				for(int j=i+1; j<_listaGresaka.Count; j++)
				{
					GreskaJezicnogProcesora greska1 = (GreskaJezicnogProcesora)_listaGresaka[i];
					GreskaJezicnogProcesora greska2 = (GreskaJezicnogProcesora)_listaGresaka[j];

					if (greska1.LinijaIzvornogKoda > greska2.LinijaIzvornogKoda ||
						(greska1.LinijaIzvornogKoda == greska2.LinijaIzvornogKoda &&
						(greska1.PozicijaGreske > greska2.PozicijaGreske)))
					{
						_listaGresaka[i] = greska2;
						_listaGresaka[j] = greska1;
					}
				}
			}
		}

		public string GeneriraniProgram
		{
			get { return _generiraniProgram; }
		}

		public string GeneriraniArmKod
		{
			get { return _generiraniArmKod; }
		}

		public bool GreskaPriGeneriranjuArmKoda
		{
			get
			{
				return _greskaPriGeneriranjuArmKoda;
			}
		}

		public string OpisArmGreske
		{
			get 
			{
				return _opisArmGreske;
			}
		}

		
		public void IscrtajSintaksnoStablo(TreeView treeView)
		{
			treeView.Nodes.Clear();
			if (_sintaksnoStablo!=null)
			{
				TreeNode iscrtaniCvor = treeView.Nodes.Add(_sintaksnoStablo.ZnakGramatike.OznakaZnakaGramatike);
				IscrtajSintaksnuCjelinu(_sintaksnoStablo, iscrtaniCvor);
			}
		}

		public ICollection ListaGresaka
		{
			get { return _listaGresaka; }
		}

		public ICollection ListaUniformnihZnakova
		{
			get { return _listaUniformnihZnakova; }
		}

		private void IscrtajSintaksnuCjelinu(CvorSintaksnogStabla sintaksnaCjelina, TreeNode cvorZaIscrtat)
		{
			foreach(CvorSintaksnogStabla cvor in sintaksnaCjelina.Djeca)
			{
				TreeNode iscrtaniCvor;
				if (cvor.ZnakGramatike.TipZnakaGramatike==TipZnakaGramatikeEnum.Zavrsni)
				{
					UniformniZnak unifZnak = cvor.ZnakGramatike as UniformniZnak;
					iscrtaniCvor = cvorZaIscrtat.Nodes.Add(unifZnak.LeksickaJedinka);
					iscrtaniCvor.ForeColor = System.Drawing.Color.Red;
				}
				else
				{
					iscrtaniCvor = cvorZaIscrtat.Nodes.Add(cvor.ZnakGramatike.OznakaZnakaGramatike);
				}
				IscrtajSintaksnuCjelinu(cvor, iscrtaniCvor);
			}
		}

		private bool _greskaPriGeneriranjuArmKoda;
		private string _opisArmGreske;
		private string _generiraniArmKod;
		private string _generiraniProgram;
		private IList _listaGresaka;
		private ICollection _listaUniformnihZnakova;
		private CvorSintaksnogStabla _sintaksnoStablo;
	}
}

