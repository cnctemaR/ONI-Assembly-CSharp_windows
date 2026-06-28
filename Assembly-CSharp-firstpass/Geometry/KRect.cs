using System;
using UnityEngine;

namespace Geometry
{
	public struct KRect
	{
		public KRect(Vector2 min, Vector2 max)
		{
			this.min = min;
			this.max = max;
		}

		public KRect(float x0, float y0, float x1, float y1)
		{
			this.min = new Vector2(x0, y0);
			this.max = new Vector2(x1, y1);
		}

		public Vector2 min;

		public Vector2 max;
	}
}
