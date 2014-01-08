using System;
using System.Data;
using System.IO;
using System.Text;
using System.Collections;

namespace JezicniProcesor
{
	class Instrukcija
	{
		public String mnem;
		public int kod,n;
		public uint x;
		public Instrukcija(String mnem,int kod,int n,uint x)
		{
			this.mnem=mnem;
			this.kod=kod;
			this.n=n;
			this.x=x;
		}
	}
	class PKodLabela
	{
		public int broj;
		public String labela;
		public PKodLabela(String labela,int broj)
		{
			this.broj=broj;
			this.labela=labela;
		}
	}

	public class GeneratorARMKoda
	{
		public GeneratorARMKoda()
		{
		}

		public void GenerirajARMKod(string pKod, int STOG)
		{
			try
			{
				_ARMKod="";
				_opisGreske="";
				_dosloDoGreske=false;
				StringReader sin=new StringReader(pKod);

				String Line;
				String[] element;
				int n;
				uint X;
		
				int PC;
				int brojac=0;


				ArrayList kodovi=new ArrayList();
				ArrayList labele1=new ArrayList();
				ArrayList labele2=new ArrayList();
				ArrayList p_labele=new ArrayList();
				ArrayList p_labele1=new ArrayList();
				ArrayList p_labele2=new ArrayList();

				while ((Line=sin.ReadLine())!=null)
				{
					if (Line.Trim().Length==0||Line.Trim().StartsWith("//"))
						continue;
					if (Line[0]!='\t')
					{
						if (Line.Trim().EndsWith(":"))
						{
							labele1.Add(new PKodLabela(Line.Remove(Line.Length-1,1),brojac));
						} 
						else 
						{
							element=Line.Split(' ','\t');
							p_labele.Add(new PKodLabela(element[0],Convert.ToInt32(element[1],16)));
						}
					} 
					else 
					{
						element=Line.Split(' ','\t');
						brojac++;
				
						switch (element[1].ToUpper())
						{
							case "LIT":
								if (element[2].StartsWith("0x"))
								{
									X=Convert.ToUInt32(element[2],16);
									kodovi.Add(new Instrukcija("LIT",1,0,X));
								} 
								else 
								{
									p_labele2.Add(new PKodLabela(element[2],brojac-1));
									kodovi.Add(new Instrukcija("LIT",1,0,0));
								}
								break;
							case "LDA":
								n=Convert.ToInt32(element[2],16);
								X=Convert.ToUInt32(element[3],16);
								kodovi.Add(new Instrukcija("LDA",2,n,X));
								break;
							case "LOD":
								n=Convert.ToInt32(element[2],16);
								X=Convert.ToUInt32(element[3],16);
								kodovi.Add(new Instrukcija("LOD",3,n,X));
								break;
							case "STO":
								n=Convert.ToInt32(element[2],16);
								X=Convert.ToUInt32(element[3],16);
								kodovi.Add(new Instrukcija("STO",4,n,X));
								break;
							case "LID":
								n=Convert.ToInt32(element[2],16);
								X=Convert.ToUInt32(element[3],16);
								kodovi.Add(new Instrukcija("LID",5,n,X));
								break;
							case "SID":
								n=Convert.ToInt32(element[2],16);
								X=Convert.ToUInt32(element[3],16);
								kodovi.Add(new Instrukcija("SID",6,n,X));
								break;
							case "LTI":
								kodovi.Add(new Instrukcija("LTI",7,0,0));
								break;
							case "STI":
								kodovi.Add(new Instrukcija("STI",8,0,0));
								break;
							case "LOD_B":
								n=Convert.ToInt32(element[2],16);
								X=Convert.ToUInt32(element[3],16);
								kodovi.Add(new Instrukcija("LOD_B",9,n,X));
								break;
							case "STO_B":
								n=Convert.ToInt32(element[2],16);
								X=Convert.ToUInt32(element[3],16);
								kodovi.Add(new Instrukcija("STO_B",10,n,X));
								break;
							case "LID_B":
								n=Convert.ToInt32(element[2],16);
								X=Convert.ToUInt32(element[3],16);
								kodovi.Add(new Instrukcija("LID_B",11,n,X));
								break;
							case "SID_B":
								n=Convert.ToInt32(element[2],16);
								X=Convert.ToUInt32(element[3],16);
								kodovi.Add(new Instrukcija("SID_B",12,n,X));
								break;
							case "LTI_B":
								kodovi.Add(new Instrukcija("LTI_B",13,0,0));
								break;
							case "STI_B":
								kodovi.Add(new Instrukcija("STI_B",14,0,0));
								break;
							case "JMP":
								kodovi.Add(new Instrukcija("JMP",15,0,0));
								labele2.Add(new PKodLabela(element[2],brojac-1));
								break;
							case "JPC":
								kodovi.Add(new Instrukcija("JPC",16,0,0));
								labele2.Add(new PKodLabela(element[2],brojac-1));
								break;
							case "JPN":
								kodovi.Add(new Instrukcija("JPN",116,0,0));
								labele2.Add(new PKodLabela(element[2],brojac-1));
								break;
							case "MST":
								n=Convert.ToInt32(element[2],16);
								kodovi.Add(new Instrukcija("MST",117,n,0));
								break;
							case "CAL":
								n=Convert.ToInt32(element[2],16);
								kodovi.Add(new Instrukcija("CAL",217,n,0));
								labele2.Add(new PKodLabela(element[3],brojac-1));
								break;
							case "ENT":
								if (element[2].StartsWith("0x"))
								{
									X=Convert.ToUInt32(element[2],16);
									kodovi.Add(new Instrukcija("ENT",317,0,X));
								} 
								else 
								{
									p_labele2.Add(new PKodLabela(element[2],brojac-1));
									kodovi.Add(new Instrukcija("ENT",317,0,0));
								}
								break;
							case "RTP":
								kodovi.Add(new Instrukcija("RTP",18,0,0));
								break;
							case "RTF":
								kodovi.Add(new Instrukcija("RTF",19,0,0));
								break;

							case "NOT":
								kodovi.Add(new Instrukcija("NOT",20,0,0));
								break;
							case "OR":
								kodovi.Add(new Instrukcija("OR",21,0,0));
								break;
							case "AND":
								kodovi.Add(new Instrukcija("AND",22,0,0));
								break;
							case "XOR":
								kodovi.Add(new Instrukcija("XOR",23,0,0));
								break;
							case "ODD":
								kodovi.Add(new Instrukcija("ODD",24,0,0));
								break;

							case "EQL_R":
								kodovi.Add(new Instrukcija("EQL_R",25,0,0));
								break;
							case "EQL_I":
								kodovi.Add(new Instrukcija("EQL_I",26,0,0));
								break;
							case "LSS_R":
								kodovi.Add(new Instrukcija("LSS_R",27,0,0));
								break;
							case "LSS_I":
								kodovi.Add(new Instrukcija("LSS_I",28,0,0));
								break;
							case "LEQ_R":
								kodovi.Add(new Instrukcija("LEQ_R",29,0,0));
								break;
							case "LEQ_I":
								kodovi.Add(new Instrukcija("LEQ_I",30,0,0));
								break;
							case "GTR_R":
								kodovi.Add(new Instrukcija("GTR_R",31,0,0));
								break;
							case "GTR_I":
								kodovi.Add(new Instrukcija("GTR_I",32,0,0));
								break;
							case "GEQ_R":
								kodovi.Add(new Instrukcija("GEQ_R",33,0,0));
								break;
							case "GEQ_I":
								kodovi.Add(new Instrukcija("GEQ_I",34,0,0));
								break;
							case "NEQ_R":
								kodovi.Add(new Instrukcija("NEQ_R",35,0,0));
								break;
							case "NEQ_I":
								kodovi.Add(new Instrukcija("NEQ_I",36,0,0));
								break;

							case "NEG_R":
								kodovi.Add(new Instrukcija("NEG_R",37,0,0));
								break;
							case "NEG_I":
								kodovi.Add(new Instrukcija("NEG_I",38,0,0));
								break;
							case "ADD_R":
								kodovi.Add(new Instrukcija("ADD_R",39,0,0));
								break;
							case "ADD_I":
								kodovi.Add(new Instrukcija("ADD_I",40,0,0));
								break;
							case "SUB_R":
								kodovi.Add(new Instrukcija("SUB_R",41,0,0));
								break;
							case "SUB_I":
								kodovi.Add(new Instrukcija("SUB_I",42,0,0));
								break;
							case "MUL_R":
								kodovi.Add(new Instrukcija("MUL_R",43,0,0));
								break;
							case "MUL_I":
								kodovi.Add(new Instrukcija("MUL_I",44,0,0));
								break;
							case "DIV_R":
								kodovi.Add(new Instrukcija("DIV_R",45,0,0));
								break;
							case "DIV_I":
								kodovi.Add(new Instrukcija("DIV_I",46,0,0));
								break;
							case "MOD":
								kodovi.Add(new Instrukcija("MOD",47,0,0));
								break;
							case "FLT":
								kodovi.Add(new Instrukcija("FLT",48,0,0));
								break;
							case "INT":
								if (element[2].StartsWith("0x"))
								{
									X=Convert.ToUInt32(element[2],16);
									kodovi.Add(new Instrukcija("INT",49,0,X));
								} 
								else 
								{
									p_labele2.Add(new PKodLabela(element[2],brojac-1));
									kodovi.Add(new Instrukcija("INT",49,0,0));
								}
								break;
							case "DUP":
								kodovi.Add(new Instrukcija("DUP",50,0,0));
								break;
							case "HLT":
								kodovi.Add(new Instrukcija("HLT",0,0,0));
								break;
							case "WRT_S":
								kodovi.Add(new Instrukcija("WRT_S",51,0,0));
								break;
							case "WRT_C":
								kodovi.Add(new Instrukcija("WRT_C",52,0,0));
								break;
							case "WRT_I":
								kodovi.Add(new Instrukcija("WRT_I",53,0,0));
								break;
							case "WRT_R":
								kodovi.Add(new Instrukcija("WRT_R",54,0,0));
								break;
							case "RED_S":
								X=Convert.ToUInt32(element[2],16);
								kodovi.Add(new Instrukcija("RED_S",55,0,X));
								break;
							case "RED_C":
								kodovi.Add(new Instrukcija("RED_C",56,0,0));
								break;
							case "RED_I":
								kodovi.Add(new Instrukcija("RED_I",57,0,0));
								break;
							case "RED_R":
								kodovi.Add(new Instrukcija("RED_R",58,0,0));
								break;
							case "INC":
								kodovi.Add(new Instrukcija("INC",59,0,0));
								break;
							case "DEC":
								kodovi.Add(new Instrukcija("DEC",60,0,0));
								break;
							default:
								break;
						}
					}
				}
				sin.Close();


				//povezivanje podatkovnih labela i labela u naredbama		
				foreach(PKodLabela l in p_labele2)
				{
					int poz=-1;
					for(int i=0;i<p_labele.Count;i++)
					{
						if (((PKodLabela)p_labele[i]).labela==l.labela)
							poz=i;
					}
					((Instrukcija)kodovi[l.broj]).x=(uint)((PKodLabela)p_labele[poz]).broj;
				}


				StringBuilder sb=new StringBuilder();
				StringWriter sout=new StringWriter(sb);

				sout.WriteLine("\tAREA program, CODE, READWRITE");
				sout.WriteLine("\tENTRY");
				sout.WriteLine("\tLDR\tR13, stog");
				sout.WriteLine("\tLDR\tR8, br");
				sout.WriteLine("\tB\tglavni");
				sout.WriteLine("stog\tDCD\t"+STOG);
				sout.WriteLine("br\tDCD\t"+STOG);
				sout.WriteLine();
				sout.Write("glavni");
				N=8;
				vrh=N-1;
				nstog=0;
				bool dijeli_=false;
				ArrayList konstante=new ArrayList();
				int int_konst=0;

				for(PC=0;PC<kodovi.Count;PC++)
				{
					Instrukcija instr=(Instrukcija)kodovi[PC];
					sout.WriteLine(get_labela(labele1,PC));
					switch (instr.kod)
					{
						case 0: //HLT
							sout.WriteLine("\tMOV\tR0, #0x18"); 
							sout.WriteLine("\tLDR\tR1, =0x20026"); 
							sout.WriteLine("\tSWI\t0x123456"); 
							break;
						case 1: //LIT x
							push(sout);
							X=instr.x;
							if (X<=255&&X>=0)
							{
								sout.WriteLine("\tMOV\tR"+vrh+",#"+X);
							} 
							else if (~X<=255&&~X>=0)
							{
								sout.WriteLine("\tMNV\tR"+vrh+",#"+(~X));
							}
							else 
							{
								konstante.Add("K"+int_konst+"\tDCD\t"+X);
								sout.WriteLine("\tLDR\tR"+vrh+", K"+int_konst);
								int_konst++;
							}
							break;
						case 2: //LDA n,x
							push(sout);
							if (instr.n==0)
							{
								sout.WriteLine("\tADD\tR"+vrh+", R8, #"+instr.x);
							} 
							else 
							{
								baza(instr.n,sout);
								sout.WriteLine("\tADD\tR"+vrh+", R10, #"+instr.x);
							}
							break;
						case 3: //LOD n,x
							push(sout);
							if (instr.n==0)
							{
								sout.WriteLine("\tLDR\tR"+vrh+", [R8,#"+instr.x+"]");
							} 
							else 
							{
								baza(instr.n,sout);
								sout.WriteLine("\tLDR\tR"+vrh+", [R10,#"+instr.x+"]");
							}
							break;
						case 4: //STO n,x
							uskladi(1,sout);
							vrh1=(vrh-1+N)%N;
							if (instr.n==0)
							{
								sout.WriteLine("\tSTR\tR"+vrh+", [R8,#"+instr.x+"]");
							} 
							else 
							{
								baza(instr.n,sout);
								sout.WriteLine("\tSTR\tR"+vrh+", [R10,#"+instr.x+"]");
							}
							pop(sout);
							break;
						case 5: //LID n,x
							if (instr.n==0)
							{
								sout.WriteLine("\tLDR\tR9, [R8,#"+instr.x+"]");
							} 
							else 
							{
								baza(instr.n,sout);
								sout.WriteLine("\tLDR\tR9, [R10, #"+instr.x+"]");
							}
							sout.WriteLine("\tLDR\tR"+vrh+", [R9]");
							break;
						case 6: //SID n,x
							uskladi(1,sout);
							vrh1=(vrh-1+N)%N;
							if (instr.n==0)
							{
								sout.WriteLine("\tLDR\tR9, [R8,#"+instr.x+"]");
							} 
							else 
							{
								baza(instr.n,sout);
								sout.WriteLine("\tLDR\tR9, [R10, #"+instr.x+"]");
							}
							sout.WriteLine("\tSTR\tR"+vrh+", [R9]");
							pop(sout);
							break;
						case 7: //LTI
							uskladi(1,sout);
							sout.WriteLine("\tLDR\tR"+vrh+", [R"+vrh+"]");
							break;
						case 8: //STI
							uskladi(2,sout);
							vrh1=(vrh-1+N)%N;
							sout.WriteLine("\tSTR\tR"+vrh+", [R"+vrh1+"]");
							pop(sout);
							break;
						case 9: //LOD_B n,x
							push(sout);
							if (instr.n==0)
							{
								sout.WriteLine("\tLDRB\tR"+vrh+", [R8,#"+instr.x+"]");
							} 
							else 
							{
								baza(instr.n,sout);
								sout.WriteLine("\tLDRB\tR"+vrh+", [R10,#"+instr.x+"]");
							}
							break;
						case 10: //STO_B n,x
							uskladi(1,sout);
							vrh1=(vrh-1+N)%N;
							if (instr.n==0)
							{
								sout.WriteLine("\tSTRB\tR"+vrh+", [R8,#"+instr.x+"]");
							} 
							else 
							{
								baza(instr.n,sout);
								sout.WriteLine("\tSTRB\tR"+vrh+", [R10,#"+instr.x+"]");
							}
							pop(sout);
							break;
						case 11: //LID_B n,x
							push(sout);
							if (instr.n==0)
							{
								sout.WriteLine("\tLDR\tR9, [R8,#"+instr.x+"]");
							} 
							else 
							{
								baza(instr.n,sout);
								sout.WriteLine("\tLDR\tR9, [R10, #"+instr.x+"]");
							}
							sout.WriteLine("\tLDRB\tR"+vrh+", [R9]");
							break;
						case 12: //SID_B n,x
							uskladi(1,sout);
							vrh1=(vrh-1+N)%N;
							if (instr.n==0)
							{
								sout.WriteLine("\tLDR\tR9, [R8,#"+instr.x+"]");
							} 
							else 
							{
								baza(instr.n,sout);
								sout.WriteLine("\tLDR\tR9, [R10, #"+instr.x+"]");
							}
							sout.WriteLine("\tSTRB\tR"+vrh+", [R9]");
							pop(sout);
							break;
						case 13: //LTI_B
							uskladi(1,sout);
							sout.WriteLine("\tLDRB\tR"+vrh+", [R"+vrh+"]");
							break;
						case 14: //STI_B
							uskladi(2,sout);
							vrh1=(vrh-1+N)%N;
							sout.WriteLine("\tSTR\tR"+vrh+", [R"+vrh1+"]");
							pop(sout);
							break;
						case 15: //JMP x
							sout.WriteLine("\tB\t"+get_labela(labele2,PC));
							break;
						case 16: //JPC x
							uskladi(1,sout);
							vrh1=(vrh-1+N)%N;
							sout.WriteLine("\tCMP\tR"+vrh+", #0");
							pop(sout);
							sout.WriteLine("\tBNE\t"+get_labela(labele2,PC));
							break;
						case 116: //JPN x
							uskladi(1,sout);
							vrh1=(vrh-1+N)%N;
							sout.WriteLine("\tCMP\tR"+vrh+", #0");
							pop(sout);
							sout.WriteLine("\tBEQ\t"+get_labela(labele2,PC));
							break;
						case 117: //MST n
							nstog_=nstog;
							isprazni(sout);
							sout.WriteLine("\tADD\tR13, #4");

							if (instr.n==0)
							{
								sout.WriteLine("\tSTMEA\tR13!, R8");
							} 
							else 
							{
								baza(instr.n,sout);
								sout.WriteLine("\tSTMEA\tR13!, R10");
							}
							sout.WriteLine("\tSTMEA\tR13!, R8");
							break;
						case 217: //CAL n,x
							isprazni(sout);
							sout.WriteLine("\tSUB\tR8, R13, #"+(instr.n+16));
							sout.WriteLine("\tBL\t"+get_labela(labele2,PC));
							String tmp=get_labela(labele2,PC);
							int start_=0;
							foreach(PKodLabela l in labele1)
							{
								if (l.labela==tmp)
								{
									start_=l.broj;
									break;
								}
							}
							while(((Instrukcija)kodovi[start_]).kod!=18&&((Instrukcija)kodovi[start_]).kod!=19)
								start_++;
							if (((Instrukcija)kodovi[start_]).kod==18)
							{
								nstog=nstog_;
							} 
							else
							{
								nstog=nstog_+1;
							}
							napuni(sout);
							break;
						case 317: //ENT x
							vrh=N-1;
							nstog=0;
							sout.WriteLine("\tSTR\tR14, [R8,#12]");
							sout.WriteLine("\tADD\tR13, R8, #"+instr.x);
							break;
						case 18: //RTP
							sout.WriteLine("\tMOV\tR13, R8");
							sout.WriteLine("\tMOV\tR8, [R13,#8]");
							sout.WriteLine("\tMOV\tR15, [R13,#12]");
							break;
						case 19: //RTF
							sout.WriteLine("\tADD\tR13, R8, #4");
							sout.WriteLine("\tMOV\tR8, [R13,#4]");
							sout.WriteLine("\tMOV\tR15, [R13,#8]");
							break;

						case 20: //NOT
							uskladi(1,sout);
							sout.WriteLine("\tEOR\tR"+vrh+", R"+vrh+", #0"); 
							break;
						case 21: //OR
							uskladi(2,sout);
							vrh1=(vrh-1+N)%N;
							sout.WriteLine("\tORR\tR"+vrh1+", R"+vrh1+", R"+vrh); 
							pop(sout);
							break;
						case 22: //AND
							uskladi(2,sout);
							vrh1=(vrh-1+N)%N;
							sout.WriteLine("\tAND\tR"+vrh1+", R"+vrh1+", R"+vrh); 
							pop(sout);
							break;
						case 23: //XOR
							uskladi(2,sout);
							vrh1=(vrh-1+N)%N;
							sout.WriteLine("\tEOR\tR"+vrh1+", R"+vrh1+", R"+vrh); 
							pop(sout);
							break;
						case 24: //ODD
							uskladi(1,sout);
							sout.WriteLine("\tAND\tR"+vrh+", R"+vrh+", #1"); 
							break;

						case 25: //EQL_R
							uskladi(2,sout);
							vrh1=(vrh-1+N)%N;
							sout.WriteLine("\tCMP\tR"+vrh1+", R"+vrh); 
							sout.WriteLine("\tMOVEQ\tR"+vrh1+", #1"); 
							sout.WriteLine("\tMOVNE\tR"+vrh1+", #0"); 
							pop(sout);
							break;
						case 26: //EQL_I
							uskladi(2,sout);
							vrh1=(vrh-1+N)%N;
							sout.WriteLine("\tCMP\tR"+vrh1+", R"+vrh); 
							sout.WriteLine("\tMOVEQ\tR"+vrh1+", #1"); 
							sout.WriteLine("\tMOVNE\tR"+vrh1+", #0"); 
							pop(sout);
							break;
						case 27: //LSS_R
							uskladi(2,sout);
							sout.WriteLine("; "+instr.mnem+": Nije implementirano!"); 
							vrh1=(vrh-1+N)%N;
							pop(sout);
							break;
						case 28: //LSS_I
							uskladi(2,sout);
							vrh1=(vrh-1+N)%N;
							sout.WriteLine("\tCMP\tR"+vrh1+", R"+vrh); 
							sout.WriteLine("\tMOVLT\tR"+vrh1+", #1"); 
							sout.WriteLine("\tMOVGE\tR"+vrh1+", #0"); 
							pop(sout);
							break;
						case 29: //LEQ_R
							uskladi(2,sout);
							sout.WriteLine("; "+instr.mnem+": Nije implementirano!"); 
							vrh1=(vrh-1+N)%N;
							pop(sout);
							break;
						case 30: //LEQ_I
							uskladi(2,sout);
							vrh1=(vrh-1+N)%N;
							sout.WriteLine("\tCMP\tR"+vrh1+", R"+vrh); 
							sout.WriteLine("\tMOVLE\tR"+vrh1+", #1"); 
							sout.WriteLine("\tMOVGT\tR"+vrh1+", #0"); 
							pop(sout);
							break;
						case 31: //GTR_R
							uskladi(2,sout);
							sout.WriteLine("; "+instr.mnem+": Nije implementirano!"); 
							vrh1=(vrh-1+N)%N;
							pop(sout);
							break;
						case 32: //GTR_I
							uskladi(2,sout);
							vrh1=(vrh-1+N)%N;
							sout.WriteLine("\tCMP\tR"+vrh1+", R"+vrh); 
							sout.WriteLine("\tMOVGT\tR"+vrh1+", #1"); 
							sout.WriteLine("\tMOVLE\tR"+vrh1+", #0"); 
							pop(sout);
							break;
						case 33: //GEQ_R
							uskladi(2,sout);
							sout.WriteLine("; "+instr.mnem+": Nije implementirano!"); 
							vrh1=(vrh-1+N)%N;
							pop(sout);
							break;
						case 34: //GEQ_I
							uskladi(2,sout);
							vrh1=(vrh-1+N)%N;
							sout.WriteLine("\tCMP\tR"+vrh1+", R"+vrh); 
							sout.WriteLine("\tMOVGE\tR"+vrh1+", #1"); 
							sout.WriteLine("\tMOVLT\tR"+vrh1+", #0"); 
							pop(sout);
							break;
						case 35: //NEQ_R
							uskladi(2,sout);
							sout.WriteLine("; "+instr.mnem+": Nije implementirano!"); 
							vrh1=(vrh-1+N)%N;
							sout.WriteLine("\tCMP\tR"+vrh1+", R"+vrh); 
							sout.WriteLine("\tMOVNE\tR"+vrh1+", #1"); 
							sout.WriteLine("\tMOVEQ\tR"+vrh1+", #0"); 
							pop(sout);
							break;
						case 36:  //NEQ_I
							uskladi(2,sout);
							vrh1=(vrh-1+N)%N;
							sout.WriteLine("\tCMP\tR"+vrh1+", R"+vrh); 
							sout.WriteLine("\tMOVNE\tR"+vrh1+", #1"); 
							sout.WriteLine("\tMOVEQ\tR"+vrh1+", #0"); 
							pop(sout);
							break;

						case 37: //NEG_R
							uskladi(1,sout);
							sout.WriteLine("\tEOR\tR"+vrh+", R"+vrh+", #0x80000000"); 
							break;
						case 38: //NEG_I
							uskladi(1,sout);
							sout.WriteLine("\tRSB\tR"+vrh+", R"+vrh+", #0"); 
							break;
						case 39: //ADD_R
							uskladi(2,sout);
							sout.WriteLine("; "+instr.mnem+": Nije implementirano!"); 
							vrh1=(vrh-1+N)%N;
							pop(sout);
							break;
						case 40: //ADD_I
							uskladi(2,sout);
							vrh1=(vrh-1+N)%N;
							sout.WriteLine("\tADD\tR"+vrh1+", R"+vrh1+", R"+vrh); 
							pop(sout);
							break;
						case 41: //SUB_R
							uskladi(2,sout);
							sout.WriteLine("; "+instr.mnem+": Nije implementirano!"); 
							vrh1=(vrh-1+N)%N;
							pop(sout);
							break;
						case 42: //SUB_I
							uskladi(2,sout);
							vrh1=(vrh-1+N)%N;
							sout.WriteLine("\tSUB\tR"+vrh1+", R"+vrh1+", R"+vrh); 
							pop(sout);
							break;
						case 43: //MUL_R
							uskladi(2,sout);
							sout.WriteLine("; "+instr.mnem+": Nije implementirano!"); 
							vrh1=(vrh-1+N)%N;
							pop(sout);
							break;
						case 44: //MUL_I
							uskladi(2,sout);
							vrh1=(vrh-1+N)%N;
							sout.WriteLine("\tMUL\tR"+vrh1+", R"+vrh1+", R"+vrh); 
							pop(sout);
							break;
						case 45: //DIV_R
							uskladi(2,sout);
							sout.WriteLine("; "+instr.mnem+": Nije implementirano!"); 
							vrh1=(vrh-1+N)%N;
							pop(sout);
							break;
						case 46: //DIV_I
							uskladi(2,sout);
							dijeli_=true;
							vrh1=(vrh-1+N)%N;
							sout.WriteLine("\tMOV\tR9, R"+vrh1); 
							sout.WriteLine("\tMOV\tR10, R"+vrh); 
							sout.WriteLine("\tBL\tdijeli"); 
							sout.WriteLine("\tMOV\tR"+vrh1+", R10"); 
							pop(sout);
							break;
						case 47: //MOD
							uskladi(2,sout);
							dijeli_=true;
							vrh1=(vrh-1+N)%N;
							sout.WriteLine("\tMOV\tR9, R"+vrh1); 
							sout.WriteLine("\tMOV\tR10, R"+vrh); 
							sout.WriteLine("\tBL\tdijeli"); 
							sout.WriteLine("\tMOV\tR"+vrh1+", R9"); 
							pop(sout);
							break;
						case 48: //FLT
							sout.WriteLine("; "+instr.mnem+": Nije implementirano!"); 
							break;
						case 49: //INT
							sout.WriteLine("\tADD\tR13, R13, #"+uint2int(instr.x)); 
							break;
						case 50: //DUP
							uskladi(1,sout);
							vrh1=vrh;
							push(sout);
							sout.WriteLine("\tMOV\tR"+vrh+", R"+vrh1); 
							break;
						case 51: //WRT_S
							uskladi(1,sout);
							sout.WriteLine("; "+instr.mnem+": Nije implementirano!"); 
							break;
						case 52: //WRT_C
							uskladi(1,sout);
							sout.WriteLine("; "+instr.mnem+": Nije implementirano!"); 
							break;
						case 53: //WRT_I
							uskladi(1,sout);
							sout.WriteLine("; "+instr.mnem+": Nije implementirano!"); 
							break;
						case 54: //WRT_R
							uskladi(1,sout);
							sout.WriteLine("; "+instr.mnem+": Nije implementirano!"); 
							break;
						case 55: //RED_S
							uskladi(1,sout);
							sout.WriteLine("; "+instr.mnem+": Nije implementirano!"); 
							break;
						case 56: //RED_C
							uskladi(1,sout);
							sout.WriteLine("; "+instr.mnem+": Nije implementirano!"); 
							break;
						case 57: //RED_I
							uskladi(1,sout);
							sout.WriteLine("; "+instr.mnem+": Nije implementirano!"); 
							break;
						case 58: //RED_R
							uskladi(1,sout);
							sout.WriteLine("; "+instr.mnem+": Nije implementirano!"); 
							break;
						case 59: //INC
							uskladi(1,sout);
							sout.WriteLine("\tADD\tR"+vrh+", R"+vrh+", #1"); 
							break;
						case 60: //DEC
							uskladi(1,sout);
							sout.WriteLine("\tSUB\tR"+vrh+", R"+vrh+", #1"); 
							break;
						default:
							break;
					}
					//					sout.WriteLine();
				}

				if (konstante.Count>0)
				{
					sout.WriteLine("; Konstante:");
					foreach(String s in konstante)
					{
						sout.WriteLine(s);
					}
				}

				if (dijeli_)
					dijeli(sout);

				sout.WriteLine("\tEND");
				_ARMKod=sb.ToString();
				sout.Close();

			}
			catch(System.ApplicationException exc)
			{
				_opisGreske=exc.Message;
				_dosloDoGreske=true;
				_ARMKod="";
			}
#if !DEBUG
			catch(Exception exc)
			{
				_opisGreske=exc.Message;
				_dosloDoGreske=true;
				_ARMKod="";
			}
#endif
		}
	
