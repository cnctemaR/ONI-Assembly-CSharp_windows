using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Database;
using Klei;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class Db : EntityModifierSet
{
	public static string GetPath(string dlcId, string folder)
	{
		string text;
		if (dlcId == "")
		{
			text = FileSystem.Normalize(Path.Combine(Application.streamingAssetsPath, folder));
		}
		else
		{
			string contentDirectoryName = DlcManager.GetContentDirectoryName(dlcId);
			text = FileSystem.Normalize(Path.Combine(Application.streamingAssetsPath, "dlc", contentDirectoryName, folder));
		}
		return text;
	}

	public static Db Get()
	{
		if (Db._Instance == null)
		{
			Db._Instance = Resources.Load<Db>("Db");
			Db._Instance.Initialize();
		}
		return Db._Instance;
	}

	public static ArtableStages GetArtableStages()
	{
		return Db.Get().ArtableStages;
	}

	public static EquippableFacades GetEquippableFacades()
	{
		return Db.Get().EquippableFacades;
	}

	public static StickerBombs GetStickerBombs()
	{
		return Db.Get().StickerBombs;
	}

	public override void Initialize()
	{
		base.Initialize();
		this.Urges = new Urges();
		this.AssignableSlots = new AssignableSlots();
		this.StateMachineCategories = new StateMachineCategories();
		this.Personalities = new Personalities();
		this.Faces = new Faces();
		this.Shirts = new Shirts();
		this.Expressions = new Expressions(this.Root);
		this.Emotes = new Emotes(this.Root);
		this.Thoughts = new Thoughts(this.Root);
		this.Dreams = new Dreams(this.Root);
		this.Deaths = new Deaths(this.Root);
		this.StatusItemCategories = new StatusItemCategories(this.Root);
		this.TechTreeTitles = new TechTreeTitles(this.Root);
		this.TechTreeTitles.Load(DlcManager.IsExpansion1Active() ? this.researchTreeFileExpansion1 : this.researchTreeFileVanilla);
		this.Techs = new Techs(this.Root);
		this.TechItems = new TechItems(this.Root);
		this.Techs.Init();
		this.Techs.Load(DlcManager.IsExpansion1Active() ? this.researchTreeFileExpansion1 : this.researchTreeFileVanilla);
		this.TechItems.Init();
		this.Accessories = new Accessories(this.Root);
		this.AccessorySlots = new AccessorySlots(this.Root);
		this.ScheduleBlockTypes = new ScheduleBlockTypes(this.Root);
		this.ScheduleGroups = new ScheduleGroups(this.Root);
		this.RoomTypeCategories = new RoomTypeCategories(this.Root);
		this.RoomTypes = new RoomTypes(this.Root);
		this.ArtifactDropRates = new ArtifactDropRates(this.Root);
		this.SpaceDestinationTypes = new SpaceDestinationTypes(this.Root);
		this.Diseases = new Diseases(this.Root, false);
		this.Sicknesses = new global::Database.Sicknesses(this.Root);
		this.SkillPerks = new SkillPerks(this.Root);
		this.SkillGroups = new SkillGroups(this.Root);
		this.Skills = new Skills(this.Root);
		this.ColonyAchievements = new ColonyAchievements(this.Root);
		this.MiscStatusItems = new MiscStatusItems(this.Root);
		this.CreatureStatusItems = new CreatureStatusItems(this.Root);
		this.BuildingStatusItems = new BuildingStatusItems(this.Root);
		this.RobotStatusItems = new RobotStatusItems(this.Root);
		this.ChoreTypes = new ChoreTypes(this.Root);
		this.GameplayEvents = new GameplayEvents(this.Root);
		this.GameplaySeasons = new GameplaySeasons(this.Root);
		this.Stories = new Stories(this.Root);
		if (DlcManager.FeaturePlantMutationsEnabled())
		{
			this.PlantMutations = new PlantMutations(this.Root);
		}
		this.OrbitalTypeCategories = new OrbitalTypeCategories(this.Root);
		this.ArtableStatuses = new ArtableStatuses(this.Root);
		this.EquippableFacades = new EquippableFacades(this.Root);
		this.ArtableStages = new ArtableStages(this.Root);
		this.StickerBombs = new StickerBombs(this.Root);
		Effect effect = new Effect("CenterOfAttention", DUPLICANTS.MODIFIERS.CENTEROFATTENTION.NAME, DUPLICANTS.MODIFIERS.CENTEROFATTENTION.TOOLTIP, 0f, true, true, false, null, -1f, 0f, null, "");
		effect.Add(new AttributeModifier("StressDelta", -0.008333334f, DUPLICANTS.MODIFIERS.CENTEROFATTENTION.NAME, false, false, true));
		this.effects.Add(effect);
		this.Spices = new Spices(this.Root);
		this.CollectResources(this.Root, this.ResourceTable);
	}

	public void PostProcess()
	{
		this.Techs.PostProcess();
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
			string text = "Could not find resource: ";
			ResourceGuid guid2 = guid;
			global::Debug.LogWarning(text + ((guid2 != null) ? guid2.ToString() : null));
			return default(ResourceType);
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
			}));
			return default(ResourceType);
		}
		return resourceType;
	}

	private static Db _Instance;

	public TextAsset researchTreeFileVanilla;

	public TextAsset researchTreeFileExpansion1;

	public Diseases Diseases;

	public global::Database.Sicknesses Sicknesses;

	public Urges Urges;

	public AssignableSlots AssignableSlots;

	public StateMachineCategories StateMachineCategories;

	public Personalities Personalities;

	public Faces Faces;

	public Shirts Shirts;

	public Expressions Expressions;

	public Emotes Emotes;

	public Thoughts Thoughts;

	public Dreams Dreams;

	public BuildingStatusItems BuildingStatusItems;

	public MiscStatusItems MiscStatusItems;

	public CreatureStatusItems CreatureStatusItems;

	public RobotStatusItems RobotStatusItems;

	public StatusItemCategories StatusItemCategories;

	public Deaths Deaths;

	public ChoreTypes ChoreTypes;

	public TechItems TechItems;

	public AccessorySlots AccessorySlots;

	public Accessories Accessories;

	public ScheduleBlockTypes ScheduleBlockTypes;

	public ScheduleGroups ScheduleGroups;

	public RoomTypeCategories RoomTypeCategories;

	public RoomTypes RoomTypes;

	public ArtifactDropRates ArtifactDropRates;

	public SpaceDestinationTypes SpaceDestinationTypes;

	public SkillPerks SkillPerks;

	public SkillGroups SkillGroups;

	public Skills Skills;

	public ColonyAchievements ColonyAchievements;

	public GameplayEvents GameplayEvents;

	public GameplaySeasons GameplaySeasons;

	public PlantMutations PlantMutations;

	public Spices Spices;

	public Techs Techs;

	public TechTreeTitles TechTreeTitles;

	public OrbitalTypeCategories OrbitalTypeCategories;

	public EquippableFacades EquippableFacades;

	public ArtableStages ArtableStages;

	public StickerBombs StickerBombs;

	public ArtableStatuses ArtableStatuses;

	public Stories Stories;

	[Serializable]
	public class SlotInfo : Resource
	{
	}
}
