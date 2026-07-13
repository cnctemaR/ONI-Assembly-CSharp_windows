using System;
using System.Collections.Generic;
using Database;
using Klei;
using Klei.AI;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

public class ClusterTelescope : GameStateMachine<ClusterTelescope, ClusterTelescope.Instance, IStateMachineTarget, ClusterTelescope.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.ready.no_visibility;
		this.root.Update(delegate(ClusterTelescope.Instance smi, float dt)
		{
			KSelectable component = smi.GetComponent<KSelectable>();
			bool flag = Mathf.Approximately(0f, smi.PercentClear) || smi.PercentClear < 0f;
			bool flag2 = Mathf.Approximately(1f, smi.PercentClear) || smi.PercentClear > 1f;
			component.ToggleStatusItem(Db.Get().BuildingStatusItems.SkyVisNone, flag, smi);
			component.ToggleStatusItem(Db.Get().BuildingStatusItems.SkyVisLimited, !flag && !flag2, smi);
		}, UpdateRate.SIM_200ms, false);
		this.ready.DoNothing();
		this.ready.no_visibility.UpdateTransition(this.ready.ready_to_work, (ClusterTelescope.Instance smi, float dt) => smi.HasSkyVisibility(), UpdateRate.SIM_200ms, false);
		this.ready.ready_to_work.UpdateTransition(this.ready.no_visibility, (ClusterTelescope.Instance smi, float dt) => !smi.HasSkyVisibility(), UpdateRate.SIM_200ms, false).DefaultState(this.ready.ready_to_work.decide);
		this.ready.ready_to_work.decide.EnterTransition(this.ready.ready_to_work.identifyMeteorShower, (ClusterTelescope.Instance smi) => smi.ShouldBeWorkingOnMeteorIdentification()).EnterTransition(this.ready.ready_to_work.revealTile, (ClusterTelescope.Instance smi) => smi.ShouldBeWorkingOnRevealingTile()).EnterTransition(this.all_work_complete, (ClusterTelescope.Instance smi) => !smi.IsAnyAvailableWorkToBeDone());
		this.ready.ready_to_work.identifyMeteorShower.OnSignal(this.MeteorIdenificationPriorityChangeSignal, this.ready.ready_to_work.decide, (ClusterTelescope.Instance smi, StateMachine<ClusterTelescope, ClusterTelescope.Instance, IStateMachineTarget, ClusterTelescope.Def>.SignalParameter param) => !smi.ShouldBeWorkingOnMeteorIdentification()).ParamTransition<GameObject>(this.meteorShowerTarget, this.ready.ready_to_work.decide, GameStateMachine<ClusterTelescope, ClusterTelescope.Instance, IStateMachineTarget, ClusterTelescope.Def>.IsNull).EventTransition(GameHashes.ClusterMapMeteorShowerIdentified, (ClusterTelescope.Instance smi) => Game.Instance, this.ready.ready_to_work.decide, (ClusterTelescope.Instance smi) => !smi.ShouldBeWorkingOnMeteorIdentification())
			.EventTransition(GameHashes.ClusterMapMeteorShowerMoved, (ClusterTelescope.Instance smi) => Game.Instance, this.ready.ready_to_work.decide, (ClusterTelescope.Instance smi) => !smi.ShouldBeWorkingOnMeteorIdentification())
			.ToggleChore((ClusterTelescope.Instance smi) => smi.CreateIdentifyMeteorChore(), this.ready.no_visibility);
		this.ready.ready_to_work.revealTile.OnSignal(this.MeteorIdenificationPriorityChangeSignal, this.ready.ready_to_work.decide, (ClusterTelescope.Instance smi, StateMachine<ClusterTelescope, ClusterTelescope.Instance, IStateMachineTarget, ClusterTelescope.Def>.SignalParameter param) => smi.ShouldBeWorkingOnMeteorIdentification()).EventTransition(GameHashes.ClusterFogOfWarRevealed, (ClusterTelescope.Instance smi) => Game.Instance, this.ready.ready_to_work.decide, (ClusterTelescope.Instance smi) => !smi.ShouldBeWorkingOnRevealingTile()).EventTransition(GameHashes.ClusterMapMeteorShowerMoved, (ClusterTelescope.Instance smi) => Game.Instance, this.ready.ready_to_work.decide, (ClusterTelescope.Instance smi) => smi.ShouldBeWorkingOnMeteorIdentification())
			.ToggleChore((ClusterTelescope.Instance smi) => smi.CreateRevealTileChore(), this.ready.no_visibility);
		this.all_work_complete.OnSignal(this.MeteorIdenificationPriorityChangeSignal, this.ready.no_visibility, (ClusterTelescope.Instance smi, StateMachine<ClusterTelescope, ClusterTelescope.Instance, IStateMachineTarget, ClusterTelescope.Def>.SignalParameter param) => smi.IsAnyAvailableWorkToBeDone()).ToggleMainStatusItem(Db.Get().BuildingStatusItems.ClusterTelescopeAllWorkComplete, null).EventTransition(GameHashes.ClusterLocationChanged, (ClusterTelescope.Instance smi) => Game.Instance, this.ready.no_visibility, (ClusterTelescope.Instance smi) => smi.IsAnyAvailableWorkToBeDone())
			.EventTransition(GameHashes.ClusterMapMeteorShowerMoved, (ClusterTelescope.Instance smi) => Game.Instance, this.ready.no_visibility, (ClusterTelescope.Instance smi) => smi.ShouldBeWorkingOnMeteorIdentification());
	}

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

		public SkyVisibilityInfo skyVisibilityInfo;
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

	public new class Instance : GameStateMachine<ClusterTelescope, ClusterTelescope.Instance, IStateMachineTarget, ClusterTelescope.Def>.GameInstance, ICheckboxControl, BuildingStatusItems.ISkyVisInfo
	{
		public float PercentClear
		{
			get
			{
				return this.m_percentClear;
			}
		}

		float BuildingStatusItems.ISkyVisInfo.GetPercentVisible01()
		{
			return this.m_percentClear;
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
			ValueTuple<bool, float> visibilityOf = base.def.skyVisibilityInfo.GetVisibilityOf(base.gameObject);
			bool item = visibilityOf.Item1;
			float item2 = visibilityOf.Item2;
			this.m_percentClear = item2;
			return item;
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
			WorkerBase worker = base.worker;
			if (worker == null)
			{
				return;
			}
			KPrefabID component = worker.GetComponent<KPrefabID>();
			OxygenBreather component2 = worker.GetComponent<OxygenBreather>();
			global::Klei.AI.Attributes attributes = worker.GetAttributes();
			KSelectable component3 = base.GetComponent<KSelectable>();
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
					if (component2 != null)
					{
						component2.AddGasProvider(this);
					}
					worker.GetComponent<CreatureSimTemperatureTransfer>().enabled = false;
					component.AddTag(GameTags.Shaded, false);
				}
				base.GetComponent<Operational>().SetActive(true, false);
				this.checkMarkerFrequency = global::UnityEngine.Random.Range(2f, 5f);
				component3.AddStatusItem(Db.Get().BuildingStatusItems.TelescopeWorking, this);
				return;
			}
			if (ev != Workable.WorkableEvent.WorkStopped)
			{
				return;
			}
			if (this.m_telescope.providesOxygen)
			{
				attributes.Remove(this.radiationShielding);
				if (component2 != null)
				{
					component2.RemoveGasProvider(this);
				}
				worker.GetComponent<CreatureSimTemperatureTransfer>().enabled = true;
				component.RemoveTag(GameTags.Shaded);
			}
			base.GetComponent<Operational>().SetActive(false, false);
			if (this.telescopeTargetMarker != null)
			{
				Util.KDestroyGameObject(this.telescopeTargetMarker);
			}
			base.ShowProgressBar(false);
			component3.RemoveStatusItem(Db.Get().BuildingStatusItems.TelescopeWorking, this);
		}

		public override List<Descriptor> GetDescriptors(GameObject go)
		{
			List<Descriptor> descriptors = base.GetDescriptors(go);
			Element element = ElementLoader.FindElementByHash(SimHashes.Oxygen);
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(element.tag.ProperName(), string.Format(global::STRINGS.BUILDINGS.PREFABS.TELESCOPE.REQUIREMENT_TOOLTIP, element.tag.ProperName()), Descriptor.DescriptorType.Requirement);
			descriptors.Add(descriptor);
			Descriptor descriptor2 = default(Descriptor);
			descriptor2.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.PRODUCESDATABANKS, 3), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.PRODUCESDATABANKS, 3), Descriptor.DescriptorType.Effect);
			descriptors.Add(descriptor2);
			return descriptors;
		}

		public override float GetEfficiencyMultiplier(WorkerBase worker)
		{
			return base.GetEfficiencyMultiplier(worker) * Mathf.Clamp01(this.m_telescope.PercentClear);
		}

		protected override bool OnWorkTick(WorkerBase worker, float dt)
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
			if (this.m_fowManager.EarnRevealPointsForLocation(this.currentTarget, num2))
			{
				for (int i = 0; i < 3; i++)
				{
					Util.KInstantiate(Assets.GetPrefab(DatabankHelper.ID), base.transform.position + Vector3.up * 2f + Vector3.right, Quaternion.identity, null, null, true, 23).SetActive(true);
				}
				PopFXManager.Instance.SpawnFX(global::Def.GetUISprite(DatabankHelper.ID, "ui", false).first, DatabankHelper.NAME_PLURAL, base.transform, Vector3.up * 2f, 1.5f, true, false);
			}
			return base.OnWorkTick(worker, dt);
		}

		protected override void OnCompleteWork(WorkerBase worker)
		{
			base.OnCompleteWork(worker);
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
			float mass = gameObject.GetComponent<PrimaryElement>().Mass;
			float num = 0f;
			float num2 = 0f;
			SimHashes simHashes = SimHashes.Vacuum;
			SimUtil.DiseaseInfo diseaseInfo;
			this.storage.ConsumeAndGetDisease(GameTags.Breathable, amount, out num, out diseaseInfo, out num2, out simHashes);
			bool flag = num >= amount;
			OxygenBreather.BreathableGasConsumed(oxygen_breather, simHashes, num, num2, diseaseInfo.idx, diseaseInfo.count);
			return flag;
		}

		public bool IsLowOxygen()
		{
			if (this.storage.items.Count <= 0)
			{
				return true;
			}
			PrimaryElement primaryElement = this.storage.FindFirstWithMass(GameTags.Breathable, 0f);
			return primaryElement == null || primaryElement.Mass == 0f;
		}

		public bool HasOxygen()
		{
			if (this.storage.items.Count <= 0)
			{
				return false;
			}
			PrimaryElement primaryElement = this.storage.FindFirstWithMass(GameTags.Breathable, 0f);
			return primaryElement != null && primaryElement.Mass > 0f;
		}

		public bool IsBlocked()
		{
			return false;
		}

		[MySmiReq]
		private ClusterTelescope.Instance m_telescope;

		private ClusterFogOfWarManager.Instance m_fowManager;

		private GameObject telescopeTargetMarker;

		private AxialI currentTarget;

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
			WorkerBase worker = base.worker;
			if (worker == null)
			{
				return;
			}
			KPrefabID component = worker.GetComponent<KPrefabID>();
			OxygenBreather component2 = worker.GetComponent<OxygenBreather>();
			global::Klei.AI.Attributes attributes = worker.GetAttributes();
			KSelectable component3 = base.GetComponent<KSelectable>();
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
					TelescopeTarget component4 = this.telescopeTargetMarker.GetComponent<TelescopeTarget>();
					component4.Init(axialI);
					component4.SetTargetMeteorShower(this.currentTarget);
				}
				if (this.m_telescope.providesOxygen)
				{
					attributes.Add(this.radiationShielding);
					component2.AddGasProvider(this);
					component2.GetComponent<CreatureSimTemperatureTransfer>().enabled = false;
					component.AddTag(GameTags.Shaded, false);
				}
				base.GetComponent<Operational>().SetActive(true, false);
				this.checkMarkerFrequency = global::UnityEngine.Random.Range(2f, 5f);
				component3.AddStatusItem(Db.Get().BuildingStatusItems.ClusterTelescopeMeteorWorking, this);
				return;
			}
			if (ev != Workable.WorkableEvent.WorkStopped)
			{
				return;
			}
			if (this.m_telescope.providesOxygen)
			{
				attributes.Remove(this.radiationShielding);
				component2.RemoveGasProvider(this);
				component2.GetComponent<CreatureSimTemperatureTransfer>().enabled = true;
				component.RemoveTag(GameTags.Shaded);
			}
			base.GetComponent<Operational>().SetActive(false, false);
			if (this.telescopeTargetMarker != null)
			{
				Util.KDestroyGameObject(this.telescopeTargetMarker);
			}
			base.ShowProgressBar(false);
			component3.RemoveStatusItem(Db.Get().BuildingStatusItems.ClusterTelescopeMeteorWorking, this);
		}

		protected override bool OnWorkTick(WorkerBase worker, float dt)
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
			float mass = gameObject.GetComponent<PrimaryElement>().Mass;
			float num = 0f;
			float num2 = 0f;
			SimHashes simHashes = SimHashes.Vacuum;
			SimUtil.DiseaseInfo diseaseInfo;
			this.storage.ConsumeAndGetDisease(GameTags.Breathable, amount, out num, out diseaseInfo, out num2, out simHashes);
			bool flag = num >= amount;
			OxygenBreather.BreathableGasConsumed(oxygen_breather, simHashes, num, num2, diseaseInfo.idx, diseaseInfo.count);
			return flag;
		}

		public bool IsLowOxygen()
		{
			if (this.storage.items.Count <= 0)
			{
				return true;
			}
			GameObject gameObject = this.storage.items[0];
			return !(gameObject == null) && gameObject.GetComponent<PrimaryElement>().Mass > 0f;
		}

		public bool HasOxygen()
		{
			if (this.storage.items.Count <= 0)
			{
				return false;
			}
			GameObject gameObject = this.storage.items[0];
			return !(gameObject == null) && gameObject.GetComponent<PrimaryElement>().Mass > 0f;
		}

		public bool IsBlocked()
		{
			return false;
		}

		[MySmiReq]
		private ClusterTelescope.Instance m_telescope;

		private ClusterFogOfWarManager.Instance m_fowManager;

		private GameObject telescopeTargetMarker;

		private ClusterMapMeteorShower.Instance currentTarget;

		[MyCmpGet]
		private Storage storage;

		private AttributeModifier radiationShielding;

		private float checkMarkerTimer;

		private float checkMarkerFrequency = 1f;
	}
}
