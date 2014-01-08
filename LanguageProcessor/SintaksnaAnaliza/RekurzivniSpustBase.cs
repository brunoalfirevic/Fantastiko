using System;
using System.Collections;

namespace JezicniProcesor
{
	/// <summary>
	/// Summary description for RekurzivniSpustBase.
	/// </summary>
	class RekurzivniSpustBase
	{
		protected RekurzivniSpustBase(ICollection listaUniformnihZnakova, TablicaZnakova tablicaZnakova,
			IList listaGresaka, bool ulaznoIzlazneProcedure)
		{
			_listaUniformnihZnakova = listaUniformnihZnakova;
			_tablicaZnakova = tablicaZnakova;
			_enumeratorUniformnihZnakova = _listaUniformnihZnakova.GetEnumerator();
			_stogBrojaDjeceUCvorovima = new Stack();
			_enumeratorUniformnihZnakova.MoveNext();
			_listaGresaka=listaGresaka;
			_semantickiAnalizator = new SemantickiAnalizator(listaGresaka, tablicaZnakova,
				_enumeratorUniformnihZnakova,ulaznoIzlazneProcedure);
		}

		public string GeneriraniProgram
		{
			get 
			{
				return _semantickiAnalizator.GeneriraniProgram;
			}
		}


		protected UniformniZnak TrenutniZnak
		{
			get 
			{ 
				try
				{
					return (UniformniZnak)_enumeratorUniformnihZnakova.Current;
				}
				catch(InvalidOperationException)
				{
					throw new FatalnaGreskaException(-1,-1, "Neocekivani kraj ulaznog programa.");					
				}
			}
		}

		protected SemantickiAnalizator SemantickiAnalizator
		{
			get { return _semantickiAnalizator; }
		}
		
		protected void PojediZavrsniZnak()
		{
			_enumeratorUniformnihZnakova.MoveNext();
		}

		protected void PojediZavrsniZnak(string zavrsniZnak, out string vrijednost)
		{
			switch (zavrsniZnak)
			{
				case OznakeZnakovaGramatike.Identifikator:
				{
					vrijednost = TrenutniZnak.LeksickaJedinka;
					break;
				}
				default:
				{
					vrijednost = "";
					break;
				}
			}
			PojediZavrsniZnak(zavrsniZnak);
		}

		protected void PojediZavrsniZnak(string zavrsniZnak, out UInt32 vrijednost)
		{
			switch (zavrsniZnak)
			{
				case OznakeZnakovaGramatike.UnsignedInteger:
				{
					_tablicaZnakova.DohvatiKonstantu(TrenutniZnak, out vrijednost);
					break;
				}
				case OznakeZnakovaGramatike.UnsignedReal:
				{
					float tmp;
					_tablicaZnakova.DohvatiKonstantu(TrenutniZnak, out tmp);
					vrijednost = KonverterPodataka.FloatToBitRepresentation(tmp);
					break;
				}
				case OznakeZnakovaGramatike.Boolean:
				{
					bool tmp;
					_tablicaZnakova.DohvatiKonstantu(TrenutniZnak, out tmp);
					vrijednost = KonverterPodataka.BoolToBitRepresentation(tmp);
					break;
				}
				default:
				{
					vrijednost = 0;
					break;
				}
			}
			PojediZavrsniZnak(zavrsniZnak);
		}

		protected void PojediZavrsniZnak(string zavrsniZnak)
		{
			if (zavrsniZnak==TrenutniZnak.OznakaZnakaGramatike)
			{
				_aktivniCvor.Djeca.Add(new CvorSintaksnogStabla(TrenutniZnak, _aktivniCvor));

				int brDjece = (int)_stogBrojaDjeceUCvorovima.Pop();
				brDjece--;
				_stogBrojaDjeceUCvorovima.Push(brDjece);
					
				while (_stogBrojaDjeceUCvorovima.Count>0 
					&& (int)_stogBrojaDjeceUCvorovima.Peek()==0)
				{
					_aktivniCvor = _aktivniCvor.Roditelj;
					_stogBrojaDjeceUCvorovima.Pop();
				}

				_enumeratorUniformnihZnakova.MoveNext();
			}
		}

