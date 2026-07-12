using System;
using ImGuiNET;
using STRINGS;
using UnityEngine;

public class DevToolWarning : DevTool
{
	public DevToolWarning()
	{
		this.Name = UI.FRONTEND.DEVTOOLS.TITLE;
	}

	protected override void Render()
	{
		ImGui.SetWindowSize(new Vector2(500f, 250f));
		ImGui.TextWrapped(UI.FRONTEND.DEVTOOLS.WARNING);
		ImGui.Spacing();
		ImGui.Spacing();
		ImGui.Spacing();
		ImGui.Spacing();
		ImGui.Checkbox(UI.FRONTEND.DEVTOOLS.DONTSHOW, ref this.showAgain);
		if (ImGui.Button(UI.FRONTEND.DEVTOOLS.BUTTON))
		{
			if (this.showAgain)
			{
				KPlayerPrefs.SetInt("ShowDevtools", 1);
			}
			DevToolManager.Instance.UserAcceptedWarning = true;
			base.Hide();
		}
	}

	private bool showAgain;
}
