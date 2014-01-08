using System;
using System.Collections;
using JezicniProcesor;

namespace JezicniProcesor
{
	///	<summary>
	///	Pravila	za pisanje metoda rekurzivnog spusta:
	///	-	prvo slovo metode postaje veliko slovo (iako nezavrsni znak	gramatike ne
	///		pocinje	velikim	slovom
	///	-	crtica (-) se zamjenjuje podvlakom (_)
	///	-	metode rekurzivnog spusta su oznacene kao privatne
	///	-	ne treba prepisivati komentare koji	sluze pojasnjenju pojedinih	djelova
	///		u metodi Pascal_program
	///	-	ako	trebate	procitati zavrsni znak identifier koristite	predefiniranu
	///		vrijednost OznakeZnakovaGramatike.Identifikator.	Slicno za unsigned-integer,	unsigned-real i	string
	///		zavrsne	znakove
	///	Obratite paznju	na to da se	neka svojstva i	metode naslijedjuju	iz bazne klase RekurzivniSpustBase
	///	(recimo	svojstvo TrenutniZnak, metoda PrimijenioProdukciju() itd.)
	///
	///	Dodatak: sve nedovrsene	funkcije-produkcije	nek	budu na	dnu	file-a da ih je	lakse pronaci...
	///	
	///	Pravila za pisanje semantickog dijela rekurzivnog spusta:
	///		- procitajte poglavlje 4.3.5 u knjizi JP2 (str 195-198, ukljucivo)
	///		- za svaku produkciju koju pisete potrebno je uskladiti imena svojstava 
	///       (kako je to opisano u gore navedenom poglavlju knjige). Napomena: ako se uslijed uskladjivanja imena
	///       dogodi da je neka varijabla istovremeno izvedeno i nasljedno svojstvo, njem prefiks treba biti 'in' (npr. inVrijednost)
	///     - postoji sljedece odstupanje od pravila u knjizi: nikada se ne razmjenjuje adresa svojstava.
	///       Umjesto toga, koriste se C# 'out' parametri. Na taj nacin se svojstvo automatski salje po referenci,
	///       C# se brine o tome. Dakle, kada dodajete parametre u metode koje oznacuju nezavrsne znakove, izvedena
	///       svojstva trebaju biti oznacena kao 'out'. Takodjer, kad pozivate metode koje sadrze 'out' parametre,
	///       njih trebate oznaciti kljucnom rijeci 'out' (jasnije ce bit iz primjera).
	///       Postoji jos jedno pravilo glede out parametera. Njima se MORA pridruziti neka vrijednost unutar tijela metode.
	///       Zbog toga na pocetku metode out parametrima pridruzite sljedece vrijednosti:
	///			- za tip UInt32: 0
	///			- za tip string: "" (prazan string)
	///			- za tip class: null
	///			- za enumeracije: Unknown (svaka enumeracija za svojstva ima definiranu vrijednost invalid, npr. UnarniOperator.Unknown)
	///     - izvedena svojstva zavrsnih znakova gramatike dobiju se kroz metode KonzumirajZavrsniZnak i PojediZavrsni znak.
	///       One, osim zavrsnog znaka kojeg trebaju konzumirati, primaju jos jedan 'out' parametar koji predstavlja izvedeno
	///       svojstvo. Tip tog parametra je ili 'string' ili 'UInt32', ovisno o tome koji se zavrsni znak konzumira.
	///     - izlazni zavrsni znakovi su metode(tj. akcije) semantickog analizatora i nalaze se u klasi SemantickiAnalizator(SemantickiAnalizator.cs)
	///       (ako neka akcija jos nije isprogramirana a trebate je pozvati, oznacite to mjesto u rek. spustu 
	///       kako bi se poslije mogli lako pronaci nedovrseni dijelovi)
	///       Do objekta SemantickiAnalizator se dolazi preko property-a istog imena 'SemantickiAnalizator', definiranog u
	///       RekurzivniSpustBase klasi
	///     - lokalne varijable za svojstva deklarirajte unutar 'switch' naredbe, za svaku produkciju posebno.
	///       Te varijable se trebaju deklarirati ispod poziva metode 'PrimijenioProdukciju'
	///     - za primjer metode rek. spusta vidi metode 'Label_declaration' i 'Label'
	///    
	///	</summary>
	class RekurzivniSpust: RekurzivniSpustBase
	{
		#region	Konstruktor	i metoda Pokreni

		public RekurzivniSpust(ICollection listaUniformnihZnakova, TablicaZnakova tablicaZnakova, IList listaGresaka,
			out CvorSintaksnogStabla sintaksnoStablo, bool ulaznoIzlazneProcedure):	
			base(listaUniformnihZnakova, tablicaZnakova, listaGresaka, ulaznoIzlazneProcedure)
		{
			sintaksnoStablo = PostaviPocetniNezavrsniZnak("<pascal-program>");
			_pocetakBloka=new ArrayList();
			_pocetakNaredbe1=new ArrayList();
			_pocetakNaredbe=new ArrayList();
			_pocetakIzraza=new ArrayList();
			_pocetakIzraza1=new ArrayList();
			_pocetakTipa=new ArrayList();
			_pocetakVarijante=new ArrayList();

			_pocetakBloka.Add("label");
			_pocetakBloka.Add("const");
			_pocetakBloka.Add("type"); 
			_pocetakBloka.Add("var"); 
			_pocetakBloka.Add("procedure");
			_pocetakBloka.Add("function"); 
			_pocetakBloka.Add("begin");
 
			_pocetakNaredbe.Add("begin"); 
			_pocetakNaredbe.Add("if");
			_pocetakNaredbe.Add("while"); 
			_pocetakNaredbe.Add("repeat");
			_pocetakNaredbe.Add("for"); 
			_pocetakNaredbe.Add("case");
			_pocetakNaredbe.Add("goto"); 
			_pocetakNaredbe.Add("with");

			_pocetakIzraza.Add("not"); 
			_pocetakIzraza.Add(OznakeZnakovaGramatike.UnsignedReal);
			_pocetakIzraza.Add(OznakeZnakovaGramatike.UnsignedInteger); 
			_pocetakIzraza.Add(OznakeZnakovaGramatike.Boolean);
			_pocetakIzraza.Add("+"); 
			//_pocetakIzraza.Add(OznakeZnakovaGramatike.String);
			_pocetakIzraza.Add(OznakeZnakovaGramatike.Identifikator);
			_pocetakIzraza.Add("@"); 
			_pocetakIzraza.Add("-");
			_pocetakIzraza.Add("nil"); 
			_pocetakIzraza.Add("(");

			_pocetakTipa.Add(OznakeZnakovaGramatike.UnsignedInteger);
			_pocetakTipa.Add(OznakeZnakovaGramatike.Identifikator); 
			_pocetakTipa.Add("file"); 
			_pocetakTipa.Add("record"); 
			_pocetakTipa.Add("packed"); 
			_pocetakTipa.Add("array"); 
			_pocetakTipa.Add("set"); 
			_pocetakTipa.Add("(");
			_pocetakTipa.Add("^");

			_pocetakVarijante.Add("@"); 
			_pocetakVarijante.Add("(");
			_pocetakVarijante.Add("nil"); 
			_pocetakVarijante.Add(";");
			_pocetakVarijante.Add("-"); 
			_pocetakVarijante.Add("+");
			_pocetakVarijante.Add(OznakeZnakovaGramatike.Identifikator);
			_pocetakVarijante.Add(OznakeZnakovaGramatike.Boolean);
			_pocetakVarijante.Add(OznakeZnakovaGramatike.UnsignedInteger);
			_pocetakVarijante.Add(OznakeZnakovaGramatike.UnsignedReal);
			_pocetakVarijante.Add("not"); 

			_pocetakNaredbe1.AddRange(_pocetakNaredbe);
			_pocetakNaredbe1.Add("end"); 
			_pocetakNaredbe1.Add(";");
			_pocetakNaredbe1.Add("else"); 
			_pocetakNaredbe1.Add("until");
			_pocetakNaredbe1.Add(OznakeZnakovaGramatike.UnsignedInteger);
			_pocetakNaredbe1.Add(OznakeZnakovaGramatike.Identifikator);

			_pocetakIzraza1.Add("then"); 
			_pocetakIzraza1.Add("downto");
			_pocetakIzraza1.Add("end"); 
			_pocetakIzraza1.Add("..");
			_pocetakIzraza1.Add("of"); 
			_pocetakIzraza1.Add("]");
			_pocetakIzraza1.Add(")"); 
			_pocetakIzraza1.Add("else");
			_pocetakIzraza1.Add(","); 
			_pocetakIzraza1.Add(":");
			_pocetakIzraza1.Add(";"); 
			_pocetakIzraza1.Add("do");
			_pocetakIzraza1.Add("to"); 
			_pocetakIzraza1.Add("until");
			_pocetakIzraza1.Add("<"); 
			_pocetakIzraza1.Add("=");
			_pocetakIzraza1.Add("<>");
			_pocetakIzraza1.Add(">=");
			_pocetakIzraza1.Add("<=");	
			_pocetakIzraza1.Add(">");
		}

		public void	Pokreni()
		{
			try
			{
				Pascal_program();
				PrijaviZaostaluGresku();
			}
			catch(FatalnaGreskaException exc)
			{
				UpisiDirektnoUListuGresaka(exc.LinijaIzvornogKoda, exc.PozicijaGreske, exc.Opis);
			}
#if !DEBUG
			catch(System.Exception exc)
			{
				UpisiDirektnoUListuGresaka(-1, -1, "Kriticna greska: Kompilacija se zaustavlja.");
			}
#endif
		}
		
		#endregion

		private	void Pascal_program()
		{
			//switch kojim se bira produkcija koja ce se primijeniti
			//konstruira se	na temelju skupa PRIMIJENI
			//u	slucaju	da vise	produkcija NEMA	isti nezavrsni znak	na lijevoj strani
			//(dakle nema ni dileme	koju produkciju	primijeniti) switch	se moze	ispustiti
			//ova metoda bi	trebala	biti upravo	takva, medjutim	switch je ovdje	radi ilustracije
			//njegovog koristenja
			switch(TrenutniZnak.OznakaZnakaGramatike)
			{
				case "program":
				{
					#region	Produkcija <pascal-program>	-> program identifier ; <block> .
					PrimijenioProdukciju("<pascal-program>", "program identifier ; <block> .");
					string inImeIdentifikatora;

					//slijedi implementacija desne strane produkcije
					KonzumirajZavrsniZnak("program");
					//ova metoda (KonzumirajZavrsniZnak) sama poziva metodu	FatalnaGreska() ako ne	uspije procitati zadani	zavrsni	znak
					ArrayList slijedi=new ArrayList();
					slijedi.Add(";");
					Provjeri(OznakeZnakovaGramatike.Identifikator,slijedi,"Oèekivan identifikator!");
					PojediZavrsniZnak(OznakeZnakovaGramatike.Identifikator, out inImeIdentifikatora);

					slijedi.Remove(";");
					Provjeri(";",slijedi,"Oèekivana toèka-zarez!");
					PojediZavrsniZnak(";");
					SemantickiAnalizator.InicijalizirajProgram(inImeIdentifikatora);
					Block(slijedi, inImeIdentifikatora);
					Provjeri(".",slijedi,"Oèekivano 'END.' na kraju programa!");
					PojediZavrsniZnak("."); 
					
					#endregion
					break;
				}


				default:
				{
					PrijaviGresku("Oèekivana kljuèna rijeè PROGRAM!");
					break;
				}
			}
		}

		#region Nevescaninov dio
		private	void Block(ArrayList slijedi, string nImeIdentifikatora)
		{
			ArrayList zapocinje=_pocetakBloka;
			if (Provjeri(zapocinje,slijedi,"Pogrešan poèetak bloka!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case "procedure":
					case "function":
					case "begin":
					case "type":
					case "const":
					case "var":
					{
						#region	Produkcija <block> -> <block1> 
						PrimijenioProdukciju("<block>","<block1>");
						//string nImePotprograma,nImePotprograma1;
						
						SemantickiAnalizator.OtvorenBlok(nImeIdentifikatora);
						Block1(slijedi, nImeIdentifikatora);
						SemantickiAnalizator.ZatvorenBlok(nImeIdentifikatora);
						
						#endregion
						break;
					}
					case "label":
					{
						#region	Produkcija <block> -> <label-declaration> ;	<block1> 
						PrimijenioProdukciju("<block>","<label-declaration>	; <block1>");

						SemantickiAnalizator.OtvorenBlok(nImeIdentifikatora);	
						ArrayList slijedi1=new ArrayList(_pocetakBloka);
						slijedi1.Remove("label"); slijedi1.Add(";");
						Label_declaration(slijedi1);
						PojediZavrsniZnak(";");
						Block1(slijedi, nImeIdentifikatora);
						SemantickiAnalizator.ZatvorenBlok(nImeIdentifikatora);

						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Ne postoji blok!");
						break;
					}
				}
			}
		}

		private	void Block1(ArrayList slijedi, string nImeIdentifikatora)
		{
			ArrayList zapocinje=new ArrayList(_pocetakBloka);
			zapocinje.Remove("label");
			if (Provjeri(zapocinje,slijedi,"Pogrešan poèetak bloka (1)!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case "procedure":
					case "function":
					case "begin":
					case "type":
					case "var":
					{
						#region	Produkcija <block1>	-> <block2>	
						PrimijenioProdukciju("<block1>","<block2>");

						Block2(slijedi, nImeIdentifikatora);
						#endregion
						break;
					}
					case "const":
					{
						#region	Produkcija <block1>	-> <constant-declaration> <block2>
						PrimijenioProdukciju("<block1>","<constant-declaration> <block2>");

						ArrayList slijedi1=new ArrayList(zapocinje);
						slijedi1.AddRange(slijedi);
						slijedi1.Remove("const");
						Constant_declaration(slijedi1);
						Block2(slijedi, nImeIdentifikatora);
						#endregion
						break;
					}
					default:
					{
						FatalnaGreska();
						break;
					}
				}
			}
		}
		
		private	void Block2(ArrayList slijedi, string nImeIdentifikatora)
		{
			ArrayList zapocinje=new ArrayList(_pocetakBloka);
			zapocinje.Remove("label"); zapocinje.Remove("const");
			if (Provjeri(zapocinje,slijedi,"Pogrešan poèetak bloka (2)!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case "type":
					{
						#region	<block2> ->	<type-declaration> <block3> 
						PrimijenioProdukciju("<block2>","<type-declaration>	<block3>");
					
						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.AddRange(_pocetakBloka);
						slijedi1.Remove("type");
						slijedi1.Remove("label"); slijedi1.Remove("const");
						Type_declaration(slijedi1);
						Block3(slijedi, nImeIdentifikatora);
						#endregion
						break;
					}
					case "procedure":
					case "function":
					case "begin":
					case "var":
					{
						#region	Produkcija <block2>	-> <block3>
						PrimijenioProdukciju("<block2>","<block3>");

						Block3(slijedi, nImeIdentifikatora);
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Pogreška u bloku (blok3)!");
						break;
					}
				}
			}
		}

		private	void Block3(ArrayList slijedi, string nImeIdentifikatora)
		{
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add("var"); zapocinje.Add("procedure");
			zapocinje.Add("function");
			zapocinje.Add("begin");
			if (Provjeri(zapocinje,slijedi,"Pogrešan poèetak bloka (3)!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case "var":
					{
						#region	Produkcija <block3>	-> <variable-declaration> <block4>
						PrimijenioProdukciju("<block3>","<variabe-declaration> <block4>");

						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.AddRange(zapocinje);
						slijedi1.Remove("var");
						Variable_declaration(slijedi1);
						Block4(slijedi, nImeIdentifikatora);
						#endregion
						break;
					}
					case "procedure":
					case "function":
					case "begin":
					{
						#region	Produkcija <block3>	-> <block4>
						PrimijenioProdukciju("<block3>","<block4>");

						Block4(slijedi, nImeIdentifikatora);
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Pogrešan poèetak bloka!");
						break;
					}
				}
			}
		}

		private	void Block4(ArrayList slijedi, string nImeIdentifikatora)
		{
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add("procedure"); zapocinje.Add("function");
			zapocinje.Add("begin");
			if (Provjeri(zapocinje,slijedi,"Pogreška u bloku!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{	
					case "begin":
					{
						#region	Produkcija <block4>	-> <block5>
						PrimijenioProdukciju("<block4>","<block5>");

						Block5(slijedi, nImeIdentifikatora);
						#endregion
						break;
					}
					case "procedure":
					case "function":
					{
						#region	Produkcija <block4>	-> <proc-and-func-declaration> <block5> 
						PrimijenioProdukciju("<block4>","<proc-and-func-declaration> <block5>");
						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add(";"); slijedi1.Add("begin");
						Proc_and_func_declaration(slijedi1);

						ArrayList slijedi2=new ArrayList(slijedi);
						slijedi2.Add("begin");
						Block5(slijedi, nImeIdentifikatora);
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Pogreška u bloku!");
						break;
					}
				}
			}
		}

		private	void Block5(ArrayList slijedi, string nImeIdentifikatora)
		{
			if (Provjeri("begin",slijedi,"Oèekivana kljuèna rijeè BEGIN!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case "begin":
					{
						#region	Produkcija <block5>	-> begin <statement-list> end
						PrimijenioProdukciju("<block5>","begin <statement-list>	end");

                        SemantickiAnalizator.ZapocelaImplementacija(nImeIdentifikatora);
						KonzumirajZavrsniZnak("begin");
						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add("end");
						Statement_List(slijedi1);
						Provjeri("end",slijedi,"Oèekivana kljuèna rijeè END!");
						PojediZavrsniZnak("end");
						SemantickiAnalizator.ZavrsilaImplementacija(nImeIdentifikatora);
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Oèekivana kljuèna rijeè BEGIN!");
						break;
					}
				}
			}
		}

		private	void Label_declaration(ArrayList slijedi)
		{
			if (Provjeri("label",slijedi,"Pogreška u bloku (label)!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case "label":
					{
						#region	Produkcija <label-declaration> -> label	unsigned-integer <label-declaration_1>
						PrimijenioProdukciju("<label-declaration>","label unsigned-integer <label-declaration_1>");
						UInt32 inVrijednost;

						KonzumirajZavrsniZnak("label");

						ArrayList slijedi0=new ArrayList(slijedi);
						slijedi0.Add(",");
						Provjeri(OznakeZnakovaGramatike.UnsignedInteger,slijedi0,"Nedostaje cijeli broj!");

						//metoda PojediZavrsniZnak vraca svoje izvedeno svojstvo kroz varijablu inVrijednost
						PojediZavrsniZnak(OznakeZnakovaGramatike.UnsignedInteger, out inVrijednost);

						//poziva se akcija semantickog analizatora
						SemantickiAnalizator.NovaLabela(inVrijednost);

						Label_declaration_1(slijedi);
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Oèekivana kljuèna rijeè LABEL!");
						break;
					}
				}
			}
		}

		private	void Label_declaration_1(ArrayList slijedi)
		{
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add(","); zapocinje.Add(";");
			if (Provjeri(zapocinje,slijedi,"Oèekivan zarez ili toèka-zarez!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case ",":
					{
						#region	Produkcija <label-declaration_1> ->	, unsigned-integer <label-declaration_1>
						PrimijenioProdukciju("<label-declaration_1>",",	unsigned-integer <label-declaration_1>");
						UInt32 inVrijednost;

						KonzumirajZavrsniZnak(",");

						ArrayList slijedi0=new ArrayList(slijedi);
						slijedi0.Add(",");
						Provjeri(OznakeZnakovaGramatike.UnsignedInteger,slijedi0,"Oèekivan cijeli broj!");
						PojediZavrsniZnak(OznakeZnakovaGramatike.UnsignedInteger, out inVrijednost);

						SemantickiAnalizator.NovaLabela(inVrijednost);
						Label_declaration_1(slijedi);
						#endregion
						break;
					}
					case ";":
					{
						#region	Produkcija <label-declaration_1> ->	<empty>
						PrimijenioProdukciju("<label-declaration_1>","<empty>");

						Empty();
						#endregion;
						break;
					}
					default:
					{
						FatalnaGreska();
						break;
					}
				}	
			}
		}

