using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;

namespace Satsuma.Drawing
{
	public sealed class GraphDrawer
	{
		public IGraph Graph { get; set; }

		public Func<Node, PointF> NodePosition { get; set; }

		public Func<Node, string> NodeCaption { get; set; }

		public Func<Node, NodeStyle> NodeStyle { get; set; }

		public Func<Arc, Pen> ArcPen { get; set; }

		public Pen DirectedPen { get; set; }

		public Pen UndirectedPen { get; set; }

		public GraphDrawer()
		{
			this.NodeCaption = (Node node) => "";
			NodeStyle defaultNodeStyle = new NodeStyle();
			this.NodeStyle = (Node node) => defaultNodeStyle;
			this.ArcPen = delegate(Arc arc)
			{
				if (!this.Graph.IsEdge(arc))
				{
					return this.DirectedPen;
				}
				return this.UndirectedPen;
			};
			this.DirectedPen = new Pen(Color.Black)
			{
				CustomEndCap = new AdjustableArrowCap(3f, 5f)
			};
			this.UndirectedPen = Pens.Black;
		}

		public void Draw(Graphics graphics, Matrix matrix = null)
		{
			PointF[] array = new PointF[2];
			PointF[] array2 = new PointF[2];
			foreach (Arc arc in this.Graph.Arcs(ArcFilter.All))
			{
				Node node = this.Graph.U(arc);
				array[0] = this.NodePosition(node);
				Node node2 = this.Graph.V(arc);
				array[1] = this.NodePosition(node2);
				if (matrix != null)
				{
					matrix.TransformPoints(array);
				}
				double num = Math.Atan2((double)(array[1].Y - array[0].Y), (double)(array[1].X - array[0].X));
				array2[0] = this.NodeStyle(node).Shape.GetBoundary(num);
				array2[1] = this.NodeStyle(node2).Shape.GetBoundary(num + 3.141592653589793);
				graphics.DrawLine(this.ArcPen(arc), array[0].X + array2[0].X, array[0].Y + array2[0].Y, array[1].X + array2[1].X, array[1].Y + array2[1].Y);
			}
			PointF[] array3 = new PointF[1];
			foreach (Node node3 in this.Graph.Nodes())
			{
				array3[0] = this.NodePosition(node3);
				if (matrix != null)
				{
					matrix.TransformPoints(array3);
				}
				this.NodeStyle(node3).DrawNode(graphics, array3[0].X, array3[0].Y, this.NodeCaption(node3));
			}
		}

		public void Draw(Graphics graphics, RectangleF box)
		{
			if (!this.Graph.Nodes().Any<Node>())
			{
				return;
			}
			float num = 0f;
			float num2 = 0f;
			float num3 = float.PositiveInfinity;
			float num4 = float.PositiveInfinity;
			float num5 = float.NegativeInfinity;
			float num6 = float.NegativeInfinity;
			foreach (Node node in this.Graph.Nodes())
			{
				PointF size = this.NodeStyle(node).Shape.Size;
				num = Math.Max(num, size.X);
				num2 = Math.Max(num2, size.Y);
				PointF pointF = this.NodePosition(node);
				num3 = Math.Min(num3, pointF.X);
				num5 = Math.Max(num5, pointF.X);
				num4 = Math.Min(num4, pointF.Y);
				num6 = Math.Max(num6, pointF.Y);
			}
			float num7 = num5 - num3;
			if (num7 == 0f)
			{
				num7 = 1f;
			}
			float num8 = num6 - num4;
			if (num8 == 0f)
			{
				num8 = 1f;
			}
			Matrix matrix = new Matrix();
			matrix.Translate(num * 0.6f, num2 * 0.6f);
			matrix.Scale((box.Width - num * 1.2f) / num7, (box.Height - num2 * 1.2f) / num8);
			matrix.Translate(-num3, -num4);
			this.Draw(graphics, matrix);
		}

		public Bitmap Draw(int width, int height, Color backColor, bool antialias = true, PixelFormat pixelFormat = PixelFormat.Format32bppArgb)
		{
			Bitmap bitmap = new Bitmap(width, height, pixelFormat);
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				graphics.SmoothingMode = (antialias ? SmoothingMode.AntiAlias : SmoothingMode.None);
				graphics.Clear(backColor);
				this.Draw(graphics, new RectangleF(0f, 0f, (float)width, (float)height));
			}
			return bitmap;
		}
	}
}
