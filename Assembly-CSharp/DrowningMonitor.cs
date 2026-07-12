using System;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/DrowningMonitor")]
public class DrowningMonitor : KMonoBehaviour, IWiltCause, ISlicedSim1000ms
{
	private OccupyArea occupyArea
	{
		get
		{
			if (this._occupyArea == null)
			{
				this._occupyArea = base.GetComponent<OccupyArea>();
			}
			return this._occupyArea;
		}
	}

	public bool Drowning
	{
		get
		{
			return this.drowning;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.timeToDrown = 75f;
		if (DrowningMonitor.drowningEffect == null)
		{
			DrowningMonitor.drowningEffect = new Effect("Drowning", CREATURES.STATUSITEMS.DROWNING.NAME, CREATURES.STATUSITEMS.DROWNING.TOOLTIP, 0f, false, false, true, null, 0f, null, "");
			DrowningMonitor.drowningEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, -100f, CREATURES.STATUSITEMS.DROWNING.NAME, false, false, true));
		}
		if (DrowningMonitor.saturatedEffect == null)
		{
			DrowningMonitor.saturatedEffect = new Effect("Saturated", CREATURES.STATUSITEMS.SATURATED.NAME, CREATURES.STATUSITEMS.SATURATED.TOOLTIP, 0f, false, false, true, null, 0f, null, "");
			DrowningMonitor.saturatedEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, -100f, CREATURES.STATUSITEMS.SATURATED.NAME, false, false, true));
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		SlicedUpdaterSim1000ms<DrowningMonitor>.instance.RegisterUpdate1000ms(this);
		this.OnMove();
		this.CheckDrowning(null);
		Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, new global::System.Action(this.OnMove), "DrowningMonitor.OnSpawn");
	}

	private void OnMove()
	{
		if (this.partitionerEntry.IsValid())
		{
			Extents extents = this.occupyArea.GetExtents();
			GameScenePartitioner.Instance.UpdatePosition(this.partitionerEntry, extents);
		}
		else
		{
			this.partitionerEntry = GameScenePartitioner.Instance.Add("DrowningMonitor.OnSpawn", base.gameObject, this.occupyArea.GetExtents(), GameScenePartitioner.Instance.liquidChangedLayer, new Action<object>(this.OnLiquidChanged));
		}
		this.CheckDrowning(null);
	}

	protected override void OnCleanUp()
	{
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, new global::System.Action(this.OnMove));
		GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
		SlicedUpdaterSim1000ms<DrowningMonitor>.instance.UnregisterUpdate1000ms(this);
		base.OnCleanUp();
	}

	private void CheckDrowning(object data = null)
	{
		if (this.drowned)
		{
			return;
		}
		int num = Grid.PosToCell(base.gameObject.transform.GetPosition());
		if (!this.IsCellSafe(num))
		{
			if (!this.drowning)
			{
				this.drowning = true;
				base.GetComponent<KPrefabID>().AddTag(GameTags.Creatures.Drowning, false);
				base.Trigger(1949704522, null);
			}
			if (this.timeToDrown <= 0f && this.canDrownToDeath)
			{
				DeathMonitor.Instance smi = this.GetSMI<DeathMonitor.Instance>();
				if (smi != null)
				{
					smi.Kill(Db.Get().Deaths.Drowned);
				}
				base.Trigger(-750750377, null);
				this.drowned = true;
			}
		}
		else if (this.drowning)
		{
			this.drowning = false;
			base.GetComponent<KPrefabID>().RemoveTag(GameTags.Creatures.Drowning);
			base.Trigger(99949694, null);
		}
		if (this.livesUnderWater)
		{
			this.saturatedStatusGuid = this.selectable.ToggleStatusItem(Db.Get().CreatureStatusItems.Saturated, this.saturatedStatusGuid, this.drowning, this);
		}
		else
		{
			this.drowningStatusGuid = this.selectable.ToggleStatusItem(Db.Get().CreatureStatusItems.Drowning, this.drowningStatusGuid, this.drowning, this);
		}
		if (this.effects != null)
		{
			if (this.drowning)
			{
				if (this.livesUnderWater)
				{
					this.effects.Add(DrowningMonitor.saturatedEffect, false);
					return;
				}
				this.effects.Add(DrowningMonitor.drowningEffect, false);
				return;
			}
			else
			{
				if (this.livesUnderWater)
				{
					this.effects.Remove(DrowningMonitor.saturatedEffect);
					return;
				}
				this.effects.Remove(DrowningMonitor.drowningEffect);
			}
		}
	}

	private static bool CellSafeTest(int testCell, object data)
	{
		int num = Grid.CellAbove(testCell);
		if (!Grid.IsValidCell(testCell) || !Grid.IsValidCell(num))
		{
			return false;
		}
		if (Grid.IsSubstantialLiquid(testCell, 0.95f))
		{
			return false;
		}
		if (Grid.IsLiquid(testCell))
		{
			if (Grid.Element[num].IsLiquid)
			{
				return false;
			}
			if (Grid.Element[num].IsSolid)
			{
				return false;
			}
		}
		return true;
	}

	public bool IsCellSafe(int cell)
	{
		return this.occupyArea.TestArea(cell, this, DrowningMonitor.CellSafeTestDelegate);
	}

	WiltCondition.Condition[] IWiltCause.Conditions
	{
		get
		{
			return new WiltCondition.Condition[] { WiltCondition.Condition.Drowning };
		}
	}

	public string WiltStateString
	{
		get
		{
			if (this.livesUnderWater)
			{
				return "    • " + CREATURES.STATUSITEMS.SATURATED.NAME;
			}
			return "    • " + CREATURES.STATUSITEMS.DROWNING.NAME;
		}
	}

	private void OnLiquidChanged(object data)
	{
		this.CheckDrowning(null);
	}

	public void SlicedSim1000ms(float dt)
	{
		this.CheckDrowning(null);
		if (this.drowning)
		{
			if (!this.drowned)
			{
				this.timeToDrown -= dt;
				if (this.timeToDrown <= 0f)
				{
					this.CheckDrowning(null);
					return;
				}
			}
		}
		else
		{
			this.timeToDrown += dt * 5f;
			this.timeToDrown = Mathf.Clamp(this.timeToDrown, 0f, 75f);
		}
	}

	[MyCmpReq]
	private KSelectable selectable;

	[MyCmpGet]
	private Effects effects;

	private OccupyArea _occupyArea;

	[Serialize]
	[SerializeField]
	private float timeToDrown;

	[Serialize]
	private bool drowned;

	private bool drowning;

	protected const float MaxDrownTime = 75f;

	protected const float RegenRate = 5f;

	protected const float CellLiquidThreshold = 0.95f;

	public bool canDrownToDeath = true;

	public bool livesUnderWater;

	private Guid drowningStatusGuid;

	private Guid saturatedStatusGuid;

	private Extents extents;

	private HandleVector<int>.Handle partitionerEntry;

	public static Effect drowningEffect;

	public static Effect saturatedEffect;

	private static readonly Func<int, object, bool> CellSafeTestDelegate = (int testCell, object data) => DrowningMonitor.CellSafeTest(testCell, data);
}
