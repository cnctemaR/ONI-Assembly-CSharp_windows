using System;
using Klei.AI;
using STRINGS;
using TUNING;

namespace Database
{
	public class GameplayEvents : ResourceSet<GameplayEvent>
	{
		public GameplayEvents(ResourceSet parent)
			: base("GameplayEvents", parent)
		{
			this.HatchSpawnEvent = base.Add(new CreatureSpawnEvent());
			this.PartyEvent = base.Add(new PartyEvent());
			this.EclipseEvent = base.Add(new EclipseEvent());
			this.SatelliteCrashEvent = base.Add(new SatelliteCrashEvent());
			this.FoodFightEvent = base.Add(new FoodFightEvent());
			this.BaseGameMeteorEvents();
			this.Expansion1MeteorEvents();
			this.DLCMeteorEvents();
			this.PrickleFlowerBlightEvent = base.Add(new PlantBlightEvent("PrickleFlowerBlightEvent", "PrickleFlower", 3600f, 30f));
			this.CryoFriend = base.Add(new SimpleEvent("CryoFriend", GAMEPLAY_EVENTS.EVENT_TYPES.CRYOFRIEND.NAME, GAMEPLAY_EVENTS.EVENT_TYPES.CRYOFRIEND.DESCRIPTION, "cryofriend_kanim", GAMEPLAY_EVENTS.EVENT_TYPES.CRYOFRIEND.BUTTON, null));
			this.WarpWorldReveal = base.Add(new SimpleEvent("WarpWorldReveal", GAMEPLAY_EVENTS.EVENT_TYPES.WARPWORLDREVEAL.NAME, GAMEPLAY_EVENTS.EVENT_TYPES.WARPWORLDREVEAL.DESCRIPTION, "warpworldreveal_kanim", GAMEPLAY_EVENTS.EVENT_TYPES.WARPWORLDREVEAL.BUTTON, null));
			this.ArtifactReveal = base.Add(new SimpleEvent("ArtifactReveal", GAMEPLAY_EVENTS.EVENT_TYPES.ARTIFACT_REVEAL.NAME, GAMEPLAY_EVENTS.EVENT_TYPES.ARTIFACT_REVEAL.DESCRIPTION, "analyzeartifact_kanim", GAMEPLAY_EVENTS.EVENT_TYPES.ARTIFACT_REVEAL.BUTTON, null));
		}

		private void BaseGameMeteorEvents()
		{
			string text = "MeteorShowerGoldEvent";
			float num = 3000f;
			float num2 = 0.4f;
			MathUtil.MinMax minMax = new MathUtil.MinMax(50f, 100f);
			this.MeteorShowerGoldEvent = base.Add(new MeteorShowerEvent(text, num, num2, new MathUtil.MinMax(800f, 1200f), minMax, null, true).AddMeteor(GoldCometConfig.ID, 2f).AddMeteor(RockCometConfig.ID, 0.5f).AddMeteor(DustCometConfig.ID, 5f));
			string text2 = "MeteorShowerCopperEvent";
			float num3 = 4200f;
			float num4 = 5.5f;
			minMax = new MathUtil.MinMax(100f, 400f);
			this.MeteorShowerCopperEvent = base.Add(new MeteorShowerEvent(text2, num3, num4, new MathUtil.MinMax(300f, 1200f), minMax, null, true).AddMeteor(CopperCometConfig.ID, 1f).AddMeteor(RockCometConfig.ID, 1f));
			string text3 = "MeteorShowerIronEvent";
			float num5 = 6000f;
			float num6 = 1.25f;
			minMax = new MathUtil.MinMax(100f, 400f);
			this.MeteorShowerIronEvent = base.Add(new MeteorShowerEvent(text3, num5, num6, new MathUtil.MinMax(300f, 1200f), minMax, null, true).AddMeteor(IronCometConfig.ID, 1f).AddMeteor(RockCometConfig.ID, 2f).AddMeteor(DustCometConfig.ID, 5f));
		}

