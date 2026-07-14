using System;
using UnityEngine.Pool;

public class MinionPathFinderAbilities : PathFinderAbilities
{
	public MinionPathFinderAbilities(Navigator navigator)
		: base(navigator)
	{
	}

	private MinionPathFinderAbilities()
		: base(null)
	{
	}

	protected override void Refresh(Navigator navigator)
	{
		MinionAssignablesProxy minionAssignablesProxy = navigator.GetComponent<MinionIdentity>().assignableProxy.Get();
		this.proxyID = minionAssignablesProxy.GetComponent<KPrefabID>().InstanceID;
		this.accessControlDefaultKey = GridRestrictionSerializer.Instance.GetTagId(minionAssignablesProxy.GetMinionModel());
		this.out_of_fuel = navigator.HasTag(GameTags.JetSuitOutOfFuel);
		this.hasSwimSkill = navigator.GetComponent<MinionResume>().HasPerk(Db.Get().SkillPerks.CanSwim);
	}

	public void SetIdleNavMaskEnabled(bool enabled)
	{
		this.idleNavMaskEnabled = enabled;
	}

	private static bool IsAccessPermitted(int proxyID, int proxyTag, int cell, int from_cell, NavType from_nav_type)
	{
		return Grid.HasPermission(cell, proxyID, proxyTag, from_cell, from_nav_type);
	}

	public override int GetSubmergedPathCostPenalty(PathFinder.PotentialPath path, NavGrid.Link link)
	{
		bool flag = path.HasAnyFlag(PathFinder.PotentialPath.Flags.HasAtmoSuit | PathFinder.PotentialPath.Flags.HasJetPack | PathFinder.PotentialPath.Flags.HasLeadSuit);
		bool flag2 = link.endNavType == NavType.Swim;
		if (!this.hasSwimSkill)
		{
			if (!flag)
			{
				return (int)(link.cost * 2);
			}
			return 0;
		}
		else
		{
			if (flag && flag2)
			{
				return (int)(link.cost * 50);
			}
			if (!flag && !flag2)
			{
				return (int)(link.cost * 2);
			}
			if (!flag && flag2 && PathFinder.IsSubmerged(link.link))
			{
				return (int)(link.cost / 2);
			}
			return 0;
		}
	}

	public override bool TraversePath(ref PathFinder.PotentialPath path, int from_cell, NavType from_nav_type, int cost, int transition_id, bool submerged)
	{
		if (!MinionPathFinderAbilities.IsAccessPermitted(this.proxyID, this.accessControlDefaultKey, path.cell, from_cell, from_nav_type))
		{
			return false;
		}
		foreach (CellOffset cellOffset in this.navigator.NavGrid.transitions[transition_id].voidOffsets)
		{
			int num = Grid.OffsetCell(from_cell, cellOffset);
			if (!MinionPathFinderAbilities.IsAccessPermitted(this.proxyID, this.accessControlDefaultKey, num, from_cell, from_nav_type))
			{
				return false;
			}
		}
		if (path.navType == NavType.Tube && from_nav_type == NavType.Floor && !Grid.HasUsableTubeEntrance(from_cell, this.prefabInstanceID))
		{
			return false;
		}
		if (!this.hasSwimSkill && (path.navType == NavType.Swim || from_nav_type == NavType.Swim))
		{
			return false;
		}
		if (path.navType == NavType.Hover && (this.out_of_fuel || !path.HasFlag(PathFinder.PotentialPath.Flags.HasJetPack)))
		{
			return false;
		}
		Grid.SuitMarker.Flags flags = (Grid.SuitMarker.Flags)0;
		PathFinder.PotentialPath.Flags flags2 = PathFinder.PotentialPath.Flags.None;
		bool flag = path.HasFlag(PathFinder.PotentialPath.Flags.PerformSuitChecks) && Grid.TryGetSuitMarkerFlags(from_cell, out flags, out flags2) && (flags & Grid.SuitMarker.Flags.Operational) > (Grid.SuitMarker.Flags)0;
		bool flag2 = SuitMarker.DoesTraversalDirectionRequireSuit(from_cell, path.cell, flags);
		bool flag3 = path.HasAnyFlag(PathFinder.PotentialPath.Flags.HasAtmoSuit | PathFinder.PotentialPath.Flags.HasJetPack | PathFinder.PotentialPath.Flags.HasOxygenMask | PathFinder.PotentialPath.Flags.HasLeadSuit);
		if (flag)
		{
			bool flag4 = path.HasFlag(flags2);
			if (flag2)
			{
				if (!flag3 && !Grid.HasSuit(from_cell, this.prefabInstanceID))
				{
					return false;
				}
			}
			else if (flag3 && (flags & Grid.SuitMarker.Flags.OnlyTraverseIfUnequipAvailable) != (Grid.SuitMarker.Flags)0 && (!flag4 || !Grid.HasEmptyLocker(from_cell, this.prefabInstanceID)))
			{
				return false;
			}
		}
		if (this.idleNavMaskEnabled && (Grid.PreventIdleTraversal[path.cell] || Grid.PreventIdleTraversal[from_cell]))
		{
			return false;
		}
		if (flag)
		{
			if (flag2)
			{
				if (!flag3)
				{
					path.SetFlags(flags2);
				}
			}
			else
			{
				path.ClearFlags(PathFinder.PotentialPath.Flags.HasAtmoSuit | PathFinder.PotentialPath.Flags.HasJetPack | PathFinder.PotentialPath.Flags.HasOxygenMask | PathFinder.PotentialPath.Flags.HasLeadSuit);
			}
		}
		return true;
	}

	public override PathFinderAbilities Clone()
	{
		MinionPathFinderAbilities minionPathFinderAbilities = MinionPathFinderAbilities.pool.Get();
		minionPathFinderAbilities.navigator = this.navigator;
		minionPathFinderAbilities.prefabInstanceID = this.prefabInstanceID;
		minionPathFinderAbilities.proxyID = this.proxyID;
		minionPathFinderAbilities.accessControlDefaultKey = this.accessControlDefaultKey;
		minionPathFinderAbilities.out_of_fuel = this.out_of_fuel;
		minionPathFinderAbilities.idleNavMaskEnabled = this.idleNavMaskEnabled;
		minionPathFinderAbilities.hasSwimSkill = this.hasSwimSkill;
		return minionPathFinderAbilities;
	}

	public override void RecycleClone()
	{
		MinionPathFinderAbilities.pool.Release(this);
	}

	private int proxyID;

	private int accessControlDefaultKey;

	private bool out_of_fuel;

	private bool idleNavMaskEnabled;

	private bool hasSwimSkill;

	private static ObjectPool<MinionPathFinderAbilities> pool = new ObjectPool<MinionPathFinderAbilities>(() => new MinionPathFinderAbilities(), null, delegate(MinionPathFinderAbilities obj)
	{
		obj.navigator = null;
		obj.prefabInstanceID = -1;
	}, null, false, 4, 8);
}
