using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Database;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class StoredMinionIdentity : KMonoBehaviour, ISaveLoadable, IAssignableIdentity, IListableOption, IPersonalPriorityManager
{
	[Serialize]
	public string genderStringKey { get; set; }

	[Serialize]
	public string nameStringKey { get; set; }

	[OnDeserialized]
	private void OnDeserializedMethod()
	{
		if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 7))
		{
			int num = 0;
			foreach (KeyValuePair<string, bool> keyValuePair in this.MasteryByRoleID)
			{
				if (keyValuePair.Value && keyValuePair.Key != "NoRole")
				{
					num++;
				}
			}
			this.TotalExperienceGained = MinionResume.CalculatePreviousExperienceBar(num);
			foreach (KeyValuePair<HashedString, float> keyValuePair2 in this.AptitudeByRoleGroup)
			{
				this.AptitudeBySkillGroup[keyValuePair2.Key] = keyValuePair2.Value;
			}
		}
		this.OnDeserializeModifiers();
	}

	public bool HasPerk(SkillPerk perk)
	{
		foreach (KeyValuePair<string, bool> keyValuePair in this.MasteryBySkillID)
		{
			if (keyValuePair.Value && Db.Get().Skills.Get(keyValuePair.Key).perks.Contains(perk))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasMasteredSkill(string skillId)
	{
		return this.MasteryBySkillID.ContainsKey(skillId) && this.MasteryBySkillID[skillId];
	}

	protected override void OnPrefabInit()
	{
		this.assignableProxy = new Ref<MinionAssignablesProxy>();
		this.minionModifiers = base.GetComponent<MinionModifiers>();
		this.savedAttributeValues = new Dictionary<string, float>();
	}

	[OnSerializing]
	private void OnSerialize()
	{
		this.savedAttributeValues.Clear();
		foreach (AttributeInstance attributeInstance in this.minionModifiers.attributes)
		{
			this.savedAttributeValues.Add(attributeInstance.Attribute.Id, attributeInstance.GetTotalValue());
		}
	}

	protected override void OnSpawn()
	{
		MinionConfig.AddMinionAmounts(this.minionModifiers);
		MinionConfig.AddMinionTraits(DUPLICANTS.MODIFIERS.BASEDUPLICANT.NAME, this.minionModifiers);
		this.ValidateProxy();
		this.CleanupLimboMinions();
	}

	public void OnHardDelete()
	{
		if (this.assignableProxy.Get() != null)
		{
			Util.KDestroyGameObject(this.assignableProxy.Get().gameObject);
		}
		ScheduleManager.Instance.OnStoredDupeDestroyed(this);
		Components.StoredMinionIdentities.Remove(this);
	}

	private void OnDeserializeModifiers()
	{
		foreach (KeyValuePair<string, float> keyValuePair in this.savedAttributeValues)
		{
			Klei.AI.Attribute attribute = Db.Get().Attributes.TryGet(keyValuePair.Key);
			if (attribute == null)
			{
				attribute = Db.Get().BuildingAttributes.TryGet(keyValuePair.Key);
			}
			if (attribute != null)
			{
				if (this.minionModifiers.attributes.Get(attribute.Id) != null)
				{
					this.minionModifiers.attributes.Get(attribute.Id).Modifiers.Clear();
					this.minionModifiers.attributes.Get(attribute.Id).ClearModifiers();
				}
				else
				{
					this.minionModifiers.attributes.Add(attribute);
				}
				this.minionModifiers.attributes.Add(new AttributeModifier(attribute.Id, keyValuePair.Value, () => DUPLICANTS.ATTRIBUTES.STORED_VALUE, false, false));
			}
		}
	}

	public void ValidateProxy()
	{
		this.assignableProxy = MinionAssignablesProxy.InitAssignableProxy(this.assignableProxy, this);
	}

	private void CleanupLimboMinions()
	{
		KPrefabID component = base.GetComponent<KPrefabID>();
		bool flag = false;
		if (component.InstanceID == -1)
		{
			DebugUtil.LogWarningArgs(new object[] { "Stored minion with an invalid kpid! Attempting to recover...", this.storedName });
			flag = true;
			if (KPrefabIDTracker.Get().GetInstance(component.InstanceID) != null)
			{
				KPrefabIDTracker.Get().Unregister(component);
			}
			component.InstanceID = KPrefabID.GetUniqueID();
			KPrefabIDTracker.Get().Register(component);
			DebugUtil.LogWarningArgs(new object[] { "Restored as:", component.InstanceID });
		}
		if (component.conflicted)
		{
			DebugUtil.LogWarningArgs(new object[] { "Minion with a conflicted kpid! Attempting to recover... ", component.InstanceID, this.storedName });
			if (KPrefabIDTracker.Get().GetInstance(component.InstanceID) != null)
			{
				KPrefabIDTracker.Get().Unregister(component);
			}
			component.InstanceID = KPrefabID.GetUniqueID();
			KPrefabIDTracker.Get().Register(component);
			DebugUtil.LogWarningArgs(new object[] { "Restored as:", component.InstanceID });
		}
		this.assignableProxy.Get().SetTarget(this, base.gameObject);
		bool flag2 = false;
		foreach (MinionStorage minionStorage in Components.MinionStorages.Items)
		{
			List<MinionStorage.Info> storedMinionInfo = minionStorage.GetStoredMinionInfo();
			for (int i = 0; i < storedMinionInfo.Count; i++)
			{
				MinionStorage.Info info = storedMinionInfo[i];
				if (flag && info.serializedMinion != null && info.serializedMinion.GetId() == -1 && info.name == this.storedName)
				{
					DebugUtil.LogWarningArgs(new object[]
					{
						"Found a minion storage with an invalid ref, rebinding.",
						component.InstanceID,
						this.storedName,
						minionStorage.gameObject.name
					});
					info = new MinionStorage.Info(this.storedName, new Ref<KPrefabID>(component));
					storedMinionInfo[i] = info;
					minionStorage.GetComponent<Assignable>().Assign(this);
					flag2 = true;
					break;
				}
				if (info.serializedMinion != null && info.serializedMinion.Get() == component)
				{
					flag2 = true;
					break;
				}
			}
			if (flag2)
			{
				break;
			}
		}
		if (!flag2)
		{
			DebugUtil.LogWarningArgs(new object[] { "Found a stored minion that wasn't in any minion storage. Respawning them at the portal.", component.InstanceID, this.storedName });
			GameObject telepad = GameUtil.GetTelepad();
			if (telepad != null)
			{
				MinionStorage.DeserializeMinion(component.gameObject, telepad.transform.GetPosition());
			}
		}
	}

	public string GetProperName()
	{
		return this.storedName;
	}

	public List<Ownables> GetOwners()
	{
		return this.assignableProxy.Get().ownables;
	}

	public Ownables GetSoleOwner()
	{
		return this.assignableProxy.Get().GetComponent<Ownables>();
	}

	public Accessory GetAccessory(AccessorySlot slot)
	{
		for (int i = 0; i < this.accessories.Count; i++)
		{
			if (this.accessories[i].Get() != null && this.accessories[i].Get().slot == slot)
			{
				return this.accessories[i].Get();
			}
		}
		return null;
	}

	public bool IsNull()
	{
		return this == null;
	}

	public string GetStorageReason()
	{
		KPrefabID component = base.GetComponent<KPrefabID>();
		foreach (MinionStorage minionStorage in Components.MinionStorages.Items)
		{
			using (List<MinionStorage.Info>.Enumerator enumerator2 = minionStorage.GetStoredMinionInfo().GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current.serializedMinion.Get() == component)
					{
						return minionStorage.GetProperName();
					}
				}
			}
		}
		return "";
	}

	public bool IsPermittedToConsume(string consumable)
	{
		using (List<Tag>.Enumerator enumerator = this.forbiddenTags.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current == consumable)
				{
					return false;
				}
			}
		}
		return true;
	}

	public bool IsChoreGroupDisabled(ChoreGroup chore_group)
	{
		foreach (string text in this.traitIDs)
		{
			if (Db.Get().traits.Exists(text))
			{
				Trait trait = Db.Get().traits.Get(text);
				if (trait.disabledChoreGroups != null)
				{
					ChoreGroup[] disabledChoreGroups = trait.disabledChoreGroups;
					for (int i = 0; i < disabledChoreGroups.Length; i++)
					{
						if (disabledChoreGroups[i].IdHash == chore_group.IdHash)
						{
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	public int GetPersonalPriority(ChoreGroup chore_group)
	{
		ChoreConsumer.PriorityInfo priorityInfo;
		if (this.choreGroupPriorities.TryGetValue(chore_group.IdHash, out priorityInfo))
		{
			return priorityInfo.priority;
		}
		return 0;
	}

	public int GetAssociatedSkillLevel(ChoreGroup group)
	{
		return 0;
	}

	public void SetPersonalPriority(ChoreGroup group, int value)
	{
	}

	public void ResetPersonalPriorities()
	{
	}

	[Serialize]
	public string storedName;

	[Serialize]
	public string gender;

	[Serialize]
	[ReadOnly]
	public float arrivalTime;

	[Serialize]
	public int voiceIdx;

	[Serialize]
	public KCompBuilder.BodyData bodyData;

	[Serialize]
	public List<Ref<KPrefabID>> assignedItems;

	[Serialize]
	public List<Ref<KPrefabID>> equippedItems;

	[Serialize]
	public List<string> traitIDs;

	[Serialize]
	public List<ResourceRef<Accessory>> accessories;

	[Serialize]
	public List<Tag> forbiddenTags;

	[Serialize]
	public Ref<MinionAssignablesProxy> assignableProxy;

	[Serialize]
	public Dictionary<string, bool> MasteryByRoleID = new Dictionary<string, bool>();

	[Serialize]
	public Dictionary<string, bool> MasteryBySkillID = new Dictionary<string, bool>();

	[Serialize]
	public Dictionary<HashedString, float> AptitudeByRoleGroup = new Dictionary<HashedString, float>();

	[Serialize]
	public Dictionary<HashedString, float> AptitudeBySkillGroup = new Dictionary<HashedString, float>();

	[Serialize]
	public float TotalExperienceGained;

	[Serialize]
	public string currentHat;

	[Serialize]
	public string targetHat;

	[Serialize]
	public Dictionary<HashedString, ChoreConsumer.PriorityInfo> choreGroupPriorities = new Dictionary<HashedString, ChoreConsumer.PriorityInfo>();

	[Serialize]
	public List<AttributeLevels.LevelSaveLoad> attributeLevels;

	[Serialize]
	public Dictionary<string, float> savedAttributeValues;

	public MinionModifiers minionModifiers;
}