		private void Expansion1MeteorEvents()
		{
			string text = "MeteorShowerDustEvent";
			float num = 9000f;
			float num2 = 1.25f;
			string text2 = ClusterMapMeteorShowerConfig.GetFullID("Regolith");
			MathUtil.MinMax minMax = new MathUtil.MinMax(100f, 400f);
			this.MeteorShowerDustEvent = base.Add(new MeteorShowerEvent(text, num, num2, new MathUtil.MinMax(300f, 1200f), minMax, text2, true).AddMeteor(RockCometConfig.ID, 1f).AddMeteor(DustCometConfig.ID, 6f));
			string text3 = "GassyMooteorEvent";
			float num3 = 15f;
			float num4 = 3.125f;
			text2 = ClusterMapMeteorShowerConfig.GetFullID("Moo");
			minMax = new MathUtil.MinMax(15f, 15f);
			this.GassyMooteorEvent = base.Add(new MeteorShowerEvent(text3, num3, num4, METEORS.BOMBARDMENT_OFF.NONE, minMax, text2, false).AddMeteor(GassyMooCometConfig.ID, 1f));
			string text4 = "MeteorShowerFullereneEvent";
			float num5 = 30f;
			float num6 = 0.5f;
			minMax = new MathUtil.MinMax(80f, 80f);
			this.MeteorShowerFullereneEvent = base.Add(new MeteorShowerEvent(text4, num5, num6, METEORS.BOMBARDMENT_OFF.NONE, minMax, null, false).AddMeteor(FullereneCometConfig.ID, 6f).AddMeteor(DustCometConfig.ID, 1f));
			string text5 = "ClusterSnowShower";
			float num7 = 600f;
			float num8 = 3f;
			text2 = ClusterMapMeteorShowerConfig.GetFullID("Snow");
			minMax = METEORS.BOMBARDMENT_ON.UNLIMITED;
			this.ClusterSnowShower = base.Add(new MeteorShowerEvent(text5, num7, num8, METEORS.BOMBARDMENT_OFF.NONE, minMax, text2, true).AddMeteor(SnowballCometConfig.ID, 2f).AddMeteor(LightDustCometConfig.ID, 1f));
			string text6 = "ClusterIceShower";
			float num9 = 300f;
			float num10 = 1.4f;
			text2 = ClusterMapMeteorShowerConfig.GetFullID("Ice");
			minMax = METEORS.BOMBARDMENT_ON.UNLIMITED;
			this.ClusterIceShower = base.Add(new MeteorShowerEvent(text6, num9, num10, METEORS.BOMBARDMENT_OFF.NONE, minMax, text2, true).AddMeteor(SnowballCometConfig.ID, 14f).AddMeteor(HardIceCometConfig.ID, 1f));
			string text7 = "ClusterOxyliteShower";
			float num11 = 300f;
			float num12 = 4f;
			text2 = ClusterMapMeteorShowerConfig.GetFullID("Oxylite");
			minMax = METEORS.BOMBARDMENT_ON.UNLIMITED;
			this.ClusterOxyliteShower = base.Add(new MeteorShowerEvent(text7, num11, num12, METEORS.BOMBARDMENT_OFF.NONE, minMax, text2, true).AddMeteor(OxyliteCometConfig.ID, 4f).AddMeteor(LightDustCometConfig.ID, 4f));
			string text8 = "ClusterBleachStoneShower";
			float num13 = 300f;
			float num14 = 3f;
			text2 = ClusterMapMeteorShowerConfig.GetFullID("BleachStone");
			minMax = METEORS.BOMBARDMENT_ON.UNLIMITED;
			this.ClusterBleachStoneShower = base.Add(new MeteorShowerEvent(text8, num13, num14, METEORS.BOMBARDMENT_OFF.NONE, minMax, text2, true).AddMeteor(BleachStoneCometConfig.ID, 13f).AddMeteor(LightDustCometConfig.ID, 3f));
			string text9 = "ClusterBiologicalShower";
			float num15 = 300f;
			float num16 = 3f;
			text2 = ClusterMapMeteorShowerConfig.GetFullID("Biological");
			minMax = METEORS.BOMBARDMENT_ON.UNLIMITED;
			this.ClusterBiologicalShower = base.Add(new MeteorShowerEvent(text9, num15, num16, METEORS.BOMBARDMENT_OFF.NONE, minMax, text2, true).AddMeteor(SlimeCometConfig.ID, 2f).AddMeteor(AlgaeCometConfig.ID, 1f).AddMeteor(PhosphoricCometConfig.ID, 1f));
			string text10 = "ClusterLightRegolithShower";
			float num17 = 300f;
			float num18 = 4f;
			text2 = ClusterMapMeteorShowerConfig.GetFullID("LightDust");
			minMax = METEORS.BOMBARDMENT_ON.UNLIMITED;
			this.ClusterLightRegolithShower = base.Add(new MeteorShowerEvent(text10, num17, num18, METEORS.BOMBARDMENT_OFF.NONE, minMax, text2, true).AddMeteor(DustCometConfig.ID, 1f).AddMeteor(LightDustCometConfig.ID, 1f));
			string text11 = "ClusterRegolithShower";
			float num19 = 300f;
			float num20 = 3.5f;
			text2 = ClusterMapMeteorShowerConfig.GetFullID("HeavyDust");
			minMax = METEORS.BOMBARDMENT_ON.UNLIMITED;
			this.ClusterRegolithShower = base.Add(new MeteorShowerEvent(text11, num19, num20, METEORS.BOMBARDMENT_OFF.NONE, minMax, text2, true).AddMeteor(DustCometConfig.ID, 3f).AddMeteor(RockCometConfig.ID, 2f).AddMeteor(LightDustCometConfig.ID, 1f));
			string text12 = "ClusterGoldShower";
			float num21 = 75f;
			float num22 = 1f;
			text2 = ClusterMapMeteorShowerConfig.GetFullID("Gold");
			minMax = METEORS.BOMBARDMENT_ON.UNLIMITED;
			this.ClusterGoldShower = base.Add(new MeteorShowerEvent(text12, num21, num22, METEORS.BOMBARDMENT_OFF.NONE, minMax, text2, true).AddMeteor(GoldCometConfig.ID, 4f).AddMeteor(RockCometConfig.ID, 1f).AddMeteor(LightDustCometConfig.ID, 2f));
			string text13 = "ClusterCopperShower";
			float num23 = 150f;
			float num24 = 2.5f;
			text2 = ClusterMapMeteorShowerConfig.GetFullID("Copper");
			minMax = METEORS.BOMBARDMENT_ON.UNLIMITED;
			this.ClusterCopperShower = base.Add(new MeteorShowerEvent(text13, num23, num24, METEORS.BOMBARDMENT_OFF.NONE, minMax, text2, true).AddMeteor(CopperCometConfig.ID, 2f).AddMeteor(RockCometConfig.ID, 1f));
			string text14 = "ClusterIronShower";
			float num25 = 300f;
			float num26 = 4.5f;
			text2 = ClusterMapMeteorShowerConfig.GetFullID("Iron");
			minMax = METEORS.BOMBARDMENT_ON.UNLIMITED;
			this.ClusterIronShower = base.Add(new MeteorShowerEvent(text14, num25, num26, METEORS.BOMBARDMENT_OFF.NONE, minMax, text2, true).AddMeteor(IronCometConfig.ID, 4f).AddMeteor(DustCometConfig.ID, 1f).AddMeteor(LightDustCometConfig.ID, 2f));
			string text15 = "ClusterUraniumShower";
			float num27 = 150f;
			float num28 = 4.5f;
			text2 = ClusterMapMeteorShowerConfig.GetFullID("Uranium");
			minMax = METEORS.BOMBARDMENT_ON.UNLIMITED;
			this.ClusterUraniumShower = base.Add(new MeteorShowerEvent(text15, num27, num28, METEORS.BOMBARDMENT_OFF.NONE, minMax, text2, true).AddMeteor(UraniumCometConfig.ID, 2.5f).AddMeteor(DustCometConfig.ID, 1f).AddMeteor(LightDustCometConfig.ID, 2f));
		}

