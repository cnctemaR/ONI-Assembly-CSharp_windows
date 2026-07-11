using System;
using System.Collections;

namespace System.Xml.Schema
{
	internal class Positions
	{
		public int Add(int symbol, object particle)
		{
			return this.positions.Add(new Position(symbol, particle));
		}

		public Position this[int pos]
		{
			get
			{
				return (Position)this.positions[pos];
			}
		}

		public int Count
		{
			get
			{
				return this.positions.Count;
			}
		}

		private ArrayList positions = new ArrayList();
	}
}
