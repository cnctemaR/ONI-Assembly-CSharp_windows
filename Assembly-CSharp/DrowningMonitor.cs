using System;
using KSerialization;
using STRINGS;
using UnityEngine;

public class DrowningMonitor : KMonoBehaviour, IWiltCause, ISim1000ms
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
		this.stamina = this.maxStamina;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.OnMove();
		this.CheckDrowning(null);
		CellChangeMonitor.Instance.RegisterCellChangedHandler(base.transform, new global::System.Action(this.OnMove));
	}

	private void OnMove()
	{
		if (this.partitionerEntry != null)
		{
			Extents extents = this.occupyArea.GetExtents();
			this.partitionerEntry.UpdatePosition(extents.x, extents.y);
		}
		else
		{
			this.partitionerEntry = GameScenePartitioner.Instance.Add("DrowningMonitor.OnSpawn", base.gameObject, this.occupyArea.GetExtents(), GameScenePartitioner.Instance.liquidChangedLayer, new Action<object>(this.OnLiquidChanged));
		}
		this.CheckDrowning(null);
	}

	protected override void OnCleanUp()
	{
		CellChangeMonitor.Instance.UnregisterCellChangedHandler(base.transform, new global::System.Action(this.OnMove));
		if (this.partitionerEntry != null)
		{
			this.partitionerEntry.Release();
		}
		base.OnCleanUp();
	}

	public void Configure(float maxStamina, float staminaRegenRate, float cellLiquidThreshold = 0.95f)
	{
		this.maxStamina = maxStamina;
		this.stamina = maxStamina;
		this.staminaRegenRate = staminaRegenRate;
		this.cellLiquidThreshold = cellLiquidThreshold;
	}

	private void CheckDrowning(object data = null)
	{
		if (this.incapacitated)
		{
			return;
		}
		int num = Grid.PosToCell(base.gameObject.transform.GetPosition());
		if (!this.IsCellSafe(num))
		{
			if (!this.drowning)
			{
				this.drowning = true;
				base.Trigger(1949704522, null);
				base.GetComponent<KPrefabID>().AddTag(GameTags.Creatures.Drowning);
			}
			if (this.stamina <= 0f)
			{
				DeathMonitor.Instance smi = this.GetSMI<DeathMonitor.Instance>();
				if (smi != null)
				{
					smi.Kill(Db.Get().Deaths.Drowned);
				}
				base.Trigger(-750750377, null);
				this.SetIncapacitated(true);
			}
		}
		else if (this.drowning)
		{
			this.drowning = false;
			base.GetComponent<KPrefabID>().RemoveTag(GameTags.Creatures.Drowning);
			base.Trigger(99949694, null);
		}
	}

	public bool IsCellSafe(int cell)
	{
		return this.occupyArea.TestArea(cell, this, delegate(int testCell, object data)
		{
			DrowningMonitor drowningMonitor = (DrowningMonitor)data;
			int num = Grid.CellAbove(testCell);
			return Grid.IsValidCell(testCell) && Grid.IsValidCell(num) && (!Grid.IsLiquid(num) || !Grid.IsLiquid(testCell)) && !Grid.IsSubstantialLiquid(testCell, drowningMonitor.cellLiquidThreshold);
		});
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
			if (this.drowning)
			{
				return Db.Get().CreatureStatusItems.Drowning.resolveStringCallback(CREATURES.STATUSITEMS.DROWNING.NAME, this);
			}
			return string.Empty;
		}
	}

	private void OnLiquidChanged(object data)
	{
		this.CheckDrowning(null);
	}

	public void Sim1000ms(float dt)
	{
		this.CheckDrowning(null);
		if (this.drowning)
		{
			if (!this.incapacitated)
			{
				this.stamina -= this.staminaUpdateFrequency;
				if (this.stamina <= 0f)
				{
					this.CheckDrowning(null);
				}
			}
		}
		else
		{
			this.stamina = Mathf.Clamp(this.stamina + this.staminaUpdateFrequency * this.staminaRegenRate, 0f, this.maxStamina);
		}
	}

	public void SetIncapacitated(bool state)
	{
		this.incapacitated = state;
	}

	[MyCmpReq]
	private KSelectable selectable;

	private OccupyArea _occupyArea;

	private int position;

	[Serialize]
	private float stamina = -1f;

	[Serialize]
	private bool incapacitated;

	private bool drowning;

	protected float maxStamina = 10f;

	protected float staminaRegenRate = 5f;

	protected float cellLiquidThreshold = 0.95f;

	private float staminaUpdateFrequency = 1f;

	private Extents extents;

	private GameScenePartitionerEntry partitionerEntry;
}
