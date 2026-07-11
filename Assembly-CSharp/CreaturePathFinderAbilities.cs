using System;
using Klei.AI;

public class CreaturePathFinderAbilities : PathFinderAbilities
{
	public CreaturePathFinderAbilities(Navigator navigator)
		: base(navigator)
	{
	}

	protected override void Refresh(Navigator navigator)
	{
		if (PathFinder.IsSubmerged(Grid.PosToCell(navigator)))
		{
			this.maxUnderwaterCost = int.MaxValue;
			return;
		}
		AttributeInstance attributeInstance = Db.Get().Attributes.MaxUnderwaterTravelCost.Lookup(navigator);
		this.maxUnderwaterCost = ((attributeInstance != null) ? ((int)attributeInstance.GetTotalValue()) : int.MaxValue);
	}

	public override bool TraversePath(ref PathFinder.PotentialPath path, int from_cell, NavType from_nav_type, int cost, int transition_id, int underwater_cost)
	{
		return underwater_cost <= this.maxUnderwaterCost;
	}

	public int maxUnderwaterCost;
}
