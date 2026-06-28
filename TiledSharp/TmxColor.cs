using System;
using System.Globalization;
using System.Xml.Linq;

namespace TiledSharp
{
	public class TmxColor
	{
		public TmxColor(XAttribute xColor)
		{
			if (xColor != null)
			{
				string text = ((string)xColor).TrimStart("#".ToCharArray());
				this.R = int.Parse(text.Substring(0, 2), NumberStyles.HexNumber);
				this.G = int.Parse(text.Substring(2, 2), NumberStyles.HexNumber);
				this.B = int.Parse(text.Substring(4, 2), NumberStyles.HexNumber);
			}
		}

		public int R;

		public int G;

		public int B;
	}
}
