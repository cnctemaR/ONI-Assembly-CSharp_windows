using System;
using Unity.Collections;

namespace UnityEngine.ParticleSystemJobs
{
	public struct ParticleSystemNativeArray3
	{
		public Vector3 this[int index]
		{
			get
			{
				return new Vector3(this.x[index], this.y[index], this.z[index]);
			}
			set
			{
				this.x[index] = value.x;
				this.y[index] = value.y;
				this.z[index] = value.z;
			}
		}

		public NativeArray<float> x;

		public NativeArray<float> y;

		public NativeArray<float> z;
	}
}
