using System;

namespace Unity.Profiling.LowLevel
{
	public enum ProfilerMarkerDataType : byte
	{
		InstanceId = 1,
		Int32,
		UInt32,
		Int64,
		UInt64,
		Float,
		Double,
		String16 = 9,
		Blob8 = 11,
		GfxResourceId
	}
}
