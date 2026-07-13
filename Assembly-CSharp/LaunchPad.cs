using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using UnityEngine;

public class LaunchPad : KMonoBehaviour, ISim1000ms, IListableOption, IProcessConditionSet
{
	public RocketModuleCluster LandedRocket
	{
		get
		{
			GameObject gameObject = null;
			Grid.ObjectLayers[1].TryGetValue(this.RocketBottomPosition, out gameObject);
			if (gameObject != null)
			{
				RocketModuleCluster component = gameObject.GetComponent<RocketModuleCluster>();
				Clustercraft clustercraft = ((component != null && component.CraftInterface != null) ? component.CraftInterface.GetComponent<Clustercraft>() : null);
				if (clustercraft != null && (clustercraft.Status == Clustercraft.CraftStatus.Grounded || clustercraft.Status == Clustercraft.CraftStatus.Landing))
				{
					return component;
				}
			}
			return null;
		}
	}

	public int RocketBottomPosition
	{
		get
		{
			return Grid.OffsetCell(Grid.PosToCell(this), this.baseModulePosition);
		}
	}

	[OnDeserialized]
	private void OnDeserialzed()
	{
		if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 24))
		{
			Building component = base.GetComponent<Building>();
			if (component != null)
			{
				component.RunOnArea(delegate(int cell)
				{
					if (Grid.IsValidCell(cell) && Grid.Solid[cell])
					{
						SimMessages.ReplaceElement(cell, SimHashes.Vacuum, CellEventLogger.Instance.LaunchpadDesolidify, 0f, -1f, byte.MaxValue, 0, -1);
					}
				});
			}
		}
	}

	protected override void OnPrefabInit()
	{
		UserNameable component = base.GetComponent<UserNameable>();
		if (component != null)
		{
			component.SetName(GameUtil.GenerateRandomLaunchPadName());
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.tower = new LaunchPad.LaunchPadTower(this, this.maxTowerHeight);
		this.OnRocketBuildingChanged(this.GetRocketBaseModule());
		this.partitionerEntry = GameScenePartitioner.Instance.Add("LaunchPad.OnSpawn", base.gameObject, Extents.OneCell(this.RocketBottomPosition), GameScenePartitioner.Instance.objectLayers[1], new Action<object>(this.OnRocketBuildingChanged));
		Components.LaunchPads.Add(this);
		this.CheckLandedRocketPassengerModuleStatus();
		int num = ConditionFlightPathIsClear.PadTopEdgeDistanceToCeilingEdge(base.gameObject);
		if (num < 35)
		{
			this.selectable.AddStatusItem(Db.Get().BuildingStatusItems.RocketPlatformCloseToCeiling, num);
		}
	}

	protected override void OnCleanUp()
	{
		Components.LaunchPads.Remove(this);
		GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
		if (this.lastBaseAttachable != null)
		{
			AttachableBuilding attachableBuilding = this.lastBaseAttachable;
			attachableBuilding.onAttachmentNetworkChanged = (Action<object>)Delegate.Remove(attachableBuilding.onAttachmentNetworkChanged, new Action<object>(this.OnRocketLayoutChanged));
			this.lastBaseAttachable = null;
		}
		this.RebuildLaunchTowerHeightHandler.ClearScheduler();
		base.OnCleanUp();
	}

	private void CheckLandedRocketPassengerModuleStatus()
	{
		if (this.LandedRocket == null)
		{
			this.selectable.RemoveStatusItem(this.landedRocketPassengerModuleStatusItem, false);
			this.landedRocketPassengerModuleStatusItem = Guid.Empty;
			return;
		}
		if (this.LandedRocket.CraftInterface.GetPassengerModule() == null)
		{
			if (this.landedRocketPassengerModuleStatusItem == Guid.Empty)
			{
				this.landedRocketPassengerModuleStatusItem = this.selectable.AddStatusItem(Db.Get().BuildingStatusItems.LandedRocketLacksPassengerModule, null);
				return;
			}
		}
		else if (this.landedRocketPassengerModuleStatusItem != Guid.Empty)
		{
			this.selectable.RemoveStatusItem(this.landedRocketPassengerModuleStatusItem, false);
			this.landedRocketPassengerModuleStatusItem = Guid.Empty;
		}
	}

	public bool IsLogicInputConnected()
	{
		int portCell = this.ports.GetPortCell(this.triggerPort);
		return Game.Instance.logicCircuitManager.GetNetworkForCell(portCell) != null;
	}

	public void Sim1000ms(float dt)
	{
		RocketModuleCluster landedRocket = this.LandedRocket;
		if (landedRocket != null && this.IsLogicInputConnected())
		{
			if (this.ports.GetInputValue(this.triggerPort) == 1)
			{
				if (landedRocket.CraftInterface.CheckReadyForAutomatedLaunchCommand())
				{
					landedRocket.CraftInterface.TriggerLaunch(true);
				}
				else
				{
					landedRocket.CraftInterface.CancelLaunch();
				}
			}
			else
			{
				landedRocket.CraftInterface.CancelLaunch();
			}
		}
		this.CheckLandedRocketPassengerModuleStatus();
		this.ports.SendSignal(this.landedRocketPort, (landedRocket != null) ? 1 : 0);
		if (landedRocket != null)
		{
			this.ports.SendSignal(this.statusPort, (landedRocket.CraftInterface.CheckReadyForAutomatedLaunch() || landedRocket.CraftInterface.HasTag(GameTags.RocketNotOnGround)) ? 1 : 0);
			return;
		}
		this.ports.SendSignal(this.statusPort, 0);
	}

	public GameObject AddBaseModule(BuildingDef moduleDefID, IList<Tag> elements)
	{
		int num = Grid.OffsetCell(Grid.PosToCell(base.gameObject), this.baseModulePosition);
		GameObject gameObject;
		if (DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive)
		{
			gameObject = moduleDefID.Build(num, Orientation.Neutral, null, elements, 293.15f, true, GameClock.Instance.GetTime());
		}
		else
		{
			gameObject = moduleDefID.TryPlace(null, Grid.CellToPosCBC(num, moduleDefID.SceneLayer), Orientation.Neutral, elements, 0);
		}
		GameObject gameObject2 = Util.KInstantiate(Assets.GetPrefab("Clustercraft"), null, null);
		gameObject2.SetActive(true);
		Clustercraft component = gameObject2.GetComponent<Clustercraft>();
		component.GetComponent<CraftModuleInterface>().AddModule(gameObject.GetComponent<RocketModuleCluster>());
		component.Init(this.GetMyWorldLocation(), this);
		if (gameObject.GetComponent<BuildingUnderConstruction>() != null)
		{
			this.OnRocketBuildingChanged(gameObject);
		}
		base.Trigger(374403796, null);
		return gameObject;
	}

	private void OnRocketBuildingChanged(object data)
	{
		GameObject gameObject = (GameObject)data;
		RocketModuleCluster landedRocket = this.LandedRocket;
		global::Debug.Assert(gameObject == null || landedRocket == null || landedRocket.gameObject == gameObject, "Launch Pad had a rocket land or take off on it twice??");
		Clustercraft clustercraft = ((landedRocket != null && landedRocket.CraftInterface != null) ? landedRocket.CraftInterface.GetComponent<Clustercraft>() : null);
		if (clustercraft != null)
		{
			if (clustercraft.Status == Clustercraft.CraftStatus.Landing)
			{
				base.Trigger(-887025858, landedRocket);
			}
			else if (clustercraft.Status == Clustercraft.CraftStatus.Launching)
			{
				base.Trigger(-1277991738, landedRocket);
				AttachableBuilding component = landedRocket.CraftInterface.ClusterModules[0].Get().GetComponent<AttachableBuilding>();
				component.onAttachmentNetworkChanged = (Action<object>)Delegate.Remove(component.onAttachmentNetworkChanged, new Action<object>(this.OnRocketLayoutChanged));
			}
		}
		this.OnRocketLayoutChanged(null);
	}

	private void OnRocketLayoutChanged(object data)
	{
		if (this.lastBaseAttachable != null)
		{
			AttachableBuilding attachableBuilding = this.lastBaseAttachable;
			attachableBuilding.onAttachmentNetworkChanged = (Action<object>)Delegate.Remove(attachableBuilding.onAttachmentNetworkChanged, new Action<object>(this.OnRocketLayoutChanged));
			this.lastBaseAttachable = null;
		}
		GameObject rocketBaseModule = this.GetRocketBaseModule();
		if (rocketBaseModule != null)
		{
			this.lastBaseAttachable = rocketBaseModule.GetComponent<AttachableBuilding>();
			AttachableBuilding attachableBuilding2 = this.lastBaseAttachable;
			attachableBuilding2.onAttachmentNetworkChanged = (Action<object>)Delegate.Combine(attachableBuilding2.onAttachmentNetworkChanged, new Action<object>(this.OnRocketLayoutChanged));
		}
		this.DirtyTowerHeight();
	}

	public bool HasRocket()
	{
		return this.LandedRocket != null;
	}

	public bool HasRocketWithCommandModule()
	{
		return this.HasRocket() && this.LandedRocket.CraftInterface.FindLaunchableRocket() != null;
	}

	private GameObject GetRocketBaseModule()
	{
		GameObject gameObject = Grid.Objects[Grid.OffsetCell(Grid.PosToCell(base.gameObject), this.baseModulePosition), 1];
		if (!(gameObject != null) || !(gameObject.GetComponent<RocketModule>() != null))
		{
			return null;
		}
		return gameObject;
	}

	public void DirtyTowerHeight()
	{
		if (!this.dirtyTowerHeight)
		{
			this.dirtyTowerHeight = true;
			if (!this.RebuildLaunchTowerHeightHandler.IsValid)
			{
				this.RebuildLaunchTowerHeightHandler = GameScheduler.Instance.ScheduleNextFrame("RebuildLaunchTowerHeight", new Action<object>(this.RebuildLaunchTowerHeight), null, null);
			}
		}
	}

	private void RebuildLaunchTowerHeight(object obj)
	{
		RocketModuleCluster landedRocket = this.LandedRocket;
		if (landedRocket != null)
		{
			this.tower.SetTowerHeight(landedRocket.CraftInterface.MaxHeight);
		}
		this.dirtyTowerHeight = false;
		this.RebuildLaunchTowerHeightHandler.ClearScheduler();
	}

	public string GetProperName()
	{
		return base.gameObject.GetProperName();
	}

	public List<ProcessCondition> GetConditionSet(ProcessCondition.ProcessConditionType conditionType)
	{
		RocketProcessConditionDisplayTarget rocketProcessConditionDisplayTarget = null;
		RocketModuleCluster landedRocket = this.LandedRocket;
		if (landedRocket != null)
		{
			for (int i = 0; i < landedRocket.CraftInterface.ClusterModules.Count; i++)
			{
				RocketProcessConditionDisplayTarget component = landedRocket.CraftInterface.ClusterModules[i].Get().GetComponent<RocketProcessConditionDisplayTarget>();
				if (component != null)
				{
					rocketProcessConditionDisplayTarget = component;
					break;
				}
			}
		}
		if (rocketProcessConditionDisplayTarget != null)
		{
			return ((IProcessConditionSet)rocketProcessConditionDisplayTarget).GetConditionSet(conditionType);
		}
		return new List<ProcessCondition>();
	}

	public int PopulateConditionSet(ProcessCondition.ProcessConditionType conditionType, List<ProcessCondition> conditions)
	{
		RocketProcessConditionDisplayTarget rocketProcessConditionDisplayTarget = null;
		RocketModuleCluster landedRocket = this.LandedRocket;
		if (landedRocket != null)
		{
			for (int i = 0; i < landedRocket.CraftInterface.ClusterModules.Count; i++)
			{
				RocketProcessConditionDisplayTarget component = landedRocket.CraftInterface.ClusterModules[i].Get().GetComponent<RocketProcessConditionDisplayTarget>();
				if (component != null)
				{
					rocketProcessConditionDisplayTarget = component;
					break;
				}
			}
		}
		if (rocketProcessConditionDisplayTarget != null)
		{
			return ((IProcessConditionSet)rocketProcessConditionDisplayTarget).PopulateConditionSet(conditionType, conditions);
		}
		return 0;
	}

	public static List<LaunchPad> GetLaunchPadsForDestination(AxialI destination)
	{
		List<LaunchPad> list = new List<LaunchPad>();
		foreach (object obj in Components.LaunchPads)
		{
			LaunchPad launchPad = (LaunchPad)obj;
			if (launchPad.GetMyWorldLocation() == destination)
			{
				list.Add(launchPad);
			}
		}
		return list;
	}

	public HashedString triggerPort;

	public HashedString statusPort;

	public HashedString landedRocketPort;

	private CellOffset baseModulePosition = new CellOffset(0, 2);

	[MyCmpReq]
	private LogicPorts ports;

	[MyCmpReq]
	private KSelectable selectable;

	private SchedulerHandle RebuildLaunchTowerHeightHandler;

	private AttachableBuilding lastBaseAttachable;

	private LaunchPad.LaunchPadTower tower;

	[Serialize]
	public int maxTowerHeight;

	private bool dirtyTowerHeight;

	private HandleVector<int>.Handle partitionerEntry;

	private Guid landedRocketPassengerModuleStatusItem = Guid.Empty;

	public class LaunchPadTower
	{
		public LaunchPadTower(LaunchPad pad, int startHeight)
		{
			this.pad = pad;
			this.SetTowerHeight(startHeight);
		}

		public void AddTowerRow()
		{
			GameObject gameObject = new GameObject("LaunchPadTowerRow");
			gameObject.SetActive(false);
			gameObject.transform.SetParent(this.pad.transform);
			gameObject.transform.SetLocalPosition(Grid.CellSizeInMeters * Vector3.up * (float)(this.towerAnimControllers.Count + this.pad.baseModulePosition.y));
			gameObject.transform.SetPosition(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, Grid.GetLayerZ(Grid.SceneLayer.Backwall)));
			KBatchedAnimController kbatchedAnimController = gameObject.AddComponent<KBatchedAnimController>();
			kbatchedAnimController.AnimFiles = new KAnimFile[] { Assets.GetAnim("rocket_launchpad_tower_kanim") };
			gameObject.SetActive(true);
			this.towerAnimControllers.Add(kbatchedAnimController);
			kbatchedAnimController.initialAnim = this.towerBGAnimNames[this.towerAnimControllers.Count % this.towerBGAnimNames.Length] + this.towerBGAnimSuffix_off;
			this.animLink = new KAnimLink(this.pad.GetComponent<KAnimControllerBase>(), kbatchedAnimController);
		}

		public void RemoveTowerRow()
		{
		}

		public void SetTowerHeight(int height)
		{
			if (height < 8)
			{
				height = 0;
			}
			this.targetHeight = height;
			this.pad.maxTowerHeight = height;
			while (this.targetHeight > this.towerAnimControllers.Count)
			{
				this.AddTowerRow();
			}
			if (this.activeAnimationRoutine != null)
			{
				this.pad.StopCoroutine(this.activeAnimationRoutine);
			}
			this.activeAnimationRoutine = this.pad.StartCoroutine(this.TowerRoutine());
		}

		private IEnumerator TowerRoutine()
		{
			while (this.currentHeight < this.targetHeight)
			{
				LaunchPad.LaunchPadTower.<>c__DisplayClass15_0 CS$<>8__locals1 = new LaunchPad.LaunchPadTower.<>c__DisplayClass15_0();
				CS$<>8__locals1.animComplete = false;
				this.towerAnimControllers[this.currentHeight].Queue(this.towerBGAnimNames[this.currentHeight % this.towerBGAnimNames.Length] + this.towerBGAnimSuffix_on_pre, KAnim.PlayMode.Once, 1f, 0f);
				this.towerAnimControllers[this.currentHeight].Queue(this.towerBGAnimNames[this.currentHeight % this.towerBGAnimNames.Length] + this.towerBGAnimSuffix_on, KAnim.PlayMode.Once, 1f, 0f);
				this.towerAnimControllers[this.currentHeight].onAnimComplete += delegate(HashedString arg)
				{
					CS$<>8__locals1.animComplete = true;
				};
				float delay = 0.25f;
				while (!CS$<>8__locals1.animComplete && delay > 0f)
				{
					delay -= Time.deltaTime;
					yield return 0;
				}
				this.currentHeight++;
				CS$<>8__locals1 = null;
			}
			while (this.currentHeight > this.targetHeight)
			{
				LaunchPad.LaunchPadTower.<>c__DisplayClass15_1 CS$<>8__locals2 = new LaunchPad.LaunchPadTower.<>c__DisplayClass15_1();
				this.currentHeight--;
				CS$<>8__locals2.animComplete = false;
				this.towerAnimControllers[this.currentHeight].Queue(this.towerBGAnimNames[this.currentHeight % this.towerBGAnimNames.Length] + this.towerBGAnimSuffix_off_pre, KAnim.PlayMode.Once, 1f, 0f);
				this.towerAnimControllers[this.currentHeight].Queue(this.towerBGAnimNames[this.currentHeight % this.towerBGAnimNames.Length] + this.towerBGAnimSuffix_off, KAnim.PlayMode.Once, 1f, 0f);
				this.towerAnimControllers[this.currentHeight].onAnimComplete += delegate(HashedString arg)
				{
					CS$<>8__locals2.animComplete = true;
				};
				float delay = 0.25f;
				while (!CS$<>8__locals2.animComplete && delay > 0f)
				{
					delay -= Time.deltaTime;
					yield return 0;
				}
				CS$<>8__locals2 = null;
			}
			yield return 0;
			yield break;
		}

		private LaunchPad pad;

		private KAnimLink animLink;

		private Coroutine activeAnimationRoutine;

		private string[] towerBGAnimNames = new string[] { "A1", "A2", "A3", "B", "C", "D", "E1", "E2", "F1", "F2" };

		private string towerBGAnimSuffix_on = "_on";

		private string towerBGAnimSuffix_on_pre = "_on_pre";

		private string towerBGAnimSuffix_off_pre = "_off_pre";

		private string towerBGAnimSuffix_off = "_off";

		private List<KBatchedAnimController> towerAnimControllers = new List<KBatchedAnimController>();

		private int targetHeight;

		private int currentHeight;
	}
}
