using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[Serializable]
	public enum GCHandleType
	{
		Weak,
		WeakTrackResurrection,
		Normal,
		Pinned
	}
}
