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
		default_state = this.ready.no_visibility;
		this.ready.EventTransition(GameHashes.ClusterFogOfWarRevealed, (ClusterTelescope.Instance smi) => Game.Instance, this.all_work_complete, (ClusterTelescope.Instance smi) => !smi.CheckHasAnalyzeTarget());
		this.ready.no_visibility.UpdateTransition(this.ready.ready_to_work, (ClusterTelescope.Instance smi, float dt) => smi.HasSkyVisibility(), UpdateRate.SIM_200ms, false).ToggleStatusItem(ClusterTelescope.noVisibilityStatusItem, null);
		this.ready.ready_to_work.UpdateTransition(this.ready.no_visibility, (ClusterTelescope.Instance smi, float dt) => !smi.HasSkyVisibility(), UpdateRate.SIM_200ms, false).ToggleChore((ClusterTelescope.Instance smi) => smi.CreateChore(), this.ready.no_visibility);
		this.all_work_complete.ToggleMainStatusItem(Db.Get().BuildingStatusItems.ClusterTelescopeAllWorkComplete, null).EventTransition(GameHashes.ClusterLocationChanged, (ClusterTelescope.Instance smi) => Game.Instance, this.ready.no_visibility, (ClusterTelescope.Instance smi) => smi.CheckHasAnalyzeTarget());
	}

	private static StatusItem noVisibilityStatusItem = new StatusItem("SPACE_VISIBILITY_NONE", "BUILDING", "status_item_no_sky", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022, new Func<string, object, string>(ClusterTelescope.GetStatusItemString));

	public GameStateMachine<ClusterTelescope, ClusterTelescope.Instance, IStateMachineTarget, ClusterTelescope.Def>.State all_work_complete;

	public ClusterTelescope.ReadyStates ready;

	public class Def : StateMachine.BaseDef
	{
		public int clearScanCellRadius = 15;

		public int analyzeClusterRadius = 3;

		public KAnimFile[] workableOverrideAnims;

		public bool providesOxygen;
	}

	public class ReadyStates : GameStateMachine<ClusterTelescope, ClusterTelescope.Instance, IStateMachineTarget, ClusterTelescope.Def>.State
	{
		public GameStateMachine<ClusterTelescope, ClusterTelescope.Instance, IStateMachineTarget, ClusterTelescope.Def>.State no_visibility;

		public GameStateMachine<ClusterTelescope, ClusterTelescope.Instance, IStateMachineTarget, ClusterTelescope.Def>.State ready_to_work;
	}

	public new class Instance : GameStateMachine<ClusterTelescope, ClusterTelescope.Instance, IStateMachineTarget, ClusterTelescope.Def>.GameInstance
	{
		public float PercentClear
		{
			get
			{
				return this.m_percentClear;
			}
		}

		public Instance(IStateMachineTarget smi, ClusterTelescope.Def def)
			: base(smi, def)
		{
			this.workableOverrideAnims = def.workableOverrideAnims;
			this.providesOxygen = def.providesOxygen;
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

		public Chore CreateChore()
		{
			WorkChore<ClusterTelescope.ClusterTelescopeWorkable> workChore = new WorkChore<ClusterTelescope.ClusterTelescopeWorkable>(Db.Get().ChoreTypes.Research, this.m_workable, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
			if (this.providesOxygen)
			{
				workChore.AddPrecondition(Telescope.ContainsOxygen, null);
			}
			return workChore;
		}

		public AxialI GetAnalyzeTarget()
		{
			global::Debug.Assert(this.m_hasAnalyzeTarget, "GetAnalyzeTarget called but this telescope has no target assigned.");
			return this.m_analyzeTarget;
		}

		public bool HasSkyVisibility()
		{
			Extents extents = base.GetComponent<Building>().GetExtents();
			int num = Mathf.Max(0, extents.x - base.def.clearScanCellRadius);
			int num2 = Mathf.Min(new int[] { extents.x + base.def.clearScanCellRadius });
			int num3 = extents.y + extents.height - 3;
			int num4 = num2 - num + 1;
			int num5 = Grid.XYToCell(num, num3);
			int num6 = Grid.XYToCell(num2, num3);
			int num7 = 0;
			for (int i = num5; i <= num6; i++)
			{
				if (Grid.ExposedToSunlight[i] >= 253)
				{
					num7++;
				}
			}
			this.m_percentClear = (float)num7 / (float)num4;
			return this.m_percentClear > 0f;
		}

		private float m_percentClear;

		[Serialize]
		private bool m_hasAnalyzeTarget;

		[Serialize]
		private AxialI m_analyzeTarget;

		[MyCmpAdd]
		private ClusterTelescope.ClusterTelescopeWorkable m_workable;

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
			this.OnWorkableEventCB = (Action<Workable.WorkableEvent>)Delegate.Combine(this.OnWorkableEventCB, new Action<Workable.WorkableEvent>(this.OnWorkableEvent));
			this.m_fowManager = SaveGame.Instance.GetSMI<ClusterFogOfWarManager.Instance>();
			base.SetWorkTime(float.PositiveInfinity);
			this.overrideAnims = this.m_telescope.workableOverrideAnims;
		}

		private void OnWorkableEvent(Workable.WorkableEvent ev)
		{
			Worker worker = base.worker;
			if (worker == null)
			{
				return;
			}
			KPrefabID component = worker.GetComponent<KPrefabID>();
			OxygenBreather component2 = worker.GetComponent<OxygenBreather>();
			Attributes attributes = worker.GetAttributes();
			if (ev != Workable.WorkableEvent.WorkStarted)
			{
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
				Util.KDestroyGameObject(this.telescopeTargetMarker);
				base.ShowProgressBar(false);
			}
			else
			{
				base.ShowProgressBar(true);
				this.telescopeTargetMarker = GameUtil.KInstantiate(Assets.GetPrefab("TelescopeTarget"), Grid.SceneLayer.Background, null, 0);
				this.telescopeTargetMarker.SetActive(true);
				this.progressBar.SetUpdateFunc(() => this.m_fowManager.GetRevealCompleteFraction(this.currentTarget));
				this.currentTarget = this.m_telescope.GetAnalyzeTarget();
				this.telescopeTargetMarker.GetComponent<TelescopeTarget>().Init(this.currentTarget);
				if (this.m_telescope.providesOxygen)
				{
					attributes.Add(this.radiationShielding);
					this.workerGasProvider = component2.GetGasProvider();
					component2.SetGasProvider(this);
					component2.GetComponent<CreatureSimTemperatureTransfer>().enabled = false;
					component.AddTag(GameTags.Shaded, false);
					return;
				}
			}
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
			if (analyzeTarget != this.currentTarget)
			{
				this.telescopeTargetMarker.GetComponent<TelescopeTarget>().Init(analyzeTarget);
				this.currentTarget = analyzeTarget;
			}
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
	}
}
