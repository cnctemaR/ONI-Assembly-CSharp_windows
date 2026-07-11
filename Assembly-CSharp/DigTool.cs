using System;
using UnityEngine;

public class DigTool : DragTool
{
	public static void DestroyInstance()
	{
		DigTool.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		DigTool.Instance = this;
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		if (!Grid.Solid[cell])
		{
			foreach (Uprootable uprootable in Components.Uprootables.Items)
			{
				if (Grid.PosToCell(uprootable.gameObject) == cell)
				{
					uprootable.MarkForUproot();
					break;
				}
				OccupyArea area = uprootable.area;
				if (area != null && area.CheckIsOccupying(cell))
				{
					uprootable.MarkForUproot();
				}
			}
		}
		if (DebugHandler.InstantBuildMode)
		{
			if (Grid.IsValidCell(cell) && Grid.Solid[cell] && !Grid.Foundation[cell])
			{
				WorldDamage.Instance.DestroyCell(cell, -1);
			}
		}
		else
		{
			GameObject gameObject = DigTool.PlaceDig(cell, distFromOrigin);
			if (gameObject != null)
			{
				Prioritizable component = gameObject.GetComponent<Prioritizable>();
				if (component != null)
				{
					component.SetMasterPriority(ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority());
				}
			}
		}
	}

	public static GameObject PlaceDig(int cell, int animationDelay = 0)
	{
		if (Grid.Solid[cell] && !Grid.Foundation[cell] && Grid.Objects[cell, 7] == null)
		{
			for (int i = 0; i < 37; i++)
			{
				if (Grid.Objects[cell, i] != null && Grid.Objects[cell, i].GetComponent<Constructable>() != null)
				{
					return null;
				}
			}
			GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(new Tag("DigPlacer")), null, null);
			gameObject.SetActive(true);
			Grid.Objects[cell, 7] = gameObject;
			Vector3 vector = Grid.CellToPosCBC(cell, DigTool.Instance.visualizerLayer);
			float num = -0.15f;
			vector.z += num;
			gameObject.transform.SetPosition(vector);
			gameObject.GetComponentInChildren<EasingAnimations>().PlayAnimation("ScaleUp", Mathf.Max(0f, (float)animationDelay * 0.02f));
			return gameObject;
		}
		if (Grid.Objects[cell, 7] != null)
		{
			return Grid.Objects[cell, 7];
		}
		return null;
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		ToolMenu.Instance.PriorityScreen.Show(true);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		ToolMenu.Instance.PriorityScreen.Show(false);
	}

	public static DigTool Instance;
}