		protected void KonzumirajZavrsniZnak(string zavrsniZnak, out string vrijednost)
		{
			switch (zavrsniZnak)
			{
				case OznakeZnakovaGramatike.Identifikator:
				{
					vrijednost = TrenutniZnak.LeksickaJedinka;
					break;
				}
				default:
				{
					vrijednost = "";
					break;
				}
			}
			KonzumirajZavrsniZnak(zavrsniZnak);
		}

		protected void KonzumirajZavrsniZnak(string zavrsniZnak, out UInt32 vrijednost)
		{
			switch (zavrsniZnak)
			{
				case OznakeZnakovaGramatike.UnsignedInteger:
				{
					_tablicaZnakova.DohvatiKonstantu(TrenutniZnak, out vrijednost);
					break;
				}
				case OznakeZnakovaGramatike.UnsignedReal:
				{
					float tmp;
					_tablicaZnakova.DohvatiKonstantu(TrenutniZnak, out tmp);
					vrijednost = KonverterPodataka.FloatToBitRepresentation(tmp);
					break;
				}
				case OznakeZnakovaGramatike.Boolean:
				{
					bool tmp;
					_tablicaZnakova.DohvatiKonstantu(TrenutniZnak, out tmp);
					vrijednost = KonverterPodataka.BoolToBitRepresentation(tmp);
					break;
				}
				default:
				{
					vrijednost = 0;
					break;
				}
			}
			KonzumirajZavrsniZnak(zavrsniZnak);
		}

		protected void KonzumirajZavrsniZnak(string zavrsniZnak)
		{
			if (zavrsniZnak!=TrenutniZnak.OznakaZnakaGramatike)
			{
				PrijaviGresku("Neocekivani znak u ulaznom programu.");
			}
			else
			{
				_aktivniCvor.Djeca.Add(new CvorSintaksnogStabla(TrenutniZnak, _aktivniCvor));

				int brDjece = (int)_stogBrojaDjeceUCvorovima.Pop();
				brDjece--;
				_stogBrojaDjeceUCvorovima.Push(brDjece);
					
				while (_stogBrojaDjeceUCvorovima.Count>0 
					&& (int)_stogBrojaDjeceUCvorovima.Peek()==0)
				{
					_aktivniCvor = _aktivniCvor.Roditelj;
					_stogBrojaDjeceUCvorovima.Pop();
				}

				_enumeratorUniformnihZnakova.MoveNext();
			}
		}

		protected CvorSintaksnogStabla PostaviPocetniNezavrsniZnak(string pocetniNezavrsniZnak)
		{
			NezavrsniZnak znakGramatike = new NezavrsniZnak(pocetniNezavrsniZnak);
			_korijenSintaksnogStabla = new CvorSintaksnogStabla(znakGramatike,null);
			_aktivniCvor = _korijenSintaksnogStabla;
			return _korijenSintaksnogStabla;
		}

		protected void PrimijenioProdukciju(string lijevaStrana, string desnaStrana)
		{
			//ako ne primjenjujemo prvu produkciju dodaj nezavrsni znak na lijevoj strani
			//primijenjene produkcije u stablo
			//ako primjenjujemo prvu produkciju, taj nezavrsni znak je vec u stablu, pa ga
			//ne treba dodavati
			if (_stogBrojaDjeceUCvorovima.Count!=0)
			{
				NezavrsniZnak znakGramatike = new NezavrsniZnak(lijevaStrana.Trim());
				CvorSintaksnogStabla noviCvor = new CvorSintaksnogStabla(znakGramatike, _aktivniCvor);
				_aktivniCvor.Djeca.Add(noviCvor);
				_aktivniCvor = noviCvor;

				int brDjeceUStogu = (int)_stogBrojaDjeceUCvorovima.Pop();
				brDjeceUStogu--;
				_stogBrojaDjeceUCvorovima.Push(brDjeceUStogu);

			}

			string[] djecaArray = desnaStrana.Split(' ','\t');
			int brDjece = 0;
			foreach(string s in djecaArray)
			{
				if (s.Trim()!="")
				{
					brDjece++;
				}
			}
			_stogBrojaDjeceUCvorovima.Push(brDjece);

			while (_stogBrojaDjeceUCvorovima.Count>0 
				&& (int)_stogBrojaDjeceUCvorovima.Peek()==0)
			{
				_aktivniCvor = _aktivniCvor.Roditelj;
				_stogBrojaDjeceUCvorovima.Pop();
			}
		}

