using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Immigration")]
public class Immigration : KMonoBehaviour, ISaveLoadable, ISim200ms, IPersonalPriorityManager
{
	public static void DestroyInstance()
	{
		Immigration.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		this.bImmigrantAvailable = false;
		Immigration.Instance = this;
		int num = Math.Min(this.spawnIdx, this.spawnInterval.Length - 1);
		this.timeBeforeSpawn = this.spawnInterval[num];
		this.ResetPersonalPriorities();
		this.ConfigureCarePackages();
	}

	private void ConfigureCarePackages()
	{
		if (DlcManager.FeatureClusterSpaceEnabled())
		{
			this.ConfigureMultiWorldCarePackages();
			return;
		}
		this.ConfigureBaseGameCarePackages();
	}

	private void ConfigureBaseGameCarePackages()
	{
		this.carePackages = new CarePackageInfo[]
		{
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.SandStone).tag.ToString(), 1000f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Dirt).tag.ToString(), 500f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Algae).tag.ToString(), 500f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.OxyRock).tag.ToString(), 100f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Water).tag.ToString(), 2000f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Sand).tag.ToString(), 3000f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Carbon).tag.ToString(), 3000f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Fertilizer).tag.ToString(), 3000f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Ice).tag.ToString(), 4000f, () => this.CycleCondition(12)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Brine).tag.ToString(), 2000f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.SaltWater).tag.ToString(), 2000f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Rust).tag.ToString(), 1000f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Cuprite).tag.ToString(), 2000f, () => this.CycleCondition(12) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Cuprite).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.GoldAmalgam).tag.ToString(), 2000f, () => this.CycleCondition(12) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.GoldAmalgam).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Copper).tag.ToString(), 400f, () => this.CycleCondition(24) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Copper).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Iron).tag.ToString(), 400f, () => this.CycleCondition(24) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Iron).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Lime).tag.ToString(), 150f, () => this.CycleCondition(48) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Lime).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Polypropylene).tag.ToString(), 500f, () => this.CycleCondition(48) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Polypropylene).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Glass).tag.ToString(), 200f, () => this.CycleCondition(48) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Glass).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Steel).tag.ToString(), 100f, () => this.CycleCondition(48) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Steel).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Ethanol).tag.ToString(), 100f, () => this.CycleCondition(48) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Ethanol).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.AluminumOre).tag.ToString(), 100f, () => this.CycleCondition(48) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.AluminumOre).tag)),
			new CarePackageInfo("PrickleGrassSeed", 3f, null),
			new CarePackageInfo("LeafyPlantSeed", 3f, null),
			new CarePackageInfo("CactusPlantSeed", 3f, null),
			new CarePackageInfo("MushroomSeed", 1f, null),
			new CarePackageInfo("PrickleFlowerSeed", 2f, null),
			new CarePackageInfo("OxyfernSeed", 1f, null),
			new CarePackageInfo("ForestTreeSeed", 1f, null),
			new CarePackageInfo(BasicFabricMaterialPlantConfig.SEED_ID, 3f, () => this.CycleCondition(24)),
			new CarePackageInfo("SwampLilySeed", 1f, () => this.CycleCondition(24)),
			new CarePackageInfo("ColdBreatherSeed", 1f, () => this.CycleCondition(24)),
			new CarePackageInfo("SpiceVineSeed", 1f, () => this.CycleCondition(24)),
			new CarePackageInfo("FieldRation", 5f, null),
			new CarePackageInfo("BasicForagePlant", 6f, null),
			new CarePackageInfo("CookedEgg", 3f, () => this.CycleCondition(6)),
			new CarePackageInfo(PrickleFruitConfig.ID, 3f, () => this.CycleCondition(12)),
			new CarePackageInfo("FriedMushroom", 3f, () => this.CycleCondition(24)),
			new CarePackageInfo("CookedMeat", 3f, () => this.CycleCondition(48)),
			new CarePackageInfo("SpicyTofu", 3f, () => this.CycleCondition(48)),
			new CarePackageInfo("LightBugBaby", 1f, null),
			new CarePackageInfo("HatchBaby", 1f, null),
			new CarePackageInfo("PuftBaby", 1f, null),
			new CarePackageInfo("SquirrelBaby", 1f, null),
			new CarePackageInfo("CrabBaby", 1f, null),
			new CarePackageInfo("DreckoBaby", 1f, () => this.CycleCondition(24)),
			new CarePackageInfo("Pacu", 8f, () => this.CycleCondition(24)),
			new CarePackageInfo("MoleBaby", 1f, () => this.CycleCondition(48)),
			new CarePackageInfo("OilfloaterBaby", 1f, () => this.CycleCondition(48)),
			new CarePackageInfo("LightBugEgg", 3f, null),
			new CarePackageInfo("HatchEgg", 3f, null),
			new CarePackageInfo("PuftEgg", 3f, null),
			new CarePackageInfo("OilfloaterEgg", 3f, () => this.CycleCondition(12)),
			new CarePackageInfo("MoleEgg", 3f, () => this.CycleCondition(24)),
			new CarePackageInfo("DreckoEgg", 3f, () => this.CycleCondition(24)),
			new CarePackageInfo("SquirrelEgg", 2f, null),
			new CarePackageInfo("BasicCure", 3f, null),
			new CarePackageInfo("Funky_Vest", 1f, null)
		};
	}

	private void ConfigureMultiWorldCarePackages()
	{
		this.carePackages = new CarePackageInfo[]
		{
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.SandStone).tag.ToString(), 1000f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Dirt).tag.ToString(), 500f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Algae).tag.ToString(), 500f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.OxyRock).tag.ToString(), 100f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Water).tag.ToString(), 2000f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Sand).tag.ToString(), 3000f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Carbon).tag.ToString(), 3000f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Fertilizer).tag.ToString(), 3000f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Ice).tag.ToString(), 4000f, () => this.CycleCondition(12)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Brine).tag.ToString(), 2000f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.SaltWater).tag.ToString(), 2000f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Rust).tag.ToString(), 1000f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Cuprite).tag.ToString(), 2000f, () => this.CycleCondition(12) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Cuprite).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.GoldAmalgam).tag.ToString(), 2000f, () => this.CycleCondition(12) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.GoldAmalgam).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Copper).tag.ToString(), 400f, () => this.CycleCondition(24) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Copper).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Iron).tag.ToString(), 400f, () => this.CycleCondition(24) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Iron).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Lime).tag.ToString(), 150f, () => this.CycleCondition(48) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Lime).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Polypropylene).tag.ToString(), 500f, () => this.CycleCondition(48) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Polypropylene).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Glass).tag.ToString(), 200f, () => this.CycleCondition(48) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Glass).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Steel).tag.ToString(), 100f, () => this.CycleCondition(48) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Steel).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Ethanol).tag.ToString(), 100f, () => this.CycleCondition(48) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Ethanol).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.AluminumOre).tag.ToString(), 100f, () => this.CycleCondition(48) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.AluminumOre).tag)),
			new CarePackageInfo("PrickleGrassSeed", 3f, null),
			new CarePackageInfo("LeafyPlantSeed", 3f, null),
			new CarePackageInfo("CactusPlantSeed", 3f, null),
			new CarePackageInfo("MushroomSeed", 1f, () => this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.SlimeMold).tag)),
			new CarePackageInfo("PrickleFlowerSeed", 2f, () => this.DiscoveredCondition("PrickleFlowerSeed")),
			new CarePackageInfo("OxyfernSeed", 1f, null),
			new CarePackageInfo("ForestTreeSeed", 1f, () => this.DiscoveredCondition("ForestTreeSeed")),
			new CarePackageInfo(BasicFabricMaterialPlantConfig.SEED_ID, 3f, () => this.CycleCondition(24) && this.DiscoveredCondition(BasicFabricMaterialPlantConfig.SEED_ID)),
			new CarePackageInfo("SwampLilySeed", 1f, () => this.CycleCondition(24) && this.DiscoveredCondition("SwampLilySeed")),
			new CarePackageInfo("ColdBreatherSeed", 1f, () => this.CycleCondition(24) && this.DiscoveredCondition("ColdBreatherSeed")),
			new CarePackageInfo("SpiceVineSeed", 1f, () => this.CycleCondition(24) && this.DiscoveredCondition("SpiceVineSeed")),
			new CarePackageInfo("FieldRation", 5f, null),
			new CarePackageInfo("BasicForagePlant", 6f, () => this.DiscoveredCondition("BasicForagePlant")),
			new CarePackageInfo("ForestForagePlant", 2f, () => this.DiscoveredCondition("ForestForagePlant")),
			new CarePackageInfo("SwampForagePlant", 2f, () => this.DiscoveredCondition("SwampForagePlant")),
			new CarePackageInfo("CookedEgg", 3f, () => this.CycleCondition(6)),
			new CarePackageInfo(PrickleFruitConfig.ID, 3f, () => this.CycleCondition(12) && this.DiscoveredCondition(PrickleFruitConfig.ID)),
			new CarePackageInfo("FriedMushroom", 3f, () => this.CycleCondition(24) && this.DiscoveredCondition("FriedMushroom")),
			new CarePackageInfo("CookedMeat", 3f, () => this.CycleCondition(48)),
			new CarePackageInfo("SpicyTofu", 3f, () => this.CycleCondition(48) && this.DiscoveredCondition("SpicyTofu")),
			new CarePackageInfo("WormSuperFood", 2f, () => this.DiscoveredCondition("WormPlantSeed")),
			new CarePackageInfo("LightBugBaby", 1f, () => this.DiscoveredCondition("LightBugEgg")),
			new CarePackageInfo("HatchBaby", 1f, () => this.DiscoveredCondition("HatchEgg")),
			new CarePackageInfo("PuftBaby", 1f, () => this.DiscoveredCondition("PuftEgg")),
			new CarePackageInfo("SquirrelBaby", 1f, () => this.DiscoveredCondition("SquirrelEgg")),
			new CarePackageInfo("CrabBaby", 1f, () => this.DiscoveredCondition("CrabEgg")),
			new CarePackageInfo("DreckoBaby", 1f, () => this.CycleCondition(24) && this.DiscoveredCondition("DreckoEgg")),
			new CarePackageInfo("Pacu", 8f, () => this.CycleCondition(24) && this.DiscoveredCondition("PacuEgg")),
			new CarePackageInfo("MoleBaby", 1f, () => this.CycleCondition(48) && this.DiscoveredCondition("MoleEgg")),
			new CarePackageInfo("OilfloaterBaby", 1f, () => this.CycleCondition(48) && this.DiscoveredCondition("OilfloaterEgg")),
			new CarePackageInfo("DivergentBeetleBaby", 1f, () => this.CycleCondition(48) && this.DiscoveredCondition("DivergentBeetleEgg")),
			new CarePackageInfo("StaterpillarBaby", 1f, () => this.CycleCondition(48) && this.DiscoveredCondition("StaterpillarEgg")),
			new CarePackageInfo("LightBugEgg", 3f, () => this.DiscoveredCondition("LightBugEgg")),
			new CarePackageInfo("HatchEgg", 3f, () => this.DiscoveredCondition("HatchEgg")),
			new CarePackageInfo("PuftEgg", 3f, () => this.DiscoveredCondition("PuftEgg")),
			new CarePackageInfo("OilfloaterEgg", 3f, () => this.CycleCondition(12) && this.DiscoveredCondition("OilfloaterEgg")),
			new CarePackageInfo("MoleEgg", 3f, () => this.CycleCondition(24) && this.DiscoveredCondition("MoleEgg")),
			new CarePackageInfo("DreckoEgg", 3f, () => this.CycleCondition(24) && this.DiscoveredCondition("DreckoEgg")),
			new CarePackageInfo("SquirrelEgg", 2f, () => this.DiscoveredCondition("SquirrelEgg")),
			new CarePackageInfo("DivergentBeetleEgg", 2f, () => this.CycleCondition(48) && this.DiscoveredCondition("DivergentBeetleEgg")),
			new CarePackageInfo("StaterpillarEgg", 2f, () => this.CycleCondition(48) && this.DiscoveredCondition("StaterpillarEgg")),
			new CarePackageInfo("BasicCure", 3f, null),
			new CarePackageInfo("Funky_Vest", 1f, null)
		};
	}

	private bool CycleCondition(int cycle)
	{
		return GameClock.Instance.GetCycle() >= cycle;
	}

	private bool DiscoveredCondition(Tag tag)
	{
		return DiscoveredResources.Instance.IsDiscovered(tag);
	}

	public bool ImmigrantsAvailable
	{
		get
		{
			return this.bImmigrantAvailable;
		}
	}

	public int EndImmigration()
	{
		this.bImmigrantAvailable = false;
		this.spawnIdx++;
		int num = Math.Min(this.spawnIdx, this.spawnInterval.Length - 1);
		this.timeBeforeSpawn = this.spawnInterval[num];
		return this.spawnTable[num];
	}

	public float GetTimeRemaining()
	{
		return this.timeBeforeSpawn;
	}

	public float GetTotalWaitTime()
	{
		int num = Math.Min(this.spawnIdx, this.spawnInterval.Length - 1);
		return this.spawnInterval[num];
	}

	public void Sim200ms(float dt)
	{
		if (this.IsHalted() || this.bImmigrantAvailable)
		{
			return;
		}
		this.timeBeforeSpawn -= dt;
		this.timeBeforeSpawn = Math.Max(this.timeBeforeSpawn, 0f);
		if (this.timeBeforeSpawn <= 0f)
		{
			this.bImmigrantAvailable = true;
		}
	}

	private bool IsHalted()
	{
		foreach (Telepad telepad in Components.Telepads.Items)
		{
			Operational component = telepad.GetComponent<Operational>();
			if (component != null && component.IsOperational)
			{
				return false;
			}
		}
		return true;
	}

	public int GetPersonalPriority(ChoreGroup group)
	{
		int num;
		if (!this.defaultPersonalPriorities.TryGetValue(group.IdHash, out num))
		{
			num = 3;
		}
		return num;
	}

	public CarePackageInfo RandomCarePackage()
	{
		List<CarePackageInfo> list = new List<CarePackageInfo>();
		foreach (CarePackageInfo carePackageInfo in this.carePackages)
		{
			if (carePackageInfo.requirement == null || carePackageInfo.requirement())
			{
				list.Add(carePackageInfo);
			}
		}
		return list[global::UnityEngine.Random.Range(0, list.Count)];
	}

	public void SetPersonalPriority(ChoreGroup group, int value)
	{
		this.defaultPersonalPriorities[group.IdHash] = value;
	}

	public int GetAssociatedSkillLevel(ChoreGroup group)
	{
		return 0;
	}

	public void ApplyDefaultPersonalPriorities(GameObject minion)
	{
		IPersonalPriorityManager instance = Immigration.Instance;
		IPersonalPriorityManager component = minion.GetComponent<ChoreConsumer>();
		foreach (ChoreGroup choreGroup in Db.Get().ChoreGroups.resources)
		{
			int personalPriority = instance.GetPersonalPriority(choreGroup);
			component.SetPersonalPriority(choreGroup, personalPriority);
		}
	}

	public void ResetPersonalPriorities()
	{
		bool advancedPersonalPriorities = Game.Instance.advancedPersonalPriorities;
		foreach (ChoreGroup choreGroup in Db.Get().ChoreGroups.resources)
		{
			this.defaultPersonalPriorities[choreGroup.IdHash] = (advancedPersonalPriorities ? choreGroup.DefaultPersonalPriority : 3);
		}
	}

	public bool IsChoreGroupDisabled(ChoreGroup g)
	{
		return false;
	}

	public float[] spawnInterval;

	public int[] spawnTable;

	[Serialize]
	private Dictionary<HashedString, int> defaultPersonalPriorities = new Dictionary<HashedString, int>();

	[Serialize]
	public float timeBeforeSpawn = float.PositiveInfinity;

	[Serialize]
	private bool bImmigrantAvailable;

	[Serialize]
	private int spawnIdx;

	private CarePackageInfo[] carePackages;

	public static Immigration Instance;

	private const int CYCLE_THRESHOLD_A = 6;

	private const int CYCLE_THRESHOLD_B = 12;

	private const int CYCLE_THRESHOLD_C = 24;

	private const int CYCLE_THRESHOLD_D = 48;
}
