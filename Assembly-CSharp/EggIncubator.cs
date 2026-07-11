using System;
using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class EggIncubator : SingleEntityReceptacle, ISaveLoadable, ISim1000ms
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.statusItemNeed = Db.Get().BuildingStatusItems.NeedEgg;
		this.statusItemNoneAvailable = Db.Get().BuildingStatusItems.NoAvailableEgg;
		this.statusItemAwaitingDelivery = Db.Get().BuildingStatusItems.AwaitingEggDelivery;
		this.requiredRolePerk = RoleManager.rolePerks.CanWrangleCreatures.id;
		this.occupyingObjectRelativePosition = new Vector3(0.5f, 1f, -1f);
		this.synchronizeAnims = false;
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		component.SetSymbolVisiblity("egg_target", false);
		this.meter = new MeterController(this, Meter.Offset.Infront, Array.Empty<string>());
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (base.occupyingObject)
		{
			if (base.occupyingObject.HasTag(GameTags.Creature))
			{
				this.storage.allowItemRemoval = true;
			}
			this.storage.RenotifyAll();
			this.PositionOccupyingObject();
		}
		base.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
		base.Subscribe(-731304873, new Action<object>(this.OnOccupantChanged));
		base.Subscribe(-1697596308, new Action<object>(this.OnStorageChange));
		this.smi = new EggIncubatorStates.Instance(this);
		this.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		this.smi.StopSM("cleanup");
		base.OnCleanUp();
	}

	protected override void SubscribeToOccupant()
	{
		base.SubscribeToOccupant();
		if (base.occupyingObject != null)
		{
			this.tracker = base.occupyingObject.AddComponent<KBatchedAnimTracker>();
			this.tracker.symbol = "egg_target";
			this.tracker.forceAlwaysVisible = true;
		}
		this.UpdateProgress();
	}

	protected override void UnsubscribeFromOccupant()
	{
		base.UnsubscribeFromOccupant();
		global::UnityEngine.Object.Destroy(this.tracker);
		this.tracker = null;
		this.UpdateProgress();
	}

	private void OnOperationalChanged(object data = null)
	{
		if (!base.occupyingObject)
		{
			this.storage.DropAll(false);
		}
	}

	private void OnOccupantChanged(object data = null)
	{
		if (!base.occupyingObject)
		{
			this.storage.allowItemRemoval = false;
		}
	}

	private void OnStorageChange(object data = null)
	{
		if (base.occupyingObject && !this.storage.items.Contains(base.occupyingObject))
		{
			this.UnsubscribeFromOccupant();
			base.occupyingObject = null;
			this.ClearOccupant();
		}
	}

	protected override void PositionOccupyingObject()
	{
		base.PositionOccupyingObject();
		KBatchedAnimController component = base.occupyingObject.GetComponent<KBatchedAnimController>();
		component.SetSceneLayer(Grid.SceneLayer.BuildingUse);
		KSelectable component2 = base.occupyingObject.GetComponent<KSelectable>();
		if (component2 != null)
		{
			component2.enabled = true;
		}
	}

	public override void OrderRemoveOccupant()
	{
		global::UnityEngine.Object.Destroy(this.tracker);
		this.tracker = null;
		this.storage.DropAll(false);
		base.occupyingObject = null;
		this.ClearOccupant();
	}

	public float GetProgress()
	{
		float num = 0f;
		if (base.occupyingObject)
		{
			Amounts amounts = base.occupyingObject.GetAmounts();
			AmountInstance amountInstance = amounts.Get(Db.Get().Amounts.Incubation);
			if (amountInstance != null)
			{
				num = amountInstance.value / amountInstance.GetMax();
			}
			else
			{
				num = 1f;
			}
		}
		return num;
	}

	private void UpdateProgress()
	{
		this.meter.SetPositionPercent(this.GetProgress());
	}

	public void Sim1000ms(float dt)
	{
		this.UpdateProgress();
		this.UpdateChore();
	}

	public void StoreBaby(GameObject baby)
	{
		this.UnsubscribeFromOccupant();
		this.storage.DropAll(false);
		this.storage.allowItemRemoval = true;
		this.storage.Store(baby, false, false, true, false);
		base.occupyingObject = baby;
		this.SubscribeToOccupant();
		base.Trigger(-731304873, base.occupyingObject);
	}

	private void UpdateChore()
	{
		if (this.operational.IsOperational && this.EggNeedsAttention())
		{
			if (this.chore == null)
			{
				this.chore = new WorkChore<EggIncubatorWorkable>(Db.Get().ChoreTypes.EggSing, this.workable, null, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 0, false);
			}
		}
		else if (this.chore != null)
		{
			this.chore.Cancel("now is not the time for song");
			this.chore = null;
		}
	}

	private bool EggNeedsAttention()
	{
		if (!base.Occupant)
		{
			return false;
		}
		IncubationMonitor.Instance instance = base.Occupant.GetSMI<IncubationMonitor.Instance>();
		return instance != null && !instance.HasSongBuff();
	}

	[MyCmpAdd]
	private EggIncubatorWorkable workable;

	private Chore chore;

	private EggIncubatorStates.Instance smi;

	private KBatchedAnimTracker tracker;

	private MeterController meter;
}
