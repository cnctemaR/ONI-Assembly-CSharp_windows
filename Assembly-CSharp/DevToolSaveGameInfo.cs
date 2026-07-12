using System;
using System.Collections.Generic;
using ImGuiNET;

public class DevToolSaveGameInfo : DevTool
{
	protected override void Render()
	{
		if (Game.Instance == null)
		{
			ImGui.Text("No game loaded");
			return;
		}
		ImGui.Text("Seed: " + CustomGameSettings.Instance.GetSettingsCoordinate());
		ImGui.Text("Generated: " + Game.Instance.dateGenerated);
		ImGui.PushItemWidth(100f);
		ImGui.NewLine();
		ImGui.Text("Changelists played on");
		ImGui.InputText("Search", ref this.clSearch, 10U);
		ImGui.PopItemWidth();
		foreach (uint num in Game.Instance.changelistsPlayedOn)
		{
			if (this.clSearch.IsNullOrWhiteSpace() || num.ToString().Contains(this.clSearch))
			{
				ImGui.Text(num.ToString());
			}
		}
		if (StoryManager.Instance != null)
		{
			ImGui.NewLine();
			ImGui.Text(string.Format("Stories (count: {0})", StoryManager.Instance.GetStoryInstances().Count));
			string text = ((StoryManager.Instance.GetHighestCoordinate() == -2) ? "Before stories" : StoryManager.Instance.GetHighestCoordinate().ToString());
			ImGui.Text("Highest generated: " + text);
			foreach (KeyValuePair<int, StoryInstance> keyValuePair in StoryManager.Instance.GetStoryInstances())
			{
				ImGui.Text(" - " + keyValuePair.Value.storyId + ": " + keyValuePair.Value.CurrentState.ToString());
			}
			if (StoryManager.Instance.GetStoryInstances().Count == 0)
			{
				ImGui.Text(" - No stories");
			}
		}
	}

	private string clSearch = "";
}
