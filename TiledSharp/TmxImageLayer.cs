using System;
using System.Xml.Linq;

namespace TiledSharp
{
	public class TmxImageLayer : ITmxElement
	{
		public string Name { get; private set; }

		public int Width { get; private set; }

		public int Height { get; private set; }

		public bool Visible { get; private set; }

		public double Opacity { get; private set; }

		public TmxImage Image { get; private set; }

		public PropertyDict Properties { get; private set; }

		public TmxImageLayer(XElement xImageLayer, string tmxDir = "")
		{
			this.Name = (string)xImageLayer.Attribute("name");
			this.Width = (int)xImageLayer.Attribute("width");
			this.Height = (int)xImageLayer.Attribute("height");
			this.Visible = ((bool?)xImageLayer.Attribute("visible")) ?? true;
			double? num = (double?)xImageLayer.Attribute("opacity");
			this.Opacity = ((num != null) ? num.GetValueOrDefault() : 1.0);
			this.Image = new TmxImage(xImageLayer.Element("image"), tmxDir);
			this.Properties = new PropertyDict(xImageLayer.Element("properties"));
		}
	}
}
