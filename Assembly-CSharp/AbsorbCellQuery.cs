using System;
using TUNING;
using UnityEngine;

public class AbsorbCellQuery : PathFinderQuery
{
	public AbsorbCellQuery()
	{
		this.checker = Game.Instance.safetyConditions.AbsorbCellCellChecker;
	}

	public AbsorbCellQuery Reset(MinionBrain brain, bool criticalMode, float currentOxygenTankMass)
	{
		this.brain = brain;
		this.targetCell = PathFinder.InvalidCell;
		this.targetCost = int.MaxValue;
		this.targetOxygenScore = float.MinValue;
		this.targetCellSafetyFlags = (AbsorbCellQuery.AbsorbOxygenSafeCellFlags)0;
		this.targetBreathableMassAvailable = 0f;
		this.criticalMode = criticalMode;
		this.bionicOxygenRemaining = currentOxygenTankMass;
		this.context = new SafetyChecker.Context(brain);
		ScaldingMonitor.Instance instance = ((brain == null) ? null : brain.GetSMI<ScaldingMonitor.Instance>());
		this.scaldingTreshold = ((instance == null) ? (-1f) : instance.GetScaldingThreshold());
		return this;
	}

	public static AbsorbCellQuery.AbsorbOxygenSafeCellFlags GetAbsorbOxygenFlags(int cell, MinionBrain brain, float scaldingTreshold, out float totalBreathableMassAroundCell, out float breathableCellRatioInSample)
	{
		totalBreathableMassAroundCell = 0f;
		breathableCellRatioInSample = 0f;
		int num = Grid.CellAbove(cell);
		if (!Grid.IsValidCell(num))
		{
			return (AbsorbCellQuery.AbsorbOxygenSafeCellFlags)0;
		}
		if (Grid.Solid[cell] || Grid.Solid[num])
		{
			return (AbsorbCellQuery.AbsorbOxygenSafeCellFlags)0;
		}
		if (Grid.IsTileUnderConstruction[cell] || Grid.IsTileUnderConstruction[num])
		{
			return (AbsorbCellQuery.AbsorbOxygenSafeCellFlags)0;
		}
		bool flag = true;
		bool flag2 = !Grid.Element[cell].IsLiquid;
		bool flag3 = !Grid.Element[num].IsLiquid;
		bool flag4 = scaldingTreshold < 0f || Grid.Temperature[cell] < scaldingTreshold;
		bool flag5 = Grid.Radiation[cell] < 250f;
		bool flag6 = false;
		if (brain.OxygenBreather != null)
		{
			for (int i = 0; i < GasBreatherFromWorldProvider.DEFAULT_BREATHABLE_OFFSETS.Length; i++)
			{
				int num2 = Grid.OffsetCell(cell, GasBreatherFromWorldProvider.DEFAULT_BREATHABLE_OFFSETS[i]);
				if (Grid.IsValidCell(num2) && Grid.AreCellsInSameWorld(cell, num2) && Grid.Element[num2].HasTag(GameTags.Breathable))
				{
					breathableCellRatioInSample += 1f / (float)GasBreatherFromWorldProvider.DEFAULT_BREATHABLE_OFFSETS.Length;
				}
			}
			flag6 = GasBreatherFromWorldProvider.GetBestBreathableCellAroundSpecificCell(cell, GasBreatherFromWorldProvider.DEFAULT_BREATHABLE_OFFSETS, brain.OxygenBreather, out totalBreathableMassAroundCell).IsBreathable;
		}
		bool flag7 = !brain.Navigator.NavGrid.NavTable.IsValid(cell, NavType.Tube);
		AbsorbCellQuery.AbsorbOxygenSafeCellFlags absorbOxygenSafeCellFlags = (AbsorbCellQuery.AbsorbOxygenSafeCellFlags)0;
		if (flag4)
		{
			absorbOxygenSafeCellFlags |= AbsorbCellQuery.AbsorbOxygenSafeCellFlags.IsNotScaldingTemperatures;
		}
		if (flag5)
		{
			absorbOxygenSafeCellFlags |= AbsorbCellQuery.AbsorbOxygenSafeCellFlags.IsNotRadiated;
		}
		if (flag6)
		{
			absorbOxygenSafeCellFlags |= AbsorbCellQuery.AbsorbOxygenSafeCellFlags.IsBreathable;
		}
		if (flag)
		{
			absorbOxygenSafeCellFlags |= AbsorbCellQuery.AbsorbOxygenSafeCellFlags.IsClear;
		}
		if (flag7)
		{
			absorbOxygenSafeCellFlags |= AbsorbCellQuery.AbsorbOxygenSafeCellFlags.IsNotTube;
		}
		if (flag2)
		{
			absorbOxygenSafeCellFlags |= AbsorbCellQuery.AbsorbOxygenSafeCellFlags.IsNotLiquid;
		}
		if (flag3)
		{
			absorbOxygenSafeCellFlags |= AbsorbCellQuery.AbsorbOxygenSafeCellFlags.IsNotLiquidOnMyFace;
		}
		return absorbOxygenSafeCellFlags;
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		float num = 2.5f * (float)GasBreatherFromWorldProvider.DEFAULT_BREATHABLE_OFFSETS.Length;
		float num2 = (float)(54 / GasBreatherFromWorldProvider.DEFAULT_BREATHABLE_OFFSETS.Length);
		float num3 = (float)cost;
		bool flag;
		this.checker.GetSafetyConditions(cell, cost, this.context, out flag);
		if (flag)
		{
			float num4 = 0.03f;
			float num5 = num3 / 10f;
			float num6 = num4 * num5;
			float num7 = 0f;
			float num8 = 0f;
			AbsorbCellQuery.AbsorbOxygenSafeCellFlags absorbOxygenFlags = AbsorbCellQuery.GetAbsorbOxygenFlags(cell, this.brain, this.scaldingTreshold, out num7, out num8);
			num7 = Mathf.Clamp(num7, 0f, num2);
			float num9 = (float)absorbOxygenFlags;
			float num10 = 10f * num8;
			float num11 = num7 * num10 - num6;
			bool flag2 = false;
			if (this.targetCell == Grid.InvalidCell)
			{
				flag2 = true;
			}
			bool flag3 = this.targetBreathableMassAvailable > 0f;
			bool flag4 = num3 < (float)this.targetCost;
			bool flag5 = this.targetOxygenScore >= num;
			bool flag6 = num9 >= (float)this.targetCellSafetyFlags || !flag3;
			float num12 = this.targetOxygenScore;
			if (this.criticalMode)
			{
				num12 = Mathf.Min(num, num12);
			}
			if (num11 >= num12 && flag6)
			{
				if (this.criticalMode)
				{
					if (flag4 || !flag5)
					{
						flag2 = true;
					}
				}
				else
				{
					flag2 = true;
				}
			}
			flag2 = flag2 && num7 > DUPLICANTSTATS.BIONICS.BaseStats.NO_OXYGEN_THRESHOLD;
			if (flag2)
			{
				this.targetBreathableMassAvailable = num7;
				this.targetCellSafetyFlags = absorbOxygenFlags;
				this.targetCost = cost;
				this.targetCell = cell;
				this.targetOxygenScore = num11;
			}
		}
		return false;
	}

	public override int GetResultCell()
	{
		return this.targetCell;
	}

	private MinionBrain brain;

	private float scaldingTreshold = -1f;

	private int targetCell;

	private int targetCost;

	private float targetOxygenScore;

	private bool criticalMode;

	private float bionicOxygenRemaining;

	private float targetBreathableMassAvailable;

	public AbsorbCellQuery.AbsorbOxygenSafeCellFlags targetCellSafetyFlags;

	public float targetCellBreathabilityScore;

	private SafetyChecker checker;

	private SafetyChecker.Context context;

	public enum AbsorbOxygenSafeCellFlags
	{
		IsNotTube = 1,
		IsNotRadiated,
		IsBreathable = 4,
		IsNotScaldingTemperatures = 8,
		IsClear = 16,
		IsNotLiquidOnMyFace = 32,
		IsNotLiquid = 64
	}
}
