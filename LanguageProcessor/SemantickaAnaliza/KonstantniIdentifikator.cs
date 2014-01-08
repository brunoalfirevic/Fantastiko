using System;
using System.Collections;

namespace JezicniProcesor
{
	/// <summary>
	/// Summary description for Konstanta.
	/// </summary>
	class KonstantniIdentifikator
	{
		//a provjera da li su tip i vrijednost kompatibilni?
		public KonstantniIdentifikator(string identifikator, TipPodatka tip, UInt32 vrijednost)
		{
			_identifikator = identifikator;
			_tip = tip;
			_vrijednost = vrijednost;
		}

		public string Identifikator
		{
			get { return _identifikator; }
		}

		public TipPodatka Tip
		{
			get { return _tip; }
		}

		public UInt32 Vrijednost
		{
			get { return _vrijednost; }
		}

		private UInt32 _vrijednost;
		private string _identifikator;
		private TipPodatka _tip;
	}
}
