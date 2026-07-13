using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

public class Clustercraft : ClusterGridEntity, IClusterRange, ISim4000ms, ISim1000ms
{
	public override string Name
	{
		get
		{
			return this.m_name;
		}
	}

	public bool Exploding { get; protected set; }

	public override EntityLayer Layer
	{
		get
		{
			return EntityLayer.Craft;
		}
	}

	public override List<ClusterGridEntity.AnimConfig> AnimConfigs
	{
		get
		{
			return new List<ClusterGridEntity.AnimConfig>
			{
				new ClusterGridEntity.AnimConfig
				{
					animFile = Assets.GetAnim("rocket01_kanim"),
					initialAnim = "idle_loop"
				}
			};
		}
	}

	public override Sprite GetUISprite()
	{
		PassengerRocketModule passengerModule = this.m_moduleInterface.GetPassengerModule();
		if (passengerModule != null)
		{
			return Def.GetUISprite(passengerModule.gameObject, "ui", false).first;
		}
		return Assets.GetSprite("ic_rocket");
	}

	public override bool IsVisible
	{
		get
		{
			return !this.Exploding;
		}
	}

	public override ClusterRevealLevel IsVisibleInFOW
	{
		get
		{
			return ClusterRevealLevel.Visible;
		}
	}

	public override bool SpaceOutInSameHex()
	{
		return true;
	}

	public CraftModuleInterface ModuleInterface
	{
		get
		{
			return this.m_moduleInterface;
		}
	}

	public AxialI Destination
	{
		get
		{
			return this.m_moduleInterface.GetClusterDestinationSelector().GetDestination();
		}
	}

	public float Speed
	{
		get
		{
			float num = this.EnginePower / this.TotalBurden;
			float num2 = num * this.PilotSkillMultiplier;
			bool flag = this.AutoPilotMultiplier > 0.5f;
			bool flag2 = this.ModuleInterface.GetPassengerModule() != null;
			RoboPilotModule robotPilotModule = this.ModuleInterface.GetRobotPilotModule();
			bool flag3 = robotPilotModule != null && robotPilotModule.GetDataBanksStored() > 1f;
			if (flag3 && flag)
			{
				num2 *= 1.5f;
			}
			else if (!flag && flag2)
			{
				num2 *= 0.5f;
			}
			else if (!flag3 && !flag2)
			{
				num2 = 0f;
			}
			if (this.controlStationBuffTimeRemaining > 0f)
			{
				num2 += num * 0.20000005f;
			}
			return num2;
		}
	}

	public float EnginePower
	{
		get
		{
			float num = 0f;
			foreach (Ref<RocketModuleCluster> @ref in this.m_moduleInterface.ClusterModules)
			{
				num += @ref.Get().performanceStats.EnginePower;
			}
			return num;
		}
	}

	public float FuelPerDistance
	{
		get
		{
			float num = 0f;
			foreach (Ref<RocketModuleCluster> @ref in this.m_moduleInterface.ClusterModules)
			{
				num += @ref.Get().performanceStats.FuelKilogramPerDistance;
			}
			return num;
		}
	}

	public float TotalBurden
	{
		get
		{
			float num = 0f;
			foreach (Ref<RocketModuleCluster> @ref in this.m_moduleInterface.ClusterModules)
			{
				num += @ref.Get().performanceStats.Burden;
			}
			global::Debug.Assert(num > 0f);
			return num;
		}
	}

	public bool LaunchRequested
	{
		get
		{
			return this.m_launchRequested;
		}
		private set
		{
			this.m_launchRequested = value;
			this.m_moduleInterface.TriggerEventOnCraftAndRocket(GameHashes.RocketRequestLaunch, this);
		}
	}

	public Clustercraft.CraftStatus Status
	{
		get
		{
			return this.status;
		}
	}

	public void SetCraftStatus(Clustercraft.CraftStatus craft_status)
	{
		this.status = craft_status;
		this.UpdateGroundTags();
		this.m_moduleInterface.TriggerEventOnCraftAndRocket(GameHashes.ClustercraftStateChanged, craft_status);
	}

