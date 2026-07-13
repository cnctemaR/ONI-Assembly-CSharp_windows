using System;
using System.Collections.Generic;
using Klei.CustomSettings;
using KSerialization;
using ProcGen;
using UnityEngine;

public class LargeImpactorEvent : GameplayEvent<LargeImpactorEvent.StatesInstance>
{
	public LargeImpactorEvent(string id, string[] requiredDlcIds, string[] forbiddenDlcIds)
		: base(id, 0, 0, requiredDlcIds, forbiddenDlcIds)
	{
	}

	public override StateMachine.Instance GetSMI(GameplayEventManager manager, GameplayEventInstance eventInstance)
	{
		return new LargeImpactorEvent.StatesInstance(manager, eventInstance, this);
	}

	private static void SpawnIridiumShowers(LargeImpactorEvent.StatesInstance smi)
	{
		GameplayEventManager.Instance.StartNewEvent(Db.Get().GameplayEvents.IridiumShowerEvent, smi.eventInstance.worldId, null);
	}

	private static void PreventDemoliorFragmentsBGFromPlaying(LargeImpactorEvent.StatesInstance smi)
	{
		TerrainBG.preventLargeImpactorFragmentsFromProgressing = true;
	}

	private static void AllowDemoliorFragmentsBGFromPlaying(LargeImpactorEvent.StatesInstance smi)
	{
		TerrainBG.preventLargeImpactorFragmentsFromProgressing = false;
	}

	private static void DestroyEventInstance(LargeImpactorEvent.StatesInstance smi)
	{
		smi.eventInstance.smi.StopSM("end");
	}

	private static bool WasWinAchievementAlreadyGranted(LargeImpactorEvent.StatesInstance smi)
	{
		return SaveGame.Instance.ColonyAchievementTracker.IsAchievementUnlocked(Db.Get().ColonyAchievements.AsteroidDestroyed);
	}

	private static void UnlockWinAchievement(LargeImpactorEvent.StatesInstance smi)
	{
		SaveGame.Instance.ColonyAchievementTracker.largeImpactorState = ColonyAchievementTracker.LargeImpactorState.Defeated;
	}

	private static void RegisterDemoliorSize(LargeImpactorEvent.StatesInstance smi)
	{
		ParallaxBackgroundObject component = smi.impactorInstance.GetComponent<ParallaxBackgroundObject>();
		SaveGame.Instance.ColonyAchievementTracker.LargeImpactorBackgroundScale = component.lastScaleUsed;
	}

	private static void RegisterLandedCycle(LargeImpactorEvent.StatesInstance smi)
	{
		SaveGame.Instance.ColonyAchievementTracker.largeImpactorState = ColonyAchievementTracker.LargeImpactorState.Landed;
		SaveGame.Instance.ColonyAchievementTracker.largeImpactorLandedCycle = GameClock.Instance.GetCycle();
	}

	private static bool IsSuitablePOISpawnLocation(AxialI location)
	{
		if (!ClusterGrid.Instance.IsValidCell(location))
		{
			return false;
		}
		foreach (ClusterGridEntity clusterGridEntity in ClusterGrid.Instance.GetEntitiesOnCell(location))
		{
			if (clusterGridEntity.Layer == EntityLayer.Asteroid || clusterGridEntity.Layer == EntityLayer.POI)
			{
				return false;
			}
		}
		return true;
	}

	private static List<AxialI> FindAvailablePOISpawnLocations(AxialI location)
	{
		List<AxialI> list = new List<AxialI>();
		if (LargeImpactorEvent.IsSuitablePOISpawnLocation(location))
		{
			list.Add(location);
		}
		for (int i = 1; i <= 2; i++)
		{
			foreach (AxialI axialI in AxialI.DIRECTIONS)
			{
				AxialI axialI2 = location + axialI * i;
				if (LargeImpactorEvent.IsSuitablePOISpawnLocation(axialI2))
				{
					list.Add(axialI2);
				}
			}
		}
		return list;
	}

	private static void SpawnPOI(string id, AxialI location)
	{
		GameObject gameObject = global::Util.KInstantiate(Assets.GetPrefab(id), null, null);
		gameObject.GetComponent<HarvestablePOIClusterGridEntity>().Init(location);
		gameObject.SetActive(true);
	}

