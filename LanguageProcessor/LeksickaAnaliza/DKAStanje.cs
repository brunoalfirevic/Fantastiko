using System;
using System.Collections;

namespace JezicniProcesor
{
	/// <summary>
	/// Predstavlja stanje deterministickog konacnog automata koji se koristi pri leksickoj analizi
	/// </summary>
	class DKAStanje
	{
		public DKAStanje(bool nullStanje, bool prihvatljivoStanje, string kodStanja)
		{
			_nullStanje = nullStanje;
			_prihvatljivoStanje = prihvatljivoStanje;
			_kodStanja = kodStanja;
		}

		public bool NullStanje
		{
			get { return _nullStanje; }
		}

		public bool Prihvatljivo
		{
			get { return _prihvatljivoStanje; }
		}

		public string KodStanja
		{
			get { return _kodStanja; }
		}

		private bool _nullStanje;
		private bool _prihvatljivoStanje;
		private string _kodStanja;
	}
}
