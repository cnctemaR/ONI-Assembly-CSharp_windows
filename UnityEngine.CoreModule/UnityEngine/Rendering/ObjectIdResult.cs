using System;

namespace UnityEngine.Rendering
{
	public class ObjectIdResult
	{
		public Object[] idToObjectMapping { get; }

		internal ObjectIdResult(Object[] idToObjectMapping)
		{
			this.idToObjectMapping = idToObjectMapping;
		}

		public static int DecodeIdFromColor(Color color)
		{
			return (int)(color.r * 255f) + ((int)(color.g * 255f) << 8) + ((int)(color.b * 255f) << 16) + ((int)(color.a * 255f) << 24);
		}
	}
}
