using System;
using KSerialization;
using UnityEngine;

public class DrowningMonitor : KMonoBehaviour
{
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
		this.staminaHandle = GameScheduler.Instance.SchedulePeriodic(base.name, this.staminaUpdateFrequency, new Action<object>(this.UpdateStamina), null, null, 0f);
		this.checkDrowningHandle = GameScheduler.Instance.SchedulePeriodic(base.name, this.staminaUpdateFrequency, new Action<object>(this.CheckDrowning), null, null, 0f);
		this.selectable = base.GetComponent<KSelectable>();
		this.OnMove(null);
		this.CheckDrowning(null);
		this.Subscribe(1088554450, new EventSystem.EventHandler(this.OnMove));
	}

	private void OnMove(object data = null)
	{
		this.position = Grid.PosToCell(base.gameObject);
		if (this.partitionerEntry != null)
		{
			this.partitionerEntry.UpdatePosition(this.position);
		}
		else
		{
			Vector2I vector2I = Grid.PosToXY(this.transform.position);
			Extents extents = new Extents(vector2I.x, vector2I.y, 1, 2);
			this.partitionerEntry = GameScenePartitioner.Instance.Add("DrowningMonitor.OnSpawn", base.gameObject, extents, GameScenePartitioner.Instance.liquidChangedMask.mask, new Action<object>(this.OnLiquidChanged));
		}
		this.CheckDrowning(null);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		for (int i = 0; i < this.partitionerEntry.height; i++)
		{
			for (int j = 0; j < this.partitionerEntry.width; j++)
			{
				int num = Grid.PosToCell(new Vector2((float)(this.partitionerEntry.x + j) + 0.5f, (float)(this.partitionerEntry.y + i) + 1f));
				Gizmos.DrawCube(Grid.CellToPos(num) + Vector3.up / 2f + Vector3.right / 2f, Vector3.one);
			}
		}
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
		bool flag = true;
		for (int i = 0; i < this.partitionerEntry.height; i++)
		{
			for (int j = 0; j < this.partitionerEntry.width; j++)
			{
				int num = Grid.PosToCell(new Vector2((float)(this.partitionerEntry.x + j) + 0.5f, (float)(this.partitionerEntry.y + i) + 1f));
				flag = flag && this.IsCellSafe(num);
			}
		}
		if (!flag)
		{
			if (!this.drowning)
			{
				this.drowning = true;
				this.Trigger(1949704522, null);
				this.selectable.AddStatusItem(Db.Get().CreatureStatusItems.Drowning, null);
			}
			if (this.stamina <= 0f)
			{
				this.Trigger(-750750377, null);
				this.SetIncapacitated(true);
			}
		}
		else
		{
			if (this.drowning)
			{
				this.drowning = false;
				this.Trigger(99949694, null);
			}
			this.selectable.RemoveStatusItem(Db.Get().CreatureStatusItems.Drowning);
		}
	}

	public bool IsCellSafe(int cell)
	{
		int num;
		int num2;
		Vector2 vector;
		if (this.partitionerEntry == null)
		{
			num = this.extents.width;
			num2 = this.extents.height;
			vector = new Vector2((float)this.extents.x, (float)this.extents.y);
		}
		else
		{
			num = this.partitionerEntry.width;
			num2 = this.partitionerEntry.height;
			vector = new Vector2((float)this.partitionerEntry.x, (float)this.partitionerEntry.y);
		}
		for (int i = 0; i < num2; i++)
		{
			for (int j = 0; j < num; j++)
			{
				int num3 = Grid.PosToCell(new Vector2(vector.x + (float)j + 0.5f, vector.y + (float)i + 1f));
				int num4 = Grid.CellAbove(num3);
				if (!Grid.IsValidCell(num3) || !Grid.IsValidCell(num4))
				{
					return false;
				}
				if ((Grid.IsSubstantialLiquid(num4, 0.2f) && Grid.IsLiquid(num3)) || Grid.IsSubstantialLiquid(num3, this.cellLiquidThreshold))
				{
					return false;
				}
			}
		}
		return true;
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

	private KSelectable selectable;

	private GameScenePartitionerEntry partitionerEntry;

	private SchedulerHandle staminaHandle;

	private SchedulerHandle checkDrowningHandle;
}
