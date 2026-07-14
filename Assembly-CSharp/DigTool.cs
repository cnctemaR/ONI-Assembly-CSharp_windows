using System;
using UnityEngine;

public class DigTool : FilteredDragTool
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

	protected override void GetDefaultFilters(out ToolParameterMenu.ToggleData[] filters)
	{
		filters = new ToolParameterMenu.ToggleData[]
		{
			new ToolParameterMenu.ToggleData(ToolParameterMenu.FILTERLAYERS.TILES, ToolParameterMenu.ToggleState.On, true),
			new ToolParameterMenu.ToggleData(ToolParameterMenu.FILTERLAYERS.NATURALBACKWALL, ToolParameterMenu.ToggleState.Off, true),
			new ToolParameterMenu.ToggleData(ToolParameterMenu.FILTERLAYERS.UPROOTPLANTS, ToolParameterMenu.ToggleState.On, true)
		};
	}

	protected override void OnOverlayChanged(HashedString overlay)
	{
		if (!base.IsActive)
		{
			return;
		}
		ToolMenu.Instance.toolParameterMenu.PopulateMenu(this.currentFilters);
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		if (base.IsActiveLayer(ToolParameterMenu.FILTERLAYERS.UPROOTPLANTS))
		{
			InterfaceTool.ActiveConfig.DigAction.Uproot(cell);
		}
		InterfaceTool.ActiveConfig.DigAction.Dig(cell, distFromOrigin);
	}

	public static GameObject PlaceDig(int cell, int animationDelay = 0)
	{
		bool flag = DigTool.Instance.IsActiveLayer(ToolParameterMenu.FILTERLAYERS.TILES);
		bool flag2 = DigTool.Instance.IsActiveLayer(ToolParameterMenu.FILTERLAYERS.NATURALBACKWALL);
		bool flag3 = Grid.Solid[cell] && !Grid.Foundation[cell];
		bool flag4 = !Grid.Solid[cell] && BackwallManager.HasBackwall(cell) && !Grid.Foundation[cell];
		if (Grid.Objects[cell, 7] == null && ((flag3 && flag) || (flag4 && flag2)))
		{
			for (int i = 0; i < 45; i++)
			{
				if (Grid.Objects[cell, i] != null && Grid.Objects[cell, i].GetComponent<Constructable>() != null)
				{
					return null;
				}
			}
			GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(new Tag("DigPlacer")), null, null);
			gameObject.GetComponent<Diggable>().digTypeFlags = (flag ? 1 : 0) | (flag2 ? 2 : 0);
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
