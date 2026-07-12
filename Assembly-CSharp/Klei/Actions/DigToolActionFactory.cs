using System;
using Klei.Input;

namespace Klei.Actions
{
	public class DigToolActionFactory : ActionFactory<DigToolActionFactory, DigAction, DigToolActionFactory.Actions>
	{
		protected override DigAction CreateAction(DigToolActionFactory.Actions action)
		{
			if (action == DigToolActionFactory.Actions.Immediate)
			{
				return new ImmediateDigAction();
			}
			if (action == DigToolActionFactory.Actions.ClearCell)
			{
				return new ClearCellDigAction();
			}
			if (action == DigToolActionFactory.Actions.MarkCell)
			{
				return new MarkCellDigAction();
			}
			throw new InvalidOperationException("Can not create DigAction 'Count'. Please provide a valid action.");
		}

		public enum Actions
		{
			MarkCell = 145163119,
			Immediate = -1044758767,
			ClearCell = -1011242513,
			Count = -1427607121
		}
	}
}
