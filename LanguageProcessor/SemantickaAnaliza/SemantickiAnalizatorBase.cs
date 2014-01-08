using System;
using System.Collections;

namespace JezicniProcesor
{
	/// <summary>
	/// Summary description for SemantickiAnalizatorBase.
	/// </summary>
	class SemantickiAnalizatorBase
	{
		public SemantickiAnalizatorBase(IList listaGresaka, TablicaZnakova tablicaZnakova,
			IEnumerator enumeratorUniformnihZnakova, bool ulaznoIzlazneProcedure)
		{
			_tablicaZnakova = tablicaZnakova;
			_generatorMedjukoda = new GeneratorMedjukoda();
			_listaGresaka = listaGresaka;
			_enumeratorUniformnihZnakova = enumeratorUniformnihZnakova;
			GenerirajUgradjeneFunkcijeiTipove(ulaznoIzlazneProcedure);
		}

		public string GeneriraniProgram
		{
			get { return _generatorMedjukoda.GeneriraniProgram; }
		}

		protected TablicaZnakova TablicaZnakova
		{
			get { return _tablicaZnakova; }
		}

		protected GeneratorMedjukoda GeneratorMedjukoda
		{
			get { return _generatorMedjukoda; }
		}

		private void GenerirajPocetakPrograma()
		{
			GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Mst, 0);
			GeneratorMedjukoda.Generiraj(TroadresneNaredbeEnum.Cal, 0, 
				GeneratorLabela.GlavniProgram());
			GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Hlt);
		}

		private void GenerirajUlaznoIzlazneProcedure()
		{
			ArrayList parametri;

			//write integer
			parametri = new ArrayList();
			parametri.Add(new Polje("vrijednost", KonstruktoriTipova.Integer(), 0));
			OpisnikPotprograma writeInteger = new OpisnikPotprograma("WriteInteger", TipPotprogramaEnum.Procedura,
				null, parametri, false);
			TablicaZnakova.DodajOpisnikPotprograma(writeInteger);

			GeneratorMedjukoda.PostaviAdresnuLabelu(GeneratorLabela.PocetakPotprograma(writeInteger));
			GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Ent, GeneratorLabela.VelicinaRezerviranogStoga(writeInteger));

			GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Wrt_i);

			GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Rtp);
			GeneratorMedjukoda.PostaviPodatkovnuLabelu(GeneratorLabela.VelicinaRezerviranogStoga(writeInteger),
				KonverterPodataka.SignedIntToBitRepresentation(writeInteger.VelicinaRezerviranogStoga));

			//write line integer
			parametri = new ArrayList();
			parametri.Add(new Polje("vrijednost", KonstruktoriTipova.Integer(), 0));
			OpisnikPotprograma writeLineInteger = new OpisnikPotprograma("WriteLineInteger", TipPotprogramaEnum.Procedura,
				null, parametri, false);
			TablicaZnakova.DodajOpisnikPotprograma(writeLineInteger);

			GeneratorMedjukoda.PostaviAdresnuLabelu(GeneratorLabela.PocetakPotprograma(writeLineInteger));
			GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Ent, GeneratorLabela.VelicinaRezerviranogStoga(writeLineInteger));

			GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Wrt_i);
			GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Lit, 10);
			GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Wrt_c);

			GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Rtp);
			GeneratorMedjukoda.PostaviPodatkovnuLabelu(GeneratorLabela.VelicinaRezerviranogStoga(writeLineInteger),
				KonverterPodataka.SignedIntToBitRepresentation(writeLineInteger.VelicinaRezerviranogStoga));

			//write line real
			parametri = new ArrayList();
			parametri.Add(new Polje("vrijednost", KonstruktoriTipova.Real(), 0));
			OpisnikPotprograma writeLineReal = new OpisnikPotprograma("WriteLineReal", TipPotprogramaEnum.Procedura,
				null, parametri, false);
			TablicaZnakova.DodajOpisnikPotprograma(writeLineReal);

			GeneratorMedjukoda.PostaviAdresnuLabelu(GeneratorLabela.PocetakPotprograma(writeLineReal));
			GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Ent, GeneratorLabela.VelicinaRezerviranogStoga(writeLineReal));

			GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Wrt_r);
			GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Lit, 10);
			GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Wrt_c);

			GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Rtp);
			GeneratorMedjukoda.PostaviPodatkovnuLabelu(GeneratorLabela.VelicinaRezerviranogStoga(writeLineReal),
				KonverterPodataka.SignedIntToBitRepresentation(writeLineReal.VelicinaRezerviranogStoga));

			//write real
			parametri = new ArrayList();
			parametri.Add(new Polje("vrijednost", KonstruktoriTipova.Real(), 0));
			OpisnikPotprograma writeReal = new OpisnikPotprograma("WriteReal", TipPotprogramaEnum.Procedura,
				null, parametri, false);
			TablicaZnakova.DodajOpisnikPotprograma(writeReal);

			GeneratorMedjukoda.PostaviAdresnuLabelu(GeneratorLabela.PocetakPotprograma(writeReal));
			GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Ent, GeneratorLabela.VelicinaRezerviranogStoga(writeReal));

			GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Wrt_r);

			GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Rtp);
			GeneratorMedjukoda.PostaviPodatkovnuLabelu(GeneratorLabela.VelicinaRezerviranogStoga(writeReal),
				KonverterPodataka.SignedIntToBitRepresentation(writeReal.VelicinaRezerviranogStoga));

			//read integer
			parametri = new ArrayList();
			parametri.Add(new Polje("vrijednost", KonstruktoriTipova.PointerNa(KonstruktoriTipova.Integer()), 0));
			OpisnikPotprograma readInteger = new OpisnikPotprograma("ReadInteger", TipPotprogramaEnum.Procedura,
				null, parametri, false);
			TablicaZnakova.DodajOpisnikPotprograma(readInteger);

			GeneratorMedjukoda.PostaviAdresnuLabelu(GeneratorLabela.PocetakPotprograma(readInteger));
			GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Ent, GeneratorLabela.VelicinaRezerviranogStoga(readInteger));

			GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Red_i);
			GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Sti);

			GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Rtp);
			GeneratorMedjukoda.PostaviPodatkovnuLabelu(GeneratorLabela.VelicinaRezerviranogStoga(readInteger),
				KonverterPodataka.SignedIntToBitRepresentation(readInteger.VelicinaRezerviranogStoga));

			//read real
			parametri = new ArrayList();
			parametri.Add(new Polje("vrijednost", KonstruktoriTipova.PointerNa(KonstruktoriTipova.Real()), 0));
			OpisnikPotprograma readReal = new OpisnikPotprograma("ReadReal", TipPotprogramaEnum.Procedura,
				null, parametri, false);
			TablicaZnakova.DodajOpisnikPotprograma(readReal);

			GeneratorMedjukoda.PostaviAdresnuLabelu(GeneratorLabela.PocetakPotprograma(readReal));
			GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Ent, GeneratorLabela.VelicinaRezerviranogStoga(readReal));

			GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Red_r);
			GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Sti);

			GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Rtp);
			GeneratorMedjukoda.PostaviPodatkovnuLabelu(GeneratorLabela.VelicinaRezerviranogStoga(readReal),
				KonverterPodataka.SignedIntToBitRepresentation(readReal.VelicinaRezerviranogStoga));
		}

		private void GenerirajInkrementDekrement()
		{
			ArrayList parametri;

			//inc
			parametri = new ArrayList();
			parametri.Add(new Polje("vrijednost", KonstruktoriTipova.PointerNa(KonstruktoriTipova.Integer()), 0));
			OpisnikPotprograma inc = new OpisnikPotprograma("inc", TipPotprogramaEnum.Procedura,
				null, parametri, false);
			TablicaZnakova.DodajOpisnikPotprograma(inc);

			GeneratorMedjukoda.PostaviAdresnuLabelu(GeneratorLabela.PocetakPotprograma(inc));
			GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Ent, GeneratorLabela.VelicinaRezerviranogStoga(inc));

			GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Dup);
			GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Lti);
			GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Inc);
			GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Sti);

			GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Rtp);
			GeneratorMedjukoda.PostaviPodatkovnuLabelu(GeneratorLabela.VelicinaRezerviranogStoga(inc),
				KonverterPodataka.SignedIntToBitRepresentation(inc.VelicinaRezerviranogStoga));


			//dec
			parametri = new ArrayList();
			parametri.Add(new Polje("vrijednost", KonstruktoriTipova.PointerNa(KonstruktoriTipova.Integer()), 0));
			OpisnikPotprograma dec = new OpisnikPotprograma("dec", TipPotprogramaEnum.Procedura,
				null, parametri, false);
			TablicaZnakova.DodajOpisnikPotprograma(dec);

			GeneratorMedjukoda.PostaviAdresnuLabelu(GeneratorLabela.PocetakPotprograma(dec));
			GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Ent, GeneratorLabela.VelicinaRezerviranogStoga(dec));


			GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Dup);
			GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Lti);
			GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Dec);
			GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Sti);

			GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Rtp);
			GeneratorMedjukoda.PostaviPodatkovnuLabelu(GeneratorLabela.VelicinaRezerviranogStoga(dec),
				KonverterPodataka.SignedIntToBitRepresentation(dec.VelicinaRezerviranogStoga));
		}

		private void GenerirajUgradjeneFunkcijeiTipove(bool ulaznoIzlazneProcedure)
		{
			TablicaZnakova.DodajTipPodatka("integer", KonstruktoriTipova.Integer());
			TablicaZnakova.DodajTipPodatka("boolean", KonstruktoriTipova.Boolean());
			TablicaZnakova.DodajTipPodatka("real", KonstruktoriTipova.Real());

			GenerirajPocetakPrograma();
			GenerirajInkrementDekrement();
			if (ulaznoIzlazneProcedure)
			{
				GenerirajUlaznoIzlazneProcedure();
			}
		}

		protected UniformniZnak TrenutniZnak
		{
			get 
			{ 
				try
				{
					return (UniformniZnak)_enumeratorUniformnihZnakova.Current;
				}
				catch(InvalidOperationException)
				{
					throw new FatalnaGreskaException(-1,-1, "Neocekivani kraj ulaznog programa.");					
				}
			}
		}

		protected void PrijaviGresku(string greska)
		{
			GreskaJezicnogProcesora objGreska;
			objGreska = new GreskaJezicnogProcesora(TrenutniZnak.LinijaUKojojSeNalaziJedinka,
				TrenutniZnak.PozicijaUProgramu, "'" + TrenutniZnak.OznakaZnakaGramatike + "'" + " " + greska);
			_listaGresaka.Add(objGreska);
		}

		private GeneratorMedjukoda _generatorMedjukoda;
		private IList _listaGresaka;
		private TablicaZnakova _tablicaZnakova;
		private IEnumerator _enumeratorUniformnihZnakova;
	}
}
