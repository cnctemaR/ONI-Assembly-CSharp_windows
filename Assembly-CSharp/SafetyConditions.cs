using System;
using System.Collections.Generic;

public class SafetyConditions
{
	public SafetyConditions()
	{
		int num = 1;
		this.IsNotLiquid = new SafetyChecker.Condition("IsNotLiquid", num *= 2, (int cell, SafetyChecker.Context context) => !Grid.Element[cell].IsLiquid);
		this.IsNotLadder = new SafetyChecker.Condition("IsNotLadder", num *= 2, (int cell, SafetyChecker.Context context) => !context.navigator.NavGrid.NavTable.IsValid(cell, NavType.Ladder));
		this.IsNotFoundation = new SafetyChecker.Condition("IsNotFoundation", num *= 2, delegate(int cell, SafetyChecker.Context context)
		{
			int num2 = Grid.CellAbove(cell);
			return !Grid.Foundation[cell] || (Grid.IsValidCell(num2) && Grid.Foundation[num2]);
		});
		this.IsCorrectTemperature = new SafetyChecker.Condition("IsCorrectTemperature", num *= 2, (int cell, SafetyChecker.Context context) => Grid.Temperature[cell] < 303f);
		this.HasSomeOxygen = new SafetyChecker.Condition("HasSomeOxygen", num *= 2, (int cell, SafetyChecker.Context context) => context.oxygenBreather.IsBreathableElementAtCell(cell, null));
		this.IsClear = new SafetyChecker.Condition("IsClear", num *= 2, (int cell, SafetyChecker.Context context) => context.minionBrain.IsCellClear(cell));
		this.DoesNotRequireSuit = new SafetyChecker.Condition("DoesNotRequireSuit", num * 2, (int cell, SafetyChecker.Context context) => !Grid.SuitRequired[cell]);
		List<SafetyChecker.Condition> list = new List<SafetyChecker.Condition>();
		list.Add(this.DoesNotRequireSuit);
		list.Add(this.HasSomeOxygen);
		this.RecoverBreathChecker = new SafetyChecker(list.ToArray());
		List<SafetyChecker.Condition> list2 = new List<SafetyChecker.Condition>(list);
		list2.Add(this.IsNotLiquid);
		list2.Add(this.IsCorrectTemperature);
		this.SafeCellChecker = new SafetyChecker(list2.ToArray());
		this.IdleCellChecker = new SafetyChecker(new List<SafetyChecker.Condition>(list2) { this.IsClear, this.IsNotLadder }.ToArray());
	}

	public SafetyChecker.Condition IsNotLiquid;

	public SafetyChecker.Condition IsNotLadder;

	public SafetyChecker.Condition IsCorrectTemperature;

	public SafetyChecker.Condition HasSomeOxygen;

	public SafetyChecker.Condition IsClear;

	public SafetyChecker.Condition DoesNotRequireSuit;

	public SafetyChecker.Condition IsNotFoundation;

	public SafetyChecker RecoverBreathChecker;

	public SafetyChecker SafeCellChecker;

	public SafetyChecker IdleCellChecker;
}
