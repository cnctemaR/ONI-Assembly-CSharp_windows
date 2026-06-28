using System;
using UnityEngine;

public class DigTool : DragTool
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		DigTool.Instance = this;
	}

	public override void Update()
	{
		this.cell_new = Grid.PosToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		if (!Grid.IsValidCell(this.cell_new))
		{
			return;
		}
		if (!HoverTextScreen.Instance.IsVisible)
		{
			this.hoverScreenUpdate.Prime();
		}
		else if (this.hoverScreenUpdate.tick() || this.cell_old != this.cell_new)
		{
			if (this.hoverText == null)
			{
				this.hoverText = base.gameObject.GetComponent<HoverTextConfiguration>();
			}
			this.hoverText.UpdateHoverElements(null);
		}
		this.cell_old = this.cell_new;
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		if (!Grid.Solid[cell])
		{
			foreach (Uprootable uprootable in Components.Uprootables)
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
			if (Grid.IsValidCell(cell))
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
					component.SetMasterPriority(ToolMenuPriorityScreen.Instance.GetScreenPriority());
				}
			}
		}
	}

	public static GameObject PlaceDig(int cell, int animationDelay = 0)
	{
		if (Grid.Solid[cell] && !Grid.Foundation[cell] && Grid.Objects[cell, 7] == null)
		{
			for (int i = 0; i < 23; i++)
			{
				if (Grid.Objects[cell, i] != null && Grid.Objects[cell, i].GetComponent<Constructable>() != null)
				{
					return null;
				}
			}
			GameObject gameObject = Util.KInstantiate(DigTool.Instance.Placer, SceneOrganizer.Instance.GetFolder(Folder.Placers), null);
			Grid.Objects[cell, 7] = gameObject;
			Vector3 vector = Grid.CellToPosCBC(cell, DigTool.Instance.visualizerLayer);
			float depthBias = InterfaceTool.DepthBias;
			vector.z += depthBias;
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
		ToolMenuPriorityScreen.Instance.Show(true);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		ToolMenuPriorityScreen.Instance.Show(false);
	}

	public GameObject Placer;

	public static DigTool Instance;

	protected int cell_new;

	protected int cell_old;
}
