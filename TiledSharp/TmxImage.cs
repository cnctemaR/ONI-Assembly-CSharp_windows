using System;
using System.IO;
using System.Xml.Linq;

namespace TiledSharp
{
	public class TmxImage
	{
		public string Source { get; private set; }

		public string Format { get; private set; }

		public Stream Data { get; private set; }

		public TmxColor Trans { get; private set; }

		public int? Width { get; private set; }

		public int? Height { get; private set; }

		public TmxImage(XElement xImage, string tmxDir = "")
		{
			if (xImage != null)
			{
				XAttribute xattribute = xImage.Attribute("source");
				if (xattribute != null)
				{
					this.Source = Path.Combine(tmxDir, (string)xattribute);
				}
				else
				{
					this.Format = (string)xImage.Attribute("format");
					XElement xelement = xImage.Element("data");
					TmxBase64Data tmxBase64Data = new TmxBase64Data(xelement);
					this.Data = tmxBase64Data.Data;
				}
				this.Trans = new TmxColor(xImage.Attribute("trans"));
				this.Width = (int?)xImage.Attribute("width");
				this.Height = (int?)xImage.Attribute("height");
			}
		}
	}
}
