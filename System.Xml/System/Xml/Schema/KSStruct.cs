using System;

namespace System.Xml.Schema
{
	internal class KSStruct
	{
		public KSStruct(KeySequence ks, int dim)
		{
			this.ks = ks;
			this.fields = new LocatedActiveAxis[dim];
		}

		public int depth;

		public KeySequence ks;

		public LocatedActiveAxis[] fields;
	}
}
