using System;

namespace JezicniProcesor
{
	enum TipZnakaGramatikeEnum
	{
		Zavrsni,
		Nezavrsni
	}

	/// <summary>
	/// Ova klasa sadrzi predefinirane vrijednosti koje prestavljaju neke znakove gramatike. Iako je ovo klasa,
	/// nije namjenjena tome da se prave objekti njenog tipa, vec samo sluzi kao mjesto za definiranje ovih vrijednosti
	/// </summary>
	class OznakeZnakovaGramatike
	{
		public const string Identifikator = "identifier";
		public const string UnsignedInteger = "unsigned-integer";
		public const string UnsignedReal = "unsigned-real";
		public const string Boolean = "boolean-constant";
		public const string String = "string";
		public const string KrajUlaznogNiza = "&";
	}

	abstract class ZnakGramatike
	{
		public ZnakGramatike(string oznakaZnakaGramatike, TipZnakaGramatikeEnum tipZnakaGramatike)
		{
			_oznakaZnakaGramatike = oznakaZnakaGramatike;
			_tipZnakaGramatike = tipZnakaGramatike;
		}

		public string OznakaZnakaGramatike
		{
			get { return _oznakaZnakaGramatike; }
		}

		public TipZnakaGramatikeEnum TipZnakaGramatike
		{
			get { return _tipZnakaGramatike; }
		}

		private string _oznakaZnakaGramatike;
		private TipZnakaGramatikeEnum _tipZnakaGramatike;
	}
}