	public void SetExploding()
	{
		this.Exploding = true;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Components.Clustercrafts.Add(this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.m_clusterTraveler.getSpeedCB = new Func<float>(this.GetSpeed);
		this.m_clusterTraveler.getCanTravelCB = new Func<bool, bool>(this.CanTravel);
		this.m_clusterTraveler.onTravelCB = new global::System.Action(this.BurnFuelForTravel);
		this.m_clusterTraveler.validateTravelCB = new Func<AxialI, bool>(this.CanTravelToCell);
		this.UpdateGroundTags();
		base.Subscribe<Clustercraft>(1512695988, Clustercraft.RocketModuleChangedHandler);
		base.Subscribe<Clustercraft>(543433792, Clustercraft.ClusterDestinationChangedHandler);
		base.Subscribe<Clustercraft>(1796608350, Clustercraft.ClusterDestinationReachedHandler);
		base.Subscribe(-688990705, delegate(object o)
		{
			this.UpdateStatusItem();
		});
		base.Subscribe<Clustercraft>(1102426921, Clustercraft.NameChangedHandler);
		this.SetRocketName(this.m_name);
		this.UpdateStatusItem();
		this.RefreshStarBackgroundVariables();
	}

	public void Sim1000ms(float dt)
	{
		this.controlStationBuffTimeRemaining = Mathf.Max(this.controlStationBuffTimeRemaining - dt, 0f);
		if (this.controlStationBuffTimeRemaining > 0f)
		{
			this.missionControlStatusHandle = this.selectable.AddStatusItem(Db.Get().BuildingStatusItems.MissionControlBoosted, this);
			return;
		}
		this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.MissionControlBoosted, false);
		this.missionControlStatusHandle = Guid.Empty;
	}

	public void Sim4000ms(float dt)
	{
		RocketClusterDestinationSelector clusterDestinationSelector = this.m_moduleInterface.GetClusterDestinationSelector();
		if (this.Status == Clustercraft.CraftStatus.InFlight && this.m_location == clusterDestinationSelector.GetDestination())
		{
			this.OnClusterDestinationReached(null);
		}
	}

	public void Init(AxialI location, LaunchPad pad)
	{
		this.m_location = location;
		base.GetComponent<RocketClusterDestinationSelector>().SetDestination(this.m_location);
		this.SetRocketName(GameUtil.GenerateRandomRocketName());
		if (pad != null)
		{
			this.Land(pad, true);
		}
		this.UpdateStatusItem();
	}

	protected override void OnCleanUp()
	{
		Components.Clustercrafts.Remove(this);
		base.OnCleanUp();
	}

	private bool CanTravel(bool tryingToLand)
	{
		return this.HasTag(GameTags.RocketInSpace) && (tryingToLand || this.HasResourcesToMove(1, Clustercraft.CombustionResource.All));
	}

	private bool CanTravelToCell(AxialI location)
	{
		return !(ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(location, EntityLayer.Asteroid) != null) || this.CanLandAtAsteroid(location, true);
	}

	private float GetSpeed()
	{
		return this.Speed;
	}

	private void RocketModuleChanged(object data)
	{
		RocketModuleCluster rocketModuleCluster = (RocketModuleCluster)data;
		if (rocketModuleCluster != null)
		{
			this.UpdateGroundTags(rocketModuleCluster.gameObject);
		}
	}

	private void OnClusterDestinationChanged(object _)
	{
		this.UpdateStatusItem();
		this.RefreshStarBackgroundVariables();
	}

	private void OnClusterDestinationReached(object _)
	{
		RocketClusterDestinationSelector clusterDestinationSelector = this.m_moduleInterface.GetClusterDestinationSelector();
		global::Debug.Assert(base.Location == clusterDestinationSelector.GetDestination());
		if (clusterDestinationSelector.HasAsteroidDestination())
		{
			LaunchPad destinationPad = clusterDestinationSelector.GetDestinationPad();
			this.Land(base.Location, destinationPad);
		}
		this.UpdateStatusItem();
		this.RefreshStarBackgroundVariables();
	}

	public void SetRocketName(object newName)
	{
		this.SetRocketName((string)newName);
	}

	public void RefreshStarBackgroundVariables()
	{
		bool flag = this.IsFlightInProgress() && this.HasResourcesToMove(1, Clustercraft.CombustionResource.All);
		if (this.wasFlying != flag)
		{
			if (flag)
			{
				this.LastTimeFlightBegan = Time.timeSinceLevelLoad;
			}
			else
			{
				this.LastTimeFlightStopped = Time.timeSinceLevelLoad;
			}
			this.wasFlying = flag;
		}
	}

	public void SetRocketName(string newName)
	{
		this.m_name = newName;
		base.name = "Clustercraft: " + newName;
		foreach (Ref<RocketModuleCluster> @ref in this.m_moduleInterface.ClusterModules)
		{
			CharacterOverlay component = @ref.Get().GetComponent<CharacterOverlay>();
			if (component != null)
			{
				NameDisplayScreen.Instance.UpdateName(component.gameObject);
				break;
			}
		}
		ClusterManager.Instance.Trigger(1943181844, newName);
	}

	public bool CheckPreppedForLaunch()
	{
		return this.m_moduleInterface.CheckPreppedForLaunch();
	}

	public bool CheckReadyToLaunch()
	{
		return this.m_moduleInterface.CheckReadyToLaunch();
	}

	public bool IsFlightInProgress()
	{
		return this.Status == Clustercraft.CraftStatus.InFlight && this.m_clusterTraveler.IsTraveling();
	}

