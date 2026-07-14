using System;

public class IdleCellQuery : PathFinderQuery
{
	public IdleCellQuery Reset(MinionBrain brain, int max_cost, bool can_swim = false)
	{
		this.brain = brain;
		this.maxCost = max_cost;
		this.targetCell = Grid.InvalidCell;
		this.canSwim = can_swim;
		return this;
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		SafeCellQuery.SafeFlags flags = SafeCellQuery.GetFlags(cell, this.brain, false, (SafeCellQuery.SafeFlags)0);
		if ((flags & SafeCellQuery.SafeFlags.IsClear) != (SafeCellQuery.SafeFlags)0 && (flags & SafeCellQuery.SafeFlags.IsNotLadder) != (SafeCellQuery.SafeFlags)0 && (flags & SafeCellQuery.SafeFlags.IsNotTube) != (SafeCellQuery.SafeFlags)0 && (flags & SafeCellQuery.SafeFlags.IsBreathable) > (SafeCellQuery.SafeFlags)0)
		{
			if ((flags & SafeCellQuery.SafeFlags.IsNotLiquid) != (SafeCellQuery.SafeFlags)0)
			{
				this.targetCell = cell;
			}
			else if (this.canSwim && (flags & SafeCellQuery.SafeFlags.IsNotLiquidOnMyFace) != (SafeCellQuery.SafeFlags)0)
			{
				this.targetCell = cell;
			}
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

	private bool canSwim;
}
