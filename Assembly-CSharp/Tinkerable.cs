using System;
using Klei.AI;
using TUNING;
using UnityEngine;

public class Tinkerable : Workable
{
	public static Tinkerable MakePowerTinkerable(GameObject prefab)
	{
		RoomTracker roomTracker = prefab.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.PowerPlant.Id;
		roomTracker.requirement = RoomTracker.Requirement.TrackingOnly;
		Tinkerable tinkerable = prefab.AddOrGet<Tinkerable>();
		tinkerable.tinkerMaterialTag = PowerControlStationConfig.TINKER_TOOLS;
		tinkerable.tinkerMaterialAmount = 1f;
		tinkerable.addedEffect = "PowerTinker";
		tinkerable.requiredRolePerk = PowerControlStationConfig.ROLE_PERK;
		tinkerable.SetWorkTime(180f);
		tinkerable.workerStatusItem = Db.Get().DuplicantStatusItems.Tinkering;
		tinkerable.attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
		tinkerable.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		tinkerable.choreTags = GameTags.ChoreTypes.PowerChores;
		tinkerable.choreTypeTinker = Db.Get().ChoreTypes.PowerTinker;
		tinkerable.multitoolContext = "powertinker";
		tinkerable.multitoolHitEffectHash = new HashedString("fx_powertinker_splash");
		tinkerable.shouldShowRolePerkStatusItem = false;
		KPrefabID component = prefab.GetComponent<KPrefabID>();
		component.prefabInitFn += delegate(GameObject inst)
		{
			inst.GetComponent<Tinkerable>().SetOffsetTable(OffsetGroups.InvertedStandardTable);
		};
		return tinkerable;
	}

	public static Tinkerable MakeFarmTinkerable(GameObject prefab)
	{
		RoomTracker roomTracker = prefab.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.Farm.Id;
		roomTracker.requirement = RoomTracker.Requirement.TrackingOnly;
		Tinkerable tinkerable = prefab.AddOrGet<Tinkerable>();
		tinkerable.tinkerMaterialTag = FarmStationConfig.TINKER_TOOLS;
		tinkerable.tinkerMaterialAmount = 1f;
		tinkerable.addedEffect = "FarmTinker";
		tinkerable.requiredRolePerk = RoleManager.rolePerks.CanFarmTinker.id;
		tinkerable.workerStatusItem = Db.Get().DuplicantStatusItems.Tinkering;
		tinkerable.SetWorkTime(15f);
		tinkerable.attributeConverter = Db.Get().AttributeConverters.PlantTendSpeed;
		tinkerable.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		tinkerable.choreTypeTinker = Db.Get().ChoreTypes.CropTend;
		tinkerable.choreTags = GameTags.ChoreTypes.FarmingChores;
		tinkerable.multitoolContext = "tend";
		tinkerable.multitoolHitEffectHash = new HashedString("fx_tend_splash");
		tinkerable.shouldShowRolePerkStatusItem = false;
		KPrefabID component = prefab.GetComponent<KPrefabID>();
		component.prefabInitFn += delegate(GameObject inst)
		{
			inst.GetComponent<Tinkerable>().SetOffsetTable(OffsetGroups.InvertedStandardTable);
		};
		return tinkerable;
	}

	public static Tinkerable MakeMachineTinkerable(GameObject prefab)
	{
		RoomTracker roomTracker = prefab.AddOrGet<RoomTracker>();
		roomTracker.requirement = RoomTracker.Requirement.TrackingOnly;
		Tinkerable tinkerable = prefab.AddOrGet<Tinkerable>();
		tinkerable.tinkerMaterialTag = TagManager.Create("MachineParts", null);
		tinkerable.tinkerMaterialAmount = 1f;
		tinkerable.addedEffect = "MachineTinker";
		tinkerable.requiredRolePerk = RoleManager.rolePerks.IncreaseMachineryMedium.id;
		tinkerable.SetWorkTime(15f);
		tinkerable.workerStatusItem = Db.Get().DuplicantStatusItems.Tinkering;
		tinkerable.attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
		tinkerable.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		tinkerable.choreTypeTinker = Db.Get().ChoreTypes.MachineTinker;
		tinkerable.shouldShowRolePerkStatusItem = false;
		return tinkerable;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_use_machine_kanim") };
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Tinkering;
		this.attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
		this.faceTargetWhenWorking = true;
		this.synchronizeAnims = false;
		base.Subscribe(-1157678353, new Action<object>(this.OnEffectRemoved));
		base.Subscribe(-1697596308, new Action<object>(this.OnStorageChange));
		base.Subscribe(144050788, new Action<object>(this.OnUpdateRoom));
		base.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
	}

