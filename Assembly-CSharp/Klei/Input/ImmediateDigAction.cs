using System;
using Klei.Actions;

namespace Klei.Input
{
	[Action("Immediate")]
	public class ImmediateDigAction : DigAction
	{
		public override void Dig(int cell, int distFromOrigin)
		{
			if (DigTool.Instance.IsActiveLayer(ToolParameterMenu.FILTERLAYERS.TILES) && Grid.Solid[cell] && !Grid.Foundation[cell])
			{
				SimMessages.Dig(cell, -1, false, false);
				return;
			}
			if (DigTool.Instance.IsActiveLayer(ToolParameterMenu.FILTERLAYERS.NATURALBACKWALL) && BackwallManager.HasBackwall(cell))
			{
				SimMessages.Dig(cell, -1, false, true);
			}
		}

		protected override void EntityDig(IDigActionEntity digActionEntity)
		{
			if (digActionEntity == null)
			{
				return;
			}
			digActionEntity.Dig();
		}
	}
}
