using System;

namespace UnityEngine.Experimental.UIElements
{
	[Flags]
	internal enum VersionChangeType
	{
		Bindings = 256,
		PersistentData = 128,
		Hierarchy = 64,
		Layout = 32,
		StyleSheet = 16,
		Styles = 8,
		Transform = 4,
		Clip = 2,
		Repaint = 1
	}
}
