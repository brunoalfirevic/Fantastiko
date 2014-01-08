using System;
using System.IO;
using System.Collections;

namespace p_stroj
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
	class Labela
	{
		public int broj;
		public String labela;
		public Labela(String labela,int broj)
		{
			this.broj=broj;
			this.labela=labela;
		}

	}

	class Class1
	{
		static byte[] stog;
		static int baza(int br,int n)
		{
			int x;
			while (n>0)
			{
				x=get_int(br+4);
				br=x;
				n--;
			}
			return br;
		}

		static int uint2int(uint x)
		{
			byte[] tmp=BitConverter.GetBytes(x);
			return BitConverter.ToInt32(tmp,0);
		}

		static int get_int(int addr)
		{
			return BitConverter.ToInt32(stog,addr);
		}

		static void put_int(int br,int addr)
		{
			byte[] tmp=BitConverter.GetBytes(br);
			stog[addr+0]=tmp[0];
			stog[addr+1]=tmp[1];
			stog[addr+2]=tmp[2];
			stog[addr+3]=tmp[3];
		}

		static uint get_uint(int addr)
		{
			return BitConverter.ToUInt32(stog,addr);
		}

		static void put_uint(uint br,int addr)
		{
			byte[] tmp=BitConverter.GetBytes(br);
			stog[addr+0]=tmp[0];
			stog[addr+1]=tmp[1];
			stog[addr+2]=tmp[2];
			stog[addr+3]=tmp[3];
		}

		static float get_float(int addr)
		{
			return BitConverter.ToSingle(stog,addr);
		}

		static void put_float(float br,int addr)
		{
			byte[] tmp=BitConverter.GetBytes(br);
			stog[addr+0]=tmp[0];
			stog[addr+1]=tmp[1];
			stog[addr+2]=tmp[2];
			stog[addr+3]=tmp[3];
		}

		static uint get_byte(int addr)
		{
			return (uint)stog[addr];
		}

		static void put_byte(uint br,int addr)
		{
			byte[] tmp=BitConverter.GetBytes(br);
			stog[addr]=tmp[0];
		}

		[STAThread]
		static void Main(string[] args)
		{
			Console.Write("Datoteka sa p-kodom: ");
			String ulaz=Console.ReadLine();
			FileStream fin=new FileStream(ulaz,FileMode.Open);
			StreamReader sin=new StreamReader(fin);

			String Line;
			String[] element;
			int n,x,y;
			uint X,Y;
			float xf,yf;
		
			int PC, BR, SP;
			int NSTOG=10000;
			int brojac=0,start=0;

			stog=new byte[NSTOG];
			SP=0;
			BR=0;

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
						labele1.Add(new Labela(Line.Remove(Line.Length-1,1),brojac));
					} 
					else 
					{
						element=Line.Split(' ','\t');
						p_labele.Add(new Labela(element[0],Convert.ToInt32(element[1],16)));
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
								p_labele2.Add(new Labela(element[2],brojac-1));
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
							labele2.Add(new Labela(element[2],brojac-1));
							break;
						case "JPC":
							kodovi.Add(new Instrukcija("JPC",16,0,0));
							labele2.Add(new Labela(element[2],brojac-1));
							break;
						case "JPN":
							kodovi.Add(new Instrukcija("JPN",116,0,0));
							labele2.Add(new Labela(element[2],brojac-1));
							break;
						case "MST":
							n=Convert.ToInt32(element[2],16);
							kodovi.Add(new Instrukcija("MST",117,n,0));
							break;
						case "CAL":
							n=Convert.ToInt32(element[2],16);
							kodovi.Add(new Instrukcija("CAL",217,n,0));
							labele2.Add(new Labela(element[3],brojac-1));
							break;
						case "ENT":
							if (element[2].StartsWith("0x"))
							{
								X=Convert.ToUInt32(element[2],16);
								kodovi.Add(new Instrukcija("ENT",317,0,X));
							} 
							else 
							{
								p_labele2.Add(new Labela(element[2],brojac-1));
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
							kodovi.Add(new Instrukcija("ADD_I",39,0,0));
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
								p_labele2.Add(new Labela(element[2],brojac-1));
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

			Console.WriteLine("Adresene labele:");
			foreach(Labela l in labele1)
			{
				Console.WriteLine(" "+l.labela);
			}
			Console.WriteLine("\nPodatkovne labele:");
			foreach(Labela l in p_labele)
			{
				Console.WriteLine(" "+l.labela+"\t"+l.broj);
			}

			//povezivanje adresnih labela i labela u naredbama		
			foreach(Labela l in labele2)
			{
				int poz=-1;
				for(int i=0;i<labele1.Count;i++)
				{
					if (((Labela)labele1[i]).labela==l.labela)
						poz=i;
				}
				((Instrukcija)kodovi[l.broj]).x=(uint)((Labela)labele1[poz]).broj;
			}

			//povezivanje podatkovnih labela i labela u naredbama		
			foreach(Labela l in p_labele2)
			{
				int poz=-1;
				for(int i=0;i<p_labele.Count;i++)
				{
					if (((Labela)p_labele[i]).labela==l.labela)
						poz=i;
				}
				((Instrukcija)kodovi[l.broj]).x=(uint)((Labela)p_labele[poz]).broj;
			}


			Console.WriteLine("\nInterni kod programa:");
			for(int i=0;i<kodovi.Count;i++)
			{
				Instrukcija instr=(Instrukcija)kodovi[i];
				Console.WriteLine(i+"\t("+instr.kod+")\t"+instr.mnem+"\t"+instr.n+","+instr.x);
			}
			for(int i=0;i<labele1.Count;i++)
			{
				if (((Labela)labele1[i]).labela=="glavni")
					start=i;
			}

			Console.WriteLine("\nRezultat rada programa:");

			PC=start;
			bool kraj=false;
			int br;
			while (!kraj)
			{
				Instrukcija instr=(Instrukcija)kodovi[PC];
				//Console.WriteLine(instr.mnem);
				switch (instr.kod)
				{
					case 0: //HLT
						kraj=true;
						PC++;
						break;
					case 1: //LIT x
						put_uint(instr.x,SP);
						SP+=4;
						PC++;
						break;
					case 2: //LDA n,x
						x=baza(BR,instr.n)+uint2int(instr.x);
						put_int(x,SP);
						SP+=4;
						PC++;
						break;
					case 3: //LOD n,x
						br=baza(BR,instr.n)+uint2int(instr.x);
						X=get_uint(br);
						put_uint(X,SP);
						SP+=4;
						PC++;
						break;
					case 4: //STO n,x
						br=baza(BR,instr.n)+uint2int(instr.x);
						X=get_uint(SP-4);
						SP-=4;
						put_uint(X,br);
						PC++;
						break;
					case 5: //LID n,x
						br=baza(BR,instr.n)+uint2int(instr.x);
						X=get_uint(br);
						Y=get_uint((int)X);
						put_uint(Y,SP);
						SP+=4;
						PC++;
						break;
					case 6: //SID n,x
						br=baza(BR,instr.n)+uint2int(instr.x);
						X=get_uint(br);
						Y=get_uint(SP-4);
						SP-=4;
						put_uint(X,(int)Y);
						PC++;
						break;
					case 7: //LTI
						X=get_uint(SP-4);
						Y=get_uint((int)X);
						put_uint(Y,SP-4);
						PC++;
						break;
					case 8: //STI
						X=get_uint(SP-4);
						SP-=4;
						Y=get_uint(SP-4);
						SP-=4;
						put_uint(X,(int)Y);
						PC++;
						break;
					case 9: //LOD_B n,x
						br=baza(BR,instr.n)+uint2int(instr.x);
						X=get_byte(br);
						put_byte(X,SP);
						SP+=4;
						PC++;
						break;
					case 10: //STO_B n,x
						br=baza(BR,instr.n)+uint2int(instr.x);
						X=get_byte(SP-4);
						SP-=4;
						put_byte(X,br);
						PC++;
						break;
					case 11: //LID_B n,x
						br=baza(BR,instr.n)+uint2int(instr.x);
						X=get_uint(br);
						Y=get_byte((int)X);
						put_byte(Y,SP);
						SP+=4;
						PC++;
						break;
					case 12: //SID_B n,x
						br=baza(BR,instr.n)+uint2int(instr.x);
						X=get_uint(br);
						Y=get_byte(SP-4);
						SP-=4;
						put_byte(X,(int)Y);
						PC++;
						break;
					case 13: //LTI_B
						X=get_uint(SP-4);
						Y=get_byte((int)X);
						put_byte(Y,SP-4);
						PC++;
						break;
					case 14: //STI_B
						X=get_uint(SP-4);
						SP-=4;
						Y=get_byte(SP-4);
						SP-=4;
						put_byte(X,(int)Y);
						PC++;
						break;
					case 15: //JMP x
						PC=(int)instr.x;
						break;
					case 16: //JPC x
						x=get_int(SP-4);
						SP-=4;
						if (x!=0) PC=(int)instr.x; else PC++;
						break;
					case 116: //JPN x
						x=get_int(SP-4);
						SP-=4;
						if (x==0) PC=(int)instr.x; else PC++;
						break;
					case 117: //MST n
						SP+=4;
						br=baza(BR,instr.n);
						put_int(br,SP);
						SP+=4;
						put_int(BR,SP);
						SP+=8;
						PC++;
						break;
					case 217: //CAL n,x
						PC++;
						put_int(PC,SP-instr.n-4);
						BR=SP-instr.n-16;
						PC=(int)instr.x;
						break;
					case 317: //ENT x
						SP=BR+(int)instr.x;
						PC++;
						break;
					case 18: //RTP
						SP=BR;
						BR=get_int(SP+8);
						PC=get_int(SP+12);
						break;
					case 19: //RTF
						SP=BR+4;
						BR=get_int(SP+4);
						PC=get_int(SP+8);
						break;

					case 20: //NOT
						X=get_uint(SP-4);
						X=~X;
						put_uint(X,SP-4);
						PC++;
						break;
					case 21: //OR
						X=get_uint(SP-4);
						SP-=4;
						Y=get_uint(SP-4);
						X=X|Y;
						put_uint(X,SP-4);
						PC++;
						break;
					case 22: //AND
						X=get_uint(SP-4);
						SP-=4;
						Y=get_uint(SP-4);
						X=X&Y;
						put_uint(X,SP-4);
						PC++;
						break;
					case 23: //XOR
						X=get_uint(SP-4);
						SP-=4;
						Y=get_uint(SP-4);
						X=X^Y;
						put_uint(X,SP-4);
						PC++;
						break;
					case 24: //ODD
						X=get_uint(SP-4);
						X%=2;
						put_uint(X,SP-4);
						PC++;
						break;

					case 25: //EQL_R
						xf=get_float(SP-4);
						SP-=4;
						yf=get_float(SP-4);
						if (yf==xf) x=1; else x=0;
						put_int(x,SP-4);
						PC++;
						break;
					case 26: //EQL_I
						x=get_int(SP-4);
						SP-=4;
						y=get_int(SP-4);
						if (y==x) x=1; else x=0;
						put_int(x,SP-4);
						PC++;
						break;
					case 27: //LSS_R
						xf=get_float(SP-4);
						SP-=4;
						yf=get_float(SP-4);
						if (yf<xf) x=1; else x=0;
						put_int(x,SP-4);
						PC++;
						break;
					case 28: //LSS_I
						x=get_int(SP-4);
						SP-=4;
						y=get_int(SP-4);
						if (y<x) x=1; else x=0;
						put_int(x,SP-4);
						PC++;
						break;
					case 29: //LEQ_R
						xf=get_float(SP-4);
						SP-=4;
						yf=get_float(SP-4);
						if (yf<=xf) x=1; else x=0;
						put_int(x,SP-4);
						PC++;
						break;
					case 30: //LEQ_I
						x=get_int(SP-4);
						SP-=4;
						y=get_int(SP-4);
						if (y<=x) x=1; else x=0;
						put_int(x,SP-4);
						PC++;
						break;
					case 31: //GTR_R
						xf=get_float(SP-4);
						SP-=4;
						yf=get_float(SP-4);
						if (yf>xf) x=1; else x=0;
						put_int(x,SP-4);
						PC++;
						break;
					case 32: //GTR_I
						x=get_int(SP-4);
						SP-=4;
						y=get_int(SP-4);
						if (y>x) x=1; else x=0;
						put_int(x,SP-4);
						PC++;
						break;
					case 33: //GEQ_R
						xf=get_float(SP-4);
						SP-=4;
						yf=get_float(SP-4);
						if (yf>=xf) x=1; else x=0;
						put_int(x,SP-4);
						PC++;
						break;
					case 34: //GEQ_I
						x=get_int(SP-4);
						SP-=4;
						y=get_int(SP-4);
						if (y>=x) x=1; else x=0;
						put_int(x,SP-4);
						PC++;
						break;
					case 35: //NEQ_R
						xf=get_float(SP-4);
						SP-=4;
						yf=get_float(SP-4);
						if (yf!=xf) x=1; else x=0;
						put_int(x,SP-4);
						PC++;
						break;
					case 36:  //NEQ_I
						x=get_int(SP-4);
						SP-=4;
						y=get_int(SP-4);
						if (y!=x) x=1; else x=0;
						put_int(x,SP-4);
						PC++;
						break;

					case 37: //NEG_R
						xf=get_float(SP-4);
						xf=-xf;
						put_float(xf,SP-4);
						PC++;
						break;
					case 38: //NEG_I
						x=get_int(SP-4);
						x=-x;
						put_int(x,SP-4);
						PC++;
						break;
					case 39: //ADD_R
						xf=get_float(SP-4);
						SP-=4;
						yf=get_float(SP-4);
						xf=yf+xf;
						put_float(xf,SP-4);
						PC++;
						break;
					case 40: //ADD_I
						x=get_int(SP-4);
						SP-=4;
						y=get_int(SP-4);
						x=y+x;
						put_int(x,SP-4);
						PC++;
						break;
					case 41: //SUB_R
						xf=get_float(SP-4);
						SP-=4;
						yf=get_float(SP-4);
						xf=yf-xf;
						put_float(xf,SP-4);
						PC++;
						break;
					case 42: //SUB_I
						x=get_int(SP-4);
						SP-=4;
						y=get_int(SP-4);
						x=y-x;
						put_int(x,SP-4);
						PC++;
						break;
					case 43: //MUL_R
						xf=get_float(SP-4);
						SP-=4;
						yf=get_float(SP-4);
						xf=yf*xf;
						put_float(xf,SP-4);
						PC++;
						break;
					case 44: //MUL_I
						x=get_int(SP-4);
						SP-=4;
						y=get_int(SP-4);
						x=y*x;
						put_int(x,SP-4);
						PC++;
						break;
					case 45: //DIV_R
						xf=get_float(SP-4);
						SP-=4;
						yf=get_float(SP-4);
						xf=yf/xf;
						put_float(xf,SP-4);
						PC++;
						break;
					case 46: //DIV_I
						x=get_int(SP-4);
						SP-=4;
						y=get_int(SP-4);
						x=y/x;
						put_int(x,SP-4);
						PC++;
						break;
					case 47: //MOD
						x=get_int(SP-4);
						SP-=4;
						y=get_int(SP-4);
						x=y%x;
						put_int(x,SP-4);
						PC++;
						break;
					case 48: //FLT
						x=get_int(SP-4);
						xf=Convert.ToSingle(x);
						put_float(xf,SP-4);
						PC++;
						break;
					case 49: //INT
						SP+=uint2int(instr.x);
						PC++;
						break;
					case 50: //DUP
						X=get_uint(SP-4);
						SP+=4;
						put_uint(X,SP-4);
						PC++;
						break;
					case 51: //WRT_S
						x=get_int(SP-4);
						while(stog[x]!=0)
						{
							Console.Write((char)stog[x]);
							x++;
						}
						PC++;
						break;
					case 52: //WRT_C
						x=stog[SP-4];
						Console.Write((char)x);
						PC++;
						break;
					case 53: //WRT_I
						x=get_int(SP-4);
						Console.Write(x);
						PC++;
						break;
					case 54: //WRT_R
						xf=get_float(SP-4);
						Console.Write(xf);
						PC++;
						break;
					case 55: //RED_S
						x=get_int(SP-4);
						String tmp=Console.ReadLine();
						for(y=0;y<instr.x;y++)
							stog[x+y]=(byte)tmp[y];
						PC++;
						break;
					case 56: //RED_C
						x=(int)Console.ReadLine()[0];
						put_int(x,SP);
						SP+=4;
						PC++;
						break;
					case 57: //RED_I
						x=Convert.ToInt32(Console.ReadLine());
						put_int(x,SP);
						SP+=4;
						PC++;
						break;
					case 58: //RED_R
						xf=Convert.ToSingle(Console.ReadLine());
						put_float(xf,SP);
						SP+=4;
						PC++;
						break;
					case 59: //INC
						x=get_int(SP-4);
						x++;
						put_int(x,SP-4);
						PC++;
						break;
					case 60: //DEC
						x=get_int(SP-4);
						x--;
						put_int(x,SP-4);
						PC++;
						break;
					default:
						break;
				}
				/*Console.WriteLine("Stog "+SP+":");
				for (x=SP-1;x>=0;x--)
				{
					Console.WriteLine(" "+stog[x]);
				}
				Console.ReadLine();*/
			}
			Console.WriteLine();
		}
	}
}
