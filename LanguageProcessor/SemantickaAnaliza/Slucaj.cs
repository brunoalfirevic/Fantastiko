using System;
using System.Collections;

namespace JezicniProcesor
{
	/// <summary>
	/// Summary description for Slucaj.
	/// </summary>
	class Slucaj
	{
		public Slucaj(ICollection listaOdabira, int oznakaCasePodNaredbe)
		{
			if (listaOdabira==null)
			{
				listaOdabira = new ArrayList();
			}

			_listaOdabira = listaOdabira;
			_oznakaCasePodNaredbe = oznakaCasePodNaredbe;
		}

		public ICollection ListaOdabira
		{
			get { return _listaOdabira; }
		}

		public int OznakaCasePodNaredbe
		{
			get { return _oznakaCasePodNaredbe; }
		}

		private ICollection  _listaOdabira;
		private int _oznakaCasePodNaredbe;
	}
}
