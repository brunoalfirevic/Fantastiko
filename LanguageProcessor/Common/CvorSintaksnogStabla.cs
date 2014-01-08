using System;
using System.Collections;

namespace JezicniProcesor
{
	class CvorSintaksnogStabla
	{
		public CvorSintaksnogStabla(ZnakGramatike znakGramatike, CvorSintaksnogStabla roditelj)
		{
			_znakGramatike = znakGramatike;
			_roditelj = roditelj;
		}

		public ZnakGramatike ZnakGramatike
		{
			get { return _znakGramatike; }
		}

		public CvorSintaksnogStabla Roditelj
		{
			get { return _roditelj; }
		}

		public IList Djeca
		{
			get { return _djeca; }
		}

		private ZnakGramatike _znakGramatike;		
		private ArrayList _djeca = new ArrayList();
		private CvorSintaksnogStabla _roditelj;
	}
}