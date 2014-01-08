using System;

namespace JezicniProcesor
{
	/// <summary>
	/// Prestavlja gresku koja se javila tijekom leksicke analize
	/// </summary>
	public class GreskaJezicnogProcesora
	{
		public GreskaJezicnogProcesora(int linijaIzvornogKoda, int pozicijaGreske, string opis)
		{
			_linijaIzvornogKoda = linijaIzvornogKoda;
			_pozicijaGreske = pozicijaGreske;
			_opis = opis;
		}

		public int LinijaIzvornogKoda
		{
			get { return _linijaIzvornogKoda; }
		}

		public string Opis
		{
			get { return _opis; }
		}
		public int PozicijaGreske
		{
			get { return _pozicijaGreske; }
		}

		private int _linijaIzvornogKoda;
		private int _pozicijaGreske;
		private string _opis;
	}
}
