================================================================================
=== Ovdje ce biti objasnjene neke stvari oko dizajna aplikacije, kao i neke  ===
===                  sitnice na koje bi bio dobro paziti                     ===
================================================================================

-----------------------------Opcenite napomene----------------------------------

- treba paziti kad se dodaju nove datotke i klase u poddirektorij projekta 
  (recimo LeksickaAnaliza). U tom slucaju se automatski dodijeli namespace koji
  ukljucuje naziv tog direktorija, a sve klase bi trebale biti pod jednim 
  namespaceom (namespace JezicniProcesor).
  
- klase ne treba oznacavati sa "internal" modifikatorom (npr. "internal class 
  UniformniZnak"). Ako se napise samo "class UniformniZnak", taj se modifikator
  podrazumijeva. Inace, taj modifikator znaci da je klasa vidljiva samo u
  njenom projektu (npr. vidljiva je u projektu LanguageProcessor, ali nije u 
  projektu UserInterface).

- treba izbjegavati koristenje stringova kako bi se razmjenjivali podaci medju 
  razlicitim djelovima aplikacije. Primjerice, nije dobro koristiti string 
  "integer" kako bi se u leksickom analizatoru oznacilo da je neka leksicka 
  jedinka tipa integer zbog toga što je onda potrebno kopati po kodu leksickog
  analizatora da bi se u drugim dijelovima aplikacije znalo sto se moze
  ocekivati kao vrijednost. Puno je bolje koristiti enumeraciju i pobrojati sve
  vrijednosti koje se mogu pojaviti. Za primjer vidjeti datoteku 
  LeksickaJedinka.cs
  
- u slucaju iznimke od gornjeg pravila, ako se to moze, treba na jednom mjestu
  definirati koji se stringovi mogu pojaviti kao vrijednost. Za primjer 
  pogledati datoteku ZnakGramatike.cs (klasa OznakeZnakovaGramatike).


-------------------------------Naming guidelines--------------------------------

- imena enumeracijskih tipova trebaju zavrsavati sa enum 
  (npr. 'UniformniZnakEnum')
- imena klasa, metoda i svojstava (property) pocinju velikim slovom 
  (npr. 'LeksickiAnalizator.Analiziraj')
- imena lokalnih varijabli i parametara zapocinju malim slovom 
  (npr. 'nullStanje')
- imena member varijabli, dakle varijabli koje pripadaju klasi (field, polje) 
  zapocinju znakom '_' (underscore),  nakon cega slijedi malo slobo. Npr:
	class ImeKlase
	{
		public ImeKlase()
		{
			//konstruktor
		}

		private bool _imeMemberVarijable;
	}

- imena public svojstava i metoda se pisu na vrhu klase, protected ispod njih, 
  private jos ispod. Private varijable klase (polja) se pisu na samom dnu klase.
  Npr:
  	class ImeKlase
	{
		public ImeKlase()
		{ /*konstruktor*/ }
		
		public void JavnaMetoda()
		{}		
		protected void ZasticenaMetoda()
		{}		
		private void PrivatnaMetoda()
		{}
		private bool _imeMemberVarijable;
	}