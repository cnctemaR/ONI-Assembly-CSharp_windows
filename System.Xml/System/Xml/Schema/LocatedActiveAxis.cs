using System;

namespace System.Xml.Schema
{
	internal class LocatedActiveAxis : ActiveAxis
	{
		internal int Column
		{
			get
			{
				return this.column;
			}
		}

		internal LocatedActiveAxis(Asttree astfield, KeySequence ks, int column)
			: base(astfield)
		{
			this.Ks = ks;
			this.column = column;
			this.isMatched = false;
		}

		internal void Reactivate(KeySequence ks)
		{
			base.Reactivate();
			this.Ks = ks;
		}

		private int column;

		internal bool isMatched;

		internal KeySequence Ks;
	}
}