	protected override void OnCleanUp()
	{
		this.UpdateMaterialReservation(false);
		base.OnCleanUp();
	}

	private void OnOperationalChanged(object data)
	{
		this.UpdateChore();
	}

	private void OnEffectRemoved(object data)
	{
		this.UpdateChore();
	}

	private void OnUpdateRoom(object data)
	{
		this.UpdateChore();
	}

	private void OnStorageChange(object data)
	{
		this.UpdateChore();
	}

	private void UpdateChore()
	{
		Operational component = base.GetComponent<Operational>();
		bool flag = component == null || component.IsFunctional;
		bool flag2 = !this.HasEffect() && this.RoomHasActiveTinkerstation() && flag;
		if (this.chore == null && flag2)
		{
			this.UpdateMaterialReservation(true);
			base.SetWorkTime(this.workTime);
			if (this.HasMaterial())
			{
				this.chore = new WorkChore<Tinkerable>(this.choreTypeTinker, this, null, null, true, null, null, null, true, null, false, null, false, true, true, PriorityScreen.PriorityClass.basic, int.MaxValue, false);
				if (component != null)
				{
					this.chore.AddPrecondition(ChorePreconditions.instance.IsFunctional, base.gameObject);
				}
			}
			else
			{
				this.chore = new FetchChore(Db.Get().ChoreTypes.TinkerFetch, this.storage, this.tinkerMaterialAmount, new Tag[] { this.tinkerMaterialTag }, null, null, true, new Action<Chore>(this.OnFetchComplete), null, null, FetchOrder2.OperationalRequirement.Functional, 0, this.choreTags);
			}
			this.chore.AddPrecondition(ChorePreconditions.instance.HasRolePerk, this.requiredRolePerk);
			RoomTracker component2 = base.GetComponent<RoomTracker>();
			if (!string.IsNullOrEmpty(component2.requiredRoomType))
			{
				this.chore.AddPrecondition(ChorePreconditions.instance.IsInMyRoom, component2.room);
			}
		}
		else if (this.chore != null && !flag2)
		{
			this.UpdateMaterialReservation(false);
			this.chore.Cancel("No longer needed");
			this.chore = null;
		}
	}

	private bool RoomHasActiveTinkerstation()
	{
		if (!this.roomTracker.IsInCorrectRoom())
		{
			return false;
		}
		if (this.roomTracker.room == null)
		{
			return false;
		}
		foreach (KPrefabID kprefabID in this.roomTracker.room.buildings)
		{
			if (!(kprefabID == null))
			{
				TinkerStation component = kprefabID.GetComponent<TinkerStation>();
				if (component != null && component.outputPrefab == this.tinkerMaterialTag)
				{
					Operational component2 = kprefabID.GetComponent<Operational>();
					if (component2.IsOperational)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	private void UpdateMaterialReservation(bool shouldReserve)
	{
		if (shouldReserve && !this.hasReservedMaterial)
		{
			MaterialNeeds.Instance.UpdateNeed(this.tinkerMaterialTag, this.tinkerMaterialAmount);
			this.hasReservedMaterial = shouldReserve;
		}
		else if (!shouldReserve && this.hasReservedMaterial)
		{
			MaterialNeeds.Instance.UpdateNeed(this.tinkerMaterialTag, -this.tinkerMaterialAmount);
			this.hasReservedMaterial = shouldReserve;
		}
	}

	private void OnFetchComplete(Chore data)
	{
		this.UpdateMaterialReservation(false);
		this.chore = null;
		this.UpdateChore();
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		this.storage.ConsumeIgnoringDisease(this.tinkerMaterialTag, this.tinkerMaterialAmount);
		this.effects.Add(this.addedEffect, true);
		this.UpdateMaterialReservation(false);
		this.chore = null;
		this.UpdateChore();
	}

	private bool HasMaterial()
	{
		return this.storage.GetAmountAvailable(this.tinkerMaterialTag) >= this.tinkerMaterialAmount;
	}

	private bool HasEffect()
	{
		return this.effects.HasEffect(this.addedEffect);
	}

	private Chore chore;

	[MyCmpAdd]
	private Storage storage;

	[MyCmpAdd]
	private Effects effects;

	[MyCmpGet]
	private RoomTracker roomTracker;

	public Tag tinkerMaterialTag;

	public float tinkerMaterialAmount;

	public string addedEffect;

	public Tag[] choreTags;

	protected ChoreType choreTypeTinker = Db.Get().ChoreTypes.PowerTinker;

	private bool hasReservedMaterial;
}
