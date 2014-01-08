using System;
using System.Collections;
using System.Globalization;

namespace JezicniProcesor
{
	enum TipKonstanteEnum
	{
		Integer,
		Real,
		String,
		Boolean
	}

	/// <summary>
	/// Summary description for TablicaZnakova.
	/// </summary>
	class TablicaZnakova
	{
		public TablicaZnakova()
		{
			_vrijednostiKonstanti = new Hashtable();
			_tablicaKROS = new Hashtable();
			_tabliceIdentifikatora = new ArrayList();
			_tabliceIdentifikatora.Add(new Hashtable());
		}

		public bool DodajKonstantu(string leksickaJedinka, TipKonstanteEnum tipKonstante, UniformniZnak objUniformniZnak, ArrayList listaGresaka)
		{
			if (objUniformniZnak.UniformniZnakID!=UniformniZnakEnum.Konstanta)
			{
				throw new InvalidOperationException("Pokusana dodati vrijednost u tablicu znakova za leksicku jedinku koja nije konstanta.");
			}

			switch(tipKonstante)
			{
				case TipKonstanteEnum.Integer:
					try {
						_vrijednostiKonstanti[objUniformniZnak] = Convert.ToUInt32(leksickaJedinka);
					}
					catch (OverflowException) {
						GreskaJezicnogProcesora greska = new GreskaJezicnogProcesora(objUniformniZnak.LinijaUKojojSeNalaziJedinka, objUniformniZnak.PozicijaUProgramu, "Konstanta je izvan intervala tipa integer.");
						listaGresaka.Add(greska);
						return false;
					}
					break;
				case TipKonstanteEnum.Real:
					try {
						NumberFormatInfo format = new NumberFormatInfo();
						format.NumberDecimalSeparator = ".";
						_vrijednostiKonstanti[objUniformniZnak] = Convert.ToSingle(leksickaJedinka, format);
					}
					catch (OverflowException) {
						GreskaJezicnogProcesora greska = new GreskaJezicnogProcesora(objUniformniZnak.LinijaUKojojSeNalaziJedinka, objUniformniZnak.PozicijaUProgramu, "Konstanta je izvan intervala tipa real.");
						listaGresaka.Add(greska);
						return false;
					}
					break;
				case TipKonstanteEnum.Boolean:
					_vrijednostiKonstanti[objUniformniZnak] = Convert.ToBoolean(leksickaJedinka);
					break;
				case TipKonstanteEnum.String:
					_vrijednostiKonstanti[objUniformniZnak] = leksickaJedinka;
					break;
			}
			return true;
		}

		public void DohvatiKonstantu(UniformniZnak objUniformniZnak, out UInt32 vrijednost)
		{
			vrijednost = (UInt32)_vrijednostiKonstanti[objUniformniZnak];
		}

		public void DohvatiKonstantu(UniformniZnak objUniformniZnak, out float vrijednost)
		{
			vrijednost = (float)_vrijednostiKonstanti[objUniformniZnak];
		}

		public void DohvatiKonstantu(UniformniZnak objUniformniZnak, out string vrijednost)
		{
			vrijednost = (string)_vrijednostiKonstanti[objUniformniZnak];
		}

		public void DohvatiKonstantu(UniformniZnak objUniformniZnak, out bool vrijednost)
		{
			vrijednost = (bool)_vrijednostiKonstanti[objUniformniZnak];
		}

		public void DodajKROS(string leksickaJedinka)
		{
			_tablicaKROS[leksickaJedinka] = leksickaJedinka;
		}

		public bool LeksickaJedinkaJeKROS(string leksickaJedinka)
		{
			return _tablicaKROS[leksickaJedinka]!=null;
		}

		public void DodajOpisnikPotprograma(OpisnikPotprograma opisnikPotprograma)
		{
			if (_tabliceIdentifikatora.Count>0)
			{
				Hashtable tablica = (Hashtable)_tabliceIdentifikatora[_tabliceIdentifikatora.Count-1];
				tablica.Add(opisnikPotprograma.Identifikator, opisnikPotprograma);
			}
		}

		public void DodajTipPodatka(string identifikator, TipPodatka tipPodatka)
		{
			if (_tabliceIdentifikatora.Count>0)
			{
				Hashtable tablica = (Hashtable)_tabliceIdentifikatora[_tabliceIdentifikatora.Count-1];
				tablica.Add(identifikator, tipPodatka);
			}
		}

		public void DohvatiTipPodatka(string identifikator, out TipPodatka tipPodatka)
		{
			for(int i=_tabliceIdentifikatora.Count-1; i>=0; i--)
			{
				Hashtable tablica = (Hashtable)_tabliceIdentifikatora[i];
				object o = tablica[identifikator];
				if (o!=null)
				{
					if (o is TipPodatka)
					{
						tipPodatka = (TipPodatka)o;
						return;
					}
					else
					{
						break;
					}
				}
			}
			tipPodatka = null;
		}

