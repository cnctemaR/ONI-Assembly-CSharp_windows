using System;
using UnityEngine;

namespace Unity.VectorGraphics
{
	public class PatternFill : IFill
	{
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

		public SceneNode Pattern { get; set; }

		public Rect Rect { get; set; }

		private float m_Opacity = 1f;
	}
}
