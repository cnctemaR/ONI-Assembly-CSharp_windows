using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class Db : EntityModifierSet
{
	public static Db Get()
	{
		if (Db._Instance == null)
		{
			Db._Instance = Resources.Load<Db>("Db");
			Db._Instance.Initialize();
		}
		return Db._Instance;
	}

	public override void Initialize()
	{
		base.Initialize();
		this.Diseases = new global::Database.Diseases(this.Root);
		this.Urges = new Urges();
		this.OwnableSlots = new OwnableSlots();
		this.StateMachineCategories = new StateMachineCategories();
		this.Personalities = new Personalities(this.personalitiesFile);
		this.Faces = new Faces();
		this.Shirts = new Shirts();
		this.Expressions = new Expressions(this.Root);
		this.Thoughts = new Thoughts(this.Root);
		this.Deaths = new Deaths(this.Root);
		this.StatusItemCategories = new StatusItemCategories(this.Root);
		this.Techs = new Techs(this.Root);
		this.Techs.Load(this.researchTreeFile);
		this.TechItems = new TechItems(this.Root);
		this.Accessories = new Accessories(this.Root);
		this.AccessorySlots = new AccessorySlots(this.Root, null, null, null);
		this.ScheduleBlockTypes = new ScheduleBlockTypes(this.Root);
		this.Roles = new Roles(this.Root);
		this.MiscStatusItems = new MiscStatusItems(this.Root);
		this.CreatureStatusItems = new CreatureStatusItems(this.Root);
		this.BuildingStatusItems = new BuildingStatusItems(this.Root);
		this.ChoreTypes = new ChoreTypes(this.Root);
		Effect effect = new Effect("CenterOfAttention", DUPLICANTS.MODIFIERS.CENTEROFATTENTION.NAME, DUPLICANTS.MODIFIERS.CENTEROFATTENTION.TOOLTIP, 0f, true, true, false);
		effect.Add(new AttributeModifier("StressDelta", -0.008333334f, DUPLICANTS.MODIFIERS.CENTEROFATTENTION.NAME, false, false, true));
		this.effects.Add(effect);
		this.CollectResources(this.Root, this.ResourceTable);
	}

	private void CollectResources(Resource resource, List<Resource> resource_table)
	{
		if (resource.Guid != null)
		{
			resource_table.Add(resource);
		}
		ResourceSet resourceSet = resource as ResourceSet;
		if (resourceSet != null)
		{
			for (int i = 0; i < resourceSet.Count; i++)
			{
				this.CollectResources(resourceSet.GetResource(i), resource_table);
			}
		}
	}

	public ResourceType GetResource<ResourceType>(ResourceGuid guid) where ResourceType : Resource
	{
		Resource resource = this.ResourceTable.FirstOrDefault<Resource>((Resource s) => s.Guid == guid);
		if (resource == null)
		{
			global::Debug.LogWarning("Could not find resource: " + guid, null);
			return (ResourceType)((object)null);
		}
		ResourceType resourceType = (ResourceType)((object)resource);
		if (resourceType == null)
		{
			global::Debug.LogError(string.Concat(new string[]
			{
				"Resource type mismatch for resource: ",
				resource.Id,
				"\nExpecting Type: ",
				typeof(ResourceType).Name,
				"\nGot Type: ",
				resource.GetType().Name
			}), null);
			return (ResourceType)((object)null);
		}
		return resourceType;
	}

	private static Db _Instance;

	public TextAsset personalitiesFile;

	public TextAsset researchTreeFile;

	public global::Database.Diseases Diseases;

	public Urges Urges;

	public OwnableSlots OwnableSlots;

	public StateMachineCategories StateMachineCategories;

	public Personalities Personalities;

	public Faces Faces;

	public Shirts Shirts;

	public Expressions Expressions;

	public Thoughts Thoughts;

	public BuildingStatusItems BuildingStatusItems;

	public MiscStatusItems MiscStatusItems;

	public CreatureStatusItems CreatureStatusItems;

	public StatusItemCategories StatusItemCategories;

	public Deaths Deaths;

	public ChoreTypes ChoreTypes;

	public Techs Techs;

	public TechItems TechItems;

	public AccessorySlots AccessorySlots;

	public Accessories Accessories;

	public ScheduleBlockTypes ScheduleBlockTypes;

	public Roles Roles;

	[Serializable]
	public class SlotInfo : Resource
	{
	}
}
