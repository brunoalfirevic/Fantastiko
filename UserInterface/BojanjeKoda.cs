using System;
using System.IO;
using System.Collections;

namespace UserInterface
{
	/// <summary>
	/// Summary description for Bojac.
	/// </summary>
	public class Bojac
	{
		private string TheFile;
		private  ArrayList Keywords = new ArrayList();
		private ArrayList  Identifikatori = new ArrayList();
		private ArrayList  Comments = new ArrayList();
		
		public Bojac(string file)
		{
			//
			// TODO: Add constructor logic here
			//
			FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
			StreamReader sr = new StreamReader(fs);
			TheFile = sr.ReadToEnd();
			sr.Close();
			fs.Close();
			FillArrays();
		}

		
		public Bojac()
		{
		Keywords.Add("and");		Keywords.Add("array");
		Keywords.Add("begin");		Keywords.Add("case");
		Keywords.Add("const");		Keywords.Add("div");
		Keywords.Add("do");			Keywords.Add("downto");
		Keywords.Add("else");		Keywords.Add("to");
		Keywords.Add("for");		Keywords.Add("forward");
		
		Keywords.Add("function");	Keywords.Add("goto");
		Keywords.Add("if");			
		Keywords.Add("label");		
		Keywords.Add("mod");		Keywords.Add("nil");
		Keywords.Add("not");		
		Keywords.Add("or");			

		Keywords.Add("procedure");	Keywords.Add("array");
		Keywords.Add("program");	Keywords.Add("record");
		Keywords.Add("repeat");		
		Keywords.Add("type");		Keywords.Add("until");
		Keywords.Add("var");		Keywords.Add("while");
		Keywords.Add("then");		Keywords.Add("end");

		Keywords.Sort();
		//Comments.Add("{");

		}

		public void FillArrays()
		{
			StringReader sr = new StringReader(TheFile);
			string nextLine;

			nextLine = sr.ReadLine();
			nextLine = nextLine.Trim();

			// find functions header
			while (nextLine != null)
			{
				if (nextLine == "[IDENTIFIKATORI]")
				{
					// read all of the functions into the arraylist
					nextLine = sr.ReadLine();
					if (nextLine != null)
						nextLine = nextLine.Trim();
					while (nextLine != null && nextLine[0] != '[' 
						)
					{
						Identifikatori.Add(nextLine);

						nextLine = "";
						while (nextLine != null && nextLine == "")
						{
							nextLine = sr.ReadLine();
							if (nextLine != null)
								nextLine = nextLine.Trim();
						}
					}
				}

				if (nextLine == "[KLJUCNE]")
				{
					// read all of the functions into the arraylist
					nextLine = sr.ReadLine();
					if (nextLine != null)
						nextLine = nextLine.Trim();
					while (nextLine != null && nextLine[0] != '[' 
						)
					{
						Keywords.Add(nextLine);

						nextLine = "";
						while (nextLine != null && nextLine == "")
						{
							nextLine = sr.ReadLine();
							if (nextLine != null)
							nextLine = nextLine.Trim();
						}
						
					}
				}

				if (nextLine == "[KOMENTAR]")
				{
					// read all of the functions into the arraylist
					nextLine = sr.ReadLine();
					if (nextLine != null)
						nextLine = nextLine.Trim();
					while (nextLine != null && nextLine[0] != '[' 
						)
					{
						Comments.Add(nextLine);

						nextLine = "";
						while (nextLine != null && nextLine == "")
						{
							nextLine = sr.ReadLine();
							if (nextLine != null)
								nextLine = nextLine.Trim();
						}
						
					}
				}

				if (nextLine != null && nextLine.Length > 0 && nextLine[0] == '[')
				{
				}
				else
				{
					nextLine = sr.ReadLine();
					if (nextLine != null)
						nextLine = nextLine.Trim();
				}
			}

			Keywords.Sort();
			Identifikatori.Sort();
			Comments.Sort();
											
		}


		public bool IsKeyword(string s)
		{
			int index = Keywords.BinarySearch(s);
			if (index >= 0)
				return true;

			return false;
		}

		public bool IsIdentifikator(string s)
		{
			int index = Identifikatori.BinarySearch(s);
			if (index >= 0)
				return true;

			return false;
		}

		public bool IsComment(string s)
		{
			int index = Comments.BinarySearch(s);
			if (index >= 0)
				return true;

			return false;
		}


	}
}