		private String get_labela(ArrayList labele,int broj)
		{
			foreach(PKodLabela l in labele)
			{
				if (l.broj==broj)
				{
					return l.labela;
				}
			}
			return "";
		}

		private void dijeli(StringWriter sout)
		{
			sout.Write("dijeli");
			sout.WriteLine("\tSTMEA\tR13!, {R0-R2}");

			sout.WriteLine("\tMOV\tR0, #0"); 
			sout.WriteLine("\tCMP\tR9, #0"); 
			sout.WriteLine("\tMOVLT\tR0, #1"); 
			sout.WriteLine("\tRSBLT\tR9, R9, #0"); 
			sout.WriteLine("\tCMP\tR10, #0"); 
			sout.WriteLine("\tEORLT\tR0,R0, #1"); 
			sout.WriteLine("\tRSBLT\tR10, R10, #0"); 

			sout.WriteLine("\tMOV\tR1, #0"); 
			sout.WriteLine("\tMOV\tR2, #0"); 
			sout.WriteLine("\tCMP\tR10, R1");
			sout.WriteLine("\tBHI\tk_div");
			sout.Write("p_div");
			sout.WriteLine("\tCMP\tR10, #0x80000000"); 
			sout.WriteLine("\tBHS\td_div");
			sout.WriteLine("\tCMP\tR9, R10, LSL #1");
			sout.WriteLine("\tMOVHS\tR10, R10, LSL #1");
			sout.WriteLine("\tADDHS\tR2, R2, #1");
			sout.WriteLine("\tBHI\tp_div");
			sout.Write("d_div");
			sout.WriteLine("\tCMP\tR9, R10");
			sout.WriteLine("\tSUBHS\tR9, R9, R10");
			sout.WriteLine("\tADDHS\tR1, R1, #1");
			sout.WriteLine("\tCMP\tR2, #0");
			sout.WriteLine("\tMOVHI\tR10, R10, LSR #1");
			sout.WriteLine("\tMOVHI\tR1, R1, LSL #1");
			sout.WriteLine("\tSUBHI\tR2, R2, #1");
			sout.WriteLine("\tBHI\td_div");
			sout.Write("k_div");
			sout.WriteLine("\tCMP\tR0, #1");
			sout.WriteLine("\tRSBEQ\tR10, R1, #0");
			sout.WriteLine("\tMOVNE\tR10, R1");
			sout.WriteLine("\tLDMEA\tR13!, {R0-R2}");
			sout.WriteLine("\tMOV\tR15, R14");
		}

