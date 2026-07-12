using System;
using ImGuiNET;
using UnityEngine;

public class DevToolPrintingPodDebug : DevTool
{
	protected override void RenderTo(DevPanel panel)
	{
		if (Immigration.Instance != null)
		{
			this.ShowButtons();
			return;
		}
		ImGui.Text("Game not available");
	}

	private void ShowButtons()
	{
		if (Components.Telepads.Count == 0)
		{
			ImGui.Text("No printing pods available");
			return;
		}
		ImGui.Text("Time until next print available: " + Mathf.CeilToInt(Immigration.Instance.timeBeforeSpawn).ToString() + "s");
		if (ImGui.Button("Activate now"))
		{
			Immigration.Instance.timeBeforeSpawn = 0f;
		}
		if (ImGui.Button("Shuffle Options"))
		{
			if (ImmigrantScreen.instance.Telepad == null)
			{
				ImmigrantScreen.InitializeImmigrantScreen(Components.Telepads[0]);
				return;
			}
			ImmigrantScreen.instance.DebugShuffleOptions();
		}
	}
}
