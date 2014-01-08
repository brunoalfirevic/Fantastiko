using System;
using System.Collections;

namespace JezicniProcesor
{
	enum UniformniZnakEnum
	{
		Identifikator,
		Konstanta,
		KROS,
		TerminatorUlaznogNiza
	}

	class UniformniZnak : ZnakGramatike
	{
		public UniformniZnak(string oznakaZnakaGramatike, string leksickaJedinka, UniformniZnakEnum uniformniZnakID, int linijaukojojsenalazijedinka, int pozicijauprogramu):
			base(oznakaZnakaGramatike, TipZnakaGramatikeEnum.Zavrsni)
		{
			_leksickaJedinka = leksickaJedinka;
			_linijaUKojojSeNalaziJedinka=linijaukojojsenalazijedinka;
			_uniformniZnakID = uniformniZnakID;
			_pozicijauprogramu=pozicijauprogramu;
		}

		public UniformniZnakEnum UniformniZnakID
		{
			get { return _uniformniZnakID; }
		}

		public int LinijaUKojojSeNalaziJedinka
		{
			get
			{
				return _linijaUKojojSeNalaziJedinka;
			}
		}

		public int PozicijaUProgramu
		{
			get
			{
				return _pozicijauprogramu;
			}
		}

		public string LeksickaJedinka
		{
			get { return _leksickaJedinka; }
		}

		private string _leksickaJedinka;
		private int _pozicijauprogramu;
		private UniformniZnakEnum _uniformniZnakID;
		private int _linijaUKojojSeNalaziJedinka;
	}
}