		private void isprazni(StringWriter sout)
		{
			for(int x=0;x<N;x++)
			{
				push(sout);
			}
			nstog=0;
			vrh=N-1;
		}

		private void napuni(StringWriter sout)
		{
			int n;
			n=N;
			if (nstog<N) n=nstog;
			for(int x=0;x<n;x++)
			{
				sout.WriteLine("\tLDMEA\tR13!, {R"+x+"}");
			}
			vrh=(N-1+n)%N;
		}
		
		private void push(StringWriter sout)
		{
			vrh=(vrh+1)%N;
			if (nstog>N-1)
			{
				sout.WriteLine("\tSTMEA\tR13!, {R"+vrh+"}");
			} 
			nstog++;
		}
		private void pop(StringWriter sout)
		{
			if (nstog>N)
			{
				sout.WriteLine("\tLDMEA\tR13!, {R"+vrh+"}");
			} 
			nstog--;
			vrh=vrh1;
		}

		private void uskladi(int n,StringWriter sout)
		{
			if(n==1)
			{
				if (nstog==0)
				{
					sout.WriteLine("\tLDMEA\tR13!, {R0}");
					vrh=0;
					nstog=1;
				}
			}
			else
			{
				if (nstog==0)
				{
					sout.WriteLine("\tLDMEA\tR13!, {R1}");
					sout.WriteLine("\tLDMEA\tR13!, {R0}");
					vrh=1;
					nstog=2;
				} 
				else if (nstog==1)
				{
					sout.WriteLine("\tLDMEA\tR13!, {R"+(N-1)+"}");
					vrh=vrh;
					nstog=2;
				}
			}
		}

		private void baza(int n,StringWriter sout)
		{
			if (n<=4)
			{
				sout.WriteLine("\tMOV\tR10, R8");
				for(int i=0;i<n;i++)
					sout.WriteLine("\tLDR\tR10, [R10,#4]");
			} 
			else 
			{
				sout.WriteLine("\tMOV\tR9, #"+n);
				sout.WriteLine("\tMOV\tR10, R8");
				sout.WriteLine("L"+int_label+"\tLDR\tR10, [R10,#4]");
				sout.WriteLine("\tSUB\tR9, R9, #1");
				sout.WriteLine("\tBNE\tL"+int_label);
				int_label++;
			}
		}

		private int uint2int(uint x)
		{
			byte[] tmp=BitConverter.GetBytes(x);
			return BitConverter.ToInt32(tmp,0);
		}


		public string VratiARMKod
		{
			get { return _ARMKod; }
		}


		public string OpisGreske
		{
			get { return _opisGreske; }
		}

		public bool DosloDoGreske
		{
			get { return _dosloDoGreske; }
		}

		private string _ARMKod;
		private string _opisGreske;
		private bool _dosloDoGreske;
		private int int_label=0;
		private int N,nstog,vrh,vrh1,nstog_;
	}
}
