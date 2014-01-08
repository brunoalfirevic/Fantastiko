using System;
using System.Collections;

namespace JezicniProcesor
{
	/// <summary>
	/// Ova klasa obavlja leksicku analizu
	/// </summary>
	class LeksickiAnalizator
	{
		public LeksickiAnalizator()
		{
		}

		/// <summary>
		/// Obavlja leksicku analizu
		/// </summary>
		public void Analiziraj(string sourceProgram)
		{
			DKASimulator dkaSimulator = new DKASimulator();
			dkaSimulator.Simuliraj(sourceProgram, _listaGresaka);
			_listaUniformnihZnakova = dkaSimulator.TablicaUniformnihZnakova;
			_tablicaZnakova = dkaSimulator.TablicaZnakova;
			UniformniZnak uniformniZnak=new UniformniZnak(OznakeZnakovaGramatike.KrajUlaznogNiza, OznakeZnakovaGramatike.KrajUlaznogNiza,
				UniformniZnakEnum.TerminatorUlaznogNiza, 0, -2);
			_listaUniformnihZnakova.Add(uniformniZnak);

		}

		/// <summary>
		/// Ova metoda se koristi za dohvat liste uniformnif znakova iz analizianog programa
		/// </summary>
		/// <returns>Vraca listu UniformnifZnakova</returns>
		public ICollection VratiListuUniformnihZnakova()
		{
			return _listaUniformnihZnakova ;
		}


		/// <summary>
		/// Ova metoda vraca listu gresaka koje su se javile tijekom leksicke analize
		/// </summary>
		/// <returns></returns>
		public IList VratiListuGresaka()
		{
			return _listaGresaka;
		}

		/// <summary>
		/// Ova metoda se koristi za dohvat tablice leksickih jedinki iz analiziranog programa
		/// </summary>
		/// <returns>Vraca TabicuLeksickihJedinki</returns>
		public TablicaZnakova VratiTablicuZnakova()
		{
			return _tablicaZnakova;
		}

		private TablicaZnakova _tablicaZnakova = new TablicaZnakova();
		private ArrayList _listaUniformnihZnakova = new ArrayList();
		private ArrayList _listaGresaka = new ArrayList();
		
	}
}
