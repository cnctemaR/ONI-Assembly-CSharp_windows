using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FMOD.Studio;
using Klei;
using Klei.CustomSettings;
using KSerialization;
using ProcGenGame;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class Game : KMonoBehaviour
{
	public static bool IsQuitting()
	{
		return Game.quitting;
	}

	public KInputHandler inputHandler { get; set; }

	public static Game Instance { get; private set; }

	public bool SandboxModeActive
	{
		get
		{
			return this.sandboxModeActive;
		}
		set
		{
			this.sandboxModeActive = value;
			base.Trigger(-1948169901, null);
			if (PlanScreen.Instance != null)
			{
				PlanScreen.Instance.Refresh();
			}
			if (BuildMenu.Instance != null)
			{
				BuildMenu.Instance.Refresh();
			}
		}
	}

	public StatusItemRenderer statusItemRenderer { get; private set; }

	public PrioritizableRenderer prioritizableRenderer { get; private set; }

	protected override void OnPrefabInit()
	{
		Output.Log(new object[]
		{
			Time.realtimeSinceStartup,
			"Level Loaded....",
			SceneManager.GetActiveScene().name
		});
		SimTemperatureTransfer.ClearInstanceMap();
		StructureTemperatureComponents.ClearInstanceMap();
		App.OnPreLoadScene = (global::System.Action)Delegate.Combine(App.OnPreLoadScene, new global::System.Action(this.StopBE));
		Game.Instance = this;
		this.statusItemRenderer = new StatusItemRenderer();
		this.prioritizableRenderer = new PrioritizableRenderer();
		CellChangeMonitor.Destroy();
		this.LoadEventHashes();
		this.gasFlowPos = new Vector3(0f, 0f, Grid.GetLayerZ(Grid.SceneLayer.GasConduits) - 0.4f);
		this.liquidFlowPos = new Vector3(0f, 0f, Grid.GetLayerZ(Grid.SceneLayer.LiquidConduits) - 0.4f);
		this.solidFlowPos = new Vector3(0f, 0f, Grid.GetLayerZ(Grid.SceneLayer.SolidConduitContents) - 0.4f);
		Shader.WarmupAllShaders();
		Db.Get();
		Game.quitting = false;
		Game.PickupableLayer = LayerMask.NameToLayer("Pickupable");
		Game.PickupableLayerMask = LayerMask.GetMask(new string[] { "Pickupable" });
		this.ColliderRoot = global::Util.NewGameObject(base.gameObject, "Colliders");
		Game.BlockSelectionLayer = LayerMask.NameToLayer("BlockSelection");
		Game.BlockSelectionLayerMask = LayerMask.GetMask(new string[] { "BlockSelection" });
		this.world = World.Instance;
		KPrefabID.NextUniqueID = KPlayerPrefs.GetInt(Game.NextUniqueIDKey, 0);
		this.circuitManager = new CircuitManager();
		this.emergySim = new EnergySim();
		this.elementInteractions = new ElementInteractions(this.elementInteractionsData);
		this.gasConduitSystem = new UtilityNetworkManager<FlowUtilityNetwork, Vent>(Grid.WidthInCells, Grid.HeightInCells, 13);
		this.liquidConduitSystem = new UtilityNetworkManager<FlowUtilityNetwork, Vent>(Grid.WidthInCells, Grid.HeightInCells, 17);
		this.electricalConduitSystem = new UtilityNetworkManager<ElectricalUtilityNetwork, Wire>(Grid.WidthInCells, Grid.HeightInCells, 25);
		this.logicCircuitSystem = new UtilityNetworkManager<LogicCircuitNetwork, LogicWire>(Grid.WidthInCells, Grid.HeightInCells, 30);
		this.logicCircuitManager = new LogicCircuitManager(this.logicCircuitSystem);
		this.travelTubeSystem = new UtilityNetworkTubesManager(Grid.WidthInCells, Grid.HeightInCells, 32);
		this.solidConduitSystem = new UtilityNetworkManager<FlowUtilityNetwork, SolidConduit>(Grid.WidthInCells, Grid.HeightInCells, 21);
		this.conduitTemperatureManager = new ConduitTemperatureManager();
		this.conduitDiseaseManager = new ConduitDiseaseManager(this.conduitTemperatureManager);
		this.gasConduitFlow = new ConduitFlow(ConduitType.Gas, Grid.CellCount, this.gasConduitSystem, 1f, 0.25f);
		this.liquidConduitFlow = new ConduitFlow(ConduitType.Liquid, Grid.CellCount, this.liquidConduitSystem, 10f, 0.75f);
		this.solidConduitFlow = new SolidConduitFlow(Grid.CellCount, this.solidConduitSystem, 0.75f);
		this.gasFlowVisualizer = new ConduitFlowVisualizer(this.gasConduitFlow, this.gasConduitVisInfo, GlobalResources.Instance().ConduitOverlaySoundGas, Lighting.Instance.Settings.GasConduit);
		this.liquidFlowVisualizer = new ConduitFlowVisualizer(this.liquidConduitFlow, this.liquidConduitVisInfo, GlobalResources.Instance().ConduitOverlaySoundLiquid, Lighting.Instance.Settings.LiquidConduit);
		this.solidFlowVisualizer = new SolidConduitFlowVisualizer(this.solidConduitFlow, this.solidConduitVisInfo, GlobalResources.Instance().ConduitOverlaySoundSolid, Lighting.Instance.Settings.SolidConduit);
		this.accumulators = new Accumulators();
		this.plantElementAbsorbers = new PlantElementAbsorbers();
		this.activeFX = new ushort[Grid.CellCount];
		this.simActiveRegionMax = new Vector2I(0, 0);
		this.simActiveRegionMin = new Vector2I(Grid.WidthInCells - 1, Grid.HeightInCells - 1);
		this.UnsafePrefabInit();
		Shader.SetGlobalVector("_MetalParameters", new Vector4(0f, 0f, 0f, 0f));
		Shader.SetGlobalVector("_WaterParameters", new Vector4(0f, 0f, 0f, 0f));
		this.InitializeFXSpawners();
		PathFinder.Initialize();
		new GameNavGrids(Pathfinding.Instance);
		this.screenMgr = global::Util.KInstantiate(this.screenManagerPrefab, null, null).GetComponent<GameScreenManager>();
		this.roleManager = new RoleManager();
		this.roomProber = new RoomProber();
		this.roomProber.Init();
		CellChangeMonitor.Instance.SetGridSize(Grid.WidthInCells, Grid.HeightInCells);
	}

	public void SetGameStarted()
	{
		this.gameStarted = true;
	}

	public bool GameStarted()
	{
		return this.gameStarted;
	}

	private void UnsafePrefabInit()
	{
		this.StepTheSim(0.2f);
	}

	protected override void OnLoadLevel()
	{
		base.Unsubscribe(1798162660, new Action<object>(this.MarkStatusItemRendererDirty));
		base.OnLoadLevel();
	}

	private void MarkStatusItemRendererDirty(object data)
	{
		this.statusItemRenderer.MarkAllDirty();
	}

	protected override void OnForcedCleanUp()
	{
		if (this.prioritizableRenderer != null)
		{
			this.prioritizableRenderer.Cleanup();
			this.prioritizableRenderer = null;
		}
		if (this.statusItemRenderer != null)
		{
			this.statusItemRenderer.Destroy();
			this.statusItemRenderer = null;
		}
		if (this.conduitTemperatureManager != null)
		{
			this.conduitTemperatureManager.Shutdown();
		}
		this.gasFlowVisualizer.FreeResources();
		this.liquidFlowVisualizer.FreeResources();
		this.solidFlowVisualizer.FreeResources();
		LightGridManager.Shutdown();
		App.OnPreLoadScene = (global::System.Action)Delegate.Remove(App.OnPreLoadScene, new global::System.Action(this.StopBE));
		base.OnForcedCleanUp();
	}

	protected override void OnSpawn()
	{
		this.LocalPlayer = this.SpawnPlayer();
		WaterCubes.Instance.Init();
		SpeedControlScreen.Instance.Pause(false);
		LightGridManager.Initialise();
		this.UnsafeOnSpawn();
		Time.timeScale = 0f;
		if (this.tempIntroScreenPrefab != null)
		{
			global::Util.KInstantiate(this.tempIntroScreenPrefab, null, null);
		}
		if (SaveLoader.Instance.cachedGSD != null)
		{
			this.Reset(SaveLoader.Instance.cachedGSD);
			NewBaseScreen.SetInitialCamera();
		}
		TagManager.FillMissingProperNames();
		CameraController.Instance.SetOrthographicsSize(20f);
		this.customSettings = global::UnityEngine.Object.FindObjectOfType<CustomGameSettings>();
		if (SaveLoader.Instance.loadedFromSave)
		{
			this.baseAlreadyCreated = true;
			base.Trigger(-1992507039, null);
			base.Trigger(-838649377, null);
		}
		else
		{
			this.ResetTime();
		}
		KScreen kscreen = this.LocalPlayer.ScreenManager.StartScreen(ScreenPrefabs.Instance.ResourceCategoryScreen.gameObject, null, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
		kscreen.transform.SetSiblingIndex(1);
		foreach (MeshRenderer meshRenderer in Resources.FindObjectsOfTypeAll(typeof(MeshRenderer)))
		{
			meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
		}
		base.Subscribe(1798162660, new Action<object>(this.MarkStatusItemRendererDirty));
		this.solidConduitFlow.Initialize();
		SimAndRenderScheduler.instance.Add(this.roomProber, false);
		SimAndRenderScheduler.instance.Add(KComponentSpawn.instance, false);
		if (!SaveLoader.Instance.loadedFromSave)
		{
			SettingConfig settingConfig = Game.Instance.customSettings.QualitySettings["SandboxMode"];
			SettingLevel currentQualitySetting = Game.Instance.customSettings.GetCurrentQualitySetting("SandboxMode");
			SaveGame.Instance.sandboxEnabled = !settingConfig.IsDefaultLevel(currentQualitySetting.id);
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		SimAndRenderScheduler.instance.Remove(KComponentSpawn.instance);
	}

	private void UnsafeOnSpawn()
	{
		this.world.UpdateCellInfo(this.gameSolidInfo, this.callbackInfo, 0, null, 0, null);
	}

	public void SetMusicEnabled(bool enabled)
	{
		if (enabled)
		{
			MusicManager.instance.PlaySong("Music_FrontEnd", false);
		}
		else
		{
			MusicManager.instance.StopSong("Music_FrontEnd", true, STOP_MODE.ALLOWFADEOUT);
		}
	}

	private Player SpawnPlayer()
	{
		GameObject gameObject = global::Util.KInstantiate(this.playerPrefab, base.gameObject, null);
		Player component = gameObject.GetComponent<Player>();
		component.ScreenManager = this.screenMgr;
		component.ScreenManager.StartScreen(ScreenPrefabs.Instance.HudScreen.gameObject, null, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
		component.ScreenManager.StartScreen(ScreenPrefabs.Instance.HoverTextScreen.gameObject, null, GameScreenManager.UIRenderTarget.HoverTextScreen);
		component.ScreenManager.StartScreen(ScreenPrefabs.Instance.ToolTipScreen.gameObject, null, GameScreenManager.UIRenderTarget.HoverTextScreen);
		this.cameraController = global::Util.KInstantiate(this.cameraControllerPrefab, SceneOrganizer.Instance.GetFolder(Folder.Cameras), null).GetComponent<CameraController>();
		component.CameraController = this.cameraController;
		KInputHandler.Add(Global.Instance.GetInputManager().GetDefaultController(), this.cameraController, 1);
		this.playerController = component.GetComponent<PlayerController>();
		KInputHandler.Add(Global.Instance.GetInputManager().GetDefaultController(), this.playerController, 20);
		return component;
	}

	public void SetForceField(int cell, bool force_field, bool solid)
	{
		Grid.ForceField[cell] = force_field;
		this.gameSolidInfo.Add(new SolidInfo(cell, solid));
	}

	private unsafe Sim.GameDataUpdate* StepTheSim(float dt)
	{
		Sim.GameDataUpdate* ptr;
		using (new KProfiler.Region("StepTheSim", null))
		{
			IntPtr intPtr = IntPtr.Zero;
			using (new KProfiler.Region("WaitingForSim", null))
			{
				if (Grid.Visible == null || Grid.Visible.Length == 0)
				{
					Output.LogError(new object[] { "Invalid Grid.Visible, what have you done?!" });
					return null;
				}
				intPtr = Sim.HandleMessage(SimMessageHashes.PrepareGameData, Grid.Visible.Length, Grid.Visible);
			}
			if (intPtr == IntPtr.Zero)
			{
				ptr = null;
			}
			else
			{
				Sim.GameDataUpdate* ptr2 = (Sim.GameDataUpdate*)(void*)intPtr;
				Grid.elementIdx = ptr2->elementIdx;
				Grid.temperature = ptr2->temperature;
				Grid.mass = ptr2->mass;
				Grid.properties = ptr2->properties;
				Grid.strengthInfo = ptr2->strengthInfo;
				Grid.insulation = ptr2->insulation;
				Grid.diseaseIdx = ptr2->diseaseIdx;
				Grid.diseaseCount = ptr2->diseaseCount;
				Grid.AccumulatedFlowValues = ptr2->accumulatedFlow;
				PropertyTextures.externalFlowTex = ptr2->propertyTextureFlow;
				PropertyTextures.externalLiquidTex = ptr2->propertyTextureLiquid;
				List<Element> elements = ElementLoader.elements;
				this.simData.emittedMassEntries = ptr2->emittedMassEntries;
				this.simData.elementChunks = ptr2->elementChunkInfos;
				this.simData.buildingTemperatures = ptr2->buildingTemperatures;
				this.simData.diseaseEmittedInfos = ptr2->diseaseEmittedInfos;
				this.simData.diseaseConsumedInfos = ptr2->diseaseConsumedInfos;
				for (int i = 0; i < ptr2->numSubstanceChangeInfo; i++)
				{
					Sim.SubstanceChangeInfo substanceChangeInfo = ptr2->substanceChangeInfo[i];
					Element element = elements[(int)substanceChangeInfo.newElemIdx];
					Grid.Element[substanceChangeInfo.cellIdx] = element;
				}
				for (int j = 0; j < ptr2->numSolidInfo; j++)
				{
					Sim.SolidInfo solidInfo = ptr2->solidInfo[j];
					if (!this.solidChangedFilter.Contains(solidInfo.cellIdx))
					{
						this.solidInfo.Add(new SolidInfo(solidInfo.cellIdx, solidInfo.isSolid != 0));
						Grid.PreviousSolid[solidInfo.cellIdx] = Grid.Solid[solidInfo.cellIdx];
						bool flag = solidInfo.isSolid != 0;
						Grid.SetSolid(solidInfo.cellIdx, flag, CellEventLogger.Instance.SimMessagesSolid);
					}
				}
				for (int k = 0; k < ptr2->numCallbackInfo; k++)
				{
					Sim.CallbackInfo callbackInfo = ptr2->callbackInfo[k];
					HandleVector<Game.CallbackInfo>.Handle handle = new HandleVector<Game.CallbackInfo>.Handle
					{
						index = callbackInfo.callbackIdx
					};
					this.callbackInfo.Add(new global::Klei.CallbackInfo(handle));
				}
				int numSpawnFallingLiquidInfo = ptr2->numSpawnFallingLiquidInfo;
				for (int l = 0; l < numSpawnFallingLiquidInfo; l++)
				{
					Sim.SpawnFallingLiquidInfo spawnFallingLiquidInfo = ptr2->spawnFallingLiquidInfo[l];
					FallingWater.instance.AddParticle(spawnFallingLiquidInfo.cellIdx, spawnFallingLiquidInfo.elemIdx, spawnFallingLiquidInfo.mass, spawnFallingLiquidInfo.temperature, spawnFallingLiquidInfo.diseaseIdx, spawnFallingLiquidInfo.diseaseCount, false, false, false, false);
				}
				int numDigInfo = ptr2->numDigInfo;
				WorldDamage component = this.world.GetComponent<WorldDamage>();
				for (int m = 0; m < numDigInfo; m++)
				{
					Sim.SpawnOreInfo spawnOreInfo = ptr2->digInfo[m];
					if (spawnOreInfo.temperature <= 0f && spawnOreInfo.mass > 0f)
					{
						Output.LogError(new object[] { "Sim is telling us to spawn a zero temperature object. This shouldn't be possible because I have asserts in the dll about this...." });
					}
					component.OnDigComplete(spawnOreInfo.cellIdx, spawnOreInfo.mass, spawnOreInfo.temperature, spawnOreInfo.elemIdx, spawnOreInfo.diseaseIdx, spawnOreInfo.diseaseCount);
				}
				int numSpawnOreInfo = ptr2->numSpawnOreInfo;
				for (int n = 0; n < numSpawnOreInfo; n++)
				{
					Sim.SpawnOreInfo spawnOreInfo2 = ptr2->spawnOreInfo[n];
					Vector3 vector = Grid.CellToPosCCC(spawnOreInfo2.cellIdx, Grid.SceneLayer.Ore);
					Element element2 = ElementLoader.elements[(int)spawnOreInfo2.elemIdx];
					if (spawnOreInfo2.temperature <= 0f && spawnOreInfo2.mass > 0f)
					{
						Output.LogError(new object[] { "Sim is telling us to spawn a zero temperature object. This shouldn't be possible because I have asserts in the dll about this...." });
					}
					element2.substance.SpawnResource(vector, spawnOreInfo2.mass, spawnOreInfo2.temperature, spawnOreInfo2.diseaseIdx, spawnOreInfo2.diseaseCount, false, false);
				}
				int numSpawnFXInfo = ptr2->numSpawnFXInfo;
				for (int num = 0; num < numSpawnFXInfo; num++)
				{
					Sim.SpawnFXInfo spawnFXInfo = ptr2->spawnFXInfo[num];
					this.SpawnFX((SpawnFXHashes)spawnFXInfo.fxHash, spawnFXInfo.cellIdx, spawnFXInfo.rotation);
				}
				UnstableGroundManager component2 = this.world.GetComponent<UnstableGroundManager>();
				int numUnstableCellInfo = ptr2->numUnstableCellInfo;
				for (int num2 = 0; num2 < numUnstableCellInfo; num2++)
				{
					Sim.UnstableCellInfo unstableCellInfo = ptr2->unstableCellInfo[num2];
					Sim.UnstableCellInfo.FallingInfo fallingInfo = (Sim.UnstableCellInfo.FallingInfo)unstableCellInfo.fallingInfo;
					if (fallingInfo == Sim.UnstableCellInfo.FallingInfo.StartedFalling)
					{
						component2.Spawn(unstableCellInfo.cellIdx, ElementLoader.elements[(int)unstableCellInfo.elemIdx], unstableCellInfo.mass, unstableCellInfo.temperature, unstableCellInfo.diseaseIdx, unstableCellInfo.diseaseCount);
					}
				}
				int numWorldDamageInfo = ptr2->numWorldDamageInfo;
				for (int num3 = 0; num3 < numWorldDamageInfo; num3++)
				{
					Sim.WorldDamageInfo worldDamageInfo = ptr2->worldDamageInfo[num3];
					WorldDamage.Instance.ApplyDamage(worldDamageInfo);
				}
				for (int num4 = 0; num4 < ptr2->numRemovedMassEntries; num4++)
				{
					Sim.ConsumedMassInfo consumedMassInfo = ptr2->removedMassEntries[num4];
					ElementConsumer.AddMass(consumedMassInfo);
				}
				int numMassConsumedCallbacks = ptr2->numMassConsumedCallbacks;
				HandleVector<Game.ComplexCallbackInfo>.Handle handle2 = default(HandleVector<Game.ComplexCallbackInfo>.Handle);
				for (int num5 = 0; num5 < numMassConsumedCallbacks; num5++)
				{
					Sim.MassConsumedCallback massConsumedCallback = ptr2->massConsumedCallbacks[num5];
					handle2.index = massConsumedCallback.callbackIdx;
					Game.ComplexCallbackInfo complexCallbackInfo = this.complexCallbackManager.Release(handle2);
					if (complexCallbackInfo.cb != null)
					{
						if (massConsumedCallback.GetType() != typeof(Sim.MassConsumedCallback))
						{
							Output.LogError(new object[] { "Somehow a callback from", complexCallbackInfo.debugInfo, "got into the MassEmittedCallbacks list" });
						}
						complexCallbackInfo.cb(massConsumedCallback);
					}
				}
				int numMassEmittedCallbacks = ptr2->numMassEmittedCallbacks;
				HandleVector<Game.ComplexCallbackInfo>.Handle handle3 = default(HandleVector<Game.ComplexCallbackInfo>.Handle);
				for (int num6 = 0; num6 < numMassEmittedCallbacks; num6++)
				{
					Sim.MassEmittedCallback massEmittedCallback = ptr2->massEmittedCallbacks[num6];
					handle3.index = massEmittedCallback.callbackIdx;
					Game.ComplexCallbackInfo item = this.complexCallbackManager.GetItem(handle3);
					if (item.cb != null)
					{
						if (massEmittedCallback.GetType() != typeof(Sim.MassEmittedCallback))
						{
							Output.LogError(new object[] { "Somehow a callback from", item.debugInfo, "got into the MassEmittedCallbacks list" });
						}
						item.cb(massEmittedCallback);
					}
				}
				int numDiseaseConsumptionCallbacks = ptr2->numDiseaseConsumptionCallbacks;
				HandleVector<Game.ComplexCallbackInfo>.Handle handle4 = default(HandleVector<Game.ComplexCallbackInfo>.Handle);
				for (int num7 = 0; num7 < numDiseaseConsumptionCallbacks; num7++)
				{
					Sim.DiseaseConsumptionCallback diseaseConsumptionCallback = ptr2->diseaseConsumptionCallbacks[num7];
					handle4.index = diseaseConsumptionCallback.callbackIdx;
					Game.ComplexCallbackInfo item2 = this.complexCallbackManager.GetItem(handle4);
					if (item2.cb != null)
					{
						item2.cb(diseaseConsumptionCallback);
					}
				}
				int numComponentStateChangedMessages = ptr2->numComponentStateChangedMessages;
				HandleVector<Game.ComplexCallbackInfo>.Handle handle5 = default(HandleVector<Game.ComplexCallbackInfo>.Handle);
				for (int num8 = 0; num8 < numComponentStateChangedMessages; num8++)
				{
					Sim.ComponentStateChangedMessage componentStateChangedMessage = ptr2->componentStateChangedMessages[num8];
					handle5.index = componentStateChangedMessage.callbackIdx;
					Game.ComplexCallbackInfo complexCallbackInfo2 = this.complexCallbackManager.Release(handle5);
					if (complexCallbackInfo2.cb != null)
					{
						complexCallbackInfo2.cb(componentStateChangedMessage.simHandle);
					}
				}
				int numElementChunkMeltedInfos = ptr2->numElementChunkMeltedInfos;
				for (int num9 = 0; num9 < numElementChunkMeltedInfos; num9++)
				{
					Sim.MeltedInfo meltedInfo = ptr2->elementChunkMeltedInfos[num9];
					SimTemperatureTransfer.DoStateTransition(meltedInfo.handle);
				}
				int numBuildingOverheatInfos = ptr2->numBuildingOverheatInfos;
				for (int num10 = 0; num10 < numBuildingOverheatInfos; num10++)
				{
					Sim.MeltedInfo meltedInfo2 = ptr2->buildingOverheatInfos[num10];
					StructureTemperatureComponents.DoOverheat(meltedInfo2.handle);
				}
				int numBuildingNoLongerOverheatedInfos = ptr2->numBuildingNoLongerOverheatedInfos;
				for (int num11 = 0; num11 < numBuildingNoLongerOverheatedInfos; num11++)
				{
					Sim.MeltedInfo meltedInfo3 = ptr2->buildingNoLongerOverheatedInfos[num11];
					StructureTemperatureComponents.DoNoLongerOverheated(meltedInfo3.handle);
				}
				int numBuildingMeltedInfos = ptr2->numBuildingMeltedInfos;
				for (int num12 = 0; num12 < numBuildingMeltedInfos; num12++)
				{
					Sim.MeltedInfo meltedInfo4 = ptr2->buildingMeltedInfos[num12];
					StructureTemperatureComponents.DoStateTransition(meltedInfo4.handle);
				}
				this.conduitTemperatureManager.Sim200ms(0.2f);
				this.conduitDiseaseManager.Sim200ms(0.2f);
				this.gasConduitFlow.Sim200ms(0.2f);
				this.liquidConduitFlow.Sim200ms(0.2f);
				this.solidConduitFlow.Sim200ms(0.2f);
				this.accumulators.Sim200ms(0.2f);
				this.plantElementAbsorbers.Sim200ms(0.2f);
				Sim.DebugProperties debugProperties;
				debugProperties.buildingTemperatureScale = 100f;
				debugProperties.contaminatedOxygenEmitProbability = 0.001f;
				debugProperties.contaminatedOxygenConversionPercent = 0.001f;
				debugProperties.biomeTemperatureLerpRate = 0.001f;
				debugProperties.isDebugEditing = ((!(DebugPaintElementScreen.Instance != null) || !DebugPaintElementScreen.Instance.gameObject.activeSelf) ? 0 : 1);
				debugProperties.pad0 = (debugProperties.pad1 = (debugProperties.pad2 = 0));
				SimMessages.NewGameFrame(dt, this.simActiveRegionMin, this.simActiveRegionMax);
				SimMessages.SetDebugProperties(debugProperties);
				if (this.circuitManager != null)
				{
					this.circuitManager.Sim200msFirst(dt);
				}
				if (this.emergySim != null)
				{
					this.emergySim.EnergySim200ms(dt);
				}
				if (this.logicCircuitManager != null)
				{
					this.logicCircuitManager.Sim200ms(dt);
				}
				if (this.circuitManager != null)
				{
					this.circuitManager.Sim200msLast(dt);
				}
				ptr = ptr2;
			}
		}
		return ptr;
	}

	public void AddSolidChangedFilter(int cell)
	{
		this.solidChangedFilter.Add(cell);
	}

	public void RemoveSolidChangedFilter(int cell)
	{
		this.solidChangedFilter.Remove(cell);
	}

	public void UpdateGameActiveRegion(int x0, int y0, int x1, int y1)
	{
		this.simActiveRegionMin.x = Mathf.Max(0, Mathf.Min(x0, this.simActiveRegionMin.x));
		this.simActiveRegionMin.y = Mathf.Max(0, Mathf.Min(y0, this.simActiveRegionMin.y));
		this.simActiveRegionMax.x = Mathf.Min(Grid.WidthInCells - 1, Mathf.Max(x1, this.simActiveRegionMax.x));
		this.simActiveRegionMax.y = Mathf.Min(Grid.HeightInCells - 1, Mathf.Max(y1, this.simActiveRegionMax.y));
	}

	public void SetIsLoading()
	{
		this.isLoading = true;
	}

	public bool IsLoading()
	{
		return this.isLoading;
	}

	private void ShowDebugCellInfo()
	{
		int mouseCell = DebugHandler.GetMouseCell();
		int num = 0;
		int num2 = 0;
		Grid.CellToXY(mouseCell, out num, out num2);
		string text = string.Concat(new object[]
		{
			mouseCell.ToString(),
			" (",
			num,
			", ",
			num2,
			")"
		});
		DebugText.Instance.Draw(text, Grid.CellToPosCCC(mouseCell, Grid.SceneLayer.Move), Color.white);
	}

	public void ForceSimStep()
	{
		Output.Log(new object[] { "Force-stepping the sim" });
		this.forceSimStep = true;
		this.simDt = 0.2f;
	}

	private void Update()
	{
		if (this.isLoading)
		{
			return;
		}
		using (new KProfiler.Region("Game.Update", null))
		{
			float deltaTime = Time.deltaTime;
			if (global::Debug.developerConsoleVisible)
			{
				global::Debug.developerConsoleVisible = false;
			}
			if (DebugHandler.DebugCellInfo)
			{
				this.ShowDebugCellInfo();
			}
			this.gasConduitSystem.Update();
			this.liquidConduitSystem.Update();
			this.solidConduitSystem.Update();
			this.circuitManager.RenderEveryTick(deltaTime);
			this.logicCircuitManager.RenderEveryTick(deltaTime);
			this.solidConduitFlow.RenderEveryTick(deltaTime);
			if (this.forceActiveArea)
			{
				this.simActiveRegionMin = new Vector2I((int)Mathf.Max(0f, this.minForcedActiveArea.x), (int)Mathf.Max(0f, this.minForcedActiveArea.y));
				this.simActiveRegionMax = new Vector2I((int)Mathf.Min((float)(Grid.WidthInCells - 1), this.maxForcedActiveArea.x), (int)Mathf.Min((float)(Grid.HeightInCells - 1), this.maxForcedActiveArea.y));
			}
			this.simActiveRegionMin = new Vector2I(0, 0);
			this.simActiveRegionMax = new Vector2I(Grid.WidthInCells, Grid.HeightInCells);
			LightGridManager.SetActiveWindow(this.simActiveRegionMin, this.simActiveRegionMax);
			Pathfinding.Instance.RenderEveryTick();
			CellChangeMonitor.Instance.RenderEveryTick();
			if (this.forceSimStep || Mathf.CeilToInt(Time.timeScale) != 0)
			{
				this.SimEveryTick(deltaTime);
				this.forceSimStep = false;
			}
		}
	}

	private void SimEveryTick(float dt)
	{
		dt = Mathf.Min(dt, 0.2f);
		this.simDt += dt;
		while (this.simDt >= 0.016666668f)
		{
			this.simSubTick++;
			this.simSubTick %= 12;
			if (this.simSubTick == 0)
			{
				this.hasFirstSimTickRun = true;
				this.UnsafeSim200ms(0.2f);
			}
			if (this.hasFirstSimTickRun)
			{
				StateMachineUpdater.instance.AdvanceOneSimSubTick();
			}
			this.simDt -= 0.016666668f;
		}
	}

	private unsafe void UnsafeSim200ms(float dt)
	{
		Sim.GameDataUpdate* ptr = this.StepTheSim(dt);
		if (ptr == null)
		{
			global::Debug.LogError("UNEXPECTED!", null);
			return;
		}
		if (ptr->numFramesProcessed <= 0)
		{
			return;
		}
		this.callbackManager.NextFrame();
		this.complexCallbackManager.NextFrame();
		this.gameSolidInfo.AddRange(this.solidInfo);
		this.world.UpdateCellInfo(this.gameSolidInfo, this.callbackInfo, ptr->numSolidSubstanceChangeInfo, ptr->solidSubstanceChangeInfo, ptr->numLiquidChangeInfo, ptr->liquidChangeInfo);
		this.gameSolidInfo.Clear();
		this.solidInfo.Clear();
		this.callbackInfo.Clear();
		Pathfinding.Instance.UpdateNavGrids(false);
	}

	private void LateUpdateComponents()
	{
		if (OverlayScreen.Instance != null)
		{
			SimViewMode mode = OverlayScreen.Instance.GetMode();
			foreach (BuildingCellVisualizer buildingCellVisualizer in Components.BuildingCellVisualizers)
			{
				buildingCellVisualizer.Tick(mode);
			}
		}
	}

	public void ForceOverlayUpdate()
	{
		this.previousOverlayMode = SimViewMode.None;
	}

	private void LateUpdate()
	{
		if (Time.timeScale == 0f && !this.IsPaused)
		{
			this.IsPaused = true;
			base.Trigger(-1788536802, this.IsPaused);
		}
		else if (Time.timeScale != 0f && this.IsPaused)
		{
			this.IsPaused = false;
			base.Trigger(-1788536802, this.IsPaused);
		}
		if (Input.GetMouseButton(0))
		{
			Game.VisualTunerElement = null;
			int mouseCell = DebugHandler.GetMouseCell();
			if (Grid.IsValidCell(mouseCell))
			{
				Element element = Grid.Element[mouseCell];
				Game.VisualTunerElement = element;
			}
		}
		this.gasConduitSystem.Update();
		this.liquidConduitSystem.Update();
		this.solidConduitSystem.Update();
		this.flowBlock = new MaterialPropertyBlock();
		this.flowBlock.SetColor("_Color", this.flowColour);
		SimViewMode mode = SimDebugView.Instance.GetMode();
		if (mode != this.previousOverlayMode)
		{
			this.previousOverlayMode = mode;
			if (mode != SimViewMode.LiquidVentMap)
			{
				if (mode != SimViewMode.GasVentMap)
				{
					if (mode != SimViewMode.SolidConveyorMap)
					{
						this.liquidFlowVisualizer.ColourizePipeContents(false, false);
						this.gasFlowVisualizer.ColourizePipeContents(false, false);
						this.solidFlowVisualizer.ColourizePipeContents(false, false);
					}
					else
					{
						this.liquidFlowVisualizer.ColourizePipeContents(false, true);
						this.gasFlowVisualizer.ColourizePipeContents(false, true);
						this.solidFlowVisualizer.ColourizePipeContents(true, true);
					}
				}
				else
				{
					this.liquidFlowVisualizer.ColourizePipeContents(false, true);
					this.gasFlowVisualizer.ColourizePipeContents(true, true);
					this.solidFlowVisualizer.ColourizePipeContents(false, true);
				}
			}
			else
			{
				this.liquidFlowVisualizer.ColourizePipeContents(true, true);
				this.gasFlowVisualizer.ColourizePipeContents(false, true);
				this.solidFlowVisualizer.ColourizePipeContents(false, true);
			}
		}
		this.gasFlowVisualizer.Render(this.gasFlowPos.z, 0, this.gasConduitFlow.ContinuousLerpPercent, mode == SimViewMode.GasVentMap && this.gasConduitFlow.DiscreteLerpPercent != this.previousGasConduitFlowDiscreteLerpPercent);
		this.liquidFlowVisualizer.Render(this.liquidFlowPos.z, 0, this.liquidConduitFlow.ContinuousLerpPercent, mode == SimViewMode.LiquidVentMap && this.liquidConduitFlow.DiscreteLerpPercent != this.previousLiquidConduitFlowDiscreteLerpPercent);
		this.solidFlowVisualizer.Render(this.solidFlowPos.z, 0, this.solidConduitFlow.ContinuousLerpPercent, mode == SimViewMode.SolidConveyorMap && this.solidConduitFlow.DiscreteLerpPercent != this.previousSolidConduitFlowDiscreteLerpPercent);
		this.previousGasConduitFlowDiscreteLerpPercent = ((mode != SimViewMode.GasVentMap) ? (-1f) : this.gasConduitFlow.DiscreteLerpPercent);
		this.previousLiquidConduitFlowDiscreteLerpPercent = ((mode != SimViewMode.LiquidVentMap) ? (-1f) : this.liquidConduitFlow.DiscreteLerpPercent);
		this.previousSolidConduitFlowDiscreteLerpPercent = ((mode != SimViewMode.SolidConveyorMap) ? (-1f) : this.solidConduitFlow.DiscreteLerpPercent);
		Vector3 vector = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, Camera.main.transform.GetPosition().z));
		Vector3 vector2 = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, Camera.main.transform.GetPosition().z));
		Shader.SetGlobalVector("_WsToCs", new Vector4(vector.x / (float)Grid.WidthInCells, vector.y / (float)Grid.HeightInCells, (vector2.x - vector.x) / (float)Grid.WidthInCells, (vector2.y - vector.y) / (float)Grid.HeightInCells));
		if (this.drawStatusItems)
		{
			this.statusItemRenderer.RenderEveryTick();
			this.prioritizableRenderer.RenderEveryTick();
		}
		this.LateUpdateComponents();
		StateMachineUpdater.instance.Render(Time.unscaledDeltaTime);
		StateMachineUpdater.instance.RenderEveryTick(Time.unscaledDeltaTime);
		if (SelectTool.Instance != null && SelectTool.Instance.selected != null)
		{
			Navigator component = SelectTool.Instance.selected.GetComponent<Navigator>();
			if (component != null)
			{
				component.DrawPath();
			}
		}
	}

	public void Reset(GameSpawnData gsd)
	{
		using (new KProfiler.Region("World.Reset", null))
		{
			if (gsd != null)
			{
				foreach (KeyValuePair<Vector2I, bool> keyValuePair in gsd.preventFoWReveal)
				{
					if (keyValuePair.Value)
					{
						Grid.PreventFogOfWarReveal[Grid.PosToCell(keyValuePair.Key)] = keyValuePair.Value;
					}
				}
			}
		}
	}

	private void OnApplicationQuit()
	{
		Game.quitting = true;
		KAnimBatchManager.Destroy();
		Sim.Shutdown();
		AudioMixer.Destroy();
		if (this.screenMgr != null && this.screenMgr.gameObject != null)
		{
			global::UnityEngine.Object.Destroy(this.screenMgr.gameObject);
		}
		Console.WriteLine("Game.OnApplicationQuit()");
	}

	private void InitializeFXSpawners()
	{
		for (int i = 0; i < this.fxSpawnData.Length; i++)
		{
			int fx_idx = i;
			this.fxSpawnData[fx_idx].fxPrefab.SetActive(false);
			ushort fx_mask = (ushort)(1 << fx_idx);
			Action<SpawnFXHashes, GameObject> destroyer = delegate(SpawnFXHashes fxid, GameObject go)
			{
				if (!Game.IsQuitting())
				{
					int num = Grid.PosToCell(go);
					ushort[] array = this.activeFX;
					int num2 = num;
					array[num2] &= ~fx_mask;
					go.GetComponent<KAnimControllerBase>().enabled = false;
					this.fxPools[fxid].ReleaseInstance(go);
				}
			};
			Func<GameObject> func = delegate
			{
				GameObject gameObject = GameUtil.KInstantiate(this.fxSpawnData[fx_idx].fxPrefab, Grid.SceneLayer.Front, Folder.FX, null, 0);
				KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
				component.enabled = false;
				gameObject.SetActive(true);
				component.onDestroySelf = delegate(GameObject go)
				{
					destroyer(this.fxSpawnData[fx_idx].id, go);
				};
				return gameObject;
			};
			ObjectPool pool = new ObjectPool(func, this.fxSpawnData[fx_idx].initialCount);
			this.fxPools[this.fxSpawnData[fx_idx].id] = pool;
			this.fxSpawner[this.fxSpawnData[fx_idx].id] = delegate(Vector3 pos, float rotation)
			{
				GameScheduler.Instance.Schedule("SpawnFX", 0f, delegate(object obj)
				{
					int num3 = Grid.PosToCell(pos);
					if ((<InitializeFXSpawners>c__AnonStorey.activeFX[num3] & fx_mask) == 0)
					{
						ushort[] array2 = <InitializeFXSpawners>c__AnonStorey.activeFX;
						int num4 = num3;
						array2[num4] |= fx_mask;
						GameObject instance = pool.GetInstance();
						Game.SpawnPoolData spawnPoolData = <InitializeFXSpawners>c__AnonStorey.fxSpawnData[fx_idx];
						Quaternion quaternion = Quaternion.identity;
						bool flag = false;
						string text = spawnPoolData.initialAnim;
						Game.SpawnRotationConfig rotationConfig = spawnPoolData.rotationConfig;
						if (rotationConfig != Game.SpawnRotationConfig.Normal)
						{
							if (rotationConfig == Game.SpawnRotationConfig.StringName)
							{
								int num5 = (int)(rotation / 90f);
								if (num5 < 0)
								{
									num5 += spawnPoolData.rotationData.Length;
								}
								text = spawnPoolData.rotationData[num5].animName;
								flag = spawnPoolData.rotationData[num5].flip;
							}
						}
						else
						{
							quaternion = Quaternion.Euler(0f, 0f, rotation);
						}
						pos += spawnPoolData.spawnOffset;
						Vector2 vector = global::UnityEngine.Random.insideUnitCircle;
						vector.x *= spawnPoolData.spawnRandomOffset.x;
						vector.y *= spawnPoolData.spawnRandomOffset.y;
						vector = quaternion * vector;
						pos.x += vector.x;
						pos.y += vector.y;
						instance.transform.SetPosition(pos);
						instance.transform.rotation = quaternion;
						KBatchedAnimController component2 = instance.GetComponent<KBatchedAnimController>();
						component2.FlipX = flag;
						component2.TintColour = spawnPoolData.colour;
						component2.Play(text, KAnim.PlayMode.Once, 1f, 0f);
						component2.enabled = true;
					}
				}, null, null);
			};
		}
	}

	public void SpawnFX(SpawnFXHashes fx_id, int cell, float rotation)
	{
		Vector3 vector = Grid.CellToPosCBC(cell, Grid.SceneLayer.Front);
		this.fxSpawner[fx_id](vector, rotation);
	}

	public void SpawnFX(SpawnFXHashes fx_id, Vector3 pos, float rotation)
	{
		this.fxSpawner[fx_id](pos, rotation);
	}

	public static void SaveSettings(BinaryWriter writer)
	{
		Game.Settings settings = new Game.Settings(Game.Instance);
		Serializer.Serialize(settings, writer);
	}

	public static void LoadSettings(Deserializer deserializer)
	{
		Game.Settings settings = new Game.Settings();
		deserializer.Deserialize(settings);
		KPlayerPrefs.SetInt(Game.NextUniqueIDKey, settings.nextUniqueID);
		KleiMetrics.SetGameID(settings.gameID);
	}

	public void Save(BinaryWriter writer)
	{
		Game.GameSaveData gameSaveData = new Game.GameSaveData();
		gameSaveData.gasConduitFlow = this.gasConduitFlow;
		gameSaveData.liquidConduitFlow = this.liquidConduitFlow;
		gameSaveData.simActiveRegionMin = this.simActiveRegionMin;
		gameSaveData.simActiveRegionMax = this.simActiveRegionMax;
		gameSaveData.fallingWater = this.world.GetComponent<FallingWater>();
		gameSaveData.unstableGround = this.world.GetComponent<UnstableGroundManager>();
		gameSaveData.worldDetail = SaveLoader.Instance.worldDetailSave;
		gameSaveData.debugWasUsed = this.debugWasUsed;
		gameSaveData.customGameSettings = this.customSettings;
		gameSaveData.autoPrioritizeRoles = this.autoPrioritizeRoles;
		gameSaveData.advancedPersonalPriorities = this.advancedPersonalPriorities;
		if (this.OnSave != null)
		{
			this.OnSave(gameSaveData);
		}
		Serializer.Serialize(gameSaveData, writer);
	}

	public void Load(Deserializer deserializer)
	{
		Game.GameSaveData gameSaveData = new Game.GameSaveData();
		gameSaveData.gasConduitFlow = this.gasConduitFlow;
		gameSaveData.liquidConduitFlow = this.liquidConduitFlow;
		gameSaveData.simActiveRegionMin = new Vector2I(Grid.WidthInCells - 1, Grid.HeightInCells - 1);
		gameSaveData.simActiveRegionMax = new Vector2I(0, 0);
		gameSaveData.fallingWater = this.world.GetComponent<FallingWater>();
		gameSaveData.unstableGround = this.world.GetComponent<UnstableGroundManager>();
		gameSaveData.worldDetail = new WorldDetailSave();
		gameSaveData.customGameSettings = global::UnityEngine.Object.FindObjectOfType<CustomGameSettings>();
		gameSaveData.customGameSettings.Reset();
		deserializer.Deserialize(gameSaveData);
		this.gasConduitFlow = gameSaveData.gasConduitFlow;
		this.liquidConduitFlow = gameSaveData.liquidConduitFlow;
		this.simActiveRegionMin = gameSaveData.simActiveRegionMin;
		this.simActiveRegionMax = gameSaveData.simActiveRegionMax;
		this.debugWasUsed = gameSaveData.debugWasUsed;
		this.customSettings = gameSaveData.customGameSettings;
		this.autoPrioritizeRoles = gameSaveData.autoPrioritizeRoles;
		this.advancedPersonalPriorities = gameSaveData.advancedPersonalPriorities;
		if (this.customSettings != null)
		{
			this.customSettings.Print();
		}
		KCrashReporter.debugWasUsed = this.debugWasUsed;
		SaveLoader.Instance.SetWorldDetail(gameSaveData.worldDetail);
		if (this.OnLoad != null)
		{
			this.OnLoad(gameSaveData);
		}
	}

	public void ResetTime()
	{
		KPlayerPrefs.DeleteKey(Game.NextUniqueIDKey);
		KPrefabID.NextUniqueID = 0;
	}

	public void SetAutoSaveCallbacks(Game.SavingPreCB activatePreCB, Game.SavingActiveCB activateActiveCB, Game.SavingPostCB activatePostCB)
	{
		this.activatePreCB = activatePreCB;
		this.activateActiveCB = activateActiveCB;
		this.activatePostCB = activatePostCB;
	}

	public void StartDelayedInitialSave()
	{
		base.StartCoroutine(this.DelayedInitialSave());
	}

	private IEnumerator DelayedInitialSave()
	{
		for (int i = 0; i < 1; i++)
		{
			yield return null;
		}
		SaveLoader.Instance.InitialSave();
		yield break;
	}

	public void StartDelayedSave(string filename, bool isAutoSave = false, bool updateSavePointer = true)
	{
		if (this.activatePreCB != null)
		{
			this.activatePreCB(delegate
			{
				this.StartCoroutine(this.DelayedSave(filename, isAutoSave, updateSavePointer));
			});
		}
		else
		{
			base.StartCoroutine(this.DelayedSave(filename, isAutoSave, updateSavePointer));
		}
	}

	private IEnumerator DelayedSave(string filename, bool isAutoSave, bool updateSavePointer)
	{
		for (int i = 0; i < 1; i++)
		{
			yield return null;
		}
		if (this.activateActiveCB != null)
		{
			this.activateActiveCB();
			for (int j = 0; j < 1; j++)
			{
				yield return null;
			}
		}
		SaveLoader.Instance.Save(filename, isAutoSave, updateSavePointer);
		if (this.activatePostCB != null)
		{
			this.activatePostCB();
		}
		yield break;
	}

	public void StartDelayed(int tick_delay, global::System.Action action)
	{
		base.StartCoroutine(this.DelayedExecutor(tick_delay, action));
	}

	private IEnumerator DelayedExecutor(int tick_delay, global::System.Action action)
	{
		for (int i = 0; i < tick_delay; i++)
		{
			yield return null;
		}
		action();
		yield break;
	}

	private void LoadEventHashes()
	{
		IEnumerator enumerator = Enum.GetValues(typeof(GameHashes)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				GameHashes gameHashes = (GameHashes)obj;
				HashCache.Get().Add((int)gameHashes, gameHashes.ToString());
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
		IEnumerator enumerator2 = Enum.GetValues(typeof(UtilHashes)).GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				object obj2 = enumerator2.Current;
				UtilHashes utilHashes = (UtilHashes)obj2;
				HashCache.Get().Add((int)utilHashes, utilHashes.ToString());
			}
		}
		finally
		{
			IDisposable disposable2;
			if ((disposable2 = enumerator2 as IDisposable) != null)
			{
				disposable2.Dispose();
			}
		}
		IEnumerator enumerator3 = Enum.GetValues(typeof(UIHashes)).GetEnumerator();
		try
		{
			while (enumerator3.MoveNext())
			{
				object obj3 = enumerator3.Current;
				UIHashes uihashes = (UIHashes)obj3;
				HashCache.Get().Add((int)uihashes, uihashes.ToString());
			}
		}
		finally
		{
			IDisposable disposable3;
			if ((disposable3 = enumerator3 as IDisposable) != null)
			{
				disposable3.Dispose();
			}
		}
	}

	public void StopFE()
	{
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FrontEndSnapshot, STOP_MODE.ALLOWFADEOUT);
		if (MusicManager.instance.SongIsPlaying("Music_FrontEnd"))
		{
			MusicManager.instance.StopSong("Music_FrontEnd", true, STOP_MODE.ALLOWFADEOUT);
		}
		if (MusicManager.instance.SongIsPlaying("Music_TitleTheme"))
		{
			MusicManager.instance.StopSong("Music_TitleTheme", true, STOP_MODE.ALLOWFADEOUT);
		}
	}

	public void StartBE()
	{
		Resources.UnloadUnusedAssets();
		if (TimeOfDay.Instance != null && !MusicManager.instance.SongIsPlaying("Underscore_Night_LP") && TimeOfDay.Instance.GetCurrentTimeRegion() == TimeOfDay.TimeRegion.Night)
		{
			MusicManager.instance.PlaySong("Underscore_Night_LP", false);
		}
		AudioMixer.instance.Reset();
		AudioMixer.instance.StartPersistentSnapshots();
		if (MusicManager.instance.ShouldPlayDynamicMusicLoadedGame())
		{
			MusicManager.instance.PlayDynamicMusic();
		}
		else
		{
			MusicManager.instance.daysSinceDynamicMusic = MusicManager.instance.daysBetweenDynamicMusic;
		}
	}

	public void StopBE()
	{
		LoopingSoundManager loopingSoundManager = LoopingSoundManager.Get();
		if (loopingSoundManager != null)
		{
			loopingSoundManager.StopAllSounds();
		}
		MusicManager.instance.KillAllSongs(STOP_MODE.ALLOWFADEOUT);
		AudioMixer.instance.StopPersistentSnapshots();
		Dictionary<Tag, List<SaveLoadRoot>> lists = SaveLoader.Instance.saveManager.GetLists();
		foreach (List<SaveLoadRoot> list in lists.Values)
		{
			foreach (SaveLoadRoot saveLoadRoot in list)
			{
				if (saveLoadRoot.gameObject != null)
				{
					global::Util.KDestroyGameObject(saveLoadRoot.gameObject);
				}
			}
		}
		base.GetComponent<EntombedItemVisualizer>().Clear();
		global::UnityEngine.Object.Destroy(WorldGenSpawner.Instance);
		KBatchedAnimUpdater.Destroy();
		KComponentSpawn.instance.comps.Clear();
		KInputHandler.Remove(Global.Instance.GetInputManager().GetDefaultController(), this.cameraController);
		KInputHandler.Remove(Global.Instance.GetInputManager().GetDefaultController(), this.playerController);
		Sim.Shutdown();
		SimAndRenderScheduler.instance.Reset();
		StateMachineUpdater.instance.Reset();
		Resources.UnloadUnusedAssets();
	}

	public void SetStatusItemOffset(Transform transform, Vector3 offset)
	{
		this.statusItemRenderer.SetOffset(transform, offset);
	}

	public void AddStatusItem(Transform transform, StatusItem status_item)
	{
		this.statusItemRenderer.Add(transform, status_item);
	}

	public void RemoveStatusItem(Transform transform, StatusItem status_item)
	{
		this.statusItemRenderer.Remove(transform, status_item);
	}

	public float LastTimeWorkStarted
	{
		get
		{
			return this.lastTimeWorkStarted;
		}
	}

	public void StartedWork()
	{
		this.lastTimeWorkStarted = Time.time;
	}

	private void SpawnOxygenBubbles(Vector3 position, float angle)
	{
	}

	[ContextMenu("Print")]
	private void Print()
	{
		Console.WriteLine("This is a console writeline test");
		global::Debug.Log("This is a debug log test", null);
	}

	private static readonly string NextUniqueIDKey = "NextUniqueID";

	private PlayerController playerController;

	private CameraController cameraController;

	public Action<Game.GameSaveData> OnSave;

	public Action<Game.GameSaveData> OnLoad;

	[NonSerialized]
	public bool baseAlreadyCreated;

	[NonSerialized]
	public bool autoPrioritizeRoles;

	[NonSerialized]
	public bool advancedPersonalPriorities;

	public static bool quitting;

	public AssignmentManager assignmentManager;

	public GameObject playerPrefab;

	public GameObject screenManagerPrefab;

	public GameObject cameraControllerPrefab;

	public GameObject tempIntroScreenPrefab;

	public static int BlockSelectionLayer;

	public static int BlockSelectionLayerMask;

	public static int PickupableLayer;

	public static int PickupableLayerMask;

	public static Element VisualTunerElement;

	public RoomProber roomProber;

	public RoleManager roleManager;

	public CustomGameSettings customSettings;

	private bool sandboxModeActive;

	public FrameDelayedHandleVector<Game.CallbackInfo> callbackManager = new FrameDelayedHandleVector<Game.CallbackInfo>(256);

	public Game.ComplexCallbackHandleVector complexCallbackManager = new Game.ComplexCallbackHandleVector(256);

	[NonSerialized]
	public Player LocalPlayer;

	[NonSerialized]
	public GameObject ColliderRoot;

	[NonSerialized]
	public GameObject Collider3DRoot;

	public ElementInteractions elementInteractions;

	[SerializeField]
	private TextAsset elementInteractionsData;

	[SerializeField]
	public TextAsset maleNamesFile;

	[SerializeField]
	public TextAsset femaleNamesFile;

	[SerializeField]
	private TextAsset buildingUpgrades;

	[NonSerialized]
	public World world;

	[NonSerialized]
	public CircuitManager circuitManager;

	[NonSerialized]
	public EnergySim emergySim;

	[NonSerialized]
	public LogicCircuitManager logicCircuitManager;

	private GameScreenManager screenMgr;

	public UtilityNetworkManager<FlowUtilityNetwork, Vent> gasConduitSystem;

	public UtilityNetworkManager<FlowUtilityNetwork, Vent> liquidConduitSystem;

	public UtilityNetworkManager<ElectricalUtilityNetwork, Wire> electricalConduitSystem;

	public UtilityNetworkManager<LogicCircuitNetwork, LogicWire> logicCircuitSystem;

	public UtilityNetworkTubesManager travelTubeSystem;

	public UtilityNetworkManager<FlowUtilityNetwork, SolidConduit> solidConduitSystem;

	public ConduitFlow gasConduitFlow;

	public ConduitFlow liquidConduitFlow;

	public SolidConduitFlow solidConduitFlow;

	public Accumulators accumulators;

	public PlantElementAbsorbers plantElementAbsorbers;

	public bool showGasConduitDisease;

	public bool showLiquidConduitDisease;

	public ConduitFlowVisualizer gasFlowVisualizer;

	public ConduitFlowVisualizer liquidFlowVisualizer;

	public SolidConduitFlowVisualizer solidFlowVisualizer;

	public ConduitTemperatureManager conduitTemperatureManager;

	public ConduitDiseaseManager conduitDiseaseManager;

	private int simSubTick;

	private bool hasFirstSimTickRun;

	private float simDt;

	[SerializeField]
	public Game.ConduitVisInfo liquidConduitVisInfo;

	[SerializeField]
	public Game.ConduitVisInfo gasConduitVisInfo;

	[SerializeField]
	public Game.ConduitVisInfo solidConduitVisInfo;

	[SerializeField]
	private Material liquidFlowMaterial;

	[SerializeField]
	private Material gasFlowMaterial;

	[SerializeField]
	private Color flowColour;

	private Vector3 gasFlowPos;

	private Vector3 liquidFlowPos;

	private Vector3 solidFlowPos;

	private MaterialPropertyBlock flowBlock;

	public bool drawStatusItems = true;

	private List<SolidInfo> solidInfo = new List<SolidInfo>();

	private List<global::Klei.CallbackInfo> callbackInfo = new List<global::Klei.CallbackInfo>();

	private List<SolidInfo> gameSolidInfo = new List<SolidInfo>();

	private bool IsPaused;

	private EventInstance music;

	private HashSet<int> solidChangedFilter = new HashSet<int>();

	public SafetyConditions safetyConditions = new SafetyConditions();

	public SimData simData = new SimData();

	[MyCmpGet]
	private GameScenePartitioner gameScenePartitioner;

	private bool gameStarted;

	private ushort[] activeFX;

	private Vector2I simActiveRegionMin;

	private Vector2I simActiveRegionMax;

	public bool debugWasUsed;

	[SerializeField]
	private bool forceActiveArea;

	[SerializeField]
	private Vector2 minForcedActiveArea = new Vector2(0f, 0f);

	[SerializeField]
	private Vector2 maxForcedActiveArea = new Vector2(128f, 128f);

	private bool isLoading;

	private bool forceSimStep;

	private SimViewMode previousOverlayMode;

	private float previousGasConduitFlowDiscreteLerpPercent = -1f;

	private float previousLiquidConduitFlowDiscreteLerpPercent = -1f;

	private float previousSolidConduitFlowDiscreteLerpPercent = -1f;

	[SerializeField]
	private Game.SpawnPoolData[] fxSpawnData;

	private Dictionary<SpawnFXHashes, Action<Vector3, float>> fxSpawner = new Dictionary<SpawnFXHashes, Action<Vector3, float>>();

	private Dictionary<SpawnFXHashes, ObjectPool> fxPools = new Dictionary<SpawnFXHashes, ObjectPool>();

	private Game.SavingPreCB activatePreCB;

	private Game.SavingActiveCB activateActiveCB;

	private Game.SavingPostCB activatePostCB;

	[SerializeField]
	public Game.UIColours uiColours = new Game.UIColours();

	private float lastTimeWorkStarted = float.NegativeInfinity;

	public struct CallbackInfo
	{
		public CallbackInfo(global::System.Action cb, bool manually_release = false)
		{
			this.cb = cb;
			this.manuallyRelease = manually_release;
		}

		public global::System.Action cb;

		public bool manuallyRelease;
	}

	public struct ComplexCallbackInfo
	{
		public ComplexCallbackInfo(Action<object> cb, string debug_info)
		{
			this.cb = cb;
			this.debugInfo = debug_info;
		}

		public Action<object> cb;

		public string debugInfo;
	}

	public class ComplexCallbackHandleVector : FrameDelayedHandleVector<Game.ComplexCallbackInfo>
	{
		public ComplexCallbackHandleVector(int initial_size)
			: base(initial_size)
		{
		}

		public override void Reset(HandleVector<Game.ComplexCallbackInfo>.Handle handle)
		{
			Game.ComplexCallbackInfo complexCallbackInfo = this.items[handle.index];
			complexCallbackInfo.cb = null;
			this.items[handle.index] = complexCallbackInfo;
		}
	}

	[Serializable]
	public class ConduitVisInfo
	{
		public GameObject prefab;

		[Header("Main View")]
		public Color32 tint;

		public Color32 insulatedTint;

		public Color32 radiantTint;

		[Header("Overlay")]
		public Color32 overlayTint;

		public Color32 overlayInsulatedTint;

		public Color32 overlayRadiantTint;

		public Vector2 overlayMassScaleRange = new Vector2f(1f, 1000f);

		public Vector2 overlayMassScaleValues = new Vector2f(0.1f, 1f);
	}

	private enum SpawnRotationConfig
	{
		Normal,
		StringName
	}

	[Serializable]
	private struct SpawnRotationData
	{
		public string animName;

		public bool flip;
	}

	[Serializable]
	private struct SpawnPoolData
	{
		[HashedEnum]
		public SpawnFXHashes id;

		public int initialCount;

		public Color32 colour;

		public GameObject fxPrefab;

		public string initialAnim;

		public Vector3 spawnOffset;

		public Vector2 spawnRandomOffset;

		public Game.SpawnRotationConfig rotationConfig;

		public Game.SpawnRotationData[] rotationData;
	}

	[Serializable]
	private class Settings
	{
		public Settings(Game game)
		{
			this.baseAlreadyCreated = game.baseAlreadyCreated;
			this.nextUniqueID = KPrefabID.NextUniqueID;
			this.gameID = KleiMetrics.GameID();
		}

		public Settings()
		{
		}

		public bool baseAlreadyCreated;

		public int nextUniqueID;

		public int gameID;
	}

	public class GameSaveData
	{
		public ConduitFlow gasConduitFlow;

		public ConduitFlow liquidConduitFlow;

		public Vector2I simActiveRegionMin;

		public Vector2I simActiveRegionMax;

		public FallingWater fallingWater;

		public UnstableGroundManager unstableGround;

		public WorldDetailSave worldDetail;

		public CustomGameSettings customGameSettings;

		public bool debugWasUsed;

		public bool autoPrioritizeRoles;

		public bool advancedPersonalPriorities;
	}

	public delegate void CansaveCB();

	public delegate void SavingPreCB(Game.CansaveCB cb);

	public delegate void SavingActiveCB();

	public delegate void SavingPostCB();

	[Serializable]
	public struct LocationColours
	{
		public Color unreachable;

		public Color invalidLocation;

		public Color validLocation;

		public Color requiresRole;

		public Color unreachable_requiresRole;
	}

	[Serializable]
	public class UIColours
	{
		public Game.LocationColours Dig
		{
			get
			{
				return this.digColours;
			}
		}

		public Game.LocationColours Build
		{
			get
			{
				return this.buildColours;
			}
		}

		[SerializeField]
		private Game.LocationColours digColours;

		[SerializeField]
		private Game.LocationColours buildColours;
	}
}
