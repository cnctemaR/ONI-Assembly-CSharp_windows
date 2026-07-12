using System;

namespace ImGuiNET
{
	public struct ImGuiWindowClass
	{
		public uint ClassId;

		public uint ParentViewportId;

		public ImGuiViewportFlags ViewportFlagsOverrideSet;

		public ImGuiViewportFlags ViewportFlagsOverrideClear;

		public ImGuiTabItemFlags TabItemFlagsOverrideSet;

		public ImGuiDockNodeFlags DockNodeFlagsOverrideSet;

		public ImGuiDockNodeFlags DockNodeFlagsOverrideClear;

		public byte DockingAlwaysTabBar;

		public byte DockingAllowUnclassed;
	}
}
