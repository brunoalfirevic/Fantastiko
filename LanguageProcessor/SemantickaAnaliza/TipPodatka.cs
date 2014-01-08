using System;
using System.Collections;

namespace JezicniProcesor
{
	enum UgradjeniTipoviEnum
	{
		Nil,
		Integer,
		Real,
		Boolean,
		Pointer,
		Array,
		Record,
		Interval,
		Enumerated
	}

	class KonstruktoriTipova
	{
		public static TipPodatka Nil()
		{
			return new TipPodatka(UgradjeniTipoviEnum.Nil, null, 0, 0, null, null, null);
		}

		public static TipPodatka Boolean()
		{
			return new TipPodatka(UgradjeniTipoviEnum.Boolean, null, 0, 0, null, null, null);
		}

		public static TipPodatka Integer()
		{
			return new TipPodatka(UgradjeniTipoviEnum.Integer, null, int.MinValue, int.MaxValue, null, null, null);
		}

		public static TipPodatka Real()
		{
			return new TipPodatka(UgradjeniTipoviEnum.Real, null, 0, 0, null, null, null);
		}

		public static TipPodatka PointerNa(TipPodatka pokazaniTip)
		{
			return new TipPodatka(UgradjeniTipoviEnum.Pointer, pokazaniTip, 0, 0, null, null, null);
		}

		public static TipPodatka Niz(ICollection listaTipova, TipPodatka bazniTip)
		{
			return new TipPodatka(UgradjeniTipoviEnum.Array, bazniTip, 0, 0, null, listaTipova, null);
		}

		public static TipPodatka Interval(UInt32 donjaGranica, UInt32 gornjaGranica)
		{
			return new TipPodatka(UgradjeniTipoviEnum.Interval, null, KonverterPodataka.BitRepresentationToSignedInt(donjaGranica), KonverterPodataka.BitRepresentationToSignedInt(gornjaGranica), null, null, null);
		}

		public static TipPodatka Pobrojani(ICollection listaImena)
		{
			return new TipPodatka(UgradjeniTipoviEnum.Enumerated, null, 0, listaImena.Count-1, listaImena, null, null);
		}

		public static TipPodatka Record(ICollection listaPolja)
		{
			return new TipPodatka(UgradjeniTipoviEnum.Record, null, 0, 0, null, null, listaPolja);
		}
	}

	/// <summary>
	/// Summary description for TipPodatka.
	/// </summary>
	class TipPodatka
	{
		public TipPodatka()
		{
		}

		public TipPodatka(UgradjeniTipoviEnum ugradjeniTip, TipPodatka podTip, int donjaGranica, int gornjaGranica,
			ICollection listaPobrojanihImena, ICollection listaIndeksa, ICollection listaPolja)
		{
			if (listaPobrojanihImena==null)
			{
				listaPobrojanihImena = new ArrayList();
			}
			if (listaIndeksa==null)
			{
				listaIndeksa = new ArrayList();
			}
			if (listaPolja==null)
			{
				listaPolja = new ArrayList();
			}

			_ugradjeniTip = ugradjeniTip;
			_podTip = podTip;
			_listaPobrojanihImena = listaPobrojanihImena;
			_listaIndeksa = listaIndeksa;
			_listaPolja = listaPolja;
			
			switch(_ugradjeniTip)
			{
				case UgradjeniTipoviEnum.Boolean:
					_velicinaUBajtovima = 1;
					_kardinalitet = 2;
					_donjaGranica = 0;
					_gornjaGranica = 1;
					break;
				case UgradjeniTipoviEnum.Enumerated:
					_velicinaUBajtovima = 4;
					_kardinalitet = _listaPobrojanihImena.Count;
					_donjaGranica = 0;
					_gornjaGranica = _listaPobrojanihImena.Count-1;
					break;
				case UgradjeniTipoviEnum.Integer:
					_velicinaUBajtovima = 4;
					_donjaGranica = Int32.MinValue;
					_gornjaGranica = Int32.MaxValue;
					_kardinalitet = _gornjaGranica - _donjaGranica + 1;
					break;
				case UgradjeniTipoviEnum.Interval:
					_velicinaUBajtovima = 4;
					_donjaGranica = donjaGranica;
					_gornjaGranica = gornjaGranica;
					_kardinalitet = _gornjaGranica - _donjaGranica + 1;
					break;
				case UgradjeniTipoviEnum.Real:
					_velicinaUBajtovima = 4;
					break;
				case UgradjeniTipoviEnum.Pointer:
					_velicinaUBajtovima = 4;
					break;
				case UgradjeniTipoviEnum.Array:
					_velicinaUBajtovima = _podTip.VelicinaUBajtovima;
					foreach(TipPodatka tip in _listaIndeksa)
					{
						_velicinaUBajtovima *= tip.Kardinalitet;
					}
					break;
				case UgradjeniTipoviEnum.Record:
					_velicinaUBajtovima = 0;
					foreach(Polje polje in _listaPolja)
					{
						_velicinaUBajtovima += polje.Tip.VelicinaUBajtovima;
					}
					break;
				case UgradjeniTipoviEnum.Nil:
					_velicinaUBajtovima = 0;
					_kardinalitet = 0;
					break;
			}
		}
		
		public UgradjeniTipoviEnum Tip
		{
			get { return _ugradjeniTip; }
			set { _ugradjeniTip = value; }
		}