		public void DodajLokalnuVarijablu(Polje varijabla)
		{
			if (_tabliceIdentifikatora.Count>0)
			{
				Hashtable tablica = (Hashtable)_tabliceIdentifikatora[_tabliceIdentifikatora.Count-1];
				tablica.Add(varijabla.Identifikator, varijabla);
			}
		}

		public void PronadjiPotprogram(string identifikator, out OpisnikPotprograma opisnikPotprograma,
			out int razinaGnijezdjenja)
		{
			for(int i=_tabliceIdentifikatora.Count-1; i>=0; i--)
			{
				Hashtable tablica = (Hashtable)_tabliceIdentifikatora[i];
				object o = tablica[identifikator];
				if (o!=null)
				{
					if (o is OpisnikPotprograma)
					{
						opisnikPotprograma = (OpisnikPotprograma)o;
						razinaGnijezdjenja = _tabliceIdentifikatora.Count - 1 - i;
						return;
					}
				}
			}
			opisnikPotprograma = null;
			razinaGnijezdjenja = 0;
		}

		public void PronadjiPotprogram(string identifikator, TipPotprogramaEnum tipPotprograma, 
			out OpisnikPotprograma opisnikPotprograma, out int razinaGnijezdjenja)
		{
			for(int i=_tabliceIdentifikatora.Count-1; i>=0; i--)
			{
				Hashtable tablica = (Hashtable)_tabliceIdentifikatora[i];
				object o = tablica[identifikator];
				if (o!=null)
				{
					if (o is OpisnikPotprograma)
					{
						OpisnikPotprograma tmp = (OpisnikPotprograma)o;
						if (tmp.TipPotprograma==tipPotprograma)
						{
							opisnikPotprograma = tmp;
							razinaGnijezdjenja = _tabliceIdentifikatora.Count - 1 - i;
							return;
						}
						else
						{
							break;
						}
					}
				}
			}
			opisnikPotprograma = null;
			razinaGnijezdjenja = 0;
		}

		public void PronadjiVarijablu(string identifikator,	out Polje varijabla, out int razinaGnijezdjenja)
		{
			for(int i=_tabliceIdentifikatora.Count-1; i>=0; i--)
			{
				Hashtable tablica = (Hashtable)_tabliceIdentifikatora[i];
				object o = tablica[identifikator];
				if (o!=null)
				{
					if (o is Polje)
					{
						varijabla = (Polje)o;
						razinaGnijezdjenja = _tabliceIdentifikatora.Count - 1 - i;
						return;
					}
					else
					{
						break;
					}
				}
			}
			varijabla = null;
			razinaGnijezdjenja = 0;
		}

		public bool PostojiIdentifikator(string identifikator, bool samoTrenutnaRazinaGnijezdjenja)
		{
			for(int i=_tabliceIdentifikatora.Count-1; i>=0; i--)
			{
				Hashtable tablica = (Hashtable)_tabliceIdentifikatora[i];
				if (tablica[identifikator]!=null)
				{
					return true;
				}

				if (samoTrenutnaRazinaGnijezdjenja)
				{
					break;
				}
			}
			return false;
		}

		public void DodajLabelu(Labela labela)
		{
			if (_tabliceIdentifikatora.Count>0)
			{
				Hashtable tablica = (Hashtable)_tabliceIdentifikatora[_tabliceIdentifikatora.Count-1];
				tablica.Add(labela.Identifikator, labela);
			}			
		}

		public Labela PronadjiLabelu(UInt32 labela)
		{
			if (_tabliceIdentifikatora.Count>0)
			{
				Hashtable tablica = (Hashtable)_tabliceIdentifikatora[_tabliceIdentifikatora.Count-1];
				object o = tablica[labela];
				if (o!=null && o is Labela)
				{
						return (Labela)o;
				}
			}
			return null;
		}

		public void DodajKonstantniIdentifikator(KonstantniIdentifikator konstantniIdentifikator)
		{
			if (_tabliceIdentifikatora.Count>0)
			{
				Hashtable tablica = (Hashtable)_tabliceIdentifikatora[_tabliceIdentifikatora.Count-1];
				tablica.Add(konstantniIdentifikator.Identifikator, konstantniIdentifikator);
			}
		}

		public void PronadjiKonstantniIdentifikator(string identifikator, out KonstantniIdentifikator konstantniIdentifikator)
		{
			for(int i=_tabliceIdentifikatora.Count-1; i>=0; i--)
			{
				Hashtable tablica = (Hashtable)_tabliceIdentifikatora[i];
				object o = tablica[identifikator];
				if (o!=null)
				{
					if (o is KonstantniIdentifikator)
					{
						konstantniIdentifikator = (KonstantniIdentifikator)o;
						return;
					}
					else
					{
						break;
					}
				}
			}
			konstantniIdentifikator = null;
		}

		public void OtvorenBlok()
		{
			_tabliceIdentifikatora.Add(new Hashtable());
		}

		public void ZatvorenBlok()
		{
			_tabliceIdentifikatora.RemoveAt(_tabliceIdentifikatora.Count-1);
		}

		private ArrayList _tabliceIdentifikatora;
		private Hashtable _tablicaKROS;
		private Hashtable _vrijednostiKonstanti;
	}
}
