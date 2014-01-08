using System;
using System.Collections;

namespace JezicniProcesor
{
	/// <summary>
	/// Summary description for UvjetnaNaredba.
	/// </summary>
	public class UvjetnaNaredba
	{
		public UvjetnaNaredba(bool uvjetKonstantan, bool uvjetIspunjen)
		{
			_uvjetKonstantan = uvjetKonstantan;
			_uvjetIspunjen = uvjetIspunjen;
		}

		public bool UvjetKonstantan
		{
			get { return _uvjetKonstantan; }
		}

		public bool UvjetIspunjen
		{
			get { return _uvjetIspunjen; }
		}

		private bool _uvjetKonstantan;
		private bool _uvjetIspunjen;
	}
}
