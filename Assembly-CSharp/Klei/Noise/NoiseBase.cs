using System;

namespace Klei.Noise
{
	public class NoiseBase
	{
		public virtual Type GetObjectType()
		{
			return null;
		}

		public string name { get; set; }

		public Vector2f pos { get; set; }
	}
}