		private	void Constant_declaration(ArrayList slijedi)
		{
			if(Provjeri("const",slijedi,"Oèekivana kljuèna rijeè CONST!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case "const":
					{
						#region	Produkcija <constant-declaration> -> const identifier =	<constant> ; <constant-declaration_1>	
						PrimijenioProdukciju("<constant-declaration>","const identifier	= <constant> ; <constant-declaration_1>");
						TipPodatka inTipKonstante;
						UInt32 inVrijednost;
						string inImeIdentifikatora;

						KonzumirajZavrsniZnak("const");

						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add("=");
						slijedi1.AddRange(_pocetakIzraza);
						slijedi1.Add("type");
						slijedi1.Add("procedure");
						slijedi1.Add("begin");
						slijedi1.Add("var");
						slijedi1.Add("function");
						slijedi1.Add(OznakeZnakovaGramatike.Identifikator);
						Provjeri(OznakeZnakovaGramatike.Identifikator,slijedi1,"Oèekivan identifikator!");
						PojediZavrsniZnak(OznakeZnakovaGramatike.Identifikator,out inImeIdentifikatora);

						slijedi1.Remove("=");
						Provjeri("=",slijedi1,"Oèekivan znak jednako!");
						PojediZavrsniZnak("=");

						ArrayList slijedi2=new ArrayList(slijedi);
						slijedi2.Add("type");
						slijedi2.Add("procedure");
						slijedi2.Add("begin");
						slijedi2.Add("var");
						slijedi2.Add("function");
						slijedi2.Add(OznakeZnakovaGramatike.Identifikator);
						slijedi2.Add(";");
						Constant(slijedi2, out inTipKonstante, out inVrijednost);

						SemantickiAnalizator.NovaKonstanta(inImeIdentifikatora, inTipKonstante, inVrijednost);

						slijedi1=new ArrayList(slijedi);
						slijedi1.Add("type");
						slijedi1.Add("procedure");
						slijedi1.Add("begin");
						slijedi1.Add("var");
						slijedi1.Add("function");
						slijedi1.Add(OznakeZnakovaGramatike.Identifikator);
						Provjeri(";",slijedi1,"Oèekivana toèka-zarez!");
						PojediZavrsniZnak(";");

						Constant_declaration_1(slijedi);
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Oèekivana kljuèna rijeè CONST!");
						break;
					}
				}
			}
		}

		private	void Constant_declaration_1(ArrayList slijedi)
		{
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add("type");
			zapocinje.Add("procedure");
			zapocinje.Add("begin");
			zapocinje.Add("var");
			zapocinje.Add("function");
			zapocinje.Add(OznakeZnakovaGramatike.Identifikator);
			if (Provjeri(zapocinje,slijedi,"Oèekivan identifikator!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case OznakeZnakovaGramatike.Identifikator:
					{
						#region	Produkcija <constant-declaration_1>	-> identifier = <constant> ; <constant-declaration_1>
						PrimijenioProdukciju("<constant-declaration_1>","identifier = <constant> ; <constant-declaration_1>");
						TipPodatka inTipKonstante;
						UInt32 inVrijednost;
						string inImeIdentifikatora;

						KonzumirajZavrsniZnak(OznakeZnakovaGramatike.Identifikator, out inImeIdentifikatora);

						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add("=");
						slijedi1.AddRange(_pocetakIzraza);
						slijedi1.Add("type");
						slijedi1.Add("procedure");
						slijedi1.Add("begin");
						slijedi1.Add("var");
						slijedi1.Add("function");
						slijedi1.Add(OznakeZnakovaGramatike.Identifikator);
						Provjeri("=",slijedi1,"Oèekivan znak jednako!");
						PojediZavrsniZnak("=");

						ArrayList slijedi2=new ArrayList(slijedi);
						slijedi2.Add("type");
						slijedi2.Add("procedure");
						slijedi2.Add("begin");
						slijedi2.Add("var");
						slijedi2.Add("function");
						slijedi2.Add(OznakeZnakovaGramatike.Identifikator);
						slijedi2.Add(";");
						Constant(slijedi2, out inTipKonstante, out inVrijednost);
						SemantickiAnalizator.NovaKonstanta(inImeIdentifikatora,inTipKonstante,inVrijednost);

						slijedi1=new ArrayList(slijedi);
						slijedi1.Add("type");
						slijedi1.Add("procedure");
						slijedi1.Add("begin");
						slijedi1.Add("var");
						slijedi1.Add("function");
						slijedi1.Add(OznakeZnakovaGramatike.Identifikator);

						Provjeri(";",slijedi1,"Oèekivana toèka-zarez!");
						PojediZavrsniZnak(";");
						Constant_declaration_1(slijedi);
						#endregion
						break;
					}
					case "type":
					case "procedure":
					case "begin":
					case "var":
					case "function":
					{
						#region	Produkcija <constant-declaration_1>	-> <empty>
						PrimijenioProdukciju("<constant-declaration_1>","<empty>");

						Empty();
						#endregion;
						break;
					}
					default:
					{
						PrijaviGresku("Oèekivan zarez ili toèka-zarez!");
						break;
					}
				}
			}
		}

		private	void Type_declaration(ArrayList slijedi)
		{
			if (Provjeri("type",slijedi,"Oèekivana kljuèna rijeè TYPE!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case "type":
					{
						#region	Produkcija <type-declaration> -> type identifier = <type> ; <type-declaration_1> 
						PrimijenioProdukciju("<type-declaration>","type	identifier = <type>	; <type-declaration_1>");
						TipPodatka inTip;
						string inImeIdentifikatora;

						KonzumirajZavrsniZnak("type");

						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add("procedure");
						slijedi1.Add("begin");
						slijedi1.Add("var");
						slijedi1.Add("function");
						slijedi1.Add(OznakeZnakovaGramatike.Identifikator);
						slijedi1.AddRange(_pocetakTipa);
						slijedi1.Add(";");
						slijedi1.Add("=");
						Provjeri(OznakeZnakovaGramatike.Identifikator,slijedi1,"Oèekivan identifikator!");
						PojediZavrsniZnak(OznakeZnakovaGramatike.Identifikator, out inImeIdentifikatora);

						slijedi1.Remove("=");
						Provjeri("=",slijedi1,"Oèekivan znak jednako!");
						PojediZavrsniZnak("=");

						ArrayList slijedi2=new ArrayList(slijedi);
						slijedi2.Add("procedure");
						slijedi2.Add("begin");
						slijedi2.Add("var");
						slijedi2.Add("function");
						slijedi2.Add(";");
						Type(slijedi2, out inTip);

						SemantickiAnalizator.NoviTip(inImeIdentifikatora,inTip);

						slijedi1=new ArrayList(slijedi);
						slijedi1.Add("procedure");
						slijedi1.Add("begin");
						slijedi1.Add("var");
						slijedi1.Add("function");
						slijedi1.Add(OznakeZnakovaGramatike.Identifikator);
						Provjeri(";",slijedi1,"Oèekivana toèka-zarez!");
						PojediZavrsniZnak(";");

						Type_declaration_1(slijedi);
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Oèekivana kljuèna rijeè TYPE!");
						break;
					}
				}
			}
		}

		private	void Type_declaration_1(ArrayList slijedi)
		{
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add("procedure");
			zapocinje.Add("begin");
			zapocinje.Add("var");
			zapocinje.Add("function");
			zapocinje.Add(OznakeZnakovaGramatike.Identifikator);
			if (Provjeri(zapocinje,slijedi,"Oèekivan identifikator!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case OznakeZnakovaGramatike.Identifikator:
					{
						#region	Produkcija <type-declaration_1>	-> identifier = <type> ; <type-declaration_1>
						PrimijenioProdukciju("<type-declaration_1>","identifier = <type> ; <type-declaration_1>");
						TipPodatka inTip;
						string inImeIdentifikatora;

						KonzumirajZavrsniZnak(OznakeZnakovaGramatike.Identifikator, out inImeIdentifikatora);

						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add("procedure");
						slijedi1.Add("begin");
						slijedi1.Add("var");
						slijedi1.Add("function");
						slijedi1.Add(OznakeZnakovaGramatike.Identifikator);
						slijedi1.AddRange(_pocetakTipa);
						slijedi1.Add(";");
						Provjeri("=",slijedi1,"Oèekivan znak jednako!");
						PojediZavrsniZnak("=");

						ArrayList slijedi2=new ArrayList(slijedi);
						slijedi2.Add("procedure");
						slijedi2.Add("begin");
						slijedi2.Add("var");
						slijedi2.Add("function");
						slijedi2.Add(";");
						Type(slijedi2, out inTip);

						SemantickiAnalizator.NoviTip(inImeIdentifikatora,inTip);

						slijedi1=new ArrayList(slijedi);
						slijedi1.Add("procedure");
						slijedi1.Add("begin");
						slijedi1.Add("var");
						slijedi1.Add("function");
						slijedi1.Add(OznakeZnakovaGramatike.Identifikator);
						Provjeri(";",slijedi1,"Oèekivana toèka-zarez!");
						PojediZavrsniZnak(";");

						Type_declaration_1(slijedi);						
						#endregion
						break;
					}
					case "procedure":
					case "begin":
					case "var":
					case "function":
					{
						#region	Produkcija <type-declaration_1>	-> <empty>
						PrimijenioProdukciju("<type-declaration_1>","<empty>");

						Empty();
						#endregion;
						break;
					}
					default:
					{
						PrijaviGresku("Oèekivan zarez ili toèka-zarez!");
						break;
					}
				}
			}
		}

		private	void Variable_declaration(ArrayList slijedi)
		{
			if (Provjeri("var",slijedi,"Oèekivana kljuèna rijeè VAR!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case "var":
					{
						#region	Produkcija <variable-declaration> -> var <identifier-list> : <type>	; <variable-declaration_1>
						PrimijenioProdukciju("<variable-declaration>","var <identifier-list> : <type> ; <variable-declaration_1>");
						ICollection inListaImena;
						TipPodatka inTip;

						KonzumirajZavrsniZnak("var");
						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add(":"); //slijedi1.AddRange(pocetakTipa);
						slijedi1.Add(";"); slijedi1.Add("function");
						slijedi1.Add("procedure"); slijedi1.Add("procedure");
						Identifier_list(slijedi1, out inListaImena);

						slijedi1.Remove(":");
						//slijedi1.AddRange(pocetakTipa);
						Provjeri(":",slijedi1,"Oèekivana dvotoèka!");
						PojediZavrsniZnak(":");

						ArrayList slijedi2=new ArrayList(slijedi);
						slijedi2.Add(";"); 
						//slijedi2.Add(OznakeZnakovaGramatike.Identifikator);
						Type(slijedi2, out inTip);

						SemantickiAnalizator.NovaVarijabla(inListaImena, inTip);

						//slijedi2.Remove(";");
						Provjeri(";",slijedi2,"Nedostaje toèka-zarez!");
						PojediZavrsniZnak(";");

						Variable_declaration_1(slijedi);
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Oèekivana kljuèna rijeè VAR!");
						break;
					}
				}
			}
		}

		private	void Variable_declaration_1(ArrayList slijedi)
		{
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add(OznakeZnakovaGramatike.Identifikator);
			zapocinje.Add("procedure"); zapocinje.Add("function");
			zapocinje.Add("begin");
			if (Provjeri(zapocinje,slijedi,"Oèekivan identifikator!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case OznakeZnakovaGramatike.Identifikator:
					{
						#region	Produkcija <variable-declaration_1>	-> <identifier-list> : <type> ; <variable-declaration_1>
						PrimijenioProdukciju("<variable-declaration_1>","<identifier-list> : <type> ; <variable-declaration_1>");
						ICollection inListaImena;
						TipPodatka inTip;

						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add(":"); //slijedi1.AddRange(pocetakTipa);
						slijedi1.Add(";"); slijedi1.Add("function");
						slijedi1.Add("procedure"); slijedi1.Add("procedure");
						Identifier_list(slijedi1, out inListaImena);

						slijedi1.Remove(":");
						Provjeri(":",slijedi1,"Oèekivana dvotoèka!");
						PojediZavrsniZnak(":");

						ArrayList slijedi2=new ArrayList(slijedi);
						slijedi2.Add(";"); 
						//slijedi2.Add(OznakeZnakovaGramatike.Identifikator);
						Type(slijedi2, out inTip);

						SemantickiAnalizator.NovaVarijabla(inListaImena, inTip);

						//slijedi2.Remove(";");
						Provjeri(";",slijedi2,"Nedostaje toèka-zarez!");
						PojediZavrsniZnak(";");

						Variable_declaration_1(slijedi);
						#endregion
						break;
					}
					case "function":
					case "begin":
					case "procedure":
					{
						#region	Produkcija <variable-declaration_1>	-> <empty>
						PrimijenioProdukciju("<variable-declaration_1>","<empty>");

						Empty();
						#endregion;
						break;
					}
					default:
					{
						PrijaviGresku("Oèekivan identifikator!");
						break;
					}
				}
			}
		}

		void Identifier_list(ArrayList slijedi, out ICollection iZavrsnaListaImena)
		{
			iZavrsnaListaImena=null;
			if (Provjeri(OznakeZnakovaGramatike.Identifikator,slijedi,"Oèekivan identifikator!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case OznakeZnakovaGramatike.Identifikator:
					{
						#region	Produkcija <identifier-list> ->	identifier <identifier-list_1>
						PrimijenioProdukciju("<identifier-list>","identifier <identifier-list_1>");
						string inImeIdentifikatora;
						ICollection inListaImena;

						KonzumirajZavrsniZnak(OznakeZnakovaGramatike.Identifikator, out inImeIdentifikatora);
						SemantickiAnalizator.KreirajListuImena(inImeIdentifikatora, out inListaImena);
						Identifier_list_1(slijedi, inListaImena, out iZavrsnaListaImena);
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Nedostaju identifikatori!");
						break;
					}
				}			
			}
		}
		
		void Identifier_list_1(ArrayList slijedi, ICollection nListaImena, out ICollection iZavrsnaListaImena)
		{
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add(","); zapocinje.Add(":"); zapocinje.Add(")");
			iZavrsnaListaImena=null;

			if (Provjeri(zapocinje,slijedi,"Oèekivana dvotoèka ili zarez!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case ",":
					{
						#region	Produkcija <identifier-list_1> -> , identifier <identifier-list_1>
						PrimijenioProdukciju("<identifier-list_1>", ", identifier <identifier-list_1>");
						string inImeIdentifikatora;
						ICollection inListaImena;

						KonzumirajZavrsniZnak(",");
						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add(","); slijedi1.Add(":");  slijedi1.Add(")");
						Provjeri(OznakeZnakovaGramatike.Identifikator,slijedi1,"Oèekivan identifikator!");
						PojediZavrsniZnak(OznakeZnakovaGramatike.Identifikator, out inImeIdentifikatora);
						SemantickiAnalizator.NadopuniListuImena(inImeIdentifikatora,nListaImena, out inListaImena);
						Identifier_list_1(slijedi, inListaImena, out iZavrsnaListaImena);
						#endregion
						break;
					}
					case ")":
					case ":":
					{
						#region	Produkcija <identifier-list_1> -> <empty>
						PrimijenioProdukciju("<identifier-list_1>","<empty>");
						
						Empty();
						iZavrsnaListaImena=nListaImena;
						#endregion;
						break;
					}
					default:
					{
						PrijaviGresku("Oèekivana dvotoèka ili zarez!");
						break;
					}
				}
			}
		}
		#endregion

