using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/MinionStorage")]
public class MinionStorage : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Components.MinionStorages.Add(this);
	}

	protected override void OnCleanUp()
	{
		Components.MinionStorages.Remove(this);
		base.OnCleanUp();
	}

	private KPrefabID CreateSerializedMinion(GameObject src_minion)
	{
		GameObject gameObject = Util.KInstantiate(SaveLoader.Instance.saveManager.GetPrefab(StoredMinionConfig.ID), Vector3.zero);
		gameObject.SetActive(true);
		MinionIdentity component = src_minion.GetComponent<MinionIdentity>();
		StoredMinionIdentity component2 = gameObject.GetComponent<StoredMinionIdentity>();
		this.CopyMinion(component, component2);
		MinionStorage.RedirectInstanceTracker(src_minion, gameObject);
		component.assignableProxy.Get().SetTarget(component2, gameObject);
		Util.KDestroyGameObject(src_minion);
		return gameObject.GetComponent<KPrefabID>();
	}

	private void CopyMinion(MinionIdentity src_id, StoredMinionIdentity dest_id)
	{
		dest_id.storedName = src_id.name;
		dest_id.nameStringKey = src_id.nameStringKey;
		dest_id.gender = src_id.gender;
		dest_id.genderStringKey = src_id.genderStringKey;
		dest_id.arrivalTime = src_id.arrivalTime;
		dest_id.voiceIdx = src_id.voiceIdx;
		dest_id.bodyData = src_id.bodyData;
		Traits component = src_id.GetComponent<Traits>();
		dest_id.traitIDs = new List<string>(component.GetTraitIds());
		dest_id.assignableProxy.Set(src_id.assignableProxy.Get());
		dest_id.assignableProxy.Get().SetTarget(dest_id, dest_id.gameObject);
		Accessorizer component2 = src_id.GetComponent<Accessorizer>();
		dest_id.accessories = component2.GetAccessories();
		ConsumableConsumer component3 = src_id.GetComponent<ConsumableConsumer>();
		if (component3.forbiddenTags != null)
		{
			dest_id.forbiddenTags = new List<Tag>(component3.forbiddenTags);
		}
		MinionResume component4 = src_id.GetComponent<MinionResume>();
		dest_id.MasteryBySkillID = component4.MasteryBySkillID;
		dest_id.grantedSkillIDs = component4.GrantedSkillIDs;
		dest_id.AptitudeBySkillGroup = component4.AptitudeBySkillGroup;
		dest_id.TotalExperienceGained = component4.TotalExperienceGained;
		dest_id.currentHat = component4.CurrentHat;
		dest_id.targetHat = component4.TargetHat;
		ChoreConsumer component5 = src_id.GetComponent<ChoreConsumer>();
		dest_id.choreGroupPriorities = component5.GetChoreGroupPriorities();
		AttributeLevels component6 = src_id.GetComponent<AttributeLevels>();
		component6.OnSerializing();
		dest_id.attributeLevels = new List<AttributeLevels.LevelSaveLoad>(component6.SaveLoadLevels);
		Effects component7 = src_id.GetComponent<Effects>();
		dest_id.saveLoadEffects = component7.GetAllEffectsForSerialization();
		MinionStorage.StoreModifiers(src_id, dest_id);
		Schedulable component8 = src_id.GetComponent<Schedulable>();
		Schedule schedule = component8.GetSchedule();
		if (schedule != null)
		{
			schedule.Unassign(component8);
			Schedulable component9 = dest_id.GetComponent<Schedulable>();
			schedule.Assign(component9);
		}
	}

	private static void StoreModifiers(MinionIdentity src_id, StoredMinionIdentity dest_id)
	{
		foreach (AttributeInstance attributeInstance in src_id.GetComponent<MinionModifiers>().attributes)
		{
			if (dest_id.minionModifiers.attributes.Get(attributeInstance.Attribute.Id) == null)
			{
				dest_id.minionModifiers.attributes.Add(attributeInstance.Attribute);
			}
			for (int i = 0; i < attributeInstance.Modifiers.Count; i++)
			{
				dest_id.minionModifiers.attributes.Get(attributeInstance.Id).Add(attributeInstance.Modifiers[i]);
			}
		}
	}

	private static void CopyMinion(StoredMinionIdentity src_id, MinionIdentity dest_id)
	{
		dest_id.SetName(src_id.storedName);
		dest_id.nameStringKey = src_id.nameStringKey;
		dest_id.gender = src_id.gender;
		dest_id.genderStringKey = src_id.genderStringKey;
		dest_id.arrivalTime = src_id.arrivalTime;
		dest_id.voiceIdx = src_id.voiceIdx;
		dest_id.bodyData = src_id.bodyData;
		if (src_id.traitIDs != null)
		{
			dest_id.GetComponent<Traits>().SetTraitIds(src_id.traitIDs);
		}
		if (src_id.accessories != null)
		{
			dest_id.GetComponent<Accessorizer>().SetAccessories(src_id.accessories);
		}
		ConsumableConsumer component = dest_id.GetComponent<ConsumableConsumer>();
		if (src_id.forbiddenTags != null)
		{
			component.forbiddenTags = src_id.forbiddenTags.ToArray();
		}
		if (src_id.MasteryBySkillID != null)
		{
			MinionResume component2 = dest_id.GetComponent<MinionResume>();
			component2.RestoreResume(src_id.MasteryBySkillID, src_id.AptitudeBySkillGroup, src_id.grantedSkillIDs, src_id.TotalExperienceGained);
			component2.SetHats(src_id.currentHat, src_id.targetHat);
		}
		if (src_id.choreGroupPriorities != null)
		{
			dest_id.GetComponent<ChoreConsumer>().SetChoreGroupPriorities(src_id.choreGroupPriorities);
		}
		AttributeLevels component3 = dest_id.GetComponent<AttributeLevels>();
		if (src_id.attributeLevels != null)
		{
			component3.SaveLoadLevels = src_id.attributeLevels.ToArray();
			component3.OnDeserialized();
		}
		Effects component4 = dest_id.GetComponent<Effects>();
		if (src_id.saveLoadEffects != null)
		{
			foreach (Effects.SaveLoadEffect saveLoadEffect in src_id.saveLoadEffects)
			{
				if (Db.Get().effects.Exists(saveLoadEffect.id))
				{
					Effect effect = Db.Get().effects.Get(saveLoadEffect.id);
					EffectInstance effectInstance = component4.Add(effect, saveLoadEffect.saved);
					if (effectInstance != null)
					{
						effectInstance.timeRemaining = saveLoadEffect.timeRemaining;
					}
				}
			}
		}
		dest_id.GetComponent<Accessorizer>().ApplyAccessories();
		dest_id.assignableProxy = new Ref<MinionAssignablesProxy>();
		dest_id.assignableProxy.Set(src_id.assignableProxy.Get());
		dest_id.assignableProxy.Get().SetTarget(dest_id, dest_id.gameObject);
		Equipment equipment = dest_id.GetEquipment();
		foreach (AssignableSlotInstance assignableSlotInstance in equipment.Slots)
		{
			Equippable equippable = assignableSlotInstance.assignable as Equippable;
			if (equippable != null)
			{
				equipment.Equip(equippable);
			}
		}
		Schedulable component5 = src_id.GetComponent<Schedulable>();
		Schedule schedule = component5.GetSchedule();
		if (schedule != null)
		{
			schedule.Unassign(component5);
			Schedulable component6 = dest_id.GetComponent<Schedulable>();
			schedule.Assign(component6);
		}
	}

	public static void RedirectInstanceTracker(GameObject src_minion, GameObject dest_minion)
	{
		KPrefabID component = src_minion.GetComponent<KPrefabID>();
		dest_minion.GetComponent<KPrefabID>().InstanceID = component.InstanceID;
		component.InstanceID = -1;
	}

	public void SerializeMinion(GameObject minion)
	{
		this.CleanupBadReferences();
		KPrefabID kprefabID = this.CreateSerializedMinion(minion);
		MinionStorage.Info info = new MinionStorage.Info(kprefabID.GetComponent<StoredMinionIdentity>().storedName, new Ref<KPrefabID>(kprefabID));
		this.serializedMinions.Add(info);
	}

	private void CleanupBadReferences()
	{
		for (int i = this.serializedMinions.Count - 1; i >= 0; i--)
		{
			if (this.serializedMinions[i].serializedMinion == null || this.serializedMinions[i].serializedMinion.Get() == null)
			{
				this.serializedMinions.RemoveAt(i);
			}
		}
	}

	private int GetMinionIndex(Guid id)
	{
		int num = -1;
		for (int i = 0; i < this.serializedMinions.Count; i++)
		{
			if (this.serializedMinions[i].id == id)
			{
				num = i;
				break;
			}
		}
		return num;
	}

	public GameObject DeserializeMinion(Guid id, Vector3 pos)
	{
		int minionIndex = this.GetMinionIndex(id);
		if (minionIndex < 0 || minionIndex >= this.serializedMinions.Count)
		{
			return null;
		}
		KPrefabID kprefabID = this.serializedMinions[minionIndex].serializedMinion.Get();
		this.serializedMinions.RemoveAt(minionIndex);
		if (kprefabID == null)
		{
			return null;
		}
		return MinionStorage.DeserializeMinion(kprefabID.gameObject, pos);
	}

	public static GameObject DeserializeMinion(GameObject sourceMinion, Vector3 pos)
	{
		GameObject gameObject = Util.KInstantiate(SaveLoader.Instance.saveManager.GetPrefab(MinionConfig.ID), pos);
		StoredMinionIdentity component = sourceMinion.GetComponent<StoredMinionIdentity>();
		MinionIdentity component2 = gameObject.GetComponent<MinionIdentity>();
		MinionStorage.RedirectInstanceTracker(sourceMinion, gameObject);
		gameObject.SetActive(true);
		MinionStorage.CopyMinion(component, component2);
		component.assignableProxy.Get().SetTarget(component2, gameObject);
		Util.KDestroyGameObject(sourceMinion);
		return gameObject;
	}

	public void DeleteStoredMinion(Guid id)
	{
		int minionIndex = this.GetMinionIndex(id);
		if (minionIndex < 0)
		{
			return;
		}
		if (this.serializedMinions[minionIndex].serializedMinion != null)
		{
			this.serializedMinions[minionIndex].serializedMinion.Get().GetComponent<StoredMinionIdentity>().OnHardDelete();
			Util.KDestroyGameObject(this.serializedMinions[minionIndex].serializedMinion.Get().gameObject);
		}
		this.serializedMinions.RemoveAt(minionIndex);
	}

	public List<MinionStorage.Info> GetStoredMinionInfo()
	{
		return this.serializedMinions;
	}

	public void SetStoredMinionInfo(List<MinionStorage.Info> info)
	{
		this.serializedMinions = info;
	}

	[Serialize]
	private List<MinionStorage.Info> serializedMinions = new List<MinionStorage.Info>();

	public struct Info
	{
		public Info(string name, Ref<KPrefabID> ref_obj)
		{
			this.id = Guid.NewGuid();
			this.name = name;
			this.serializedMinion = ref_obj;
		}

		public static MinionStorage.Info CreateEmpty()
		{
			return new MinionStorage.Info
			{
				id = Guid.Empty,
				name = null,
				serializedMinion = null
			};
		}

		public Guid id;

		public string name;

		public Ref<KPrefabID> serializedMinion;
	}
}
