using System;
using System.Collections;

namespace JezicniProcesor
{
	/// <summary>
	/// Summary description for ForNaredba.
	/// </summary>
	class ForNaredba
	{
		public ForNaredba(UInt32 adresaForVarijable, int razinaGnijezdjenjaForVarijable,
			TipPodatka tipForVarijable, UInt32 adresaGranice)
		{
			_adresaGranice = adresaGranice;
			_razinaGnijezdjenjaForVarijable = razinaGnijezdjenjaForVarijable;
			_adresaForVarijable = adresaForVarijable;
			_tipForVarijable = tipForVarijable;
		}

		public UInt32 AdresaForVarijable
		{
			get { return _adresaForVarijable; }
		}

		public int RazinaGnijezdjenjaForVarijable
		{
			get { return _razinaGnijezdjenjaForVarijable; }
		}

		public UInt32 AdresaGranice
		{
			get { return _adresaGranice; }
		}

		public TipPodatka TipForVarijable
		{
			get { return _tipForVarijable; }
		}

		private UInt32 _adresaForVarijable;
		private int _razinaGnijezdjenjaForVarijable;
		private UInt32 _adresaGranice;
		private TipPodatka _tipForVarijable;
	}
}
