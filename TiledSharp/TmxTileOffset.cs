using System;
using System.Xml.Linq;

namespace TiledSharp
{
	public class TmxTileOffset
	{
		public int X { get; private set; }

		public int Y { get; private set; }

		public TmxTileOffset(XElement xTileOffset)
		{
			if (xTileOffset == null)
			{
				this.X = 0;
				this.Y = 0;
			}
			else
			{
				this.X = (int)xTileOffset.Attribute("x");
				this.Y = (int)xTileOffset.Attribute("y");
			}
		}
	}
}
