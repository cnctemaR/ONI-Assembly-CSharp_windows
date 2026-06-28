using System;

public class MinionGroupProber : KMonoBehaviour
{
	public static MinionGroupProber Get()
	{
		return MinionGroupProber.Instance;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		MinionGroupProber.Instance = this;
		this.navGrid = Pathfinding.Instance.GetNavGrid("MinionNavGrid");
		this.pathProber.SetValidNavTypes(this.navGrid.ValidNavTypes, 0);
	}

	public bool IsReachable(Workable workable)
	{
		int num = Grid.PosToCell(workable);
		return this.IsReachable(num, workable.GetOffsets());
	}

	public bool IsReachable(int cell, CellOffset[] offsets)
	{
		if (Grid.IsValidCell(cell))
		{
			int num = offsets.Length;
			for (int i = 0; i < num; i++)
			{
				int num2 = Grid.OffsetCell(cell, offsets[i]);
				if (this.pathProber.GetCost(num2) != PathProber.InvalidCost)
				{
					return true;
				}
			}
		}
		return false;
	}

	public PathProber GetPathProber()
	{
		return this.pathProber;
	}

	private void Update()
	{
		bool flag = true;
		foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities)
		{
			int num = Grid.PosToCell(minionIdentity);
			if (!flag)
			{
				if (this.pathProber.GetCost(num) != PathProber.InvalidCost)
				{
					continue;
				}
			}
			Navigator component = minionIdentity.GetComponent<Navigator>();
			PathFinderAbilities currentAbilities = minionIdentity.GetComponent<Navigator>().GetCurrentAbilities();
			currentAbilities.maxUnderwaterCost = 8;
			currentAbilities.ignoreNavigationMasks = true;
			this.pathProber.UpdateProbe(this.navGrid, num, component.CurrentNavType, currentAbilities, PathFinder.PotentialPath.Flags.UnlimitedSubmergedTravel, flag);
			flag = false;
		}
		if (this.pathProber.IslandCount > this.islands.Length)
		{
			this.islands = new MinionGroupProber.Island[this.pathProber.IslandCount];
		}
	}

	private static MinionGroupProber Instance;

	[MyCmpReq]
	private PathProber pathProber;

	private NavGrid navGrid;

	private MinionGroupProber.Island[] islands = new MinionGroupProber.Island[0];

	public struct Island
	{
	}
}
