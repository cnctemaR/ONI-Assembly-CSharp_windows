using System;

public class IdleCellQuery : PathFinderQuery
{
	public IdleCellQuery Reset(MinionBrain brain, int max_cost)
	{
		this.brain = brain;
		this.maxCost = max_cost;
		this.targetCell = Grid.InvalidCell;
		return this;
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		SafeCellQuery.SafeFlags flags = SafeCellQuery.GetFlags(cell, this.brain, false, (SafeCellQuery.SafeFlags)0);
		if ((flags & SafeCellQuery.SafeFlags.IsClear) != (SafeCellQuery.SafeFlags)0 && (flags & SafeCellQuery.SafeFlags.IsNotLadder) != (SafeCellQuery.SafeFlags)0 && (flags & SafeCellQuery.SafeFlags.IsNotTube) != (SafeCellQuery.SafeFlags)0 && (flags & SafeCellQuery.SafeFlags.IsBreathable) != (SafeCellQuery.SafeFlags)0 && (flags & SafeCellQuery.SafeFlags.IsNotLiquid) != (SafeCellQuery.SafeFlags)0)
		{
			this.targetCell = cell;
		}
		return cost > this.maxCost;
	}

	public override int GetResultCell()
	{
		return this.targetCell;
	}

	private MinionBrain brain;

	private int targetCell;

	private int maxCost;
}
