using System;
using ImGuiNET;

public class DevToolBigBaseMutations : DevTool
{
	protected override void RenderTo(DevPanel panel)
	{
		if (Game.Instance != null)
		{
			this.ShowButtons();
			return;
		}
		ImGui.Text("Game not available");
	}

	private void ShowButtons()
	{
		if (ImGui.Button("Destroy Ladders"))
		{
			this.DestroyGameObjects<Ladder>(Components.Ladders, Tag.Invalid);
		}
		if (ImGui.Button("Destroy Tiles"))
		{
			this.DestroyGameObjects<BuildingComplete>(Components.BuildingCompletes, GameTags.FloorTiles);
		}
		if (ImGui.Button("Destroy Wires"))
		{
			this.DestroyGameObjects<BuildingComplete>(Components.BuildingCompletes, GameTags.Wires);
		}
		if (ImGui.Button("Destroy Pipes"))
		{
			this.DestroyGameObjects<BuildingComplete>(Components.BuildingCompletes, GameTags.Pipes);
		}
	}

	private void DestroyGameObjects<T>(Components.Cmps<T> componentsList, Tag filterForTag)
	{
		for (int i = componentsList.Count - 1; i >= 0; i--)
		{
			if (!componentsList[i].IsNullOrDestroyed() && (!(filterForTag != Tag.Invalid) || (componentsList[i] as KMonoBehaviour).gameObject.HasTag(filterForTag)))
			{
				Util.KDestroyGameObject(componentsList[i] as KMonoBehaviour);
			}
		}
	}
}
