using System;
using UnityEngine;

public static class DevToolUtil
{
	public static DevPanel Open(DevTool devTool)
	{
		return DevToolManager.Instance.panels.AddPanelFor(devTool);
	}

	public static DevPanel Open<T>() where T : DevTool, new()
	{
		return DevToolManager.Instance.panels.AddPanelFor<T>();
	}

	public static void Close(DevTool devTool)
	{
		devTool.ClosePanel();
	}

	public static void Close(DevPanel devPanel)
	{
		devPanel.Close();
	}

	public static string GenerateDevToolName(DevTool devTool)
	{
		return DevToolUtil.GenerateDevToolName(devTool.GetType());
	}

	public static string GenerateDevToolName(Type devToolType)
	{
		string text;
		if (DevToolManager.Instance != null && DevToolManager.Instance.devToolNameDict.TryGetValue(devToolType, out text))
		{
			return text;
		}
		string text2 = devToolType.Name;
		if (text2.StartsWith("DevTool_"))
		{
			text2 = text2.Substring("DevTool_".Length);
		}
		else if (text2.StartsWith("DevTool"))
		{
			text2 = text2.Substring("DevTool".Length);
		}
		return text2;
	}

	public static bool CanRevealAndFocus(GameObject gameObject)
	{
		return DevToolUtil.GetCellIndexFor(gameObject).HasValue;
	}

	public static void RevealAndFocus(GameObject gameObject)
	{
		Option<int> cellIndexFor = DevToolUtil.GetCellIndexFor(gameObject);
		if (!cellIndexFor.HasValue)
		{
			return;
		}
		DevToolUtil.RevealAndFocusAt(cellIndexFor.Value);
		if (!gameObject.GetComponent<KSelectable>().IsNullOrDestroyed())
		{
			SelectTool.Instance.Select(gameObject.GetComponent<KSelectable>(), false);
			return;
		}
		SelectTool.Instance.Select(null, false);
	}

	public static void FocusCameraOnCell(int cellIndex)
	{
		Vector3 vector = Grid.CellToPos2D(cellIndex);
		CameraController.Instance.SetPosition(vector);
	}

	public static Option<int> GetCellIndexFor(GameObject gameObject)
	{
		if (gameObject.IsNullOrDestroyed())
		{
			return Option.None;
		}
		if (!gameObject.GetComponent<RectTransform>().IsNullOrDestroyed())
		{
			return Option.None;
		}
		return Grid.PosToCell(gameObject);
	}

	public static Option<int> GetCellIndexForUniqueBuilding(string prefabId)
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

	public static void RevealAndFocusAt(int cellIndex)
	{
		int num;
		int num2;
		Grid.CellToXY(cellIndex, out num, out num2);
		GridVisibility.Reveal(num + 2, num2 + 2, 10, 10f);
		DevToolUtil.FocusCameraOnCell(cellIndex);
		Option<int> cellIndexForUniqueBuilding = DevToolUtil.GetCellIndexForUniqueBuilding("Headquarters");
		if (cellIndexForUniqueBuilding.IsSome())
		{
			Vector3 vector = Grid.CellToPos2D(cellIndex);
			Vector3 vector2 = Grid.CellToPos2D(cellIndexForUniqueBuilding.Unwrap());
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
}