	private static void HandleInterception(LargeImpactorEvent.StatesInstance smi)
	{
		if (DlcManager.IsExpansion1Active())
		{
			List<AxialI> list = LargeImpactorEvent.FindAvailablePOISpawnLocations(smi.impactorInstance.GetSMI<ClusterMapLargeImpactor.Instance>().ClusterGridPosition());
			if (list.Count > 0)
			{
				LargeImpactorEvent.SpawnPOI("HarvestableSpacePOI_DLC4ImpactorDebrisField1", list[0]);
			}
			if (list.Count > 1)
			{
				LargeImpactorEvent.SpawnPOI("HarvestableSpacePOI_DLC4ImpactorDebrisField2", list[1]);
			}
			if (list.Count > 2)
			{
				LargeImpactorEvent.SpawnPOI("HarvestableSpacePOI_DLC4ImpactorDebrisField3", list[2]);
			}
		}
		else
		{
			if (!SpacecraftManager.instance.AddDestination(Db.Get().SpaceDestinationTypes.DLC4PrehistoricDemoliorSpaceDestination.Id, SpacecraftManager.DestinationLocationSelectionType.Nearest, 0, 2147483647, 3))
			{
				SpacecraftManager.instance.AddDestination(Db.Get().SpaceDestinationTypes.DLC4PrehistoricDemoliorSpaceDestination.Id, SpacecraftManager.DestinationLocationSelectionType.Random, 0, int.MaxValue, 5);
			}
			if (!SpacecraftManager.instance.AddDestination(Db.Get().SpaceDestinationTypes.DLC4PrehistoricDemoliorSpaceDestination2.Id, SpacecraftManager.DestinationLocationSelectionType.Random, 1, 5, 5))
			{
				SpacecraftManager.instance.AddDestination(Db.Get().SpaceDestinationTypes.DLC4PrehistoricDemoliorSpaceDestination2.Id, SpacecraftManager.DestinationLocationSelectionType.Random, 0, int.MaxValue, 5);
			}
			if (!SpacecraftManager.instance.AddDestination(Db.Get().SpaceDestinationTypes.DLC4PrehistoricDemoliorSpaceDestination3.Id, SpacecraftManager.DestinationLocationSelectionType.Random, 1, 5, 5))
			{
				SpacecraftManager.instance.AddDestination(Db.Get().SpaceDestinationTypes.DLC4PrehistoricDemoliorSpaceDestination3.Id, SpacecraftManager.DestinationLocationSelectionType.Random, 0, int.MaxValue, 5);
			}
		}
		smi.GoTo(smi.sm.finished);
	}

	private static bool WasKilled(LargeImpactorEvent.StatesInstance smi, object _)
	{
		return smi.impactorInstance.GetSMI<LargeImpactorStatus.Instance>().Health <= 0;
	}

	private static void PrepareForLargeImpactorDefeatedSequence(LargeImpactorEvent.StatesInstance smi)
	{
		smi.impactorInstance.GetComponent<LargeImpactorCrashStamp>();
		LargeImpactorEvent.ToggleOffLandingZoneVisualizer(smi);
		ClusterManager.Instance.GetWorld(smi.eventInstance.worldId).RevealSurface();
	}

	private static void InitializeLandingSequence(LargeImpactorEvent.StatesInstance smi)
	{
		GameObject impactorInstance = smi.impactorInstance;
		LargeImpactorCrashStamp component = impactorInstance.GetComponent<LargeImpactorCrashStamp>();
		ParallaxBackgroundObject component2 = impactorInstance.GetComponent<ParallaxBackgroundObject>();
		LargeImpactorEvent.ToggleOffLandingZoneVisualizer(smi);
		WorldContainer world = ClusterManager.Instance.GetWorld(smi.eventInstance.worldId);
		world.RevealHiddenY();
		world.RevealSurface();
		component.RevealFogOfWar(7);
		component2.SetVisibilityState(false);
		LargeComet largeComet = LargeImpactorEvent.CreateLargeImpactorInWorldFallingAsteroid(smi, component, world);
		LargeImpactorLandingSequence.Start(component, largeComet, component, world.id);
	}

	private static void ToggleOffLandingZoneVisualizer(LargeImpactorEvent.StatesInstance smi)
	{
		LargeImpactorVisualizer component = smi.impactorInstance.GetComponent<LargeImpactorVisualizer>();
		if (component.Active)
		{
			component.Active = false;
		}
	}

	private static LargeComet CreateLargeImpactorInWorldFallingAsteroid(LargeImpactorEvent.StatesInstance smi, LargeImpactorCrashStamp crashStamp, WorldContainer world)
	{
		TemplateContainer asteroidTemplate = crashStamp.asteroidTemplate;
		Vector2I stampLocation = crashStamp.stampLocation;
		float layerZ = Grid.GetLayerZ(Grid.SceneLayer.FXFront);
		Vector3 vector = new Vector3((float)stampLocation.X, (float)(world.Height - world.HiddenYOffset - 1), layerZ);
		GameObject gameObject = global::Util.KInstantiate(Assets.GetPrefab(LargeImpactorCometConfig.ID), vector, Quaternion.identity, null, null, true, 0);
		LargeComet component = gameObject.GetComponent<LargeComet>();
		gameObject.SetActive(true);
		component.stampLocation = stampLocation;
		component.crashPosition = stampLocation;
		LargeComet largeComet = component;
		largeComet.crashPosition.y = largeComet.crashPosition.y + asteroidTemplate.GetTemplateBounds(0).yMin;
		component.asteroidTemplate = asteroidTemplate;
		component.bottomCellsOffsetOfTemplate = crashStamp.TemplateBottomCellsOffsets;
		return component;
	}

