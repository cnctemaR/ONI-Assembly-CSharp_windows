using System;

namespace Unity.Burst
{
	public enum BurstExecutionEnvironment
	{
		Default,
		NonDeterministic = 0,
		Deterministic
	}
}
