using System;

namespace UnityEngine.Experimental.UIElements
{
	internal enum VisualTreeUpdatePhase
	{
		PersistentData,
		Bindings,
		Styles,
		Layout,
		TransformClip,
		Repaint,
		Count
	}
}
