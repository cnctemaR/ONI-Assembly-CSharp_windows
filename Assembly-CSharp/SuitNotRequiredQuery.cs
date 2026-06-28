using System;

public class SuitNotRequiredQuery : PathFinderQuery
{
	public SuitNotRequiredQuery Reset(MinionBrain brain)
	{
		return this;
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		return !MinionBrain.RequiresSuitAtCell(cell);
	}

	private MinionBrain brain;
}
