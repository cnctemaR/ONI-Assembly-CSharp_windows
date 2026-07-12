using System;
using STRINGS;
using UnityEngine;

public class MopTool : DragTool
{
	public static void DestroyInstance()
	{
		MopTool.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.Placer = Assets.GetPrefab(new Tag("MopPlacer"));
		this.interceptNumberKeysForPriority = true;
		MopTool.Instance = this;
	}

	public void Activate()
	{
		PlayerController.Instance.ActivateTool(this);
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		if (Grid.IsValidCell(cell))
		{
			if (DebugHandler.InstantBuildMode)
			{
				if (Grid.IsValidCell(cell))
				{
					Moppable.MopCell(cell, 1000000f, null);
					return;
				}
			}
			else
			{
				GameObject gameObject = Grid.Objects[cell, 8];
				if (!Grid.Solid[cell] && gameObject == null && Grid.Element[cell].IsLiquid)
				{
					bool flag = Grid.IsValidCell(Grid.CellBelow(cell)) && Grid.Solid[Grid.CellBelow(cell)];
					bool flag2 = Grid.Mass[cell] <= MopTool.maxMopAmt;
					if (flag && flag2)
					{
						gameObject = Util.KInstantiate(this.Placer, null, null);
						Grid.Objects[cell, 8] = gameObject;
						Vector3 vector = Grid.CellToPosCBC(cell, this.visualizerLayer);
						float num = -0.15f;
						vector.z += num;
						gameObject.transform.SetPosition(vector);
						gameObject.SetActive(true);
					}
					else
					{
						string text = UI.TOOLS.MOP.TOO_MUCH_LIQUID;
						if (!flag)
						{
							text = UI.TOOLS.MOP.NOT_ON_FLOOR;
						}
						PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, text, null, Grid.CellToPosCBC(cell, this.visualizerLayer), 1.5f, false, false);
					}
				}
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

	private GameObject Placer;

	public static MopTool Instance;

	private SimHashes Element;

	public static float maxMopAmt = 150f;
}
