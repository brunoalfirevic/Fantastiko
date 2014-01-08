using System;
using System.Collections;

namespace JezicniProcesor
{

	/// <summary>
	/// Ova klasa predstavlja simulator DKA koji se koristi pri leksickoj analizi
	/// U njoj implementiran algoritam u knjizi JP2 na stranici 58 (2.9.6)
	/// </summary>
	class DKASimulator
	{
		public DKASimulator()
		{
			#region Punjenje _tabliceLeksickihJedinki
			_tablicaZnakova.DodajKROS("+");
			_tablicaZnakova.DodajKROS("-");
			_tablicaZnakova.DodajKROS("*");
			_tablicaZnakova.DodajKROS("/");
			_tablicaZnakova.DodajKROS("=");
			_tablicaZnakova.DodajKROS("<");
			_tablicaZnakova.DodajKROS(">");
			_tablicaZnakova.DodajKROS("[");
			_tablicaZnakova.DodajKROS("]");
			_tablicaZnakova.DodajKROS(".");
			_tablicaZnakova.DodajKROS(",");
			_tablicaZnakova.DodajKROS(":=");
			_tablicaZnakova.DodajKROS(":");
			_tablicaZnakova.DodajKROS(";");
			_tablicaZnakova.DodajKROS("(");
			_tablicaZnakova.DodajKROS(")");
			_tablicaZnakova.DodajKROS("<>");
			_tablicaZnakova.DodajKROS("<=");
			_tablicaZnakova.DodajKROS(">=");
			_tablicaZnakova.DodajKROS("..");
			_tablicaZnakova.DodajKROS("^");
			//reserved words
			_tablicaZnakova.DodajKROS(bool.TrueString);
			_tablicaZnakova.DodajKROS(bool.FalseString);
			_tablicaZnakova.DodajKROS("and");
			_tablicaZnakova.DodajKROS("array");
			_tablicaZnakova.DodajKROS("begin");
			_tablicaZnakova.DodajKROS("case");
			_tablicaZnakova.DodajKROS("const");
			_tablicaZnakova.DodajKROS("div");
			_tablicaZnakova.DodajKROS("do");
			_tablicaZnakova.DodajKROS("downto");
			_tablicaZnakova.DodajKROS("else");
			_tablicaZnakova.DodajKROS("end");
			_tablicaZnakova.DodajKROS("file");
			_tablicaZnakova.DodajKROS("for");
			_tablicaZnakova.DodajKROS("forward");
			_tablicaZnakova.DodajKROS("function");
			_tablicaZnakova.DodajKROS("goto");
			_tablicaZnakova.DodajKROS("if");
			_tablicaZnakova.DodajKROS("label");
			_tablicaZnakova.DodajKROS("main");
			_tablicaZnakova.DodajKROS("mod");
			_tablicaZnakova.DodajKROS("nil");
			_tablicaZnakova.DodajKROS("not");
			_tablicaZnakova.DodajKROS("of");
			_tablicaZnakova.DodajKROS("or");
			_tablicaZnakova.DodajKROS("procedure");
			_tablicaZnakova.DodajKROS("program");
			_tablicaZnakova.DodajKROS("record");
			_tablicaZnakova.DodajKROS("repeat");
			_tablicaZnakova.DodajKROS("then");
			_tablicaZnakova.DodajKROS("to");
			_tablicaZnakova.DodajKROS("type");
			_tablicaZnakova.DodajKROS("until");
			_tablicaZnakova.DodajKROS("var");
			_tablicaZnakova.DodajKROS("while");
			
			#endregion		
		}