		#region Aldov dio
		private void Type(ArrayList slijedi, out TipPodatka iTip)
		{
			iTip = null;
			
			ArrayList zapocinje=_pocetakTipa;
			if (Provjeri(zapocinje,slijedi,"Oèekivana oznaka tipa!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case "(":
					case OznakeZnakovaGramatike.UnsignedInteger:
					case OznakeZnakovaGramatike.Identifikator:
					{
						#region <type> -> <simple-type>
						PrimijenioProdukciju("<type>", "<simple-type>");
						Simple_type(slijedi,out iTip);
						#endregion  
						break; 
					}
					
					case "record" : 					
					case "array" :

					{
						#region <type> ->  <structured-type>
						PrimijenioProdukciju("<type>", "<structured-type>");
						Structured_type(slijedi,out iTip);
						#endregion 
						break; 
					}
					case "^":
					{
						#region <type> ->  ^  <typeid> 
						PrimijenioProdukciju("<type>", "^  <typeid>");
						TipPodatka inTip;

						KonzumirajZavrsniZnak("^");
						Typeid(slijedi,out inTip);
						iTip = KonstruktoriTipova.PointerNa(inTip);
						#endregion
						break; 
					}
					default:
					{
						PrijaviGresku("Oèekivana oznaka tipa!");
						break;
					}
				}
			}
		}

		private void Simple_type(ArrayList slijedi,out TipPodatka iTip)
		{
			iTip = null;

			ArrayList zapocinje=new ArrayList();
			zapocinje.Add("("); zapocinje.Add(OznakeZnakovaGramatike.UnsignedInteger);
			zapocinje.Add(OznakeZnakovaGramatike.Identifikator);

			Provjeri(zapocinje,slijedi,"Pogrešana oznaka tipa!");
			switch(TrenutniZnak.OznakaZnakaGramatike)
			{
				case "(":
				{
					#region <simple_type> ->  (  <identifier_list>  )
					PrimijenioProdukciju("<simple_type>", "(  <identifier_list>  )");
					ICollection inListaImena;
					
					KonzumirajZavrsniZnak("(");
					ArrayList slijedi1=new ArrayList(slijedi);
					slijedi1.Add(")");
					Identifier_list(slijedi1,out inListaImena);
					PojediZavrsniZnak(")");
					SemantickiAnalizator.KreirajPobrojaniTip(inListaImena, out iTip);
					#endregion
					break;
				}
				case OznakeZnakovaGramatike.UnsignedInteger:
				{
					#region <simple_type> ->  unsigned_integer  ..  unsigned_integer
					PrimijenioProdukciju("<simple_type>", "unsigned_integer  ..  unsigned_integer");
					UInt32 inVrijednost1;
					UInt32 inVrijednost2;
					KonzumirajZavrsniZnak(OznakeZnakovaGramatike.UnsignedInteger,out inVrijednost1);

					ArrayList slijedi1=new ArrayList(slijedi);
					//slijedi1.Add(OznakeZnakovaGramatike.UnsignedInteger);
					if (!Provjeri("..",slijedi1,"Oèekivan niz '..'!")) return;
					PojediZavrsniZnak("..");

					Provjeri(OznakeZnakovaGramatike.UnsignedInteger,slijedi,"Oèekivan cijeli broj bez predznaka!");
					PojediZavrsniZnak(OznakeZnakovaGramatike.UnsignedInteger, out inVrijednost2);

					SemantickiAnalizator.KreirajIntervalniTip(inVrijednost1, inVrijednost2, out iTip);
					#endregion
					break;
				}
				case OznakeZnakovaGramatike.Identifikator :
				{
					#region <simple_type> ->  <typeid>
					PrimijenioProdukciju("<simple_type>", "<typeid>");
					Typeid(slijedi, out iTip);
					#endregion
					break;
				}
				default:
				{
					PrijaviGresku("Pogrešana oznaka tipa!");
					break;
				}
			}
			Provjeri(slijedi,"Pogrešan tip!");
		}

		private void Structured_type(ArrayList slijedi,out TipPodatka iTip)
		{
			ArrayList zapocinje=new ArrayList();
			iTip=null;
			zapocinje.Add("array"); zapocinje.Add("record");
			if (Provjeri(zapocinje,slijedi,"Pogrešna oznaka tipa!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case "array":
					{
						#region <structured_type> ->  array  [  <index_list>  ]  of  <type>
						PrimijenioProdukciju("<structured_type>", "array  [  <index_list>  ]  of  <type>");
						TipPodatka inBazniTipl;
						ICollection inListaTipova;

						KonzumirajZavrsniZnak("array");

						ArrayList slijedi1=new ArrayList(slijedi);
						//slijedi1.Add(OznakeZnakovaGramatike.UnsignedInteger);
						//slijedi1.Add(OznakeZnakovaGramatike.Identifikator);
						slijedi1.Add("]"); slijedi1.Add("of");
						slijedi1.AddRange(_pocetakTipa);
						Provjeri("[",slijedi1,"Oèekivana desna uglata zagrada!");
						PojediZavrsniZnak("[");

						slijedi1.Remove(OznakeZnakovaGramatike.UnsignedInteger);
						slijedi1.Remove(OznakeZnakovaGramatike.Identifikator);
						Index_list(slijedi1,out inListaTipova);

						slijedi1.Remove("]");
						Provjeri("]",slijedi1,"Oèekivana desna uglata zagrada!");
						PojediZavrsniZnak("]");

						Provjeri("of",slijedi,"Oèekivana kljuèna rijeè OF!");
						PojediZavrsniZnak("of");

						Type(slijedi, out inBazniTipl);
						SemantickiAnalizator.KreirajNiz(inListaTipova, inBazniTipl, out iTip);
						#endregion 
						break;
					}
					case "record":
					{
						#region <structured_type> ->  record  <field_list>  end
						PrimijenioProdukciju("<structured_type>", "record  <field_list>  end");
						ICollection inListaPolja;

						KonzumirajZavrsniZnak("record");

						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add("end");
						Field_list(slijedi1,out inListaPolja);

						PojediZavrsniZnak("end");
						SemantickiAnalizator.NapraviStrukturu(inListaPolja, out iTip);
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Pogrešna oznaka tipa!");
						break;
					}
				}
			}
		} 
		private void Index_list(ArrayList slijedi, out ICollection iKonacnaListaIndeksa)
		{	
			iKonacnaListaIndeksa=null;
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add(OznakeZnakovaGramatike.UnsignedInteger);
			zapocinje.Add(OznakeZnakovaGramatike.Identifikator);
			zapocinje.Add("(");
			if (Provjeri(zapocinje,slijedi,"Oèekivana lijeva zagrada, identifikator ili broj!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case OznakeZnakovaGramatike.UnsignedInteger :
					case  OznakeZnakovaGramatike.Identifikator :
					case "(" :
					{
						#region <index_list> ->  <simple_type>  <index_list_1> 
						PrimijenioProdukciju("<index_list>", "<simple_type>  <index_list_1>");
						
						TipPodatka inTip;
						ICollection inListaIndeksa;
						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add(","); slijedi1.Add("]");

						Simple_type(slijedi1,out inTip);
						
						SemantickiAnalizator.StvoriListuIndeksa(inTip, out inListaIndeksa);
						Index_list_1(slijedi, inListaIndeksa, out iKonacnaListaIndeksa);
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Oèekivana lijeva zagrada, identifikator ili broj!"); 
						break;
					}
				}
			}
		}
		private void Index_list_1(ArrayList slijedi,  ICollection nListaIndeksa, out ICollection iKonacnaListaIndeksa)
		{	
			iKonacnaListaIndeksa=null;
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add(","); zapocinje.Add("]");
			if (Provjeri(zapocinje,slijedi,"Oèekivan zarez ili desna uglata zagrada!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case ",":
					{
						#region <index_list_1> ->  ,  <simple_type>  <index_list_1> 
						PrimijenioProdukciju("<index_list>", ",  <simple_type>  <index_list_1>");
						TipPodatka inTip;
						ICollection inDopunjenaListaIndeksa;

						KonzumirajZavrsniZnak(",");
						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add(","); slijedi1.Add("]");
						Simple_type(slijedi1, out inTip);
						SemantickiAnalizator.NadopuniListuIndeksa(inTip,nListaIndeksa, out inDopunjenaListaIndeksa);						
						Index_list_1(slijedi,inDopunjenaListaIndeksa,out iKonacnaListaIndeksa);

						#endregion
						break;
					}
					case "]":
					{
						#region <index_list_1> ->  <empty>
						PrimijenioProdukciju("<index_list>", "<empty>");
						Empty();
						iKonacnaListaIndeksa=nListaIndeksa;
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Oèekivan zarez ili desna uglata zagrada!");
						break;
					}
				}
			}
		}

		private void Field_list(ArrayList slijedi,out ICollection iListaPolja)
		{	
			iListaPolja=null;
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add("end"); 
			zapocinje.Add(OznakeZnakovaGramatike.Identifikator);
			zapocinje.Add(";"); zapocinje.Add("case");
			Provjeri(zapocinje,slijedi,"Nelegalan poèetak zapisa (Field-list)!");
			switch(TrenutniZnak.OznakaZnakaGramatike)
			{
				case "end" :
				case OznakeZnakovaGramatike.Identifikator:
				case ";":
				{
					#region <field_list> ->  <fixed_part> 
					PrimijenioProdukciju("<field_list>", "<fixed_part>");
					Fixed_part(slijedi, out iListaPolja);
					#endregion
					break;
				}

				default:
				{
					PrijaviGresku("Nelegalan poèetak zapisa 1!");
					break;
				}
			}
			Provjeri(slijedi,"Smeæe u zapisu!");
		}
/*		private void Field_list1()
		{
			switch(TrenutniZnak.OznakaZnakaGramatike)
			{
				case ";":
				{
					#region <field_list1> ->  ;  <variant_part> 
					PrimijenioProdukciju("<field_list1> ", ";  <variant_part> ");
					Variant_part();
					#endregion
					break;
				}
				case "end" :
				case ")":
				{
					#region <field_list1> ->  <empty>
					PrimijenioProdukciju("<field_list1> ", ";  <variant_part> ");
					KonzumirajZavrsniZnak(";");
					Variant_part();
					#endregion
					break;
				}
				default:
				{
					FatalnaGreska();
					break;
				}
			}
		}*/
		private void Fixed_part(ArrayList slijedi,out ICollection iListaPolja)
		{	
			iListaPolja=null;
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add("end");
			zapocinje.Add(";");
			zapocinje.Add(OznakeZnakovaGramatike.Identifikator);
			Provjeri(zapocinje,slijedi,"Nelegalan poèetak fiksnog dijela zapisa!");
			switch(TrenutniZnak.OznakaZnakaGramatike)
			{
				case "end" : 
				case ";":
				case OznakeZnakovaGramatike.Identifikator:
				{
					#region <fixed_part> ->  <record_field>  <fixed_part_1> 
					PrimijenioProdukciju("<fixed_part>", "<record_field>  <fixed_part_1>");
					ICollection inListaPolja1, inListaPolja2;
					SemantickiAnalizator.KreirajPraznuListuPolja(out inListaPolja1);
					

					ArrayList slijedi1=new ArrayList(slijedi);
					slijedi1.Add("end"); 
					slijedi1.Add(";");
					Record_field(slijedi1, inListaPolja1, out inListaPolja2);
					Fixed_part_1(slijedi, inListaPolja2, out iListaPolja);
					#endregion
					break;
				}
				default:
				{
					PrijaviGresku("Nelegalan poèetak fiksnog dijela zapisa!");
					break;
				}
			}
			Provjeri(slijedi,"Smeæe u zapisu!");
		}
		private void Fixed_part_1(ArrayList slijedi,ICollection nListaPolja,out ICollection iListaPolja)
		{	
			iListaPolja=null;
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add("end"); 
			zapocinje.Add(";");
			Provjeri(zapocinje,slijedi,"Nelegalni terminator!");
			switch(TrenutniZnak.OznakaZnakaGramatike)
			{
				case "end" :
				{
					#region <fixed_part_1> ->  <empty>  
					PrimijenioProdukciju("<fixed_part_1>", " <empty>");
					Empty();
					iListaPolja=nListaPolja;
					#endregion
					break;
				}
						case ";":
							{
								#region <fixed_part_1> ->  ;  <record_field>  <fixed_part_1>   
								PrimijenioProdukciju("<fixed_part_1>", " ;  <record_field>  <fixed_part_1>");
								ICollection inPostojecaListaPolja2;
								KonzumirajZavrsniZnak(";");
								Record_field(slijedi,nListaPolja, out inPostojecaListaPolja2);
								Fixed_part_1(slijedi,inPostojecaListaPolja2,out iListaPolja);
								#endregion
								break;
							} //BILO JE  PRIJE ZAKOMENTIRANO
				/*case ";":
				{
					#region <fixed_part_1> -> ; <fixed_part_continue>
					PrimijenioProdukciju("<fixed_part_1>","; <fixed_part_continue>");
					KonzumirajZavrsniZnak(";");
					fixed_part_continue(slijedi);
					#endregion
					break;
				}*/

				default:
				{
					PrijaviGresku("Nelegalni terminator!");
					break;
				}
			}
			Provjeri(slijedi,"Smeæe u zapisu!");
		}

		/*private void fixed_part_continue(ArrayList slijedi)
		{
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add("case"); zapocinje.Add(";");
			zapocinje.Add(")"); zapocinje.Add("end");
			zapocinje.Add(OznakeZnakovaGramatike.Identifikator);
			Provjeri(zapocinje,slijedi,"Nelegalni terminator!");
			switch(TrenutniZnak.OznakaZnakaGramatike)
			{
				case "case":
				{
					#region <fixed_part_continue> -> <variant_part>
					PrimijenioProdukciju("<fixed_part_continue>","<variant_part>");
					Variant_part(slijedi);
					#endregion
					break;
				}
				case ";":
				case ")":
				case "end":
				case OznakeZnakovaGramatike.Identifikator:
				{
					#region <fixed-part_continue> ->  <record-field>  <fixed-part_1> 
					PrimijenioProdukciju("<fixed-part_continue>", " <record-field>  <fixed-part_1>");
					ArrayList slijedi1=new ArrayList(slijedi);
					slijedi1.Add("end"); slijedi1.Add(")");
					slijedi1.Add(";");
					Record_field(slijedi1);
					Fixed_part_1(slijedi);
					#endregion
					break;
				}
				default:
				{
					PrijaviGresku("Nelegalni terminator!");
					break;
				}
			}
			Provjeri(slijedi,"Smeæe u zapisu!");
		}*/


		private void Record_field(ArrayList slijedi,ICollection nListaPolja,out ICollection iListaPolja)
		{	
			iListaPolja=null;
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add(";");
			zapocinje.Add("end");
			zapocinje.Add(OznakeZnakovaGramatike.Identifikator);
			Provjeri(zapocinje,slijedi,"Nelegalni terminator!");
			switch(TrenutniZnak.OznakaZnakaGramatike)
			{
				case "end" :			
				case  ";":
				{
					#region <record_field> ->  <empty> 
					PrimijenioProdukciju("<record_field>", "<empty>");
					Empty();
					iListaPolja=nListaPolja;
					#endregion
					break;
				}
				case OznakeZnakovaGramatike.Identifikator :
				{
					#region <record_field> ->  <fieldid_list>  :  <type> 
					PrimijenioProdukciju("<record_field>", "<fieldid_list>  :  <type>");
					ICollection inListaImena;
					TipPodatka inTip;
					ArrayList slijedi1=new ArrayList(slijedi);
					slijedi1.AddRange(_pocetakTipa);
					slijedi1.Add(":");
					Fieldid_list(slijedi1, out inListaImena);
					slijedi1.Remove(":");
					Provjeri(":",slijedi1,"Oèekivana je dvotoèka!");
					PojediZavrsniZnak(":");
					Type(slijedi,out inTip);

					SemantickiAnalizator.NadopuniListuPolja(inListaImena, inTip, nListaPolja, out iListaPolja);

					#endregion
					break;
				}
				default:
				{
					PrijaviGresku("Nelegalni terminator!");
					break;
				}
			}
			Provjeri(slijedi,"Smeæe u zapisu!");
		}
		private void Fieldid_list(ArrayList slijedi,out ICollection iListaImena)
		{
			Provjeri(OznakeZnakovaGramatike.Identifikator,slijedi,"Oèekivan identifikator!");
			iListaImena=null;
			switch(TrenutniZnak.OznakaZnakaGramatike)
			{
				case OznakeZnakovaGramatike.Identifikator :
				{
					#region <fieldid_list> ->  identifier  <fieldid_list_1> 
					PrimijenioProdukciju("<fieldid_list>", "identifier  <fieldid_list_1>");
					string inImeIdentifikatora;
					ICollection inListaImena1;
					KonzumirajZavrsniZnak(OznakeZnakovaGramatike.Identifikator, out inImeIdentifikatora);
					SemantickiAnalizator.KreirajListuImena(inImeIdentifikatora, out inListaImena1);
					Fieldid_list_1(slijedi,inListaImena1, out iListaImena);
					#endregion
					break;

				}
				default:
				{
					PrijaviGresku("Oèekivan identifikator!");
					break;
				}
			}
			Provjeri(slijedi,"Neoèekivani znak!");
		}
		private void Fieldid_list_1(ArrayList slijedi,ICollection nListaImena,out ICollection iListaImena)
		{
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add(","); zapocinje.Add(":");
			iListaImena=null;
			Provjeri(zapocinje,slijedi,"Oèekivan zarez ili dvotoèka!");
			switch(TrenutniZnak.OznakaZnakaGramatike)
			{
				case "," :
				{
					#region <fieldid_list_1> ->  ,  identifier  <fieldid_list_1> 
					PrimijenioProdukciju("<fieldid_list>", ",  identifier  <fieldid_list_1>");
					string inImeIdentifikatora;
					ICollection inListaImena1;

					KonzumirajZavrsniZnak(",");
					ArrayList slijedi1=new ArrayList(slijedi);
					slijedi1.Add(","); slijedi1.Add(":");
					Provjeri(OznakeZnakovaGramatike.Identifikator,slijedi1,"Oèekivan identifikator!");
					PojediZavrsniZnak(OznakeZnakovaGramatike.Identifikator, out inImeIdentifikatora);
					SemantickiAnalizator.NadopuniListuImena(inImeIdentifikatora, nListaImena, out inListaImena1);
					Fieldid_list_1(slijedi,inListaImena1, out iListaImena);


					#endregion
					break;

				}
				case ":" :
				{
					#region <fieldid_list_1> ->  <empty> 
					PrimijenioProdukciju("<fieldid_list>", "<empty>");
					
					Empty();
					iListaImena = nListaImena;
					#endregion
					break;

				}
				default:
				{
					PrijaviGresku("Oèekivan zarez ili dvotoèka!");
					break;
				}
			}
			Provjeri(slijedi,"Neoèekivani znak!");
		}
		#endregion

		#region Marijanov dio

		private void Case_label_list(ArrayList slijedi, TipPodatka nZahtjevaniTip, out ICollection iListaMogucihOdabira)
		{
			iListaMogucihOdabira = null;

			ArrayList zapocinje=_pocetakVarijante;
			if (Provjeri(zapocinje,slijedi,"Greška!"))
			{

				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case "not" :
					case OznakeZnakovaGramatike.UnsignedReal :
					case OznakeZnakovaGramatike.UnsignedInteger :
					case "+" :
					case "(" :
					case OznakeZnakovaGramatike.Identifikator:
					case OznakeZnakovaGramatike.Boolean:
					case "nil": 
					case "@" :
					case "-" :
					
					{	
						#region Produkcija <case-label-list> ->  <constant>  <case-label-list_1>
						PrimijenioProdukciju("<case-label-list>", "<constant>  <case-label-list_1>");
					

						TipPodatka	inTipKonstante;
						UInt32		inVrijednostKonstante;
						ICollection inNovaLista;
						ICollection iKonacnaListaOdabira;
						

						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add(","); slijedi1.Add(":");
						Constant(slijedi1,out inTipKonstante, out inVrijednostKonstante);
						SemantickiAnalizator.StvoriListuOdabira(nZahtjevaniTip,inTipKonstante,inVrijednostKonstante, out inNovaLista);
						Case_Label_list_1(slijedi,nZahtjevaniTip,inNovaLista, out iKonacnaListaOdabira);
					
						#endregion
						break;

					}
					default:
					{
						PrijaviGresku("Greška!");
						break;									
					}
				}
			}
		}


		private void Case_Label_list_1(ArrayList slijedi, TipPodatka nZahtjevaniTip, ICollection nDosadasnjaListaOdabira, out ICollection iKonacnaListaOdabira)
		{
			iKonacnaListaOdabira = null;
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add(":"); zapocinje.Add(",");
			if (Provjeri(zapocinje,slijedi,"Oèekivan zarez ili dvotoèka!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
				
					case ",":
					{
						#region Produkcija <case-label-list_1> ->  ,  <constant>  <case-label-list_1>
						PrimijenioProdukciju("<case-label-list_1>", ",  <constant>  <case-label-list_1>");
					
						TipPodatka inTipKonstante;
						UInt32 inVrijednostKonstante;
						ICollection inDopunjenaListaOdabira;
				
						KonzumirajZavrsniZnak(",");
						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add(","); slijedi1.Add(":");
						Constant(slijedi1, out inTipKonstante, out inVrijednostKonstante);
		
						SemantickiAnalizator.NadopuniListuOdabira(nZahtjevaniTip, inTipKonstante, inVrijednostKonstante, nDosadasnjaListaOdabira, out inDopunjenaListaOdabira);
						Case_Label_list_1(slijedi, nZahtjevaniTip, inDopunjenaListaOdabira, out iKonacnaListaOdabira);
					
						#endregion
						break;
					}
					case ":":
					{
						#region Produkcija <case-label-list_1> ->  <empty>
						PrimijenioProdukciju("<case-label-list_1>", "<empty>");
						
						Empty();
						iKonacnaListaOdabira = nDosadasnjaListaOdabira;					
						#endregion
						break;
					}
					default:
					{

						PrijaviGresku("Oèekivan zarez ili dvotoèka!");
						break;
					}			
				}
			}
		}


		private void Proc_and_func_declaration(ArrayList slijedi)
		{
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add("function"); zapocinje.Add("procedure");
			zapocinje.Add("begin");
			if (Provjeri(zapocinje,slijedi,"Pogreška u bloku!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case "procedure" :
					case "function":
					{
						#region Produkcija <proc-and-func-declaration> ->  <proc-or-func> ; <proc-and-func-declaration_1> 
						PrimijenioProdukciju("<proc-and-func-declaration>", "<proc-or-func> ; <proc-and-func-declaration_1>");
						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add(";"); slijedi1.Add("procedure");
						slijedi1.Add("function"); slijedi1.Add("begin");
						Proc_or_func(slijedi1);
						ArrayList slijedi2=new ArrayList(slijedi);
						slijedi2.Add("procedure");
						slijedi2.Add("function"); slijedi2.Add("begin");
						Provjeri(";",slijedi2,"Oèekivana toèka-zarez (paf)!");
						PojediZavrsniZnak(";");
						Proc_and_func_declaration_1(slijedi);
					
						#endregion
						break;
					}
					default :
					{
						PrijaviGresku("Pogreška u bloku!");
						break;
					}
				}		
			}
		}