	public ClusterGridEntity GetPOIAtCurrentLocation()
	{
		if ((this.status != Clustercraft.CraftStatus.InFlight || this.IsFlightInProgress()) && (this.status != Clustercraft.CraftStatus.Launching || !(this.m_location == this.Destination)))
		{
			return null;
		}
		return ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(this.m_location, EntityLayer.POI);
	}

	public ClusterGridEntity GetStableOrbitAsteroid()
	{
		if (this.status != Clustercraft.CraftStatus.InFlight || this.IsFlightInProgress())
		{
			return null;
		}
		return ClusterGrid.Instance.GetVisibleEntityOfLayerAtAdjacentCell(this.m_location, EntityLayer.Asteroid);
	}

	public ClusterGridEntity GetOrbitAsteroid()
	{
		if (this.status != Clustercraft.CraftStatus.InFlight)
		{
			return null;
		}
		return ClusterGrid.Instance.GetVisibleEntityOfLayerAtAdjacentCell(this.m_location, EntityLayer.Asteroid);
	}

	public ClusterGridEntity GetAdjacentAsteroid()
	{
		return ClusterGrid.Instance.GetVisibleEntityOfLayerAtAdjacentCell(this.m_location, EntityLayer.Asteroid);
	}

	private bool CheckDesinationInRange()
	{
		return this.m_clusterTraveler.CurrentPath != null && this.Speed * this.m_clusterTraveler.TravelETA() <= this.ModuleInterface.Range;
	}

	public bool HasResourcesToMove(int hexes = 1, Clustercraft.CombustionResource combustionResource = Clustercraft.CombustionResource.All)
	{
		switch (combustionResource)
		{
		case Clustercraft.CombustionResource.Fuel:
			return this.m_moduleInterface.FuelRemaining / this.FuelPerDistance >= 600f * (float)hexes - 0.001f;
		case Clustercraft.CombustionResource.Oxidizer:
			return this.m_moduleInterface.OxidizerPowerRemaining / this.FuelPerDistance >= 600f * (float)hexes - 0.001f;
		case Clustercraft.CombustionResource.All:
			return this.m_moduleInterface.BurnableMassRemaining / this.FuelPerDistance >= 600f * (float)hexes - 0.001f;
		default:
		{
			bool flag;
			RocketModuleCluster primaryPilotModule = this.m_moduleInterface.GetPrimaryPilotModule(out flag);
			return flag && primaryPilotModule.GetComponent<RoboPilotModule>().HasResourcesToMove(hexes);
		}
		}
	}

	private void BurnFuelForTravel()
	{
		float num = 600f;
		foreach (Ref<RocketModuleCluster> @ref in this.m_moduleInterface.ClusterModules)
		{
			RocketModuleCluster rocketModuleCluster = @ref.Get();
			RocketEngineCluster component = rocketModuleCluster.GetComponent<RocketEngineCluster>();
			if (component != null)
			{
				Tag fuelTag = component.fuelTag;
				float num2 = 0f;
				if (component.requireOxidizer)
				{
					num2 = this.ModuleInterface.OxidizerPowerRemaining;
				}
				if (num > 0f)
				{
					foreach (Ref<RocketModuleCluster> ref2 in this.m_moduleInterface.ClusterModules)
					{
						IFuelTank component2 = ref2.Get().GetComponent<IFuelTank>();
						if (!component2.IsNullOrDestroyed())
						{
							num -= this.BurnFromTank(num, component, fuelTag, component2.Storage, ref num2);
						}
						if (num <= 0f)
						{
							break;
						}
					}
				}
			}
			RoboPilotModule component3 = rocketModuleCluster.GetComponent<RoboPilotModule>();
			if (component3 != null)
			{
				component3.ConsumeDataBanksInFlight();
			}
		}
		this.UpdateStatusItem();
	}

	private float BurnFromTank(float attemptTravelAmount, RocketEngineCluster engine, Tag fuelTag, IStorage storage, ref float totalOxidizerRemaining)
	{
		float num = attemptTravelAmount * engine.GetComponent<RocketModuleCluster>().performanceStats.FuelKilogramPerDistance;
		num = Mathf.Min(storage.GetAmountAvailable(fuelTag), num);
		if (engine.requireOxidizer)
		{
			num = Mathf.Min(num, totalOxidizerRemaining);
		}
		storage.ConsumeIgnoringDisease(fuelTag, num);
		if (engine.requireOxidizer)
		{
			this.BurnOxidizer(num);
			totalOxidizerRemaining -= num;
		}
		return num / engine.GetComponent<RocketModuleCluster>().performanceStats.FuelKilogramPerDistance;
	}

