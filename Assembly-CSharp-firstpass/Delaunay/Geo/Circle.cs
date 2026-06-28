using System;
using UnityEngine;

namespace Delaunay.Geo
{
	public sealed class Circle
	{
		public Circle(float centerX, float centerY, float radius)
		{
			this.center = new Vector2(centerX, centerY);
			this.radius = radius;
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"Circle (center: ",
				this.center.ToString(),
				"; radius: ",
				this.radius.ToString(),
				")"
			});
		}

		public Vector2 center;

		public float radius;
	}
}
