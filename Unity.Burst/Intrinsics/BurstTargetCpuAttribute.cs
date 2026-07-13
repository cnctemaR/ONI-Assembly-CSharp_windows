using System;

namespace Unity.Burst.Intrinsics
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	[BurstRuntime.PreserveAttribute]
	internal sealed class BurstTargetCpuAttribute : Attribute
	{
		public BurstTargetCpuAttribute(BurstTargetCpu TargetCpu)
		{
			this.TargetCpu = TargetCpu;
		}

		public readonly BurstTargetCpu TargetCpu;
	}
}
