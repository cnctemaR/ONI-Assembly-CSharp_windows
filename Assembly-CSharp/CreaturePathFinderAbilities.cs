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
			this.canTraverseSubmered = true;
			return;
		}
		AttributeInstance attributeInstance = Db.Get().Attributes.MaxUnderwaterTravelCost.Lookup(navigator);
		this.canTraverseSubmered = attributeInstance == null;
	}

	public override bool TraversePath(ref PathFinder.PotentialPath path, int from_cell, NavType from_nav_type, int cost, int transition_id, bool submerged)
	{
		return !submerged || this.canTraverseSubmered;
	}

	public bool canTraverseSubmered;
}
