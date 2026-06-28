using System;
using KSerialization;
using UnityEngine;

public class DrowningMonitor : KMonoBehaviour
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
		this.staminaHandle = GameScheduler.Instance.SchedulePeriodic(base.name, this.staminaUpdateFrequency, new Action<object>(this.UpdateStamina), null, null, 0f, null);
		this.checkDrowningHandle = GameScheduler.Instance.SchedulePeriodic(base.name, this.staminaUpdateFrequency, new Action<object>(this.CheckDrowning), null, null, 0f, null);
		this.OnMove(null);
		this.CheckDrowning(null);
		this.Subscribe(1088554450, new Action<object>(this.OnMove));
	}

	private void OnMove(object data = null)
	{
		if (this.partitionerEntry != null)
		{
			Extents extents = this.occupyArea.GetExtents();
			this.partitionerEntry.UpdatePosition(extents.x, extents.y);
		}
		else
		{
			this.partitionerEntry = GameScenePartitioner.Instance.Add("DrowningMonitor.OnSpawn", base.gameObject, this.occupyArea.GetExtents(), GameScenePartitioner.Instance.liquidChangedMask.mask, new Action<object>(this.OnLiquidChanged));
		}
		this.CheckDrowning(null);
	}

	protected override void OnCleanUp()
	{
		if (this.partitionerEntry != null)
		{
			this.partitionerEntry.Release();
		}
		base.OnCleanUp();
		this.staminaHandle.Clear();
		this.checkDrowningHandle.Clear();
	}

	public void Configure(float _maxStamina, float _staminaRegenRate, float _cellLiquidThreshold = 0.95f)
	{
		this.maxStamina = _maxStamina;
		this.stamina = this.maxStamina;
		this.staminaRegenRate = _staminaRegenRate;
		this.cellLiquidThreshold = _cellLiquidThreshold;
	}

	private void CheckDrowning(object data = null)
	{
		if (this.incapacitated)
		{
			return;
		}
		int num = Grid.PosToCell(base.gameObject.transform.position);
		if (!this.IsCellSafe(num))
		{
			if (!this.drowning)
			{
				this.drowning = true;
				this.selectable.AddStatusItem(Db.Get().CreatureStatusItems.Drowning, null);
				this.Trigger(1949704522, null);
			}
			if (this.stamina <= 0f)
			{
				this.Trigger(-750750377, null);
				this.SetIncapacitated(true);
			}
		}
		else if (this.drowning)
		{
			this.drowning = false;
			this.selectable.RemoveStatusItem(Db.Get().CreatureStatusItems.Drowning, false);
			this.Trigger(99949694, null);
		}
	}

	public bool IsCellSafe(int cell)
	{
		return this.occupyArea.TestArea(cell, delegate(int testCell)
		{
			int num = Grid.CellAbove(testCell);
			return Grid.IsValidCell(testCell) && Grid.IsValidCell(num) && (!Grid.IsLiquid(num) || !Grid.IsLiquid(testCell)) && !Grid.IsSubstantialLiquid(testCell, this.cellLiquidThreshold);
		});
	}

	private void OnLiquidChanged(object data)
	{
		this.CheckDrowning(null);
	}

	private void UpdateStamina(object data)
	{
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

	private SchedulerHandle staminaHandle;

	private SchedulerHandle checkDrowningHandle;
}
