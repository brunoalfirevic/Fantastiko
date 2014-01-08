using System;

namespace JezicniProcesor
{
	/// <summary>
	/// Summary description for GeneratorJedinstvenihBrojeva.
	/// </summary>
	public class GeneratorJedinstvenihBrojeva
	{
		public static int Generiraj()
		{
			_zadnjiBroj++;
			return _zadnjiBroj;
		}

		private static int _zadnjiBroj = 56;
	}
}
