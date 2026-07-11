using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class SubmersionMonitor : KMonoBehaviour, IGameObjectEffectDescriptor, IWiltCause, ISim1000ms
{
	public bool Dry
	{
		get
		{
			return this.dry;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.OnMove();
		this.CheckDry();
		Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, new global::System.Action(this.OnMove), "SubmersionMonitor.OnSpawn");
	}

	private void OnMove()
	{
		this.position = Grid.PosToCell(base.gameObject);
		if (this.partitionerEntry.IsValid())
		{
			GameScenePartitioner.Instance.UpdatePosition(this.partitionerEntry, this.position);
		}
		else
		{
			Vector2I vector2I = Grid.PosToXY(base.transform.GetPosition());
			Extents extents = new Extents(vector2I.x, vector2I.y, 1, 2);
			this.partitionerEntry = GameScenePartitioner.Instance.Add("DrowningMonitor.OnSpawn", base.gameObject, extents, GameScenePartitioner.Instance.liquidChangedLayer, new Action<object>(this.OnLiquidChanged));
		}
		this.CheckDry();
	}

	private void OnDrawGizmosSelected()
	{
	}

	protected override void OnCleanUp()
	{
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, new global::System.Action(this.OnMove));
		GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
		base.OnCleanUp();
	}

	public void Configure(float _maxStamina, float _staminaRegenRate, float _cellLiquidThreshold = 0.95f)
	{
		this.cellLiquidThreshold = _cellLiquidThreshold;
	}

	public void Sim1000ms(float dt)
	{
		this.CheckDry();
	}

	private void CheckDry()
	{
		if (!this.IsCellSafe())
		{
			if (!this.dry)
			{
				this.dry = true;
				base.Trigger(-2057657673, null);
				return;
			}
		}
		else if (this.dry)
		{
			this.dry = false;
			base.Trigger(1555379996, null);
		}
	}

	public bool IsCellSafe()
	{
		int num = Grid.PosToCell(base.gameObject);
		return Grid.IsValidCell(num) && Grid.IsSubstantialLiquid(num, this.cellLiquidThreshold);
	}

	private void OnLiquidChanged(object data)
	{
		this.CheckDry();
	}

	WiltCondition.Condition[] IWiltCause.Conditions
	{
		get
		{
			return new WiltCondition.Condition[] { WiltCondition.Condition.DryingOut };
		}
	}

	public string WiltStateString
	{
		get
		{
			if (this.Dry)
			{
				return Db.Get().CreatureStatusItems.DryingOut.resolveStringCallback(CREATURES.STATUSITEMS.DRYINGOUT.NAME, this);
			}
			return "";
		}
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

	private Extents extents;

	private HandleVector<int>.Handle partitionerEntry;
}
