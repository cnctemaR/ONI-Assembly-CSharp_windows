using System;
using System.Drawing;

namespace Satsuma.Drawing
{
	public interface INodeShape
	{
		PointF Size { get; }

		void Draw(Graphics graphics, Pen pen, Brush brush);

		PointF GetBoundary(double angle);
	}
}
