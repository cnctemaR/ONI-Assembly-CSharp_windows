using System;
using System.Collections.Generic;
using ImGuiNET;
using UnityEngine;

public class DevTool_StoryTraits_Reveal : DevTool
{
	protected override void Render()
	{
		Option<int> cellIndexForExisting = this.GetCellIndexForExisting("Headquarters");
		bool hasValue = cellIndexForExisting.HasValue;
		if (ImGuiEx.Button("Focus on headquaters", hasValue))
		{
			this.FocusCameraOnCell(cellIndexForExisting);
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
					this.RevealAndFocusAt(cellIndexForSpawnable.Value);
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

	public void RevealAndFocusAt(int cellIndex)
	{
		int num;
		int num2;
		Grid.CellToXY(cellIndex, out num, out num2);
		GridVisibility.Reveal(num + 2, num2 + 2, 10, 10f);
		this.FocusCameraOnCell(cellIndex);
		Option<int> cellIndexForExisting = this.GetCellIndexForExisting("Headquarters");
		if (cellIndexForExisting.HasValue)
		{
			Vector3 vector = Grid.CellToPos2D(cellIndex);
			Vector3 vector2 = Grid.CellToPos2D(cellIndexForExisting);
			float num3 = 2f / Vector3.Distance(vector, vector2);
			for (float num4 = 0f; num4 < 1f; num4 += num3)
			{
				int num5;
				int num6;
				Grid.PosToXY(Vector3.Lerp(vector, vector2, num4), out num5, out num6);
				GridVisibility.Reveal(num5 + 2, num6 + 2, 4, 4f);
			}
		}
	}

	public void FocusCameraOnCell(int cellIndex)
	{
		Vector3 vector = Grid.CellToPos2D(cellIndex);
		CameraController.Instance.SetPosition(vector);
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

	private Option<int> GetCellIndexForExisting(string prefabId)
	{
		BuildingComplete[] array = global::UnityEngine.Object.FindObjectsOfType<BuildingComplete>(true);
		if (array == null)
		{
			return Option.None;
		}
		foreach (BuildingComplete buildingComplete in array)
		{
			if (prefabId == buildingComplete.Def.PrefabID)
			{
				return buildingComplete.GetCell();
			}
		}
		return Option.None;
	}
}
