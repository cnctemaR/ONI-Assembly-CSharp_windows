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
		this.SetupDLCCarePackages();
		this.ResetPersonalPriorities();
		this.ConfigureCarePackages();
	}

	private void SetupDLCCarePackages()
	{
		Dictionary<string, List<CarePackageInfo>> dictionary = new Dictionary<string, List<CarePackageInfo>>();
		string text = "DLC2_ID";
		List<CarePackageInfo> list = new List<CarePackageInfo>();
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Cinnabar).tag.ToString(), 2000f, () => Immigration.CycleCondition(12) && Immigration.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Cinnabar).tag)));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.WoodLog).tag.ToString(), 200f, () => Immigration.CycleCondition(24) && Immigration.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.WoodLog).tag)));
		list.Add(new CarePackageInfo("WoodDeerBaby", 1f, () => Immigration.CycleCondition(24)));
		list.Add(new CarePackageInfo("SealBaby", 1f, () => Immigration.CycleCondition(48)));
		list.Add(new CarePackageInfo("IceBellyEgg", 1f, () => Immigration.CycleCondition(100)));
		list.Add(new CarePackageInfo("Pemmican", 3f, null));
		list.Add(new CarePackageInfo("FriesCarrot", 3f, () => Immigration.CycleCondition(24)));
		list.Add(new CarePackageInfo("IceFlowerSeed", 3f, null));
		list.Add(new CarePackageInfo("BlueGrassSeed", 1f, null));
		list.Add(new CarePackageInfo("CarrotPlantSeed", 1f, () => Immigration.CycleCondition(24)));
		list.Add(new CarePackageInfo("SpaceTreeSeed", 1f, () => Immigration.CycleCondition(24)));
		list.Add(new CarePackageInfo("HardSkinBerryPlantSeed", 3f, null));
		dictionary.Add(text, list);
		dictionary.Add("DLC3_ID", new List<CarePackageInfo>
		{
			new CarePackageInfo("DisposableElectrobank_BasicSingleHarvestPlant", 5f, null),
			new CarePackageInfo("bionic_upgrade_" + BionicUpgradeComponentConfig.SUFFIX_CONSTRUCTION, 1f, null),
			new CarePackageInfo("bionic_upgrade_" + BionicUpgradeComponentConfig.SUFFIX_EXCAVATION, 1f, null),
			new CarePackageInfo("bionic_upgrade_" + BionicUpgradeComponentConfig.SUFFIX_MACHINERY, 1f, null),
			new CarePackageInfo("bionic_upgrade_" + BionicUpgradeComponentConfig.SUFFIX_ATHLETICS, 1f, null),
			new CarePackageInfo("bionic_upgrade_" + BionicUpgradeComponentConfig.SUFFIX_COOKING, 1f, null),
			new CarePackageInfo("bionic_upgrade_" + BionicUpgradeComponentConfig.SUFFIX_MEDICINE, 1f, null),
			new CarePackageInfo("bionic_upgrade_" + BionicUpgradeComponentConfig.SUFFIX_STRENGTH, 1f, null),
			new CarePackageInfo("bionic_upgrade_" + BionicUpgradeComponentConfig.SUFFIX_CREATIVITY, 1f, null),
			new CarePackageInfo("bionic_upgrade_" + BionicUpgradeComponentConfig.SUFFIX_AGRICULTURE, 1f, null),
			new CarePackageInfo("bionic_upgrade_" + BionicUpgradeComponentConfig.SUFFIX_HUSBANDRY, 1f, null),
			new CarePackageInfo("bionic_upgrade_" + BionicUpgradeComponentConfig.SUFFIX_SCIENCE, 1f, null),
			new CarePackageInfo("bionic_upgrade_" + BionicUpgradeComponentConfig.SUFFIX_PILOTING, 1f, null)
		});
		this.carePackagesByDlc = dictionary;
	}

	private void ConfigureCarePackages()
	{
		if (DlcManager.FeatureClusterSpaceEnabled())
		{
			this.ConfigureMultiWorldCarePackages();
		}
		else
		{
			this.ConfigureBaseGameCarePackages();
		}
		foreach (string text in SaveLoader.Instance.GameInfo.dlcIds)
		{
			if (this.carePackagesByDlc.ContainsKey(text))
			{
				this.carePackages.AddRange(this.carePackagesByDlc[text]);
			}
		}
	}

	private void ConfigureBaseGameCarePackages()
	{
		List<CarePackageInfo> list = new List<CarePackageInfo>();
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.SandStone).tag.ToString(), 1000f, null));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Dirt).tag.ToString(), 500f, null));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Algae).tag.ToString(), 500f, null));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.OxyRock).tag.ToString(), 100f, null));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Water).tag.ToString(), 2000f, null));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Sand).tag.ToString(), 3000f, null));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Carbon).tag.ToString(), 3000f, null));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Fertilizer).tag.ToString(), 3000f, null));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Ice).tag.ToString(), 4000f, () => Immigration.CycleCondition(12)));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Brine).tag.ToString(), 2000f, null));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.SaltWater).tag.ToString(), 2000f, null));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Rust).tag.ToString(), 1000f, null));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Cuprite).tag.ToString(), 2000f, () => Immigration.CycleCondition(12) && Immigration.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Cuprite).tag)));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.GoldAmalgam).tag.ToString(), 2000f, () => Immigration.CycleCondition(12) && Immigration.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.GoldAmalgam).tag)));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Copper).tag.ToString(), 400f, () => Immigration.CycleCondition(24) && Immigration.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Copper).tag)));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Iron).tag.ToString(), 400f, () => Immigration.CycleCondition(24) && Immigration.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Iron).tag)));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Lime).tag.ToString(), 150f, () => Immigration.CycleCondition(48) && Immigration.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Lime).tag)));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Polypropylene).tag.ToString(), 500f, () => Immigration.CycleCondition(48) && Immigration.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Polypropylene).tag)));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Glass).tag.ToString(), 200f, () => Immigration.CycleCondition(48) && Immigration.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Glass).tag)));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Steel).tag.ToString(), 100f, () => Immigration.CycleCondition(48) && Immigration.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Steel).tag)));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Ethanol).tag.ToString(), 100f, () => Immigration.CycleCondition(48) && Immigration.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Ethanol).tag)));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.AluminumOre).tag.ToString(), 100f, () => Immigration.CycleCondition(48) && Immigration.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.AluminumOre).tag)));
		list.Add(new CarePackageInfo("PrickleGrassSeed", 3f, null));
		list.Add(new CarePackageInfo("LeafyPlantSeed", 3f, null));
		list.Add(new CarePackageInfo("CactusPlantSeed", 3f, null));
		list.Add(new CarePackageInfo("MushroomSeed", 1f, null));
		list.Add(new CarePackageInfo("PrickleFlowerSeed", 2f, null));
		list.Add(new CarePackageInfo("OxyfernSeed", 1f, null));
		list.Add(new CarePackageInfo("ForestTreeSeed", 1f, null));
		list.Add(new CarePackageInfo(BasicFabricMaterialPlantConfig.SEED_ID, 3f, () => Immigration.CycleCondition(24)));
		list.Add(new CarePackageInfo("SwampLilySeed", 1f, () => Immigration.CycleCondition(24)));
		list.Add(new CarePackageInfo("ColdBreatherSeed", 1f, () => Immigration.CycleCondition(24)));
		list.Add(new CarePackageInfo("SpiceVineSeed", 1f, () => Immigration.CycleCondition(24)));
		list.Add(new CarePackageInfo("FieldRation", 5f, null));
		list.Add(new CarePackageInfo("BasicForagePlant", 6f, null));
		list.Add(new CarePackageInfo("CookedEgg", 3f, () => Immigration.CycleCondition(6)));
		list.Add(new CarePackageInfo(PrickleFruitConfig.ID, 3f, () => Immigration.CycleCondition(12)));
		list.Add(new CarePackageInfo("FriedMushroom", 3f, () => Immigration.CycleCondition(24)));
		list.Add(new CarePackageInfo("CookedMeat", 3f, () => Immigration.CycleCondition(48)));
		list.Add(new CarePackageInfo("SpicyTofu", 3f, () => Immigration.CycleCondition(48)));
		list.Add(new CarePackageInfo("LightBugBaby", 1f, null));
		list.Add(new CarePackageInfo("HatchBaby", 1f, null));
		list.Add(new CarePackageInfo("PuftBaby", 1f, null));
		list.Add(new CarePackageInfo("SquirrelBaby", 1f, null));
		list.Add(new CarePackageInfo("CrabBaby", 1f, null));
		list.Add(new CarePackageInfo("DreckoBaby", 1f, () => Immigration.CycleCondition(24)));
		list.Add(new CarePackageInfo("Pacu", 8f, () => Immigration.CycleCondition(24)));
		list.Add(new CarePackageInfo("MoleBaby", 1f, () => Immigration.CycleCondition(48)));
		list.Add(new CarePackageInfo("OilfloaterBaby", 1f, () => Immigration.CycleCondition(48)));
		list.Add(new CarePackageInfo("LightBugEgg", 3f, null));
		list.Add(new CarePackageInfo("HatchEgg", 3f, null));
		list.Add(new CarePackageInfo("PuftEgg", 3f, null));
		list.Add(new CarePackageInfo("OilfloaterEgg", 3f, () => Immigration.CycleCondition(12)));
		list.Add(new CarePackageInfo("MoleEgg", 3f, () => Immigration.CycleCondition(24)));
		list.Add(new CarePackageInfo("DreckoEgg", 3f, () => Immigration.CycleCondition(24)));
		list.Add(new CarePackageInfo("SquirrelEgg", 2f, null));
		list.Add(new CarePackageInfo("BasicCure", 3f, null));
		list.Add(new CarePackageInfo("CustomClothing", 1f, null, "SELECTRANDOM"));
		list.Add(new CarePackageInfo("Funky_Vest", 1f, null));
		this.carePackages = list;
	}

	private void ConfigureMultiWorldCarePackages()
	{
		List<CarePackageInfo> list = new List<CarePackageInfo>();
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.SandStone).tag.ToString(), 1000f, null));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Dirt).tag.ToString(), 500f, null));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Algae).tag.ToString(), 500f, null));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.OxyRock).tag.ToString(), 100f, null));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Water).tag.ToString(), 2000f, null));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Sand).tag.ToString(), 3000f, null));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Carbon).tag.ToString(), 3000f, null));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Fertilizer).tag.ToString(), 3000f, null));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Ice).tag.ToString(), 4000f, () => Immigration.CycleCondition(12)));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Brine).tag.ToString(), 2000f, null));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.SaltWater).tag.ToString(), 2000f, null));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Rust).tag.ToString(), 1000f, null));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Cuprite).tag.ToString(), 2000f, () => Immigration.CycleCondition(12) && Immigration.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Cuprite).tag)));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.GoldAmalgam).tag.ToString(), 2000f, () => Immigration.CycleCondition(12) && Immigration.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.GoldAmalgam).tag)));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Copper).tag.ToString(), 400f, () => Immigration.CycleCondition(24) && Immigration.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Copper).tag)));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Iron).tag.ToString(), 400f, () => Immigration.CycleCondition(24) && Immigration.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Iron).tag)));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Lime).tag.ToString(), 150f, () => Immigration.CycleCondition(48) && Immigration.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Lime).tag)));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Polypropylene).tag.ToString(), 500f, () => Immigration.CycleCondition(48) && Immigration.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Polypropylene).tag)));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Glass).tag.ToString(), 200f, () => Immigration.CycleCondition(48) && Immigration.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Glass).tag)));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Steel).tag.ToString(), 100f, () => Immigration.CycleCondition(48) && Immigration.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Steel).tag)));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Ethanol).tag.ToString(), 100f, () => Immigration.CycleCondition(48) && Immigration.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Ethanol).tag)));
		list.Add(new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.AluminumOre).tag.ToString(), 100f, () => Immigration.CycleCondition(48) && Immigration.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.AluminumOre).tag)));
		list.Add(new CarePackageInfo("PrickleGrassSeed", 3f, null));
		list.Add(new CarePackageInfo("LeafyPlantSeed", 3f, null));
		list.Add(new CarePackageInfo("CactusPlantSeed", 3f, null));
		list.Add(new CarePackageInfo("WineCupsSeed", 3f, null));
		list.Add(new CarePackageInfo("CylindricaSeed", 3f, null));
		list.Add(new CarePackageInfo("MushroomSeed", 1f, null));
		list.Add(new CarePackageInfo("PrickleFlowerSeed", 2f, () => Immigration.DiscoveredCondition("PrickleFlowerSeed") || Immigration.CycleCondition(500)));
		list.Add(new CarePackageInfo("OxyfernSeed", 1f, null));
		list.Add(new CarePackageInfo("ForestTreeSeed", 1f, () => Immigration.DiscoveredCondition("ForestTreeSeed") || Immigration.CycleCondition(500)));
		list.Add(new CarePackageInfo(BasicFabricMaterialPlantConfig.SEED_ID, 3f, () => Immigration.CycleCondition(24) && (Immigration.DiscoveredCondition(BasicFabricMaterialPlantConfig.SEED_ID) || Immigration.CycleCondition(500))));
		list.Add(new CarePackageInfo("SwampLilySeed", 1f, () => Immigration.CycleCondition(24) && (Immigration.DiscoveredCondition("SwampLilySeed") || Immigration.CycleCondition(500))));
		list.Add(new CarePackageInfo("ColdBreatherSeed", 1f, () => Immigration.CycleCondition(24) && (Immigration.DiscoveredCondition("ColdBreatherSeed") || Immigration.CycleCondition(500))));
		list.Add(new CarePackageInfo("SpiceVineSeed", 1f, () => Immigration.CycleCondition(24) && (Immigration.DiscoveredCondition("SpiceVineSeed") || Immigration.CycleCondition(500))));
		list.Add(new CarePackageInfo("WormPlantSeed", 1f, () => Immigration.CycleCondition(24) && (Immigration.DiscoveredCondition("WormPlantSeed") || Immigration.CycleCondition(500))));
		list.Add(new CarePackageInfo("FieldRation", 5f, null));
		list.Add(new CarePackageInfo("BasicForagePlant", 6f, () => Immigration.DiscoveredCondition("BasicForagePlant")));
		list.Add(new CarePackageInfo("ForestForagePlant", 2f, () => Immigration.DiscoveredCondition("ForestForagePlant")));
		list.Add(new CarePackageInfo("SwampForagePlant", 2f, () => Immigration.DiscoveredCondition("SwampForagePlant")));
		list.Add(new CarePackageInfo("CookedEgg", 3f, () => Immigration.CycleCondition(6)));
		list.Add(new CarePackageInfo(PrickleFruitConfig.ID, 3f, () => Immigration.CycleCondition(12) && (Immigration.DiscoveredCondition(PrickleFruitConfig.ID) || Immigration.CycleCondition(500))));
		list.Add(new CarePackageInfo("FriedMushroom", 3f, () => Immigration.CycleCondition(24)));
		list.Add(new CarePackageInfo("CookedMeat", 3f, () => Immigration.CycleCondition(48)));
		list.Add(new CarePackageInfo("SpicyTofu", 3f, () => Immigration.CycleCondition(48)));
		list.Add(new CarePackageInfo("WormSuperFood", 2f, () => Immigration.DiscoveredCondition("WormPlantSeed") || Immigration.CycleCondition(500)));
		list.Add(new CarePackageInfo("LightBugBaby", 1f, () => Immigration.DiscoveredCondition("LightBugEgg") || Immigration.CycleCondition(500)));
		list.Add(new CarePackageInfo("HatchBaby", 1f, () => Immigration.DiscoveredCondition("HatchEgg") || Immigration.CycleCondition(500)));
		list.Add(new CarePackageInfo("PuftBaby", 1f, () => Immigration.DiscoveredCondition("PuftEgg") || Immigration.CycleCondition(500)));
		list.Add(new CarePackageInfo("SquirrelBaby", 1f, () => Immigration.DiscoveredCondition("SquirrelEgg") || Immigration.CycleCondition(24) || Immigration.CycleCondition(500)));
		list.Add(new CarePackageInfo("CrabBaby", 1f, () => Immigration.DiscoveredCondition("CrabEgg") || Immigration.CycleCondition(500)));
		list.Add(new CarePackageInfo("DreckoBaby", 1f, () => Immigration.CycleCondition(24) && (Immigration.DiscoveredCondition("DreckoEgg") || Immigration.CycleCondition(500))));
		list.Add(new CarePackageInfo("Pacu", 8f, () => Immigration.CycleCondition(24) && (Immigration.DiscoveredCondition("PacuEgg") || Immigration.CycleCondition(500))));
		list.Add(new CarePackageInfo("MoleBaby", 1f, () => Immigration.CycleCondition(48) && (Immigration.DiscoveredCondition("MoleEgg") || Immigration.CycleCondition(500))));
		list.Add(new CarePackageInfo("OilfloaterBaby", 1f, () => Immigration.CycleCondition(48) && (Immigration.DiscoveredCondition("OilfloaterEgg") || Immigration.CycleCondition(500))));
		list.Add(new CarePackageInfo("DivergentBeetleBaby", 1f, () => Immigration.CycleCondition(48) && (Immigration.DiscoveredCondition("DivergentBeetleEgg") || Immigration.CycleCondition(500))));
		list.Add(new CarePackageInfo("StaterpillarBaby", 1f, () => Immigration.CycleCondition(48) && (Immigration.DiscoveredCondition("StaterpillarEgg") || Immigration.CycleCondition(500))));
		list.Add(new CarePackageInfo("LightBugEgg", 3f, () => Immigration.DiscoveredCondition("LightBugEgg") || Immigration.CycleCondition(500)));
		list.Add(new CarePackageInfo("HatchEgg", 3f, () => Immigration.DiscoveredCondition("HatchEgg") || Immigration.CycleCondition(500)));
		list.Add(new CarePackageInfo("PuftEgg", 3f, () => Immigration.DiscoveredCondition("PuftEgg") || Immigration.CycleCondition(500)));
		list.Add(new CarePackageInfo("OilfloaterEgg", 3f, () => Immigration.CycleCondition(12) && (Immigration.DiscoveredCondition("OilfloaterEgg") || Immigration.CycleCondition(500))));
		list.Add(new CarePackageInfo("MoleEgg", 3f, () => Immigration.CycleCondition(24) && (Immigration.DiscoveredCondition("MoleEgg") || Immigration.CycleCondition(500))));
		list.Add(new CarePackageInfo("DreckoEgg", 3f, () => Immigration.CycleCondition(24) && (Immigration.DiscoveredCondition("DreckoEgg") || Immigration.CycleCondition(500))));
		list.Add(new CarePackageInfo("SquirrelEgg", 2f, () => Immigration.DiscoveredCondition("SquirrelEgg") || Immigration.CycleCondition(24) || Immigration.CycleCondition(500)));
		list.Add(new CarePackageInfo("DivergentBeetleEgg", 2f, () => Immigration.CycleCondition(48) && (Immigration.DiscoveredCondition("DivergentBeetleEgg") || Immigration.CycleCondition(500))));
		list.Add(new CarePackageInfo("StaterpillarEgg", 2f, () => Immigration.CycleCondition(48) && (Immigration.DiscoveredCondition("StaterpillarEgg") || Immigration.CycleCondition(500))));
		list.Add(new CarePackageInfo("BasicCure", 3f, null));
		list.Add(new CarePackageInfo("CustomClothing", 1f, null, "SELECTRANDOM"));
		list.Add(new CarePackageInfo("Funky_Vest", 1f, null));
		this.carePackages = list;
	}

	private static bool CycleCondition(int cycle)
	{
		return GameClock.Instance.GetCycle() >= cycle;
	}

	private static bool DiscoveredCondition(Tag tag)
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

	private List<CarePackageInfo> carePackages;

	private Dictionary<string, List<CarePackageInfo>> carePackagesByDlc;

	public static Immigration Instance;

	private const int CYCLE_THRESHOLD_A = 6;

	private const int CYCLE_THRESHOLD_B = 12;

	private const int CYCLE_THRESHOLD_C = 24;

	private const int CYCLE_THRESHOLD_D = 48;

	private const int CYCLE_THRESHOLD_E = 100;

	private const int CYCLE_THRESHOLD_UNLOCK_EVERYTHING = 500;

	public const string FACADE_SELECT_RANDOM = "SELECTRANDOM";
}
