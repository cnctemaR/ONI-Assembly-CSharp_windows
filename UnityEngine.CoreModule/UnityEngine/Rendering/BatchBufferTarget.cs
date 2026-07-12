using System;

namespace UnityEngine.Rendering
{
	public enum BatchBufferTarget
	{
		Unknown,
		UnsupportedByUnderlyingGraphicsApi = -1,
		RawBuffer = 1,
		ConstantBuffer
	}
}
