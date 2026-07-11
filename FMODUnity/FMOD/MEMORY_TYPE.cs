using System;

namespace FMOD
{
	[Flags]
	public enum MEMORY_TYPE : uint
	{
		NORMAL = 0U,
		STREAM_FILE = 1U,
		STREAM_DECODE = 2U,
		SAMPLEDATA = 4U,
		DSP_BUFFER = 8U,
		PLUGIN = 16U,
		PERSISTENT = 2097152U,
		ALL = 4294967295U
	}
}
