using System;
using Klei.Actions;

namespace Klei.Input
{
	[Action("Immediate")]
	public class ImmediateDigAction : DigAction
	{
		public override void Dig(int cell, int distFromOrigin)
		{
			if (Grid.Solid[cell] && !Grid.Foundation[cell])
			{
				SimMessages.Dig(cell, -1, false);
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
