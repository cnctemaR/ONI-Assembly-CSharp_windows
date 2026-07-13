using System;
using UnityEngine;

namespace Unity.VectorGraphics
{
	public class Stroke
	{
		public Color Color
		{
			get
			{
				SolidFill solidFill = this.Fill as SolidFill;
				bool flag = solidFill == null;
				Color color;
				if (flag)
				{
					color = default(Color);
				}
				else
				{
					color = solidFill.Color;
				}
				return color;
			}
			set
			{
				this.Fill = new SolidFill
				{
					Color = value
				};
			}
		}

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

		public float HalfThickness { get; set; }

		public float[] Pattern { get; set; }

		public float PatternOffset { get; set; }

		public float TippedCornerLimit { get; set; }

		private Matrix2D m_FillTransform = Matrix2D.identity;
	}
}
