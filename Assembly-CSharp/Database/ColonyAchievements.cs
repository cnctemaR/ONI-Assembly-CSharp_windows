using System;
using System.Collections.Generic;
using FMODUnity;
using STRINGS;
using TUNING;

namespace Database
{
	public class ColonyAchievements : ResourceSet<ColonyAchievement>
	{
		public ColonyAchievements(ResourceSet parent)
			: base("ColonyAchievements", parent)
		{
			this.Thriving = base.Add(new ColonyAchievement("Thriving", "WINCONDITION_STAY", COLONY_ACHIEVEMENTS.THRIVING.NAME, COLONY_ACHIEVEMENTS.THRIVING.DESCRIPTION, true, new List<ColonyAchievementRequirement>
			{
				new CycleNumber(200),
				new MinimumMorale(16),
				new NumberOfDupes(12),
				new MonumentBuilt()
			}, COLONY_ACHIEVEMENTS.THRIVING.MESSAGE_TITLE, COLONY_ACHIEVEMENTS.THRIVING.MESSAGE_BODY, "victoryShorts/Stay", "victoryLoops/Stay_loop", new Action<KMonoBehaviour>(ThrivingSequence.Start), AudioMixerSnapshots.Get().VictoryNISGenericSnapshot, "home_sweet_home", null, null, null, null));
			this.ReachedDistantPlanet = (DlcManager.IsExpansion1Active() ? base.Add(new ColonyAchievement("ReachedDistantPlanet", "WINCONDITION_LEAVE", COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.NAME, COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.DESCRIPTION, true, new List<ColonyAchievementRequirement>
			{
				new EstablishColonies(),
				new OpenTemporalTear(),
				new SentCraftIntoTemporalTear()
			}, COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.MESSAGE_TITLE_DLC1, COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.MESSAGE_BODY_DLC1, "victoryShorts/Leave", "victoryLoops/Leave_loop", new Action<KMonoBehaviour>(EnterTemporalTearSequence.Start), AudioMixerSnapshots.Get().VictoryNISRocketSnapshot, "rocket", null, null, null, null)) : base.Add(new ColonyAchievement("ReachedDistantPlanet", "WINCONDITION_LEAVE", COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.NAME, COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.DESCRIPTION, true, new List<ColonyAchievementRequirement>
			{
				new ReachedSpace(Db.Get().SpaceDestinationTypes.Wormhole)
			}, COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.MESSAGE_TITLE, COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.MESSAGE_BODY, "victoryShorts/Leave", "victoryLoops/Leave_loop", new Action<KMonoBehaviour>(ReachedDistantPlanetSequence.Start), AudioMixerSnapshots.Get().VictoryNISRocketSnapshot, "rocket", null, null, null, null)));
			if (DlcManager.IsExpansion1Active())
			{
				this.CollectedArtifacts = new ColonyAchievement("CollectedArtifacts", "WINCONDITION_ARTIFACTS", COLONY_ACHIEVEMENTS.STUDY_ARTIFACTS.NAME, COLONY_ACHIEVEMENTS.STUDY_ARTIFACTS.DESCRIPTION, true, new List<ColonyAchievementRequirement>
				{
					new CollectedArtifacts(),
					new CollectedSpaceArtifacts()
				}, COLONY_ACHIEVEMENTS.STUDY_ARTIFACTS.MESSAGE_TITLE, COLONY_ACHIEVEMENTS.STUDY_ARTIFACTS.MESSAGE_BODY, "victoryShorts/Artifact", "victoryLoops/Artifact_loop", new Action<KMonoBehaviour>(ArtifactSequence.Start), AudioMixerSnapshots.Get().VictoryNISGenericSnapshot, "cosmic_archaeology", DlcManager.EXPANSION1, null, "EXPANSION1_ID", null);
				base.Add(this.CollectedArtifacts);
			}
			if (DlcManager.IsContentSubscribed("DLC2_ID"))
			{
				this.ActivateGeothermalPlant = base.Add(new ColonyAchievement("ActivatedGeothermalPlant", "WINCONDITION_GEOPLANT", COLONY_ACHIEVEMENTS.ACTIVATEGEOTHERMALPLANT.NAME, COLONY_ACHIEVEMENTS.ACTIVATEGEOTHERMALPLANT.DESCRIPTION, true, new List<ColonyAchievementRequirement>
				{
					new DiscoverGeothermalFacility(),
					new RepairGeothermalController(),
					new UseGeothermalPlant(),
					new ClearBlockedGeothermalVent()
				}, COLONY_ACHIEVEMENTS.ACTIVATEGEOTHERMALPLANT.MESSAGE_TITLE, COLONY_ACHIEVEMENTS.ACTIVATEGEOTHERMALPLANT.MESSAGE_BODY, "victoryShorts/Geothermal", "victoryLoops/Geothermal_loop", new Action<KMonoBehaviour>(GeothermalVictorySequence.Start), AudioMixerSnapshots.Get().VictoryNISGenericSnapshot, "geothermalplant", DlcManager.DLC2, null, "DLC2_ID", "GeothermalImperative"));
			}
			this.Survived100Cycles = base.Add(new ColonyAchievement("Survived100Cycles", "SURVIVE_HUNDRED_CYCLES", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SURVIVE_HUNDRED_CYCLES, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SURVIVE_HUNDRED_CYCLES_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new CycleNumber(100)
			}, "", "", "", "", null, default(EventReference), "Turn_of_the_Century", null, null, null, null));
			this.ReachedSpace = (DlcManager.IsExpansion1Active() ? base.Add(new ColonyAchievement("ReachedSpace", "REACH_SPACE_ANY_DESTINATION", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.REACH_SPACE_ANY_DESTINATION, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.REACH_SPACE_ANY_DESTINATION_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new LaunchedCraft()
			}, "", "", "", "", null, default(EventReference), "space_race", null, null, null, null)) : base.Add(new ColonyAchievement("ReachedSpace", "REACH_SPACE_ANY_DESTINATION", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.REACH_SPACE_ANY_DESTINATION, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.REACH_SPACE_ANY_DESTINATION_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new ReachedSpace(null)
			}, "", "", "", "", null, default(EventReference), "space_race", null, null, null, null)));
			this.CompleteSkillBranch = base.Add(new ColonyAchievement("CompleteSkillBranch", "COMPLETED_SKILL_BRANCH", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.COMPLETED_SKILL_BRANCH, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.COMPLETED_SKILL_BRANCH_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new SkillBranchComplete(Db.Get().Skills.GetTerminalSkills())
			}, "", "", "", "", null, default(EventReference), "CompleteSkillBranch", null, null, null, null));
			this.CompleteResearchTree = base.Add(new ColonyAchievement("CompleteResearchTree", "COMPLETED_RESEARCH", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.COMPLETED_RESEARCH, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.COMPLETED_RESEARCH_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new ResearchComplete()
			}, "", "", "", "", null, default(EventReference), "honorary_doctorate", null, null, null, null));
			this.Clothe8Dupes = base.Add(new ColonyAchievement("Clothe8Dupes", "EQUIP_EIGHT_DUPES", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EQUIP_N_DUPES, string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EQUIP_N_DUPES_DESCRIPTION, 8), false, new List<ColonyAchievementRequirement>
			{
				new EquipNDupes(Db.Get().AssignableSlots.Outfit, 8)
			}, "", "", "", "", null, default(EventReference), "and_nowhere_to_go", null, null, null, null));
			this.TameAllBasicCritters = base.Add(new ColonyAchievement("TameAllBasicCritters", "TAME_BASIC_CRITTERS", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.TAME_BASIC_CRITTERS, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.TAME_BASIC_CRITTERS_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new CritterTypesWithTraits(new List<Tag> { "Drecko", "Hatch", "LightBug", "Mole", "Oilfloater", "Pacu", "Puft", "Moo", "Crab", "Squirrel" })
			}, "", "", "", "", null, default(EventReference), "Animal_friends", null, null, null, null));
			this.Build4NatureReserves = base.Add(new ColonyAchievement("Build4NatureReserves", "BUILD_NATURE_RESERVES", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BUILD_NATURE_RESERVES, string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BUILD_NATURE_RESERVES_DESCRIPTION, Db.Get().RoomTypes.NatureReserve.Name, 4), false, new List<ColonyAchievementRequirement>
			{
				new BuildNRoomTypes(Db.Get().RoomTypes.NatureReserve, 4)
			}, "", "", "", "", null, default(EventReference), "Some_Reservations", null, null, null, null));
			this.Minimum20LivingDupes = base.Add(new ColonyAchievement("Minimum20LivingDupes", "TWENTY_DUPES", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.TWENTY_DUPES, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.TWENTY_DUPES_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new NumberOfDupes(20)
			}, "", "", "", "", null, default(EventReference), "no_place_like_clone", null, null, null, null));
			this.TameAGassyMoo = base.Add(new ColonyAchievement("TameAGassyMoo", "TAME_GASSYMOO", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.TAME_GASSYMOO, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.TAME_GASSYMOO_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new CritterTypesWithTraits(new List<Tag> { "Moo" })
			}, "", "", "", "", null, default(EventReference), "moovin_on_up", null, null, null, null));
			this.CoolBuildingTo6K = base.Add(new ColonyAchievement("CoolBuildingTo6K", "SIXKELVIN_BUILDING", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SIXKELVIN_BUILDING, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SIXKELVIN_BUILDING_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new CoolBuildingToXKelvin(6)
			}, "", "", "", "", null, default(EventReference), "not_0k", null, null, null, null));
			this.EatkCalFromMeatByCycle100 = base.Add(new ColonyAchievement("EatkCalFromMeatByCycle100", "EAT_MEAT", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EAT_MEAT, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EAT_MEAT_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new BeforeCycleNumber(100),
				new EatXCaloriesFromY(400000, new List<string>
				{
					FOOD.FOOD_TYPES.MEAT.Id,
					FOOD.FOOD_TYPES.DEEP_FRIED_MEAT.Id,
					FOOD.FOOD_TYPES.PEMMICAN.Id,
					FOOD.FOOD_TYPES.FISH_MEAT.Id,
					FOOD.FOOD_TYPES.COOKED_FISH.Id,
					FOOD.FOOD_TYPES.DEEP_FRIED_FISH.Id,
					FOOD.FOOD_TYPES.SHELLFISH_MEAT.Id,
					FOOD.FOOD_TYPES.DEEP_FRIED_SHELLFISH.Id,
					FOOD.FOOD_TYPES.COOKED_MEAT.Id,
					FOOD.FOOD_TYPES.SURF_AND_TURF.Id,
					FOOD.FOOD_TYPES.BURGER.Id,
					FOOD.FOOD_TYPES.JAWBOFILLET.Id,
					FOOD.FOOD_TYPES.SMOKED_FISH.Id,
					FOOD.FOOD_TYPES.SMOKED_DINOSAURMEAT.Id
				})
			}, "", "", "", "", null, default(EventReference), "Carnivore", null, null, null, null));
			this.NoFarmTilesAndKCal = base.Add(new ColonyAchievement("NoFarmTilesAndKCal", "NO_PLANTERBOX", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.NO_PLANTERBOX, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.NO_PLANTERBOX_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new NoFarmables(),
				new EatXCalories(400000)
			}, "", "", "", "", null, default(EventReference), "Locavore", null, null, null, null));
			this.Generate240000kJClean = base.Add(new ColonyAchievement("Generate240000kJClean", "CLEAN_ENERGY", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.CLEAN_ENERGY, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.CLEAN_ENERGY_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new ProduceXEngeryWithoutUsingYList(240000f, new List<Tag> { "MethaneGenerator", "PetroleumGenerator", "WoodGasGenerator", "Generator", "PeatGenerator" })
			}, "", "", "", "", null, default(EventReference), "sustainably_sustaining", null, null, null, null));
			this.BuildOutsideStartBiome = base.Add(new ColonyAchievement("BuildOutsideStartBiome", "BUILD_OUTSIDE_BIOME", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BUILD_OUTSIDE_BIOME, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BUILD_OUTSIDE_BIOME_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new BuildOutsideStartBiome()
			}, "", "", "", "", null, default(EventReference), "build_outside", null, null, null, null));
			this.Travel10000InTubes = base.Add(new ColonyAchievement("Travel10000InTubes", "TUBE_TRAVEL_DISTANCE", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.TUBE_TRAVEL_DISTANCE, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.TUBE_TRAVEL_DISTANCE_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new TravelXUsingTransitTubes(NavType.Tube, 10000)
			}, "", "", "", "", null, default(EventReference), "Totally-Tubular", null, null, null, null));
			this.VarietyOfRooms = base.Add(new ColonyAchievement("VarietyOfRooms", "VARIETY_OF_ROOMS", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.VARIETY_OF_ROOMS, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.VARIETY_OF_ROOMS_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new BuildRoomType(Db.Get().RoomTypes.NatureReserve),
				new BuildRoomType(Db.Get().RoomTypes.Hospital),
				new BuildRoomType(Db.Get().RoomTypes.RecRoom),
				new BuildRoomType(Db.Get().RoomTypes.GreatHall),
				new BuildRoomType(Db.Get().RoomTypes.Bedroom),
				new BuildRoomType(Db.Get().RoomTypes.PlumbedBathroom),
				new BuildRoomType(Db.Get().RoomTypes.Farm),
				new BuildRoomType(Db.Get().RoomTypes.CreaturePen)
			}, "", "", "", "", null, default(EventReference), "Get-a-Room", null, null, null, null));
			this.SurviveOneYear = base.Add(new ColonyAchievement("SurviveOneYear", "SURVIVE_ONE_YEAR", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SURVIVE_ONE_YEAR, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SURVIVE_ONE_YEAR_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new FractionalCycleNumber(365.25f)
			}, "", "", "", "", null, default(EventReference), "One_year", null, null, null, null));
			this.ExploreOilBiome = base.Add(new ColonyAchievement("ExploreOilBiome", "EXPLORE_OIL_BIOME", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EXPLORE_OIL_BIOME, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EXPLORE_OIL_BIOME_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new ExploreOilFieldSubZone()
			}, "", "", "", "", null, default(EventReference), "enter_oil_biome", null, null, null, null));
			this.EatCookedFood = base.Add(new ColonyAchievement("EatCookedFood", "COOKED_FOOD", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.COOKED_FOOD, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.COOKED_FOOD_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new EatXKCalProducedByY(1, new List<Tag> { "GourmetCookingStation", "CookingStation", "Deepfryer", "Smoker" })
			}, "", "", "", "", null, default(EventReference), "its_not_raw", null, null, null, null));
			this.BasicPumping = base.Add(new ColonyAchievement("BasicPumping", "BASIC_PUMPING", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BASIC_PUMPING, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BASIC_PUMPING_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new VentXKG(SimHashes.Oxygen, 1000f)
			}, "", "", "", "", null, default(EventReference), "BasicPumping", null, null, null, null));
			this.BasicComforts = base.Add(new ColonyAchievement("BasicComforts", "BASIC_COMFORTS", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BASIC_COMFORTS, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BASIC_COMFORTS_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new AtLeastOneBuildingForEachDupe(new List<Tag> { "FlushToilet", "Outhouse" }),
				new AtLeastOneBuildingForEachDupe(new List<Tag> { "Bed", "LuxuryBed" })
			}, "", "", "", "", null, default(EventReference), "1bed_1toilet", null, null, null, null));
			this.PlumbedWashrooms = base.Add(new ColonyAchievement("PlumbedWashrooms", "PLUMBED_WASHROOMS", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.PLUMBED_WASHROOMS, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.PLUMBED_WASHROOMS_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new UpgradeAllBasicBuildings("Outhouse", "FlushToilet"),
				new UpgradeAllBasicBuildings("WashBasin", "WashSink")
			}, "", "", "", "", null, default(EventReference), "royal_flush", null, null, null, null));
			this.AutomateABuilding = base.Add(new ColonyAchievement("AutomateABuilding", "AUTOMATE_A_BUILDING", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.AUTOMATE_A_BUILDING, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.AUTOMATE_A_BUILDING_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new AutomateABuilding()
			}, "", "", "", "", null, default(EventReference), "red_light_green_light", null, null, null, null));
			this.MasterpiecePainting = base.Add(new ColonyAchievement("MasterpiecePainting", "MASTERPIECE_PAINTING", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.MASTERPIECE_PAINTING, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.MASTERPIECE_PAINTING_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new CreateMasterPainting()
			}, "", "", "", "", null, default(EventReference), "art_underground", null, null, null, null));
			this.InspectPOI = base.Add(new ColonyAchievement("InspectPOI", "INSPECT_POI", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.INSPECT_POI, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.INSPECT_POI_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new ActivateLorePOI()
			}, "", "", "", "", null, default(EventReference), "ghosts_of_gravitas", null, null, null, null));
			this.HatchACritter = base.Add(new ColonyAchievement("HatchACritter", "HATCH_A_CRITTER", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.HATCH_A_CRITTER, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.HATCH_A_CRITTER_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new CritterTypeExists(new List<Tag>
				{
					"DreckoPlasticBaby", "HatchHardBaby", "HatchMetalBaby", "HatchVeggieBaby", "LightBugBlackBaby", "LightBugBlueBaby", "LightBugCrystalBaby", "LightBugOrangeBaby", "LightBugPinkBaby", "LightBugPurpleBaby",
					"OilfloaterDecorBaby", "OilfloaterHighTempBaby", "PacuCleanerBaby", "PacuTropicalBaby", "PuftBleachstoneBaby", "PuftOxyliteBaby", "SquirrelHugBaby", "CrabWoodBaby", "CrabFreshWaterBaby", "MoleDelicacyBaby"
				})
			}, "", "", "", "", null, default(EventReference), "good_egg", null, null, null, null));
			this.CuredDisease = base.Add(new ColonyAchievement("CuredDisease", "CURED_DISEASE", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.CURED_DISEASE, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.CURED_DISEASE_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new CureDisease()
			}, "", "", "", "", null, default(EventReference), "medic", null, null, null, null));
			this.GeneratorTuneup = base.Add(new ColonyAchievement("GeneratorTuneup", "GENERATOR_TUNEUP", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.GENERATOR_TUNEUP, string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.GENERATOR_TUNEUP_DESCRIPTION, 100), false, new List<ColonyAchievementRequirement>
			{
				new TuneUpGenerator(100f)
			}, "", "", "", "", null, default(EventReference), "tune_up_for_what", null, null, null, null));
			this.ClearFOW = base.Add(new ColonyAchievement("ClearFOW", "CLEAR_FOW", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.CLEAR_FOW, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.CLEAR_FOW_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new RevealAsteriod(0.8f)
			}, "", "", "", "", null, default(EventReference), "pulling_back_the_veil", null, null, null, null));
			this.HatchRefinement = base.Add(new ColonyAchievement("HatchRefinement", "HATCH_REFINEMENT", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.HATCH_REFINEMENT, string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.HATCH_REFINEMENT_DESCRIPTION, GameUtil.GetFormattedMass(10000f, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}")), false, new List<ColonyAchievementRequirement>
			{
				new CreaturePoopKGProduction("HatchMetal", 10000f)
			}, "", "", "", "", null, default(EventReference), "down_the_hatch", null, null, null, null));
			this.BunkerDoorDefense = base.Add(new ColonyAchievement("BunkerDoorDefense", "BUNKER_DOOR_DEFENSE", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BUNKER_DOOR_DEFENSE, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BUNKER_DOOR_DEFENSE_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new BlockedCometWithBunkerDoor()
			}, "", "", "", "", null, default(EventReference), "Immovable_Object", null, null, null, null));
			this.IdleDuplicants = base.Add(new ColonyAchievement("IdleDuplicants", "IDLE_DUPLICANTS", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.IDLE_DUPLICANTS, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.IDLE_DUPLICANTS_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new DupesVsSolidTransferArmFetch(1f, 5)
			}, "", "", "", "", null, default(EventReference), "easy_livin", null, null, null, null));
			this.ExosuitCycles = base.Add(new ColonyAchievement("ExosuitCycles", "EXOSUIT_CYCLES", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EXOSUIT_CYCLES, string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EXOSUIT_CYCLES_DESCRIPTION, 10), false, new List<ColonyAchievementRequirement>
			{
				new DupesCompleteChoreInExoSuitForCycles(10)
			}, "", "", "", "", null, default(EventReference), "job_suitability", null, null, null, null));
			if (DlcManager.IsExpansion1Active())
			{
				string text = "FirstTeleport";
				string text2 = "FIRST_TELEPORT";
				string text3 = COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.FIRST_TELEPORT;
				string text4 = COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.FIRST_TELEPORT_DESCRIPTION;
				bool flag = false;
				List<ColonyAchievementRequirement> list = new List<ColonyAchievementRequirement>();
				list.Add(new TeleportDuplicant());
				list.Add(new DefrostDuplicant());
				string text5 = "";
				string text6 = "";
				string text7 = "";
				string text8 = "";
				Action<KMonoBehaviour> action = null;
				string[] array = DlcManager.EXPANSION1;
				this.FirstTeleport = base.Add(new ColonyAchievement(text, text2, text3, text4, flag, list, text5, text6, text7, text8, action, default(EventReference), "first_teleport_of_call", array, null, "EXPANSION1_ID", null));
				string text9 = "SoftLaunch";
				string text10 = "SOFT_LAUNCH";
				string text11 = COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SOFT_LAUNCH;
				string text12 = COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SOFT_LAUNCH_DESCRIPTION;
				bool flag2 = false;
				List<ColonyAchievementRequirement> list2 = new List<ColonyAchievementRequirement>();
				list2.Add(new BuildALaunchPad());
				string text13 = "";
				string text14 = "";
				string text15 = "";
				string text16 = "";
				Action<KMonoBehaviour> action2 = null;
				array = DlcManager.EXPANSION1;
				this.SoftLaunch = base.Add(new ColonyAchievement(text9, text10, text11, text12, flag2, list2, text13, text14, text15, text16, action2, default(EventReference), "soft_launch", array, null, "EXPANSION1_ID", null));
				string text17 = "GMOOK";
				string text18 = "GMO_OK";
				string text19 = COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.GMO_OK;
				string text20 = COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.GMO_OK_DESCRIPTION;
				bool flag3 = false;
				List<ColonyAchievementRequirement> list3 = new List<ColonyAchievementRequirement>();
				list3.Add(new AnalyzeSeed(BasicFabricMaterialPlantConfig.ID));
				list3.Add(new AnalyzeSeed("BasicSingleHarvestPlant"));
				list3.Add(new AnalyzeSeed("GasGrass"));
				list3.Add(new AnalyzeSeed("MushroomPlant"));
				list3.Add(new AnalyzeSeed("PrickleFlower"));
				list3.Add(new AnalyzeSeed("SaltPlant"));
				list3.Add(new AnalyzeSeed(SeaLettuceConfig.ID));
				list3.Add(new AnalyzeSeed("SpiceVine"));
				list3.Add(new AnalyzeSeed("SwampHarvestPlant"));
				list3.Add(new AnalyzeSeed(SwampLilyConfig.ID));
				list3.Add(new AnalyzeSeed("WormPlant"));
				list3.Add(new AnalyzeSeed("ColdWheat"));
				list3.Add(new AnalyzeSeed("BeanPlant"));
				string text21 = "";
				string text22 = "";
				string text23 = "";
				string text24 = "";
				Action<KMonoBehaviour> action3 = null;
				array = DlcManager.EXPANSION1;
				this.GMOOK = base.Add(new ColonyAchievement(text17, text18, text19, text20, flag3, list3, text21, text22, text23, text24, action3, default(EventReference), "gmo_ok", array, null, "EXPANSION1_ID", null));
				string text25 = "MineTheGap";
				string text26 = "MINE_THE_GAP";
				string text27 = COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.MINE_THE_GAP;
				string text28 = COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.MINE_THE_GAP_DESCRIPTION;
				bool flag4 = false;
				List<ColonyAchievementRequirement> list4 = new List<ColonyAchievementRequirement>();
				list4.Add(new HarvestAmountFromSpacePOI(1000000f));
				string text29 = "";
				string text30 = "";
				string text31 = "";
				string text32 = "";
				Action<KMonoBehaviour> action4 = null;
				array = DlcManager.EXPANSION1;
				this.MineTheGap = base.Add(new ColonyAchievement(text25, text26, text27, text28, flag4, list4, text29, text30, text31, text32, action4, default(EventReference), "mine_the_gap", array, null, "EXPANSION1_ID", null));
				string text33 = "LandedOnAllWorlds";
				string text34 = "LANDED_ON_ALL_WORLDS";
				string text35 = COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.LAND_ON_ALL_WORLDS;
				string text36 = COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.LAND_ON_ALL_WORLDS_DESCRIPTION;
				bool flag5 = false;
				List<ColonyAchievementRequirement> list5 = new List<ColonyAchievementRequirement>();
				list5.Add(new LandOnAllWorlds());
				string text37 = "";
				string text38 = "";
				string text39 = "";
				string text40 = "";
				Action<KMonoBehaviour> action5 = null;
				array = DlcManager.EXPANSION1;
				this.LandedOnAllWorlds = base.Add(new ColonyAchievement(text33, text34, text35, text36, flag5, list5, text37, text38, text39, text40, action5, default(EventReference), "land_on_all_worlds", array, null, "EXPANSION1_ID", null));
				string text41 = "RadicalTrip";
				string text42 = "RADICAL_TRIP";
				string text43 = COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.RADICAL_TRIP;
				string text44 = string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.RADICAL_TRIP_DESCRIPTION, 10);
				bool flag6 = false;
				List<ColonyAchievementRequirement> list6 = new List<ColonyAchievementRequirement>();
				list6.Add(new RadBoltTravelDistance(10000));
				string text45 = "";
				string text46 = "";
				string text47 = "";
				string text48 = "";
				Action<KMonoBehaviour> action6 = null;
				array = DlcManager.EXPANSION1;
				this.RadicalTrip = base.Add(new ColonyAchievement(text41, text42, text43, text44, flag6, list6, text45, text46, text47, text48, action6, default(EventReference), "radical_trip", array, null, "EXPANSION1_ID", null));
				string text49 = "SweeterThanHoney";
				string text50 = "SWEETER_THAN_HONEY";
				string text51 = COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SWEETER_THAN_HONEY;
				string text52 = COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SWEETER_THAN_HONEY_DESCRIPTION;
				bool flag7 = false;
				List<ColonyAchievementRequirement> list7 = new List<ColonyAchievementRequirement>();
				list7.Add(new HarvestAHiveWithoutBeingStung());
				string text53 = "";
				string text54 = "";
				string text55 = "";
				string text56 = "";
				Action<KMonoBehaviour> action7 = null;
				array = DlcManager.EXPANSION1;
				this.SweeterThanHoney = base.Add(new ColonyAchievement(text49, text50, text51, text52, flag7, list7, text53, text54, text55, text56, action7, default(EventReference), "sweeter_than_honey", array, null, "EXPANSION1_ID", null));
				string text57 = "SurviveInARocket";
				string text58 = "SURVIVE_IN_A_ROCKET";
				string text59 = COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SURVIVE_IN_A_ROCKET;
				string text60 = string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SURVIVE_IN_A_ROCKET_DESCRIPTION, 10, 25);
				bool flag8 = false;
				List<ColonyAchievementRequirement> list8 = new List<ColonyAchievementRequirement>();
				list8.Add(new SurviveARocketWithMinimumMorale(25f, 10));
				string text61 = "";
				string text62 = "";
				string text63 = "";
				string text64 = "";
				Action<KMonoBehaviour> action8 = null;
				array = DlcManager.EXPANSION1;
				this.SurviveInARocket = base.Add(new ColonyAchievement(text57, text58, text59, text60, flag8, list8, text61, text62, text63, text64, action8, default(EventReference), "survive_a_rocket", array, null, "EXPANSION1_ID", null));
				string text65 = "RunAReactor";
				string text66 = "REACTOR_USAGE";
				string text67 = COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.REACTOR_USAGE;
				string text68 = string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.REACTOR_USAGE_DESCRIPTION, 5);
				bool flag9 = false;
				List<ColonyAchievementRequirement> list9 = new List<ColonyAchievementRequirement>();
				list9.Add(new RunReactorForXDays(5));
				string text69 = "";
				string text70 = "";
				string text71 = "";
				string text72 = "";
				Action<KMonoBehaviour> action9 = null;
				array = DlcManager.EXPANSION1;
				this.RunAReactor = base.Add(new ColonyAchievement(text65, text66, text67, text68, flag9, list9, text69, text70, text71, text72, action9, default(EventReference), "thats_rad", array, null, "EXPANSION1_ID", null));
			}
			if (DlcManager.IsContentSubscribed("DLC3_ID"))
			{
				string text73 = "EfficientData";
				string text74 = "EFFICIENT_DATAMINING";
				string text75 = COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.DATA_DRIVEN;
				string text76 = COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.DATA_DRIVEN_DESCRIPTION;
				bool flag10 = false;
				List<ColonyAchievementRequirement> list10 = new List<ColonyAchievementRequirement>();
				list10.Add(new EfficientDataMiningCheck());
				string text77 = "";
				string text78 = "";
				string text79 = "";
				string text80 = "";
				Action<KMonoBehaviour> action10 = null;
				string[] array = DlcManager.DLC3;
				this.EfficientData = base.Add(new ColonyAchievement(text73, text74, text75, text76, flag10, list10, text77, text78, text79, text80, action10, default(EventReference), "efficient_data_mining", array, null, "DLC3_ID", null));
				string text81 = "AllTheCircuits";
				string text82 = "ALL_THE_CIRCUITS";
				string text83 = COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.MVB;
				string text84 = string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.MVB_DESCRIPTION, 8);
				bool flag11 = false;
				List<ColonyAchievementRequirement> list11 = new List<ColonyAchievementRequirement>();
				list11.Add(new AllTheCircuitsCompleteCheck());
				string text85 = "";
				string text86 = "";
				string text87 = "";
				string text88 = "";
				Action<KMonoBehaviour> action11 = null;
				array = DlcManager.DLC3;
				this.AllTheCircuits = base.Add(new ColonyAchievement(text81, text82, text83, text84, flag11, list11, text85, text86, text87, text88, action11, default(EventReference), "all_the_circuits", array, null, "DLC3_ID", null));
			}
			if (DlcManager.IsContentSubscribed("DLC4_ID"))
			{
				string text89 = "AsteroidDestroyed";
				string text90 = "ASTEROID_DESTROYED";
				string text91 = COLONY_ACHIEVEMENTS.ASTEROID_DESTROYED.NAME;
				string text92 = COLONY_ACHIEVEMENTS.ASTEROID_DESTROYED.DESCRIPTION;
				bool flag12 = true;
				List<ColonyAchievementRequirement> list12 = new List<ColonyAchievementRequirement>();
				list12.Add(new DefeatPrehistoricAsteroid());
				this.AsteroidDestroyed = base.Add(new ColonyAchievement(text89, text90, text91, text92, flag12, list12, COLONY_ACHIEVEMENTS.ASTEROID_DESTROYED.MESSAGE_TITLE, COLONY_ACHIEVEMENTS.ASTEROID_DESTROYED.MESSAGE_BODY, "DLC4/LargeImpactorDefeatedVideo", "DLC4/LargeImpactorSpacePOIVideo", delegate(KMonoBehaviour a)
				{
					LargeImpactorDestroyedSequence.Start();
				}, AudioMixerSnapshots.Get().VictoryNISGenericSnapshot, "blast_line_of_defense", DlcManager.DLC4, null, "DLC4_ID", "DemoliorImperative"));
				string text93 = "AsteroidSurvived";
				string text94 = "ASTEROID_SURVIVED";
				string text95 = COLONY_ACHIEVEMENTS.ASTEROID_SURVIVED.NAME;
				string text96 = COLONY_ACHIEVEMENTS.ASTEROID_SURVIVED.DESCRIPTION;
				bool flag13 = false;
				List<ColonyAchievementRequirement> list13 = new List<ColonyAchievementRequirement>();
				list13.Add(new SurvivedPrehistoricAsteroidImpact(100));
				list13.Add(new NoDuplicantsCanDie());
				string text97 = "";
				string text98 = "";
				string text99 = "";
				string text100 = "";
				Action<KMonoBehaviour> action12 = null;
				string[] array = DlcManager.DLC4;
				this.AsteroidSurvived = base.Add(new ColonyAchievement(text93, text94, text95, text96, flag13, list13, text97, text98, text99, text100, action12, default(EventReference), "life_found_a_way", array, null, "DLC4_ID", "DemoliorSurivedAchievement"));
			}
		}

		public ColonyAchievement Thriving;

		public ColonyAchievement ReachedDistantPlanet;

		public ColonyAchievement CollectedArtifacts;

		public ColonyAchievement Survived100Cycles;

		public ColonyAchievement ReachedSpace;

		public ColonyAchievement CompleteSkillBranch;

		public ColonyAchievement CompleteResearchTree;

		public ColonyAchievement Clothe8Dupes;

		public ColonyAchievement Build4NatureReserves;

		public ColonyAchievement Minimum20LivingDupes;

		public ColonyAchievement TameAGassyMoo;

		public ColonyAchievement CoolBuildingTo6K;

		public ColonyAchievement EatkCalFromMeatByCycle100;

		public ColonyAchievement NoFarmTilesAndKCal;

		public ColonyAchievement Generate240000kJClean;

		public ColonyAchievement BuildOutsideStartBiome;

		public ColonyAchievement Travel10000InTubes;

		public ColonyAchievement VarietyOfRooms;

		public ColonyAchievement TameAllBasicCritters;

		public ColonyAchievement SurviveOneYear;

		public ColonyAchievement ExploreOilBiome;

		public ColonyAchievement EatCookedFood;

		public ColonyAchievement BasicPumping;

		public ColonyAchievement BasicComforts;

		public ColonyAchievement PlumbedWashrooms;

		public ColonyAchievement AutomateABuilding;

		public ColonyAchievement MasterpiecePainting;

		public ColonyAchievement InspectPOI;

		public ColonyAchievement HatchACritter;

		public ColonyAchievement CuredDisease;

		public ColonyAchievement GeneratorTuneup;

		public ColonyAchievement ClearFOW;

		public ColonyAchievement HatchRefinement;

		public ColonyAchievement BunkerDoorDefense;

		public ColonyAchievement IdleDuplicants;

		public ColonyAchievement ExosuitCycles;

		public ColonyAchievement FirstTeleport;

		public ColonyAchievement SoftLaunch;

		public ColonyAchievement GMOOK;

		public ColonyAchievement MineTheGap;

		public ColonyAchievement LandedOnAllWorlds;

		public ColonyAchievement RadicalTrip;

		public ColonyAchievement SweeterThanHoney;

		public ColonyAchievement SurviveInARocket;

		public ColonyAchievement RunAReactor;

		public ColonyAchievement ActivateGeothermalPlant;

		public ColonyAchievement EfficientData;

		public ColonyAchievement AllTheCircuits;

		public ColonyAchievement AsteroidDestroyed;

		public ColonyAchievement AsteroidSurvived;
	}
}
