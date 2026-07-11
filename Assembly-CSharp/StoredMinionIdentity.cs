using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class StoredMinionIdentity : KMonoBehaviour, ISaveLoadable, IAssignableIdentity, IListableOption
{
	[Serialize]
	public string genderStringKey { get; set; }

	[Serialize]
	public string nameStringKey { get; set; }

	protected override void OnPrefabInit()
	{
		Ownables component = base.GetComponent<Ownables>();
		Equipment component2 = base.GetComponent<Equipment>();
		foreach (AssignableSlot assignableSlot in Db.Get().AssignableSlots.resources)
		{
			if (assignableSlot is OwnableSlot)
			{
				OwnableSlotInstance ownableSlotInstance = new OwnableSlotInstance(component, (OwnableSlot)assignableSlot);
				component.Add(ownableSlotInstance);
			}
			else if (assignableSlot is EquipmentSlot)
			{
				EquipmentSlotInstance equipmentSlotInstance = new EquipmentSlotInstance(component2, (EquipmentSlot)assignableSlot);
				component2.Add(equipmentSlotInstance);
			}
		}
		this.ownablesList = new List<Ownables> { component };
	}

	protected override void OnSpawn()
	{
	}

	public string GetProperName()
	{
		return this.storedName;
	}

	public List<Ownables> GetOwners()
	{
		return this.ownablesList;
	}

	public Ownables GetSoleOwner()
	{
		return base.GetComponent<Ownables>();
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

	private List<Ownables> ownablesList;

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
