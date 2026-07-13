using System;
using UnityEngine;

namespace Unity.VectorGraphics
{
	public class SolidFill : IFill
	{
		public Color Color { get; set; }

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

		public FillMode Mode { get; set; }

		private float m_Opacity = 1f;
	}
}