		public void Simuliraj(string ulazniProgram, ArrayList listaGresaka)
		{
			//D1
			DKATablica dkaTablica = new DKATablica();

			//D2
			_ulazniProgram = ulazniProgram;
			_trenutnaLinija= 1; _krajReda=false;
			
			//D3
			//Sve varijable poprimaju pocetnu vrijednost manju nego je to opisano u knjizi
			//jer se u knjizi podrazumijeva da prvi znak u stringu ima index 1
			int pocetak = 0;
			int zavrsetak = -1;
			int posljednji = 0;
			string izraz = null;
			char a;

			DKAStanje trenutnoStanje = new DKAStanje(false,false,"Pocetno");

			while (zavrsetak!=ulazniProgram.Length-1)
			{
				//D4
				
				zavrsetak = zavrsetak + 1;
				a = Citaj(zavrsetak);
				trenutnoStanje = dkaTablica.DajPrijelaz(trenutnoStanje, a);
				while (trenutnoStanje.NullStanje==false) 
				{
					if (!trenutnoStanje.Prihvatljivo)
					{
						zavrsetak = zavrsetak + 1;
						a = Citaj(zavrsetak);
						trenutnoStanje = dkaTablica.DajPrijelaz(trenutnoStanje, a);
					} 
					else if (trenutnoStanje.Prihvatljivo)
					{
						//u izraz zapisujemo zadnje prihvatljivo stanje
						izraz=trenutnoStanje.KodStanja;
						posljednji=zavrsetak;
						zavrsetak = zavrsetak + 1;
						a = Citaj(zavrsetak);				
						trenutnoStanje = dkaTablica.DajPrijelaz(trenutnoStanje, a);
					}
				}
				//doslo je do nullStanja

				//D5
				if (izraz==null)
				{
					if (Citaj(pocetak)==10)
					{
						_krajReda=false;
						zavrsetak = pocetak;
						pocetak = pocetak + 1;
					}
					else if (char.IsWhiteSpace(Citaj(pocetak)))
					{   
						zavrsetak = pocetak;
						pocetak = pocetak + 1;
					}
					else if (Citaj(pocetak)=='\'')
					{
						string pogreska=ulazniProgram.Substring(pocetak,zavrsetak-pocetak);
						GreskaJezicnogProcesora greska=new GreskaJezicnogProcesora(_trenutnaLinija,pocetak, "Pogresno napisan niz znakova:  "+pogreska);
						listaGresaka.Add(greska);
						pocetak = zavrsetak;
						zavrsetak--;
					}
					else
					{
						GreskaJezicnogProcesora greska=new GreskaJezicnogProcesora(_trenutnaLinija,pocetak, "Nepoznat znak:  "+a);
						listaGresaka.Add(greska);
						zavrsetak = pocetak;
						pocetak = pocetak + 1;
					}
					DKAStanje pomocnoStanje=new DKAStanje(false,false,"Pocetno");
					trenutnoStanje=pomocnoStanje;
				}
				else if (izraz!=null)
				{
					string leksickaJedinka=_ulazniProgram.Substring(pocetak,posljednji-pocetak+1);
					switch(izraz.Substring(0,3))		/*na temelju 3 pocetna slova odredjujemo 
														 kojem regularnom izrazu pripada jedinka*/
					{
						case "IDN"://ispitaj jel kros ili idn
						{
							//snimi u tablicu uniformnih znakova od pocetak do posljednji-pocetak+1
							//i ako je identifikator u tablicu identifikatora
							#region provjeri je li pripada tablici KROS ili tablici Identifikatora

							if (_tablicaZnakova.LeksickaJedinkaJeKROS(leksickaJedinka))
							{
								//provjeri da li je to boolean konstanta true ili false
								if (leksickaJedinka==bool.TrueString || leksickaJedinka == bool.FalseString)
								{
									UniformniZnak uniformniZnak=new UniformniZnak(OznakeZnakovaGramatike.Boolean, leksickaJedinka, UniformniZnakEnum.Konstanta,_trenutnaLinija,pocetak);
									if (_tablicaZnakova.DodajKonstantu(leksickaJedinka, TipKonstanteEnum.Boolean, uniformniZnak, listaGresaka) == true) {
										_tablicaUniformnihZnakova.Add(uniformniZnak);
									}
								}
								else
								{
									UniformniZnak uniformniZnak=new UniformniZnak(leksickaJedinka, leksickaJedinka, UniformniZnakEnum.KROS,_trenutnaLinija,pocetak);
									_tablicaUniformnihZnakova.Add(uniformniZnak);
								}
							}
							else
							{
								UniformniZnak uniformniZnak=new UniformniZnak(OznakeZnakovaGramatike.Identifikator, leksickaJedinka, UniformniZnakEnum.Identifikator,_trenutnaLinija,pocetak);
								_tablicaUniformnihZnakova.Add(uniformniZnak);
							}
							#endregion;
							break;
						}
						case "KON"://ako je zadnje stanje KONST onda je konstanta integer tipa
						{
							//inace za KONSTre1 i re2 je real tipa
							//snimi u tablicu konstanti i tablicu uniformnih znakova
							#region provjeri da li je konstanta integer ili real

							if (izraz=="KONST")
							{
								UniformniZnak uniformniZnak=new UniformniZnak(OznakeZnakovaGramatike.UnsignedInteger, leksickaJedinka, UniformniZnakEnum.Konstanta,_trenutnaLinija,pocetak);
								if (_tablicaZnakova.DodajKonstantu(leksickaJedinka, TipKonstanteEnum.Integer, uniformniZnak, listaGresaka) == true) {
									_tablicaUniformnihZnakova.Add(uniformniZnak);
								}

							}
							else //izraz==KONSTre1 || izraz==KONSTre2
							{
								
								UniformniZnak uniformniZnak=new UniformniZnak(OznakeZnakovaGramatike.UnsignedReal, leksickaJedinka, UniformniZnakEnum.Konstanta,_trenutnaLinija,pocetak);
								if (_tablicaZnakova.DodajKonstantu(leksickaJedinka, TipKonstanteEnum.Real, uniformniZnak, listaGresaka) == true) {
									_tablicaUniformnihZnakova.Add(uniformniZnak);
								}
							}
							#endregion;
							break;
						}
						case "STR"://snimi u tablicu identifikatora i unif znakova
						{
							#region dodaj STRING u tablicu identifikatora i uniformnih znakova
							UniformniZnak uniformniZnak=new UniformniZnak(OznakeZnakovaGramatike.String, leksickaJedinka, UniformniZnakEnum.Konstanta,_trenutnaLinija,pocetak);
							if (_tablicaZnakova.DodajKonstantu(leksickaJedinka, TipKonstanteEnum.String, uniformniZnak, listaGresaka) == true) {
								_tablicaUniformnihZnakova.Add(uniformniZnak);
							}
							#endregion;
							break;
						}
						case "SIM"://snimi u tablicu unif znakova
						{
							#region	dodaj simbol u tablicu Uniformnih znakova
							UniformniZnak uniformniZnak=new UniformniZnak(leksickaJedinka, leksickaJedinka, UniformniZnakEnum.KROS,_trenutnaLinija,pocetak);
							_tablicaUniformnihZnakova.Add(uniformniZnak);
							#endregion;
							break;
						}
						default:
							break;
								
					}
					
					izraz = null;
					pocetak = posljednji + 1;
					zavrsetak = posljednji;
					DKAStanje pomocnoStanje=new DKAStanje(false,false,"Pocetno");
					trenutnoStanje=pomocnoStanje;
				}

				//D6
				//ako je kraj programa zavrsi sa radom leksickog analizatora
			}
		}

		public ArrayList TablicaUniformnihZnakova
		{
			get{return  _tablicaUniformnihZnakova;}
		}

		public TablicaZnakova TablicaZnakova
		{
			get { return _tablicaZnakova; }
		}
		
		private char Citaj(int pozicija)
		{
			if (pozicija>=_ulazniProgram.Length) return (char)0;
			if (_krajReda==true) 
			{
				_trenutnaLinija++;
				_krajReda=false;
			}
			else if (_ulazniProgram[pozicija]==10)
			{
				_krajReda=true;
			}
			return _ulazniProgram[pozicija];
		}

		private bool _krajReda;
		private string _ulazniProgram;
		private int _trenutnaLinija;
		private TablicaZnakova _tablicaZnakova=new TablicaZnakova();
		private ArrayList _tablicaUniformnihZnakova=new ArrayList();
	}

}
