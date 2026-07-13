using System;
using UnityEngine;

namespace Unity.VectorGraphics
{
	public class TextureFill : IFill
	{
		public Texture2D Texture { get; set; }

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

		private float m_Opacity = 1f;
	}
}
