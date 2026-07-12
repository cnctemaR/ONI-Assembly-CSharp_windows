using System;
using System.Collections.Generic;
using ImGuiNET;
using UnityEngine;

public class DevTool_StoryTraits_Reveal : DevTool
{
	protected override void RenderTo(DevPanel panel)
	{
		Option<int> cellIndexForUniqueBuilding = DevToolUtil.GetCellIndexForUniqueBuilding("Headquarters");
		bool hasValue = cellIndexForUniqueBuilding.HasValue;
		if (ImGuiEx.Button("Focus on headquaters", hasValue))
		{
			DevToolUtil.FocusCameraOnCell(cellIndexForUniqueBuilding);
		}
		if (!hasValue)
		{
			ImGuiEx.TooltipForPrevious("Couldn't find headquaters");
		}
		if (ImGui.CollapsingHeader("Search world for entity", ImGuiTreeNodeFlags.DefaultOpen))
		{
			Option<IReadOnlyList<WorldGenSpawner.Spawnable>> allSpawnables = this.GetAllSpawnables();
			if (!allSpawnables.HasValue)
			{
				ImGui.Text("Couldn't find a list of spawnables");
				return;
			}
			foreach (string text in this.GetPrefabIDsToSearchFor())
			{
				Option<int> cellIndexForSpawnable = this.GetCellIndexForSpawnable(text, allSpawnables.Value);
				string text2 = "\"" + text + "\"";
				bool hasValue2 = cellIndexForSpawnable.HasValue;
				if (ImGuiEx.Button("Reveal and focus on " + text2, hasValue2))
				{
					DevToolUtil.RevealAndFocusAt(cellIndexForSpawnable.Value);
				}
				if (!hasValue2)
				{
					ImGuiEx.TooltipForPrevious("Couldn't find a cell that contained a spawnable with component " + text2);
				}
			}
		}
	}

	public IEnumerable<string> GetPrefabIDsToSearchFor()
	{
		yield return "MegaBrainTank";
		yield return "GravitasCreatureManipulator";
		yield break;
	}

	private Option<ClusterManager> GetClusterManager()
	{
		if (ClusterManager.Instance == null)
		{
			return Option.None;
		}
		return ClusterManager.Instance;
	}

	private Option<int> GetCellIndexForSpawnable(string prefabId, IReadOnlyList<WorldGenSpawner.Spawnable> spawnablesToSearch)
	{
		foreach (WorldGenSpawner.Spawnable spawnable in spawnablesToSearch)
		{
			if (prefabId == spawnable.spawnInfo.id)
			{
				return spawnable.cell;
			}
		}
		return Option.None;
	}

	private Option<IReadOnlyList<WorldGenSpawner.Spawnable>> GetAllSpawnables()
	{
		WorldGenSpawner worldGenSpawner = global::UnityEngine.Object.FindObjectOfType<WorldGenSpawner>(true);
		if (worldGenSpawner == null)
		{
			return Option.None;
		}
		IReadOnlyList<WorldGenSpawner.Spawnable> spawnables = worldGenSpawner.GetSpawnables();
		if (spawnables == null)
		{
			return Option.None;
		}
		return Option.Some<IReadOnlyList<WorldGenSpawner.Spawnable>>(spawnables);
	}
}
