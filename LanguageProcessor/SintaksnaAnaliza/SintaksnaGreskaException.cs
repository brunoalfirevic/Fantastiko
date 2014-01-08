using System;
using System.Collections;

namespace JezicniProcesor
{
	/// <summary>
	/// Summary description for SintaksnaGreskaException.
	/// </summary>
	class SintaksnaGreskaException: ApplicationException
	{
		public SintaksnaGreskaException(int linijaIzvornogKoda, int pozicijaGreske, string opis)
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
