using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Satsuma.Drawing
{
	public sealed class NodeStyle
	{
		public Pen Pen { get; set; }

		public Brush Brush { get; set; }

		public INodeShape Shape { get; set; }

		public Font TextFont { get; set; }

		public Brush TextBrush { get; set; }

		public NodeStyle()
		{
			this.Pen = Pens.Black;
			this.Brush = Brushes.White;
			this.Shape = NodeStyle.DefaultShape;
			this.TextFont = SystemFonts.DefaultFont;
			this.TextBrush = Brushes.Black;
		}

		internal void DrawNode(Graphics graphics, float x, float y, string text)
		{
			GraphicsState graphicsState = graphics.Save();
			graphics.TranslateTransform(x, y);
			this.Shape.Draw(graphics, this.Pen, this.Brush);
			if (text != "")
			{
				graphics.DrawString(text, this.TextFont, this.TextBrush, 0f, 0f, new StringFormat
				{
					Alignment = StringAlignment.Center,
					LineAlignment = StringAlignment.Center
				});
			}
			graphics.Restore(graphicsState);
		}

		public static readonly INodeShape DefaultShape = new NodeShape(NodeShapeKind.Ellipse, new PointF(10f, 10f));
	}
}
