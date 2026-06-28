using System;
using UnityEngine;

public class MopTool : DragTool
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
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
					Moppable.MopCell(cell, null);
				}
			}
			else
			{
				GameObject gameObject = Grid.Objects[cell, 8];
				if (!Grid.Solid[cell] && gameObject == null && Grid.Solid[Grid.CellBelow(cell)])
				{
					gameObject = Util.KInstantiate(this.Placer, SceneOrganizer.Instance.GetFolder(Folder.Placers), null);
					Grid.Objects[cell, 8] = gameObject;
					Vector3 vector = Grid.CellToPosCBC(cell, this.visualizerLayer);
					float depthBias = InterfaceTool.DepthBias;
					vector.z += depthBias;
					gameObject.transform.position = vector;
				}
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

	public static MopTool Instance;

	private SimHashes Element;
}