		private void Proc_and_func_declaration_1(ArrayList slijedi)
		{
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add("function"); zapocinje.Add("procedure");
			zapocinje.Add("begin");
			if (Provjeri(zapocinje,slijedi,"Pogreška u bloku!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
				
					case "procedure":
					case "function" :
					{
						#region Produkcija <proc-and-func-declaration_1> ->    <proc-or-func> ; <proc-and-func-declaration_1>
						PrimijenioProdukciju("<proc-and-func-declaration_1>", "<proc-or-func> ; <proc-and-func-declaration_1>");
						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add(";"); slijedi1.Add("procedure");
						slijedi1.Add("function");
						
						Proc_or_func(slijedi1);
						
						ArrayList slijedi2=new ArrayList(slijedi);
						slijedi2.Add("procedure");
						slijedi2.Add("function"); slijedi2.Add("begin");
						Provjeri(";",slijedi2,"Oèekivana toèka-zarez(paf1)!");
						PojediZavrsniZnak(";");
						
						Proc_and_func_declaration_1(slijedi);
					
						#endregion
						break;
					}
					case OznakeZnakovaGramatike.KrajUlaznogNiza:
					case "begin":
					{
						#region Produkcija <proc-and-func-declaration_1> ->  <empty>
						PrimijenioProdukciju("<proc-and-func-declaration_1>", "<empty>");

						Empty();				
						#endregion
						break;
					}
					
					default:
					{
						PrijaviGresku("Pogreška u bloku!");
						break;
					}			
				}
			}
		}


		private void Proc_or_func(ArrayList slijedi)
		{
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add("procedure");
			zapocinje.Add("function");
			if (Provjeri(zapocinje,slijedi,"Oèekivana funkcija ili procedura!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
				
					case "procedure":
					{
						#region Produkcija <proc-or-func> ->  procedure  identifier  <proc-or-func1>
						PrimijenioProdukciju("<proc-or-func>", "procedure  identifier  <proc-or-func1>");

						string inIdentifikator;
					
						KonzumirajZavrsniZnak("procedure");
						
						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add("("); slijedi1.Add(";");
						Provjeri(OznakeZnakovaGramatike.Identifikator,slijedi1,"Oèekivan identifikator!");
						
						PojediZavrsniZnak(OznakeZnakovaGramatike.Identifikator, out inIdentifikator);
						
						Proc_or_func1(slijedi, inIdentifikator);
					
						#endregion
						break;
					}
					case "function":
					{
						#region Produkcija <proc-or-func> ->  function  identifier  <proc-or-func2>
						PrimijenioProdukciju("<proc-or-func>", "function  identifier  <proc-or-func2>");

						string inIdentifikator;
						
						KonzumirajZavrsniZnak("function");
						
						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add("("); slijedi1.Add(";");
						Provjeri(OznakeZnakovaGramatike.Identifikator,slijedi1,"Oèekivan identifikator!");
						
						PojediZavrsniZnak(OznakeZnakovaGramatike.Identifikator, out inIdentifikator);
						
						Proc_or_func2(slijedi, inIdentifikator);

						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Oèekivana funkcija ili procedura!");
						break;
					}			
				}
			}
		}


		private void Proc_or_func1(ArrayList slijedi, string nIdentifikator)
		{
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add("("); zapocinje.Add(";");
			if (Provjeri(zapocinje,slijedi,"Oèekivana lijeva zagrada ili toèka-zarez!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
				
					case "(":
					{
						#region Produkcija <proc-or-func1> ->  <parameters>  ;  <block-or-forward>
						PrimijenioProdukciju("<proc-or-func1>", "<parameters>  ;  <block-or-forward>");
						ICollection inListaParametara;

						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add(";"); slijedi1.Add("procedure" );
						slijedi1.Add("function" ); 	slijedi1.Add("var" );
						slijedi1.Add("begin" ); slijedi1.Add("type" );
						slijedi1.Add("const" ); slijedi1.Add("label" );
						slijedi1.Add("forward");
		
						Parameters(slijedi1, out inListaParametara);
						slijedi1=new ArrayList(slijedi);
						slijedi1.Add("procedure" );
						slijedi1.Add("function" ); 	slijedi1.Add("var" );
						slijedi1.Add("begin" ); slijedi1.Add("type" );
						slijedi1.Add("const" ); slijedi1.Add("label" );
						slijedi1.Add("forward");
						Provjeri(";",slijedi,"Oèekivana toèka-zarez!");
						PojediZavrsniZnak(";");

						SemantickiAnalizator.ProceduraDeklarirana(nIdentifikator,inListaParametara);

						Block_or_forward(slijedi,nIdentifikator);
					
						#endregion
						break;
					}
					case ";":
					{
						#region Produkcija <proc-or-func1> ->  ;  <block-or-forward>
						PrimijenioProdukciju("<proc-or-func1>", ";  <block-or-forward>");
						ICollection inListaParametara;

						KonzumirajZavrsniZnak(";");
						SemantickiAnalizator.KreirajPraznuListuPolja(out inListaParametara);
						SemantickiAnalizator.ProceduraDeklarirana(nIdentifikator,inListaParametara);
						Block_or_forward(slijedi, nIdentifikator);
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Oèekivana lijeva zagrada ili toèka-zarez!");
						break;
					}			
				}
			}
		}



		private void Proc_or_func2(ArrayList slijedi, string nIdentifikator)
		{
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add("("); zapocinje.Add(";");
			if (Provjeri(zapocinje,slijedi,"Oèekivana lijeva zagrada ili toèka-zarez!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
				
					case "(":
					{
						#region Produkcija <proc-or-func2> ->  <parameters>  :  <typeid>  ;  <block-or-forward>
						PrimijenioProdukciju("<proc-or-func2>", "<parameters>  :  <typeid>  ;  <block-or-forward>");
						ICollection inListaParametara;
						TipPodatka inTip;
					
						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add(OznakeZnakovaGramatike.Identifikator);
						slijedi1.Add(";"); slijedi1.Add("procedure" );
						slijedi1.Add("function" ); 	slijedi1.Add("var" );
						slijedi1.Add("begin" ); slijedi1.Add("type" );
						slijedi1.Add("const" ); slijedi1.Add("label" );
						slijedi1.Add("forward"); slijedi1.Add(":");

						Parameters(slijedi1, out inListaParametara);
						
						slijedi1=new ArrayList(slijedi);
						slijedi1.Add(OznakeZnakovaGramatike.Identifikator);
						slijedi1.Add(";"); slijedi1.Add("procedure" );
						slijedi1.Add("function" ); 	slijedi1.Add("var" );
						slijedi1.Add("begin" ); slijedi1.Add("type" );
						slijedi1.Add("const" ); slijedi1.Add("label" );
						slijedi1.Add("forward");

						Provjeri(":",slijedi1,"Oèekivana tvotoèka!");
						PojediZavrsniZnak(":");
						
						slijedi1=new ArrayList(slijedi);
						slijedi1.Add(";"); slijedi1.Add("procedure" );
						slijedi1.Add("function" ); 	slijedi1.Add("var" );
						slijedi1.Add("begin" ); slijedi1.Add("type" );
						slijedi1.Add("const" ); slijedi1.Add("label" );
						slijedi1.Add("forward");
						
						Typeid(slijedi, out inTip);
						SemantickiAnalizator.FunkcijaDeklarirana(nIdentifikator, inListaParametara, inTip);
						Provjeri(";",slijedi,"Oèekivana toèka-zarez (por2)!");
						PojediZavrsniZnak(";");
						Block_or_forward(slijedi,nIdentifikator);
						#endregion
						break;
					}
					case ":":
					{
						#region Produkcija <proc-or-func2> ->  : <typeid>  ;  <block-or-forward>
						PrimijenioProdukciju("<proc-or-func2>", ": <typeid>  ;  <block-or-forward>");
						ICollection inListaParametara;
						TipPodatka inTip;

						KonzumirajZavrsniZnak(":");
						SemantickiAnalizator.KreirajPraznuListuPolja(out inListaParametara);

						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add(";"); slijedi1.Add("procedure" );
						slijedi1.Add("function" ); 	slijedi1.Add("var" );
						slijedi1.Add("begin" ); slijedi1.Add("type" );
						slijedi1.Add("const" ); slijedi1.Add("label" );
						slijedi1.Add("forward");

						Typeid(slijedi1, out inTip);				
						SemantickiAnalizator.FunkcijaDeklarirana(nIdentifikator, inListaParametara, inTip);

						slijedi1=new ArrayList(slijedi);
						slijedi1.Add("procedure" );
						slijedi1.Add("function" ); 	slijedi1.Add("var" );
						slijedi1.Add("begin" ); slijedi1.Add("type" );
						slijedi1.Add("const" ); slijedi1.Add("label" );
						slijedi1.Add("forward");
						Provjeri(";",slijedi1,"Oèekivana toèka-zarez (por2)");
						PojediZavrsniZnak(";");

						Block_or_forward(slijedi, nIdentifikator);
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Oèekivana lijeva zagrada ili toèka-zarez!");
						break;
					}			
				}
			}
		}


		private void Block_or_forward(ArrayList slijedi, string nIdentifikator)
		{
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add("procedure" );
			zapocinje.Add("function" ); 	zapocinje.Add("var" );
			zapocinje.Add("begin" ); zapocinje.Add("type" );
			zapocinje.Add("const" ); zapocinje.Add("label" );
			zapocinje.Add("forward");

			if (Provjeri(zapocinje,slijedi,"Greška u bloku potprograma!"))
			{

				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case "procedure" :
					case "function" :
					case "var" :
					case "begin" :
					case "type" :
					case "const" :
					case "label" :
					{
						#region Produkcija <block-or-forward> ->  <block> 
						PrimijenioProdukciju("<block-or-forward>", "<block>");
						bool nImplementirano = true;

						SemantickiAnalizator.OznaciImplementaciju(nIdentifikator, nImplementirano);
						Block(slijedi, nIdentifikator);
						#endregion
						break;

					}

				
					case "forward":
					{
						#region Produkcija <block-or-forward> ->  forward 
						PrimijenioProdukciju("<block-or-forward>", "forward");
						bool nImplementirano = false;
					
						SemantickiAnalizator.OznaciImplementaciju(nIdentifikator, nImplementirano);
						KonzumirajZavrsniZnak("forward");					
						#endregion
						break;
					}

					
					default:
					{

						PrijaviGresku("Greška u bloku potprograma!");
						break;
					}			
				}
			}
		}
		
		private void Parameters(ArrayList slijedi, out ICollection iListaParametara)
		{
			iListaParametara = null;

			if (Provjeri("(",slijedi,"Oèekivana lijeva zagrada!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
				
					case "(":
					{
						#region Produkcija <parameters> ->  (  <formal-parameter-list>  )
						PrimijenioProdukciju("<parameters>", "(  <formal-parameter-list>  )");
														
						KonzumirajZavrsniZnak("(");
						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add(")");

						Formal_parameter_list(slijedi1, out iListaParametara);
						Provjeri(")",slijedi,"Oèekivana desna zagrada!");
						PojediZavrsniZnak(")");
						#endregion
						break;
					}
				
					default:
					{
						PrijaviGresku("Oèekivana lijeva zagrada!");
						break;
					}			
				}
			}
		}

