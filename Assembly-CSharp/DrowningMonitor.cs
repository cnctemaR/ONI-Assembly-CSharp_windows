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
		this.timeToDrown = 15f;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.OnMove();
		this.CheckDrowning(null);
		Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, new global::System.Action(this.OnMove), "DrowningMonitor.OnSpawn");
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
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, new global::System.Action(this.OnMove));
		if (this.partitionerEntry != null)
		{
			this.partitionerEntry.Release();
		}
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
				base.Trigger(1949704522, null);
				base.GetComponent<KPrefabID>().AddTag(GameTags.Creatures.Drowning);
			}
			if (this.timeToDrown <= 0f)
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
		return this.occupyArea.TestArea(cell, this, new Func<int, object, bool>(DrowningMonitor.CellSafeTest));
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
			if (!this.drowned)
			{
				this.timeToDrown -= dt;
				if (this.timeToDrown <= 0f)
				{
					this.CheckDrowning(null);
				}
			}
		}
		else
		{
			this.timeToDrown += dt * 5f;
			this.timeToDrown = Mathf.Clamp(this.timeToDrown, 0f, 15f);
		}
	}

	private OccupyArea _occupyArea;

	[Serialize]
	[SerializeField]
	private float timeToDrown;

	[Serialize]
	private bool drowned;

	private bool drowning;

	protected const float MaxDrownTime = 15f;

	protected const float RegenRate = 5f;

	protected const float CellLiquidThreshold = 0.95f;

	private Extents extents;

	private GameScenePartitionerEntry partitionerEntry;
}
