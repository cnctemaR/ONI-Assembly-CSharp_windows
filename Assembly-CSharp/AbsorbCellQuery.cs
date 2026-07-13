using System;
using TUNING;
using UnityEngine;

public class AbsorbCellQuery : PathFinderQuery
{
	public AbsorbCellQuery()
	{
		this.checker = Game.Instance.safetyConditions.AbsorbCellCellChecker;
	}

	public AbsorbCellQuery Reset(MinionBrain brain, bool criticalMode, float currentOxygenTankMass, float breathPercentage, int allowCellEvenIfReserved, bool isRecoveringFromSuffocation)
	{
		this.brain = brain;
		this.targetCell = PathFinder.InvalidCell;
		this.targetCost = int.MaxValue;
		this.targetOxygenScore = float.MinValue;
		this.targetCellSafetyFlags = (AbsorbCellQuery.AbsorbOxygenSafeCellFlags)0;
		this.targetBreathableMassAvailable = 0f;
		this.criticalMode = criticalMode;
		this.bionicOxygenRemaining = currentOxygenTankMass;
		this.breathPercentage = breathPercentage;
		this.allowCellEvenIfReserved = allowCellEvenIfReserved;
		this.context = new SafetyChecker.Context(brain);
		ScaldingMonitor.Instance instance = ((brain == null) ? null : brain.GetSMI<ScaldingMonitor.Instance>());
		this.scaldingTreshold = ((instance == null) ? (-1f) : instance.GetScaldingThreshold());
		this.isRecoveringFromSuffocation = isRecoveringFromSuffocation;
		return this;
	}

	public static AbsorbCellQuery.AbsorbOxygenSafeCellFlags GetAbsorbOxygenFlags(int cell, MinionBrain brain, float scaldingTreshold, out float totalBreathableMassAroundCell, out float breathableCellRatioInSample, int allowCellEvenIfReserved)
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
		bool flag = cell == allowCellEvenIfReserved || brain.IsCellClear(cell);
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
		float num = 0.1f * (float)GasBreatherFromWorldProvider.DEFAULT_BREATHABLE_OFFSETS.Length;
		float num2 = 2.5f * (float)GasBreatherFromWorldProvider.DEFAULT_BREATHABLE_OFFSETS.Length;
		float num3 = (float)(54 / GasBreatherFromWorldProvider.DEFAULT_BREATHABLE_OFFSETS.Length);
		float num4 = num3 / 2.8f;
		float num5 = (float)cost;
		bool flag;
		this.checker.GetSafetyConditions(cell, cost, this.context, out flag);
		if (flag)
		{
			float num6 = 0.03f;
			float num7 = num5 / 10f;
			float num8 = num6 * num7;
			float num9 = 0f;
			float num10 = 0f;
			AbsorbCellQuery.AbsorbOxygenSafeCellFlags absorbOxygenFlags = AbsorbCellQuery.GetAbsorbOxygenFlags(cell, this.brain, this.scaldingTreshold, out num9, out num10, this.allowCellEvenIfReserved);
			num9 = Mathf.Clamp(num9, 0f, num3);
			if (this.criticalMode)
			{
				if (!this.isRecoveringFromSuffocation && this.breathPercentage > DUPLICANTSTATS.BIONICS.Breath.SUFFOCATE_AMOUNT && num9 < num)
				{
					num9 = 0f;
				}
			}
			else if (num9 < num4)
			{
				num9 = 0f;
			}
			float num11 = (float)absorbOxygenFlags;
			float num12 = 10f * num10;
			float num13 = num9 * num12 - num8;
			bool flag2 = false;
			if (this.targetCell == Grid.InvalidCell)
			{
				flag2 = true;
			}
			bool flag3 = this.targetBreathableMassAvailable > 0f;
			bool flag4 = num5 < (float)this.targetCost;
			bool flag5 = this.targetOxygenScore >= num2;
			bool flag6 = num11 >= (float)this.targetCellSafetyFlags || !flag3;
			float num14 = this.targetOxygenScore;
			if (this.criticalMode)
			{
				num14 = Mathf.Min(num2, num14);
			}
			if (num13 >= num14 && flag6)
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
			flag2 = flag2 && num9 > DUPLICANTSTATS.BIONICS.BaseStats.NO_OXYGEN_THRESHOLD;
			if (flag2)
			{
				this.targetBreathableMassAvailable = num9;
				this.targetCellSafetyFlags = absorbOxygenFlags;
				this.targetCost = cost;
				this.targetCell = cell;
				this.targetOxygenScore = num13;
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

	private float breathPercentage;

	private float targetBreathableMassAvailable;

	public AbsorbCellQuery.AbsorbOxygenSafeCellFlags targetCellSafetyFlags;

	public float targetCellBreathabilityScore;

	private int allowCellEvenIfReserved = -1;

	private SafetyChecker checker;

	private SafetyChecker.Context context;

	private bool isRecoveringFromSuffocation;

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
