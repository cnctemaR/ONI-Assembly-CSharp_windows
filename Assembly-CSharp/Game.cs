using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FMOD.Studio;
using Klei;
using Klei.AI;
using KSerialization;
using TUNING;
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

	public StatusItemRenderer statusItemRenderer { get; private set; }

	protected override void OnPrefabInit()
	{
		SimTemperatureTransfer.ClearInstanceMap();
		StructureTemperatureComponents.ClearInstanceMap();
		App.OnPreLoadScene = (global::System.Action)Delegate.Combine(App.OnPreLoadScene, new global::System.Action(this.StopBE));
		Game.Instance = this;
		this.statusItemRenderer = new StatusItemRenderer();
		CellChangeMonitor.Destroy();
		this.LoadEventHashes();
		this.gasFlowPos = new Vector3(0f, 0f, Grid.GetLayerZ(Grid.SceneLayer.GasConduits) - 0.4f);
		this.liquidFlowPos = new Vector3(0f, 0f, Grid.GetLayerZ(Grid.SceneLayer.LiquidConduits) - 0.4f);
		Shader.WarmupAllShaders();
		Db.Get();
		Game.quitting = false;
		Game.PickupableLayer = LayerMask.NameToLayer("Pickupable");
		Game.PickupableLayerMask = LayerMask.GetMask(new string[] { "Pickupable" });
		this.ColliderRoot = global::Util.NewGameObject(base.gameObject, "Colliders");
		Game.BlockSelectionLayer = LayerMask.NameToLayer("BlockSelection");
		Game.BlockSelectionLayerMask = LayerMask.GetMask(new string[] { "BlockSelection" });
		this.world = World.Instance;
		KPrefabID.NextUniqueID = PlayerPrefs.GetInt(Game.NextUniqueIDKey, 0);
		this.roomProber = new RoomProber(Grid.CellCount);
		this.circuitManager = new CircuitManager();
		this.RegionManager = new RegionManager(Grid.CellCount, REGIONS.REGIONS_TYPES);
		this.elementInteractions = new ElementInteractions(this.elementInteractionsData);
		this.gasConduitSystem = new UtilityNetworkManager<FlowUtilityNetwork, Vent>(Grid.WidthInCells, Grid.HeightInCells, 12, 1, false);
		this.liquidConduitSystem = new UtilityNetworkManager<FlowUtilityNetwork, Vent>(Grid.WidthInCells, Grid.HeightInCells, 16, 1, false);
		this.electricalConduitSystem = new UtilityNetworkManager<ElectricalUtilityNetwork, Wire>(Grid.WidthInCells, Grid.HeightInCells, 20, 19, true);
		this.gasConduitFlow = new ConduitFlow(ConduitType.Gas, Grid.CellCount, this.gasConduitSystem, 1f);
		this.liquidConduitFlow = new ConduitFlow(ConduitType.Liquid, Grid.CellCount, this.liquidConduitSystem, 10f);
		this.gasFlowVisualizer = new ConduitFlowVisualizer(this.gasConduitFlow, this.gasConduitVisInfo, GlobalResources.Instance().ConduitOverlaySoundGas);
		this.liquidFlowVisualizer = new ConduitFlowVisualizer(this.liquidConduitFlow, this.liquidConduitVisInfo, GlobalResources.Instance().ConduitOverlaySoundLiquid);
		this.activeFX = new ushort[Grid.CellCount];
		this.simActiveRegionMax = new Vector2I(0, 0);
		this.simActiveRegionMin = new Vector2I(Grid.WidthInCells - 1, Grid.HeightInCells - 1);
		this.gameActiveRegionMax = new Vector2I(0, 0);
		this.gameActiveRegionMin = new Vector2I(Grid.WidthInCells - 1, Grid.HeightInCells - 1);
		this.UnsafePrefabInit();
		Shader.SetGlobalVector("_MetalParameters", new Vector4(0f, 0f, 0f, 0f));
		Shader.SetGlobalVector("_WaterParameters", new Vector4(0f, 0f, 0f, 0f));
		this.gasTransitionPool = new ObjectPool(new Func<GameObject>(this.InstantiateGasTransition), 256);
		this.InitializeFXSpawners();
		PathFinder.Initialize();
		new GameNavGrids(Pathfinding.Instance);
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
		this.StepTheSim();
	}

	protected override void OnForcedCleanUp()
	{
		LightGridManager.Shutdown();
		App.OnPreLoadScene = (global::System.Action)Delegate.Remove(App.OnPreLoadScene, new global::System.Action(this.StopBE));
		base.OnForcedCleanUp();
	}

	protected override void OnSpawn()
	{
		this.LocalPlayer = this.SpawnPlayer();
		WaterCubes.Instance.Init();
		UpdateManager.instance.SkipNextUpdate();
		SpeedControlScreen.Instance.Pause(false);
		LightGridManager.Initialise();
		this.UnsafeOnSpawn();
		if (this.startPaused && this.tempIntroScreenPrefab != null)
		{
			Time.timeScale = 0f;
			global::Util.KInstantiate(this.tempIntroScreenPrefab, null, null);
		}
		if (SaveLoader.Instance.cachedGSD != null)
		{
			this.Reset(SaveLoader.Instance.cachedGSD);
			NewBaseScreen.SetInitialCamera();
		}
		TagManager.FillMissingProperNames();
		CameraController.Instance.SetOrthographicsSize(20f);
		if (PlayerPrefs.HasKey(Game.BaseAlreadyCreatedKey))
		{
			this.baseAlreadyCreated = true;
			PlayerPrefs.DeleteKey(Game.BaseAlreadyCreatedKey);
			this.Trigger(-1992507039, null);
			this.Trigger(-838649377, null);
		}
		else
		{
			this.ResetTime();
		}
		LightGridManager.SetActiveWindowOnce(new Vector2I(0, 0), new Vector2I(Grid.WidthInCells, Grid.HeightInCells));
		KScreen kscreen = this.LocalPlayer.ScreenManager.StartScreen(ScreenPrefabs.Instance.ResourceCategoryScreen.gameObject, null, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
		kscreen.transform.SetSiblingIndex(1);
		foreach (MeshRenderer meshRenderer in Resources.FindObjectsOfTypeAll(typeof(MeshRenderer)))
		{
			meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
		}
		OverlayScreen.OnOverlayChanged = (Action<SimViewMode>)Delegate.Combine(OverlayScreen.OnOverlayChanged, new Action<SimViewMode>(delegate(SimViewMode mode)
		{
			this.statusItemRenderer.MarkAllDirty();
		}));
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
		this.screenMgr = global::Util.KInstantiate(this.screenManagerPrefab, null, null).GetComponent<GameScreenManager>();
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

	private void SimUpdateFirst(float dt)
	{
		Sim.DebugProperties debugProperties;
		debugProperties.buildingTemperatureScale = 100f;
		debugProperties.contaminatedOxygenEmitProbability = 0.001f;
		debugProperties.contaminatedOxygenConversionPercent = 0.001f;
		debugProperties.biomeTemperatureLerpRate = 0.001f;
		SimMessages.NewGameFrame(dt, this.simActiveRegionMin, this.simActiveRegionMax);
		SimMessages.SetDebugProperties(debugProperties);
		if (this.circuitManager != null)
		{
			this.circuitManager.Update();
		}
	}

	private void SimUpdateLast(float dt)
	{
		if (this.circuitManager != null)
		{
			this.circuitManager.SimUpdateLast(dt);
		}
	}

	private unsafe Sim.GameDataUpdate* StepTheSim()
	{
		this.simDT += Time.deltaTime;
		if (this.simElapsedTime < 0.25f)
		{
			this.simElapsedTime += Time.deltaTime;
			return null;
		}
		this.simElapsedTime -= 0.25f;
		Sim.GameDataUpdate* ptr;
		using (new KProfiler.Region("StepTheSim", null))
		{
			IntPtr intPtr = IntPtr.Zero;
			using (new KProfiler.Region("Step", null))
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
				Grid.CellValues = ptr2->cells;
				List<Element> elements = ElementLoader.elements;
				this.simData.emittedMassEntries = ptr2->emittedMassEntries;
				this.simData.elementChunks = ptr2->elementChunkInfos;
				this.simData.buildingTemperatures = ptr2->buildingTemperatures;
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
					this.callbackInfo.Add(new CallbackInfo(new HandleVector<global::System.Action>.Handle
					{
						index = callbackInfo.callbackIdx
					}));
				}
				int numSpawnFallingLiquidInfo = ptr2->numSpawnFallingLiquidInfo;
				for (int l = 0; l < numSpawnFallingLiquidInfo; l++)
				{
					Sim.SpawnFallingLiquidInfo spawnFallingLiquidInfo = ptr2->spawnFallingLiquidInfo[l];
					FallingWater.instance.AddParticle(spawnFallingLiquidInfo.cellIdx, spawnFallingLiquidInfo.elemIdx, spawnFallingLiquidInfo.mass, spawnFallingLiquidInfo.temperature, false, false, false);
				}
				int numSpawnOreInfo = ptr2->numSpawnOreInfo;
				for (int m = 0; m < numSpawnOreInfo; m++)
				{
					Sim.SpawnOreInfo spawnOreInfo = ptr2->spawnOreInfo[m];
					Vector3 vector = Grid.CellToPosCCC(spawnOreInfo.cellIdx, Grid.SceneLayer.Use);
					Element element2 = ElementLoader.elements[(int)spawnOreInfo.elemIdx];
					if (spawnOreInfo.temperature <= 0f && spawnOreInfo.mass > 0f)
					{
						Output.LogError(new object[] { "Sim is telling us to spawn a zero temperature object. This shouldn't be possible because I have asserts in the dll about this...." });
					}
					element2.substance.SpawnResource(vector, spawnOreInfo.mass, spawnOreInfo.temperature, false, false);
				}
				int numSpawnFXInfo = ptr2->numSpawnFXInfo;
				for (int n = 0; n < numSpawnFXInfo; n++)
				{
					Sim.SpawnFXInfo spawnFXInfo = ptr2->spawnFXInfo[n];
					this.SpawnFX((SpawnFXHashes)spawnFXInfo.fxHash, spawnFXInfo.cellIdx, spawnFXInfo.rotation);
				}
				UnstableGroundManager component = this.world.GetComponent<UnstableGroundManager>();
				int numUnstableCellInfo = ptr2->numUnstableCellInfo;
				for (int num = 0; num < numUnstableCellInfo; num++)
				{
					Sim.UnstableCellInfo unstableCellInfo = ptr2->unstableCellInfo[num];
					Sim.UnstableCellInfo.FallingInfo fallingInfo = (Sim.UnstableCellInfo.FallingInfo)unstableCellInfo.fallingInfo;
					Sim.UnstableCellInfo.FallingInfo fallingInfo2 = fallingInfo;
					if (fallingInfo2 == Sim.UnstableCellInfo.FallingInfo.StartedFalling)
					{
						component.Spawn(unstableCellInfo.cellIdx, ElementLoader.elements[(int)unstableCellInfo.elemIdx], unstableCellInfo.mass, unstableCellInfo.temperature);
					}
				}
				int numWorldDamageInfo = ptr2->numWorldDamageInfo;
				for (int num2 = 0; num2 < numWorldDamageInfo; num2++)
				{
					Sim.WorldDamageInfo worldDamageInfo = ptr2->worldDamageInfo[num2];
					WorldDamage.Instance.ApplyDamage(worldDamageInfo);
				}
				for (int num3 = 0; num3 < ptr2->numConsumedMassEntries; num3++)
				{
					Sim.ConsumedMassInfo consumedMassInfo = ptr2->consumedMassEntries[num3];
					ElementConsumer.AddMass(consumedMassInfo);
				}
				int numMassConsumptionCallbacks = ptr2->numMassConsumptionCallbacks;
				HandleVector<Action<object>>.Handle handle = default(HandleVector<Action<object>>.Handle);
				for (int num4 = 0; num4 < numMassConsumptionCallbacks; num4++)
				{
					Sim.MassConsumptionCallback massConsumptionCallback = ptr2->massConsumptionCallbacks[num4];
					handle.index = massConsumptionCallback.callbackIdx;
					Action<object> action = this.complexCallbackManager.Release(handle);
					action(massConsumptionCallback);
				}
				int numComponentStateChangedMessages = ptr2->numComponentStateChangedMessages;
				HandleVector<Action<object>>.Handle handle2 = default(HandleVector<Action<object>>.Handle);
				for (int num5 = 0; num5 < numComponentStateChangedMessages; num5++)
				{
					Sim.ComponentStateChangedMessage componentStateChangedMessage = ptr2->componentStateChangedMessages[num5];
					handle2.index = componentStateChangedMessage.callbackIdx;
					Action<object> action2 = this.complexCallbackManager.Release(handle2);
					if (action2 != null)
					{
						action2(componentStateChangedMessage.simHandle);
					}
					else
					{
						Output.LogError(new object[] { "Null callback with handle", componentStateChangedMessage.callbackIdx });
					}
				}
				int numElementChunkMeltedInfos = ptr2->numElementChunkMeltedInfos;
				for (int num6 = 0; num6 < numElementChunkMeltedInfos; num6++)
				{
					Sim.MeltedInfo meltedInfo = ptr2->elementChunkMeltedInfos[num6];
					SimTemperatureTransfer.DoStateTransition(meltedInfo.handle);
				}
				int numBuildingMeltedInfos = ptr2->numBuildingMeltedInfos;
				for (int num7 = 0; num7 < numBuildingMeltedInfos; num7++)
				{
					Sim.MeltedInfo meltedInfo2 = ptr2->buildingMeltedInfos[num7];
					StructureTemperatureComponents.DoStateTransition(meltedInfo2.handle);
				}
				this.gasConduitFlow.Update(this.simDT);
				this.liquidConduitFlow.Update(this.simDT);
				UpdateManager.instance.Step(this.simDT);
				this.simDT = 0f;
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
		this.gameActiveRegionMin.x = Mathf.Max(0, Mathf.Min(x0, this.gameActiveRegionMin.x));
		this.gameActiveRegionMin.y = Mathf.Max(0, Mathf.Min(y0, this.gameActiveRegionMin.y));
		this.gameActiveRegionMax.x = Mathf.Min(Grid.WidthInCells - 1, Mathf.Max(x1, this.gameActiveRegionMax.x));
		this.gameActiveRegionMax.y = Mathf.Min(Grid.HeightInCells - 1, Mathf.Max(y1, this.gameActiveRegionMax.y));
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
		string text = string.Concat(new object[] { "(", num, ", ", num2, ")" });
		DebugText.Instance.Draw(text, Grid.CellToPosCCC(mouseCell, Grid.SceneLayer.Move), Color.white);
	}

	public void ForceSimStep()
	{
		Output.Log(new object[] { "Force-stepping the sim" });
		this.forceSimStep = true;
		this.simElapsedTime = 0.25f;
		this.simDT = 0.25f;
	}

	private void Update()
	{
		if (this.isLoading)
		{
			return;
		}
		using (new KProfiler.Region("Game.Update", null))
		{
			if (Debug.developerConsoleVisible)
			{
				Debug.developerConsoleVisible = false;
			}
			if (Time.frameCount == 5)
			{
				Output.LogWarning(new object[] { "Load time: " + Time.realtimeSinceStartup });
			}
			if (DebugHandler.DebugCellInfo)
			{
				this.ShowDebugCellInfo();
			}
			this.gasConduitSystem.Update();
			this.liquidConduitSystem.Update();
			this.circuitManager.Update();
			if (this.forceActiveArea)
			{
				this.simActiveRegionMin = new Vector2I((int)Mathf.Max(0f, this.minForcedActiveArea.x), (int)Mathf.Max(0f, this.minForcedActiveArea.y));
				this.simActiveRegionMax = new Vector2I((int)Mathf.Min((float)(Grid.WidthInCells - 1), this.maxForcedActiveArea.x), (int)Mathf.Min((float)(Grid.HeightInCells - 1), this.maxForcedActiveArea.y));
			}
			this.simActiveRegionMin = new Vector2I(0, 0);
			this.simActiveRegionMax = new Vector2I(Grid.WidthInCells, Grid.HeightInCells);
			LightGridManager.SetActiveWindow(this.simActiveRegionMin, this.simActiveRegionMax);
			Pathfinding.Instance.DebugUpdate();
			if (this.forceSimStep || Mathf.CeilToInt(Time.timeScale) != 0)
			{
				this.UpdateModifiers();
				CellChangeMonitor.Instance.Update();
				this.UnsafeUpdate();
				this.forceSimStep = false;
			}
		}
	}

	private unsafe void UnsafeUpdate()
	{
		Sim.GameDataUpdate* ptr = this.StepTheSim();
		if (ptr == null)
		{
			return;
		}
		this.UpdateSpawners(true);
		this.gameSolidInfo.AddRange(this.solidInfo);
		this.world.UpdateCellInfo(this.gameSolidInfo, this.callbackInfo, ptr->numSolidSubstanceChangeInfo, ptr->solidSubstanceChangeInfo, ptr->numLiquidChangeInfo, ptr->liquidChangeInfo);
		this.gameSolidInfo.Clear();
		this.solidInfo.Clear();
		this.callbackInfo.Clear();
		Pathfinding.Instance.UpdateNavGrids();
	}

	private void UpdateModifiers()
	{
		foreach (Modifiers modifiers in Components.Modifiers)
		{
			modifiers.DoUpdate();
		}
	}

	private void LateUpdate()
	{
		if (!this.hasUpdatedNetworks)
		{
			this.hasUpdatedNetworks = true;
			this.gasConduitSystem.ForceRebuildNetworks();
			this.gasConduitSystem.Update();
			this.liquidConduitSystem.ForceRebuildNetworks();
			this.liquidConduitSystem.Update();
		}
		if (Time.timeScale == 0f && !this.IsPaused)
		{
			this.IsPaused = true;
			this.Trigger(-1788536802, this.IsPaused);
		}
		else if (Time.timeScale != 0f && this.IsPaused)
		{
			this.IsPaused = false;
			this.Trigger(-1788536802, this.IsPaused);
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
		this.flowBlock = new MaterialPropertyBlock();
		this.flowBlock.SetColor("_Color", this.flowColour);
		SimViewMode mode = SimDebugView.Instance.GetMode();
		if (mode != this.previousOverlayMode)
		{
			this.previousOverlayMode = mode;
			this.liquidFlowVisualizer.ColourizePipeContents(mode == SimViewMode.LiquidVentMap);
			this.gasFlowVisualizer.ColourizePipeContents(mode == SimViewMode.GasVentMap);
		}
		this.gasFlowVisualizer.Render(this.gasFlowPos.z, 0, this.gasConduitFlow.ContinuousLerpPercent, mode == SimViewMode.GasVentMap && this.gasConduitFlow.DiscreteLerpPercent != this.previousGasConduitFlowDiscreteLerpPercent);
		this.liquidFlowVisualizer.Render(this.liquidFlowPos.z, 0, this.liquidConduitFlow.ContinuousLerpPercent, mode == SimViewMode.LiquidVentMap && this.liquidConduitFlow.DiscreteLerpPercent != this.previousLiquidConduitFlowDiscreteLerpPercent);
		this.previousGasConduitFlowDiscreteLerpPercent = ((mode != SimViewMode.GasVentMap) ? (-1f) : this.gasConduitFlow.DiscreteLerpPercent);
		this.previousLiquidConduitFlowDiscreteLerpPercent = ((mode != SimViewMode.LiquidVentMap) ? (-1f) : this.liquidConduitFlow.DiscreteLerpPercent);
		Vector3 vector = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, Camera.main.transform.position.z));
		Vector3 vector2 = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, Camera.main.transform.position.z));
		Shader.SetGlobalVector("_WsToCs", new Vector4(vector.x / (float)Grid.WidthInCells, vector.y / (float)Grid.HeightInCells, (vector2.x - vector.x) / (float)Grid.WidthInCells, (vector2.y - vector.y) / (float)Grid.HeightInCells));
		if (this.drawStatusItems)
		{
			this.statusItemRenderer.Render();
		}
	}

	public void Reset(WorldGen.GameSpawnData gsd)
	{
		using (new KProfiler.Region("World.Reset", null))
		{
			if (gsd != null)
			{
				if (gsd.clouds != null)
				{
					this.worldGapManager.SetGasClouds(gsd.clouds);
				}
				List<KeyValuePair<int, Tag>> drops = gsd.GetDrops();
				if (drops != null)
				{
					for (int i = 0; i < drops.Count; i++)
					{
						Debug.Assert(Grid.IsValidCell(drops[i].Key));
						GameObject gameObject = GameUtil.KInstantiate(EntityPrefabs.Instance.Spawner, Grid.SceneLayer.Move, Folder.Misc, null, 0);
						gameObject.name = "Spawner:" + drops[i].Value.Name;
						gameObject.transform.localPosition = Grid.CellToPosCCC(drops[i].Key, Grid.SceneLayer.Move);
						Spawner component = gameObject.GetComponent<Spawner>();
						component.SetPrefabTag(drops[i].Value);
						Mob mob;
						if (WorldGen.Settings.mobs.MobLookupTable.TryGetValue(drops[i].Value.Name, out mob))
						{
							int num = Mathf.Max(1, Mathf.RoundToInt(mob.units.GetValue()));
							if (num > 1)
							{
								component.SetUnits(num);
							}
							if (mob.prefabName != null)
							{
								component.SetPrefabTag(new Tag(mob.prefabName));
							}
						}
					}
				}
			}
		}
	}

	public void UpdateSpawners(bool delaySpawnsUntilRegionActive = true)
	{
		for (int i = Components.Spawners.Count - 1; i >= 0; i--)
		{
			if (delaySpawnsUntilRegionActive)
			{
				Vector2 vector = Components.Spawners[i].gameObject.transform.position;
				if (vector.x > (float)this.gameActiveRegionMin.x && vector.y > (float)this.gameActiveRegionMin.y && vector.x < (float)this.gameActiveRegionMax.x && vector.y < (float)this.gameActiveRegionMax.y)
				{
					Components.Spawners[i].DoSpawn();
				}
			}
			else
			{
				Components.Spawners[i].DoSpawn();
			}
		}
	}

	private void OnLevelWasLoaded()
	{
		Output.Log(new object[]
		{
			Time.realtimeSinceStartup,
			"Level Loaded....",
			SceneManager.GetActiveScene().name
		});
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

	private GameObject InstantiateGasTransition()
	{
		GameObject gameObject = GameUtil.KInstantiate(this.gasTransitionPrefab, Grid.SceneLayer.BuildingBack, Folder.FX, null, 0);
		gameObject.SetActive(false);
		KBatchedAnimController kanim = gameObject.GetComponent<KBatchedAnimController>();
		kanim.enabled = false;
		kanim.onDestroySelf = delegate(GameObject go)
		{
			if (!Game.IsQuitting())
			{
				kanim.enabled = false;
				this.gasTransitionPool.ReleaseInstance(go);
				int num = Grid.PosToCell(go.transform.position);
				this.activeGasTransitions.Remove(num);
			}
		};
		return gameObject;
	}

	private void SpawnGasTransition(Vector3 position, Element element)
	{
		GameObject instance = this.gasTransitionPool.GetInstance();
		position.z = Grid.GetLayerZ(Grid.SceneLayer.BuildingBack);
		instance.transform.SetPosition(position);
		KBatchedAnimController component = instance.GetComponent<KBatchedAnimController>();
		Color32 colour = element.substance.colour;
		colour.a = (byte)(255f * this.gasTransitionAlpha);
		component.TintColour = colour;
		component.Play("gas_Puff", KAnim.PlayMode.Once, 1f, 0f);
		component.PlaySpeedMultiplier = 0.6666667f;
		component.enabled = true;
	}

	private void InitializeFXSpawners()
	{
		this.gasTransitionPrefab.SetActive(false);
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
					if ((this.activeFX[num3] & fx_mask) == 0)
					{
						ushort[] array2 = this.activeFX;
						int num4 = num3;
						array2[num4] |= fx_mask;
						GameObject instance = pool.GetInstance();
						Game.SpawnPoolData spawnPoolData = this.fxSpawnData[fx_idx];
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
						component2.enabled = true;
						component2.TintColour = spawnPoolData.colour;
						component2.Play(text, KAnim.PlayMode.Once, 1f, 0f);
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
		PlayerPrefs.SetInt(Game.BaseAlreadyCreatedKey, (!settings.baseAlreadyCreated) ? 0 : 1);
		PlayerPrefs.SetInt(Game.NextUniqueIDKey, settings.nextUniqueID);
		KleiMetrics.SetGameID(settings.gameID);
	}

	public void Save(BinaryWriter writer)
	{
		Game.GameSaveData gameSaveData = new Game.GameSaveData();
		gameSaveData.worldGaps = this.worldGapManager.gaps;
		gameSaveData.gasConduitFlow = this.gasConduitFlow;
		gameSaveData.liquidConduitFlow = this.liquidConduitFlow;
		gameSaveData.simActiveRegionMin = this.simActiveRegionMin;
		gameSaveData.simActiveRegionMax = this.simActiveRegionMax;
		gameSaveData.gameActiveRegionMin = this.gameActiveRegionMin;
		gameSaveData.gameActiveRegionMax = this.gameActiveRegionMax;
		gameSaveData.fallingWater = this.world.GetComponent<FallingWater>();
		gameSaveData.unstableGround = this.world.GetComponent<UnstableGroundManager>();
		gameSaveData.worldDetail = SaveLoader.Instance.worldDetailSave;
		gameSaveData.debugWasUsed = this.debugWasUsed;
		Debug.Assert(gameSaveData.worldDetail != null, "World detail null");
		byte[] array = new byte[Grid.CellCount];
		for (int i = 0; i < Grid.CellCount; i++)
		{
			array[i] = ((!Grid.SuitRequired[i]) ? 0 : 1);
		}
		gameSaveData.suitRequired = array;
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
		gameSaveData.worldGaps = this.worldGapManager.gaps;
		gameSaveData.simActiveRegionMin = new Vector2I(Grid.WidthInCells - 1, Grid.HeightInCells - 1);
		gameSaveData.simActiveRegionMax = new Vector2I(0, 0);
		gameSaveData.gameActiveRegionMin = new Vector2I(Grid.WidthInCells - 1, Grid.HeightInCells - 1);
		gameSaveData.gameActiveRegionMax = new Vector2I(0, 0);
		gameSaveData.fallingWater = this.world.GetComponent<FallingWater>();
		gameSaveData.unstableGround = this.world.GetComponent<UnstableGroundManager>();
		gameSaveData.worldDetail = new WorldDetailSave();
		deserializer.Deserialize(gameSaveData);
		this.gasConduitFlow = gameSaveData.gasConduitFlow;
		this.liquidConduitFlow = gameSaveData.liquidConduitFlow;
		this.simActiveRegionMin = gameSaveData.simActiveRegionMin;
		this.simActiveRegionMax = gameSaveData.simActiveRegionMax;
		this.gameActiveRegionMin = gameSaveData.gameActiveRegionMin;
		this.gameActiveRegionMax = gameSaveData.gameActiveRegionMax;
		this.debugWasUsed = gameSaveData.debugWasUsed;
		KCrashReporter.debugWasUsed = this.debugWasUsed;
		for (int i = 0; i < gameSaveData.suitRequired.Length; i++)
		{
			Grid.SuitRequired[i] = gameSaveData.suitRequired[i] != 0;
		}
		SaveLoader.Instance.SetWorldDetail(gameSaveData.worldDetail);
		this.worldGapManager.OnDeserialized();
		if (this.OnLoad != null)
		{
			this.OnLoad(gameSaveData);
		}
	}

	public void ResetTime()
	{
		PlayerPrefs.DeleteKey(Game.NextUniqueIDKey);
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
		foreach (object obj in Enum.GetValues(typeof(GameHashes)))
		{
			GameHashes gameHashes = (GameHashes)((int)obj);
			HashCache.Get().Add((int)gameHashes, gameHashes.ToString());
		}
		foreach (object obj2 in Enum.GetValues(typeof(UtilHashes)))
		{
			UtilHashes utilHashes = (UtilHashes)((int)obj2);
			HashCache.Get().Add((int)utilHashes, utilHashes.ToString());
		}
		foreach (object obj3 in Enum.GetValues(typeof(UIHashes)))
		{
			UIHashes uihashes = (UIHashes)((int)obj3);
			HashCache.Get().Add((int)uihashes, uihashes.ToString());
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
		SystemScheduler.instance.Clear();
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
		KBatchedAnimUpdater.instance.DestroyGrid();
		KComponentSpawn.instance.comps.Clear();
		KInputHandler.Remove(Global.Instance.GetInputManager().GetDefaultController(), this.cameraController);
		KInputHandler.Remove(Global.Instance.GetInputManager().GetDefaultController(), this.playerController);
		SystemScheduler.instance.Clear();
		Sim.Shutdown();
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
		Debug.Log("This is a debug log test");
	}

	private bool startPaused = true;

	private static readonly string BaseAlreadyCreatedKey = "BaseAlreadyCreated";

	private static readonly string NextUniqueIDKey = "NextUniqueID";

	public Action<Game.GameSaveData> OnSave;

	public Action<Game.GameSaveData> OnLoad;

	[NonSerialized]
	public bool baseAlreadyCreated;

	public static bool quitting;

	public GameObject playerPrefab;

	public GameObject screenManagerPrefab;

	public GameObject cameraControllerPrefab;

	[MyCmpReq]
	private WorldGapManager worldGapManager;

	public GameObject tempIntroScreenPrefab;

	public static int BlockSelectionLayer;

	public static int BlockSelectionLayerMask;

	public static int PickupableLayer;

	public static int PickupableLayerMask;

	public static Element VisualTunerElement;

	public RoomProber roomProber;

	public CheckedHandleVector<global::System.Action> callbackManager = new CheckedHandleVector<global::System.Action>(256);

	public CheckedHandleVector<Action<object>> complexCallbackManager = new CheckedHandleVector<Action<object>>(256);

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
	public List<WarmingPoint> warmingPoints = new List<WarmingPoint>();

	[NonSerialized]
	public World world;

	[NonSerialized]
	public CircuitManager circuitManager;

	[NonSerialized]
	public RegionManager RegionManager;

	private GameScreenManager screenMgr;

	public UtilityNetworkManager<FlowUtilityNetwork, Vent> gasConduitSystem;

	public UtilityNetworkManager<FlowUtilityNetwork, Vent> liquidConduitSystem;

	public UtilityNetworkManager<ElectricalUtilityNetwork, Wire> electricalConduitSystem;

	public ConduitFlow gasConduitFlow;

	public ConduitFlow liquidConduitFlow;

	public ConduitFlowVisualizer gasFlowVisualizer;

	public ConduitFlowVisualizer liquidFlowVisualizer;

	[SerializeField]
	public Game.ConduitVisInfo liquidConduitVisInfo;

	[SerializeField]
	public Game.ConduitVisInfo gasConduitVisInfo;

	[SerializeField]
	private Material liquidFlowMaterial;

	[SerializeField]
	private Material gasFlowMaterial;

	[SerializeField]
	private Color flowColour;

	private Vector3 gasFlowPos;

	private Vector3 liquidFlowPos;

	private MaterialPropertyBlock flowBlock;

	public bool drawStatusItems = true;

	private List<SolidInfo> solidInfo = new List<SolidInfo>();

	private List<CallbackInfo> callbackInfo = new List<CallbackInfo>();

	private List<SolidInfo> gameSolidInfo = new List<SolidInfo>();

	private bool IsPaused;

	private EventInstance music;

	private HashSet<int> solidChangedFilter = new HashSet<int>();

	private ObjectPool gasTransitionPool;

	[SerializeField]
	private GameObject gasTransitionPrefab;

	[SerializeField]
	private float gasTransitionAlpha = 0.5f;

	[SerializeField]
	private List<Region> regionPrefabs;

	public SafetyConditions safetyConditions = new SafetyConditions();

	public SimData simData = new SimData();

	private bool gameStarted;

	private PlayerController playerController;

	private CameraController cameraController;

	private float simElapsedTime = 0.25f;

	private float simDT;

	private HashSet<int> activeGasTransitions = new HashSet<int>();

	private ushort[] activeFX;

	private Vector2I simActiveRegionMin;

	private Vector2I simActiveRegionMax;

	private Vector2I gameActiveRegionMin;

	private Vector2I gameActiveRegionMax;

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

	private bool hasUpdatedNetworks;

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

	[Serializable]
	public class ConduitVisInfo
	{
		public GameObject prefab;

		[Header("Main View")]
		public Color32 tint;

		public Color32 insulatedTint;

		[Header("Overlay")]
		public Color32 overlayTint;

		public Color32 overlayInsulatedTint;
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
		public WorldGaps worldGaps;

		public ConduitFlow gasConduitFlow;

		public ConduitFlow liquidConduitFlow;

		public byte[] suitRequired;

		public Vector2I simActiveRegionMin;

		public Vector2I simActiveRegionMax;

		public Vector2I gameActiveRegionMin;

		public Vector2I gameActiveRegionMax;

		public FallingWater fallingWater;

		public UnstableGroundManager unstableGround;

		public WorldDetailSave worldDetail;

		public bool debugWasUsed;
	}

	[Serializable]
	public struct LocationColours
	{
		public Color unreachable;

		public Color invalidLocation;

		public Color validLocation;
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

	public delegate void CansaveCB();

	public delegate void SavingPreCB(Game.CansaveCB cb);

	public delegate void SavingActiveCB();

	public delegate void SavingPostCB();
}
