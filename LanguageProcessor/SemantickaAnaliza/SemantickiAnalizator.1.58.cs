using System;
using System.Collections;

namespace JezicniProcesor
{
	/// <summary>
	/// Summary description for SemantickiAnalizator.
	/// </summary>
	class SemantickiAnalizator : SemantickiAnalizatorBase
	{
		public SemantickiAnalizator(IList listaGresaka, TablicaZnakova tablicaZnakova,
			IEnumerator enumeratorZavrsnihZnakova, bool ulaznoIzlazneProcedure):
			base(listaGresaka, tablicaZnakova, enumeratorZavrsnihZnakova, ulaznoIzlazneProcedure)
		{
			_kontekst = new Stack();
			_naredbe = new Hashtable();
		}

		public void InicijalizirajProgram(string nImeIdentifikatora)
		{
			OpisnikPotprograma opisnik = new OpisnikPotprograma(nImeIdentifikatora, TipPotprogramaEnum.GlavniProgram,
				null, null);

			TablicaZnakova.DodajOpisnikPotprograma(opisnik);
		}

		public void KreirajNilKonstantu(out TipPodatka iTip, out UInt32 iVrijednost)
		{
			iTip = KonstruktoriTipova.Nil();
			iVrijednost = 0;
		}

		public void NovaLabela(UInt32 nVrijednost)
		{
			if (TablicaZnakova.PronadjiLabelu(nVrijednost)!=null)
			{
				PrijaviGresku("Labela vec deklarirana.");
			}
			else
			{
				TablicaZnakova.DodajLabelu(new Labela(nVrijednost));
			}
		}
		
		public void ZapocelaImplementacija(string nImeIdentifikatora)
		{
			OpisnikPotprograma opisnik;
			int razina;
			TablicaZnakova.PronadjiPotprogram(nImeIdentifikatora, out opisnik, out razina);
			if (opisnik==null)
			{
				PrijaviGresku("Neispravno deklariran potprogram " + nImeIdentifikatora + ".");
			}
			else
			{
				if (opisnik.TipPotprograma==TipPotprogramaEnum.GlavniProgram)
				{
					GeneratorMedjukoda.PostaviAdresnuLabelu(GeneratorLabela.GlavniProgram());
				}
				else
				{
					GeneratorMedjukoda.PostaviAdresnuLabelu(GeneratorLabela.PocetakPotprograma(opisnik));
				}
				GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Ent, GeneratorLabela.VelicinaRezerviranogStoga(opisnik));

				//za funkcije, dodaj varijablu koja predstavlja povratnu vrijednost
				if (opisnik.TipPotprograma==TipPotprogramaEnum.Funkcija)
				{
					Polje varijabla = new Polje(opisnik.Identifikator, opisnik.TipPovratneVrijednosti, 0);
					TablicaZnakova.DodajLokalnuVarijablu(varijabla);
				}
			}
		}
		
