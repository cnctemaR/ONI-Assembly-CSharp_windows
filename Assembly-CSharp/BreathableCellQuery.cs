using System;

public class BreathableCellQuery : PathFinderQuery
{
	public BreathableCellQuery Reset(Brain brain)
	{
		this.breather = brain.GetComponent<OxygenBreather>();
		return this;
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		return this.breather.IsBreathableElementAtCell(cell, null);
	}

	private OxygenBreather breather;
}
