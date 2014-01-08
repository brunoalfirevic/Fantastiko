using System;
using System.Collections;

namespace JezicniProcesor
{
	/// <summary>
	/// Ova klasa obavlja sintaksnu analizu
	/// </summary>
	class SintaksniAnalizator
	{
		public SintaksniAnalizator()
		{

		}

		/// <summary>
		/// Obavlja sintaksnu analizu
		/// </summary>
		public void Analiziraj(ICollection listaUniformnihZnakova, TablicaZnakova tablicaZnakova,
			IList listaGresaka, bool ulaznoIzlazneProcedure)
		{
			_tablicaZnakova = tablicaZnakova;
			_listaGresaka = listaGresaka;
			RekurzivniSpust rekurzivniSpust = new RekurzivniSpust(listaUniformnihZnakova, tablicaZnakova,
				_listaGresaka, out _sintaksnoStablo, ulaznoIzlazneProcedure);
			rekurzivniSpust.Pokreni();
			_generiraniProgram = rekurzivniSpust.GeneriraniProgram;

		}

		/// <summary>
		/// Vraca korijen dijela sintaksnog stabla, tj. onog dijela koji se generirao do trenutka pozivanja
		/// ovog svojstva
		/// </summary>
		public CvorSintaksnogStabla SintaksnoStablo
		{
			get { return _sintaksnoStablo; }
		}

		public string GeneriraniProgram
		{
			get { return _generiraniProgram; }
		}


		/// <summary>
		/// Ova metoda vraca listu gresaka koje su se javile tijekom analize izvornog programa
		/// </summary>
		/// <returns></returns>
		public IList VratiListuGresaka()
		{
			return _listaGresaka;
		}

		private TablicaZnakova _tablicaZnakova = null;
		private IList _listaGresaka = null;
		private CvorSintaksnogStabla _sintaksnoStablo = null;
		private string _generiraniProgram = "";
	}
}
