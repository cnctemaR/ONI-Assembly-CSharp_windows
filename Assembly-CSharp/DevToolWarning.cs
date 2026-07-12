using System;
using ImGuiNET;
using STRINGS;
using UnityEngine;

public class DevToolWarning
{
	public DevToolWarning()
	{
		this.Name = UI.FRONTEND.DEVTOOLS.TITLE;
	}

	public void DrawMenuBar()
	{
		if (ImGui.BeginMainMenuBar())
		{
			ImGui.Checkbox(this.Name, ref this.ShouldDrawWindow);
			ImGui.EndMainMenuBar();
		}
	}

	public void DrawWindow(out bool isOpen)
	{
		ImGuiWindowFlags imGuiWindowFlags = ImGuiWindowFlags.None;
		isOpen = true;
		if (ImGui.Begin(this.Name + "###ID_DevToolWarning", ref isOpen, imGuiWindowFlags))
		{
			if (!isOpen)
			{
				ImGui.End();
				return;
			}
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
				isOpen = false;
			}
			ImGui.End();
		}
	}

	private bool showAgain;

	public string Name;

	public bool ShouldDrawWindow;
}
