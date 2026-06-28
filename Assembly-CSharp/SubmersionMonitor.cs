using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class SubmersionMonitor : KMonoBehaviour, IGameObjectEffectDescriptor
{
	public bool Dry
	{
		get
		{
			return this.dry;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.checkDrynessHandle = GameScheduler.Instance.SchedulePeriodic(base.name, this.pollFrequency, new Action<object>(this.CheckDry), null, null, 0f, null);
		this.selectable = base.GetComponent<KSelectable>();
		this.OnMove(null);
		this.CheckDry(null);
		this.Subscribe(1088554450, new Action<object>(this.OnMove));
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
		this.CheckDry(null);
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
		this.checkDrynessHandle.Clear();
	}

	public void Configure(float _maxStamina, float _staminaRegenRate, float _cellLiquidThreshold = 0.95f)
	{
		this.cellLiquidThreshold = _cellLiquidThreshold;
	}

	private void CheckDry(object data = null)
	{
		if (!this.IsCellSafe())
		{
			if (!this.dry)
			{
				this.dry = true;
				this.Trigger(-2057657673, null);
				this.selectable.AddStatusItem(Db.Get().CreatureStatusItems.DryingOut, null);
			}
		}
		else
		{
			if (this.dry)
			{
				this.dry = false;
				this.Trigger(1555379996, null);
			}
			this.selectable.RemoveStatusItem(Db.Get().CreatureStatusItems.DryingOut, false);
		}
	}

	public bool IsCellSafe()
	{
		int num = Grid.PosToCell(base.gameObject);
		return Grid.IsValidCell(num) && Grid.IsSubstantialLiquid(num, this.cellLiquidThreshold);
	}

	private void OnLiquidChanged(object data)
	{
		this.CheckDry(null);
	}

	public void SetIncapacitated(bool state)
	{
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return new List<Descriptor>
		{
			new Descriptor(UI.GAMEOBJECTEFFECTS.REQUIRES_SUBMERSION, UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_SUBMERSION, Descriptor.DescriptorType.Requirement, false)
		};
	}

	private int position;

	private bool dry;

	protected float cellLiquidThreshold = 0.2f;

	private float pollFrequency = 1f;

	private Extents extents;

	private KSelectable selectable;

	private GameScenePartitionerEntry partitionerEntry;

	private SchedulerHandle checkDrynessHandle;
}
