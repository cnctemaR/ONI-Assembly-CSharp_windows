using System;
using System.Drawing;

namespace Satsuma.Drawing
{
	public sealed class NodeShape : INodeShape
	{
		public NodeShapeKind Kind { get; private set; }

		public PointF Size { get; private set; }

		public NodeShape(NodeShapeKind kind, PointF size)
		{
			this.Kind = kind;
			this.Size = size;
			this.rect = new RectangleF(-size.X * 0.5f, -size.Y * 0.5f, size.X, size.Y);
			switch (this.Kind)
			{
			case NodeShapeKind.Diamond:
				this.points = new PointF[]
				{
					NodeShape.P(this.rect, 0f, 0.5f),
					NodeShape.P(this.rect, 0.5f, 1f),
					NodeShape.P(this.rect, 1f, 0.5f),
					NodeShape.P(this.rect, 0.5f, 0f)
				};
				return;
			case NodeShapeKind.Ellipse:
				break;
			case NodeShapeKind.Rectangle:
				this.points = new PointF[]
				{
					this.rect.Location,
					new PointF(this.rect.Left, this.rect.Bottom),
					new PointF(this.rect.Right, this.rect.Bottom),
					new PointF(this.rect.Right, this.rect.Top)
				};
				return;
			case NodeShapeKind.Triangle:
				this.points = new PointF[]
				{
					NodeShape.P(this.rect, 0.5f, 0f),
					NodeShape.P(this.rect, 0f, 1f),
					NodeShape.P(this.rect, 1f, 1f)
				};
				return;
			case NodeShapeKind.UpsideDownTriangle:
				this.points = new PointF[]
				{
					NodeShape.P(this.rect, 0.5f, 1f),
					NodeShape.P(this.rect, 0f, 0f),
					NodeShape.P(this.rect, 1f, 0f)
				};
				break;
			default:
				return;
			}
		}

		private static PointF P(RectangleF rect, float x, float y)
		{
			return new PointF(rect.Left + rect.Width * x, rect.Top + rect.Height * y);
		}

		public void Draw(Graphics graphics, Pen pen, Brush brush)
		{
			NodeShapeKind kind = this.Kind;
			if (kind == NodeShapeKind.Ellipse)
			{
				graphics.FillEllipse(brush, this.rect);
				graphics.DrawEllipse(pen, this.rect);
				return;
			}
			graphics.FillPolygon(brush, this.points);
			graphics.DrawPolygon(pen, this.points);
		}

		public PointF GetBoundary(double angle)
		{
			double num = Math.Cos(angle);
			double num2 = Math.Sin(angle);
			NodeShapeKind kind = this.Kind;
			if (kind == NodeShapeKind.Ellipse)
			{
				return new PointF((float)((double)(this.Size.X * 0.5f) * num), (float)((double)(this.Size.Y * 0.5f) * num2));
			}
			for (int i = 0; i < this.points.Length; i++)
			{
				int num3 = (i + 1) % this.points.Length;
				float num4 = (float)(((double)this.points[i].Y * num - (double)this.points[i].X * num2) / ((double)(this.points[num3].X - this.points[i].X) * num2 - (double)(this.points[num3].Y - this.points[i].Y) * num));
				if (num4 >= 0f && num4 <= 1f)
				{
					PointF pointF = new PointF(this.points[i].X + num4 * (this.points[num3].X - this.points[i].X), this.points[i].Y + num4 * (this.points[num3].Y - this.points[i].Y));
					if ((double)pointF.X * num + (double)pointF.Y * num2 > 0.0)
					{
						return pointF;
					}
				}
			}
			return new PointF(0f, 0f);
		}

		private readonly RectangleF rect;

		private readonly PointF[] points;
	}
}
