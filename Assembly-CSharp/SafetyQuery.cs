using System;

public class SafetyQuery : PathFinderQuery
{
	public SafetyQuery(SafetyChecker checker, KMonoBehaviour cmp)
	{
		this.checker = checker;
		this.cmp = cmp;
	}

	public void Reset()
	{
		this.targetCell = PathFinder.InvalidCell;
		this.targetCost = int.MaxValue;
		this.targetConditions = 0;
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		int safetyConditions = this.checker.GetSafetyConditions(cell, this.cmp);
		if (safetyConditions != 0 && (safetyConditions > this.targetConditions || (safetyConditions == this.targetConditions && cost < this.targetCost)))
		{
			this.targetCell = cell;
			this.targetConditions = safetyConditions;
			this.targetCost = cost;
		}
		return false;
	}

	public override int GetResultCell()
	{
		return this.targetCell;
	}

	private int targetCell;

	private int targetCost;

	private int targetConditions;

	private SafetyChecker checker;

	private KMonoBehaviour cmp;
}
