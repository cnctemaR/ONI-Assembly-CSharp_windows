using System;
using System.Collections.Generic;

public class SafetyConditions
{
	public SafetyConditions()
	{
		int num = 1;
		this.IsNearby = new SafetyChecker.Condition("IsNearby", num *= 2, (int cell, int cost, SafetyChecker.Context context) => cost > 5);
		this.IsNotLedge = new SafetyChecker.Condition("IsNotLedge", num *= 2, delegate(int cell, int cost, SafetyChecker.Context context)
		{
			int num2 = Grid.CellLeft(cell);
			int num3 = Grid.CellBelow(num2);
			if (Grid.Solid[num3])
			{
				return false;
			}
			int num4 = Grid.CellRight(cell);
			int num5 = Grid.CellBelow(num4);
			return Grid.Solid[num5];
		});
		this.IsNotLiquid = new SafetyChecker.Condition("IsNotLiquid", num *= 2, (int cell, int cost, SafetyChecker.Context context) => !Grid.Element[cell].IsLiquid);
		this.IsNotLadder = new SafetyChecker.Condition("IsNotLadder", num *= 2, (int cell, int cost, SafetyChecker.Context context) => !context.navigator.NavGrid.NavTable.IsValid(cell, NavType.Ladder));
		this.IsNotFoundation = new SafetyChecker.Condition("IsNotFoundation", num *= 2, delegate(int cell, int cost, SafetyChecker.Context context)
		{
			int num6 = Grid.CellAbove(cell);
			return !Grid.Foundation[cell] || (Grid.IsValidCell(num6) && Grid.Foundation[num6]);
		});
		this.IsNotDoor = new SafetyChecker.Condition("IsNotDoor", num *= 2, delegate(int cell, int cost, SafetyChecker.Context context)
		{
			int num7 = Grid.CellAbove(cell);
			return !Grid.HasDoor[cell] && Grid.IsValidCell(num7) && !Grid.HasDoor[num7];
		});
		this.IsCorrectTemperature = new SafetyChecker.Condition("IsCorrectTemperature", num *= 2, (int cell, int cost, SafetyChecker.Context context) => Grid.Temperature[cell] > 285.15f && Grid.Temperature[cell] < 303.15f);
		this.IsWarming = new SafetyChecker.Condition("IsWarming", num *= 2, (int cell, int cost, SafetyChecker.Context context) => true);
		this.IsCooling = new SafetyChecker.Condition("IsCooling", num *= 2, (int cell, int cost, SafetyChecker.Context context) => false);
		this.HasSomeOxygen = new SafetyChecker.Condition("HasSomeOxygen", num *= 2, (int cell, int cost, SafetyChecker.Context context) => context.oxygenBreather.IsBreathableElementAtCell(cell, null));
		this.IsClear = new SafetyChecker.Condition("IsClear", num *= 2, (int cell, int cost, SafetyChecker.Context context) => context.minionBrain.IsCellClear(cell));
		this.DoesNotRequireSuit = new SafetyChecker.Condition("DoesNotRequireSuit", num * 2, (int cell, int cost, SafetyChecker.Context context) => !Grid.SuitRequired[cell]);
		this.WarmUpChecker = new SafetyChecker(new List<SafetyChecker.Condition> { this.IsWarming }.ToArray());
		this.CoolDownChecker = new SafetyChecker(new List<SafetyChecker.Condition> { this.IsCooling }.ToArray());
		List<SafetyChecker.Condition> list = new List<SafetyChecker.Condition>();
		list.Add(this.DoesNotRequireSuit);
		list.Add(this.HasSomeOxygen);
		list.Add(this.IsNotDoor);
		this.RecoverBreathChecker = new SafetyChecker(list.ToArray());
		List<SafetyChecker.Condition> list2 = new List<SafetyChecker.Condition>(list);
		list2.Add(this.IsNotLiquid);
		list2.Add(this.IsCorrectTemperature);
		this.SafeCellChecker = new SafetyChecker(list2.ToArray());
		this.IdleCellChecker = new SafetyChecker(new List<SafetyChecker.Condition>(list2) { this.IsClear, this.IsNotLadder }.ToArray());
		this.VomitCellChecker = new SafetyChecker(new List<SafetyChecker.Condition> { this.IsNotLiquid, this.IsNotLedge, this.IsNearby }.ToArray());
	}

	public SafetyChecker.Condition IsNotLiquid;

	public SafetyChecker.Condition IsNotLadder;

	public SafetyChecker.Condition IsCorrectTemperature;

	public SafetyChecker.Condition IsWarming;

	public SafetyChecker.Condition IsCooling;

	public SafetyChecker.Condition HasSomeOxygen;

	public SafetyChecker.Condition IsClear;

	public SafetyChecker.Condition DoesNotRequireSuit;

	public SafetyChecker.Condition IsNotFoundation;

	public SafetyChecker.Condition IsNotDoor;

	public SafetyChecker.Condition IsNotLedge;

	public SafetyChecker.Condition IsNearby;

	public SafetyChecker WarmUpChecker;

	public SafetyChecker CoolDownChecker;

	public SafetyChecker RecoverBreathChecker;

	public SafetyChecker VomitCellChecker;

	public SafetyChecker SafeCellChecker;

	public SafetyChecker IdleCellChecker;
}
