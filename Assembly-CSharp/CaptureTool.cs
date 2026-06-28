using System;
using STRINGS;
using UnityEngine;

public class CaptureTool : DragTool
{
	protected override void OnDragComplete(Vector3 downPos, Vector3 upPos)
	{
		Vector2 regularizedPos = base.GetRegularizedPos(Vector2.Min(downPos, upPos), true);
		Vector2 regularizedPos2 = base.GetRegularizedPos(Vector2.Max(downPos, upPos), false);
		CaptureTool.MarkForCapture(regularizedPos, regularizedPos2, true);
	}

	public static void MarkForCapture(Vector2 min, Vector2 max, bool mark)
	{
		foreach (Capturable capturable in Components.Capturables)
		{
			Vector2 vector = Grid.PosToXY(capturable.transform.GetPosition());
			if (vector.x >= min.x && vector.x < max.x && vector.y >= min.y && vector.y < max.y)
			{
				capturable.MarkForCapture(mark);
			}
		}
		foreach (NotCapturable notCapturable in Components.NotCapturables)
		{
			Vector2 vector2 = Grid.PosToXY(notCapturable.transform.GetPosition());
			if (vector2.x >= min.x && vector2.x < max.x && vector2.y >= min.y && vector2.y < max.y)
			{
				PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, UI.TOOLS.CAPTURE.NOT_CAPTURABLE, null, notCapturable.transform.GetPosition(), 1.5f, false, false);
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
