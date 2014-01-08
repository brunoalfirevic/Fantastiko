using System;
using System.Collections;

namespace JezicniProcesor
{
	/// <summary>
	/// Predstavlja tablicu prijelaza DKA koji se koristi pri leksickoj analizi
	/// </summary>
	class DKATablica
	{
		public DKATablica()
		{
		}
		
		/// <summary>
		/// Vraca objekt koji predstavlja stanje u koje treba prijeci.
		/// Ako nije definiran prijelaz sutomat prelazi u nullStanje
		/// </summary>
		public  DKAStanje DajPrijelaz(DKAStanje trenutnoStanje, char ulazniZnak)
		{		
			switch (trenutnoStanje.KodStanja)
			{
				case "Pocetno":						
				{
					#region Prijelazi za pocetno stanje
					if ((char.IsLetter(ulazniZnak)) || (ulazniZnak=='_') || (ulazniZnak=='$'))
					{
						DKAStanje novoStanje = new DKAStanje(false,true,"IDNiliKR");
						return novoStanje;
					}
					else if ((char.IsDigit(ulazniZnak)))
					{
						DKAStanje novoStanje = new DKAStanje(false,true,"KONST");
						return novoStanje;
					}
					else if (ulazniZnak=='\'')
					{
						DKAStanje novoStanje = new DKAStanje(false,false,"STRING");
						return novoStanje;
					}
					else if (ulazniZnak=='+' || ulazniZnak=='-')
					{
						DKAStanje novoStanje = new DKAStanje(false,true,"SIMBOL+-");
						return novoStanje;
					}
					else if (ulazniZnak==':')
					{
						DKAStanje novoStanje = new DKAStanje(false,true,"SIMBOL:");
						return novoStanje;
					}
					else if (ulazniZnak=='.')
					{
						DKAStanje novoStanje = new DKAStanje(false,true,"SIMBOL.");
						return novoStanje;
					}
					else if (ulazniZnak=='<')
					{
						DKAStanje novoStanje = new DKAStanje(false,true,"SIMBOL<");
						return novoStanje;
					}
					else if (ulazniZnak=='>')
					{
						DKAStanje novoStanje = new DKAStanje(false,true,"SIMBOL>");
						return novoStanje;
					}
					else if ("*/=[],;()^@".IndexOf(ulazniZnak)!=-1)		//ako je jedan od znakova u stringu idi u stanje SIMBOL 
					{
						DKAStanje novoStanje = new DKAStanje(false,true,"SIMBOL");
						return novoStanje;
					}
					else if (ulazniZnak=='{')
					{
						DKAStanje novoStanje = new DKAStanje(false,true,"KOMENTAR");
						return novoStanje;
					}
					else
					{
						DKAStanje novoStanje = new DKAStanje(true,false,"nullStanje");
						return novoStanje;
					}
					#endregion					
				}
				case "IDNiliKR":
				{
					#region Prijelazi za IDNiliKR stanje
					if ((char.IsLetter(ulazniZnak)) || (ulazniZnak=='_') || (ulazniZnak=='$') || (char.IsDigit(ulazniZnak)))
					{
						return trenutnoStanje;
					}
					else
					{
						DKAStanje novoStanje=new DKAStanje(true,false,"nullStanje"); 
						return novoStanje;
					}
					#endregion
				}
				case "KONST":
				{
					#region Prijelazi za KONST stanje
					if (char.IsDigit(ulazniZnak))
					{
						return trenutnoStanje;
					}
					else if (ulazniZnak=='.')
					{
						DKAStanje novoStanje=new DKAStanje(false,false,"KONST."); 
						return novoStanje;
					}
					else if (ulazniZnak=='E' || ulazniZnak=='e')
					{
						DKAStanje novoStanje=new DKAStanje(false,false,"KONSTe"); 
						return novoStanje;
					}
					else
					{
						DKAStanje novoStanje=new DKAStanje(true,false,"nullStanje"); 
						return novoStanje;
					}
					#endregion
				}
				case "KONST.":
				{
					#region Prijelazi za KONST. stanje
					if (char.IsDigit(ulazniZnak))
					{
						DKAStanje novoStanje=new DKAStanje(false,true,"KONSTre1"); 
						return novoStanje;
					}
					else
					{
						DKAStanje novoStanje=new DKAStanje(true,false,"nullStanje"); 
						return novoStanje;
					}
					#endregion
				}
				case "KONSTe":
				{
					#region Prijelazi za KONSTe stanje
					if (char.IsDigit(ulazniZnak))
					{
						DKAStanje novoStanje=new DKAStanje(false,true,"KONSTre2"); 
						return novoStanje;
					}
					else if(ulazniZnak=='+' || ulazniZnak=='-')
					{
						DKAStanje novoStanje=new DKAStanje(false,false,"KONST+-");
						return novoStanje;
					}
					else
					{
						DKAStanje novoStanje=new DKAStanje(true,false,"nullStanje"); 
						return novoStanje;
					}
					#endregion
				}
				case "KONST+-":
				{
					#region Prijelazi za KONST+- stanje
					if (char.IsDigit(ulazniZnak))
					{
						DKAStanje novoStanje=new DKAStanje(false,true,"KONSTre2"); 
						return novoStanje;
					}
					else
					{
						DKAStanje novoStanje=new DKAStanje(true,false,"nullStanje"); 
						return novoStanje;
					}
					#endregion
				}
				case "KONSTre1":
				{
					#region Prijelazi za KONSTre1 stanje
					if (char.IsDigit(ulazniZnak))
					{
						return trenutnoStanje;
					}
					else if (ulazniZnak=='E' || ulazniZnak=='e')
					{
						DKAStanje novoStanje=new DKAStanje(false,false,"KONSTe"); 
						return novoStanje;
					}
					else
					{
						DKAStanje novoStanje=new DKAStanje(true,false,"nullStanje"); 
						return novoStanje;
					}
					#endregion
				}
				case "KONSTre2":
				{
					#region Prijelazi za KONSTre2 stanje
					if (char.IsDigit(ulazniZnak))
					{
						return trenutnoStanje;
					}
					else
					{
						DKAStanje novoStanje=new DKAStanje(true,false,"nullStanje"); 
						return novoStanje;
					}
					#endregion
				}
				case "STRING":
				{
					#region Prijelazi za STRING stanje
					if (ulazniZnak=='\'')
					{
						DKAStanje novoStanje=new DKAStanje(false,true,"STRINGKRAJ"); 
						return novoStanje;
					}
					else if (ulazniZnak==(char)13 || ulazniZnak==(char)10 || ulazniZnak==(char)0)
					{
						DKAStanje novoStanje = new DKAStanje(true,false,"nullStanje");
						return novoStanje;					
					}
					else
					{
						return trenutnoStanje;					
					}
					#endregion
				}
				case "STRINGKRAJ":
				{
					#region Prijelazi za STRINGKRAJ stanje
					if (ulazniZnak=='\'')
					{
						DKAStanje novoStanje=new DKAStanje(false,false,"STRING"); 
						return novoStanje;
					}
					else 
					{
						DKAStanje novoStanje=new DKAStanje(true,false,"nullStanje"); 
						return novoStanje;					
					}
					#endregion
				} 
				case "SIMBOL:":
				{
					#region Prijelazi za SIMBOL: stanje
					if (ulazniZnak=='=')
					{
						DKAStanje novoStanje = new DKAStanje(false,true,"SIMBOL:=");
						return novoStanje;
					}
					else
					{
						DKAStanje novoStanje = new DKAStanje(true,false,"nullStanje");
						return novoStanje;
					}
					#endregion
				}
				case "SIMBOL<":
				{
					#region Prijelazi za SIMBOL< stanje
					if (ulazniZnak=='=')
					{
						DKAStanje novoStanje = new DKAStanje(false,true,"SIMBOL<=");
						return novoStanje;
					}
					else if (ulazniZnak=='>')
					{
						DKAStanje novoStanje = new DKAStanje(false,true,"SIMBOL<>");
						return novoStanje;
					}
					else
					{
						DKAStanje novoStanje = new DKAStanje(true,false,"nullStanje");
						return novoStanje;
					}
					#endregion
				}
				case "SIMBOL>":
				{
					#region Prijelazi za SIMBOL> stanje
					if (ulazniZnak=='=')
					{
						DKAStanje novoStanje = new DKAStanje(false,true,"SIMBOL>=");
						return novoStanje;
					}
					else
					{
						DKAStanje novoStanje = new DKAStanje(true,false,"nullStanje");
						return novoStanje;
					}
					#endregion
				}
				case "SIMBOL.":
				{
					#region Prijelazi za SIMBOL. stanje
					if (ulazniZnak=='.')
					{
						DKAStanje novoStanje = new DKAStanje(false,true,"SIMBOL..");
						return novoStanje;
					}
					else
					{
						DKAStanje novoStanje = new DKAStanje(true,false,"nullStanje");
						return novoStanje;
					}
					#endregion						
				}

				case "KOMENTAR":
				{
					#region Prijelazi za KOMENTAR stanje
					if (ulazniZnak=='}')
					{
						DKAStanje novoStanje = new DKAStanje(false,true,"KOMENTARKRAJ");
						return novoStanje;
					}
					else if(ulazniZnak==(char)0)
					{
						DKAStanje novoStanje = new DKAStanje(true,false,"nullStanje");
						return novoStanje;
					}
					else
					{
						return trenutnoStanje;
					}
					#endregion
				}
				default:
				{
					#region Ako nije definiran nijedan prijelaz prijedji u null stanje
					//Za stanja koja nemaju prijelaze
					DKAStanje novoStanje = new DKAStanje(true,false,"nullStanje");
					return novoStanje;
					#endregion
				}
			}
		}
	}
}