	private void BurnOxidizer(float fuelEquivalentKGs)
	{
		foreach (Ref<RocketModuleCluster> @ref in this.m_moduleInterface.ClusterModules)
		{
			OxidizerTank component = @ref.Get().GetComponent<OxidizerTank>();
			if (component != null)
			{
				foreach (KeyValuePair<Tag, float> keyValuePair in component.GetOxidizersAvailable())
				{
					float num = Clustercraft.dlc1OxidizerEfficiencies[keyValuePair.Key];
					float num2 = Mathf.Min(fuelEquivalentKGs / num, keyValuePair.Value);
					if (num2 > 0f)
					{
						component.storage.ConsumeIgnoringDisease(keyValuePair.Key, num2);
						fuelEquivalentKGs -= num2 * num;
					}
				}
			}
			if (fuelEquivalentKGs <= 0f)
			{
				break;
			}
		}
	}

	public List<ResourceHarvestModule.StatesInstance> GetAllResourceHarvestModules()
	{
		List<ResourceHarvestModule.StatesInstance> list = new List<ResourceHarvestModule.StatesInstance>();
		foreach (Ref<RocketModuleCluster> @ref in this.m_moduleInterface.ClusterModules)
		{
			ResourceHarvestModule.StatesInstance smi = @ref.Get().GetSMI<ResourceHarvestModule.StatesInstance>();
			if (smi != null)
			{
				list.Add(smi);
			}
		}
		return list;
	}

	public List<ArtifactHarvestModule.StatesInstance> GetAllArtifactHarvestModules()
	{
		List<ArtifactHarvestModule.StatesInstance> list = new List<ArtifactHarvestModule.StatesInstance>();
		foreach (Ref<RocketModuleCluster> @ref in this.m_moduleInterface.ClusterModules)
		{
			ArtifactHarvestModule.StatesInstance smi = @ref.Get().GetSMI<ArtifactHarvestModule.StatesInstance>();
			if (smi != null)
			{
				list.Add(smi);
			}
		}
		return list;
	}

	public List<RocketModuleHexCellCollector.Instance> GetAllHexCellCollectorModules()
	{
		List<RocketModuleHexCellCollector.Instance> list = new List<RocketModuleHexCellCollector.Instance>();
		foreach (Ref<RocketModuleCluster> @ref in this.m_moduleInterface.ClusterModules)
		{
			RocketModuleHexCellCollector.Instance smi = @ref.Get().GetSMI<RocketModuleHexCellCollector.Instance>();
			if (smi != null)
			{
				list.Add(smi);
			}
		}
		return list;
	}

	public List<CargoBayCluster> GetAllCargoBays()
	{
		List<CargoBayCluster> list = new List<CargoBayCluster>();
		foreach (Ref<RocketModuleCluster> @ref in this.m_moduleInterface.ClusterModules)
		{
			CargoBayCluster component = @ref.Get().GetComponent<CargoBayCluster>();
			if (component != null)
			{
				list.Add(component);
			}
		}
		return list;
	}

	public List<CargoBayCluster> GetCargoBaysOfType(CargoBay.CargoType cargoType)
	{
		List<CargoBayCluster> list = new List<CargoBayCluster>();
		foreach (Ref<RocketModuleCluster> @ref in this.m_moduleInterface.ClusterModules)
		{
			CargoBayCluster component = @ref.Get().GetComponent<CargoBayCluster>();
			if (component != null && component.storageType == cargoType)
			{
				list.Add(component);
			}
		}
		return list;
	}

	public void DestroyCraftAndModules()
	{
		WorldContainer interiorWorld = this.m_moduleInterface.GetInteriorWorld();
		if (interiorWorld != null)
		{
			NameDisplayScreen.Instance.RemoveWorldEntries(interiorWorld.id);
		}
		List<RocketModuleCluster> list = this.m_moduleInterface.ClusterModules.Select<Ref<RocketModuleCluster>, RocketModuleCluster>((Ref<RocketModuleCluster> x) => x.Get()).ToList<RocketModuleCluster>();
		for (int i = list.Count - 1; i >= 0; i--)
		{
			RocketModuleCluster rocketModuleCluster = list[i];
			Storage component = rocketModuleCluster.GetComponent<Storage>();
			if (component != null)
			{
				component.ConsumeAllIgnoringDisease();
			}
			MinionStorage component2 = rocketModuleCluster.GetComponent<MinionStorage>();
			if (component2 != null)
			{
				List<MinionStorage.Info> storedMinionInfo = component2.GetStoredMinionInfo();
				for (int j = storedMinionInfo.Count - 1; j >= 0; j--)
				{
					component2.DeleteStoredMinion(storedMinionInfo[j].id);
				}
			}
			Util.KDestroyGameObject(rocketModuleCluster.gameObject);
		}
		Util.KDestroyGameObject(base.gameObject);
	}

	public void CancelLaunch()
	{
		if (this.LaunchRequested)
		{
			global::Debug.Log("Cancelling launch!");
			this.LaunchRequested = false;
		}
	}

