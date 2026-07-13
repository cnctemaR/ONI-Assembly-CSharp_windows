using System;

namespace Unity.VectorGraphics
{
	public class Shape
	{
		public BezierContour[] Contours { get; set; }

		public IFill Fill { get; set; }

		public Matrix2D FillTransform
		{
			get
			{
				return this.m_FillTransform;
			}
			set
			{
				this.m_FillTransform = value;
			}
		}

		public PathProperties PathProps { get; set; }

		public bool IsConvex { get; set; }

		private Matrix2D m_FillTransform = Matrix2D.identity;
	}
}
