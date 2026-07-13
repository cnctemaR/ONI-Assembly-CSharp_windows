using System;
using System.Collections.Generic;

namespace ProcGen.Noise
{
	[Serializable]
	public class FloatList : NoiseBase
	{
		public override Type GetObjectType()
		{
			return typeof(FloatList);
		}

		public List<float> points { get; set; }

		public FloatList()
		{
			this.points = new List<float>();
			this.points.Add(0f);
			this.points.Add(1f);
		}
	}
}
