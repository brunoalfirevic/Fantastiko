using System;
using System.Collections;

namespace JezicniProcesor
{
	
	enum TipPotprogramaEnum
	{
		Procedura,
		Funkcija,
		GlavniProgram
	}

	/// <summary>
	/// Summary description for OpisnikPotprograma.
	/// </summary>
	class OpisnikPotprograma
	{
		public OpisnikPotprograma(string identifikator, TipPotprogramaEnum tipPotprograma,
			TipPodatka tipPovratneVrijednosti, ICollection parametri)
		{
			if (tipPovratneVrijednosti==null)
			{
				tipPovratneVrijednosti = KonstruktoriTipova.Nil();
			}
			if (parametri==null)
			{
				parametri = new ArrayList();
			}

			_identifikator = identifikator;
			_tipPotprograma = tipPotprograma;
			
			if (tipPotprograma==TipPotprogramaEnum.Funkcija)
			{
				_tipPovratneVrijednosti = tipPovratneVrijednosti;
			}
			else
			{
				_tipPovratneVrijednosti = null;
			}

			if (tipPotprograma!=TipPotprogramaEnum.GlavniProgram)
			{
				_parametri = parametri;
			}
			else
			{
				_parametri = new ArrayList();
			}

			_velicinaStogaZaLokalneVarijable = 4;
			
			_implementiran = false;
			_deklariran = false;
			_jedinstveniIdentifikator = GeneratorJedinstvenihBrojeva.Generiraj();

			_velicinaStogaZaParametre = 0;
			foreach(Polje parametar in parametri)
			{
				parametar.OdmakOdBazneAdrese+=VelicinaOpisnikaNaStogu;
				_velicinaStogaZaParametre += parametar.Tip.VelicinaUBajtovima;
			}
		}

		public OpisnikPotprograma(string identifikator, TipPotprogramaEnum tipPotprograma,
			TipPodatka tipPovratneVrijednosti, ICollection parametri, bool rezervirajJednoLokalnoMjesto)
		{
			if (tipPovratneVrijednosti==null)
			{
				tipPovratneVrijednosti = KonstruktoriTipova.Nil();
			}
			if (parametri==null)
			{
				parametri = new ArrayList();
			}

			_identifikator = identifikator;
			_tipPotprograma = tipPotprograma;
			
			if (tipPotprograma==TipPotprogramaEnum.Funkcija)
			{
				_tipPovratneVrijednosti = tipPovratneVrijednosti;
			}
			else
			{
				_tipPovratneVrijednosti = null;
			}

			if (tipPotprograma!=TipPotprogramaEnum.GlavniProgram)
			{
				_parametri = parametri;
			}
			else
			{
				_parametri = new ArrayList();
			}
			if (rezervirajJednoLokalnoMjesto)
			{
				_velicinaStogaZaLokalneVarijable = 4;
			}
			else
			{
				_velicinaStogaZaLokalneVarijable = 0;
			}

			_implementiran = false;
			_deklariran = false;
			_jedinstveniIdentifikator = GeneratorJedinstvenihBrojeva.Generiraj();

			_velicinaStogaZaParametre = 0;
			foreach(Polje parametar in parametri)
			{
				parametar.OdmakOdBazneAdrese+=VelicinaOpisnikaNaStogu;
				_velicinaStogaZaParametre += parametar.Tip.VelicinaUBajtovima;
			}
		}

		public TipPotprogramaEnum TipPotprograma
		{
			get { return _tipPotprograma; }
		}

		public TipPodatka TipPovratneVrijednosti
		{
			get { return _tipPovratneVrijednosti; }
		}

		public ICollection Parametri
		{
			get { return _parametri; }
		}

		public string Identifikator
		{
			get { return _identifikator; }
		}

		public int VelicinaStogaZaLokalneVarijable
		{
			get { return _velicinaStogaZaLokalneVarijable; }
			set { _velicinaStogaZaLokalneVarijable = value; }
		}

		public int VelicinaStogaZaParametre
		{
			get { return _velicinaStogaZaParametre; }
		}

		/// <summary>
		/// Velicina stoga za lokalne varijable + velicina stoga za parametre + velicina opisnika procedure na stogu
		/// </summary>
		public int VelicinaRezerviranogStoga
		{
			get { return _velicinaStogaZaParametre + _velicinaStogaZaLokalneVarijable + VelicinaOpisnikaNaStogu; }
		}

		public int VelicinaOpisnikaNaStogu
		{
			get { return 16; }
		}

		public bool Implementiran
		{
			get { return _implementiran; }
		}

		public void OznaciImplementaciju()
		{
			_implementiran = true;
		}

		public bool Deklariran
		{
			get
			{
				return _deklariran;
			}
		}

		public void OznaciDeklaraciju()
		{
			_deklariran = true;
		}

		public int JedinstveniIdentifikator
		{
			get { return _jedinstveniIdentifikator; }
		}

		private bool _deklariran;
		private int _jedinstveniIdentifikator;
		private bool _implementiran;
		private string _identifikator;
		private TipPotprogramaEnum _tipPotprograma;
		private TipPodatka _tipPovratneVrijednosti;
		private ICollection _parametri;

		private int _velicinaStogaZaParametre;
		private int _velicinaStogaZaLokalneVarijable;
	}
}