	public void RequestLaunch(bool automated = false)
	{
		if (this.HasTag(GameTags.RocketNotOnGround) || this.m_moduleInterface.GetClusterDestinationSelector().IsAtDestination())
		{
			return;
		}
		if (DebugHandler.InstantBuildMode && !automated)
		{
			this.Launch(false);
		}
		if (this.LaunchRequested)
		{
			return;
		}
		if (!this.CheckPreppedForLaunch())
		{
			return;
		}
		global::Debug.Log("Triggering launch!");
		if (this.m_moduleInterface.GetRobotPilotModule() != null)
		{
			this.Launch(automated);
		}
		this.LaunchRequested = true;
	}

	public void Launch(bool automated = false)
	{
		if (this.HasTag(GameTags.RocketNotOnGround) || this.m_moduleInterface.GetClusterDestinationSelector().IsAtDestination())
		{
			this.LaunchRequested = false;
			return;
		}
		if ((!DebugHandler.InstantBuildMode || automated) && !this.CheckReadyToLaunch())
		{
			return;
		}
		if (automated && !this.m_moduleInterface.CheckReadyForAutomatedLaunchCommand())
		{
			this.LaunchRequested = false;
			return;
		}
		this.LaunchRequested = false;
		this.SetCraftStatus(Clustercraft.CraftStatus.Launching);
		this.m_moduleInterface.DoLaunch();
		this.BurnFuelForTravel();
		this.m_clusterTraveler.AdvancePathOneStep();
		this.UpdateStatusItem();
	}

	public void LandAtPad(LaunchPad pad)
	{
		this.m_moduleInterface.GetClusterDestinationSelector().SetDestinationPad(pad);
	}

	public Clustercraft.PadLandingStatus CanLandAtPad(LaunchPad pad, out string failReason)
	{
		if (pad == null)
		{
			failReason = UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.NONEAVAILABLE;
			return Clustercraft.PadLandingStatus.CanNeverLand;
		}
		if (pad.HasRocket() && pad.LandedRocket.CraftInterface != this.m_moduleInterface)
		{
			failReason = "<TEMP>The pad already has a rocket on it!<TEMP>";
			return Clustercraft.PadLandingStatus.CanLandEventually;
		}
		if (ConditionFlightPathIsClear.PadTopEdgeDistanceToCeilingEdge(pad.gameObject) < this.ModuleInterface.RocketHeight)
		{
			failReason = UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.DROPDOWN_TOOLTIP_TOO_SHORT;
			return Clustercraft.PadLandingStatus.CanNeverLand;
		}
		int num = -1;
		if (!ConditionFlightPathIsClear.CheckFlightPathClear(this.ModuleInterface, pad.gameObject, out num))
		{
			failReason = string.Format(UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.DROPDOWN_TOOLTIP_PATH_OBSTRUCTED, pad.GetProperName());
			return Clustercraft.PadLandingStatus.CanNeverLand;
		}
		if (!pad.GetComponent<Operational>().IsOperational)
		{
			failReason = UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.DROPDOWN_TOOLTIP_PAD_DISABLED;
			return Clustercraft.PadLandingStatus.CanNeverLand;
		}
		int rocketBottomPosition = pad.RocketBottomPosition;
		foreach (Ref<RocketModuleCluster> @ref in this.ModuleInterface.ClusterModules)
		{
			GameObject gameObject = @ref.Get().gameObject;
			int moduleRelativeVerticalPosition = this.ModuleInterface.GetModuleRelativeVerticalPosition(gameObject);
			Building component = gameObject.GetComponent<Building>();
			BuildingUnderConstruction component2 = gameObject.GetComponent<BuildingUnderConstruction>();
			BuildingDef buildingDef = ((component != null) ? component.Def : component2.Def);
			for (int i = 0; i < buildingDef.WidthInCells; i++)
			{
				for (int j = 0; j < buildingDef.HeightInCells; j++)
				{
					int num2 = Grid.OffsetCell(rocketBottomPosition, 0, moduleRelativeVerticalPosition);
					num2 = Grid.OffsetCell(num2, -(buildingDef.WidthInCells / 2) + i, j);
					if (Grid.Solid[num2])
					{
						num = num2;
						failReason = string.Format(UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.DROPDOWN_TOOLTIP_SITE_OBSTRUCTED, pad.GetProperName());
						return Clustercraft.PadLandingStatus.CanNeverLand;
					}
				}
			}
		}
		failReason = null;
		return Clustercraft.PadLandingStatus.CanLandImmediately;
	}

