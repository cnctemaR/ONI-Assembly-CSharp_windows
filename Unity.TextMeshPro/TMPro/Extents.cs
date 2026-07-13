using System;
using UnityEngine;

namespace TMPro
{
	public struct Extents
	{
		public Extents(Vector2 min, Vector2 max)
		{
			this.min = min;
			this.max = max;
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"Min (",
				this.min.x.ToString("f2"),
				", ",
				this.min.y.ToString("f2"),
				")   Max (",
				this.max.x.ToString("f2"),
				", ",
				this.max.y.ToString("f2"),
				")"
			});
		}

		internal static Extents zero = new Extents(Vector2.zero, Vector2.zero);

		internal static Extents uninitialized = new Extents(new Vector2(32767f, 32767f), new Vector2(-32767f, -32767f));

		public Vector2 min;

		public Vector2 max;
	}
}
