using System;
using System.Collections.Generic;
using Klei;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class TinkerStation : Workable, IEffectDescriptor, ISim200ms
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
		base.SetWorkTime(15f);
		base.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
	}

	private bool CorrectRolePrecondition(MinionIdentity worker)
	{
		MinionResume component = worker.GetComponent<MinionResume>();
		return component != null && component.HasPerk(this.requiredRolePerk);
	}

	private void OnOperationalChanged(object data)
	{
		RoomTracker component = base.GetComponent<RoomTracker>();
		if (component != null && component.room != null)
		{
			component.room.RetriggerBuildings();
		}
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole("PowerTechnician", work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
		resume.AddExperienceIfRole("Farmer", work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
		resume.AddExperienceIfRole("SeniorFarmer", work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		if (!this.operational.IsOperational)
		{
			return;
		}
		this.operational.SetActive(true, false);
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		base.ShowProgressBar(false);
		this.operational.SetActive(false, false);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		SimUtil.DiseaseInfo diseaseInfo;
		float num;
		this.storage.ConsumeAndGetDisease(this.inputMaterial, this.metalPerTinker, out diseaseInfo, out num);
		GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(this.outputPrefab), base.transform.GetPosition(), Grid.SceneLayer.Ore, Folder.Ore, null, 0);
		gameObject.SetActive(true);
		this.chore = null;
	}

	public void Sim200ms(float dt)
	{
		this.UpdateChore();
	}

	private void UpdateChore()
	{
		if (this.operational.IsOperational && this.ToolsRequested() && this.HasMaterial())
		{
			if (this.chore == null)
			{
				this.chore = new WorkChore<TinkerStation>(Db.Get().ChoreTypes.GetByHash(this.choreType), this, null, null, true, null, null, null, true, null, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 0, false);
				this.chore.AddPrecondition(ChorePreconditions.instance.HasRolePerk, this.requiredRolePerk);
				base.SetWorkTime(this.workTime);
			}
		}
		else if (this.chore != null)
		{
			this.chore.Cancel("Can't tinker");
			this.chore = null;
		}
	}

	private bool HasMaterial()
	{
		return this.storage.MassStored() > 0f;
	}

	private bool ToolsRequested()
	{
		return MaterialNeeds.Instance.GetAmount(this.outputPrefab) > 0f && WorldInventory.Instance.GetAmount(this.outputPrefab) <= 0f;
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		string text = this.inputMaterial.ProperName();
		List<Descriptor> list = new List<Descriptor>();
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, text, GameUtil.GetFormattedMass(this.metalPerTinker, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, text, GameUtil.GetFormattedMass(this.metalPerTinker, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), Descriptor.DescriptorType.Requirement, false));
		list.AddRange(GameUtil.GetAllDescriptors(Assets.GetPrefab(this.outputPrefab), false));
		List<Tinkerable> list2 = new List<Tinkerable>();
		foreach (GameObject gameObject in Assets.GetPrefabsWithComponent<Tinkerable>())
		{
			Tinkerable component = gameObject.GetComponent<Tinkerable>();
			if (component.tinkerMaterialTag == this.outputPrefab)
			{
				list2.Add(component);
			}
		}
		if (list2.Count > 0)
		{
			Effect effect = Db.Get().effects.Get(list2[0].addedEffect);
			list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.ADDED_EFFECT, effect.Name), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ADDED_EFFECT, effect.Name, Effect.CreateTooltip(effect, true)), Descriptor.DescriptorType.Effect, false));
			list.Add(new Descriptor(UI.BUILDINGEFFECTS.IMPROVED_BUILDINGS, UI.BUILDINGEFFECTS.TOOLTIPS.IMPROVED_BUILDINGS, Descriptor.DescriptorType.Effect, false));
			foreach (Tinkerable tinkerable in list2)
			{
				Descriptor descriptor = new Descriptor(string.Format(UI.BUILDINGEFFECTS.IMPROVED_BUILDINGS_ITEM, tinkerable.GetProperName()), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.IMPROVED_BUILDINGS_ITEM, tinkerable.GetProperName()), Descriptor.DescriptorType.Effect, false);
				descriptor.IncreaseIndent();
				list.Add(descriptor);
			}
		}
		return list;
	}

	public static TinkerStation AddTinkerStation(GameObject go, string required_room_type)
	{
		TinkerStation tinkerStation = go.AddOrGet<TinkerStation>();
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = required_room_type;
		return tinkerStation;
	}

	public HashedString choreType;

	private Chore chore;

	[MyCmpAdd]
	private Operational operational;

	[MyCmpAdd]
	private Storage storage;

	public float metalPerTinker;

	public Tag inputMaterial;

	public Tag outputPrefab;
}
