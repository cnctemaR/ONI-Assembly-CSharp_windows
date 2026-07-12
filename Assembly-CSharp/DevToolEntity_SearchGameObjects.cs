using System;
using ImGuiNET;

public class DevToolEntity_SearchGameObjects : DevTool
{
	public DevToolEntity_SearchGameObjects(Action<DevToolEntityTarget> onSelectionMadeFn)
	{
		this.onSelectionMadeFn = onSelectionMadeFn;
	}

	protected override void RenderTo(DevPanel panel)
	{
		ImGui.Text("Not implemented yet");
	}

	private Action<DevToolEntityTarget> onSelectionMadeFn;
}
