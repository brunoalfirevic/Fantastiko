using System;
using System.Collections;

namespace JezicniProcesor
{
	/// <summary>
	/// Ova klasa predstavlja labelu
	/// </summary>
	class Labela
	{
		public Labela(UInt32 identifikator)
		{
			_jedinstveniIdentifikator = GeneratorJedinstvenihBrojeva.Generiraj();
			_identifikator = identifikator;
			_iskoristena = false;
		}

		public UInt32 Identifikator
		{
			get { return _identifikator; }
		}

		public int JedinstveniIdentifikator
		{
			get { return _jedinstveniIdentifikator; }
		}

		public bool Iskoristena
		{
			get { return _iskoristena; }
		}

		public void OznaciIskoristenost()
		{
			_iskoristena = true;
		}

		private UInt32 _identifikator;
		private int _jedinstveniIdentifikator;
		private bool _iskoristena;
	}
}
