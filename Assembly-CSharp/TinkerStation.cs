using System;
using System.Collections.Generic;
using Klei;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/TinkerStation")]
public class TinkerStation : Workable, IGameObjectEffectDescriptor, ISim1000ms
{
	public AttributeConverter AttributeConverter
	{
		set
		{
			this.attributeConverter = value;
		}
	}

	public float AttributeExperienceMultiplier
	{
		set
		{
			this.attributeExperienceMultiplier = value;
		}
	}

	public string SkillExperienceSkillGroup
	{
		set
		{
			this.skillExperienceSkillGroup = value;
		}
	}

	public float SkillExperienceMultiplier
	{
		set
		{
			this.skillExperienceMultiplier = value;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
		this.skillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
		this.skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
		if (this.useFilteredStorage)
		{
			ChoreType byHash = Db.Get().ChoreTypes.GetByHash(this.fetchChoreType);
			this.filteredStorage = new FilteredStorage(this, null, null, false, byHash);
		}
		base.SetWorkTime(15f);
		base.Subscribe<TinkerStation>(-592767678, TinkerStation.OnOperationalChangedDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.useFilteredStorage && this.filteredStorage != null)
		{
			this.filteredStorage.FilterChanged();
		}
	}

	protected override void OnCleanUp()
	{
		if (this.filteredStorage != null)
		{
			this.filteredStorage.CleanUp();
		}
		base.OnCleanUp();
	}

	private bool CorrectRolePrecondition(MinionIdentity worker)
	{
		MinionResume component = worker.GetComponent<MinionResume>();
		return component != null && component.HasPerk(this.requiredSkillPerk);
	}

	private void OnOperationalChanged(object data)
	{
		RoomTracker component = base.GetComponent<RoomTracker>();
		if (component != null && component.room != null)
		{
			component.room.RetriggerBuildings();
		}
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
		float num;
		SimUtil.DiseaseInfo diseaseInfo;
		float num2;
		this.storage.ConsumeAndGetDisease(this.inputMaterial, this.massPerTinker, out num, out diseaseInfo, out num2);
		GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(this.outputPrefab), base.transform.GetPosition(), Grid.SceneLayer.Ore, null, 0);
		gameObject.GetComponent<PrimaryElement>().Temperature = this.outputTemperature;
		gameObject.SetActive(true);
		this.chore = null;
	}

	public void Sim1000ms(float dt)
	{
		this.UpdateChore();
	}

	private void UpdateChore()
	{
		if (this.operational.IsOperational && (this.ToolsRequested() || this.alwaysTinker) && this.HasMaterial())
		{
			if (this.chore == null)
			{
				this.chore = new WorkChore<TinkerStation>(Db.Get().ChoreTypes.GetByHash(this.choreType), this, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
				this.chore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, this.requiredSkillPerk);
				base.SetWorkTime(this.workTime);
				return;
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
		return MaterialNeeds.GetAmount(this.outputPrefab, base.gameObject.GetMyWorldId(), false) > 0f && this.GetMyWorld().worldInventory.GetAmount(this.outputPrefab, true) <= 0f;
	}

	public override List<Descriptor> GetDescriptors(GameObject go)
	{
		string text = this.inputMaterial.ProperName();
		List<Descriptor> descriptors = base.GetDescriptors(go);
		descriptors.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, text, GameUtil.GetFormattedMass(this.massPerTinker, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, text, GameUtil.GetFormattedMass(this.massPerTinker, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), Descriptor.DescriptorType.Requirement, false));
		descriptors.AddRange(GameUtil.GetAllDescriptors(Assets.GetPrefab(this.outputPrefab), false));
		List<Tinkerable> list = new List<Tinkerable>();
		foreach (GameObject gameObject in Assets.GetPrefabsWithComponent<Tinkerable>())
		{
			Tinkerable component = gameObject.GetComponent<Tinkerable>();
			if (component.tinkerMaterialTag == this.outputPrefab)
			{
				list.Add(component);
			}
		}
		if (list.Count > 0)
		{
			Effect effect = Db.Get().effects.Get(list[0].addedEffect);
			descriptors.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.ADDED_EFFECT, effect.Name), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ADDED_EFFECT, effect.Name, Effect.CreateTooltip(effect, true, "\n    • ", true)), Descriptor.DescriptorType.Effect, false));
			descriptors.Add(new Descriptor(UI.BUILDINGEFFECTS.IMPROVED_BUILDINGS, UI.BUILDINGEFFECTS.TOOLTIPS.IMPROVED_BUILDINGS, Descriptor.DescriptorType.Effect, false));
			foreach (Tinkerable tinkerable in list)
			{
				Descriptor descriptor = new Descriptor(string.Format(UI.BUILDINGEFFECTS.IMPROVED_BUILDINGS_ITEM, tinkerable.GetProperName()), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.IMPROVED_BUILDINGS_ITEM, tinkerable.GetProperName()), Descriptor.DescriptorType.Effect, false);
				descriptor.IncreaseIndent();
				descriptors.Add(descriptor);
			}
		}
		return descriptors;
	}

	public static TinkerStation AddTinkerStation(GameObject go, string required_room_type)
	{
		TinkerStation tinkerStation = go.AddOrGet<TinkerStation>();
		go.AddOrGet<RoomTracker>().requiredRoomType = required_room_type;
		return tinkerStation;
	}

	public HashedString choreType;

	public HashedString fetchChoreType;

	private Chore chore;

	[MyCmpAdd]
	private Operational operational;

	[MyCmpAdd]
	private Storage storage;

	public bool useFilteredStorage;

	protected FilteredStorage filteredStorage;

	public bool alwaysTinker;

	public float massPerTinker;

	public Tag inputMaterial;

	public Tag outputPrefab;

	public float outputTemperature;

	private static readonly EventSystem.IntraObjectHandler<TinkerStation> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<TinkerStation>(delegate(TinkerStation component, object data)
	{
		component.OnOperationalChanged(data);
	});
}
