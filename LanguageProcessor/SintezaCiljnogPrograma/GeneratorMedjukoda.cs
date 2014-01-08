using System;
using System.Text;

namespace JezicniProcesor
{

	enum JednoadresneNaredbeEnum
	{
		Hlt,		
		Lti,
		Sti,		
		Lti_b,
		Sti_b,		
		Rtp,
		Rtf,
		Not,
		Or,
		And,
		Xor,
		Odd,
		Eql_r,
		Eql_i,
		Lss_r,
		Lss_i,
		Leq_r,
		Leq_i,
		Gtr_r,
		Gtr_i,
		Geq_r,
		Geq_i,
		Neq_r,
		Neq_i,
		Neg_r,
		Neg_i,
		Add_r,
		Add_i,
		Sub_r,
		Sub_i,
		Mul_r,
		Mul_i,
		Div_r,
		Div_i,
		Mod,
		Flt,
		Dup,
		Wrt_s,
		Wrt_c,
		Wrt_i,
		Wrt_r,
		Red_c,
		Red_r,
		Red_i,
		Inc,
		Dec
	}

	enum DvoadresneNaredbeEnum
	{
		Lit,				
		Jmp,
		Jpc,
		Jpn,
		Mst,
		Ent,
		Int,
		Red_s
	}

	enum TroadresneNaredbeEnum
	{
		Cal,
		Lda,		
		Lod,
		Sto,
		Lid,
		Sid,
		Lod_b,
		Sto_b,
		Lid_b,
		Sid_b,
	}


	class GeneratorMedjukoda
	{
		public GeneratorMedjukoda()
		{
			_generiraniProgram = new StringBuilder();
		}

		public void Generiraj(JednoadresneNaredbeEnum naredba)
		{
			_generiraniProgram.Append("\t" + naredba.ToString() + "\r\n");
		}
		
		public void Generiraj(DvoadresneNaredbeEnum naredba, UInt32 parametar)
		{
			_generiraniProgram.Append("\t" + naredba.ToString() + "\t0x" + Convert.ToString(parametar, 16) + "\r\n");
		}

		public void Generiraj(DvoadresneNaredbeEnum naredba, string parametar)
		{
			_generiraniProgram.Append("\t" + naredba.ToString() + "\t" + parametar + "\r\n");
		}

		public void Generiraj(TroadresneNaredbeEnum naredba, UInt32 parametar1, UInt32 parametar2)
		{
			_generiraniProgram.Append("\t" + naredba.ToString() + "\t0x" + Convert.ToString(parametar1, 16) +
				" 0x" + Convert.ToString(parametar2, 16) + "\r\n");
		}

		public void Generiraj(TroadresneNaredbeEnum naredba, UInt32 parametar1, string parametar2)
		{
			_generiraniProgram.Append("\t" + naredba.ToString() + "\t0x" + Convert.ToString(parametar1, 16) +
				" " + parametar2 + "\r\n");
		}

		public void Generiraj(TroadresneNaredbeEnum naredba, string parametar1, UInt32 parametar2)
		{
			_generiraniProgram.Append("\t" + naredba.ToString() + "\t" + parametar1 +
				" 0x" + Convert.ToString(parametar2, 16) + "\r\n");
		}

		public void Generiraj(TroadresneNaredbeEnum naredba, string parametar1, string parametar2)
		{
			_generiraniProgram.Append("\t" + naredba.ToString() + "\t" + parametar1 +
				" " + parametar2 + "\r\n");
		}

		public void PostaviAdresnuLabelu(string labela)
		{
			_generiraniProgram.Append(labela + ":\r\n");
		}

		public void PostaviPodatkovnuLabelu(string labela, UInt32 podatak)
		{
			_generiraniProgram.Append(labela + " 0x" + Convert.ToString(podatak,16) + "\r\n");
		}

		public string GeneriraniProgram
		{
			get { return _generiraniProgram.ToString(); }
		}

		private StringBuilder _generiraniProgram;
	}
}
