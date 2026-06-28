using System;
using System.Xml.Linq;

namespace TiledSharp
{
	public class TmxAnimationFrame
	{
		public int Id { get; private set; }

		public int Duration { get; private set; }

		public TmxAnimationFrame(XElement xFrame)
		{
			this.Id = (int)xFrame.Attribute("tileid");
			this.Duration = (int)xFrame.Attribute("duration");
		}
	}
}
