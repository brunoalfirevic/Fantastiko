using System;
using System.Collections;

namespace JezicniProcesor
{
	/// <summary>
	/// Ova klasa predstavlja unos u tablicu znakova za varijable. Osim toga se koristi kao opis za bilo kakav par
	/// (identifikator, tip), npr. za polja u recordu
	/// </summary>
	class Polje
	{
		public Polje(string identifikator, TipPodatka tip, int odmakOdBazneAdrese)
		{
			_odmakOdBazneAdrese = odmakOdBazneAdrese;
			_identifikator = identifikator;
			_tip = tip;
		}

		public string Identifikator
		{
			get { return _identifikator; }
		}

		public TipPodatka Tip
		{
			get { return _tip; }
		}

		public int OdmakOdBazneAdrese
		{
			get { return _odmakOdBazneAdrese; }
			set { _odmakOdBazneAdrese = value; }
		}

		private int _odmakOdBazneAdrese;
		private string _identifikator;
		private TipPodatka _tip;
	}
}
