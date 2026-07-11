using System;
using System.Globalization;
using System.Xml.Linq;

namespace Satsuma.IO.GraphML
{
	public sealed class NodeGraphics
	{
		public NodeGraphics()
		{
			double num = 0.0;
			this.Y = num;
			this.X = num;
			num = 10.0;
			this.Height = num;
			this.Width = num;
			this.Shape = NodeShape.Rectangle;
		}

		public NodeGraphics(XElement xData)
		{
			XElement xelement = Utils.ElementLocal(xData, "Geometry");
			if (xelement != null)
			{
				this.X = double.Parse(xelement.Attribute("x").Value, CultureInfo.InvariantCulture);
				this.Y = double.Parse(xelement.Attribute("y").Value, CultureInfo.InvariantCulture);
				this.Width = double.Parse(xelement.Attribute("width").Value, CultureInfo.InvariantCulture);
				this.Height = double.Parse(xelement.Attribute("height").Value, CultureInfo.InvariantCulture);
			}
			XElement xelement2 = Utils.ElementLocal(xData, "Shape");
			if (xelement2 != null)
			{
				this.Shape = this.ParseShape(xelement2.Attribute("type").Value);
			}
		}

		public double X { get; set; }

		public double Y { get; set; }

		public double Width { get; set; }

		public double Height { get; set; }

		public NodeShape Shape { get; set; }

		private NodeShape ParseShape(string s)
		{
			return (NodeShape)Math.Max(0, Array.IndexOf<string>(this.nodeShapeToString, s));
		}

		private string ShapeToGraphML(NodeShape shape)
		{
			return this.nodeShapeToString[(int)shape];
		}

		public XElement ToXml()
		{
			return new XElement("dummy", new XElement(GraphMLFormat.xmlnsY + "ShapeNode", new object[]
			{
				new XElement(GraphMLFormat.xmlnsY + "Geometry", new object[]
				{
					new XAttribute("x", this.X.ToString(CultureInfo.InvariantCulture)),
					new XAttribute("y", this.Y.ToString(CultureInfo.InvariantCulture)),
					new XAttribute("width", this.Width.ToString(CultureInfo.InvariantCulture)),
					new XAttribute("height", this.Height.ToString(CultureInfo.InvariantCulture))
				}),
				new XElement(GraphMLFormat.xmlnsY + "Shape", new XAttribute("type", this.ShapeToGraphML(this.Shape)))
			}));
		}

		public override string ToString()
		{
			return this.ToXml().ToString();
		}

		private readonly string[] nodeShapeToString = new string[]
		{
			"rectangle", "roundrectangle", "ellipse", "parallelogram", "hexagon", "triangle", "rectangle3d", "octagon", "diamond", "trapezoid",
			"trapezoid2"
		};
	}
}
