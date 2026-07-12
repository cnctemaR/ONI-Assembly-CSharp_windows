using System;
using UnityEngine;

public class AttackTool : DragTool
{
	protected override void OnDragComplete(Vector3 downPos, Vector3 upPos)
	{
		Vector2 regularizedPos = base.GetRegularizedPos(Vector2.Min(downPos, upPos), true);
		Vector2 regularizedPos2 = base.GetRegularizedPos(Vector2.Max(downPos, upPos), false);
		AttackTool.MarkForAttack(regularizedPos, regularizedPos2, true);
	}

	public static void MarkForAttack(Vector2 min, Vector2 max, bool mark)
	{
		foreach (FactionAlignment factionAlignment in Components.FactionAlignments.Items)
		{
			if (!factionAlignment.IsNullOrDestroyed())
			{
				Vector2 vector = Grid.PosToXY(factionAlignment.transform.GetPosition());
				if (vector.x >= min.x && vector.x < max.x && vector.y >= min.y && vector.y < max.y)
				{
					if (mark)
					{
						if (FactionManager.Instance.GetDisposition(FactionManager.FactionID.Duplicant, factionAlignment.Alignment) != FactionManager.Disposition.Assist)
						{
							factionAlignment.SetPlayerTargeted(true);
							Prioritizable component = factionAlignment.GetComponent<Prioritizable>();
							if (component != null)
							{
								component.SetMasterPriority(ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority());
							}
						}
					}
					else
					{
						factionAlignment.gameObject.Trigger(2127324410, null);
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
}
