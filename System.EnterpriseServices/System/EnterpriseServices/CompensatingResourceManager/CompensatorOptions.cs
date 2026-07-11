using System;

namespace System.EnterpriseServices.CompensatingResourceManager
{
	[Flags]
	[Serializable]
	public enum CompensatorOptions
	{
		PreparePhase = 1,
		CommitPhase = 2,
		AbortPhase = 4,
		AllPhases = 7,
		FailIfInDoubtsRemain = 16
	}
}
