using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

public class ClusterTelescope : GameStateMachine<ClusterTelescope, ClusterTelescope.Instance, IStateMachineTarget, ClusterTelescope.Def>
{
	private static string GetStatusItemString(string src_str, object data)
	{
		ClusterTelescope.Instance instance = (ClusterTelescope.Instance)data;
		return src_str.Replace("{VISIBILITY}", GameUtil.GetFormattedPercent(instance.PercentClear * 100f, GameUtil.TimeSlice.None)).Replace("{RADIUS}", instance.def.clearScanCellRadius.ToString());
	}

	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.ready.no_visibility;
		this.ready.DoNothing();
		this.ready.no_visibility.UpdateTransition(this.ready.ready_to_work, (ClusterTelescope.Instance smi, float dt) => smi.HasSkyVisibility(), UpdateRate.SIM_200ms, false).ToggleStatusItem(ClusterTelescope.noVisibilityStatusItem, null);
		this.ready.ready_to_work.UpdateTransition(this.ready.no_visibility, (ClusterTelescope.Instance smi, float dt) => !smi.HasSkyVisibility(), UpdateRate.SIM_200ms, false).DefaultState(this.ready.ready_to_work.decide);
		this.ready.ready_to_work.decide.EnterTransition(this.ready.ready_to_work.identifyMeteorShower, (ClusterTelescope.Instance smi) => smi.ShouldBeWorkingOnMeteorIdentification()).EnterTransition(this.ready.ready_to_work.revealTile, (ClusterTelescope.Instance smi) => smi.ShouldBeWorkingOnRevealingTile()).EnterTransition(this.all_work_complete, (ClusterTelescope.Instance smi) => !smi.IsAnyAvailableWorkToBeDone());
		this.ready.ready_to_work.identifyMeteorShower.OnSignal(this.MeteorIdenificationPriorityChangeSignal, this.ready.ready_to_work.decide, (ClusterTelescope.Instance smi) => !smi.ShouldBeWorkingOnMeteorIdentification()).ParamTransition<GameObject>(this.meteorShowerTarget, this.ready.ready_to_work.decide, GameStateMachine<ClusterTelescope, ClusterTelescope.Instance, IStateMachineTarget, ClusterTelescope.Def>.IsNull).EventTransition(GameHashes.ClusterMapMeteorShowerIdentified, (ClusterTelescope.Instance smi) => Game.Instance, this.ready.ready_to_work.decide, (ClusterTelescope.Instance smi) => !smi.ShouldBeWorkingOnMeteorIdentification())
			.EventTransition(GameHashes.ClusterMapMeteorShowerMoved, (ClusterTelescope.Instance smi) => Game.Instance, this.ready.ready_to_work.decide, (ClusterTelescope.Instance smi) => !smi.ShouldBeWorkingOnMeteorIdentification())
			.ToggleChore((ClusterTelescope.Instance smi) => smi.CreateIdentifyMeteorChore(), this.ready.no_visibility);
		this.ready.ready_to_work.revealTile.OnSignal(this.MeteorIdenificationPriorityChangeSignal, this.ready.ready_to_work.decide, (ClusterTelescope.Instance smi) => smi.ShouldBeWorkingOnMeteorIdentification()).EventTransition(GameHashes.ClusterFogOfWarRevealed, (ClusterTelescope.Instance smi) => Game.Instance, this.ready.ready_to_work.decide, (ClusterTelescope.Instance smi) => !smi.ShouldBeWorkingOnRevealingTile()).EventTransition(GameHashes.ClusterMapMeteorShowerMoved, (ClusterTelescope.Instance smi) => Game.Instance, this.ready.ready_to_work.decide, (ClusterTelescope.Instance smi) => smi.ShouldBeWorkingOnMeteorIdentification())
			.ToggleChore((ClusterTelescope.Instance smi) => smi.CreateRevealTileChore(), this.ready.no_visibility);
		this.all_work_complete.OnSignal(this.MeteorIdenificationPriorityChangeSignal, this.ready.no_visibility, (ClusterTelescope.Instance smi) => smi.IsAnyAvailableWorkToBeDone()).ToggleMainStatusItem(Db.Get().BuildingStatusItems.ClusterTelescopeAllWorkComplete, null).EventTransition(GameHashes.ClusterLocationChanged, (ClusterTelescope.Instance smi) => Game.Instance, this.ready.no_visibility, (ClusterTelescope.Instance smi) => smi.IsAnyAvailableWorkToBeDone())
			.EventTransition(GameHashes.ClusterMapMeteorShowerMoved, (ClusterTelescope.Instance smi) => Game.Instance, this.ready.no_visibility, (ClusterTelescope.Instance smi) => smi.ShouldBeWorkingOnMeteorIdentification());
	}

	private static StatusItem noVisibilityStatusItem = new StatusItem("SPACE_VISIBILITY_NONE", "BUILDING", "status_item_no_sky", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022, new Func<string, object, string>(ClusterTelescope.GetStatusItemString));

	public GameStateMachine<ClusterTelescope, ClusterTelescope.Instance, IStateMachineTarget, ClusterTelescope.Def>.State all_work_complete;

	public ClusterTelescope.ReadyStates ready;

	public StateMachine<ClusterTelescope, ClusterTelescope.Instance, IStateMachineTarget, ClusterTelescope.Def>.TargetParameter meteorShowerTarget;

	public StateMachine<ClusterTelescope, ClusterTelescope.Instance, IStateMachineTarget, ClusterTelescope.Def>.Signal MeteorIdenificationPriorityChangeSignal;

	public class Def : StateMachine.BaseDef
	{
		public int clearScanCellRadius = 15;

		public int analyzeClusterRadius = 3;

		public KAnimFile[] workableOverrideAnims;

		public bool providesOxygen;
	}

	public class WorkStates : GameStateMachine<ClusterTelescope, ClusterTelescope.Instance, IStateMachineTarget, ClusterTelescope.Def>.State
	{
		public GameStateMachine<ClusterTelescope, ClusterTelescope.Instance, IStateMachineTarget, ClusterTelescope.Def>.State decide;

		public GameStateMachine<ClusterTelescope, ClusterTelescope.Instance, IStateMachineTarget, ClusterTelescope.Def>.State identifyMeteorShower;

		public GameStateMachine<ClusterTelescope, ClusterTelescope.Instance, IStateMachineTarget, ClusterTelescope.Def>.State revealTile;
	}

	public class ReadyStates : GameStateMachine<ClusterTelescope, ClusterTelescope.Instance, IStateMachineTarget, ClusterTelescope.Def>.State
	{
		public GameStateMachine<ClusterTelescope, ClusterTelescope.Instance, IStateMachineTarget, ClusterTelescope.Def>.State no_visibility;

		public ClusterTelescope.WorkStates ready_to_work;
	}

	public new class Instance : GameStateMachine<ClusterTelescope, ClusterTelescope.Instance, IStateMachineTarget, ClusterTelescope.Def>.GameInstance, ICheckboxControl
	{
		public float PercentClear
		{
			get
			{
				return this.m_percentClear;
			}
		}

		private bool hasMeteorShowerTarget
		{
			get
			{
				return this.meteorShowerTarget != null;
			}
		}

		private ClusterMapMeteorShower.Instance meteorShowerTarget
		{
			get
			{
				GameObject gameObject = base.sm.meteorShowerTarget.Get(this);
				if (gameObject == null)
				{
					return null;
				}
				return gameObject.GetSMI<ClusterMapMeteorShower.Instance>();
			}
		}

		public Instance(IStateMachineTarget smi, ClusterTelescope.Def def)
			: base(smi, def)
		{
			this.workableOverrideAnims = def.workableOverrideAnims;
			this.providesOxygen = def.providesOxygen;
		}

		public bool ShouldBeWorkingOnRevealingTile()
		{
			return this.CheckHasAnalyzeTarget() && (!this.allowMeteorIdentification || !this.CheckHasValidMeteorTarget());
		}

		public bool ShouldBeWorkingOnMeteorIdentification()
		{
			return this.allowMeteorIdentification && this.CheckHasValidMeteorTarget();
		}

		public bool IsAnyAvailableWorkToBeDone()
		{
			return this.CheckHasAnalyzeTarget() || this.ShouldBeWorkingOnMeteorIdentification();
		}

		public bool CheckHasValidMeteorTarget()
		{
			SaveGame.Instance.GetSMI<ClusterFogOfWarManager.Instance>();
			if (this.HasValidMeteor())
			{
				return true;
			}
			ClusterMapMeteorShower.Instance instance = null;
			AxialI myWorldLocation = this.GetMyWorldLocation();
			ClusterGrid.Instance.GetVisibleUnidentifiedMeteorShowerWithinRadius(myWorldLocation, base.def.analyzeClusterRadius, out instance);
			base.sm.meteorShowerTarget.Set((instance == null) ? null : instance.gameObject, this, false);
			return instance != null;
		}

		public bool CheckHasAnalyzeTarget()
		{
			ClusterFogOfWarManager.Instance smi = SaveGame.Instance.GetSMI<ClusterFogOfWarManager.Instance>();
			if (this.m_hasAnalyzeTarget && !smi.IsLocationRevealed(this.m_analyzeTarget))
			{
				return true;
			}
			AxialI myWorldLocation = this.GetMyWorldLocation();
			this.m_hasAnalyzeTarget = smi.GetUnrevealedLocationWithinRadius(myWorldLocation, base.def.analyzeClusterRadius, out this.m_analyzeTarget);
			return this.m_hasAnalyzeTarget;
		}

		private bool HasValidMeteor()
		{
			if (!this.hasMeteorShowerTarget)
			{
				return false;
			}
			AxialI myWorldLocation = this.GetMyWorldLocation();
			bool flag = ClusterGrid.Instance.IsInRange(this.meteorShowerTarget.ClusterGridPosition(), myWorldLocation, base.def.analyzeClusterRadius);
			bool flag2 = SaveGame.Instance.GetSMI<ClusterFogOfWarManager.Instance>().IsLocationRevealed(this.meteorShowerTarget.ClusterGridPosition());
			bool hasBeenIdentified = this.meteorShowerTarget.HasBeenIdentified;
			return flag && flag2 && !hasBeenIdentified;
		}

		public Chore CreateRevealTileChore()
		{
			WorkChore<ClusterTelescope.ClusterTelescopeWorkable> workChore = new WorkChore<ClusterTelescope.ClusterTelescopeWorkable>(Db.Get().ChoreTypes.Research, this.m_workable, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
			if (this.providesOxygen)
			{
				workChore.AddPrecondition(Telescope.ContainsOxygen, null);
			}
			return workChore;
		}

		public Chore CreateIdentifyMeteorChore()
		{
			WorkChore<ClusterTelescope.ClusterTelescopeIdentifyMeteorWorkable> workChore = new WorkChore<ClusterTelescope.ClusterTelescopeIdentifyMeteorWorkable>(Db.Get().ChoreTypes.Research, this.m_identifyMeteorWorkable, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
			if (this.providesOxygen)
			{
				workChore.AddPrecondition(Telescope.ContainsOxygen, null);
			}
			return workChore;
		}

		public ClusterMapMeteorShower.Instance GetMeteorTarget()
		{
			return this.meteorShowerTarget;
		}

		public AxialI GetAnalyzeTarget()
		{
			global::Debug.Assert(this.m_hasAnalyzeTarget, "GetAnalyzeTarget called but this telescope has no target assigned.");
			return this.m_analyzeTarget;
		}

		public bool HasSkyVisibility()
		{
			Extents extents = base.GetComponent<Building>().GetExtents();
			int num;
			bool flag = Grid.IsRangeExposedToSunlight(Grid.XYToCell(extents.x, extents.y), base.def.clearScanCellRadius, new CellOffset(1, 0), out num, 1);
			this.m_percentClear = (float)num / (float)(base.def.clearScanCellRadius * 2 + 1);
			return flag;
		}

		public string CheckboxTitleKey
		{
			get
			{
				return "STRINGS.UI.UISIDESCREENS.CLUSTERTELESCOPESIDESCREEN.TITLE";
			}
		}

		public string CheckboxLabel
		{
			get
			{
				return UI.UISIDESCREENS.CLUSTERTELESCOPESIDESCREEN.CHECKBOX_METEORS;
			}
		}

		public string CheckboxTooltip
		{
			get
			{
				return UI.UISIDESCREENS.CLUSTERTELESCOPESIDESCREEN.CHECKBOX_TOOLTIP_METEORS;
			}
		}

		public bool GetCheckboxValue()
		{
			return this.allowMeteorIdentification;
		}

		public void SetCheckboxValue(bool value)
		{
			this.allowMeteorIdentification = value;
			base.sm.MeteorIdenificationPriorityChangeSignal.Trigger(this);
		}

		private float m_percentClear;

		[Serialize]
		public bool allowMeteorIdentification = true;

		[Serialize]
		private bool m_hasAnalyzeTarget;

		[Serialize]
		private AxialI m_analyzeTarget;

		[MyCmpAdd]
		private ClusterTelescope.ClusterTelescopeWorkable m_workable;

		[MyCmpAdd]
		private ClusterTelescope.ClusterTelescopeIdentifyMeteorWorkable m_identifyMeteorWorkable;

		public KAnimFile[] workableOverrideAnims;

		public bool providesOxygen;
	}

	public class ClusterTelescopeWorkable : Workable, OxygenBreather.IGasProvider
	{
		protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
			this.attributeConverter = Db.Get().AttributeConverters.ResearchSpeed;
			this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.ALL_DAY_EXPERIENCE;
			this.skillExperienceSkillGroup = Db.Get().SkillGroups.Research.Id;
			this.skillExperienceMultiplier = SKILLS.ALL_DAY_EXPERIENCE;
			this.requiredSkillPerk = Db.Get().SkillPerks.CanUseClusterTelescope.Id;
			this.workLayer = Grid.SceneLayer.BuildingUse;
			this.radiationShielding = new AttributeModifier(Db.Get().Attributes.RadiationResistance.Id, FIXEDTRAITS.COSMICRADIATION.TELESCOPE_RADIATION_SHIELDING, global::STRINGS.BUILDINGS.PREFABS.CLUSTERTELESCOPEENCLOSED.NAME, false, false, true);
		}

		protected override void OnCleanUp()
		{
			if (this.telescopeTargetMarker != null)
			{
				Util.KDestroyGameObject(this.telescopeTargetMarker);
			}
			base.OnCleanUp();
		}

		protected override void OnSpawn()
		{
			base.OnSpawn();
			this.OnWorkableEventCB = (Action<Workable, Workable.WorkableEvent>)Delegate.Combine(this.OnWorkableEventCB, new Action<Workable, Workable.WorkableEvent>(this.OnWorkableEvent));
			this.m_fowManager = SaveGame.Instance.GetSMI<ClusterFogOfWarManager.Instance>();
			base.SetWorkTime(float.PositiveInfinity);
			this.overrideAnims = this.m_telescope.workableOverrideAnims;
		}

		private void OnWorkableEvent(Workable workable, Workable.WorkableEvent ev)
		{
			Worker worker = base.worker;
			if (worker == null)
			{
				return;
			}
			KPrefabID component = worker.GetComponent<KPrefabID>();
			OxygenBreather component2 = worker.GetComponent<OxygenBreather>();
			Attributes attributes = worker.GetAttributes();
			if (ev == Workable.WorkableEvent.WorkStarted)
			{
				base.ShowProgressBar(true);
				this.progressBar.SetUpdateFunc(() => this.m_fowManager.GetRevealCompleteFraction(this.currentTarget));
				this.currentTarget = this.m_telescope.GetAnalyzeTarget();
				if (!ClusterGrid.Instance.GetEntityOfLayerAtCell(this.currentTarget, EntityLayer.Telescope))
				{
					this.telescopeTargetMarker = GameUtil.KInstantiate(Assets.GetPrefab("TelescopeTarget"), Grid.SceneLayer.Background, null, 0);
					this.telescopeTargetMarker.SetActive(true);
					this.telescopeTargetMarker.GetComponent<TelescopeTarget>().Init(this.currentTarget);
					this.telescopeTargetMarker.GetComponent<TelescopeTarget>().SetTargetMeteorShower(null);
				}
				if (this.m_telescope.providesOxygen)
				{
					attributes.Add(this.radiationShielding);
					this.workerGasProvider = component2.GetGasProvider();
					component2.SetGasProvider(this);
					component2.GetComponent<CreatureSimTemperatureTransfer>().enabled = false;
					component.AddTag(GameTags.Shaded, false);
				}
				base.GetComponent<Operational>().SetActive(true, false);
				this.checkMarkerFrequency = global::UnityEngine.Random.Range(2f, 5f);
				return;
			}
			if (ev != Workable.WorkableEvent.WorkStopped)
			{
				return;
			}
			if (this.m_telescope.providesOxygen)
			{
				attributes.Remove(this.radiationShielding);
				component2.SetGasProvider(this.workerGasProvider);
				component2.GetComponent<CreatureSimTemperatureTransfer>().enabled = true;
				component.RemoveTag(GameTags.Shaded);
			}
			base.GetComponent<Operational>().SetActive(false, false);
			if (this.telescopeTargetMarker != null)
			{
				Util.KDestroyGameObject(this.telescopeTargetMarker);
			}
			base.ShowProgressBar(false);
		}

		public override List<Descriptor> GetDescriptors(GameObject go)
		{
			List<Descriptor> descriptors = base.GetDescriptors(go);
			Element element = ElementLoader.FindElementByHash(SimHashes.Oxygen);
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(element.tag.ProperName(), string.Format(global::STRINGS.BUILDINGS.PREFABS.TELESCOPE.REQUIREMENT_TOOLTIP, element.tag.ProperName()), Descriptor.DescriptorType.Requirement);
			descriptors.Add(descriptor);
			return descriptors;
		}

		protected override bool OnWorkTick(Worker worker, float dt)
		{
			AxialI analyzeTarget = this.m_telescope.GetAnalyzeTarget();
			bool flag = false;
			if (analyzeTarget != this.currentTarget)
			{
				if (this.telescopeTargetMarker)
				{
					this.telescopeTargetMarker.GetComponent<TelescopeTarget>().Init(analyzeTarget);
				}
				this.currentTarget = analyzeTarget;
				flag = true;
			}
			if (!flag && this.checkMarkerTimer > this.checkMarkerFrequency)
			{
				this.checkMarkerTimer = 0f;
				if (!this.telescopeTargetMarker && !ClusterGrid.Instance.GetEntityOfLayerAtCell(this.currentTarget, EntityLayer.Telescope))
				{
					this.telescopeTargetMarker = GameUtil.KInstantiate(Assets.GetPrefab("TelescopeTarget"), Grid.SceneLayer.Background, null, 0);
					this.telescopeTargetMarker.SetActive(true);
					this.telescopeTargetMarker.GetComponent<TelescopeTarget>().Init(this.currentTarget);
				}
			}
			this.checkMarkerTimer += dt;
			float num = ROCKETRY.CLUSTER_FOW.POINTS_TO_REVEAL / ROCKETRY.CLUSTER_FOW.DEFAULT_CYCLES_PER_REVEAL / 600f;
			float num2 = dt * num;
			this.m_fowManager.EarnRevealPointsForLocation(this.currentTarget, num2);
			return base.OnWorkTick(worker, dt);
		}

		public void OnSetOxygenBreather(OxygenBreather oxygen_breather)
		{
		}

		public void OnClearOxygenBreather(OxygenBreather oxygen_breather)
		{
		}

		public bool ShouldEmitCO2()
		{
			return false;
		}

		public bool ShouldStoreCO2()
		{
			return false;
		}

		public bool ConsumeGas(OxygenBreather oxygen_breather, float amount)
		{
			if (this.storage.items.Count <= 0)
			{
				return false;
			}
			GameObject gameObject = this.storage.items[0];
			if (gameObject == null)
			{
				return false;
			}
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			bool flag = component.Mass >= amount;
			component.Mass = Mathf.Max(0f, component.Mass - amount);
			return flag;
		}

		[MySmiReq]
		private ClusterTelescope.Instance m_telescope;

		private ClusterFogOfWarManager.Instance m_fowManager;

		private GameObject telescopeTargetMarker;

		private AxialI currentTarget;

		private OxygenBreather.IGasProvider workerGasProvider;

		[MyCmpGet]
		private Storage storage;

		private AttributeModifier radiationShielding;

		private float checkMarkerTimer;

		private float checkMarkerFrequency = 1f;
	}

	public class ClusterTelescopeIdentifyMeteorWorkable : Workable, OxygenBreather.IGasProvider
	{
		protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
			this.attributeConverter = Db.Get().AttributeConverters.ResearchSpeed;
			this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.ALL_DAY_EXPERIENCE;
			this.skillExperienceSkillGroup = Db.Get().SkillGroups.Research.Id;
			this.skillExperienceMultiplier = SKILLS.ALL_DAY_EXPERIENCE;
			this.requiredSkillPerk = Db.Get().SkillPerks.CanUseClusterTelescope.Id;
			this.workLayer = Grid.SceneLayer.BuildingUse;
			this.radiationShielding = new AttributeModifier(Db.Get().Attributes.RadiationResistance.Id, FIXEDTRAITS.COSMICRADIATION.TELESCOPE_RADIATION_SHIELDING, global::STRINGS.BUILDINGS.PREFABS.CLUSTERTELESCOPEENCLOSED.NAME, false, false, true);
		}

		protected override void OnCleanUp()
		{
			if (this.telescopeTargetMarker != null)
			{
				Util.KDestroyGameObject(this.telescopeTargetMarker);
			}
			base.OnCleanUp();
		}

		protected override void OnSpawn()
		{
			base.OnSpawn();
			this.OnWorkableEventCB = (Action<Workable, Workable.WorkableEvent>)Delegate.Combine(this.OnWorkableEventCB, new Action<Workable, Workable.WorkableEvent>(this.OnWorkableEvent));
			this.m_fowManager = SaveGame.Instance.GetSMI<ClusterFogOfWarManager.Instance>();
			base.SetWorkTime(float.PositiveInfinity);
			this.overrideAnims = this.m_telescope.workableOverrideAnims;
		}

		private void OnWorkableEvent(Workable workable, Workable.WorkableEvent ev)
		{
			Worker worker = base.worker;
			if (worker == null)
			{
				return;
			}
			KPrefabID component = worker.GetComponent<KPrefabID>();
			OxygenBreather component2 = worker.GetComponent<OxygenBreather>();
			Attributes attributes = worker.GetAttributes();
			if (ev == Workable.WorkableEvent.WorkStarted)
			{
				base.ShowProgressBar(true);
				this.progressBar.SetUpdateFunc(delegate
				{
					if (this.currentTarget == null)
					{
						return 0f;
					}
					return this.currentTarget.IdentifyingProgress;
				});
				this.currentTarget = this.m_telescope.GetMeteorTarget();
				AxialI axialI = this.currentTarget.ClusterGridPosition();
				if (!ClusterGrid.Instance.GetEntityOfLayerAtCell(axialI, EntityLayer.Telescope))
				{
					this.telescopeTargetMarker = GameUtil.KInstantiate(Assets.GetPrefab("TelescopeTarget"), Grid.SceneLayer.Background, null, 0);
					this.telescopeTargetMarker.SetActive(true);
					TelescopeTarget component3 = this.telescopeTargetMarker.GetComponent<TelescopeTarget>();
					component3.Init(axialI);
					component3.SetTargetMeteorShower(this.currentTarget);
				}
				if (this.m_telescope.providesOxygen)
				{
					attributes.Add(this.radiationShielding);
					this.workerGasProvider = component2.GetGasProvider();
					component2.SetGasProvider(this);
					component2.GetComponent<CreatureSimTemperatureTransfer>().enabled = false;
					component.AddTag(GameTags.Shaded, false);
				}
				base.GetComponent<Operational>().SetActive(true, false);
				this.checkMarkerFrequency = global::UnityEngine.Random.Range(2f, 5f);
				return;
			}
			if (ev != Workable.WorkableEvent.WorkStopped)
			{
				return;
			}
			if (this.m_telescope.providesOxygen)
			{
				attributes.Remove(this.radiationShielding);
				component2.SetGasProvider(this.workerGasProvider);
				component2.GetComponent<CreatureSimTemperatureTransfer>().enabled = true;
				component.RemoveTag(GameTags.Shaded);
			}
			base.GetComponent<Operational>().SetActive(false, false);
			if (this.telescopeTargetMarker != null)
			{
				Util.KDestroyGameObject(this.telescopeTargetMarker);
			}
			base.ShowProgressBar(false);
		}

		public override List<Descriptor> GetDescriptors(GameObject go)
		{
			List<Descriptor> descriptors = base.GetDescriptors(go);
			Element element = ElementLoader.FindElementByHash(SimHashes.Oxygen);
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(element.tag.ProperName(), string.Format(global::STRINGS.BUILDINGS.PREFABS.TELESCOPE.REQUIREMENT_TOOLTIP, element.tag.ProperName()), Descriptor.DescriptorType.Requirement);
			descriptors.Add(descriptor);
			return descriptors;
		}

		protected override bool OnWorkTick(Worker worker, float dt)
		{
			ClusterMapMeteorShower.Instance meteorTarget = this.m_telescope.GetMeteorTarget();
			AxialI axialI = meteorTarget.ClusterGridPosition();
			bool flag = false;
			if (meteorTarget != this.currentTarget)
			{
				if (this.telescopeTargetMarker)
				{
					TelescopeTarget component = this.telescopeTargetMarker.GetComponent<TelescopeTarget>();
					component.Init(axialI);
					component.SetTargetMeteorShower(meteorTarget);
				}
				this.currentTarget = meteorTarget;
				flag = true;
			}
			if (!flag && this.checkMarkerTimer > this.checkMarkerFrequency)
			{
				this.checkMarkerTimer = 0f;
				if (!this.telescopeTargetMarker && !ClusterGrid.Instance.GetEntityOfLayerAtCell(axialI, EntityLayer.Telescope))
				{
					this.telescopeTargetMarker = GameUtil.KInstantiate(Assets.GetPrefab("TelescopeTarget"), Grid.SceneLayer.Background, null, 0);
					this.telescopeTargetMarker.SetActive(true);
					this.telescopeTargetMarker.GetComponent<TelescopeTarget>().Init(axialI);
				}
			}
			this.checkMarkerTimer += dt;
			float num = 20f;
			float num2 = dt / num;
			this.currentTarget.ProgressIdentifiction(num2);
			return base.OnWorkTick(worker, dt);
		}

		public void OnSetOxygenBreather(OxygenBreather oxygen_breather)
		{
		}

		public void OnClearOxygenBreather(OxygenBreather oxygen_breather)
		{
		}

		public bool ShouldEmitCO2()
		{
			return false;
		}

		public bool ShouldStoreCO2()
		{
			return false;
		}

		public bool ConsumeGas(OxygenBreather oxygen_breather, float amount)
		{
			if (this.storage.items.Count <= 0)
			{
				return false;
			}
			GameObject gameObject = this.storage.items[0];
			if (gameObject == null)
			{
				return false;
			}
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			bool flag = component.Mass >= amount;
			component.Mass = Mathf.Max(0f, component.Mass - amount);
			return flag;
		}

		[MySmiReq]
		private ClusterTelescope.Instance m_telescope;

		private ClusterFogOfWarManager.Instance m_fowManager;

		private GameObject telescopeTargetMarker;

		private ClusterMapMeteorShower.Instance currentTarget;

		private OxygenBreather.IGasProvider workerGasProvider;

		[MyCmpGet]
		private Storage storage;

		private AttributeModifier radiationShielding;

		private float checkMarkerTimer;

		private float checkMarkerFrequency = 1f;
	}
}
