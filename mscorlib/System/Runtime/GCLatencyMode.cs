using System;

namespace System.Runtime
{
	[Serializable]
	public enum GCLatencyMode
	{
		Batch,
		Interactive,
		LowLatency
	}
}