	private LaunchPad FindValidLandingPad(AxialI location, bool mustLandImmediately)
	{
		LaunchPad launchPad = null;
		int asteroidWorldIdAtLocation = ClusterUtil.GetAsteroidWorldIdAtLocation(location);
		LaunchPad preferredLaunchPadForWorld = this.m_moduleInterface.GetPreferredLaunchPadForWorld(asteroidWorldIdAtLocation);
		string text;
		if (preferredLaunchPadForWorld != null && this.CanLandAtPad(preferredLaunchPadForWorld, out text) == Clustercraft.PadLandingStatus.CanLandImmediately)
		{
			return preferredLaunchPadForWorld;
		}
		foreach (object obj in Components.LaunchPads)
		{
			LaunchPad launchPad2 = (LaunchPad)obj;
			if (launchPad2.GetMyWorldLocation() == location)
			{
				string text2;
				Clustercraft.PadLandingStatus padLandingStatus = this.CanLandAtPad(launchPad2, out text2);
				if (padLandingStatus == Clustercraft.PadLandingStatus.CanLandImmediately)
				{
					return launchPad2;
				}
				if (!mustLandImmediately && padLandingStatus == Clustercraft.PadLandingStatus.CanLandEventually)
				{
					launchPad = launchPad2;
				}
			}
		}
		return launchPad;
	}

	public bool CanLandAtAsteroid(AxialI location, bool mustLandImmediately)
	{
		LaunchPad destinationPad = this.m_moduleInterface.GetClusterDestinationSelector().GetDestinationPad();
		global::Debug.Assert(destinationPad == null || destinationPad.GetMyWorldLocation() == location, "A rocket is trying to travel to an asteroid but has selected a landing pad at a different asteroid!");
		if (destinationPad != null)
		{
			string text;
			Clustercraft.PadLandingStatus padLandingStatus = this.CanLandAtPad(destinationPad, out text);
			return padLandingStatus == Clustercraft.PadLandingStatus.CanLandImmediately || (!mustLandImmediately && padLandingStatus == Clustercraft.PadLandingStatus.CanLandEventually);
		}
		return this.FindValidLandingPad(location, mustLandImmediately) != null;
	}

	private void Land(LaunchPad pad, bool forceGrounded)
	{
		string text;
		if (this.CanLandAtPad(pad, out text) != Clustercraft.PadLandingStatus.CanLandImmediately)
		{
			return;
		}
		this.BurnFuelForTravel();
		this.m_location = pad.GetMyWorldLocation();
		this.SetCraftStatus(forceGrounded ? Clustercraft.CraftStatus.Grounded : Clustercraft.CraftStatus.Landing);
		this.m_moduleInterface.DoLand(pad);
		this.UpdateStatusItem();
	}

	private void Land(AxialI destination, LaunchPad chosenPad)
	{
		if (chosenPad == null)
		{
			chosenPad = this.FindValidLandingPad(destination, true);
		}
		global::Debug.Assert(chosenPad == null || chosenPad.GetMyWorldLocation() == this.m_location, "Attempting to land on a pad that isn't at our current position");
		this.Land(chosenPad, false);
	}