		/// <summary>
		/// Ukoliko se pozove, baca iznimku. Poziva se u slucaju da
		/// se dogodi greska nakon koje se ne moze nastaviti analiza
		/// </summary>
		protected void FatalnaGreska()
		{
			throw new FatalnaGreskaException(TrenutniZnak.LinijaUKojojSeNalaziJedinka, TrenutniZnak.PozicijaUProgramu, "");
		}

		/// <summary>
		/// Za razliku od metode "Greska" ova metoda ne baca iznimku, vec samo stavlja gresku
		/// sa zadanim opisom u listu gresaka, te tako omogucuje da se nastavi analiza izvornog
		/// programa. Takodjer pazi da ne doda vise poruka o pogresci za istu gresku u izvornom programu
		/// </summary>
		/// <param name="error"></param>
		protected void PrijaviGresku(string error)
		{
			if (TrenutniZnak.PozicijaUProgramu!=_brojGreske)
			{
				if (_prvi)
				{
					_prvi=false;
					_brojGreske=TrenutniZnak.PozicijaUProgramu;
				} 
				else 
				{
					_listaGresaka.Add(_tmpGreska);
					_brojGreske=TrenutniZnak.PozicijaUProgramu;

				}
			}
			_tmpGreska = new GreskaJezicnogProcesora(TrenutniZnak.LinijaUKojojSeNalaziJedinka,
				TrenutniZnak.PozicijaUProgramu, "'" + TrenutniZnak.OznakaZnakaGramatike + "'" + " " + error);
		}

		protected void PrijaviZaostaluGresku()
		{
			if (!_prvi)
			{
				_listaGresaka.Add(_tmpGreska);
				_brojGreske=TrenutniZnak.PozicijaUProgramu;
			}
		}

		protected void UpisiDirektnoUListuGresaka(int linija, int pozicija, string opis)
		{
			GreskaJezicnogProcesora greska = new GreskaJezicnogProcesora(linija, pozicija, opis);
			_listaGresaka.Add(greska);
		}

		protected void Preskoci(ArrayList slijedi)
		{
			while (!slijedi.Contains(TrenutniZnak.OznakaZnakaGramatike))
			{
				PojediZavrsniZnak();
			}
		}

		protected bool Provjeri(ArrayList zapocinje,ArrayList slijedi,string error)
		{
			if (!zapocinje.Contains(TrenutniZnak.OznakaZnakaGramatike))
			{
				PrijaviGresku(error);
				ArrayList zap=new ArrayList(slijedi);
				zap.AddRange(zapocinje);
				Preskoci(zap);
			}
			if (zapocinje.Contains(TrenutniZnak.OznakaZnakaGramatike))
				return true;
			else 
				return false;
		}
		
		protected bool Provjeri(string zapocinje,ArrayList slijedi,string error)
		{
			if (!(zapocinje==TrenutniZnak.OznakaZnakaGramatike))
			{
				PrijaviGresku(error);
				slijedi.Add(zapocinje);
				Preskoci(slijedi);
				slijedi.Remove(zapocinje);
			}
			if (zapocinje==TrenutniZnak.OznakaZnakaGramatike)
				return true;
			else
				return false;

		}

		protected bool Provjeri(ArrayList slijedi,string error)
		{
			if (!slijedi.Contains(TrenutniZnak.OznakaZnakaGramatike))
			{
				PrijaviGresku(error);
				Preskoci(slijedi);
				return true;
			} 
			else 
			{
				return false;
			}
		}

		protected bool Provjeri(ArrayList slijedi)
		{
			if (slijedi.Contains(TrenutniZnak.OznakaZnakaGramatike))
				return true;
			else 
				return false;
		}

		private int _brojGreske=-1;
		private bool _prvi=true;
		private GreskaJezicnogProcesora _tmpGreska;

		private TablicaZnakova _tablicaZnakova;
		private IEnumerator _enumeratorUniformnihZnakova;
		private ICollection _listaUniformnihZnakova;
		private CvorSintaksnogStabla _korijenSintaksnogStabla;
		private CvorSintaksnogStabla _aktivniCvor;
		private IList _listaGresaka;
		private Stack _stogBrojaDjeceUCvorovima;

		private SemantickiAnalizator _semantickiAnalizator;
	}
}