		private void Formal_parameter_list(ArrayList slijedi, out ICollection iListaFormalnihParametara)
		{
			iListaFormalnihParametara = null;

			ArrayList zapocinje=new ArrayList();
			zapocinje.Add(OznakeZnakovaGramatike.Identifikator);
			if (Provjeri(zapocinje,slijedi,"Greška u parametrima!"))
			{

				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case OznakeZnakovaGramatike.Identifikator :
					{
						#region Produkcija <formal-parameter-list> ->  <formal-parameter-section>  <formal-parameter-list_1> 
						PrimijenioProdukciju("<formal-parameter-list>", "<formal-parameter-section>  <formal-parameter-list_1>");
						ICollection inListaImena;
						TipPodatka inTip;
						ICollection inNovaLista;

						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add(")"); slijedi1.Add(";");
					
						Formal_parameter_section(slijedi1, out inListaImena, out inTip);
						SemantickiAnalizator.KreirajListuPolja(inListaImena, inTip, out inNovaLista);
						Formal_parameter_list_1(slijedi, inNovaLista, out iListaFormalnihParametara);					
						#endregion
						break;
					}
					default :
					{
						PrijaviGresku("Greška u parametrima!");
						break;
					}
				}			
			}
		}

		private void Formal_parameter_list_1(ArrayList slijedi, ICollection nDosadasnjaLista, out ICollection iKonacnaLista)
		{
			iKonacnaLista = null;

			ArrayList zapocinje=new ArrayList();
			zapocinje.Add(";"); zapocinje.Add(")");
			if (Provjeri(zapocinje,slijedi,"Oèekivana desna zagrada ili toèka-zarez!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
				
					case ";":
					{
						#region Produkcija <formal-parameter-list_1> ->  ;  <formal-parameter-section>  <formal-parameter-list_1>
						PrimijenioProdukciju("<formal-parameter-list_1>", ";  <formal-parameter-section>  <formal-parameter-list_1>");
						ICollection inListaImena;
						TipPodatka inTip;
						ICollection inListaPolja;

						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add(")"); slijedi1.Add(";");

						KonzumirajZavrsniZnak(";");
						Formal_parameter_section(slijedi1, out inListaImena, out inTip);
						SemantickiAnalizator.NadopuniListuPolja(inListaImena, inTip, nDosadasnjaLista,  out inListaPolja);
						Formal_parameter_list_1(slijedi, inListaPolja, out iKonacnaLista);					
						#endregion
						break;
					}
					case ")":
					{
						#region Produkcija <formal-parameter-list_1> ->  <empty>
						PrimijenioProdukciju("<formal-parameter-list_1>", "<empty>");
						
						Empty();
						iKonacnaLista = nDosadasnjaLista;
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Oèekivana desna zagrada ili toèka-zarez!");
						break;
					}			
				}
			}
		}

		private void Formal_parameter_section(ArrayList slijedi, out ICollection iListaImena, out TipPodatka iTip)
		{
			iListaImena = null;
			iTip = null;

			ArrayList zapocinje=new ArrayList();
			zapocinje.Add(OznakeZnakovaGramatike.Identifikator);
			zapocinje.Add("var"); zapocinje.Add("procedure");
			zapocinje.Add("function");
			if (Provjeri(zapocinje,slijedi,"Greèka u parametrima!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
				
					case OznakeZnakovaGramatike.Identifikator:
					{
						#region Produkcija <formal-parameter-section> ->  <parameterid-list>  :  <typeid>
						PrimijenioProdukciju("<formal-parameter-section>", "<parameterid-list>  :  <typeid>");

						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add(":"); 
						slijedi1.Add(OznakeZnakovaGramatike.Identifikator);

						Parameterid_list(slijedi1, out iListaImena);

						slijedi1=new ArrayList(slijedi);
						slijedi1.Add(":"); 
						Provjeri(":",slijedi1,"Oèekivana tvotoèka!");
						PojediZavrsniZnak(":");

						Typeid(slijedi, out iTip);				
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Greška u parametrima!");
						break;
					}			
				}
			}
		}



		private void Parameterid_list(ArrayList slijedi, out ICollection iListaImena)
		{
			iListaImena = null;

			if (Provjeri(OznakeZnakovaGramatike.Identifikator,slijedi,"Oèekivan identifikator!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
				
					case OznakeZnakovaGramatike.Identifikator:
					{
						#region Produkcija <parameterid-list> ->  identifier  <parameterid-list_1>
						PrimijenioProdukciju("<parameterid-list>", "identifier  <parameterid-list_1>");						
						string inImeIdentifikatora;
						ICollection inListaImena1;

						KonzumirajZavrsniZnak(OznakeZnakovaGramatike.Identifikator, out inImeIdentifikatora);
						SemantickiAnalizator.KreirajListuImena(inImeIdentifikatora, out inListaImena1);
						Parameterid_list_1(slijedi, inListaImena1, out iListaImena);
						#endregion
						break;
					}
				
					default:
					{
						PrijaviGresku("Oèekivan identifikator!");
						break;
					}			
				}
			}
		}


		private void Parameterid_list_1(ArrayList slijedi, ICollection nListaImena, out ICollection iListaImena)
		{
			iListaImena = null;
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add(","); zapocinje.Add(":");
			if (Provjeri(zapocinje,slijedi,"Oèekivan zarez ili dvotoèka!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
				
					case ",":
					{
						#region Produkcija <parameterid-list_1> ->  ,  identifier  <parameterid-list_1>
						PrimijenioProdukciju("<parameterid-list_1>", ",  identifier  <parameterid-list_1>");
						ICollection inListaImena1;
						string inImeIdentifikatora;
					
						KonzumirajZavrsniZnak(",");
						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.AddRange(zapocinje);
						Provjeri(OznakeZnakovaGramatike.Identifikator,slijedi1,"Oèekivan identifikator!");
						PojediZavrsniZnak(OznakeZnakovaGramatike.Identifikator, out inImeIdentifikatora);

						SemantickiAnalizator.NadopuniListuImena(inImeIdentifikatora, nListaImena, out inListaImena1);
						Parameterid_list_1(slijedi, inListaImena1, out iListaImena);					
						#endregion
						break;
					}
					case ":":
					{
						#region Produkcija <parameterid-list_1> ->  <empty>
						PrimijenioProdukciju("<parameterid-list_1>", "<empty>");
						
						Empty();
						iListaImena = nListaImena;
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Oèekivan zarez ili dvotoèka!");
						break;
					}			
				}
			}
		}



		private void Statement_List(ArrayList slijedi)
		{
			ArrayList zapocinje=new ArrayList(_pocetakNaredbe);
			zapocinje.Add("end"); zapocinje.Add(";");
			zapocinje.Add("until");
			zapocinje.Add(OznakeZnakovaGramatike.Identifikator);
			zapocinje.Add(OznakeZnakovaGramatike.UnsignedInteger);
			if (Provjeri(zapocinje,slijedi,"Nelegalan poèetak naredbe!"))
			{

				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case "goto":
					case OznakeZnakovaGramatike.Identifikator:
					case OznakeZnakovaGramatike.UnsignedInteger:
					case "while":
					case "begin" :
					case "repeat":					
					case ";":
					case "case":
					case "for":
					case "if":
					case "until" :
					case "end":
					{
						#region Produkcija <statement-list> ->  <statement>  <statement-list_1>
						PrimijenioProdukciju("<statement-list>", "<statement>  <statement-list_1>");
					
						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add("end"); slijedi1.Add("until");
						slijedi1.Add(";");
						//slijedi1.AddRange(pocetakNaredbe1);
						ArrayList slijedi3=new ArrayList(slijedi);
						slijedi3.AddRange(_pocetakNaredbe1);
						Statement(slijedi1);
						Statement_list_1(slijedi3);
						#endregion
						break;
					}
					default :
					{
						PrijaviGresku("Nelegalan poèetak naredbe!");
						break;
					}
				}			
			}
		}


		private void Statement_list_1(ArrayList slijedi)
		{
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add("until"); zapocinje.Add("end");
			zapocinje.Add(";");
			Provjeri(zapocinje,slijedi,"Nelegalan kraj bloka naredbi!");
		{
			switch(TrenutniZnak.OznakaZnakaGramatike)
			{
				case "end" :
				case "until" :
				{
					#region Produkcija <statement-list_1> ->  <empty> 
					PrimijenioProdukciju("<statement-list_1>", "<empty> ");
					
					Empty();				
					#endregion
					break;

				}
				case ";":
				{
					#region Produkcija <statement-list_1> ->  ;  <statement>  <statement-list_1> 
					PrimijenioProdukciju("<statement-list_1>", ";  <statement>  <statement-list_1>");
					ArrayList slijedi0=new ArrayList(slijedi);
					slijedi0.AddRange(_pocetakIzraza1);
					//slijedi1.Add("end"); slijedi1.Add("until");
					//slijedi1.Add(";");
					Provjeri(";",slijedi0,"Oèekivana toèka-zarez!");
					PojediZavrsniZnak(";");
					ArrayList slijedi1=new ArrayList(slijedi);
					slijedi1.Add("end"); slijedi1.Add("until");
					slijedi1.Add(";");
					Statement(slijedi1);
					Statement_list_1(slijedi);
					#endregion
					break;
				}					
				default:
				{
					PrijaviGresku("Nelegalan kraj bloka naredbi 1!");
					break;
				}			
			}
		}
		}	

		#endregion	
								
		#region Zeljkov dio
		private void Statement(ArrayList slijedi)
		{
			ArrayList zapocinje=new ArrayList(_pocetakNaredbe1);
			if (Provjeri(zapocinje,slijedi,"Nelegalan poèetak naredbe!"))
			{
				switch (TrenutniZnak.OznakaZnakaGramatike)
				{	
					case OznakeZnakovaGramatike.KrajUlaznogNiza:
					case "end":
					case ";":
					case "else":
					case "until":
					{
						#region Produkcija <statement> ->  <empty> 
						PrimijenioProdukciju("<statement>", "<empty>");
						
						Empty();
						
						#endregion					
						break;
					}
					case OznakeZnakovaGramatike.Identifikator:
					{
						#region Produkcija <statement> ->  <variable-or-procid> <statement1>
						PrimijenioProdukciju("<statement>", "<variable-or-procid> <statement1>");
						
						string inImeIdentifikatora;
						
						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add("end"); slijedi1.Add(";");
						slijedi1.Add("else"); slijedi1.Add("until");

						Variable_or_procid(slijedi1, out inImeIdentifikatora);
						Statement1(slijedi, inImeIdentifikatora);

						#endregion
						break;
					}
			
					case "begin":
					{
						#region Produkcija <statement> ->  begin  <statement-list>  end 
						PrimijenioProdukciju("<statement>", "begin  <statement-list>  end");
						
						KonzumirajZavrsniZnak("begin");
						
						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add("end");
						
						Statement_List(slijedi1);
						
						Provjeri("end",slijedi,"Oèekivana kljuèna rijeè END!");
						PojediZavrsniZnak("end");
						
						#endregion
						break;
					}
					case "if":
					{
						#region Produkcija <statement> ->  if  <expression>  then  <statement>  <statement_if> 
						PrimijenioProdukciju("<statement>","if <expression>  then  <statement>  <statement_if>");
						
						TipPodatka inTipIzraza;
						UInt32 inVrijednostIzraza;
						bool inSveKonstantno;
						int inOznakaNaredbe;

						KonzumirajZavrsniZnak("if");
						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add("then"); //slijedi1.AddRange(pocetakNaredbe1);
						
						Expression(slijedi1, out inTipIzraza, out inVrijednostIzraza, out inSveKonstantno);
	
						SemantickiAnalizator.ObradiIfNaredbu(inTipIzraza, inVrijednostIzraza, inSveKonstantno, out inOznakaNaredbe);

						slijedi1=new ArrayList(slijedi);
						slijedi1.AddRange(_pocetakNaredbe1);
						                        
						Provjeri("then",slijedi1,"Oèekivana kljuèna rijeè THEN!");
						PojediZavrsniZnak("then");
						slijedi1=new ArrayList(slijedi);
						slijedi1.AddRange(_pocetakNaredbe);
						slijedi1.Add("end"); slijedi1.Add(";");
						slijedi1.Add("else"); slijedi1.Add("until");
						
						Statement(slijedi1);

						Statement_if(slijedi, inOznakaNaredbe);
						#endregion
						break;
					}
					case "case":
					{
						#region Produkcija <statement> ->  case  <expression>  of  <case-list>  end 
						PrimijenioProdukciju("<statement>","case  <expression>  of  <case-list>  end");
						
						TipPodatka inTipIzraza;
						int inOznakaCaseNaredbe;
						ICollection inListaSlucajeva;
						UInt32 inVrijednostIzraza;
						bool inSveKonstantno;

						KonzumirajZavrsniZnak("case");
						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add("end");
						slijedi1.Add("not");
						slijedi1.Add(OznakeZnakovaGramatike.UnsignedInteger);
						slijedi1.Add(OznakeZnakovaGramatike.Identifikator);
						slijedi1.Add(OznakeZnakovaGramatike.UnsignedReal);
						slijedi1.Add(OznakeZnakovaGramatike.String);
						slijedi1.Add("+"); slijedi1.Add("nil");
						slijedi1.Add("-"); slijedi1.Add("[");
						ArrayList slijedi2=new ArrayList(slijedi1);
						slijedi1.Add("of");
	
						Expression(slijedi1, out inTipIzraza, out inVrijednostIzraza, out inSveKonstantno);
						
						SemantickiAnalizator.ZapocniCaseNaredbu(inTipIzraza, out inOznakaCaseNaredbe);
				
						Provjeri("of",slijedi2,"Oèekivana kljuèna rijeè OF!");
						PojediZavrsniZnak("of");
						slijedi1=new ArrayList(slijedi);
						slijedi1.Add("end");
						
						Case_List(slijedi1, inTipIzraza, inOznakaCaseNaredbe, out inListaSlucajeva);
						Provjeri("end",slijedi,"Oèekivana kljuèna rijeè END!");
						PojediZavrsniZnak("end");

						SemantickiAnalizator.ZavrsiCaseNaredbu(inListaSlucajeva, inOznakaCaseNaredbe, inTipIzraza, inVrijednostIzraza, inSveKonstantno);
						
						#endregion
						break;
					}
				
					case "while":
					{
						#region Produkcija <statement> ->  while  <expression>  do  <statement> 
						PrimijenioProdukciju("<statement>","while  <expression>  do  <statement>");
						
						TipPodatka inTipIzraza;
						UInt32 inVrijednostIzraza;
						bool inSveKonstantno;
						int inOznakaWhileNaredbe;

						SemantickiAnalizator.ZapocniWhileNaredbu(out inOznakaWhileNaredbe);

						KonzumirajZavrsniZnak("while");
						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.AddRange(_pocetakNaredbe1);
						ArrayList slijedi2=new ArrayList(slijedi1);
						slijedi1.Add("do");
						
						Expression(slijedi1, out inTipIzraza, out inVrijednostIzraza, out inSveKonstantno);
						SemantickiAnalizator.ProvjeriUvjetWhileNaredbe(inTipIzraza, inVrijednostIzraza, inSveKonstantno, inOznakaWhileNaredbe);
						
						Provjeri("do",slijedi2,"Oèekivana kljuèna rijeè DO!");
						PojediZavrsniZnak("do");

						Statement(slijedi);
	
						SemantickiAnalizator.ZavrsiWhileNaredbu(inOznakaWhileNaredbe);
						
						#endregion
						break;
					}

					case "repeat":
					{
						#region Produkcija <statement> ->  repeat  <statement-list>  until  <expression> 
						PrimijenioProdukciju("<statement>","repeat  <statement-list>  until  <expression>");

						TipPodatka inTipIzraza;
						UInt32 inVrijednostIzraza;
						bool inSveKonstantno;
						int inOznakaRepeatNaredbe;
						
						SemantickiAnalizator.ZapocniRepeatNaredbu(out inOznakaRepeatNaredbe);

						KonzumirajZavrsniZnak("repeat");
						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.AddRange(_pocetakIzraza);
						slijedi1.Add("until");
						
						Statement_List(slijedi1);
						
						slijedi1=new ArrayList(slijedi);
						slijedi1.AddRange(_pocetakIzraza);
						
						Provjeri("until",slijedi1,"Oèekivana kljuèna rijeè until!");
						PojediZavrsniZnak("until");

						Expression(slijedi, out inTipIzraza, out inVrijednostIzraza, out inSveKonstantno);					
						
						SemantickiAnalizator.ZavrsiRepeatNaredbu(inTipIzraza, inVrijednostIzraza, inSveKonstantno, inOznakaRepeatNaredbe);

						#endregion
						break;
					}

					case "for":
					{
						#region Produkcija <statement> -> for  <varid>  :=  <for-list>  do  <statement>  
						PrimijenioProdukciju("<statement>","for  <varid>  :=  <for-list>  do  <statement>");
						
						string inOznakaIdentifikatora;
						int inOznakaForNaredbe;
						bool inRastucaPetlja;
						
						KonzumirajZavrsniZnak("for");
						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add("do"); slijedi1.Add("not");
						slijedi1.Add(OznakeZnakovaGramatike.UnsignedInteger);
						slijedi1.Add(OznakeZnakovaGramatike.Identifikator);
						slijedi1.Add(OznakeZnakovaGramatike.UnsignedReal);
						slijedi1.Add(OznakeZnakovaGramatike.String);
						slijedi1.Add("+"); slijedi1.Add("nil");
						slijedi1.Add("-"); slijedi1.Add("[");
						ArrayList slijedi2=new ArrayList(slijedi1);
						slijedi1.Add(":=");
						
						Varid(slijedi1, out inOznakaIdentifikatora);

						Provjeri(":=",slijedi2,"Oèekivan znak pridruživanja!");
						PojediZavrsniZnak(":=");

						slijedi1=new ArrayList(slijedi);
						slijedi1.AddRange(_pocetakNaredbe1);
						slijedi1.Add("do");
						
						For_list(slijedi1, inOznakaIdentifikatora, out inOznakaForNaredbe, out inRastucaPetlja);

						slijedi1=new ArrayList(slijedi);
						slijedi1.AddRange(_pocetakNaredbe1);
						Provjeri("do",slijedi1,"Oèekivana kljuèna rijeè DO!");
						PojediZavrsniZnak("do");

						Statement(slijedi);
						SemantickiAnalizator.ZavrsiForNaredbu(inOznakaForNaredbe, inRastucaPetlja);
						#endregion
						break;
					}
					case "goto":
					{
						#region Produkcija <statement> ->  goto  <label>
						PrimijenioProdukciju("<statement>","goto  <label>");

						UInt32 inVrijednostLabele;

						KonzumirajZavrsniZnak("goto");
						
						Label(slijedi, out inVrijednostLabele);

						SemantickiAnalizator.Goto(inVrijednostLabele);

						#endregion
						break;
					}
					case OznakeZnakovaGramatike.UnsignedInteger:
					{
						#region Produkcija <statement> ->  <label>  :  <statement>  
						PrimijenioProdukciju("<statement>","<label>  :  <statement>");

						UInt32 inVrijednostLabele;

						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.AddRange(_pocetakNaredbe1);
						ArrayList slijedi2=new ArrayList(slijedi1);
						slijedi1.Add(":");
						
						Label(slijedi1, out inVrijednostLabele);
						SemantickiAnalizator.PostaviLabelu(inVrijednostLabele);
                        
						Provjeri(":",slijedi2,"Oèekivana dvotoèka!");
						KonzumirajZavrsniZnak(":");

						Statement(slijedi);
						#endregion
						break;
					}
								
					default:
					{
						PrijaviGresku("Nelegalan poèetak naredbe!");
						break;
					}
				}
			}
		}

		private void Statement_if(ArrayList slijedi, int nOznakaIfNaredbe)
		{
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add("end"); zapocinje.Add(";");
			zapocinje.Add("until"); zapocinje.Add("else");
			if (Provjeri(zapocinje,slijedi,"Nelegalan kraj naredbe!"))
			{
				switch (TrenutniZnak.OznakaZnakaGramatike)
				{	
					case OznakeZnakovaGramatike.KrajUlaznogNiza:
					case "end":
					case ";":
					/* case "else": 
					 * !!!! OVAJ ELSE JE ISKOMENTIRAN KAKO BI SE RIJESIO TZV.
					 * DANGLING-ELSE PROBLEM. Ovo je trajno rjesenje koje svaku
					 * else naredbu veze uz "blizi" if
					*/
					case "until":
					{
						#region Produkcija <statement_if> ->  <empty> 
					
						PrimijenioProdukciju("<statement_if>", "<empty");
						
						SemantickiAnalizator.ZavrsiIfNaredbu(nOznakaIfNaredbe);
						Empty();
					
						#endregion					
						break;
					}
					case "else":
					{
						#region Produkcija <statement_if> ->  else  <statement>
						PrimijenioProdukciju("<statement_if>", "else  <statement>");

						SemantickiAnalizator.ObradiElseNaredbu(nOznakaIfNaredbe);
						
						KonzumirajZavrsniZnak("else");
						
						Statement(slijedi);
						SemantickiAnalizator.ZavrsiElseNaredbu(nOznakaIfNaredbe);
					
						#endregion					
						break;
					}
					default:
					{
						PrijaviGresku("Nelegalan kraj naredbe!");
						break;
					}
				}
			}
		}

		private void Statement1(ArrayList slijedi, string nImeIdentifikatora)
		{
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add("("); zapocinje.Add("^");
			zapocinje.Add("."); zapocinje.Add(":=");
			zapocinje.Add("["); zapocinje.Add("end");
			zapocinje.Add(";"); zapocinje.Add("else");
			zapocinje.Add("until");
			zapocinje.Add("="); zapocinje.Add(":");
			if (Provjeri(zapocinje,slijedi,"Nelegalan nastavak naredbe!"))
			{
				switch (TrenutniZnak.OznakaZnakaGramatike)
				{	
				
					case "(":
					{
						#region Produkcija <statement1> ->  (  <expression-list>  )
						PrimijenioProdukciju("<statement1>", "(  <expression-list>  )");
						
						SemantickiAnalizator.PripremiPozivPotprograma(nImeIdentifikatora);
						
						KonzumirajZavrsniZnak("(");
						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add(")");
						
						Expression_list(slijedi1, nImeIdentifikatora);
						
						Provjeri(")",slijedi,"Oèekivana desna zagrada!");
						PojediZavrsniZnak(")");
						SemantickiAnalizator.PozoviProceduru(nImeIdentifikatora);

						#endregion					
						break;
					}
					case "^":
					case ".":
					case ":=":
					case"[":
					{
						#region Produkcija <statement1> ->  <variable-suffix> := <expression>
						PrimijenioProdukciju("<statement1>"," <variable-suffix> := <expression>");
						
						TipPodatka inTrenutniTipVarijable;
						VrstaVarijable inTrenutnaVrstaVarijable;
						UInt32 inTrenutnaAdresa;
						TipPodatka inTipVarijable;
						VrstaVarijable inVrstaVarijable;
						UInt32 inAdresa;
						TipPodatka inTipIzraza;
						UInt32 inVrijednostIzraza;
						bool inSveKonstantno;
							
						SemantickiAnalizator.IzracunajVarijabluIzIdentifikatora(nImeIdentifikatora, out inTrenutniTipVarijable, out inTrenutnaVrstaVarijable, out inTrenutnaAdresa);
						
						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.AddRange(_pocetakIzraza);
						ArrayList slijedi2=new ArrayList(slijedi1);
						slijedi1.Add(":=");
						
						Variable_suffix(slijedi1, inTrenutniTipVarijable, inTrenutnaVrstaVarijable, inTrenutnaAdresa, out inTipVarijable, out inVrstaVarijable, out inAdresa);
						
						Provjeri(":=",slijedi2,"Oèekivan znak pridruživanja!");
						PojediZavrsniZnak(":=");
						
						Expression(slijedi, out inTipIzraza, out inVrijednostIzraza, out inSveKonstantno);
												
						SemantickiAnalizator.ObaviPridruzivanje(inTipVarijable, inVrstaVarijable, inAdresa, inTipIzraza, inVrijednostIzraza, inSveKonstantno);

						#endregion
						break;
					}
					case OznakeZnakovaGramatike.KrajUlaznogNiza:
					case "end":
					case ";":
					case "else":
					case "until":
					{
						#region Produkcija <statement1> ->  <empty> 
					
						PrimijenioProdukciju("<statement1>", "<empty>");
						
						Empty();
						SemantickiAnalizator.PripremiPozivPotprograma(nImeIdentifikatora);
						SemantickiAnalizator.PozoviProceduru(nImeIdentifikatora);
					
						#endregion					
						break;
					}
					default:
					{
						PrijaviGresku("Nelegalan nastavak naredbe!");
						break;
					}
					
				}
			}
		}

		private void Subscript_list(ArrayList slijedi, TipPodatka nTipNiza)
		{
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add("@"); zapocinje.Add("not");
			zapocinje.Add(OznakeZnakovaGramatike.UnsignedReal);
			zapocinje.Add(OznakeZnakovaGramatike.Boolean);
			zapocinje.Add("+");
			zapocinje.Add(OznakeZnakovaGramatike.Identifikator);
			zapocinje.Add("nil"); zapocinje.Add("-");
			zapocinje.Add(OznakeZnakovaGramatike.UnsignedInteger);
			if (Provjeri(zapocinje,slijedi,"Greška u subscript-u!"))
			{
				switch (TrenutniZnak.OznakaZnakaGramatike)
				{	
					case "not":
					case OznakeZnakovaGramatike.UnsignedReal:
					case OznakeZnakovaGramatike.UnsignedInteger:
					case "+":
					case "(":
					case OznakeZnakovaGramatike.Identifikator:
					case OznakeZnakovaGramatike.Boolean:
					case "nil":
					case "@":
					case "-":
					{
						#region Produkcija <subscript-list> ->  <expression>  <subscript-list_1> 
						PrimijenioProdukciju("<subscript-list>", "<expression>  <subscript-list_1>");
						
						TipPodatka inTipIzraza;
						UInt32 inVrijednostIzraza;
						bool inSveKonstantno;
						int nRedniBrojSubscripta = 0;
						int inRedniBrojSubscripta;
						
						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add(","); slijedi1.Add("]");
						
						Expression(slijedi1, out inTipIzraza, out inVrijednostIzraza, out inSveKonstantno);
						
						SemantickiAnalizator.ProvjeriIZapisiSubscript(nTipNiza, inTipIzraza, inVrijednostIzraza, inSveKonstantno, nRedniBrojSubscripta, out inRedniBrojSubscripta);
						
						Subscript_list_1(slijedi,nTipNiza,inRedniBrojSubscripta);
						
						#endregion					
						break;
					}
					default:
					{
						PrijaviGresku("Greška u subscript-u!");
						break;
					}
					
				}
			}
		}
		
		private void Subscript_list_1(ArrayList slijedi, TipPodatka nTipNiza, int nRedniBrojSubscripta)
		{
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add("]"); zapocinje.Add(",");
			if (Provjeri(zapocinje,slijedi,"Oèekivan zarez ili desna uglata zagrada!"))
			{
				switch (TrenutniZnak.OznakaZnakaGramatike)
				{	
					case ",":
					{
						#region Produkcija <subscript-list_1> ->  ,  <expression>  <subscript-list_1> 
						PrimijenioProdukciju("<subscript-list_1>", ",  <expression>  <subscript-list_1>");
						
						TipPodatka inTipIzraza;
						UInt32 inVrijednostIzraza;
						bool inSveKonstantno;
						int inRedniBrojSubscripta;
						
						KonzumirajZavrsniZnak(",");
						
						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add(","); slijedi1.Add("]");
						
						Expression(slijedi1, out inTipIzraza, out inVrijednostIzraza, out inSveKonstantno);
						
						SemantickiAnalizator.ProvjeriIZapisiSubscript(nTipNiza, inTipIzraza, inVrijednostIzraza, inSveKonstantno, nRedniBrojSubscripta, out inRedniBrojSubscripta);
						
						Subscript_list_1(slijedi, nTipNiza, inRedniBrojSubscripta);
						
						#endregion					
						break;
					}
					case OznakeZnakovaGramatike.KrajUlaznogNiza:
					case "]":
					{
						#region Produkcija <subscript-list_1> ->  <empty> 
						PrimijenioProdukciju("<subscript-list_1>", "<empty>");
						
						Empty();
						
						#endregion					
						break;
					}
					default:
					{
						PrijaviGresku("Oèekivan zarez ili desna uglata zagrada!");
						break;
					}
					
				}
			}
		}

		private void Case_List(ArrayList slijedi, TipPodatka nTipIzraza, int nOznakaCaseNaredbe, out ICollection iListaSlucajeva)
		{
			iListaSlucajeva = null;
		
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add("not");
			zapocinje.Add(OznakeZnakovaGramatike.UnsignedInteger);
			zapocinje.Add(OznakeZnakovaGramatike.Identifikator);
			zapocinje.Add(OznakeZnakovaGramatike.UnsignedReal);
			zapocinje.Add(OznakeZnakovaGramatike.Boolean);
			zapocinje.Add("+"); zapocinje.Add("nil");
			zapocinje.Add("-"); zapocinje.Add("@");
			if (Provjeri(zapocinje,slijedi,"Greška u CASE bloku!"))
			{
				switch (TrenutniZnak.OznakaZnakaGramatike)
				{	
					case "not":
					case OznakeZnakovaGramatike.UnsignedReal:
					case OznakeZnakovaGramatike.UnsignedInteger:
					case "+":
					case "(":
					case OznakeZnakovaGramatike.Identifikator:
					case OznakeZnakovaGramatike.Boolean:
					case "nil":
					case "@":
					case "-":
					{
						#region Produkcija <case-list> ->  <case-label-list>  :  <statement>  <case-list_1>
						PrimijenioProdukciju("<case-list>", "<case-label-list>  :  <statement>  <case-list_1>");
						
						ICollection inListaMogucihOdabira;
						int inOznakaCasePodNaredbe;
						ICollection inNovaLista;
						
						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.AddRange(_pocetakNaredbe1);
						slijedi1.Add(";"); slijedi1.Add("end");
						ArrayList slijedi2=new ArrayList(slijedi1);
						slijedi1.Add(":");
						
						Case_label_list(slijedi1, nTipIzraza, out inListaMogucihOdabira);
						
						Provjeri(":",slijedi2,"Oèekivana dvotoèka!");
						PojediZavrsniZnak(":");
						
						SemantickiAnalizator.ZapocniCasePodNaredbu(nOznakaCaseNaredbe, out inOznakaCasePodNaredbe);
						
						slijedi1=new ArrayList(slijedi);
						slijedi1.Add(";"); slijedi1.Add("end");
						
						Statement(slijedi1);
						
						SemantickiAnalizator.ZavrsiCasePodNaredbu(nOznakaCaseNaredbe);
						SemantickiAnalizator.StvoriListuSlucajeva(inListaMogucihOdabira, inOznakaCasePodNaredbe, out inNovaLista);
						
						Case_list_1(slijedi, nTipIzraza, nOznakaCaseNaredbe, inNovaLista, out iListaSlucajeva);
						
						#endregion					
						break;
					}
				
					default:
					{
						PrijaviGresku("Greška u CASE bloku!");
						break;
					}
					
				}
			}
		}

		private void Case_list_1(ArrayList slijedi, TipPodatka nTipIzraza, int nOznakaCaseNaredbe, ICollection nDosadasnjaListaSlucajeva, out ICollection iKonacnaListaSlucajeva)
		{
			iKonacnaListaSlucajeva = null;
		
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add(";"); zapocinje.Add("end");
			if (Provjeri(zapocinje,slijedi,"Oèekivana kljuèna rijeè END ili toèka-zarez!"))
			{
				switch (TrenutniZnak.OznakaZnakaGramatike)
				{	
					case ";":
					{
						#region Produkcija <case-list_1> ->  ;  <case-label-list>  :  <statement>  <case-list_1> 
						PrimijenioProdukciju("<case-list_1>", ";  <case-label-list>  :  <statement>  <case-list_1>");
						
						ICollection inListaMogucihOdabira;
						int inOznakaCasePodNaredbe;
						ICollection inDopunjenaListaSlucajeva;
						
						KonzumirajZavrsniZnak(";");
						
						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.AddRange(_pocetakNaredbe1);
						slijedi1.Add(";"); slijedi1.Add("end");
						ArrayList slijedi2=new ArrayList(slijedi1);
						slijedi1.Add(":");

						Case_label_list(slijedi1, nTipIzraza, out inListaMogucihOdabira);
						
						Provjeri(":",slijedi2,"Oèekivana dvotoèka!");
						PojediZavrsniZnak(":");
						
						SemantickiAnalizator.ZapocniCasePodNaredbu(nOznakaCaseNaredbe, out inOznakaCasePodNaredbe);

						slijedi1=new ArrayList(slijedi);
						slijedi1.Add(";"); slijedi1.Add("end");
						
						Statement(slijedi1);
						
						SemantickiAnalizator.ZavrsiCasePodNaredbu(nOznakaCaseNaredbe);
						SemantickiAnalizator.NadopuniListuSlucajeva(inListaMogucihOdabira, inOznakaCasePodNaredbe, nDosadasnjaListaSlucajeva, out inDopunjenaListaSlucajeva);

						Case_list_1(slijedi, nTipIzraza, nOznakaCaseNaredbe, inDopunjenaListaSlucajeva, out iKonacnaListaSlucajeva);
						#endregion					
						break;
					}
					case OznakeZnakovaGramatike.KrajUlaznogNiza:
					case "end":
					{
						#region Produkcija <case-list_1> ->  <empty> 
					
						PrimijenioProdukciju("<case-list_1>", "<empty>");
						
						Empty();
						
						iKonacnaListaSlucajeva = nDosadasnjaListaSlucajeva;
						
						#endregion					
						break;
					}
					default:
					{
						PrijaviGresku("Oèekivana kljuèna rijeè END ili toèka-zarez!");
						break;
					}
					
				}
			}
		}

		private void For_list(ArrayList slijedi, string nImeIdentifikatora, out int iOznakaForPetlje, out bool iRastucaPetlja)
		{
			iOznakaForPetlje = 0;
			iRastucaPetlja = true;
		
			ArrayList zapocinje=new ArrayList(_pocetakIzraza);
			if (Provjeri(zapocinje,slijedi,"Greška u FOR bloku!"))
			{
				switch (TrenutniZnak.OznakaZnakaGramatike)
				{	
					case "not":
					case OznakeZnakovaGramatike.UnsignedReal:
					case OznakeZnakovaGramatike.UnsignedInteger:
					case "+":
					case "(":
					case OznakeZnakovaGramatike.Identifikator:
					case OznakeZnakovaGramatike.Boolean:
					case "nil":
					case "@":
					case "-":
					{
						#region Produkcija <for-list> ->  <expression>  <for-list1>
						PrimijenioProdukciju("<for-list>", "<expression>  <for-list1>");

						TipPodatka inTipIzraza;
						UInt32 inVrijednostIzraza;
						bool inSveKonstatno;
						int inOznakaForPetlje;

						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add("to"); slijedi1.Add("downto");
						
						Expression(slijedi1, out inTipIzraza, out inVrijednostIzraza, out inSveKonstatno);
						
						SemantickiAnalizator.InicijalizirajForPetlju(nImeIdentifikatora, inTipIzraza, inVrijednostIzraza, inSveKonstatno, out inOznakaForPetlje);
						
						For_list1(slijedi, inOznakaForPetlje, nImeIdentifikatora, out iRastucaPetlja);

						iOznakaForPetlje = inOznakaForPetlje;
						
						#endregion
						break;
					}
				
					default:
					{
						PrijaviGresku("Greška u FOR bloku!");
						break;
					}
					
				}
			}
		}

		private void For_list1(ArrayList slijedi, int nOznakaForPetlje, string nImeIdentifikatora, out bool iRastucaPetlja)
		{
			iRastucaPetlja = true;

			ArrayList zapocinje=new ArrayList(_pocetakIzraza);
			zapocinje.Add("to"); zapocinje.Add("downto");
			if (Provjeri(zapocinje,slijedi,"Oèekivena kljuèna rijeè TO ili DOWNTO!"))
			{
				switch (TrenutniZnak.OznakaZnakaGramatike)
				{	
					case "to":
					{
						#region Produkcija <for-list1> ->  to  <expression> 
						PrimijenioProdukciju("<for-list1>", "to  <expression>");

						TipPodatka inTipIzraza;
						UInt32 inVrijednostIzraza;
						bool inSveKonstatno;
						bool nRastucaPetlja = true;

						KonzumirajZavrsniZnak("to");
						
						Expression(slijedi, out inTipIzraza, out inVrijednostIzraza, out inSveKonstatno);

						SemantickiAnalizator.OznaciGranicuForPetljeIProvjeri(nImeIdentifikatora, inTipIzraza, inVrijednostIzraza, inSveKonstatno, nOznakaForPetlje, nRastucaPetlja);
						iRastucaPetlja = nRastucaPetlja;
					
						#endregion					
						break;
					}
					case "downto":
					{
						#region Produkcija <for-list1> ->  downto  <expression>
						PrimijenioProdukciju("<for-list1>", "downto  <expression>");
						
						TipPodatka inTipIzraza;
						UInt32 inVrijednostIzraza;
						bool inSveKonstatno;
						bool nRastucaPetlja = false;

						KonzumirajZavrsniZnak("downto");
						
						Expression(slijedi, out inTipIzraza, out inVrijednostIzraza, out inSveKonstatno);
						
						SemantickiAnalizator.OznaciGranicuForPetljeIProvjeri(nImeIdentifikatora, inTipIzraza, inVrijednostIzraza, inSveKonstatno, nOznakaForPetlje, nRastucaPetlja);
						iRastucaPetlja = nRastucaPetlja;
						#endregion					
						break;
					}
				
					default:
					{
						PrijaviGresku("Oèekivena kljuèna rijeè TO ili DOWNTO!");
						break;
					}
					
				}
			}
		}

		private void Expression_list(ArrayList slijedi, string nImePotPrograma)
		{
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add("not");
			zapocinje.Add(OznakeZnakovaGramatike.UnsignedInteger);
			zapocinje.Add(OznakeZnakovaGramatike.Identifikator);
			zapocinje.Add(OznakeZnakovaGramatike.UnsignedReal);
			zapocinje.Add(OznakeZnakovaGramatike.Boolean);
			zapocinje.Add("+"); zapocinje.Add("nil");
			zapocinje.Add("-"); zapocinje.Add("@");
			if (Provjeri(zapocinje,slijedi,"Greška u listi izraza!"))
			{
				switch (TrenutniZnak.OznakaZnakaGramatike)
				{
					case "not":
					case OznakeZnakovaGramatike.UnsignedReal:
					case OznakeZnakovaGramatike.UnsignedInteger:
					case "+":
					case "(":
					case OznakeZnakovaGramatike.Identifikator:
					case OznakeZnakovaGramatike.Boolean:
					case "nil":
					case "@":
					case "-":
					{
						#region Produkcija <expression-list> ->  <expression>  <expression-list_1>
						PrimijenioProdukciju("<expression-list>", "<expression>  <expression-list_1>");

						TipPodatka inTipIzraza;
						UInt32 inVrijednostIzraza;
						bool inSveKonstantno;
						int nRedniBrojIzraza = 0;
						int inRedniBrojIzraza;

						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add(","); slijedi1.Add(")");
						
						Expression(slijedi1, out inTipIzraza, out inVrijednostIzraza, out inSveKonstantno);
													
						SemantickiAnalizator.ProvjeriIZapisiParametar(nImePotPrograma, inTipIzraza, inVrijednostIzraza, inSveKonstantno, nRedniBrojIzraza, out inRedniBrojIzraza);
						
						Expression_list_1(slijedi, nImePotPrograma, inRedniBrojIzraza);
						#endregion					
						break;
					}
				
					default:
					{
						PrijaviGresku("Greška u listi izraza!");
						break;
					}
					
				}
			}
		}

		private void Expression_list_1(ArrayList slijedi, string nImePotPrograma, int nRedniBrojIzraza)
		{
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add(","); zapocinje.Add(")");
			if (Provjeri(zapocinje,slijedi,"Oèekivan zarez ili desna zagrada!"))
			{
				switch (TrenutniZnak.OznakaZnakaGramatike)
				{	
				
					case ",":
					{
						#region Produkcija <expression-list_1> ->  ,  <expression>  <expression-list_1>
						PrimijenioProdukciju("<expression-list_1>", ",  <expression>  <expression-list_1>");
						
						TipPodatka inTipIzraza;
						UInt32 inVrijednostIzraza;
						bool inSveKonstantno;
						int inRedniBrojIzraza;
					
						KonzumirajZavrsniZnak(",");
						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add(","); slijedi1.Add(")");
						
						Expression(slijedi1, out inTipIzraza, out inVrijednostIzraza, out inSveKonstantno);
                        					
						SemantickiAnalizator.ProvjeriIZapisiParametar(nImePotPrograma, inTipIzraza, inVrijednostIzraza, inSveKonstantno, nRedniBrojIzraza, out inRedniBrojIzraza);
                        
						Expression_list_1(slijedi, nImePotPrograma, inRedniBrojIzraza);
						#endregion					
						break;
					}
					case OznakeZnakovaGramatike.KrajUlaznogNiza:
					case ")":
					{
						#region Produkcija <expression-list_1> ->  <empty> 
					
						PrimijenioProdukciju("<expression-list_1>", "<empty>");
						Empty();
					
						#endregion					
						break;
					}
				
					default:
					{
						PrijaviGresku("Oèekivan zarez ili desna zagrada!");
						break;
					}
					
				}
			}
		}
		private void Label(ArrayList slijedi, out UInt32 iVrijednostLabele)
		{
			//defaultna vrijednost out parametra. Ona se mora definirati jer C# zahtjeva da se out parametru
			//pridijeli neka vrijednost unutar tijela metode, a moguce je da se ne pozove metoda KonzumirajZavrsniZnak
			iVrijednostLabele = 0;
			if (Provjeri(OznakeZnakovaGramatike.UnsignedInteger,slijedi,"Oèekivan cijeli broj!"))
			{
				switch (TrenutniZnak.OznakaZnakaGramatike)
				{	
				
					case OznakeZnakovaGramatike.UnsignedInteger:
					{
						#region Produkcija <label> ->  unsigned-integer 
						PrimijenioProdukciju("<label>", "unsigned-integer");

						//uslijed uskladjivanja imena svojstava, nema pravila preslikavanja
						//vec se metodi KonzumirajZavrsniZnak direktno salje parametar iVrijednostLabele
						KonzumirajZavrsniZnak(OznakeZnakovaGramatike.UnsignedInteger, out iVrijednostLabele);
						#endregion					
						break;
					}
						
					default:
					{
						PrijaviGresku("Oèekivan cijeli broj!");
						break;
					}
					
				}
			}
		}
		
		#endregion

		#region Toncijev dio
		private	void Expression(ArrayList slijedi, out TipPodatka iTipIzraza, out UInt32 iVrijednostIzraza, out bool iSveKonstantno)
		{
			iTipIzraza=null; 
			iVrijednostIzraza=0;
			iSveKonstantno=false;

			ArrayList zapocinje=new ArrayList(_pocetakIzraza);
			if (Provjeri(zapocinje,slijedi,"Nelegalan poèetak izraza!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case "(":
					case "not":
					case OznakeZnakovaGramatike.UnsignedReal:
					case OznakeZnakovaGramatike.UnsignedInteger:
					case "+":
					case OznakeZnakovaGramatike.Identifikator:
					case OznakeZnakovaGramatike.Boolean:
					case "@":
					case "-":
					case "nil":
					{
						#region	Produkcija <expression>	->	<additive-expression> <expression_1>
						PrimijenioProdukciju("<expression>", "<additive-expression> <expression_1>");
						TipPodatka inTipZbrojenogIzraza;
						UInt32 inVrijednostCijelogIzrazaZb;
						bool inSveKonstantnoZb;

						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.AddRange(_pocetakIzraza1);
						Additive_expression(slijedi1, out inTipZbrojenogIzraza, out inVrijednostCijelogIzrazaZb, out inSveKonstantnoZb);
						Expression_1(slijedi, inTipZbrojenogIzraza, inVrijednostCijelogIzrazaZb, inSveKonstantnoZb, out iTipIzraza, out iVrijednostIzraza, out iSveKonstantno);
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Nelegalan poèetak izraza!");
						break;
					}
				}
			}
		}
		
		private	void Expression_1(ArrayList slijedi, TipPodatka nTipLijevogDijela, UInt32 nVrijednostLijevogDijela, bool nLijeviDioKonstantan, out TipPodatka iTipCijelogIzraza, out UInt32 iVrijednostCijelogIzraza, out bool iSveKonstantno) 
		{
			iTipCijelogIzraza=null;
			iVrijednostCijelogIzraza=0;
			iSveKonstantno=false;

			ArrayList zapocinje=new ArrayList(_pocetakIzraza1);
			zapocinje.Add("if");
			zapocinje.Add(":=");
			if (Provjeri(zapocinje,slijedi,"Greška u izrazu!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case "then":
					case "downto":
					case "end":
					case "else":
					case "of":
					case "]":
					case ")":
					case ",":
					case ":":
					case ";":
					case "do":
					case "to":
					case "until":
					{
						#region	Produkcija <expression_1> -> <empty>
						PrimijenioProdukciju("<expression_1>", "<empty>");

						Empty();
						iTipCijelogIzraza = nTipLijevogDijela;
					    iVrijednostCijelogIzraza = nVrijednostLijevogDijela;
					    iSveKonstantno = nLijeviDioKonstantan;
						#endregion
						break;
					}
					case "<":
					case "=":
					case "<>":
					case ">=":
					case "<=":
					case ">":
					{
						#region	Produkcija <expression_1> -> <relational-op> <additive-expression> <expression_1>
						PrimijenioProdukciju("<expression_1>", "<relational-op> <additive-expression> <expression_1>");
					 	BinarniOperatoriEnum inOperator;
						TipPodatka inTipZbrojenogIzraza, inEvaluiraniTip;
						UInt32 inVrijednostCijelogIzrazaZb, inUkupnaVrijednost;
						bool inSveKonstantnoZb;

						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.AddRange(_pocetakIzraza1);
						ArrayList slijedi2=new ArrayList(slijedi1);
						slijedi1.AddRange(_pocetakIzraza);

						Relational_op(slijedi1, out inOperator);
						Additive_expression(slijedi2, out inTipZbrojenogIzraza, out inVrijednostCijelogIzrazaZb, out inSveKonstantnoZb);
						SemantickiAnalizator.Evaluiraj(inOperator,nTipLijevogDijela,inTipZbrojenogIzraza,nVrijednostLijevogDijela,nLijeviDioKonstantan,inVrijednostCijelogIzrazaZb,inSveKonstantnoZb, out inEvaluiraniTip,out inUkupnaVrijednost,out iSveKonstantno);
						Expression_1(slijedi, inEvaluiraniTip, inUkupnaVrijednost, iSveKonstantno, out iTipCijelogIzraza, out iVrijednostCijelogIzraza, out iSveKonstantno);
						#endregion
						break;
					}				
					default:
					{
						PrijaviGresku("Greška u izrazu!");
						break;
					}
				}
			}
		}

		private	void Additive_expression(ArrayList slijedi, out TipPodatka iTipZbrojenogIzraza, out UInt32 iVrijednostCijelogIzraza, out bool iSveKonstantno)
		{
			iTipZbrojenogIzraza=null;
			iVrijednostCijelogIzraza=0;
			iSveKonstantno=false;

			ArrayList zapocinje=new ArrayList(_pocetakIzraza);
			if (Provjeri(zapocinje,slijedi,"Greška u aditivnom izrazu!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case "(":
					case "not":
					case OznakeZnakovaGramatike.Identifikator:
					case OznakeZnakovaGramatike.UnsignedReal:
					case OznakeZnakovaGramatike.UnsignedInteger:
					case OznakeZnakovaGramatike.Boolean:
					case "+":
					case "@":
					case "-":
					case "nil":
					{
						#region	Produkcija <additive-expression> -> <multiplicative-expression> <additive-expression_1>
						PrimijenioProdukciju("<additive-expression>", "<multiplicative-expression>	<additive-expression_1>");
						TipPodatka inTipLijevogDijela1;
						UInt32 inVrijednostLijevogDijela1;
						bool inLijeviDioKonstantan1;

						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.AddRange(_pocetakIzraza1);
						slijedi1.Add("+"); slijedi1.Add("-");
						slijedi1.Add("or");

						Multiplicative_expression(slijedi1, out inTipLijevogDijela1, out inVrijednostLijevogDijela1, out inLijeviDioKonstantan1);
						Additive_expression_1(slijedi, inTipLijevogDijela1, inVrijednostLijevogDijela1, inLijeviDioKonstantan1, out iTipZbrojenogIzraza, out iVrijednostCijelogIzraza, out iSveKonstantno);
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Greška u aditivnom izrazu!");
						break;
					}
				}
			}
		}
				
		private	void Additive_expression_1(ArrayList slijedi, TipPodatka nTipLijevogDijela, UInt32 nVrijednostLijevogDijela, bool nLijeviDioKonstantan, out TipPodatka iTipCijelogIzraza, out UInt32 iVrijednostCijelogIzraza, out bool iSveKonstantno)
		{
			iTipCijelogIzraza=null;
			iVrijednostCijelogIzraza=0;
			iSveKonstantno=false;

			ArrayList zapocinje=new ArrayList(_pocetakIzraza1);
			zapocinje.Add("+"); zapocinje.Add("-");
			zapocinje.Add("or");

			zapocinje.Add("if");
			zapocinje.Add(":=");

			if (Provjeri(zapocinje,slijedi,"Greška u aditivnom izrazu!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case "+":
					case "-":
					case "or":
					{
						#region	Produkcija <additive-expression_1> -> <additive-op> <multiplicative-expression> <additive-expression_1>
						PrimijenioProdukciju("<additive-expression_1>",	"<additive-op>	<multiplicative-expression> <additive-expression_1>");
						BinarniOperatoriEnum inOperator;
						TipPodatka inTipMnozenogIzraza, inEvaluiraniTip;
						UInt32 inVrijednostCijelogIzrazaMn, inUkupnaVrijednost;
						bool inSveKonstantnoMn;

						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.AddRange(_pocetakIzraza1);
						slijedi1.Add("+"); slijedi1.Add("-");
						slijedi1.Add("or");
						ArrayList slijedi2=new ArrayList(slijedi1);
						slijedi1.AddRange(_pocetakIzraza);

						Additive_op(slijedi1, out inOperator);
						Multiplicative_expression(slijedi2,out inTipMnozenogIzraza, out inVrijednostCijelogIzrazaMn, out inSveKonstantnoMn);
						SemantickiAnalizator.Evaluiraj(inOperator,nTipLijevogDijela,inTipMnozenogIzraza,nVrijednostLijevogDijela,nLijeviDioKonstantan,inVrijednostCijelogIzrazaMn,inSveKonstantnoMn,out inEvaluiraniTip,out inUkupnaVrijednost,out iSveKonstantno);
						Additive_expression_1(slijedi, inEvaluiraniTip, inUkupnaVrijednost, iSveKonstantno, out iTipCijelogIzraza, out iVrijednostCijelogIzraza, out iSveKonstantno);
						#endregion
						break;
					}
					case "of":
					case ">":
					case "]":
					case "<":
					case "=":
					case "do":
					case ">=":
					case ":":
					case ")":
					case ",":
					case "until":
					case "else":
					case "<>":
					case "<=":
					case "then":
					case "downto":
					case "to":
					case "end":
					case ";":
					{
						#region	Produkcija <additive-expression_1> -> <empty>
						PrimijenioProdukciju("<additive-expression_1>",	"<empty>");
					
						Empty();
						iTipCijelogIzraza = nTipLijevogDijela;
						iVrijednostCijelogIzraza = nVrijednostLijevogDijela;
						iSveKonstantno = nLijeviDioKonstantan;
						#endregion
						break;					
					}
					default:
					{
						PrijaviGresku("Greška u aditivnom izarazu!");
						break;
					}
				}
			}
		}
		
		private	void Multiplicative_expression(ArrayList slijedi,out TipPodatka iTipMnozenogIzraza,out UInt32 iVrijednostCijelogIzraza,out bool iSveKonstantno)
		{
			iTipMnozenogIzraza=null;
			iVrijednostCijelogIzraza=0;
			iSveKonstantno=false;
			ArrayList zapocinje=new ArrayList(_pocetakIzraza);
			if (Provjeri(zapocinje,slijedi,"Greška u multiplikativnom dijelu izraza!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case "not":
					case OznakeZnakovaGramatike.UnsignedReal:
					case OznakeZnakovaGramatike.UnsignedInteger:
					case "+":
					case "(":
					case OznakeZnakovaGramatike.Identifikator:
					case OznakeZnakovaGramatike.Boolean:
					case "@":
					case "-":
					case "nil":
					{
						#region	Produkcija <multiplicative-expression> -> <unary-expression> <multiplicative-expression_1>
						PrimijenioProdukciju("<multiplicative-expression>",	"<unary-expression> <multiplicative-expression_1>");
						TipPodatka inTipUnarnogIzraza;
						UInt32 inVrijednostIzrazaUn ;
						bool inSveKonstantnoUn ;

						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.AddRange(_pocetakIzraza1);
						slijedi1.Add("div"); slijedi1.Add("and");
						slijedi1.Add("/"); slijedi1.Add("mod");
						slijedi1.Add("in"); slijedi1.Add("*");
						Unary_expression(slijedi1,out inTipUnarnogIzraza,out inVrijednostIzrazaUn,out inSveKonstantnoUn);
						Multiplicative_expression_1(slijedi,inTipUnarnogIzraza,inVrijednostIzrazaUn,inSveKonstantnoUn,out iTipMnozenogIzraza,out iVrijednostCijelogIzraza,out iSveKonstantno);
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Greška u multiplikativnom dijelu izraza!");
						break;
					}
				}
			}
		}
		
		private	void Multiplicative_expression_1(ArrayList slijedi,TipPodatka nTipLijevogDijela,UInt32 nVrijednostLijevogDijela,bool nLijeviDioKonstantan,out TipPodatka iTipCijelogIzraza,out UInt32 iVrijednostCijelogIzraza,out bool iSveKonstantno )
		{
			iTipCijelogIzraza=null;
			iVrijednostCijelogIzraza=0;
			iSveKonstantno=false;
			ArrayList zapocinje=new ArrayList();
			zapocinje.AddRange(_pocetakIzraza1);
			zapocinje.Add("div"); zapocinje.Add("and");
			zapocinje.Add("/"); zapocinje.Add("mod");
			zapocinje.Add("in"); zapocinje.Add("*");
			zapocinje.Add("+"); zapocinje.Add("-");
			zapocinje.Add("or"); 
			zapocinje.Add("if");
			zapocinje.Add(OznakeZnakovaGramatike.Identifikator);
			zapocinje.Add(":=");
			if (Provjeri(zapocinje,slijedi,"Greška u multiplikativnom dijelu izraza 1!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case "div":
					case "and":
					case "/":
					case "mod":
					case "*":
					{
						#region	Produkcija <multiplicative-expression_1> -> <multiplicative-op> <unary-expression> <multiplicative-expression_1>	
						PrimijenioProdukciju("<multiplicative-expression_1>", "<multiplicative-op>	<unary-expression>	<multiplicative-expression_1>");
						BinarniOperatoriEnum inOperator; 
						UInt32 inVrijednostIzrazaUn; 
						bool inSveKonstantnoUn;
						TipPodatka inTipDesnogOperanda;
						TipPodatka inEvaluiraniTip;
						UInt32 inUkupnaVrijednost;
						bool inSveKonstantnoEval; 

						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.AddRange(_pocetakIzraza1);
						slijedi1.Add("div"); slijedi1.Add("and");
						slijedi1.Add("/"); slijedi1.Add("mod");
						slijedi1.Add("in"); slijedi1.Add("*");
						ArrayList slijedi2=new ArrayList(slijedi1);
						slijedi1.Add("+"); slijedi1.Add("-");
						slijedi1.Add("not");

						Multiplicative_op(slijedi1,out inOperator);
						Unary_expression(slijedi2,out inTipDesnogOperanda,out inVrijednostIzrazaUn,out inSveKonstantnoUn);
						SemantickiAnalizator.Evaluiraj(inOperator,nTipLijevogDijela,inTipDesnogOperanda,nVrijednostLijevogDijela,nLijeviDioKonstantan,
							inVrijednostIzrazaUn,inSveKonstantnoUn,out inEvaluiraniTip,out inUkupnaVrijednost,out inSveKonstantnoEval);
						Multiplicative_expression_1(slijedi, inEvaluiraniTip, inUkupnaVrijednost, inSveKonstantnoEval, out iTipCijelogIzraza, out iVrijednostCijelogIzraza, out iSveKonstantno);
						#endregion
						break;
					}
					case "of":
					case ">":
					case "]":
					case "<":
					case "=":
					case "do":
					case ">=":
					case ":":
					case ")":
					case ",":
					case "until":
					case "else":
					case "<>":
					case "<=":
					case "+":
					case "then":
					case "or":
					case "-":
					case "downto":
					case "to":
					case "end":
					case ";":
					{
						#region	Produkcija <multiplicative-expression_1> -> <empty> 
						PrimijenioProdukciju("<multiplicative-expression_1>", "<empty>");
						iTipCijelogIzraza=nTipLijevogDijela;
						iVrijednostCijelogIzraza=nVrijednostLijevogDijela;
						iSveKonstantno=nLijeviDioKonstantan;
						Empty();
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Greška u multiplikativnom dijelu izraza 1!");
						break;
					}
				}
			}
		}
		
		private	void Relational_op(ArrayList slijedi, out BinarniOperatoriEnum iOperator)
		{
			iOperator = BinarniOperatoriEnum.Unknown;
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add("<"); zapocinje.Add("<=");
			zapocinje.Add("="); zapocinje.Add("<>");
			zapocinje.Add(">="); zapocinje.Add(">");

			zapocinje.Add(":=");
			if (Provjeri(zapocinje,slijedi,"Oèekivan relacijski operator!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case "<":
					{
						#region	Produkcija <relational-op> -> <
						PrimijenioProdukciju("<relational-op>",	"<");
						iOperator = BinarniOperatoriEnum.ManjiOd;
						KonzumirajZavrsniZnak("<");
						#endregion
						break;
					}
					case "<=":
					{
						#region	Produkcija <relational-op> -> <=
						PrimijenioProdukciju("<relational-op>",	"<=");
						iOperator = BinarniOperatoriEnum.ManjiIliJednak;
						KonzumirajZavrsniZnak("<=");
						#endregion
						break;
					}
					case "=":
					{
						#region	Produkcija <relational-op> -> =
						PrimijenioProdukciju("<relational-op>",	"=");
						iOperator = BinarniOperatoriEnum.Jednak;
						KonzumirajZavrsniZnak("=");
						#endregion
						break;
					}
					case "<>":
					{
						#region	Produkcija <relational-op> -> <>
						PrimijenioProdukciju("<relational-op>",	"<>");
						iOperator = BinarniOperatoriEnum.NijeJednak;
						KonzumirajZavrsniZnak("<>");
						#endregion
						break;
					}
					case ">=":
					{
						#region	Produkcija <relational-op> -> >=
						PrimijenioProdukciju("<relational-op>",	">=");
						iOperator = BinarniOperatoriEnum.VeciIliJednak;
						KonzumirajZavrsniZnak(">=");
						#endregion
						break;
					}
					case ">":
					{
						#region	Produkcija <relational-op> -> >
						PrimijenioProdukciju("<relational-op>",	">");
						iOperator = BinarniOperatoriEnum.VeciOd;
						KonzumirajZavrsniZnak(">");
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Oèekivan relacijski operator!");
						break;
					}
				}
			}
		}
		
		private	void Additive_op(ArrayList slijedi, out BinarniOperatoriEnum iOperator)
		{
			iOperator = BinarniOperatoriEnum.Unknown;
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add("+"); zapocinje.Add("-");
			zapocinje.Add("or");
			if (Provjeri(zapocinje,slijedi,"Oèekivan aditivni operator!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case "+":
					{
						#region	Produkcija <additive-op> -> +
						PrimijenioProdukciju("<additive-op>", "+");
						iOperator = BinarniOperatoriEnum.Plus;					
						KonzumirajZavrsniZnak("+");
						#endregion
						break;
					}
					case "-":
					{
						#region	Produkcija <additive-op> -> -
						PrimijenioProdukciju("<additive-op>", "-");
						iOperator = BinarniOperatoriEnum.Minus;					
						KonzumirajZavrsniZnak("-");
						#endregion
						break;
					}
					case "or":
					{
						#region	Produkcija <additive-op> -> or
						PrimijenioProdukciju("<additive-op>", "or");
						iOperator = BinarniOperatoriEnum.Or;
						KonzumirajZavrsniZnak("or");
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Oèekivan aditivni operator!");
						break;
					}
				}
			}
		}
		
		private	void Multiplicative_op(ArrayList slijedi,out BinarniOperatoriEnum iOperator)
		{
			iOperator=BinarniOperatoriEnum.Unknown;
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add("*"); zapocinje.Add("/");
			zapocinje.Add("div"); zapocinje.Add("mod");
			zapocinje.Add("and"); zapocinje.Add("in");
			if (Provjeri(zapocinje,slijedi,"Oèekivan multiplikativni operator!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case "*":
					{
						#region	Produkcija <multiplicative-op> -> *
						PrimijenioProdukciju("<multiplicative-op>",	"*");
						iOperator=BinarniOperatoriEnum.Puta;
						KonzumirajZavrsniZnak("*");
						#endregion
						break;
					}
					case "/":
					{
						#region	Produkcija <multiplicative-op> -> /
						PrimijenioProdukciju("<multiplicative-op>",	"/");
						iOperator=BinarniOperatoriEnum.Dijeljeno;
						KonzumirajZavrsniZnak("/");
						#endregion
						break;
					}
					case "div":
					{
						#region	Produkcija <multiplicative-op> -> div
						PrimijenioProdukciju("<multiplicative-op>",	"div");
						iOperator=BinarniOperatoriEnum.Div;
						KonzumirajZavrsniZnak("div");
						#endregion
						break;
					}
					case "mod":
					{
						#region	Produkcija <multiplicative-op> -> mod
						PrimijenioProdukciju("<multiplicative-op>",	"mod");
						iOperator=BinarniOperatoriEnum.Mod;
						KonzumirajZavrsniZnak("mod");
						#endregion
						break;
					}
					case "and":
					{
						#region	Produkcija <multiplicative-op> -> and
						PrimijenioProdukciju("<multiplicative-op>",	"and");
						iOperator=BinarniOperatoriEnum.And;
						KonzumirajZavrsniZnak("and");
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Oèekivan multiplikativni operator!");
						break;
					}
				}
			}
		}
		#endregion

		#region Ninov dio
		private void Unary_expression(ArrayList slijedi, out TipPodatka iTipUnarnogIzraza, out UInt32 iVrijednostIzraza, out bool iSveKonstantno)
		{
			iTipUnarnogIzraza = null;
			iVrijednostIzraza = 0;
			iSveKonstantno = false;

			ArrayList zapocinje=new ArrayList(_pocetakIzraza);
			if (Provjeri(zapocinje,slijedi,"Oèekivan unarni operator (ue)!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case "+":
					case "-":
					case "not":
					{
						#region Produkcija <unary-expression> ->  <unary-op>  <unary-expression> 
						PrimijenioProdukciju("<unary-expression>","<unary-op>  <unary-expression>");
	
						UnarniOperatoriEnum inOperator;
						TipPodatka inTipUnarnogIzraza1;
						UInt32 inVrijednostIzraza1;
						bool inSveKonstantno1;

						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.AddRange(zapocinje);
						
						Unary_op(slijedi1, out inOperator);
						Unary_expression(slijedi, out inTipUnarnogIzraza1, out inVrijednostIzraza1, out inSveKonstantno1);
						
						SemantickiAnalizator.EvaluirajUnarni(inOperator, inTipUnarnogIzraza1, inVrijednostIzraza1, inSveKonstantno1, out iTipUnarnogIzraza, out iVrijednostIzraza, out iSveKonstantno);
						
						#endregion
						break;
					}
					case "@":
					case "(":
					case OznakeZnakovaGramatike.Boolean:
					case OznakeZnakovaGramatike.UnsignedInteger:
					case "nil":
					case OznakeZnakovaGramatike.UnsignedReal:
					case OznakeZnakovaGramatike.Identifikator:
					{
						#region Produkcija <unary-expression> ->  <primary-expression>
						PrimijenioProdukciju("<unary-expression>","<primary-expression>");
						
						Primary_expression(slijedi, out iTipUnarnogIzraza, out iVrijednostIzraza, out iSveKonstantno);
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Oèekivan unarni operator!");
						break;
					}
				}
			}
		}

		private void Unary_op(ArrayList slijedi, out UnarniOperatoriEnum iOperator)
		{
			iOperator= UnarniOperatoriEnum.Unknown;

			ArrayList zapocinje=new ArrayList();
			zapocinje.Add("+"); zapocinje.Add("-");
			zapocinje.Add("not");
			if (Provjeri(zapocinje,slijedi,"Oèekivan unarni operator!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case "+":
					{	
						#region Produkcija <unary-op> ->  +
						PrimijenioProdukciju("<unary-op>","+");
						iOperator=UnarniOperatoriEnum.Plus;
						KonzumirajZavrsniZnak("+");
						#endregion
						break;
					}
					case "-":
					{
						#region Produkcija <unary-op> ->  -
						PrimijenioProdukciju("<unary-op>","-");
						iOperator=UnarniOperatoriEnum.Minus;
						KonzumirajZavrsniZnak("-");
						#endregion
						break;
					}
					case "not":
					{
						#region Produkcija <unary-op> ->  not
						PrimijenioProdukciju("<unary-op>","not");
						iOperator=UnarniOperatoriEnum.Not;
						KonzumirajZavrsniZnak("not");
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Oèekivan unarni operator!");
						break;
					}
				}
			}
		}

		private void Constid(ArrayList slijedi, out string inImeKonstante)
		{
			inImeKonstante="";
			if (Provjeri(OznakeZnakovaGramatike.Identifikator,slijedi,"Oèekivan identifikator!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case OznakeZnakovaGramatike.Identifikator:
					{
						#region	Produkcija <constid> ->  identifier
						PrimijenioProdukciju("<primary-expression>","identifier");
						KonzumirajZavrsniZnak(OznakeZnakovaGramatike.Identifikator, out inImeKonstante);
						SemantickiAnalizator.ProvjeriKonstantu(inImeKonstante);
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Oèekivan identifikator!");
						break;
					}
				}
			}
		}

		private void Typeid(ArrayList slijedi, out TipPodatka iTip)
		{
			iTip=null;
			if (Provjeri(OznakeZnakovaGramatike.Identifikator,slijedi,"Pogrešana oznaka tipa!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case OznakeZnakovaGramatike.Identifikator:
					{
						#region	Produkcija <typeid> ->  identifier
						PrimijenioProdukciju("<typeid>","identifier");
						string inImeTipa;
						KonzumirajZavrsniZnak(OznakeZnakovaGramatike.Identifikator, out inImeTipa);
						SemantickiAnalizator.DohvatiTip(inImeTipa, out iTip);
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Pogrešna oznaka tipa!");
						break;
					}
				}
			}
		}

		private void Funcid(ArrayList slijedi, out string inImeFunkcije)
		{
			inImeFunkcije="";

			if (Provjeri(OznakeZnakovaGramatike.Identifikator,slijedi,"Oèekivan identifikator!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case OznakeZnakovaGramatike.Identifikator:
					{
						#region	Produkcija <funcid> ->  identifier
						
						PrimijenioProdukciju("<funcid>","identifier");			
						KonzumirajZavrsniZnak(OznakeZnakovaGramatike.Identifikator,out inImeFunkcije);
						SemantickiAnalizator.ProvjeriFunkciju(inImeFunkcije);
						
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Oèekivan identifikator!");
						break;
					}
				}
			}
		}
		
		private void Procid(ArrayList slijedi, out string inImeProcedure)
		{
			inImeProcedure="";

			if (Provjeri(OznakeZnakovaGramatike.Identifikator,slijedi,"Oèekivan identifikator!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case OznakeZnakovaGramatike.Identifikator:
					{
						#region	Produkcija <procid> ->  identifier
						PrimijenioProdukciju("<procid>","identifier");

						KonzumirajZavrsniZnak(OznakeZnakovaGramatike.Identifikator,out inImeProcedure);
						SemantickiAnalizator.ProvjeriProceduru(inImeProcedure);
				
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Oèekivan identifikator!");
						break;
					}
				}
			}
		}

		private void Fieldid(ArrayList slijedi, out string inImePolja)
		{
			inImePolja="";

			if (Provjeri(OznakeZnakovaGramatike.Identifikator,slijedi,"Oèekivan identifikator!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case OznakeZnakovaGramatike.Identifikator:
					{
						#region	Produkcija <fieldid> ->  identifier
						
						PrimijenioProdukciju("<fieldid>","identifier");
						KonzumirajZavrsniZnak(OznakeZnakovaGramatike.Identifikator,out inImePolja);
						
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Oèekivan identifikator!");
						break;
					}
				}
			}
		}

		private void Varid(ArrayList slijedi, out string inImeVarijable)
		{
			inImeVarijable="";

			if (Provjeri(OznakeZnakovaGramatike.Identifikator,slijedi,"Oèekivan identifikator!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case OznakeZnakovaGramatike.Identifikator:
					{
						#region	Produkcija <varid> ->  identifier
						PrimijenioProdukciju("<varid>","identifier");

						KonzumirajZavrsniZnak(OznakeZnakovaGramatike.Identifikator, out inImeVarijable);
						SemantickiAnalizator.ProvjeriVarijablu(inImeVarijable);

						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Oèekivan identifikator!");
						break;
					}
				}
			}
		}

		private void Empty()
		{
			switch(TrenutniZnak.OznakaZnakaGramatike)
			{
				case "end":
				case "or":
				case ">=":
				case "of":
				case "<>":
				case "]":
				case "procedure":		
				case "/":
				case "mod":
				case ":":
				case ";":
				case ">":
				case "<":
				case "=":			
				case "do":
				case "*":
				case "+":
				case ")":
				case "<=":
				case ",":
				case "-":				
				case "and":
				case "to":				
				case "then":
				case "type":
				case "downto":
				case "div":
				case "else":
				case "until":
				case "var":
				case ":=":
				case "begin":
				case "function":
				case OznakeZnakovaGramatike.KrajUlaznogNiza:
				{
					#region	Produkcija <empty> ->  $
					PrimijenioProdukciju("<empty>","");
					/* Ovo je epsilon produkcija, $ NIJE zavrsni znak
					 * i on se NE konzumira
					 */
					#endregion
					break;
				}
				default:
				{
					FatalnaGreska();
					break;
				}
			}	
		}
	
		private void Variable_suffix(ArrayList slijedi, TipPodatka nTrenutniTipVarijable, VrstaVarijable nTrenutnaVrstaVarijable, UInt32 nTrenutnaAdresa, out TipPodatka iTipVarijable, out VrstaVarijable iVrstaVarijable, out UInt32 iAdresa)
		{
			iTipVarijable=null;
			iVrstaVarijable=null;
			iAdresa=0;
			
			ArrayList zapocinje=new ArrayList();
			zapocinje.Add("[");	zapocinje.Add(".");
			zapocinje.Add("^"); zapocinje.Add("else");
			zapocinje.Add("or"); zapocinje.Add("then");
			zapocinje.Add("of"); zapocinje.Add("<");
			zapocinje.Add("<>"); zapocinje.Add("and");
			zapocinje.Add("/");	zapocinje.Add("mod");
			zapocinje.Add(":"); zapocinje.Add(";");
			zapocinje.Add(">");	zapocinje.Add("..");
			zapocinje.Add("=");	zapocinje.Add(">=");
			zapocinje.Add("do"); zapocinje.Add("*");
			zapocinje.Add("+");	zapocinje.Add("]");
			zapocinje.Add(")");	zapocinje.Add("<=");
			zapocinje.Add(",");	zapocinje.Add("-");
			zapocinje.Add("div"); zapocinje.Add("to");
			zapocinje.Add("downto"); zapocinje.Add("end");
			zapocinje.Add("in"); zapocinje.Add("until");
			zapocinje.Add(":=");
			if (Provjeri(zapocinje,slijedi,"Nelegalan nastavak varijable!"))
			{

				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case "[":
					{
						#region	Produkcija <variable_suffix> ->  [  <subscript-list>  ]  <variable_suffix>
						PrimijenioProdukciju("<variable_suffix>","[  <subscript-list>  ]  <variable_suffix>");
						
						TipPodatka inTipVarijable1;
						VrstaVarijable inVrstaVarijable1;
						UInt32 inAdresa1;
						
						KonzumirajZavrsniZnak("[");
						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.AddRange(zapocinje);
						ArrayList slijedi2=new ArrayList(slijedi1);
						slijedi1.Add("]");
						
						Subscript_list(slijedi1, nTrenutniTipVarijable);
						SemantickiAnalizator.IzracunajElementNiza(nTrenutniTipVarijable, nTrenutnaVrstaVarijable, nTrenutnaAdresa, out inTipVarijable1, out inVrstaVarijable1, out inAdresa1);

						Provjeri("]",slijedi2,"Oèekivana desna uglata zagrada!");
						PojediZavrsniZnak("]");
						
						Variable_suffix(slijedi, inTipVarijable1, inVrstaVarijable1, inAdresa1, out iTipVarijable, out iVrstaVarijable, out iAdresa);
						#endregion
						break;
					}
					case ".":
					{
						#region	Produkcija <variable_suffix> ->  .  <fieldid>  <variable_suffix>
						PrimijenioProdukciju("<variable_suffix>",".  <fieldid>  <variable_suffix>");
		
						string inImePolja;
						VrstaVarijable inVrstaVarijable1;
						TipPodatka inTipVarijable1;
						UInt32 inAdresa1;

						KonzumirajZavrsniZnak(".");
						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.AddRange(zapocinje);
						
						Fieldid(slijedi1, out inImePolja);
						SemantickiAnalizator.IzracunajPolje(nTrenutniTipVarijable, nTrenutnaVrstaVarijable, nTrenutnaAdresa, inImePolja, out inTipVarijable1, out inVrstaVarijable1, out inAdresa1);
						Variable_suffix(slijedi, inTipVarijable1, inVrstaVarijable1, inAdresa1, out iTipVarijable, out iVrstaVarijable, out iAdresa);
						
						#endregion
						break;
					}
					case "^":
					{
						#region	Produkcija <variable_suffix> ->  ^  <variable_suffix>
						PrimijenioProdukciju("<variable_suffix>","^  <variable_suffix>");
						
						TipPodatka inTipVarijable1;
						VrstaVarijable inVrstaVarijable1;
						UInt32 inAdresa1;

						KonzumirajZavrsniZnak("^");
						SemantickiAnalizator.IzracunajDereferenciranje(nTrenutniTipVarijable, nTrenutnaVrstaVarijable, nTrenutnaAdresa, out inTipVarijable1, out inVrstaVarijable1, out inAdresa1);
						Variable_suffix(slijedi, inTipVarijable1, inVrstaVarijable1, inAdresa1, out iTipVarijable, out iVrstaVarijable, out iAdresa);
						
						#endregion
						break;
					}
					case "div":
					case ">":
					case "mod":
					case "<":
					case "=":
					case "do":
					case "end":
					case "*":
					case "+":
					case ")":
					case "/":
					case ",":
					case "until":
					case "of":						
					case "<>":
					case ">=":
					case ":=":
					case "<=":
					case "]":
					case "then":		
					case "and":
					case "or":
					case "-":
					case "downto":
					case "else":
					case "to":
					case ":":
					case ";":
					case OznakeZnakovaGramatike.KrajUlaznogNiza:
					{
						#region	Produkcija <variable_suffix> ->  <empty>
						PrimijenioProdukciju("<variable_suffix>","<empty>");
						Empty();

						iTipVarijable = nTrenutniTipVarijable;
						iVrstaVarijable = nTrenutnaVrstaVarijable;
						iAdresa = nTrenutnaAdresa;
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Nelegalan nastavak varijable!");
						break;
					}
				}	
			}
		}

		private void Constant(ArrayList slijedi, out TipPodatka iTipKonstante, out UInt32 iVrijednostKonstante)
		{
			iTipKonstante = null;
			iVrijednostKonstante = 0;
			
			ArrayList zapocinje=new ArrayList(_pocetakIzraza);
			if (Provjeri(zapocinje,slijedi,"Nelegalan poèetak konstante!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					
					case "not":
					case OznakeZnakovaGramatike.UnsignedReal:
					case OznakeZnakovaGramatike.UnsignedInteger:
					case OznakeZnakovaGramatike.Boolean:
					case "+":
					case "(":
					case OznakeZnakovaGramatike.Identifikator:
					case "nil":
					case "-":
					case "@":		
					{
						#region	Produkcija <constant> ->  <expression>
						PrimijenioProdukciju("<constant>","<expression>");
						
						bool inSveKonstantno;
		
						Expression(slijedi, out iTipKonstante, out iVrijednostKonstante, out inSveKonstantno);
						SemantickiAnalizator.ProvjeriKonstantnostIzraza(inSveKonstantno);
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Nelegalan poèetak konstante!");
						break;
					}
				}
			}
		}
		
		//DODAT SINT OPORAVAK
		private void Primary_expression1(ArrayList slijedi, string nImeIdentifikatora, out TipPodatka iTipPrimarnogIzraza, out UInt32 iVrijednostIzraza, out bool iSveKonstantno)
		{
			iTipPrimarnogIzraza=null;
			iVrijednostIzraza=0;
			iSveKonstantno=false;

			switch(TrenutniZnak.OznakaZnakaGramatike)
			{
				case "^":
				case ".":
				case "[":
				case "div":
				case ">":
				case "mod":
				case "<":
				case "=":		
				case "do":		
				case "end":
				case ">=":
				case "*":
				case "+":
				case ")":
				case "/":
				case ",":			
				case "until":
				case "of":
				case "<>":
				case "<=":				
				case "]":
				case "then":				
				case "and":
				case "or":
				case "-":
				case "downto":
				case "else":
				case "to":
				case ":":
				case ";":
				case OznakeZnakovaGramatike.KrajUlaznogNiza:
				{
					#region	Produkcija <primary-expression1> ->  <variable-suffix>
					PrimijenioProdukciju("<primary-expression1>","<variable-suffix>");
					
					//ArrayList slijedi = new ArrayList();

					TipPodatka inTrenutniTipVarijable, inTipVarijable;
					VrstaVarijable inTrenutnaVrstaVarijable, inVrstaVarijable;
					UInt32 inTrenutnaAdresa, inAdresa;

					SemantickiAnalizator.IzracunajVarijabluIzIdentifikatora(nImeIdentifikatora, out inTrenutniTipVarijable, out inTrenutnaVrstaVarijable, out inTrenutnaAdresa);
					
					Variable_suffix(slijedi, inTrenutniTipVarijable, inTrenutnaVrstaVarijable, inTrenutnaAdresa, out inTipVarijable, out inVrstaVarijable, out inAdresa);
					
					SemantickiAnalizator.EvaluirajVarijablu(inTipVarijable, inVrstaVarijable, inAdresa, out iVrijednostIzraza, out iSveKonstantno);

					iTipPrimarnogIzraza = inTipVarijable;

					#endregion
					break;
				}
				case "(":
				{
					#region	Produkcija <primary-expression1> ->  (  <expression-list>  )
					PrimijenioProdukciju("<primary-expression1>","(  <expression-list>  )");

					SemantickiAnalizator.PripremiPozivPotprograma(nImeIdentifikatora);
					KonzumirajZavrsniZnak("(");
					Expression_list(slijedi, nImeIdentifikatora);
					KonzumirajZavrsniZnak(")");
					SemantickiAnalizator.PozoviFunkciju(nImeIdentifikatora, out iTipPrimarnogIzraza);
					iVrijednostIzraza = 1;
					iSveKonstantno = false;

					#endregion
					break;
				}

				default:
				{
					FatalnaGreska();
					break;
				}
			}	
		}
		private void Primary_expression(ArrayList slijedi, out TipPodatka iTipPrimarnogIzraza, out UInt32 iVrijednostIzraza, out bool iSveKonstantno)
		{
			iTipPrimarnogIzraza=null;
			iVrijednostIzraza=0;
			iSveKonstantno=false;

			ArrayList zapocinje=new ArrayList(_pocetakIzraza);
			if (Provjeri(zapocinje,slijedi,"Greška u izrazu!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case OznakeZnakovaGramatike.Identifikator:
					{
						#region	Produkcija <primary-expression> ->  <variable-or-funcid> <primary-expression1>
						PrimijenioProdukciju("<primary-expression>","<variable-or-funcid> <primary-expression1>");

						string inImeIdentifikatora;

						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add("("); slijedi1.Add("^");
						slijedi1.Add("."); slijedi1.Add("[");
						slijedi1.Add("else"); slijedi1.Add("div");
						slijedi1.Add("then"); slijedi1.Add("or");
						slijedi1.Add("of"); slijedi1.Add("<");
						slijedi1.Add("<>");	slijedi1.Add("/");
						slijedi1.Add(","); slijedi1.Add("mod");
						slijedi1.Add(");");	slijedi1.Add(";");
						slijedi1.Add(">"); slijedi1.Add("..");
						slijedi1.Add(">=");	slijedi1.Add("=");
						slijedi1.Add("do");	slijedi1.Add("*");
						slijedi1.Add("+"); slijedi1.Add(")");
						slijedi1.Add("]"); slijedi1.Add("to");
						slijedi1.Add("<=");	slijedi1.Add("-");
						slijedi1.Add("and"); slijedi1.Add("downto");
						slijedi1.Add("end"); slijedi1.Add("in");
						slijedi1.Add("until");
						
						Variable_or_funcid(slijedi1, out inImeIdentifikatora);				
						Primary_expression1(slijedi, inImeIdentifikatora, out iTipPrimarnogIzraza, out iVrijednostIzraza, out iSveKonstantno);

						#endregion
						break;
					}
					case OznakeZnakovaGramatike.UnsignedInteger:
					{
						#region	Produkcija <primary-expression> ->  unsigned-integer
						PrimijenioProdukciju("<primary-expression>","unsigned-integer");

						UInt32 iVrijednost;
						KonzumirajZavrsniZnak(OznakeZnakovaGramatike.UnsignedInteger, out iVrijednost);
						
						iTipPrimarnogIzraza = KonstruktoriTipova.Integer();
						iVrijednostIzraza = iVrijednost;
						iSveKonstantno = true;
						
						#endregion
						break;
					}
					
						//OVDE TREBA DODAT OPORAVAK OD POGRESKE SINTAKSNOG ANALIZATORA
					case "@":
					{
						#region	Produkcija <primary-expression> ->  @  <varid>  <variable-suffix> 
						PrimijenioProdukciju("<primary-expression>","@ <varid> <variable-suffix>");
						
						//ArrayList slijedi = new ArrayList();
						ArrayList slijedi1 = new ArrayList();

						string inImeIdentifikatora;
						VrstaVarijable inTrenutnaVrstaVarijable, inVrstaVarijable;
						TipPodatka inTrenutniTipVarijable, inTipVarijable;
						UInt32 inTrenutnaAdresa, inAdresa;

						KonzumirajZavrsniZnak("@");
						Varid(slijedi, out inImeIdentifikatora);
						SemantickiAnalizator.IzracunajVarijabluIzIdentifikatora(inImeIdentifikatora, out inTrenutniTipVarijable, out inTrenutnaVrstaVarijable, out inTrenutnaAdresa);						
						Variable_suffix(slijedi1, inTrenutniTipVarijable, inTrenutnaVrstaVarijable, inTrenutnaAdresa, out inTipVarijable, out inVrstaVarijable, out inAdresa);
						SemantickiAnalizator.EvaluirajAdresuVarijable(inTipVarijable, inVrstaVarijable, inAdresa, out iTipPrimarnogIzraza, out iVrijednostIzraza, out iSveKonstantno);

						#endregion
						break;
					}

					case OznakeZnakovaGramatike.UnsignedReal:
					{
						#region	Produkcija <primary-expression> ->  unsigned-real
						PrimijenioProdukciju("<primary-expression>","unsigned-real");
						
						UInt32 iVrijednost;
						KonzumirajZavrsniZnak(OznakeZnakovaGramatike.UnsignedReal, out iVrijednost);
						
						iTipPrimarnogIzraza = KonstruktoriTipova.Real();
						iVrijednostIzraza = iVrijednost;
						iSveKonstantno = true;
						
						#endregion
						break;
					}
					case OznakeZnakovaGramatike.Boolean:
					{
						#region Produkcija <primary-expression> -> boolean-constant
						
						KonzumirajZavrsniZnak(OznakeZnakovaGramatike.Boolean, out iVrijednostIzraza);
						
						iTipPrimarnogIzraza = KonstruktoriTipova.Boolean();
						iSveKonstantno = true;

						#endregion
						break;
					}
					case "nil":
					{
						#region	Produkcija <primary-expression> ->  nil
						PrimijenioProdukciju("<primary-expression>","nil");
						
						KonzumirajZavrsniZnak("nil");
						SemantickiAnalizator.KreirajNilKonstantu(out iTipPrimarnogIzraza, out iVrijednostIzraza);
						iSveKonstantno = true;

						#endregion
						break;
					}
			
					case "(":
					{
						#region	Produkcija <primary-expression> ->  (  <expression>  )
						PrimijenioProdukciju("<primary-expression>","(  <expression>  )");
											
						KonzumirajZavrsniZnak("(");
						ArrayList slijedi1=new ArrayList(slijedi);
						slijedi1.Add(")");
						Expression(slijedi1, out iTipPrimarnogIzraza, out iVrijednostIzraza, out iSveKonstantno);
						
						Provjeri(")",slijedi,"Oèekivana desna zagrada!");
						PojediZavrsniZnak(")");
						#endregion
						break;
					}
					default:
					{
						PrijaviGresku("Greška u izrazu!");
						break;
					}
				}
			}
		}

		#endregion
	
		#region Dodani dio

		private void Variable_or_procid(ArrayList slijedi,out string inImeVarijableIliProcedure)
		{
			inImeVarijableIliProcedure="";
			if (Provjeri(OznakeZnakovaGramatike.Identifikator,slijedi,"Oèekivan identifikator!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case OznakeZnakovaGramatike.Identifikator:
					{
						#region Produkcija <variable-or-procid> -> identifier
						PrimijenioProdukciju("<Variable_or_procid>","identifier");
						KonzumirajZavrsniZnak("identifier",out inImeVarijableIliProcedure);
						SemantickiAnalizator.ProvjeriVarijabluIliProceduru(inImeVarijableIliProcedure);
						#endregion
						break;
					}
					default :
					{
						PrijaviGresku("Oèekivan identifikator!");
						break;
					}
				}
			}
		}

		private void Variable_or_funcid(ArrayList slijedi,out string inImeVarijableIliFunkcije)
		{
			inImeVarijableIliFunkcije="";
			if (Provjeri(OznakeZnakovaGramatike.Identifikator,slijedi,"Oèekivan identifikator!"))
			{
				switch(TrenutniZnak.OznakaZnakaGramatike)
				{
					case OznakeZnakovaGramatike.Identifikator:
					{
						#region Produkcija <variable-or-funcid> -> identifier
						PrimijenioProdukciju("<Variable_or_funcid>","identifier");
						KonzumirajZavrsniZnak("identifier",out inImeVarijableIliFunkcije);
						SemantickiAnalizator.ProvjeriVarijabluIliFunkciju(inImeVarijableIliFunkcije);
						#endregion
						break;
					}
					default :
					{
						PrijaviGresku("Oèekivan identifikator!");
						break;
					}
				}
			}
		}


		#endregion

		#region Privatna polja
		ArrayList _pocetakBloka;
		ArrayList _pocetakNaredbe;
		ArrayList _pocetakIzraza;
		ArrayList _pocetakTipa;
		ArrayList _pocetakVarijante;
		ArrayList _pocetakNaredbe1;
		ArrayList _pocetakIzraza1;
		#endregion
		
	}
}
