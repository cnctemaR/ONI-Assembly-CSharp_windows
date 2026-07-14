using System;
using ImGuiNET;

public class DevToolDebugModeToggle : DevTool
{
	public DevToolDebugModeToggle()
	{
		this.RequiresGameRunning = true;
	}

	protected override void RenderTo(DevPanel panel)
	{
		bool instantBuildMode = DebugHandler.InstantBuildMode;
		if (ImGui.Checkbox("Instant Build Mode (Ctrl+F4)", ref instantBuildMode))
		{
			DebugHandler.ToggleInstantBuildMode();
		}
	}
}