		public void ZavrsilaImplementacija(string nImeIdentifikatora)
		{
			OpisnikPotprograma opisnik;
			int razina;
			TablicaZnakova.PronadjiPotprogram(nImeIdentifikatora, out opisnik, out razina);
			if (opisnik!=null)
			{
				if (opisnik.TipPotprograma==TipPotprogramaEnum.Funkcija)
				{
					GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Rtf);
				}
				else
				{
					GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Rtp);
				}
				GeneratorMedjukoda.PostaviPodatkovnuLabelu(GeneratorLabela.VelicinaRezerviranogStoga(opisnik),
					KonverterPodataka.SignedIntToBitRepresentation(opisnik.VelicinaRezerviranogStoga));
			}
		}		
				
		public void KreirajListuPolja(ICollection nListaImena, TipPodatka nTip, out ICollection iNovaLista)
		{
			ICollection tmpLista;
			KreirajPraznuListuPolja(out tmpLista);
			NadopuniListuPolja(nListaImena, nTip, tmpLista, out iNovaLista);
		}
		
		public void NadopuniListuIndeksa(TipPodatka nTip,ICollection nPostojecaListaIndeksa,out ICollection iDopunjenaListaIndeksa)
		{
			ArrayList tmpLista = new ArrayList(nPostojecaListaIndeksa);
			tmpLista.Add(nTip);
			iDopunjenaListaIndeksa = tmpLista;
		}

		public void KreirajPraznuListuPolja(out ICollection iListaPolja)
		{
			iListaPolja = new ArrayList();
		}

		public void NadopuniListuPolja(ICollection nListaImena,TipPodatka nTip,ICollection nPostojecaListaPolja,out ICollection iListaPolja1)
		{
			ArrayList tmpLista = new ArrayList(nPostojecaListaPolja);
			Polje zadnjePostojecePolje = null;
			IEnumerator enumeratorPolja = nPostojecaListaPolja.GetEnumerator();

			//pronadji zadnje postojece polje kako bi se mogla izracunati adresa polja koje se dodaje
			while (enumeratorPolja.MoveNext())
			{
				zadnjePostojecePolje = (Polje)enumeratorPolja.Current;
			}
			
			foreach(string identifikator in nListaImena)
			{
				//adresa polja koje se dodaje jednaka je adresi zadnjeg polja uvecanoj za velicinu zadnjeg polja
				//(dakle novo polje se nalazi odmah iza starog u memoriji)
				Polje objPolje;
				if (zadnjePostojecePolje!=null)
				{
					objPolje = new Polje(identifikator, nTip, 
						zadnjePostojecePolje.OdmakOdBazneAdrese + zadnjePostojecePolje.Tip.VelicinaUBajtovima);
				}
				else
				{
					objPolje = new Polje(identifikator, nTip, 0);
				}
				tmpLista.Add(objPolje);
				zadnjePostojecePolje = objPolje;
			}

			iListaPolja1 = tmpLista;
		}

		public void KreirajPobrojaniTip(ICollection nListaImena, out TipPodatka iTip)
		{
			//vec je osigurano i provjereno da su sva imena u listi imena razlicita
			iTip = KonstruktoriTipova.Pobrojani(nListaImena);

			UInt32 vrijednost = 0;
			foreach(string s in nListaImena)
			{
				if (TablicaZnakova.PostojiIdentifikator(s, true))
				{
					PrijaviGresku("Identifikator " + s + " vec deklariran.");
				}
				else
				{
					KonstantniIdentifikator konstIdent = new KonstantniIdentifikator(s, iTip, vrijednost);
					TablicaZnakova.DodajKonstantniIdentifikator(konstIdent);
					vrijednost++;
				}
			}
		}

		public void KreirajIntervalniTip(UInt32 nVrijednost1, UInt32 nVrijednost2, out TipPodatka iTip)
		{
			//nVrijednost1 je donja, a nVrijednost 2 gornja granica
			if (nVrijednost1 > nVrijednost2)
			{
				PrijaviGresku("Donja granica intervalnog tipa mora biti veca od gornje.");
				iTip = KonstruktoriTipova.Interval(nVrijednost2, nVrijednost1);
			}
			else if (nVrijednost1 > int.MaxValue || nVrijednost2 > int.MaxValue)
			{
				PrijaviGresku("Prevelike granice intervalnog tipa.");
				iTip = KonstruktoriTipova.Interval(0, int.MaxValue);
			}
			else
			{
				iTip = KonstruktoriTipova.Interval(nVrijednost1,nVrijednost2);
			}
		}

		public void DohvatiTip(string nImeTipa, out TipPodatka iTip)
		{
			//ugradjeni tipovi se dodaju na pocetku u tablicu znakova (integer, boolean)
			TablicaZnakova.DohvatiTipPodatka(nImeTipa, out iTip);
			if (iTip==null)
			{
				PrijaviGresku("Nepostojeci tip " + nImeTipa + ".");
				iTip = KonstruktoriTipova.Nil();
			}
		}

		public void KreirajNiz(ICollection nListaTipova, TipPodatka nBazniTip, out TipPodatka iTip)
		{
			iTip = KonstruktoriTipova.Niz(nListaTipova,nBazniTip);

			if (iTip.VelicinaUBajtovima>_maksimalnaVelicinaVarijable)
			{
				PrijaviGresku("Velicina tipa prevelika. Maksimalna velicina tipa je " + _maksimalnaVelicinaVarijable.ToString() + ".");
			}
		}

		public void OtvorenBlok(string nImePotprograma)
		{
			TablicaZnakova.OtvorenBlok();
			OpisnikPotprograma opisnik;
			int razina;
			TablicaZnakova.PronadjiPotprogram(nImePotprograma, out opisnik, out razina);
			if (opisnik!=null)
			{
				_kontekst.Push(opisnik);
			}
		}

		public void ZatvorenBlok(string nImePotprograma)
		{
			TablicaZnakova.ZatvorenBlok();
			if (_kontekst.Count>0)
			{
				OpisnikPotprograma opisnik = (OpisnikPotprograma)_kontekst.Pop();
			}
		}

		public void NovaKonstanta(string nImeIdentifikatora, TipPodatka nTipKonstante, UInt32 nVrijednost)
		{
			if (TablicaZnakova.PostojiIdentifikator(nImeIdentifikatora, true))
			{
				PrijaviGresku("Identifikator " + nImeIdentifikatora + " je vec deklariran.");
			}
			else
			{
				KonstantniIdentifikator tmp = new KonstantniIdentifikator(nImeIdentifikatora,nTipKonstante,nVrijednost);
				TablicaZnakova.DodajKonstantniIdentifikator(tmp);
			}
		}

		public void NoviTip(string nImeIdentifikatora, TipPodatka nTip)
		{
			if (TablicaZnakova.PostojiIdentifikator(nImeIdentifikatora, true))
			{
				PrijaviGresku("Identifikator " + nImeIdentifikatora + " je vec deklariran.");
			}
			else
			{
				TablicaZnakova.DodajTipPodatka(nImeIdentifikatora, nTip);
			}
		}

		public void NovaVarijabla(ICollection nListaImena, TipPodatka nTip)
		{
			foreach(string s in nListaImena)
			{
				if (TablicaZnakova.PostojiIdentifikator(s, true))
				{
					PrijaviGresku("Identifikator " + s + " je vec deklariran.");
				}
				else
				{
					Polje varijabla = new Polje(s, nTip,
						TrenutniKontekst.VelicinaRezerviranogStoga);
					TrenutniKontekst.VelicinaStogaZaLokalneVarijable += nTip.VelicinaUBajtovima;
					TablicaZnakova.DodajLokalnuVarijablu(varijabla);
				}

			}
		}

		public void NapraviStrukturu(ICollection nListaPolja, out TipPodatka iTip)
		{
			iTip = KonstruktoriTipova.Record(nListaPolja);
			if (iTip.VelicinaUBajtovima>_maksimalnaVelicinaVarijable)
			{
				PrijaviGresku("Velicina tipa prevelika. Maksimalna velicina tipa je " + _maksimalnaVelicinaVarijable.ToString() + ".");
			}
		}

		public void StvoriListuIndeksa(TipPodatka nTip, out ICollection iListaIndeksa)
		{
			ArrayList tmp = new ArrayList();
			tmp.Add(nTip);
			iListaIndeksa = tmp;
		}

		public void KreirajListuImena(string nImeIdentifikatora, out ICollection iListaImena)
		{
			ArrayList temp = new ArrayList();
			temp.Add(nImeIdentifikatora);
			iListaImena = temp;
		}
		
		public void NadopuniListuImena(string nImeIdentifikatora, ICollection nPostojecaListaImena, out ICollection iListaImena)
		{
			ArrayList templista = new ArrayList(nPostojecaListaImena);

			//imena u listi moraju biti jedinstvena
			foreach(string s in nPostojecaListaImena)
			{
				if (s==nImeIdentifikatora)
				{
					PrijaviGresku("Identifikator " + nImeIdentifikatora + " vec deklariran.");
					iListaImena = templista;
					return;
				}
			}

			templista.Add(nImeIdentifikatora);
			iListaImena = templista;
		}		

		public void ProvjeriVarijabluIliProceduru(string nImeIdentifikatora)
		{
			Polje tmp;
			int razina;
			TablicaZnakova.PronadjiVarijablu(nImeIdentifikatora, out tmp, out razina);
			if (tmp!=null)
			{
				return;
			}

			OpisnikPotprograma opisnik;
			
			TablicaZnakova.PronadjiPotprogram(nImeIdentifikatora, TipPotprogramaEnum.Procedura, out opisnik, out razina);
			if (opisnik!=null)
			{
				return;
			}

			PrijaviGresku("Ocekivana varijabla ili procedura.");
		}

		//Provjeri: Konstantu/Tip/Funkciju/Proceduru/Varijablu
		public void ProvjeriKonstantu(string nImeIdentifikatora)
		{	
			KonstantniIdentifikator tmp;
			TablicaZnakova.PronadjiKonstantniIdentifikator(nImeIdentifikatora, out tmp); 
			if (tmp!=null)
			{
				return;
			}

			PrijaviGresku("Ocekivana konstanta.");
		}
		
		public void ProvjeriFunkciju(string nImeIdentifikatora)
		{	
			int razina;
			OpisnikPotprograma opisnik;
			TablicaZnakova.PronadjiPotprogram(nImeIdentifikatora, TipPotprogramaEnum.Funkcija, out opisnik, out razina);
			if (opisnik!=null)
			{
				return;
			}

			PrijaviGresku("Ocekivana funkcija.");
		}
		
		public void ProvjeriProceduru(string nImeIdentifikatora)
		{	

			int razina;
			OpisnikPotprograma opisnik;
			TablicaZnakova.PronadjiPotprogram(nImeIdentifikatora, TipPotprogramaEnum.Procedura, out opisnik, out razina);
			if (opisnik!=null)
			{
				return;
			}

			PrijaviGresku("Ocekivana procedura.");
		}
							
		public void ProvjeriVarijablu(string nImeIdentifikatora)
		{	
			Polje tmp;
			int razina;
			TablicaZnakova.PronadjiVarijablu(nImeIdentifikatora, out tmp, out razina);
			if (tmp!=null)
			{
				return;
			}
			PrijaviGresku("Ocekivana varijabla.");
		}	
	
		public void ProvjeriKonstantnostIzraza(bool nSveKonstantno)
		{
			if (!nSveKonstantno)
			{
				PrijaviGresku("Ocekivan konstanti izraz.");
			}
		}

		public void IzracunajVarijabluIzIdentifikatora(string nImeIdentifikatora, out TipPodatka inTrenutniTipVarijable,
			out VrstaVarijable inTrenutnaVrstaVarijable, out UInt32 inTrenutnaAdresa)
		{
			Polje varijabla;
			int razina;
			TablicaZnakova.PronadjiVarijablu(nImeIdentifikatora, out varijabla, out razina);
			if (varijabla!=null)
			{
				inTrenutniTipVarijable = varijabla.Tip;
				inTrenutnaVrstaVarijable = new VrstaVarijable(VrstaVarijableEnum.LokalnaVarijabla, razina);
				inTrenutnaAdresa = KonverterPodataka.SignedIntToBitRepresentation(varijabla.OdmakOdBazneAdrese);
			}
			else
			{
				KonstantniIdentifikator konstIdent;
				TablicaZnakova.PronadjiKonstantniIdentifikator(nImeIdentifikatora, out konstIdent);
				if (konstIdent!=null)
				{
					inTrenutniTipVarijable = konstIdent.Tip;
					inTrenutnaVrstaVarijable = new VrstaVarijable(VrstaVarijableEnum.KonstantniIdentifikator, konstIdent.Vrijednost);
					inTrenutnaAdresa = 0;
				}
				else
				{
					//greska
					if (TablicaZnakova.PostojiIdentifikator(nImeIdentifikatora, false))
					{
						PrijaviGresku("Ocekivana varijabla ili konstantni identifikator.");
					}
					else
					{
						PrijaviGresku("Nepoznati identifikator " + nImeIdentifikatora + ".");
					}
					inTrenutniTipVarijable = KonstruktoriTipova.Nil();
					inTrenutnaVrstaVarijable = new VrstaVarijable(VrstaVarijableEnum.LokalnaVarijabla, 0);
					inTrenutnaAdresa = 0;
				}
			}
		}
		
		public void ObaviPridruzivanje(TipPodatka inTipVarijable, VrstaVarijable inVrstaVarijable,
			UInt32 inAdresa, TipPodatka inTipIzraza, UInt32 inVrijednostIzraza, bool inSveKonstantno)
		{
			if (inVrstaVarijable.VratiVrstuVarijable==VrstaVarijableEnum.KonstantniIdentifikator)
			{
				PrijaviGresku("Lijeva strana izraza nije l-value.");
				return;
			}
			if (!inTipVarijable.Jednostavni)
			{
				PrijaviGresku("Samo jednostavni tipovi se mogu direktno pridruzivati.");
				return;
			}

			//provjeri kompatibilnost tipova
			if (inTipVarijable.Tip==UgradjeniTipoviEnum.Real) //ako je varijabla tipa Real, njoj se mogu pridruziti realni, intervalni i integer tipovi
			{
				if (inTipIzraza.Tip==UgradjeniTipoviEnum.Real)
				{
					//ako je sve konstantno stavimo izracunatu vrijednost na vrh stoga, inace je vec na stogu
					if (inSveKonstantno)
					{
						GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Lit, inVrijednostIzraza);
					}
				}
				else if (inTipIzraza.Tip==UgradjeniTipoviEnum.Integer || inTipIzraza.Tip==UgradjeniTipoviEnum.Interval)
				{
					//ako je sve konstantno stavimo izracunatu vrijednost na vrh stoga, inace je vec na stogu
					//vrijednost prvo pretvorimo u float
					if (inSveKonstantno)
					{
						int signedVrijednost = KonverterPodataka.BitRepresentationToSignedInt(inVrijednostIzraza);
						float floatVrijednost = (float)signedVrijednost;
						GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Lit, KonverterPodataka.FloatToBitRepresentation(floatVrijednost));
					}
					else
					{
						GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Flt);
					}
				}
				else
				{
					PrijaviGresku("Nekompatibilni tipovi.");
					return;
				}
			}
			else if (inTipVarijable.Tip==UgradjeniTipoviEnum.Integer) //ako je varijabla tipa Integer njoj se mogu pridruziti integer i intervalni tipovi
			{
				if (inTipIzraza.Tip==UgradjeniTipoviEnum.Integer || inTipIzraza.Tip==UgradjeniTipoviEnum.Interval)
				{
					//ako je sve konstantno stavimo izracunatu vrijednost na vrh stoga, inace je vec na stogu
					if (inSveKonstantno)
					{
						GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Lit, inVrijednostIzraza);
					}
				}
				else
				{
					PrijaviGresku("Nekompatibilni tipovi.");
					return;
				}
			}
			//ako je varijabla tipa interval moze joj se pridruziti samo drugi intervalni tip koji je njen podskup,
			//ili konstanta koja je u ispravnom intervalu
			else if (inTipVarijable.Tip==UgradjeniTipoviEnum.Interval)
			{
				if (inSveKonstantno)
				{
					if (inTipIzraza.Tip==UgradjeniTipoviEnum.Integer || inTipIzraza.Tip==UgradjeniTipoviEnum.Interval)
					{
						//po pravilima jezika, granice intervala mogu biti samo unsigned-integer,
						//pa ne treba nista pretvarat
						if (inVrijednostIzraza>=inTipVarijable.DonjaGranica && inVrijednostIzraza>=inTipVarijable.GornjaGranica)
						{
							GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Lit, inVrijednostIzraza);
						}
						else
						{
							PrijaviGresku("Podatak ispada iz granica intervalnog tipa.");
						}
					}
					else
					{
						PrijaviGresku("Nekompatibilni tipovi.");
					}
				}
				else
				{
					if (inTipIzraza.Tip == UgradjeniTipoviEnum.Interval &&
						inTipIzraza.DonjaGranica >= inTipVarijable.DonjaGranica &&
						inTipIzraza.GornjaGranica <= inTipVarijable.GornjaGranica)
					{
						GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Lit, inVrijednostIzraza);
					}
					else
					{
						PrijaviGresku("Nekompatibilni tipovi.");
					}
				}
			}
			else //tipovi moraju biti potpuno jednaki
			{
				if (TipPodatka.TipoviJednaki(inTipVarijable, inTipIzraza))
				{
					//ako je sve konstantno stavimo izracunatu vrijednost na vrh stoga, inace je vec na stogu
					if (inSveKonstantno)
					{
						GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Lit, inVrijednostIzraza);
					}
				}
				else
				{
					PrijaviGresku("Nekompatibilni tipovi.");
					return;
				}
			}

			//obavi samo spremanje, podatak za spreminit je sad na vrhu stoga i to u ispravnom obliku
			switch(inVrstaVarijable.VratiVrstuVarijable)
			{
				case VrstaVarijableEnum.AdresaNaStogu:
					if (inTipVarijable.VelicinaUBajtovima==1)
					{
						//32 bitni podatak na vrhu stoga sprema u memoriju kao 8 bitni podatak na adresu koja se nalazi ispod
						//njega na stogu.
						GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Sti_b);
					}
					else //velicina je 4 bajta
					{
						//32 bitni podatak na vrhu stoga sprema u memoriju na adresu koja se nalazi ispod
						//njega na stogu, adresa i podatak se izbacuju s stoga.
						GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Sti);
					}
					break;
				case VrstaVarijableEnum.LokalnaVarijabla:
					if (inTipVarijable.VelicinaUBajtovima==1)
					{
						//32 bitnu vrijednost s vrha stoga sprema u memoriju na adresu BR(n)+x kao 8 bitnu vrijednost.
						GeneratorMedjukoda.Generiraj(TroadresneNaredbeEnum.Sto_b, 
							KonverterPodataka.SignedIntToBitRepresentation(inVrstaVarijable.DubinaGnijezdjenja),
							inAdresa);
					}
					else //velicina je 4 bajta
					{
						//32 bitnu vrijednost s vrha stoga sprema u memoriju na adresu BR(n)+x.
						GeneratorMedjukoda.Generiraj(TroadresneNaredbeEnum.Sto, 
							KonverterPodataka.SignedIntToBitRepresentation(inVrstaVarijable.DubinaGnijezdjenja),
							inAdresa);
					}
					break;
			}
		}

		public void PripremiPozivPotprograma(string nImePotprograma)
		{
			OpisnikPotprograma opisnik;
			int razina;
			TablicaZnakova.PronadjiPotprogram(nImePotprograma, out opisnik, out razina);

			if (opisnik!=null)
			{
				GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Mst, KonverterPodataka.SignedIntToBitRepresentation(razina));
			}
		}

		public void PozoviFunkciju(string nImePotprograma, out TipPodatka iTipKojiSeVraca)
		{
			OpisnikPotprograma opisnik;
			int razina;
			TablicaZnakova.PronadjiPotprogram(nImePotprograma, TipPotprogramaEnum.Funkcija, out opisnik, out razina);

			if (opisnik!=null)
			{
				GeneratorMedjukoda.Generiraj(TroadresneNaredbeEnum.Cal,
					KonverterPodataka.SignedIntToBitRepresentation(opisnik.VelicinaStogaZaParametre),
					GeneratorLabela.PocetakPotprograma(opisnik));
				iTipKojiSeVraca = opisnik.TipPovratneVrijednosti;
			}
			else
			{
				if (TablicaZnakova.PostojiIdentifikator(nImePotprograma, false))
				{
					PrijaviGresku("Ocekivano ime procedure.");
				}
				else
				{
					PrijaviGresku("Nepoznati identifikator.");
				}
				iTipKojiSeVraca = KonstruktoriTipova.Nil();
			}
		}
		
		public void PozoviProceduru(string nImeIdentifikatora)
		{
			OpisnikPotprograma opisnik;
			int razina;
			TablicaZnakova.PronadjiPotprogram(nImeIdentifikatora, TipPotprogramaEnum.Procedura, out opisnik, out razina);

			if (opisnik!=null)
			{
				GeneratorMedjukoda.Generiraj(TroadresneNaredbeEnum.Cal,
					KonverterPodataka.SignedIntToBitRepresentation(opisnik.VelicinaStogaZaParametre),
					GeneratorLabela.PocetakPotprograma(opisnik));
			}
			else
			{
				if (TablicaZnakova.PostojiIdentifikator(nImeIdentifikatora, false))
				{
					PrijaviGresku("Ocekivano ime procedure.");
				}
				else
				{
					PrijaviGresku("Nepoznati identifikator.");
				}
			}
		}

		public void EvaluirajVarijablu(TipPodatka nTipVarijable, VrstaVarijable nVrstaVarijable,
			UInt32 nAdresa, out UInt32 iVrijednostVarijable, out bool iVarijablaKonstantna)
		{
			iVrijednostVarijable = 0;
			iVarijablaKonstantna = false;
			if (!nTipVarijable.Jednostavni)
			{
				PrijaviGresku("Direktno se moze manipulirati samo jednostavnim tipovima.");
				return;
			}
			switch(nVrstaVarijable.VratiVrstuVarijable)
			{
				case VrstaVarijableEnum.KonstantniIdentifikator:
					iVrijednostVarijable = nVrstaVarijable.PoznataVrijednost;
					iVarijablaKonstantna = true;
					break;
				case VrstaVarijableEnum.AdresaNaStogu:
					if (nTipVarijable.VelicinaUBajtovima==1)
					{
						GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Lti_b);
					}
					else 
					{
						GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Lti);
					}
					break;
				case VrstaVarijableEnum.LokalnaVarijabla:
					if (nTipVarijable.VelicinaUBajtovima==1)
					{
						GeneratorMedjukoda.Generiraj(TroadresneNaredbeEnum.Lod_b,
							KonverterPodataka.SignedIntToBitRepresentation(nVrstaVarijable.DubinaGnijezdjenja),
							nAdresa);
					}
					else 
					{
						GeneratorMedjukoda.Generiraj(TroadresneNaredbeEnum.Lod,
							KonverterPodataka.SignedIntToBitRepresentation(nVrstaVarijable.DubinaGnijezdjenja),
							nAdresa);
					}
					break;
			}
		}

		public void EvaluirajAdresuVarijable(TipPodatka nTipVarijable, VrstaVarijable nVrstaVarijable,
			UInt32 nAdresa, out TipPodatka iTipAdrese, out UInt32 iVrijednostAdrese, out bool iAdresaKonstantna)
		{
			//adresa nikad nije konstantna (redudantna svojstva)
			iTipAdrese = KonstruktoriTipova.PointerNa(nTipVarijable);
			iVrijednostAdrese = 0;
			iAdresaKonstantna = false;
			switch(nVrstaVarijable.VratiVrstuVarijable)
			{
				case VrstaVarijableEnum.KonstantniIdentifikator:
					PrijaviGresku("Ne moze se uzeti adresa konstante.");
					break;
				case VrstaVarijableEnum.AdresaNaStogu:
					//ne radi nista, adresa je vec na stogu
					break;
				case VrstaVarijableEnum.LokalnaVarijabla:
					//stavi adresu na stog
					GeneratorMedjukoda.Generiraj(TroadresneNaredbeEnum.Lda, 
						KonverterPodataka.SignedIntToBitRepresentation(nVrstaVarijable.DubinaGnijezdjenja),
						nAdresa);
					break;
			}
		}

		public void IzracunajElementNiza(TipPodatka nTrenutniTipVarijable, VrstaVarijable nTrenutnaVrstaVarijable,
			UInt32 nTrenutnaAdresa, out TipPodatka iTipVarijable, out VrstaVarijable iVrstaVarijable,
			out UInt32 iAdresa)
		{
			if (nTrenutniTipVarijable.Tip!=UgradjeniTipoviEnum.Array)
			{
				PrijaviGresku("Ocekivan niz.");
				iTipVarijable = KonstruktoriTipova.Nil();
				iVrstaVarijable = new VrstaVarijable(VrstaVarijableEnum.AdresaNaStogu);
				iAdresa = 0;
				return;
			}

			//na vrhu stoga je ispravan offset od pocetka niza
			iTipVarijable = nTrenutniTipVarijable.PodTip;
			iVrstaVarijable = new VrstaVarijable(VrstaVarijableEnum.AdresaNaStogu);
			iAdresa = 0;

			//u slucaju da je vrsta varijable LokalnaVarijabla, treba na stog staviti adresu prvog
			//elementa niza, dakle adresu samog niza. Ako je vrsta varijable AdresaNaStogu, onda
			//su 2 elementa na vrhu stoga offset od pocetka niza i adresa pocetka niza
			if (nTrenutnaVrstaVarijable.VratiVrstuVarijable==VrstaVarijableEnum.LokalnaVarijabla)
			{
				GeneratorMedjukoda.Generiraj(TroadresneNaredbeEnum.Lda,
					KonverterPodataka.SignedIntToBitRepresentation(nTrenutnaVrstaVarijable.DubinaGnijezdjenja),
					nTrenutnaAdresa);
			}
			
			//sad su 2 elementa na vrhu stoga offset od pocetka niza i adresa pocetka niza, zbroji ih
			GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Add_i);
		}

		public void IzracunajPolje(TipPodatka nTrenutniTipVarijable, VrstaVarijable nTrenutnaVrstaVarijable,
			UInt32 nTrenutnaAdresa, string nImePolja, out TipPodatka iTipVarijable,
			out VrstaVarijable iVrstaVarijable, out UInt32 iAdresa)
		{
			iTipVarijable = KonstruktoriTipova.Nil();
			iVrstaVarijable = new VrstaVarijable(VrstaVarijableEnum.AdresaNaStogu);
			iAdresa = 0;

			if (nTrenutniTipVarijable.Tip!=UgradjeniTipoviEnum.Record)
			{
				PrijaviGresku("Ocekivan record.");
				return;
			}

			foreach(Polje polje in nTrenutniTipVarijable.ListaPolja)
			{
				if (polje.Identifikator==nImePolja)
				{
					iTipVarijable =polje.Tip;

					switch(nTrenutnaVrstaVarijable.VratiVrstuVarijable)
					{
						case VrstaVarijableEnum.AdresaNaStogu:
							iVrstaVarijable = new VrstaVarijable(VrstaVarijableEnum.AdresaNaStogu);
							iAdresa = 0;
							GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Lit,
								KonverterPodataka.SignedIntToBitRepresentation(polje.OdmakOdBazneAdrese));
							GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Add_i);
							break;
						case VrstaVarijableEnum.LokalnaVarijabla:
							iVrstaVarijable = new VrstaVarijable(VrstaVarijableEnum.LokalnaVarijabla, nTrenutnaVrstaVarijable.DubinaGnijezdjenja);
							iAdresa = nTrenutnaAdresa + KonverterPodataka.SignedIntToBitRepresentation(polje.OdmakOdBazneAdrese);
							break;
						case VrstaVarijableEnum.KonstantniIdentifikator:
							PrijaviGresku("Varijabla tipa record ne moze biti konstantna.");
							break;
					}
					//adresa ostaje na vrhu stoga
					return;
				}
			}

			//greska, nije pronadjeno polje tog imena
			PrijaviGresku("Identifikator " + nImePolja + " nije element recorda.");

		}

		public void IzracunajDereferenciranje(TipPodatka nTrenutniTipVarijable, VrstaVarijable nTrenutnaVrstaVarijable,
			UInt32 nTrenutnaAdresa, out TipPodatka iTipVarijable, out VrstaVarijable iVrstaVarijable,
			out UInt32 iAdresa)
		{
			if (nTrenutniTipVarijable.Tip!=UgradjeniTipoviEnum.Pointer)
			{
				iTipVarijable = KonstruktoriTipova.Nil();
				iVrstaVarijable = new VrstaVarijable(VrstaVarijableEnum.AdresaNaStogu);
				iAdresa = 0;

				PrijaviGresku("Ocekivan pokazivac.");
				return;
			}
			

			iTipVarijable = nTrenutniTipVarijable.PodTip;
			iVrstaVarijable = new VrstaVarijable(VrstaVarijableEnum.AdresaNaStogu);
			iAdresa = 0;

			switch(nTrenutnaVrstaVarijable.VratiVrstuVarijable)
			{
				case VrstaVarijableEnum.AdresaNaStogu:
					//postavi adresu dereferencirane varijable na stog
					GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Lti);
					break;
				case VrstaVarijableEnum.LokalnaVarijabla:
					GeneratorMedjukoda.Generiraj(TroadresneNaredbeEnum.Lod,
						KonverterPodataka.SignedIntToBitRepresentation(nTrenutnaVrstaVarijable.DubinaGnijezdjenja),
						nTrenutnaAdresa);
					break;
			}

		}

		public void EvaluirajUnarni(UnarniOperatoriEnum nOperator, TipPodatka nTipDesneStrane, UInt32 nVrijednost,
			bool nKonstantno, out TipPodatka iEvaluiraniTip, out UInt32 iEvalVrijednost, out bool iEvalKonstantno)
		{
			iEvalKonstantno = nKonstantno;
			iEvalVrijednost = nVrijednost;
			iEvaluiraniTip = nTipDesneStrane;
			switch(nOperator)
			{
				case UnarniOperatoriEnum.Minus:
					if (nTipDesneStrane.Tip==UgradjeniTipoviEnum.Integer || nTipDesneStrane.Tip==UgradjeniTipoviEnum.Interval)
					{
						if (nKonstantno)
						{
							int i = KonverterPodataka.BitRepresentationToSignedInt(nVrijednost);
							try {
								iEvalVrijednost = KonverterPodataka.SignedIntToBitRepresentation(-i);
							} catch (OverflowException) {
								PrijaviGresku("Overflow pri obradi unarnog izraza.");
								iEvalVrijednost = KonverterPodataka.SignedIntToBitRepresentation(1);
							}
						}
						else
						{
							GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Neg_i);
						}
						iEvalKonstantno = nKonstantno;
						iEvaluiraniTip = KonstruktoriTipova.Integer();
					}
					else if (nTipDesneStrane.Tip==UgradjeniTipoviEnum.Real)
					{
						if (nKonstantno)
						{
							float f;
							try {
								f = -KonverterPodataka.BitRepresentationToFloat(nVrijednost);
							}
							catch (NotFiniteNumberException) {
								PrijaviGresku("Dogodila se pogreska pri obradi unarnog izraza.");
								f = 1.0F;
							}
							iEvalVrijednost = KonverterPodataka.FloatToBitRepresentation(f);
						}
						else
						{
							GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Neg_r);
						}
						iEvalKonstantno = nKonstantno;
						iEvaluiraniTip = nTipDesneStrane;
					}
					else
					{
						PrijaviGresku("Operator - se koristi na pogresnom tipu.");
					}
					break;
				case UnarniOperatoriEnum.Not:
					if (nTipDesneStrane.Tip==UgradjeniTipoviEnum.Boolean)
					{
						if (nKonstantno)
						{
							bool b = KonverterPodataka.BitRepresentationToBool(nVrijednost);
							iEvalVrijednost = KonverterPodataka.BoolToBitRepresentation(!b);
						}
						else
						{
							GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Not);
						}
						iEvalKonstantno = nKonstantno;
						iEvaluiraniTip = nTipDesneStrane;
					}
					else
					{
						PrijaviGresku("Operator Not se koristi na pogresnom tipu.");
					}
					break;
				case UnarniOperatoriEnum.Plus:
				{
					if (nTipDesneStrane.Tip==UgradjeniTipoviEnum.Integer || nTipDesneStrane.Tip==UgradjeniTipoviEnum.Interval ||
						nTipDesneStrane.Tip==UgradjeniTipoviEnum.Real)
					{
						iEvalKonstantno = nKonstantno;
						iEvalVrijednost = nVrijednost;
						iEvaluiraniTip = nTipDesneStrane;
					}
					else
					{
						PrijaviGresku("Operator + se koristi na pogresnom tipu.");
					}
					break;
				}
			}
		}

		public void Evaluiraj(BinarniOperatoriEnum nOperator, TipPodatka nTipLijevogOperanda,
			TipPodatka nTipDesnogOperanda, UInt32 nVrijedostLijeve, bool nLijevoKonstantno, UInt32 nVrijednostDesne,
			bool nDesnoKonstantno, out TipPodatka iEvaluiraniTip, out UInt32 iUkupnaVrijednost, out bool iSveKonstantnoEval)
		{						
			if ((nTipDesnogOperanda.Tip != nTipLijevogOperanda.Tip) && ((nTipDesnogOperanda.Tip!=UgradjeniTipoviEnum.Integer) && (nTipDesnogOperanda.Tip!=UgradjeniTipoviEnum.Real) && (nTipDesnogOperanda.Tip!=UgradjeniTipoviEnum.Interval)) || ((nTipLijevogOperanda.Tip!=UgradjeniTipoviEnum.Integer) && (nTipLijevogOperanda.Tip != UgradjeniTipoviEnum.Real) && (nTipLijevogOperanda.Tip!=UgradjeniTipoviEnum.Interval)))
			{
				PrijaviGresku("Tipovi podataka nisu kompatibilni!");
				iEvaluiraniTip = KonstruktoriTipova.Nil();
				iUkupnaVrijednost = 1;
				iSveKonstantnoEval = false;
			}
			else
			{
				if(nTipDesnogOperanda.Tip != nTipLijevogOperanda.Tip) 
				{
					if (nTipDesnogOperanda.Tip == UgradjeniTipoviEnum.Real) 
					{
						nVrijedostLijeve = KonverterPodataka.FloatToBitRepresentation( (float) KonverterPodataka.BitRepresentationToSignedInt(nVrijedostLijeve));
						nTipLijevogOperanda = KonstruktoriTipova.Real();
					} 
					else if (nTipLijevogOperanda.Tip == UgradjeniTipoviEnum.Real) 
					{
						nVrijednostDesne = KonverterPodataka.FloatToBitRepresentation( (float) KonverterPodataka.BitRepresentationToSignedInt(nVrijednostDesne));
						nTipDesnogOperanda = KonstruktoriTipova.Real();
					}
					else
					{
						nTipDesnogOperanda = KonstruktoriTipova.Integer();
						nTipLijevogOperanda = KonstruktoriTipova.Integer();
					}					
				}
				if(nTipDesnogOperanda.Tip== UgradjeniTipoviEnum.Interval && nTipLijevogOperanda.Tip == UgradjeniTipoviEnum.Interval)
				{
					iEvaluiraniTip = KonstruktoriTipova.Integer();
				}			
				else iEvaluiraniTip = nTipDesnogOperanda;
				
				iUkupnaVrijednost = KonverterPodataka.SignedIntToBitRepresentation(1);	//ostat ce 1 samo ako dodje do greske
				iSveKonstantnoEval = nLijevoKonstantno && nDesnoKonstantno;
				if (iSveKonstantnoEval == false) return;
				
				switch (nOperator)
				{
					case BinarniOperatoriEnum.Plus:
					{
						if( (nTipDesnogOperanda.Tip != UgradjeniTipoviEnum.Integer) && (nTipDesnogOperanda.Tip != UgradjeniTipoviEnum.Real) && (nTipDesnogOperanda.Tip != UgradjeniTipoviEnum.Interval))
						{
							PrijaviGresku("Ne mogu zbrajati ove tipove podataka!");
							iUkupnaVrijednost=KonverterPodataka.SignedIntToBitRepresentation(1);
						}
						else
						{
							if (nTipLijevogOperanda.Tip==UgradjeniTipoviEnum.Real)
							{
								float f;
								try {
									f = KonverterPodataka.BitRepresentationToFloat(nVrijednostDesne) + KonverterPodataka.BitRepresentationToFloat(nVrijedostLijeve);
								}
								catch (NotFiniteNumberException) {								
									PrijaviGresku("Dogodila se pogreska pri obradi binarnog izraza.");
									f = 1.0F;
								}
								iUkupnaVrijednost = KonverterPodataka.FloatToBitRepresentation(f);
								if (!iSveKonstantnoEval) GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Add_r);
							}
							else 
							{
								int i;
								try {
									i = KonverterPodataka.BitRepresentationToSignedInt(nVrijedostLijeve) + KonverterPodataka.BitRepresentationToSignedInt(nVrijednostDesne);
								}
								catch (OverflowException) {
									PrijaviGresku("Dogodila se pogreska pri obradi binarnog izraza.");
									i = 1;
								}
								iUkupnaVrijednost = KonverterPodataka.SignedIntToBitRepresentation(i);
								if(!iSveKonstantnoEval) GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Add_i);
							}						
						}
						break;
					}
					case BinarniOperatoriEnum.Minus:
					{
						if( (nTipDesnogOperanda.Tip!=JezicniProcesor.UgradjeniTipoviEnum.Integer) && (nTipDesnogOperanda.Tip!=JezicniProcesor.UgradjeniTipoviEnum.Real) && (nTipDesnogOperanda.Tip != UgradjeniTipoviEnum.Interval))
						{
							PrijaviGresku("Ne mogu oduzimati ove tipove podataka!");
							iUkupnaVrijednost=KonverterPodataka.SignedIntToBitRepresentation(1);
						}
						else
						{
							if (nTipLijevogOperanda.Tip == UgradjeniTipoviEnum.Real) 
							{	
								float f;
								try {
									f = KonverterPodataka.BitRepresentationToFloat(nVrijedostLijeve) - KonverterPodataka.BitRepresentationToFloat(nVrijednostDesne);
								}
								catch (NotFiniteNumberException) {
									PrijaviGresku("Dogodila se pogreska pri obradi binarnog izraza.");
									f = 1.0F;
								}
								iUkupnaVrijednost = KonverterPodataka.FloatToBitRepresentation(f);
								if(!iSveKonstantnoEval) GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Sub_r);
							} 
							else 
							{
								int i;
								try {
									i = KonverterPodataka.BitRepresentationToSignedInt(nVrijedostLijeve) - KonverterPodataka.BitRepresentationToSignedInt(nVrijednostDesne);
								}
								catch (OverflowException) {
									PrijaviGresku("Dogodila se pogreska pri obradi binarng izraza.");
									i = 1;
								}
								iUkupnaVrijednost = KonverterPodataka.SignedIntToBitRepresentation(i);
								if (!iSveKonstantnoEval) GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Sub_i);
							}
						}
						break;
					}
					case BinarniOperatoriEnum.Puta:
					{
						if( (nTipDesnogOperanda.Tip!=JezicniProcesor.UgradjeniTipoviEnum.Integer) && (nTipDesnogOperanda.Tip!=JezicniProcesor.UgradjeniTipoviEnum.Real) && (nTipDesnogOperanda.Tip != UgradjeniTipoviEnum.Interval))
						{
							PrijaviGresku("Ne mogu mnoziti ove tipove podataka!");
							iUkupnaVrijednost=KonverterPodataka.SignedIntToBitRepresentation(1);
						}
						else
						{
							if (nTipLijevogOperanda.Tip == UgradjeniTipoviEnum.Real) 
							{
								float f;
								try {
									f = KonverterPodataka.BitRepresentationToFloat(nVrijedostLijeve) * KonverterPodataka.BitRepresentationToFloat(nVrijednostDesne);
								}
								catch (NotFiniteNumberException) {
									PrijaviGresku("Dogodila se pogreska prilikom obrade binarnog izraza.");
									f = 1.0F;
								}
								iUkupnaVrijednost = KonverterPodataka.FloatToBitRepresentation(f);
								if(!iSveKonstantnoEval) GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Mul_r);
							} 
							else 
							{
								int i;
								try {
									i = KonverterPodataka.BitRepresentationToSignedInt(nVrijedostLijeve) * KonverterPodataka.BitRepresentationToSignedInt(nVrijednostDesne);
								}
								catch (OverflowException) {
									PrijaviGresku("Dogodila se pogreska prilikom obrade binarnog izraza.");
									i = 1;
								}
								iUkupnaVrijednost = KonverterPodataka.SignedIntToBitRepresentation(i);
								if (!iSveKonstantnoEval) GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Mul_i);
							}
						}
						break;
					}
					case BinarniOperatoriEnum.Dijeljeno:
					{
						// dijeljeno radi samo nad tipom real
						if( nTipDesnogOperanda.Tip!=UgradjeniTipoviEnum.Real)
						{
							PrijaviGresku("Mogu dijeliti samo realne brojeve");
							iUkupnaVrijednost=KonverterPodataka.SignedIntToBitRepresentation(1);
						}
						else							
						{
							float f;
							try {
								f = KonverterPodataka.BitRepresentationToFloat(nVrijedostLijeve) / KonverterPodataka.BitRepresentationToFloat(nVrijednostDesne);
							} 
							catch (NotFiniteNumberException) {
								PrijaviGresku("Dogodila se pogreska prilikom obrade binarnog izraza.");
								f = 1.0F;
							}
							catch (DivideByZeroException) {
								PrijaviGresku("Dijeljenje sa nulom.");
								f = 1.0F;
							}
							iUkupnaVrijednost = KonverterPodataka.FloatToBitRepresentation(f);
							if(!iSveKonstantnoEval) GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Div_r);
						}
						break;
					}
					case BinarniOperatoriEnum.And:
					{
						if (nTipDesnogOperanda.Tip != UgradjeniTipoviEnum.Boolean)
						{
							PrijaviGresku("And radi samo sa bool tipovima!");
							iUkupnaVrijednost=KonverterPodataka.SignedIntToBitRepresentation(1);
						}
						else
						{
							bool lijeva = KonverterPodataka.BitRepresentationToBool(nVrijedostLijeve);
							bool desna = KonverterPodataka.BitRepresentationToBool(nVrijednostDesne);
							iUkupnaVrijednost = KonverterPodataka.BoolToBitRepresentation(lijeva && desna);
							if(!iSveKonstantnoEval) GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.And);
						}
						break;
					}
						
					case BinarniOperatoriEnum.Jednak:
					{
						if (nTipDesnogOperanda.Tip != UgradjeniTipoviEnum.Boolean && nTipDesnogOperanda.Tip != UgradjeniTipoviEnum.Real && nTipDesnogOperanda.Tip != UgradjeniTipoviEnum.Integer && nTipDesnogOperanda.Tip != UgradjeniTipoviEnum.Interval)
						{
							PrijaviGresku("Ne mogu usporedjivati ove tipove podataka!");
							iUkupnaVrijednost=KonverterPodataka.SignedIntToBitRepresentation(1);
						}
						else
						{
							if (nTipLijevogOperanda.Tip == UgradjeniTipoviEnum.Real)
							{
								float lijeva = KonverterPodataka.BitRepresentationToFloat(nVrijedostLijeve);
								float desna = KonverterPodataka.BitRepresentationToFloat(nVrijednostDesne);
								iUkupnaVrijednost = KonverterPodataka.BoolToBitRepresentation(lijeva == desna);
								if(!iSveKonstantnoEval) GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Eql_r);
							}
							else if(nTipLijevogOperanda.Tip == UgradjeniTipoviEnum.Boolean)
							{
								iUkupnaVrijednost = KonverterPodataka.BoolToBitRepresentation(KonverterPodataka.BitRepresentationToBool( nVrijedostLijeve) == KonverterPodataka.BitRepresentationToBool( nVrijednostDesne));
							}

							else
							{
								int lijeva = KonverterPodataka.BitRepresentationToSignedInt(nVrijedostLijeve);
								int desna = KonverterPodataka.BitRepresentationToSignedInt(nVrijednostDesne);
								iUkupnaVrijednost = KonverterPodataka.BoolToBitRepresentation(lijeva == desna);
								if(!iSveKonstantnoEval) GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Eql_i);
							}
						}
						break;
					}
					case BinarniOperatoriEnum.ManjiIliJednak:
					{
						if (nTipDesnogOperanda.Tip != UgradjeniTipoviEnum.Real && nTipDesnogOperanda.Tip != UgradjeniTipoviEnum.Integer && nTipDesnogOperanda.Tip != UgradjeniTipoviEnum.Interval)
						{
							PrijaviGresku("Ne mogu usporedjivati ove tipove podataka!");
							iUkupnaVrijednost=KonverterPodataka.SignedIntToBitRepresentation(1);
						}
						else
						{
							if (nTipLijevogOperanda.Tip == UgradjeniTipoviEnum.Real) 
							{
								float lijeva = KonverterPodataka.BitRepresentationToFloat(nVrijedostLijeve);
								float desna = KonverterPodataka.BitRepresentationToFloat(nVrijednostDesne);
								iUkupnaVrijednost = KonverterPodataka.BoolToBitRepresentation(lijeva <= desna);
								if(!iSveKonstantnoEval) GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Leq_r);
							} 
							else if (nTipLijevogOperanda.Tip != UgradjeniTipoviEnum.Boolean)
							{
								int lijeva = KonverterPodataka.BitRepresentationToSignedInt(nVrijedostLijeve);
								int desna = KonverterPodataka.BitRepresentationToSignedInt(nVrijednostDesne);
								iUkupnaVrijednost = KonverterPodataka.BoolToBitRepresentation(lijeva <= desna);
								if(!iSveKonstantnoEval) GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Leq_i);
							}
						}
						break;
					}
					case BinarniOperatoriEnum.ManjiOd:
					{
						if (nTipDesnogOperanda.Tip != UgradjeniTipoviEnum.Real && nTipDesnogOperanda.Tip != UgradjeniTipoviEnum.Integer && nTipDesnogOperanda.Tip != UgradjeniTipoviEnum.Interval)
						{
							PrijaviGresku("Ne mogu usporedjivati ove tipove podataka!");
							iUkupnaVrijednost=KonverterPodataka.SignedIntToBitRepresentation(1);
						}
						else
						{
							if (nTipLijevogOperanda.Tip == UgradjeniTipoviEnum.Real) 
							{
								float lijeva = KonverterPodataka.BitRepresentationToFloat(nVrijedostLijeve);
								float desna = KonverterPodataka.BitRepresentationToFloat(nVrijednostDesne);
								iUkupnaVrijednost = KonverterPodataka.BoolToBitRepresentation(lijeva < desna);
								if(!iSveKonstantnoEval ) GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Lss_r);
							} 
							else 
							{
								int lijeva = KonverterPodataka.BitRepresentationToSignedInt(nVrijedostLijeve);
								int desna = KonverterPodataka.BitRepresentationToSignedInt(nVrijednostDesne);
								iUkupnaVrijednost = KonverterPodataka.BoolToBitRepresentation(lijeva < desna);
								if(!iSveKonstantnoEval ) GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Lss_i);
							}
						}
						break;
					}
					case BinarniOperatoriEnum.Mod:
					{
						if(nTipDesnogOperanda.Tip != UgradjeniTipoviEnum.Integer && nTipDesnogOperanda.Tip != UgradjeniTipoviEnum.Interval)
						{
							PrijaviGresku("Operacija mod radi samo sa integer i interval tipovima podataka!");
							iUkupnaVrijednost=KonverterPodataka.SignedIntToBitRepresentation(1);						
						}
						else
						{
							iUkupnaVrijednost = KonverterPodataka.SignedIntToBitRepresentation(KonverterPodataka.BitRepresentationToSignedInt( nVrijedostLijeve) % KonverterPodataka.BitRepresentationToSignedInt( nVrijednostDesne));
							if(!iSveKonstantnoEval) GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Mod);
						}
						break;
					}
					case BinarniOperatoriEnum.NijeJednak:
					{
						if (nTipDesnogOperanda.Tip != UgradjeniTipoviEnum.Real && nTipDesnogOperanda.Tip != UgradjeniTipoviEnum.Integer && nTipDesnogOperanda.Tip != UgradjeniTipoviEnum.Interval && nTipDesnogOperanda.Tip != UgradjeniTipoviEnum.Boolean)
						{
							PrijaviGresku("Ne mogu usporedjivati ove tipove podataka!");
							iUkupnaVrijednost=KonverterPodataka.SignedIntToBitRepresentation(1);						
						}
						else
						{
							if (nTipLijevogOperanda.Tip == UgradjeniTipoviEnum.Real) 
							{
								float lijeva = KonverterPodataka.BitRepresentationToFloat(nVrijedostLijeve);
								float desna = KonverterPodataka.BitRepresentationToFloat(nVrijednostDesne);
								iUkupnaVrijednost = KonverterPodataka.BoolToBitRepresentation(lijeva != desna);
								if(!iSveKonstantnoEval ) GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Neg_r);
							} 
							else if (nTipLijevogOperanda.Tip == UgradjeniTipoviEnum.Boolean)
							{
								iUkupnaVrijednost = KonverterPodataka.BoolToBitRepresentation(KonverterPodataka.BitRepresentationToBool( nVrijedostLijeve) != KonverterPodataka.BitRepresentationToBool( nVrijednostDesne));
							}
							else 
							{
								int lijeva = KonverterPodataka.BitRepresentationToSignedInt(nVrijedostLijeve);
								int desna = KonverterPodataka.BitRepresentationToSignedInt(nVrijednostDesne);
								iUkupnaVrijednost = KonverterPodataka.BoolToBitRepresentation(lijeva != desna);
								if(!iSveKonstantnoEval ) GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Neq_i);

							}
						}
						break;
					}
					case BinarniOperatoriEnum.Or:
					{
						if (nTipDesnogOperanda.Tip != UgradjeniTipoviEnum.Boolean)
						{
							PrijaviGresku("Or radi samo sa bool tipom podataka!");
							iUkupnaVrijednost=KonverterPodataka.SignedIntToBitRepresentation(1);						
						}
						else
						{								
							bool lijeva = KonverterPodataka.BitRepresentationToBool(nVrijedostLijeve);
							bool desna = KonverterPodataka.BitRepresentationToBool(nVrijednostDesne);
							iUkupnaVrijednost = KonverterPodataka.BoolToBitRepresentation(lijeva || desna);
							if(!iSveKonstantnoEval ) GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Or);
						}
						break;
					}
					case BinarniOperatoriEnum.VeciIliJednak:
					{
						if (nTipDesnogOperanda.Tip != UgradjeniTipoviEnum.Integer && nTipDesnogOperanda.Tip != UgradjeniTipoviEnum.Real && nTipDesnogOperanda.Tip != UgradjeniTipoviEnum.Interval)
						{
							PrijaviGresku("Ne mogu usporedjivati ove tipove podataka!");
							iUkupnaVrijednost=KonverterPodataka.SignedIntToBitRepresentation(1);						
						}
						else
						{
							if (nTipLijevogOperanda.Tip == UgradjeniTipoviEnum.Real || nTipLijevogOperanda.Tip==UgradjeniTipoviEnum.Integer) 
							{
								float lijeva = KonverterPodataka.BitRepresentationToFloat(nVrijedostLijeve);
								float desna = KonverterPodataka.BitRepresentationToFloat(nVrijednostDesne);
								iUkupnaVrijednost = KonverterPodataka.BoolToBitRepresentation(lijeva >= desna);
								if(!iSveKonstantnoEval ) GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Geq_r);
							} 
							else 
							{
								int lijeva = KonverterPodataka.BitRepresentationToSignedInt(nVrijedostLijeve);
								int desna = KonverterPodataka.BitRepresentationToSignedInt(nVrijednostDesne);
								iUkupnaVrijednost = KonverterPodataka.BoolToBitRepresentation(lijeva >= desna);
								if(!iSveKonstantnoEval ) GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Geq_i);
							}
						}
						break;
					}
					case BinarniOperatoriEnum.VeciOd:
					{
						if (nTipLijevogOperanda.Tip == UgradjeniTipoviEnum.Real && nTipLijevogOperanda.Tip==UgradjeniTipoviEnum.Integer && nTipLijevogOperanda.Tip != UgradjeniTipoviEnum.Interval) 
						{
							PrijaviGresku("Ne mogu usporedjivati ove tipove podataka!");
							iUkupnaVrijednost=KonverterPodataka.SignedIntToBitRepresentation(1);
						}
						else
						{
							if (nTipLijevogOperanda.Tip == UgradjeniTipoviEnum.Real) 
							{
								float lijeva = KonverterPodataka.BitRepresentationToFloat(nVrijedostLijeve);
								float desna = KonverterPodataka.BitRepresentationToFloat(nVrijednostDesne);
								iUkupnaVrijednost = KonverterPodataka.BoolToBitRepresentation(lijeva > desna);
								if(!iSveKonstantnoEval ) GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Gtr_r);
							} 
							else 
							{
								int lijeva = KonverterPodataka.BitRepresentationToSignedInt(nVrijedostLijeve);
								int desna = KonverterPodataka.BitRepresentationToSignedInt(nVrijednostDesne);
								iUkupnaVrijednost = KonverterPodataka.BoolToBitRepresentation(lijeva > desna);
								if(!iSveKonstantnoEval ) GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Gtr_i);
							}
						}
						break;
					}
					case BinarniOperatoriEnum.Div:
					{
						if((nTipLijevogOperanda.Tip != UgradjeniTipoviEnum.Integer) && (nTipLijevogOperanda.Tip!= UgradjeniTipoviEnum.Interval))
						{
							PrijaviGresku("Div radi samo sa integer i interval tipovima podataka!");
							iUkupnaVrijednost=KonverterPodataka.SignedIntToBitRepresentation(1);						
						}
						else
						{
							int i;
							try {
								i = KonverterPodataka.BitRepresentationToSignedInt(nVrijedostLijeve) / KonverterPodataka.BitRepresentationToSignedInt(nVrijednostDesne);
							}
							catch (OverflowException) {
								PrijaviGresku("Dogodila se pogreska prilikom obrade binarnog izraza.");
								i = 1;
							}
							catch (DivideByZeroException) {
								PrijaviGresku("Dijeljenje sa nulom.");
								i = 1;
							}
							iUkupnaVrijednost = KonverterPodataka.SignedIntToBitRepresentation(i);
							if(!iSveKonstantnoEval ) GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Div_i);
						}
						break;
					}
					default:
					{
						iUkupnaVrijednost = KonverterPodataka.SignedIntToBitRepresentation(1);
						break;
					}
				}				
			}
		}
		
		public void ObradiIfNaredbu(TipPodatka nTipIzraza, UInt32 nVrijednostIzraza, bool nSveKonstantno,
			out int iOznakaNaredbe)
		{
			iOznakaNaredbe = GeneratorJedinstvenihBrojeva.Generiraj();
			UvjetnaNaredba naredba = new UvjetnaNaredba(nSveKonstantno, KonverterPodataka.BitRepresentationToBool(nVrijednostIzraza));
			_naredbe.Add(iOznakaNaredbe, naredba);
			if (nTipIzraza.Tip!=UgradjeniTipoviEnum.Boolean)
			{
				PrijaviGresku("Uvjet if naredbe mora biti tipa boolean.");
				return;
			}

			if (!naredba.UvjetKonstantan)
			{
				GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Jpn, GeneratorLabela.KrajIfNaredbe(iOznakaNaredbe));
			}
			else if (!naredba.UvjetIspunjen)
			{
				{
					GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Jmp, GeneratorLabela.KrajIfNaredbe(iOznakaNaredbe));
				}
			}
		}
		
		public void ZavrsiIfNaredbu(int nOznakaIfNaredbe)
		{
			GeneratorMedjukoda.PostaviAdresnuLabelu(GeneratorLabela.KrajIfNaredbe(nOznakaIfNaredbe));
		}
		
		public void ObradiElseNaredbu(int nOznakaIfNaredbe)
		{
			UvjetnaNaredba naredba = (UvjetnaNaredba)_naredbe[nOznakaIfNaredbe];

			GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Jmp, GeneratorLabela.KrajElseNaredbe(nOznakaIfNaredbe));
			GeneratorMedjukoda.PostaviAdresnuLabelu(GeneratorLabela.KrajIfNaredbe(nOznakaIfNaredbe));
		}

		public void ZavrsiElseNaredbu(int nOznakaIfNaredbe)
		{
			UvjetnaNaredba naredba = (UvjetnaNaredba)_naredbe[nOznakaIfNaredbe];

			GeneratorMedjukoda.PostaviAdresnuLabelu(GeneratorLabela.KrajElseNaredbe(nOznakaIfNaredbe));
		}

		public void Goto(UInt32 nVrijednostLabele)
		{
			Labela labela = TablicaZnakova.PronadjiLabelu(nVrijednostLabele);
			if (labela!=null)
			{
				GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Jmp, GeneratorLabela.GotoLabela(labela));
			}
			else
			{
				PrijaviGresku("Nedeklarirana labela.");
			}
		}
		
		public void PostaviLabelu(UInt32 nVrijednostLabele)
		{
			Labela labela = TablicaZnakova.PronadjiLabelu(nVrijednostLabele);
			if (labela!=null)
			{
				GeneratorMedjukoda.PostaviAdresnuLabelu(GeneratorLabela.GotoLabela(labela));
			}
			else
			{
				PrijaviGresku("Nedeklarirana labela.");
			}
		}
		
		public void ZapocniWhileNaredbu(out int iOznakaWhileNaredbe)
		{
			iOznakaWhileNaredbe = GeneratorJedinstvenihBrojeva.Generiraj();
			GeneratorMedjukoda.PostaviAdresnuLabelu(GeneratorLabela.PocetakWhileNaredbe(iOznakaWhileNaredbe));
		}
		
		public void ProvjeriUvjetWhileNaredbe(TipPodatka nTipIzraza, UInt32 nVrijednostIzraza, bool nSveKonstantno, int nOznakaWhileNaredbe)
		{
			UvjetnaNaredba naredba = new UvjetnaNaredba(nSveKonstantno, KonverterPodataka.BitRepresentationToBool(nVrijednostIzraza));
			_naredbe.Add(nOznakaWhileNaredbe, naredba);

			if (naredba.UvjetKonstantan)
			{
				if (!naredba.UvjetIspunjen)
				{
					GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Jmp, GeneratorLabela.KrajWhileNaredbe(nOznakaWhileNaredbe));
				}
			}
			else
			{
				GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Jpn, GeneratorLabela.KrajWhileNaredbe(nOznakaWhileNaredbe));
			}
		}
		
		public void ZavrsiWhileNaredbu(int nOznakaWhileNaredbe)
		{
			GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Jmp, GeneratorLabela.PocetakWhileNaredbe(nOznakaWhileNaredbe));
			GeneratorMedjukoda.PostaviAdresnuLabelu(GeneratorLabela.KrajWhileNaredbe(nOznakaWhileNaredbe));
		}
		
		public void ZapocniRepeatNaredbu(out int iOznakaRepeatNaredbe)
		{
			iOznakaRepeatNaredbe = GeneratorJedinstvenihBrojeva.Generiraj();
			GeneratorMedjukoda.PostaviAdresnuLabelu(GeneratorLabela.PocetakRepeatNaredbe(iOznakaRepeatNaredbe));
		}
		
		public void ZavrsiRepeatNaredbu(TipPodatka nTipIzraza, UInt32 nVrijednostIzraza, bool nSveKonstantno, int nOznakaRepeatNaredbe)
		{
			if (nSveKonstantno && KonverterPodataka.BitRepresentationToBool(nVrijednostIzraza))
			{
				GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Jmp, GeneratorLabela.PocetakRepeatNaredbe(nOznakaRepeatNaredbe));
			}
			else
			{
				GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Jpc, GeneratorLabela.PocetakRepeatNaredbe(nOznakaRepeatNaredbe));
			}
		}
		
		
		public void StvoriListuOdabira(TipPodatka nZahtjevaniTip, TipPodatka nTipKonstante, UInt32 nVrijednostKonstante, out ICollection iNovaLista)
		{
			ArrayList tmpLista = new ArrayList();

			if ((nZahtjevaniTip.Cjelobrojni && nTipKonstante.Cjelobrojni) ||
				(TipPodatka.TipoviJednaki(nZahtjevaniTip, nTipKonstante)))
			{
				tmpLista.Add(nVrijednostKonstante);
			}
			else
			{
				PrijaviGresku("Tip konstante ne odgovara tipu izraza u case naredbi.");
			}
			iNovaLista = tmpLista;
		}

		public void NadopuniListuOdabira(TipPodatka nZahtjevaniTip, TipPodatka nTipKonstante, UInt32 nVrijednostKonstante, ICollection nDosadasnjaListaOdabira, out ICollection iDopunjenaListaOdabira)
		{
			ArrayList tmpLista = new ArrayList(nDosadasnjaListaOdabira);

			if ((nZahtjevaniTip.Cjelobrojni && nTipKonstante.Cjelobrojni) ||
				(TipPodatka.TipoviJednaki(nZahtjevaniTip, nTipKonstante)))
			{
				tmpLista.Add(nVrijednostKonstante);
			}
			else
			{
				PrijaviGresku("Tip konstante ne odgovara tipu izraza u case naredbi.");
			}

			iDopunjenaListaOdabira = tmpLista;
		}
		
		/// <summary>
		/// Ova akcija obradjuje indekse kod pristupanja elementima niza. Radi na nacin da na stogu uvijek ostavlja odmak od 
		/// pocetka niza koji stvaraju do sada obradjeni indeksi
		/// </summary>
		public void ProvjeriIZapisiSubscript(TipPodatka nTipNiza, TipPodatka nTipIzraza, UInt32 nVrijednostIzraza,
			bool nSveKonstantno, int nRedniBrojSubscripta, out int iRedniBrojSubscripta)
		{
			iRedniBrojSubscripta = nRedniBrojSubscripta + 1;
	
			if (nTipNiza.Tip!=UgradjeniTipoviEnum.Array)
			{
				PrijaviGresku("Ocekivan tip niz (array).");
				return;
			}

			if (nRedniBrojSubscripta >= nTipNiza.ListaIndeksa.Count)
			{
				PrijaviGresku("Prevelik broj indeksa kod dohvacanja elementa niza.");
				return;
			}

			int mnozitelj=1;
			int i=nRedniBrojSubscripta;

			IEnumerator enumerator = nTipNiza.ListaIndeksa.GetEnumerator();
			while (i>=0)
			{
				enumerator.MoveNext();
				i--;
			}

			TipPodatka trazeniIndeks = (TipPodatka)enumerator.Current;

			while (enumerator.MoveNext())
			{
				TipPodatka indeks = (TipPodatka)enumerator.Current;
				mnozitelj *= indeks.Kardinalitet;
			}

			if (trazeniIndeks.Cjelobrojni)
			{
				if (!nTipIzraza.Cjelobrojni)
				{
					PrijaviGresku("Ocekivan cjelobrojni indeks.");
					return;
				}
			}
			else if (!TipPodatka.TipoviJednaki(trazeniIndeks, nTipIzraza))
			{
				PrijaviGresku("Pogresan tip indeksa.");
				return;
			}


			if (nSveKonstantno)
			{
				int indeks = KonverterPodataka.BitRepresentationToSignedInt(nVrijednostIzraza);
				int odmak = (indeks - trazeniIndeks.DonjaGranica)*mnozitelj;

				GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Lit, 
					KonverterPodataka.SignedIntToBitRepresentation(odmak));
			}
			else
			{
				GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Lit,
					KonverterPodataka.SignedIntToBitRepresentation(trazeniIndeks.DonjaGranica));
				GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Sub_i);

				GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Lit, 
					KonverterPodataka.SignedIntToBitRepresentation(mnozitelj));

				GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Mul_i);
			}

			//dodaj novi odmak do sada izracunatom odmaku
			if (nRedniBrojSubscripta!=0)
			{
				GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Add_i);
			}
		}
		
		public void ProvjeriIZapisiParametar(string nImePotPrograma, TipPodatka nTipIzraza, UInt32 nVrijednostIzraza,
			bool nSveKonstantno, int nRedniBrojIzraza, out int iRedniBrojIzraza)
		{
			iRedniBrojIzraza = nRedniBrojIzraza + 1;

			OpisnikPotprograma opisnik;
			int razina;
			TablicaZnakova.PronadjiPotprogram(nImePotPrograma, out opisnik, out razina);

			if (opisnik==null)
			{
				return;
			}

			if (nRedniBrojIzraza >= opisnik.Parametri.Count)
			{
				PrijaviGresku("Prevelik broj parametara za potprogram " + nImePotPrograma + ".");
				return;
			}

			int i=nRedniBrojIzraza;

			IEnumerator enumerator = opisnik.Parametri.GetEnumerator();
			while (i>=0)
			{
				enumerator.MoveNext();
				i--;
			}
			
			Polje trazeniParametar = (Polje)enumerator.Current;

			if (trazeniParametar.Tip.Tip==UgradjeniTipoviEnum.Integer)
			{
				//dopustamo da se posalje integer i interval tip
				if (!nTipIzraza.Cjelobrojni)
				{
					PrijaviGresku("Ocekivan cjelobrojni tip.");
					return;
				}
			} 
			else if (!TipPodatka.TipoviJednaki(trazeniParametar.Tip, nTipIzraza))
			{
				PrijaviGresku("Tip parametra se ne poklapa s tipom formalnog parametra u deklaraciji potprograma.");
				return;
			}
			
			//sve ok, stavi parametar na stog ako je konstantan, inace je vec na stogu
			if (nSveKonstantno)
			{
				GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Lit,
					nVrijednostIzraza);
			}
		}
		

		public void ProceduraDeklarirana(string nIdentifikator, ICollection nListaParametara)
		{
			OpisnikPotprograma opisnik;
			int razina;

			TablicaZnakova.PronadjiPotprogram(nIdentifikator, TipPotprogramaEnum.Procedura, out opisnik, out razina);

			if (opisnik!=null && razina==1)
			{
				//procedura je vec deklarirana u trenutnom scopeu, mozda se prije radilo o forward deklaraciji
				//a sad i implementaciji pa propusti ako imaju iste parametre
				if (nListaParametara.Count!=opisnik.Parametri.Count)
				{
					PrijaviGresku("Procedura " + nIdentifikator + " vec deklarirana s drukcijim parametrima.");
				}
				else
				{
					IEnumerator enum1 = opisnik.Parametri.GetEnumerator();
					IEnumerator enum2 = nListaParametara.GetEnumerator();

					while (enum1.MoveNext())
					{
						enum2.MoveNext();
						
						Polje parametar1 = (Polje)enum1.Current;
						Polje parametar2 = (Polje)enum2.Current;

						if (parametar1.Identifikator!=parametar2.Identifikator ||
							!TipPodatka.TipoviJednaki(parametar1.Tip, parametar2.Tip))
						{
							PrijaviGresku("Procedura " + nIdentifikator + " vec deklarirana s drukcijim parametrima.");
							return;
						}
					}
				}
			}
			else
			{
				if (TablicaZnakova.PostojiIdentifikator(nIdentifikator, true))
				{
					PrijaviGresku("Identifikator " + nIdentifikator + " je vec deklariran.");
				}
				else
				{
					OpisnikPotprograma noviOpisnik = new OpisnikPotprograma(nIdentifikator, TipPotprogramaEnum.Procedura,
						null, nListaParametara);
					TablicaZnakova.DodajOpisnikPotprograma(noviOpisnik);

					//projveri tipove parametara
					foreach(Polje parametar in noviOpisnik.Parametri)
					{
						if (!parametar.Tip.Jednostavni)
						{
							PrijaviGresku("Samo jednostavni tipovi mogu biti parametri potprograma");
						}
						else
						{
							TablicaZnakova.DodajLokalnuVarijablu(parametar);
						}
					}
				}
			}
		}
		
		public void FunkcijaDeklarirana(string nIdentifikator, ICollection nListaParametara, TipPodatka nTip)
		{
			OpisnikPotprograma opisnik;
			int razina;

			TablicaZnakova.PronadjiPotprogram(nIdentifikator, TipPotprogramaEnum.Funkcija, out opisnik, out razina);

			if (opisnik!=null && razina==1)
			{
				//funkcija je vec deklarirana u trenutnom scopeu, mozda se prije radilo o forward deklaraciji
				//a sad i implementaciji pa propusti ako imaju iste parametre
				if (!TipPodatka.TipoviJednaki(nTip, opisnik.TipPovratneVrijednosti))
				{
					PrijaviGresku("Funkcija " + nIdentifikator + " vec deklarirana s drugim povratnim tipom.");
				}
				else if (nListaParametara.Count!=opisnik.Parametri.Count)
				{
					PrijaviGresku("Funkcija " + nIdentifikator + " vec deklarirana s drukcijim parametrima.");
				}
				else
				{
					IEnumerator enum1 = opisnik.Parametri.GetEnumerator();
					IEnumerator enum2 = nListaParametara.GetEnumerator();

					while (enum1.MoveNext())
					{
						enum2.MoveNext();
						
						Polje parametar1 = (Polje)enum1.Current;
						Polje parametar2 = (Polje)enum2.Current;

						if (parametar1.Identifikator!=parametar2.Identifikator ||
							!TipPodatka.TipoviJednaki(parametar1.Tip, parametar2.Tip))
						{
							PrijaviGresku("Funkcija " + nIdentifikator + " vec deklarirana s drukcijim parametrima.");
							return;
						}
					}
				}
			}
			else
			{
				if (TablicaZnakova.PostojiIdentifikator(nIdentifikator, true))
				{
					PrijaviGresku("Identifikator " + nIdentifikator + " je vec deklariran.");
				}
				else
				{
					OpisnikPotprograma noviOpisnik = new OpisnikPotprograma(nIdentifikator, TipPotprogramaEnum.Funkcija,
						null, nListaParametara);
					TablicaZnakova.DodajOpisnikPotprograma(noviOpisnik);

					//projveri tipove parametara
					foreach(Polje parametar in noviOpisnik.Parametri)
					{
						if (!parametar.Tip.Jednostavni)
						{
							PrijaviGresku("Samo jednostavni tipovi mogu biti parametri potprograma");
						}
					}

					if (!nTip.Jednostavni)
					{
						PrijaviGresku("Samo jednostavni tipovi mogu biti povratna vrijednost funkcije.");
					}
				}
			}
		}
		
		public void OznaciImplementaciju(string nIdentifikator, bool nImplementirano)
		{
			OpisnikPotprograma opisnik;
			int razina;

			TablicaZnakova.PronadjiPotprogram(nIdentifikator, out opisnik, out razina);
			if (opisnik==null)
			{
				PrijaviGresku("Potpgoram " + nIdentifikator + " neispravno deklariran.");
			}
			else
			{
				if (nImplementirano)
				{
					if (opisnik.Implementiran)
					{
						PrijaviGresku("Potprogram " + nIdentifikator + " je vec implementiran.");
					}
					else
					{
						opisnik.OznaciImplementaciju();
						opisnik.OznaciDeklaraciju();
					}
				}
				else
				{
					if (opisnik.Deklariran)
					{
						PrijaviGresku("Potprogram " + nIdentifikator + " je vec deklariran.");
					}
					else
					{
						opisnik.OznaciDeklaraciju();
					}
				}
			}
		}

		//===================================================================================================//
		//===================================================================================================//

		public void InicijalizirajForPetlju(string nImeIdentifikatora, TipPodatka nTipIzraza,
			UInt32 nVrijednostIzraza, bool nSveKonstantno, out int iOznakaForPetlje)
		{
			iOznakaForPetlje = GeneratorJedinstvenihBrojeva.Generiraj();
			
			Polje varijabla;
			int razina;
			TablicaZnakova.PronadjiVarijablu(nImeIdentifikatora, out varijabla, out razina);

			if (varijabla==null)
			{
				PrijaviGresku("Ne postoji varijabla " + nImeIdentifikatora + ".");
				return;
			}

			if (!varijabla.Tip.Diskretni)
			{
				PrijaviGresku("For varijabla mora biti diskretni tip.");
				return;
			}

			if (!TipPodatka.TipoviJednaki(varijabla.Tip, nTipIzraza))
			{
				PrijaviGresku("Neispravan tip izraza u inicijalizaciji for naredbe.");
				return;
			}

			UInt32 adresaGranice = KonverterPodataka.SignedIntToBitRepresentation(TrenutniKontekst.VelicinaRezerviranogStoga);
			TrenutniKontekst.VelicinaStogaZaLokalneVarijable += varijabla.Tip.VelicinaUBajtovima;

			ForNaredba naredba = new ForNaredba(KonverterPodataka.SignedIntToBitRepresentation(varijabla.OdmakOdBazneAdrese),
				razina, varijabla.Tip, adresaGranice);

			_naredbe[iOznakaForPetlje] = naredba;

			//inicijaliziraj for varijablu
			if (nSveKonstantno)
			{
				GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Lit,
					nVrijednostIzraza);
			}
			if (naredba.TipForVarijable.VelicinaUBajtovima==4)
			{
				GeneratorMedjukoda.Generiraj(TroadresneNaredbeEnum.Sto,
					KonverterPodataka.SignedIntToBitRepresentation(naredba.RazinaGnijezdjenjaForVarijable),
					naredba.AdresaForVarijable);
			}
			else //velicina je 1
			{
				GeneratorMedjukoda.Generiraj(TroadresneNaredbeEnum.Sto_b,
					KonverterPodataka.SignedIntToBitRepresentation(naredba.RazinaGnijezdjenjaForVarijable),
					naredba.AdresaForVarijable);
			}
		}
		

		public void OznaciGranicuForPetljeIProvjeri(string nImeIdentifikatora, TipPodatka nTipIzraza,
			UInt32 nVrijednostIzraza, bool nSveKonstatno, int nOznakaForPetlje, bool nRastucaPetlja)
		{
			if (_naredbe[nOznakaForPetlje]==null)
			{
				return;
			}
			ForNaredba naredba = (ForNaredba)_naredbe[nOznakaForPetlje];
			
			//snimi granicu na za to predvidjeno mjesto
			if (nSveKonstatno)
			{
				GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Lit, nVrijednostIzraza);
			}
			if (naredba.TipForVarijable.VelicinaUBajtovima==4)
			{
				GeneratorMedjukoda.Generiraj(TroadresneNaredbeEnum.Sto, 0,
					naredba.AdresaGranice);
			}
			else //velicina je 1
			{
				GeneratorMedjukoda.Generiraj(TroadresneNaredbeEnum.Sto_b, 0,
					naredba.AdresaGranice);
			}

			//oznaci granicu for petlje
			GeneratorMedjukoda.PostaviAdresnuLabelu(GeneratorLabela.PocetakForNaredbe(nOznakaForPetlje));

			//provjeri treba li prekinuti petlju
			if (naredba.TipForVarijable.VelicinaUBajtovima==4)
			{
				GeneratorMedjukoda.Generiraj(TroadresneNaredbeEnum.Lod,
					KonverterPodataka.SignedIntToBitRepresentation(naredba.RazinaGnijezdjenjaForVarijable),
					naredba.AdresaForVarijable);

				GeneratorMedjukoda.Generiraj(TroadresneNaredbeEnum.Lod, 0,
					naredba.AdresaGranice);
			}
			else
			{
				GeneratorMedjukoda.Generiraj(TroadresneNaredbeEnum.Lod_b,
					KonverterPodataka.SignedIntToBitRepresentation(naredba.RazinaGnijezdjenjaForVarijable),
					naredba.AdresaForVarijable);


				GeneratorMedjukoda.Generiraj(TroadresneNaredbeEnum.Lod_b, 0,
					naredba.AdresaGranice);
			}

			if (nRastucaPetlja)
			{

				GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Gtr_i);
			}
			else
			{
				GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Lss_i);
			}

			GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Jpc,
				GeneratorLabela.KrajForNaredbe(nOznakaForPetlje));

		}

		public void ZavrsiForNaredbu(int nOznakaForNaredbe, bool nRastucaForPetlja)
		{
			if (_naredbe[nOznakaForNaredbe]==null)
			{
				return;
			}
			ForNaredba naredba = (ForNaredba)_naredbe[nOznakaForNaredbe];

			//uvecaj ili smanji varijablu
			if (naredba.TipForVarijable.VelicinaUBajtovima==4)
			{
				GeneratorMedjukoda.Generiraj(TroadresneNaredbeEnum.Lod,
					KonverterPodataka.SignedIntToBitRepresentation(naredba.RazinaGnijezdjenjaForVarijable),
					naredba.AdresaForVarijable);
			}
			else
			{
				GeneratorMedjukoda.Generiraj(TroadresneNaredbeEnum.Lod_b,
					KonverterPodataka.SignedIntToBitRepresentation(naredba.RazinaGnijezdjenjaForVarijable),
					naredba.AdresaForVarijable);
			}

			if (nRastucaForPetlja)
			{
				GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Inc);
			}
			else
			{
				GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Dec);
			}

			if (naredba.TipForVarijable.VelicinaUBajtovima==4)
			{
				GeneratorMedjukoda.Generiraj(TroadresneNaredbeEnum.Sto,
					KonverterPodataka.SignedIntToBitRepresentation(naredba.RazinaGnijezdjenjaForVarijable),
					naredba.AdresaForVarijable);
			}
			else
			{
				GeneratorMedjukoda.Generiraj(TroadresneNaredbeEnum.Sto_b,
					KonverterPodataka.SignedIntToBitRepresentation(naredba.RazinaGnijezdjenjaForVarijable),
					naredba.AdresaForVarijable);
			}


			//skoci na pocetak for naredbe
			GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Jmp,
				GeneratorLabela.PocetakForNaredbe(nOznakaForNaredbe));

			//oznaci kraj for naredbe
			GeneratorMedjukoda.PostaviAdresnuLabelu(GeneratorLabela.KrajForNaredbe(nOznakaForNaredbe));
		}

		public void ZapocniCaseNaredbu(TipPodatka nTipIzraza, out int iOznakaCaseNaredbe)
		{
			iOznakaCaseNaredbe = GeneratorJedinstvenihBrojeva.Generiraj();

			if (!nTipIzraza.Diskretni)
			{
				PrijaviGresku("Izraz koji se evaluira u for naredbi mora biti diskretnog tipa.");
				return;
			}
			GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Jmp,
				GeneratorLabela.CaseSelekcija(iOznakaCaseNaredbe));
		}
		
		public void ZavrsiCaseNaredbu(ICollection nListaSlucajeva, int nOznakaCaseNaredbe,
			TipPodatka nTipIzraza, UInt32 nVrijednostIzraza, bool nSveKonstantno)
		{
			GeneratorMedjukoda.PostaviAdresnuLabelu(GeneratorLabela.CaseSelekcija(nOznakaCaseNaredbe));

			//generiraj kod za selekciju
			if (nSveKonstantno)
			{
				bool stani = false;
				foreach(Slucaj slucaj in nListaSlucajeva)
				{
					foreach(UInt32 vrijednost in slucaj.ListaOdabira)
					{
						if (vrijednost==nVrijednostIzraza)
						{
							GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Jmp,
								GeneratorLabela.CasePodNaredba(slucaj.OznakaCasePodNaredbe));
							stani = true;							
						}
						if (stani) break;
					}
					if (stani) break;
				}							
			}
			else
			{
				//spremi vrijednost izraza kao lokalnu varijablu
				UInt32 adresaIzraza = KonverterPodataka.SignedIntToBitRepresentation(TrenutniKontekst.VelicinaStogaZaLokalneVarijable);
				TrenutniKontekst.VelicinaStogaZaLokalneVarijable += nTipIzraza.VelicinaUBajtovima;

				if (nTipIzraza.VelicinaUBajtovima==4)
				{
					GeneratorMedjukoda.Generiraj(TroadresneNaredbeEnum.Sto, 0, adresaIzraza);
				}
				else
				{
					GeneratorMedjukoda.Generiraj(TroadresneNaredbeEnum.Sto_b, 0, adresaIzraza);
				}

				//generiraj kod za obavljanje skoka
				foreach(Slucaj slucaj in nListaSlucajeva)
				{
					foreach(UInt32 vrijednost in slucaj.ListaOdabira)
					{
						if (nTipIzraza.VelicinaUBajtovima==4)
						{
							GeneratorMedjukoda.Generiraj(TroadresneNaredbeEnum.Lod, 0, adresaIzraza);
						}
						else
						{
							GeneratorMedjukoda.Generiraj(TroadresneNaredbeEnum.Lod_b, 0, adresaIzraza);
						}
						GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Lit, vrijednost);
						GeneratorMedjukoda.Generiraj(JednoadresneNaredbeEnum.Eql_i);
						GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Jpc, 
							GeneratorLabela.CasePodNaredba(slucaj.OznakaCasePodNaredbe));
					}
				}
			}


			GeneratorMedjukoda.PostaviAdresnuLabelu(GeneratorLabela.KrajCaseNaredbe(nOznakaCaseNaredbe));
		}

		public void ZapocniCasePodNaredbu(int nOznakaCaseNaredbe, out int iOznakaCasePodNaredbe)
		{
			iOznakaCasePodNaredbe = GeneratorJedinstvenihBrojeva.Generiraj();
			GeneratorMedjukoda.PostaviAdresnuLabelu(GeneratorLabela.CasePodNaredba(iOznakaCasePodNaredbe));
		}
		
		public void ZavrsiCasePodNaredbu(int nOznakaCaseNaredbe)
		{
			GeneratorMedjukoda.Generiraj(DvoadresneNaredbeEnum.Jmp, 
				GeneratorLabela.KrajCaseNaredbe(nOznakaCaseNaredbe));
		}
				
		public void StvoriListuSlucajeva(ICollection nListaMogucihOdabira, int nOznakaCasePodNaredbe,
			out ICollection iNovaLista)
		{
			ArrayList tmpLista = new ArrayList();

			tmpLista.Add(new Slucaj(nListaMogucihOdabira, nOznakaCasePodNaredbe));
			iNovaLista = tmpLista;
		}
		
		public void NadopuniListuSlucajeva(ICollection nListaMogucihOdabira, int nOznakaCasePodNaredbe,
			ICollection nDosadasnjaListaSlucajeva, out ICollection iDopunjenaListaSlucajeva)
		{
			ArrayList tmpLista = new ArrayList(nDosadasnjaListaSlucajeva);

			tmpLista.Add(new Slucaj(nListaMogucihOdabira, nOznakaCasePodNaredbe));
			iDopunjenaListaSlucajeva = tmpLista;
		}
		
		//===================================================================================================//
		//===================================================================================================//

		public OpisnikPotprograma TrenutniKontekst
		{
			get 
			{
				if (_kontekst.Count > 0)
				{
					return (OpisnikPotprograma)_kontekst.Peek();
				}
				else
				{
					return null;
				}
			}
		}

		private const int _maksimalnaVelicinaVarijable = 666;
		private Stack _kontekst;
		private Hashtable _naredbe;
	}
}
