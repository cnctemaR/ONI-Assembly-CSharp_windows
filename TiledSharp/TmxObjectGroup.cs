using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace TiledSharp
{
	public class TmxObjectGroup : ITmxElement
	{
		public string Name { get; private set; }

		public TmxColor Color { get; private set; }

		public TmxObjectGroup.DrawOrderType DrawOrder { get; private set; }

		public double Opacity { get; private set; }

		public bool Visible { get; private set; }

		public TmxList<TmxObjectGroup.TmxObject> Objects { get; private set; }

		public PropertyDict Properties { get; private set; }

		public TmxObjectGroup(XElement xObjectGroup)
		{
			this.Name = (string)xObjectGroup.Attribute("name");
			this.Color = new TmxColor(xObjectGroup.Attribute("color"));
			double? num = (double?)xObjectGroup.Attribute("opacity");
			this.Opacity = ((num != null) ? num.GetValueOrDefault() : 1.0);
			this.Visible = ((bool?)xObjectGroup.Attribute("visible")) ?? true;
			Dictionary<string, TmxObjectGroup.DrawOrderType> dictionary = new Dictionary<string, TmxObjectGroup.DrawOrderType>
			{
				{
					"unknown",
					TmxObjectGroup.DrawOrderType.UnknownOrder
				},
				{
					"topdown",
					TmxObjectGroup.DrawOrderType.IndexOrder
				},
				{
					"index",
					TmxObjectGroup.DrawOrderType.TopDown
				}
			};
			string text = (string)xObjectGroup.Attribute("draworder");
			if (text != null)
			{
				this.DrawOrder = dictionary[text];
			}
			this.Objects = new TmxList<TmxObjectGroup.TmxObject>();
			foreach (XElement xelement in xObjectGroup.Elements("object"))
			{
				this.Objects.Add(new TmxObjectGroup.TmxObject(xelement));
			}
			this.Properties = new PropertyDict(xObjectGroup.Element("properties"));
		}

		public class TmxObject : ITmxElement
		{
			public string Name { get; private set; }

			public TmxObjectGroup.TmxObjectType ObjectType { get; private set; }

			public string Type { get; private set; }

			public double X { get; private set; }

			public double Y { get; private set; }

			public double Width { get; private set; }

			public double Height { get; private set; }

			public double Rotation { get; private set; }

			public TmxLayerTile Tile { get; private set; }

			public bool Visible { get; private set; }

			public List<Tuple<double, double>> Points { get; private set; }

			public PropertyDict Properties { get; private set; }

			public TmxObject(XElement xObject)
			{
				this.Name = ((string)xObject.Attribute("name")) ?? "";
				this.X = (double)xObject.Attribute("x");
				this.Y = (double)xObject.Attribute("y");
				double? num = (double?)xObject.Attribute("width");
				this.Width = ((num != null) ? num.GetValueOrDefault() : 0.0);
				num = (double?)xObject.Attribute("height");
				this.Height = ((num != null) ? num.GetValueOrDefault() : 0.0);
				this.Type = (string)xObject.Attribute("type");
				this.Visible = ((bool?)xObject.Attribute("visible")) ?? true;
				num = (double?)xObject.Attribute("rotation");
				this.Rotation = ((num != null) ? num.GetValueOrDefault() : 0.0);
				XAttribute xattribute = xObject.Attribute("gid");
				XElement xelement = xObject.Element("ellipse");
				XElement xelement2 = xObject.Element("polygon");
				XElement xelement3 = xObject.Element("polyline");
				if (xattribute != null)
				{
					this.Tile = new TmxLayerTile((uint)xattribute, Convert.ToInt32(Math.Round(this.X)), Convert.ToInt32(Math.Round(this.X)));
					this.ObjectType = TmxObjectGroup.TmxObjectType.Tile;
				}
				else if (xelement != null)
				{
					this.ObjectType = TmxObjectGroup.TmxObjectType.Ellipse;
				}
				else if (xelement2 != null)
				{
					this.Points = this.ParsePoints(xelement2);
					this.ObjectType = TmxObjectGroup.TmxObjectType.Polygon;
				}
				else if (xelement3 != null)
				{
					this.Points = this.ParsePoints(xelement3);
					this.ObjectType = TmxObjectGroup.TmxObjectType.Polyline;
				}
				else
				{
					this.ObjectType = TmxObjectGroup.TmxObjectType.Basic;
				}
				this.Properties = new PropertyDict(xObject.Element("properties"));
			}

			public List<Tuple<double, double>> ParsePoints(XElement xPoints)
			{
				List<Tuple<double, double>> list = new List<Tuple<double, double>>();
				string text = (string)xPoints.Attribute("points");
				string[] array = text.Split(new char[] { ' ' });
				foreach (string text2 in array)
				{
					string[] array3 = text2.Split(new char[] { ',' });
					double num = double.Parse(array3[0]);
					double num2 = double.Parse(array3[1]);
					list.Add(Tuple.Create<double, double>(num, num2));
				}
				return list;
			}
		}

		public enum TmxObjectType
		{
			Basic,
			Tile,
			Ellipse,
			Polygon,
			Polyline
		}

		public enum DrawOrderType
		{
			UnknownOrder = -1,
			TopDown,
			IndexOrder
		}
	}
}