	private static GameObject CreateSpacedOutImpactorInstance(LargeImpactorEvent.StatesInstance smi)
	{
		if (!DlcManager.IsExpansion1Active() || ClusterGrid.Instance == null)
		{
			return null;
		}
		GameObject gameObject = global::Util.KInstantiate(Assets.GetPrefab("LargeImpactor"), null, null);
		float num = smi.eventInstance.eventStartTime * 600f + LargeImpactorEvent.GetImpactTime();
		AxialI location = ClusterManager.Instance.GetClusterPOIManager().GetTemporalTear().Location;
		ClusterMapMeteorShowerVisualizer component = gameObject.GetComponent<ClusterMapMeteorShowerVisualizer>();
		component.SetInitialLocation(location);
		component.forceRevealed = true;
		ClusterMapLargeImpactor.Def def = gameObject.AddOrGetDef<ClusterMapLargeImpactor.Def>();
		def.destinationWorldID = 0;
		def.arrivalTime = num;
		gameObject.AddOrGet<ParallaxBackgroundObject>().worldId = new int?(smi.eventInstance.worldId);
		return gameObject;
	}

	private static GameObject CreateVanillaImpactorInstance(LargeImpactorEvent.StatesInstance smi)
	{
		if (DlcManager.IsExpansion1Active())
		{
			return null;
		}
		return global::Util.KInstantiate(Assets.GetPrefab(LargeImpactorVanillaConfig.ID), null, null);
	}

	public static void CreateImpactorInstance(LargeImpactorEvent.StatesInstance smi)
	{
		GameObject gameObject;
		if (DlcManager.IsExpansion1Active())
		{
			gameObject = LargeImpactorEvent.CreateSpacedOutImpactorInstance(smi);
		}
		else
		{
			gameObject = LargeImpactorEvent.CreateVanillaImpactorInstance(smi);
		}
		if (gameObject == null)
		{
			KCrashReporter.ReportDevNotification("Failed to create LargeImpactor Object.", Environment.StackTrace, "", false, null);
			smi.StopSM("No Impactor created");
			return;
		}
		gameObject.SetActive(true);
		smi.sm.impactorTarget.Set(gameObject.GetComponent<KPrefabID>(), smi);
	}

