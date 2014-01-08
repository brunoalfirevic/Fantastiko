using System;
using System.Collections;

namespace JezicniProcesor
{
	enum AtributLeksickeJedinkeEnum
	{
		Konstanta,
		LeksickaJedinka
	}

	enum VrijednostAtributaLeksickeJedinkeEnum
	{
		String,
		Integer,
		Real
	}

	class LeksickaJedinka
	{
		public LeksickaJedinka(string leksickaJedinkaID, UniformniZnakEnum klasaLeksickeJedinke)
		{
			_leksickaJedinkaID = leksickaJedinkaID;
			_klasaJedinke = klasaLeksickeJedinke;
		}

		public string JedinkaID
		{
			get { return _leksickaJedinkaID; }
		}

		public UniformniZnakEnum KlasaJedinke
		{
			get { return _klasaJedinke; }
		}

		public VrijednostAtributaLeksickeJedinkeEnum GetLeksickaJedinkaAttribute(AtributLeksickeJedinkeEnum jedinkaAttribute)
		{
			return (VrijednostAtributaLeksickeJedinkeEnum)_attributes[jedinkaAttribute];
		}

		public void SetLeksickaJedinkaAttribute(AtributLeksickeJedinkeEnum jedinkaAttribute,
			VrijednostAtributaLeksickeJedinkeEnum attributeValue)
		{
			_attributes[jedinkaAttribute] = attributeValue;
		}

		private string _leksickaJedinkaID;
		private UniformniZnakEnum _klasaJedinke;
		private Hashtable _attributes = new Hashtable();
	}
}
