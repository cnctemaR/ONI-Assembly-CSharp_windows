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

	public static DevPanel DebugObject<T>(T obj)
	{
		return DevToolUtil.Open(new DevToolObjectViewer<T>(() => obj));
	}

	public static DevPanel DebugObject<T>(Func<T> get_obj_fn)
	{
		return DevToolUtil.Open(new DevToolObjectViewer<T>(get_obj_fn));
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
		int num;
		return DevToolUtil.TryGetCellIndexFor(gameObject, out num);
	}

	public static void RevealAndFocus(GameObject gameObject)
	{
		int num;
		if (DevToolUtil.TryGetCellIndexFor(gameObject, out num))
		{
			return;
		}
		DevToolUtil.RevealAndFocusAt(num);
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

	public static bool TryGetCellIndexFor(GameObject gameObject, out int cellIndex)
	{
		cellIndex = -1;
		if (gameObject.IsNullOrDestroyed())
		{
			return false;
		}
		if (!gameObject.GetComponent<RectTransform>().IsNullOrDestroyed())
		{
			return false;
		}
		cellIndex = Grid.PosToCell(gameObject);
		return true;
	}

	public static bool TryGetCellIndexForUniqueBuilding(string prefabId, out int index)
	{
		index = -1;
		BuildingComplete[] array = global::UnityEngine.Object.FindObjectsOfType<BuildingComplete>(true);
		if (array == null)
		{
			return false;
		}
		foreach (BuildingComplete buildingComplete in array)
		{
			if (prefabId == buildingComplete.Def.PrefabID)
			{
				index = buildingComplete.GetCell();
				return true;
			}
		}
		return false;
	}

	public static void RevealAndFocusAt(int cellIndex)
	{
		int num;
		int num2;
		Grid.CellToXY(cellIndex, out num, out num2);
		GridVisibility.Reveal(num + 2, num2 + 2, 10, 10f);
		DevToolUtil.FocusCameraOnCell(cellIndex);
		int num3;
		if (DevToolUtil.TryGetCellIndexForUniqueBuilding("Headquarters", out num3))
		{
			Vector3 vector = Grid.CellToPos2D(cellIndex);
			Vector3 vector2 = Grid.CellToPos2D(num3);
			float num4 = 2f / Vector3.Distance(vector, vector2);
			for (float num5 = 0f; num5 < 1f; num5 += num4)
			{
				int num6;
				int num7;
				Grid.PosToXY(Vector3.Lerp(vector, vector2, num5), out num6, out num7);
				GridVisibility.Reveal(num6 + 2, num7 + 2, 4, 4f);
			}
		}
	}

	public enum TextAlignment
	{
		Center,
		Left,
		Right
	}
}