	public static float GetImpactTime()
	{
		ClusterLayout currentClusterLayout = CustomGameSettings.Instance.GetCurrentClusterLayout();
		if (currentClusterLayout != null && currentClusterLayout.clusterTags.Contains("DemoliorImminentImpact"))
		{
			return 6000f;
		}
		float num = 200f;
		SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.DemoliorDifficulty);
		if (currentQualitySetting.id == "VeryHard")
		{
			num = 100f;
		}
		else if (currentQualitySetting.id == "Hard")
		{
			num = 150f;
		}
		else if (currentQualitySetting.id == "Easy")
		{
			num = 300f;
		}
		else if (currentQualitySetting.id == "VeryEasy")
		{
			num = 500f;
		}
		return num * 600f;
	}

	public class States : GameplayEventStateMachine<LargeImpactorEvent.States, LargeImpactorEvent.StatesInstance, GameplayEventManager, LargeImpactorEvent>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			base.serializable = StateMachine.SerializeType.ParamsOnly;
			base.InitializeStates(out default_state);
			default_state = this.start;
			this.start.ParamTransition<GameObject>(this.impactorTarget, this.create, GameStateMachine<LargeImpactorEvent.States, LargeImpactorEvent.StatesInstance, GameplayEventManager, object>.IsNull).ParamTransition<GameObject>(this.impactorTarget, this.clusterMap, GameStateMachine<LargeImpactorEvent.States, LargeImpactorEvent.StatesInstance, GameplayEventManager, object>.IsNotNull);
			this.create.ParamTransition<GameObject>(this.impactorTarget, this.clusterMap, GameStateMachine<LargeImpactorEvent.States, LargeImpactorEvent.StatesInstance, GameplayEventManager, object>.IsNotNull).Enter(delegate(LargeImpactorEvent.StatesInstance smi)
			{
				LargeImpactorEvent.CreateImpactorInstance(smi);
			});
			this.clusterMap.Target(this.impactorTarget).EventTransition(GameHashes.LargeImpactorArrived, this.impacting, null).EventTransition(GameHashes.Died, this.killedByPlayer, null);
			this.impacting.Enter(new StateMachine<LargeImpactorEvent.States, LargeImpactorEvent.StatesInstance, GameplayEventManager, object>.State.Callback(LargeImpactorEvent.RegisterLandedCycle)).Enter(new StateMachine<LargeImpactorEvent.States, LargeImpactorEvent.StatesInstance, GameplayEventManager, object>.State.Callback(LargeImpactorEvent.InitializeLandingSequence)).Target(this.impactorTarget)
				.EventTransition(GameHashes.SequenceCompleted, this.finished, null);
			this.killedByPlayer.EnterTransition(this.finished, new StateMachine<LargeImpactorEvent.States, LargeImpactorEvent.StatesInstance, GameplayEventManager, object>.Transition.ConditionCallback(LargeImpactorEvent.WasWinAchievementAlreadyGranted)).Enter(new StateMachine<LargeImpactorEvent.States, LargeImpactorEvent.StatesInstance, GameplayEventManager, object>.State.Callback(LargeImpactorEvent.PrepareForLargeImpactorDefeatedSequence)).Enter(new StateMachine<LargeImpactorEvent.States, LargeImpactorEvent.StatesInstance, GameplayEventManager, object>.State.Callback(LargeImpactorEvent.PreventDemoliorFragmentsBGFromPlaying))
				.Enter(new StateMachine<LargeImpactorEvent.States, LargeImpactorEvent.StatesInstance, GameplayEventManager, object>.State.Callback(LargeImpactorEvent.UnlockWinAchievement))
				.Enter(new StateMachine<LargeImpactorEvent.States, LargeImpactorEvent.StatesInstance, GameplayEventManager, object>.State.Callback(LargeImpactorEvent.RegisterDemoliorSize))
				.Exit(new StateMachine<LargeImpactorEvent.States, LargeImpactorEvent.StatesInstance, GameplayEventManager, object>.State.Callback(LargeImpactorEvent.AllowDemoliorFragmentsBGFromPlaying))
				.Exit(new StateMachine<LargeImpactorEvent.States, LargeImpactorEvent.StatesInstance, GameplayEventManager, object>.State.Callback(LargeImpactorEvent.SpawnIridiumShowers))
				.Target(this.impactorTarget)
				.EventHandler(GameHashes.SequenceCompleted, new StateMachine<LargeImpactorEvent.States, LargeImpactorEvent.StatesInstance, GameplayEventManager, object>.State.Callback(LargeImpactorEvent.HandleInterception));
			this.finished.Enter(delegate(LargeImpactorEvent.StatesInstance smi)
			{
				global::Util.KDestroyGameObject(smi.sm.impactorTarget.Get(smi));
			}).Enter(new StateMachine<LargeImpactorEvent.States, LargeImpactorEvent.StatesInstance, GameplayEventManager, object>.State.Callback(LargeImpactorEvent.DestroyEventInstance)).GoTo(null);
		}

		public GameStateMachine<LargeImpactorEvent.States, LargeImpactorEvent.StatesInstance, GameplayEventManager, object>.State start;

		public GameStateMachine<LargeImpactorEvent.States, LargeImpactorEvent.StatesInstance, GameplayEventManager, object>.State create;

		public GameStateMachine<LargeImpactorEvent.States, LargeImpactorEvent.StatesInstance, GameplayEventManager, object>.State clusterMap;

		public GameStateMachine<LargeImpactorEvent.States, LargeImpactorEvent.StatesInstance, GameplayEventManager, object>.State killedByPlayer;

		public GameStateMachine<LargeImpactorEvent.States, LargeImpactorEvent.StatesInstance, GameplayEventManager, object>.State impacting;

		public GameStateMachine<LargeImpactorEvent.States, LargeImpactorEvent.StatesInstance, GameplayEventManager, object>.State finished;

		[Serialize]
		public StateMachine<LargeImpactorEvent.States, LargeImpactorEvent.StatesInstance, GameplayEventManager, object>.TargetParameter impactorTarget = new StateMachine<LargeImpactorEvent.States, LargeImpactorEvent.StatesInstance, GameplayEventManager, object>.TargetParameter();
	}

	public class StatesInstance : GameplayEventStateMachine<LargeImpactorEvent.States, LargeImpactorEvent.StatesInstance, GameplayEventManager, LargeImpactorEvent>.GameplayEventStateMachineInstance
	{
		public GameObject impactorInstance
		{
			get
			{
				return base.sm.impactorTarget.Get(base.smi);
			}
		}

		public StatesInstance(GameplayEventManager master, GameplayEventInstance eventInstance, LargeImpactorEvent largeImpactorEvent)
			: base(master, eventInstance, largeImpactorEvent)
		{
		}
	}
}
