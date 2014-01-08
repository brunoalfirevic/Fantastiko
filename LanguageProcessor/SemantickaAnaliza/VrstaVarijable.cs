using System;

namespace JezicniProcesor
{
	enum VrstaVarijableEnum
	{
		LokalnaVarijabla,
		KonstantniIdentifikator,
		AdresaNaStogu
	}

	class VrstaVarijable
	{
		public VrstaVarijable(VrstaVarijableEnum vrstaVarijable, int dubinaGnijezdjenja)
		{
			_vrstaVarijable = vrstaVarijable;
			_dubinaGnijezdjenja = dubinaGnijezdjenja;
			_poznataVrijednost = 0;
		}

		public VrstaVarijable(VrstaVarijableEnum vrstaVarijable)
		{
			_vrstaVarijable = vrstaVarijable;
			_dubinaGnijezdjenja = -1;
			_poznataVrijednost = 0;
		}

		public VrstaVarijable(VrstaVarijableEnum vrstaVarijable, UInt32 poznataVrijednost)
		{
			_vrstaVarijable = vrstaVarijable;
			_dubinaGnijezdjenja = -1;
			_poznataVrijednost = poznataVrijednost;
		}

		public VrstaVarijableEnum VratiVrstuVarijable
		{
			get { return _vrstaVarijable; }
		}

		public UInt32 PoznataVrijednost
		{
			get { return _poznataVrijednost; }
		}

		public int DubinaGnijezdjenja
		{
			get { return _dubinaGnijezdjenja; }
		}

		private UInt32 _poznataVrijednost;
		private VrstaVarijableEnum _vrstaVarijable;
		private int _dubinaGnijezdjenja;
	}
}