		private void DLCMeteorEvents()
		{
			string text = "ClusterIceAndTreesShower";
			float num = 300f;
			float num2 = 1.4f;
			string fullID = ClusterMapMeteorShowerConfig.GetFullID("IceAndTrees");
			MathUtil.MinMax unlimited = METEORS.BOMBARDMENT_ON.UNLIMITED;
			this.ClusterIceAndTreesShower = base.Add(new MeteorShowerEvent(text, num, num2, METEORS.BOMBARDMENT_OFF.NONE, unlimited, fullID, true).AddMeteor(SpaceTreeSeedCometConfig.ID, 1f).AddMeteor(HardIceCometConfig.ID, 2f).AddMeteor(SnowballCometConfig.ID, 22f));
		}

		private void BonusEvents()
		{
			GameplayEventMinionFilters instance = GameplayEventMinionFilters.Instance;
			GameplayEventPreconditions instance2 = GameplayEventPreconditions.Instance;
			Skills skills = Db.Get().Skills;
			RoomTypes roomTypes = Db.Get().RoomTypes;
			this.BonusDream1 = base.Add(new BonusEvent("BonusDream1", null, 1, false, 0).TriggerOnUseBuilding(1, new string[] { "Bed", "LuxuryBed" }).SetRoomConstraints(false, new RoomType[] { roomTypes.Barracks }).AddPrecondition(instance2.BuildingExists("Bed", 2))
				.AddPriorityBoost(instance2.BuildingExists("Bed", 5), 1)
				.AddPriorityBoost(instance2.BuildingExists("LuxuryBed", 1), 5)
				.TrySpawnEventOnSuccess("BonusDream2"));
			this.BonusDream2 = base.Add(new BonusEvent("BonusDream2", null, 1, false, 10).TriggerOnUseBuilding(10, new string[] { "Bed", "LuxuryBed" }).AddPrecondition(instance2.PastEventCountAndNotActive(this.BonusDream1, 1)).AddPrecondition(instance2.Or(instance2.RoomBuilt(roomTypes.Barracks), instance2.RoomBuilt(roomTypes.Bedroom)))
				.AddPriorityBoost(instance2.BuildingExists("LuxuryBed", 1), 5)
				.TrySpawnEventOnSuccess("BonusDream3"));
			this.BonusDream3 = base.Add(new BonusEvent("BonusDream3", null, 1, false, 20).TriggerOnUseBuilding(10, new string[] { "Bed", "LuxuryBed" }).AddPrecondition(instance2.PastEventCountAndNotActive(this.BonusDream2, 1)).AddPrecondition(instance2.Or(instance2.RoomBuilt(roomTypes.Barracks), instance2.RoomBuilt(roomTypes.Bedroom)))
				.TrySpawnEventOnSuccess("BonusDream4"));
			this.BonusDream4 = base.Add(new BonusEvent("BonusDream4", null, 1, false, 30).TriggerOnUseBuilding(10, new string[] { "LuxuryBed" }).AddPrecondition(instance2.PastEventCountAndNotActive(this.BonusDream2, 1)).AddPrecondition(instance2.Or(instance2.RoomBuilt(roomTypes.Barracks), instance2.RoomBuilt(roomTypes.Bedroom))));
			this.BonusToilet1 = base.Add(new BonusEvent("BonusToilet1", null, 1, false, 0).TriggerOnUseBuilding(1, new string[] { "Outhouse", "FlushToilet" }).AddPrecondition(instance2.Or(instance2.BuildingExists("Outhouse", 2), instance2.BuildingExists("FlushToilet", 1))).AddPrecondition(instance2.Or(instance2.BuildingExists("WashBasin", 2), instance2.BuildingExists("WashSink", 1)))
				.AddPriorityBoost(instance2.BuildingExists("FlushToilet", 1), 1)
				.TrySpawnEventOnSuccess("BonusToilet2"));
			this.BonusToilet2 = base.Add(new BonusEvent("BonusToilet2", null, 1, false, 10).TriggerOnUseBuilding(5, new string[] { "FlushToilet" }).AddPrecondition(instance2.BuildingExists("FlushToilet", 1)).AddPrecondition(instance2.PastEventCountAndNotActive(this.BonusToilet1, 1))
				.AddPriorityBoost(instance2.BuildingExists("FlushToilet", 2), 5)
				.TrySpawnEventOnSuccess("BonusToilet3"));
			this.BonusToilet3 = base.Add(new BonusEvent("BonusToilet3", null, 1, false, 20).TriggerOnUseBuilding(5, new string[] { "FlushToilet" }).SetRoomConstraints(false, new RoomType[] { roomTypes.Latrine, roomTypes.PlumbedBathroom }).AddPrecondition(instance2.PastEventCountAndNotActive(this.BonusToilet2, 1))
				.AddPrecondition(instance2.Or(instance2.RoomBuilt(roomTypes.Latrine), instance2.RoomBuilt(roomTypes.PlumbedBathroom)))
				.AddPriorityBoost(instance2.BuildingExists("FlushToilet", 2), 10)
				.TrySpawnEventOnSuccess("BonusToilet4"));
			this.BonusToilet4 = base.Add(new BonusEvent("BonusToilet4", null, 1, false, 30).TriggerOnUseBuilding(5, new string[] { "FlushToilet" }).SetRoomConstraints(false, new RoomType[] { roomTypes.PlumbedBathroom }).AddPrecondition(instance2.PastEventCountAndNotActive(this.BonusToilet3, 1))
				.AddPrecondition(instance2.RoomBuilt(roomTypes.PlumbedBathroom)));
			this.BonusResearch = base.Add(new BonusEvent("BonusResearch", null, 1, false, 0).AddPrecondition(instance2.BuildingExists("ResearchCenter", 1)).AddPrecondition(instance2.ResearchCompleted("FarmingTech")).AddMinionFilter(instance.HasSkillAptitude(skills.Researching1)));
			this.BonusDigging1 = base.Add(new BonusEvent("BonusDigging1", null, 1, true, 0).TriggerOnWorkableComplete(30, new Type[] { typeof(Diggable) }).AddMinionFilter(instance.Or(instance.HasChoreGroupPriorityOrHigher(Db.Get().ChoreGroups.Dig, 4), instance.HasSkillAptitude(skills.Mining1))).AddPriorityBoost(instance2.MinionsWithChoreGroupPriorityOrGreater(Db.Get().ChoreGroups.Dig, 1, 4), 1));
			this.BonusStorage = base.Add(new BonusEvent("BonusStorage", null, 1, true, 0).TriggerOnUseBuilding(10, new string[] { "StorageLocker" }).AddMinionFilter(instance.Or(instance.HasChoreGroupPriorityOrHigher(Db.Get().ChoreGroups.Hauling, 4), instance.HasSkillAptitude(skills.Hauling1))).AddPrecondition(instance2.BuildingExists("StorageLocker", 1)));
			this.BonusBuilder = base.Add(new BonusEvent("BonusBuilder", null, 1, true, 0).TriggerOnNewBuilding(10, Array.Empty<string>()).AddMinionFilter(instance.Or(instance.HasChoreGroupPriorityOrHigher(Db.Get().ChoreGroups.Build, 4), instance.HasSkillAptitude(skills.Building1))));
			this.BonusOxygen = base.Add(new BonusEvent("BonusOxygen", null, 1, false, 0).TriggerOnUseBuilding(1, new string[] { "MineralDeoxidizer" }).AddPrecondition(instance2.BuildingExists("MineralDeoxidizer", 1)).AddPrecondition(instance2.Not(instance2.PastEventCount("BonusAlgae", 1))));
			this.BonusAlgae = base.Add(new BonusEvent("BonusAlgae", "BonusOxygen", 1, false, 0).TriggerOnUseBuilding(1, new string[] { "AlgaeHabitat" }).AddPrecondition(instance2.BuildingExists("AlgaeHabitat", 1)).AddPrecondition(instance2.Not(instance2.PastEventCount("BonusOxygen", 1))));
			this.BonusGenerator = base.Add(new BonusEvent("BonusGenerator", null, 1, false, 0).TriggerOnUseBuilding(1, new string[] { "ManualGenerator" }).AddPrecondition(instance2.BuildingExists("ManualGenerator", 1)));
			this.BonusDoor = base.Add(new BonusEvent("BonusDoor", null, 1, false, 0).TriggerOnUseBuilding(1, new string[] { "Door" }).SetExtraCondition((BonusEvent.GameplayEventData data) => data.building.GetComponent<Door>().RequestedState == Door.ControlState.Locked).AddPrecondition(instance2.RoomBuilt(roomTypes.Barracks)));
			this.BonusHitTheBooks = base.Add(new BonusEvent("BonusHitTheBooks", null, 1, true, 0).TriggerOnWorkableComplete(1, new Type[]
			{
				typeof(ResearchCenter),
				typeof(NuclearResearchCenterWorkable)
			}).AddPrecondition(instance2.BuildingExists("ResearchCenter", 1)).AddMinionFilter(instance.HasSkillAptitude(skills.Researching1)));
			this.BonusLitWorkspace = base.Add(new BonusEvent("BonusLitWorkspace", null, 1, false, 0).TriggerOnWorkableComplete(1, Array.Empty<Type>()).SetExtraCondition((BonusEvent.GameplayEventData data) => data.workable.currentlyLit).AddPrecondition(instance2.CycleRestriction(10f, float.PositiveInfinity)));
			this.BonusTalker = base.Add(new BonusEvent("BonusTalker", null, 1, true, 0).TriggerOnWorkableComplete(3, new Type[] { typeof(SocialGatheringPointWorkable) }).SetExtraCondition((BonusEvent.GameplayEventData data) => (data.workable as SocialGatheringPointWorkable).timesConversed > 0).AddPrecondition(instance2.CycleRestriction(10f, float.PositiveInfinity)));
		}

