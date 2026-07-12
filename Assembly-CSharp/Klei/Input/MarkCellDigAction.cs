using System;
using Klei.Actions;
using UnityEngine;

namespace Klei.Input
{
	[Action("Mark Cell")]
	public class MarkCellDigAction : DigAction
	{
		public override void Dig(int cell, int distFromOrigin)
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

		protected override void EntityDig(IDigActionEntity digActionEntity)
		{
			if (digActionEntity == null)
			{
				return;
			}
			digActionEntity.MarkForDig(true);
		}
	}
}
