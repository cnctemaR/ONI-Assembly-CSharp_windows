using System;
using UnityEngine;

public class AttackTool : DragTool
{
	protected override void OnDragComplete(Vector3 downPos, Vector3 upPos)
	{
		Vector2 regularizedPos = base.GetRegularizedPos(Vector2.Min(downPos, upPos), true);
		Vector2 regularizedPos2 = base.GetRegularizedPos(Vector2.Max(downPos, upPos), false);
		foreach (FactionAlignment factionAlignment in Components.FactionAlignments)
		{
			Vector2 vector = Grid.PosToXY(factionAlignment.transform.GetPosition());
			if (vector.x >= regularizedPos.x && vector.x < regularizedPos2.x && vector.y >= regularizedPos.y && vector.y < regularizedPos2.y && FactionManager.Instance.GetDisposition(FactionManager.FactionID.Duplicant, factionAlignment.Alignment) != FactionManager.Disposition.Assist)
			{
				factionAlignment.SetPlayerTargeted(true);
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

	public GameObject Placer;

	public static AttackTool Instance;
}
