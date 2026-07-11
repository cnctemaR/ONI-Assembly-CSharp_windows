using System;

namespace System.Xml.Schema
{
	internal struct Position
	{
		public Position(int symbol, object particle)
		{
			this.symbol = symbol;
			this.particle = particle;
		}

		public int symbol;

		public object particle;
	}
}