		private void VerifyEvents()
		{
			foreach (GameplayEvent gameplayEvent in this.resources)
			{
				if (gameplayEvent.animFileName == null)
				{
					DebugUtil.LogWarningArgs(new object[] { "Gameplay event anim missing: " + gameplayEvent.Id });
				}
				if (gameplayEvent is BonusEvent)
				{
					this.VerifyBonusEvent(gameplayEvent as BonusEvent);
				}
			}
		}

		private void VerifyBonusEvent(BonusEvent e)
		{
			StringEntry stringEntry;
			if (!Strings.TryGet("STRINGS.GAMEPLAY_EVENTS.BONUS." + e.Id.ToUpper() + ".NAME", out stringEntry))
			{
				DebugUtil.DevLogError(string.Concat(new string[]
				{
					"Event [",
					e.Id,
					"]: STRINGS.GAMEPLAY_EVENTS.BONUS.",
					e.Id.ToUpper(),
					" is missing"
				}));
			}
			Effect effect = Db.Get().effects.TryGet(e.effect);
			if (effect == null)
			{
				DebugUtil.DevLogError(string.Concat(new string[] { "Effect ", e.effect, "[", e.Id, "]: Missing from spreadsheet" }));
				return;
			}
			if (!Strings.TryGet("STRINGS.DUPLICANTS.MODIFIERS." + effect.Id.ToUpper() + ".NAME", out stringEntry))
			{
				DebugUtil.DevLogError(string.Concat(new string[]
				{
					"Effect ",
					e.effect,
					"[",
					e.Id,
					"]: STRINGS.DUPLICANTS.MODIFIERS.",
					effect.Id.ToUpper(),
					".NAME is missing"
				}));
			}
			if (!Strings.TryGet("STRINGS.DUPLICANTS.MODIFIERS." + effect.Id.ToUpper() + ".TOOLTIP", out stringEntry))
			{
				DebugUtil.DevLogError(string.Concat(new string[]
				{
					"Effect ",
					e.effect,
					"[",
					e.Id,
					"]: STRINGS.DUPLICANTS.MODIFIERS.",
					effect.Id.ToUpper(),
					".TOOLTIP is missing"
				}));
			}
		}

