using System;
using UnityEngine;

namespace Unity.VectorGraphics
{
	public class GradientFill : IFill
	{
		public GradientFillType Type { get; set; }

		public GradientStop[] Stops { get; set; }

		public FillMode Mode { get; set; }

		public float Opacity
		{
			get
			{
				return this.m_Opacity;
			}
			set
			{
				this.m_Opacity = value;
			}
		}

		public AddressMode Addressing { get; set; }

		public Vector2 RadialFocus { get; set; }

		private float m_Opacity = 1f;
	}
}
