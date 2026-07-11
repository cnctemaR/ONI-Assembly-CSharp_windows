using System;
using System.Diagnostics;

public class MinionPathFinderAbilities : PathFinderAbilities
{
	public MinionPathFinderAbilities(Navigator navigator)
		: base(navigator)
	{
		this.accessControlNavMask = new AccessControlNavMask(navigator);
		this.travelTubeNavMask = new TravelTubeNavMask(navigator);
		this.navigationFeatureMask = new NavigationFeatureMask(navigator);
		this.idleNavMask = default(IdleNavMask);
	}

	public override void Refresh()
	{
		int num = Grid.PosToCell(base.navigator);
		if (PathFinder.IsSubmerged(num))
		{
			this.maxUnderwaterCost = int.MaxValue;
		}
		else
		{
			this.maxUnderwaterCost = (int)Db.Get().Attributes.MaxUnderwaterTravelCost.Lookup(base.navigator).GetTotalValue();
		}
	}

	public void SetIdleNavMaskEnabled(bool enabled)
	{
		this.idleNavMask.enabled = enabled;
	}

	public override bool TraversePath(ref PathFinder.PotentialPath path, int from_cell, NavType from_nav_type, int cost, int transition_id, int underwater_cost)
	{
		if (!this.accessControlNavMask.IsTraversable(base.navigator, path, from_cell, cost, transition_id))
		{
			return false;
		}
		if (!this.travelTubeNavMask.IsTraversable(base.navigator, path, from_cell, from_nav_type, cost, transition_id))
		{
			return false;
		}
		if (!this.navigationFeatureMask.IsTraversable(base.navigator, path, from_cell, cost, transition_id, this))
		{
			return false;
		}
		if (!this.idleNavMask.IsTraversable(base.navigator, path, from_cell, cost, transition_id))
		{
			return false;
		}
		if (path.HasFlag(PathFinder.PotentialPath.Flags.HasSuit) || path.navType == NavType.Tube || underwater_cost <= this.maxUnderwaterCost)
		{
			this.navigationFeatureMask.ApplyTraversalToPath(base.navigator, ref path, from_cell);
			return true;
		}
		return false;
	}

	[Conditional("ENABLE_NAVIGATION_MASK_PROFILING")]
	private void BeginSample(string region_name)
	{
	}

	[Conditional("ENABLE_NAVIGATION_MASK_PROFILING")]
	private void EndSample()
	{
	}

	public int maxUnderwaterCost;

	private AccessControlNavMask accessControlNavMask;

	private TravelTubeNavMask travelTubeNavMask;

	private NavigationFeatureMask navigationFeatureMask;

	private IdleNavMask idleNavMask;
}
