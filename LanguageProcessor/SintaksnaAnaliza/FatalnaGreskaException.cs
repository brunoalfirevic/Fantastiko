using System;

namespace JezicniProcesor
{
	/// <summary>
	/// Summary description for FatalnaGreskaException.
	/// </summary>
	class FatalnaGreskaException : SintaksnaGreskaException
	{
		public FatalnaGreskaException(int linijaIzvornogKoda, int pozicijaGreske, string opis):
			base(linijaIzvornogKoda, pozicijaGreske, opis)
		{

		}
	}
}
