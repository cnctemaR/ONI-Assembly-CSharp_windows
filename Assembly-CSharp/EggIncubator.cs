using System;
using Klei.AI;
using KSerialization;
using TUNING;
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
		component.HideSymbol(new KAnimHashedString("egg_target"), true);
		this.meter = new MeterController(this, Meter.Offset.Infront, new string[0]);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (base.occupyingObject)
		{
			base.occupyingObject.Trigger(1309017699, this.storage);
		}
		base.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
		base.Subscribe(-731304873, new Action<object>(this.OnOccupantChanged));
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
			base.Subscribe(base.occupyingObject, 657149762, new Action<object>(this.OnReadyToHatch));
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
		if (base.occupyingObject != null)
		{
			base.Unsubscribe(base.occupyingObject, 657149762, new Action<object>(this.OnReadyToHatch));
		}
		this.UpdateProgress();
	}

	private void OnReadyToHatch(object data = null)
	{
		this.UpdateChore();
	}

	private void OnOperationalChanged(object data = null)
	{
		this.UpdateChore();
		if (!base.occupyingObject)
		{
			this.storage.DropAll(false);
		}
	}

	private void OnOccupantChanged(object data = null)
	{
		this.UpdateChore();
	}

	private void UpdateChore()
	{
		this.smi.sm.readyToHatch.Set(this.HasHatchableEgg(), this.smi);
		if (this.operational.IsOperational && this.HasHatchableEgg())
		{
			if (this.chore == null)
			{
				this.chore = new WorkChore<CompleteIncubationWorkable>(Db.Get().ChoreTypes.CreatureHatch, this, null, null, true, null, null, null, true, null, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 0, false);
			}
		}
		else if (this.chore != null)
		{
			this.chore.Cancel("Can't hatch");
			this.chore = null;
		}
	}

	private bool HasHatchableEgg()
	{
		return base.occupyingObject && base.occupyingObject.HasTag(GameTags.FullyIncubated);
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole("Rancher", work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
		resume.AddExperienceIfRole("SeniorRancher", work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
	}

	public void CompleteHatch()
	{
		if (this.chore != null)
		{
			this.chore.Cancel("completed");
			this.chore = null;
		}
		GameObject occupyingObject = base.occupyingObject;
		base.occupyingObject = null;
		this.ClearOccupant();
		global::UnityEngine.Object.Destroy(this.tracker);
		this.tracker = null;
		if (occupyingObject)
		{
			this.storage.Remove(occupyingObject);
			occupyingObject.Trigger(1922945024, null);
		}
	}

	protected override void PositionOccupyingObject()
	{
		base.PositionOccupyingObject();
		KBatchedAnimController component = base.occupyingObject.GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			component.HackRefreshZOrder();
		}
		KBoxCollider2D component2 = base.occupyingObject.GetComponent<KBoxCollider2D>();
		if (component2 != null)
		{
			component2.enabled = true;
		}
		KSelectable component3 = base.occupyingObject.GetComponent<KSelectable>();
		if (component3 != null)
		{
			component3.enabled = true;
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

	private void UpdateProgress()
	{
		float num = 0f;
		if (base.occupyingObject)
		{
			Amounts amounts = base.occupyingObject.GetAmounts();
			AmountInstance amountInstance = amounts.Get(Db.Get().Amounts.Incubation);
			num = amountInstance.value / amountInstance.GetMax();
		}
		this.meter.SetPositionPercent(num);
	}

	public void Sim1000ms(float dt)
	{
		this.UpdateProgress();
	}

	[MyCmpAdd]
	private CompleteIncubationWorkable completeIncubationWorkable;

	private EggIncubatorStates.Instance smi;

	private Chore chore;

	private KBatchedAnimTracker tracker;

	private MeterController meter;
}
