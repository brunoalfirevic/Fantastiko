using System;

namespace JezicniProcesor
{
	/// <summary>
	/// Summary description for GeneratorLabela.
	/// </summary>
	class GeneratorLabela
	{
		public static string PocetakPotprograma(OpisnikPotprograma opisnik)
		{
			return "Subroutine_" + opisnik.Identifikator + "_" + opisnik.JedinstveniIdentifikator.ToString();
		}

		public static string VelicinaRezerviranogStoga(OpisnikPotprograma opisnik)
		{
			return "Reserved_stack_size_" + opisnik.JedinstveniIdentifikator.ToString();
		}

		public static string GotoLabela(Labela labela)
		{
			return "Label_" + labela.JedinstveniIdentifikator.ToString();
		}

		public static string KrajIfNaredbe(int oznakaNaredbe)
		{
			return "If_end_" + oznakaNaredbe.ToString();
		}

		public static string KrajElseNaredbe(int oznakaNaredbe)
		{
			return "Else_end_" + oznakaNaredbe.ToString();
		}

		public static string PocetakWhileNaredbe(int oznakaNaredbe)
		{
			return "While_start_" + oznakaNaredbe.ToString();
		}
		
		public static string KrajWhileNaredbe(int oznakaNaredbe)
		{
			return "While_end_" + oznakaNaredbe.ToString();
		}

		public static string PocetakRepeatNaredbe(int oznakaNaredbe)
		{
			return "Repeat_start_" + oznakaNaredbe.ToString();
		}

		public static string PocetakForNaredbe(int oznakaNaredbe)
		{
			return "For_start_" + oznakaNaredbe.ToString();
		}

		public static string KrajForNaredbe(int oznakaNaredbe)
		{
			return "For_end_" + oznakaNaredbe.ToString();
		}

		public static string CasePodNaredba(int oznakaNaredbe)
		{
			return "Case_statement_" + oznakaNaredbe.ToString();
		}

		public static string CaseSelekcija(int oznakaNaredbe)
		{
			return "Case_selection_" + oznakaNaredbe.ToString();
		}

		public static string KrajCaseNaredbe(int oznakaNaredbe)
		{
			return "Case_end_" + oznakaNaredbe.ToString();
		}

		public static string GlavniProgram()
		{
			return "Main_program";
		}
	}
}
