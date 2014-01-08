using System;

namespace JezicniProcesor
{
	/// <summary>
	/// Summary description for KonverterPodataka.
	/// </summary>
	class KonverterPodataka
	{
		public static UInt32 FloatToBitRepresentation(float f)
		{
			return BitConverter.ToUInt32(BitConverter.GetBytes(f),0);
		}

		public static float BitRepresentationToFloat(UInt32 bitRepresentation)
		{
			return BitConverter.ToSingle(BitConverter.GetBytes(bitRepresentation),0);
		}

		public static bool BitRepresentationToBool(UInt32 bitRepresentation)
		{
			return bitRepresentation!=0;
		}

		public static UInt32 BoolToBitRepresentation(bool b)
		{
			if (b)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}

		public static UInt32 SignedIntToBitRepresentation(int n)
		{
			return BitConverter.ToUInt32(BitConverter.GetBytes(n),0);
		}

		public static int BitRepresentationToSignedInt(UInt32 bitRepresentation)
		{
			return BitConverter.ToInt32(BitConverter.GetBytes(bitRepresentation),0);
		}
	}
}
