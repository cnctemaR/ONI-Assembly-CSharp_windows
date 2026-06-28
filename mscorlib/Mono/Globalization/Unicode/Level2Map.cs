using System;

namespace Mono.Globalization.Unicode
{
	internal class Level2Map
	{
		public Level2Map(byte source, byte replace)
		{
			this.Source = source;
			this.Replace = replace;
		}

		public byte Source;

		public byte Replace;
	}
}
