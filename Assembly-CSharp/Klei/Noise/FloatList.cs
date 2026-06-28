using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klei.Noise
{
	[Serializable]
	public class FloatList : NoiseBase
	{
		public FloatList()
		{
			this.points = new List<float>();
		}

		public override Type GetObjectType()
		{
			return typeof(FloatList);
		}

		[SerializeField]
		public List<float> points { get; set; }
	}
}
