using System;

namespace UnityEngine.UIElements
{
	[Flags]
	public enum VersionChangeType
	{
		Bindings = 1,
		ViewData = 2,
		Hierarchy = 4,
		Layout = 8,
		StyleSheet = 16,
		Styles = 32,
		Overflow = 64,
		BorderRadius = 128,
		BorderWidth = 256,
		Transform = 512,
		Size = 1024,
		Repaint = 2048,
		Opacity = 4096,
		Color = 8192,
		RenderHints = 16384,
		TransitionProperty = 32768,
		EventCallbackCategories = 65536,
		DisableRendering = 131072,
		BindingRegistration = 262144,
		DataSource = 524288,
		Picking = 1048576
	}
}