	public void UpdateStatusItem()
	{
		if (ClusterGrid.Instance == null)
		{
			return;
		}
		if (this.mainStatusHandle != Guid.Empty)
		{
			this.selectable.RemoveStatusItem(this.mainStatusHandle, false);
		}
		ClusterGridEntity visibleEntityOfLayerAtCell = ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(this.m_location, EntityLayer.Asteroid);
		ClusterGridEntity orbitAsteroid = this.GetOrbitAsteroid();
		bool flag = false;
		if (orbitAsteroid != null)
		{
			using (IEnumerator enumerator = Components.LaunchPads.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (((LaunchPad)enumerator.Current).GetMyWorldLocation() == orbitAsteroid.Location)
					{
						flag = true;
						break;
					}
				}
			}
		}
		bool flag2 = false;
		if (visibleEntityOfLayerAtCell != null)
		{
			this.mainStatusHandle = this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.InFlight, this.m_clusterTraveler);
		}
		else if (!this.HasResourcesToMove(1, Clustercraft.CombustionResource.All) && !flag)
		{
			flag2 = true;
			this.mainStatusHandle = this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.RocketStranded, orbitAsteroid);
		}
		else if (!this.m_moduleInterface.GetClusterDestinationSelector().IsAtDestination() && !this.CheckDesinationInRange())
		{
			this.mainStatusHandle = this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.DestinationOutOfRange, this.m_clusterTraveler);
		}
		else if (orbitAsteroid != null && this.Destination == orbitAsteroid.Location)
		{
			this.mainStatusHandle = this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.WaitingToLand, orbitAsteroid);
		}
		else if (this.IsFlightInProgress() || this.Status == Clustercraft.CraftStatus.Launching)
		{
			this.mainStatusHandle = this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.InFlight, this.m_clusterTraveler);
		}
		else if (orbitAsteroid != null)
		{
			this.mainStatusHandle = this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.InOrbit, orbitAsteroid);
		}
		else
		{
			this.mainStatusHandle = this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Normal, null);
		}
		base.GetComponent<KPrefabID>().SetTag(GameTags.RocketStranded, flag2);
		float num = 0f;
		float num2 = 0f;
		foreach (CargoBayCluster cargoBayCluster in this.GetAllCargoBays())
		{
			num += cargoBayCluster.MaxCapacity;
			num2 += cargoBayCluster.RemainingCapacity;
		}
		if (this.Status != Clustercraft.CraftStatus.Grounded && num > 0f)
		{
			if (num2 == 0f)
			{
				this.selectable.AddStatusItem(Db.Get().BuildingStatusItems.FlightAllCargoFull, null);
				this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.FlightCargoRemaining, false);
			}
			else
			{
				this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.FlightAllCargoFull, false);
				if (this.cargoStatusHandle == Guid.Empty)
				{
					this.cargoStatusHandle = this.selectable.AddStatusItem(Db.Get().BuildingStatusItems.FlightCargoRemaining, num2);
				}
				else
				{
					this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.FlightCargoRemaining, true);
					this.cargoStatusHandle = this.selectable.AddStatusItem(Db.Get().BuildingStatusItems.FlightCargoRemaining, num2);
				}
			}
		}
		else
		{
			this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.FlightCargoRemaining, false);
			this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.FlightAllCargoFull, false);
		}
		this.UpdatePilotedStatusItems();
	}

	private void UpdateGroundTags()
	{
		foreach (Ref<RocketModuleCluster> @ref in this.ModuleInterface.ClusterModules)
		{
			if (@ref != null && !(@ref.Get() == null))
			{
				this.UpdateGroundTags(@ref.Get().gameObject);
			}
		}
		this.UpdateGroundTags(base.gameObject);
	}

	private void UpdateGroundTags(GameObject go)
	{
		this.SetTagOnGameObject(go, GameTags.RocketOnGround, this.status == Clustercraft.CraftStatus.Grounded);
		this.SetTagOnGameObject(go, GameTags.RocketNotOnGround, this.status > Clustercraft.CraftStatus.Grounded);
		this.SetTagOnGameObject(go, GameTags.RocketInSpace, this.status == Clustercraft.CraftStatus.InFlight);
		this.SetTagOnGameObject(go, GameTags.EntityInSpace, this.status == Clustercraft.CraftStatus.InFlight);
	}

	private void UpdatePilotedStatusItems()
	{
		if (this.Status == Clustercraft.CraftStatus.Grounded)
		{
			this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.InFlightUnpiloted, false);
			this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.InFlightPiloted, false);
			this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.InFlightSuperPilot, false);
			return;
		}
		bool flag = false;
		bool flag2 = false;
		this.GetPilotedStatus(out flag, out flag2);
		if (flag && flag2)
		{
			this.selectable.AddStatusItem(Db.Get().BuildingStatusItems.InFlightSuperPilot, this);
			this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.InFlightUnpiloted, false);
			this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.InFlightAutoPiloted, false);
			this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.InFlightPiloted, false);
			return;
		}
		if (flag || flag2)
		{
			this.selectable.AddStatusItem(Db.Get().BuildingStatusItems.InFlightPiloted, this);
			this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.InFlightUnpiloted, false);
			this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.InFlightAutoPiloted, false);
			this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.InFlightSuperPilot, false);
			return;
		}
		if (this.ModuleInterface.GetPassengerModule() != null)
		{
			this.selectable.AddStatusItem(Db.Get().BuildingStatusItems.InFlightAutoPiloted, this);
			this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.InFlightUnpiloted, false);
			this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.InFlightPiloted, false);
			this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.InFlightSuperPilot, false);
			return;
		}
		this.selectable.AddStatusItem(Db.Get().BuildingStatusItems.InFlightUnpiloted, this);
		this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.InFlightAutoPiloted, false);
		this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.InFlightPiloted, false);
		this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.InFlightSuperPilot, false);
	}

	public void GetPilotedStatus(out bool dupe_piloted, out bool robo_piloted)
	{
		dupe_piloted = false;
		robo_piloted = false;
		global::UnityEngine.Object passengerModule = this.ModuleInterface.GetPassengerModule();
		RoboPilotModule robotPilotModule = this.ModuleInterface.GetRobotPilotModule();
		if (passengerModule != null)
		{
			dupe_piloted = this.AutoPilotMultiplier > 0.5f;
		}
		if (robotPilotModule != null)
		{
			robo_piloted = robotPilotModule.GetDataBanksStored() > 0f;
		}
	}

	private void SetTagOnGameObject(GameObject go, Tag tag, bool set)
	{
		if (set)
		{
			go.AddTag(tag);
			return;
		}
		go.RemoveTag(tag);
	}

	public override bool ShowName()
	{
		return this.status > Clustercraft.CraftStatus.Grounded;
	}

	public override bool ShowPath()
	{
		return this.status > Clustercraft.CraftStatus.Grounded;
	}

	public bool IsTravellingAndFueled()
	{
		return this.HasResourcesToMove(1, Clustercraft.CombustionResource.All) && this.m_clusterTraveler.IsTraveling();
	}

	public override bool ShowProgressBar()
	{
		return this.IsTravellingAndFueled();
	}

	public override float GetProgress()
	{
		return this.m_clusterTraveler.GetMoveProgress();
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (this.Status != Clustercraft.CraftStatus.Grounded && SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 27))
		{
			UIScheduler.Instance.ScheduleNextFrame("Check Fuel Costs", delegate(object o)
			{
				foreach (Ref<RocketModuleCluster> @ref in this.ModuleInterface.ClusterModules)
				{
					RocketModuleCluster rocketModuleCluster = @ref.Get();
					IFuelTank component = rocketModuleCluster.GetComponent<IFuelTank>();
					if (component != null && !component.Storage.IsEmpty())
					{
						component.DEBUG_FillTank();
					}
					OxidizerTank component2 = rocketModuleCluster.GetComponent<OxidizerTank>();
					if (component2 != null)
					{
						Dictionary<Tag, float> oxidizersAvailable = component2.GetOxidizersAvailable();
						if (oxidizersAvailable.Count > 0)
						{
							foreach (KeyValuePair<Tag, float> keyValuePair in oxidizersAvailable)
							{
								if (keyValuePair.Value > 0f)
								{
									component2.DEBUG_FillTank(ElementLoader.GetElementID(keyValuePair.Key));
									break;
								}
							}
						}
					}
				}
			}, null, null);
		}
	}

	public float GetRange()
	{
		return this.ModuleInterface.Range;
	}

	public int GetRangeInTiles()
	{
		return this.ModuleInterface.RangeInTiles;
	}

	public int GetMaxRangeInTiles()
	{
		return this.ModuleInterface.MaxRange;
	}

	[Serialize]
	private string m_name;

	private bool wasFlying;

	public float LastTimeFlightBegan;

	public float LastTimeFlightStopped;

	[MyCmpReq]
	private ClusterTraveler m_clusterTraveler;

	[MyCmpReq]
	private CraftModuleInterface m_moduleInterface;

	private Guid mainStatusHandle;

	private Guid cargoStatusHandle;

	private Guid missionControlStatusHandle = Guid.Empty;

	public static Dictionary<Tag, float> dlc1OxidizerEfficiencies = new Dictionary<Tag, float>
	{
		{
			SimHashes.OxyRock.CreateTag(),
			ROCKETRY.DLC1_OXIDIZER_EFFICIENCY.LOW
		},
		{
			SimHashes.LiquidOxygen.CreateTag(),
			ROCKETRY.DLC1_OXIDIZER_EFFICIENCY.HIGH
		},
		{
			SimHashes.Fertilizer.CreateTag(),
			ROCKETRY.DLC1_OXIDIZER_EFFICIENCY.VERY_LOW
		}
	};

	[Serialize]
	[Range(0f, 1f)]
	public float AutoPilotMultiplier = 1f;

	[Serialize]
	[Range(0f, 2f)]
	public float PilotSkillMultiplier = 1f;

	[Serialize]
	public float controlStationBuffTimeRemaining;

	[Serialize]
	private bool m_launchRequested;

	[Serialize]
	private Clustercraft.CraftStatus status;

	[MyCmpGet]
	private KSelectable selectable;

	private static EventSystem.IntraObjectHandler<Clustercraft> RocketModuleChangedHandler = new EventSystem.IntraObjectHandler<Clustercraft>(delegate(Clustercraft cmp, object data)
	{
		cmp.RocketModuleChanged(data);
	});

	private static EventSystem.IntraObjectHandler<Clustercraft> ClusterDestinationChangedHandler = new EventSystem.IntraObjectHandler<Clustercraft>(delegate(Clustercraft cmp, object data)
	{
		cmp.OnClusterDestinationChanged(data);
	});

	private static EventSystem.IntraObjectHandler<Clustercraft> ClusterDestinationReachedHandler = new EventSystem.IntraObjectHandler<Clustercraft>(delegate(Clustercraft cmp, object data)
	{
		cmp.OnClusterDestinationReached(data);
	});

	private static EventSystem.IntraObjectHandler<Clustercraft> NameChangedHandler = new EventSystem.IntraObjectHandler<Clustercraft>(delegate(Clustercraft cmp, object data)
	{
		cmp.SetRocketName(data);
	});

	public enum CraftStatus
	{
		Grounded,
		Launching,
		InFlight,
		Landing
	}

	public enum CombustionResource
	{
		Fuel,
		Oxidizer,
		All
	}

	public enum PadLandingStatus
	{
		CanLandImmediately,
		CanLandEventually,
		CanNeverLand
	}
}