		public GameplayEvent HatchSpawnEvent;

		public GameplayEvent PartyEvent;

		public GameplayEvent EclipseEvent;

		public GameplayEvent SatelliteCrashEvent;

		public GameplayEvent FoodFightEvent;

		public GameplayEvent PrickleFlowerBlightEvent;

		public GameplayEvent MeteorShowerIronEvent;

		public GameplayEvent MeteorShowerGoldEvent;

		public GameplayEvent MeteorShowerCopperEvent;

		public GameplayEvent MeteorShowerDustEvent;

		public GameplayEvent MeteorShowerFullereneEvent;

		public GameplayEvent GassyMooteorEvent;

		public GameplayEvent ClusterSnowShower;

		public GameplayEvent ClusterIceShower;

		public GameplayEvent ClusterBiologicalShower;

		public GameplayEvent ClusterLightRegolithShower;

		public GameplayEvent ClusterRegolithShower;

		public GameplayEvent ClusterGoldShower;

		public GameplayEvent ClusterCopperShower;

		public GameplayEvent ClusterIronShower;

		public GameplayEvent ClusterUraniumShower;

		public GameplayEvent ClusterOxyliteShower;

		public GameplayEvent ClusterBleachStoneShower;

		public GameplayEvent ClusterIceAndTreesShower;

		public GameplayEvent BonusDream1;

		public GameplayEvent BonusDream2;

		public GameplayEvent BonusDream3;

		public GameplayEvent BonusDream4;

		public GameplayEvent BonusToilet1;

		public GameplayEvent BonusToilet2;

		public GameplayEvent BonusToilet3;

		public GameplayEvent BonusToilet4;

		public GameplayEvent BonusResearch;

		public GameplayEvent BonusDigging1;

		public GameplayEvent BonusStorage;

		public GameplayEvent BonusBuilder;

		public GameplayEvent BonusOxygen;

		public GameplayEvent BonusAlgae;

		public GameplayEvent BonusGenerator;

		public GameplayEvent BonusDoor;

		public GameplayEvent BonusHitTheBooks;

		public GameplayEvent BonusLitWorkspace;

		public GameplayEvent BonusTalker;

		public GameplayEvent CryoFriend;

		public GameplayEvent WarpWorldReveal;

		public GameplayEvent ArtifactReveal;
	}
}
