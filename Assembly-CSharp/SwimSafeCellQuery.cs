using System;
using TUNING;
using UnityEngine;

public class SwimSafeCellQuery : PathFinderQuery
{
	public SwimSafeCellQuery Reset(MinionBrain brain, bool avoid_light, SafeCellQuery.SafeFlags ignoredFlags, float currentBreathValue)
	{
		this.brain = brain;
		this.targetCell = PathFinder.InvalidCell;
		this.targetCost = int.MaxValue;
		this.targetCellFlags = (SafeCellQuery.SafeFlags)0;
		this.avoid_light = avoid_light;
		this.targetRouteSubmergedCost = int.MaxValue;
		this.ignoredFlags = ignoredFlags | SafeCellQuery.SafeFlags.IsNotLiquid | SafeCellQuery.SafeFlags.IsNotLiquidOnMyFace | SafeCellQuery.SafeFlags.IsNotSwimming;
		float breath_RATE = DUPLICANTSTATS.STANDARD.Breath.BREATH_RATE;
		float num = ((breath_RATE > 0f) ? (currentBreathValue / breath_RATE) : 0f);
		float num2 = Mathf.Max(0f, num - 15f);
		this.maxSubmergedCost = num2 / 0.1f;
		if (this.routeSubmergedCost == null || this.routeSubmergedCost.Length != Grid.CellCount)
		{
			this.routeSubmergedCost = new int[Grid.CellCount];
		}
		int num3 = Grid.PosToCell(brain);
		if (Grid.IsValidCell(num3))
		{
			this.routeSubmergedCost[num3] = 0;
		}
		return this;
	}

	private static bool IsCellSubmerged(int cell)
	{
		if (!Grid.Element[cell].IsLiquid)
		{
			return false;
		}
		int num = Grid.CellAbove(cell);
		return Grid.IsValidCell(num) && Grid.Element[num].IsLiquid;
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		SafeCellQuery.SafeFlags flags = SafeCellQuery.GetFlags(cell, this.brain, this.avoid_light, this.ignoredFlags);
		int num = 0;
		if (Grid.IsValidCell(parent_cell))
		{
			num = this.routeSubmergedCost[parent_cell];
		}
		int num3;
		if (SwimSafeCellQuery.IsCellSubmerged(cell))
		{
			int num2 = 9;
			num3 = num + num2;
		}
		else
		{
			num3 = 0;
		}
		this.routeSubmergedCost[cell] = num3;
		if ((float)num3 > this.maxSubmergedCost)
		{
			return false;
		}
		bool flag = flags > this.targetCellFlags;
		bool flag2 = flags == this.targetCellFlags;
		bool flag3 = flag2 && num3 < this.targetRouteSubmergedCost;
		bool flag4 = flag2 && num3 == this.targetRouteSubmergedCost && cost < this.targetCost;
		if (flag || flag3 || flag4)
		{
			this.targetCellFlags = flags;
			this.targetRouteSubmergedCost = num3;
			this.targetCost = cost;
			this.targetCell = cell;
		}
		return (SafeCellQuery.SafeFlags.AllSafeFlags & ~(flags | this.ignoredFlags)) == (SafeCellQuery.SafeFlags)0;
	}

	public override int GetResultCell()
	{
		return this.targetCell;
	}

	private const float SECONDS_PER_COST = 0.1f;

	private const float BREATH_SAFETY_MARGIN_SECONDS = 15f;

	private MinionBrain brain;

	private int targetCell;

	private int targetCost;

	public SafeCellQuery.SafeFlags targetCellFlags;

	private bool avoid_light;

	private SafeCellQuery.SafeFlags ignoredFlags;

	private float maxSubmergedCost;

	private int[] routeSubmergedCost;

	private int targetRouteSubmergedCost;
}
