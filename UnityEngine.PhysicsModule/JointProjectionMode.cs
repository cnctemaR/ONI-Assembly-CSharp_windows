using System;
using System.ComponentModel;

namespace UnityEngine
{
	public enum JointProjectionMode
	{
		None,
		PositionAndRotation,
		[Obsolete("JointProjectionMode.PositionOnly is no longer supported", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		PositionOnly
	}
}
