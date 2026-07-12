using System;
using System.Collections.Generic;
using ImGuiNET;

public class DevToolSpaceScannerNetwork : DevTool
{
	public DevToolSpaceScannerNetwork()
	{
		this.tableDrawer = ImGuiObjectTableDrawer<DevToolSpaceScannerNetwork.Entry>.New().Column("WorldId", (DevToolSpaceScannerNetwork.Entry e) => e.worldId).Column("Network Quality (0->1)", (DevToolSpaceScannerNetwork.Entry e) => e.networkQuality)
			.Column("Targets Detected", (DevToolSpaceScannerNetwork.Entry e) => e.targetsString)
			.FixedHeight(300f)
			.Build();
	}

	protected override void RenderTo(DevPanel panel)
	{
		if (Game.Instance == null)
		{
			ImGui.Text("Game instance is null");
			return;
		}
		if (Game.Instance.spaceScannerNetworkManager == null)
		{
			ImGui.Text("SpaceScannerNetworkQualityManager instance is null");
			return;
		}
		if (ClusterManager.Instance == null)
		{
			ImGui.Text("ClusterManager instance is null");
			return;
		}
		if (ImGui.CollapsingHeader("Worlds Data"))
		{
			this.tableDrawer.Draw(DevToolSpaceScannerNetwork.GetData());
		}
		if (ImGui.CollapsingHeader("Full DevToolSpaceScannerNetwork Info"))
		{
			ImGuiEx.DrawObject(Game.Instance.spaceScannerNetworkManager, null);
		}
	}

	public static IEnumerable<DevToolSpaceScannerNetwork.Entry> GetData()
	{
		foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
		{
			yield return new DevToolSpaceScannerNetwork.Entry(worldContainer.id, Game.Instance.spaceScannerNetworkManager.GetQualityForWorld(worldContainer.id), DevToolSpaceScannerNetwork.GetTargetsString(worldContainer));
		}
		IEnumerator<WorldContainer> enumerator = null;
		yield break;
		yield break;
	}

	public static string GetTargetsString(WorldContainer world)
	{
		SpaceScannerWorldData spaceScannerWorldData;
		if (!Game.Instance.spaceScannerNetworkManager.DEBUG_GetWorldIdToDataMap().TryGetValue(world.id, out spaceScannerWorldData))
		{
			return "<none>";
		}
		if (spaceScannerWorldData.targetIdsDetected.Count == 0)
		{
			return "<none>";
		}
		return string.Join(",", spaceScannerWorldData.targetIdsDetected);
	}

	private ImGuiObjectTableDrawer<DevToolSpaceScannerNetwork.Entry> tableDrawer;

	public readonly struct Entry
	{
		public Entry(int worldId, float networkQuality, string targetsString)
		{
			this.worldId = worldId;
			this.networkQuality = networkQuality;
			this.targetsString = targetsString;
		}

		public readonly int worldId;

		public readonly float networkQuality;

		public readonly string targetsString;
	}
}
