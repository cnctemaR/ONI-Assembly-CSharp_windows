using System;
using UnityEngine;

namespace TMPro
{
	[Serializable]
	public struct GlyphAnchorPoint
	{
		public float xCoordinate
		{
			get
			{
				return this.m_XCoordinate;
			}
			set
			{
				this.m_XCoordinate = value;
			}
		}

		public float yCoordinate
		{
			get
			{
				return this.m_YCoordinate;
			}
			set
			{
				this.m_YCoordinate = value;
			}
		}

		[SerializeField]
		private float m_XCoordinate;

		[SerializeField]
		private float m_YCoordinate;
	}
}
