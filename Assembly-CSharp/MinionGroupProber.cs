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
		this.navTypeCount = new int[Grid.CellCount];
		PathGrid pathGrid = this.pathProber.GetPathGrid();
		pathGrid.OnCellCostChanged = (Action<int, NavType, int, int>)Delegate.Combine(pathGrid.OnCellCostChanged, new Action<int, NavType, int, int>(this.OnCellCostChanged));
	}

	private void OnCellCostChanged(int cell, NavType nav_type, int previous_cost, int new_cost)
	{
		int num = this.navTypeCount[cell];
		if (previous_cost == PathProber.InvalidCost && new_cost != PathProber.InvalidCost)
		{
			this.navTypeCount[cell] = this.navTypeCount[cell] + 1;
		}
		else if (previous_cost != PathProber.InvalidCost && new_cost == PathProber.InvalidCost)
		{
			this.navTypeCount[cell] = this.navTypeCount[cell] - 1;
		}
		if (num != this.navTypeCount[cell])
		{
			GameScenePartitioner.Instance.TriggerEvent(cell, GameScenePartitioner.Instance.navigableChanged.mask, null);
		}
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
			if (flag || this.pathProber.GetCost(num) == PathProber.InvalidCost)
			{
				Navigator component = minionIdentity.GetComponent<Navigator>();
				PathFinderAbilities currentAbilities = minionIdentity.GetComponent<Navigator>().GetCurrentAbilities();
				currentAbilities.maxUnderwaterCost = 8;
				this.pathProber.UpdateProbe(this.navGrid, num, component.CurrentNavType, currentAbilities, flag);
				flag = false;
			}
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

	private int[] navTypeCount;

	private MinionGroupProber.Island[] islands = new MinionGroupProber.Island[0];

	public struct Island
	{
	}
}