		public int VelicinaUBajtovima
		{
			get { return _velicinaUBajtovima; }
		}

		public bool Diskretni
		{
			get
			{
				switch(_ugradjeniTip)
				{
					case UgradjeniTipoviEnum.Boolean:
					case UgradjeniTipoviEnum.Enumerated:
					case UgradjeniTipoviEnum.Integer:
					case UgradjeniTipoviEnum.Interval:
						return true;
					default:
						return false;
				}
			}
		}

		public bool Cjelobrojni
		{
			get
			{
				switch(_ugradjeniTip)
				{
					case UgradjeniTipoviEnum.Integer:
					case UgradjeniTipoviEnum.Interval:
						return true;
					default:
						return false;
				}
			}
		}

		public bool Jednostavni
		{
			get
			{
				switch(_ugradjeniTip)
				{
					case UgradjeniTipoviEnum.Boolean:
					case UgradjeniTipoviEnum.Enumerated:
					case UgradjeniTipoviEnum.Integer:
					case UgradjeniTipoviEnum.Interval:
					case UgradjeniTipoviEnum.Real:
					case UgradjeniTipoviEnum.Nil:
					case UgradjeniTipoviEnum.Pointer:
						return true;
					default:
						return false;
				}
			}
		}

		public int Kardinalitet
		{
			get {return _kardinalitet;}
		}

		public TipPodatka PodTip
		{
			get { return _podTip; }
		}

		public int DonjaGranica
		{
			get { return _donjaGranica; }
		}

		public int GornjaGranica
		{
			get { return _gornjaGranica; }
		}

		//kolekcija objekata tipa string
		public ICollection ListaPobrojanihImena
		{
			get { return _listaPolja; }
		}

		//kolekcija objekata tipa TipPodatka
		public ICollection ListaIndeksa
		{
			get { return _listaIndeksa; }
		}

		//kolekcija objekata tipa Polje
		public ICollection ListaPolja
		{
			get { return _listaPolja; }
		}

		public static bool TipoviJednaki(TipPodatka tip1, TipPodatka tip2)
		{
			if (tip1.Tip!=tip2.Tip)
			{
				return false;
			}
			switch(tip1.Tip)
			{
				case UgradjeniTipoviEnum.Boolean:
				case UgradjeniTipoviEnum.Integer:
				case UgradjeniTipoviEnum.Real:
				case UgradjeniTipoviEnum.Nil:
					return true;
				case UgradjeniTipoviEnum.Enumerated:
				{
					if (tip1.ListaPobrojanihImena.Count!=tip2.ListaPobrojanihImena.Count)
					{
						return false;
					}
					IEnumerator enum1 = tip1.ListaPobrojanihImena.GetEnumerator();
					IEnumerator enum2 = tip2.ListaPobrojanihImena.GetEnumerator();

					while (enum1.MoveNext())
					{
						enum2.MoveNext();

						string s1 = (string)enum1.Current;
						string s2 = (string)enum2.Current;

						if (s1!=s2)
						{
							return false;
						}
					}
					return true;
				}

				case UgradjeniTipoviEnum.Interval:
					return	(tip1.DonjaGranica==tip2.DonjaGranica) &&
						(tip1.GornjaGranica==tip2.GornjaGranica);

				case UgradjeniTipoviEnum.Pointer:
					return TipoviJednaki(tip1.PodTip, tip2.PodTip);

				case UgradjeniTipoviEnum.Array:
				{
					if (!TipoviJednaki(tip1.PodTip, tip2.PodTip))
					{
						return false;
					}
					if (tip1.ListaIndeksa.Count!=tip2.ListaIndeksa.Count)
					{
						return false;
					}

					IEnumerator enum1 = tip1.ListaIndeksa.GetEnumerator();
					IEnumerator enum2 = tip2.ListaIndeksa.GetEnumerator();

					while (enum1.MoveNext())
					{
						enum2.MoveNext();

						TipPodatka t1 = (TipPodatka)enum1.Current;
						TipPodatka t2 = (TipPodatka)enum2.Current;
						
						if (!TipoviJednaki(t1, t2))
						{
							return false;
						}
					}
					return true;
				}

				case UgradjeniTipoviEnum.Record:
				{
					if (tip1.ListaPolja.Count!=tip2.ListaPolja.Count)
					{
						return false;
					}

					IEnumerator enum1 = tip1.ListaPolja.GetEnumerator();
					IEnumerator enum2 = tip2.ListaPolja.GetEnumerator();

					while (enum1.MoveNext())
					{
						enum2.MoveNext();

						Polje p1 = (Polje)enum1.Current;
						Polje p2 = (Polje)enum2.Current;
						
						if ((p1.Identifikator!=p2.Identifikator) || !TipoviJednaki(p1.Tip, p2.Tip))
						{
							return false;
						}
					}
					return true;
				}
				default:
					return false;
			}

		}

		private UgradjeniTipoviEnum _ugradjeniTip;
		private TipPodatka _podTip;
		private int _donjaGranica;
		private int _gornjaGranica;
		private ICollection _listaPobrojanihImena;
		private ICollection _listaIndeksa;
		private ICollection _listaPolja;

		private int _velicinaUBajtovima;
		private int _kardinalitet;
	}
}
