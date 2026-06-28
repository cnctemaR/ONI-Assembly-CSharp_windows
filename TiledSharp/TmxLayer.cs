using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace TiledSharp
{
	public class TmxLayer : ITmxElement
	{
		public string Name { get; private set; }

		public double Opacity { get; private set; }

		public bool Visible { get; private set; }

		public List<TmxLayerTile> Tiles { get; private set; }

		public PropertyDict Properties { get; private set; }

		public TmxLayer(XElement xLayer, int width, int height)
		{
			this.Name = (string)xLayer.Attribute("name");
			double? num = (double?)xLayer.Attribute("opacity");
			this.Opacity = ((num != null) ? num.GetValueOrDefault() : 1.0);
			this.Visible = ((bool?)xLayer.Attribute("visible")) ?? true;
			XElement xelement = xLayer.Element("data");
			string text = (string)xelement.Attribute("encoding");
			this.Tiles = new List<TmxLayerTile>();
			if (text == "base64")
			{
				TmxBase64Data tmxBase64Data = new TmxBase64Data(xelement);
				Stream data = tmxBase64Data.Data;
				using (BinaryReader binaryReader = new BinaryReader(data))
				{
					for (int i = 0; i < height; i++)
					{
						for (int j = 0; j < width; j++)
						{
							this.Tiles.Add(new TmxLayerTile(binaryReader.ReadUInt32(), j, i));
						}
					}
				}
			}
			else if (text == "csv")
			{
				string value = xelement.Value;
				int num2 = 0;
				foreach (string text2 in value.Split(new char[] { ',' }))
				{
					uint num3 = uint.Parse(text2.Trim());
					int num4 = num2 % width;
					int num5 = num2 / width;
					this.Tiles.Add(new TmxLayerTile(num3, num4, num5));
					num2++;
				}
			}
			else
			{
				if (text != null)
				{
					throw new Exception("TmxLayer: Unknown encoding.");
				}
				int num2 = 0;
				foreach (XElement xelement2 in xelement.Elements("tile"))
				{
					uint num3 = (uint)xelement2.Attribute("gid");
					int num4 = num2 % width;
					int num5 = num2 / width;
					this.Tiles.Add(new TmxLayerTile(num3, num4, num5));
					num2++;
				}
			}
			this.Properties = new PropertyDict(xLayer.Element("properties"));
		}
	}
}
