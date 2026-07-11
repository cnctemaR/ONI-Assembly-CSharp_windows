using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class StoredMinionIdentity : KMonoBehaviour, ISaveLoadable, IAssignableIdentity, IListableOption
{
	[Serialize]
	public string genderStringKey { get; set; }

	[Serialize]
	public string nameStringKey { get; set; }

	public bool HasPerk(RolePerk perk)
	{
		foreach (RoleConfig roleConfig in Game.Instance.roleManager.RolesConfigs)
		{
			if (roleConfig.HasPerk(perk) && this.MasteryByRoleID.ContainsKey(roleConfig.id) && this.MasteryByRoleID[roleConfig.id])
			{
				return true;
			}
		}
		return Game.Instance.roleManager.GetRole(this.currentRole) != null && Game.Instance.roleManager.GetRole(this.currentRole).HasPerk(perk);
	}

	protected override void OnPrefabInit()
	{
		this.assignableProxy = new Ref<MinionAssignablesProxy>();
	}

	protected override void OnSpawn()
	{
		this.ValidateProxy();
		this.CleanupLimboMinions();
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
			Output.LogWarning(new object[] { "Stored minion with an invalid kpid! Attempting to recover...", this.storedName });
			flag = true;
			if (KPrefabIDTracker.Get().GetInstance(component.InstanceID) != null)
			{
				KPrefabIDTracker.Get().Unregister(component);
			}
			component.InstanceID = KPrefabID.GetUniqueID();
			KPrefabIDTracker.Get().Register(component);
			Output.LogWarning(new object[] { "Restored as:", component.InstanceID });
		}
		if (component.conflicted)
		{
			Output.LogWarning(new object[] { "Minion with a conflicted kpid! Attempting to recover... ", component.InstanceID, this.storedName });
			if (KPrefabIDTracker.Get().GetInstance(component.InstanceID) != null)
			{
				KPrefabIDTracker.Get().Unregister(component);
			}
			component.InstanceID = KPrefabID.GetUniqueID();
			KPrefabIDTracker.Get().Register(component);
			Output.LogWarning(new object[] { "Restored as:", component.InstanceID });
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
					Output.LogWarning(new object[]
					{
						"Found a minion storage with an invalid ref, rebinding.",
						component.InstanceID,
						this.storedName,
						minionStorage.gameObject.name
					});
					info = new MinionStorage.Info(this.storedName, new Ref<KPrefabID>(component));
					storedMinionInfo[i] = info;
					Assignable component2 = minionStorage.GetComponent<Assignable>();
					component2.Assign(this);
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
			Output.LogWarning(new object[] { "Found a stored minion that wasn't in any minion storage. Respawning them at the portal.", component.InstanceID, this.storedName });
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

	public bool IsNull()
	{
		return this == null;
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
	public Dictionary<string, float> ExperienceByRoleID = new Dictionary<string, float>();

	[Serialize]
	public Dictionary<string, bool> MasteryByRoleID = new Dictionary<string, bool>();

	[Serialize]
	public Dictionary<HashedString, float> AptitudeByRoleGroup = new Dictionary<HashedString, float>();

	[Serialize]
	public string currentRole;

	[Serialize]
	public string targetRole;

	[Serialize]
	public Dictionary<HashedString, ChoreConsumer.PriorityInfo> choreGroupPriorities = new Dictionary<HashedString, ChoreConsumer.PriorityInfo>();

	[Serialize]
	public List<AttributeLevels.LevelSaveLoad> attributeLevels;
}
