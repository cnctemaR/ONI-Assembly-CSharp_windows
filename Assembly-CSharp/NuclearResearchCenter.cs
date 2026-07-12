using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class NuclearResearchCenter : StateMachineComponent<NuclearResearchCenter.StatesInstance>, IResearchCenter, IGameObjectEffectDescriptor
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.ResearchCenters.Add(this);
		this.particleMeter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", this.particleMeterOffset, Grid.SceneLayer.NoLayer, new string[] { "meter_target" });
		base.Subscribe<NuclearResearchCenter>(-1837862626, NuclearResearchCenter.OnStorageChangeDelegate);
		this.RefreshMeter();
		base.smi.StartSM();
		Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Radiation, true);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.ResearchCenters.Remove(this);
	}

	public string GetResearchType()
	{
		return this.researchTypeID;
	}

	private void OnStorageChange(object data)
	{
		this.RefreshMeter();
	}

	private void RefreshMeter()
	{
		float num = Mathf.Clamp01(this.particleStorage.Particles / this.particleStorage.Capacity());
		this.particleMeter.SetPositionPercent(num);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return new List<Descriptor>
		{
			new Descriptor(string.Format(UI.BUILDINGEFFECTS.RESEARCH_MATERIALS, this.inputMaterial.ProperName(), GameUtil.GetFormattedByTag(this.inputMaterial, this.materialPerPoint, GameUtil.TimeSlice.None)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.RESEARCH_MATERIALS, this.inputMaterial.ProperName(), GameUtil.GetFormattedByTag(this.inputMaterial, this.materialPerPoint, GameUtil.TimeSlice.None)), Descriptor.DescriptorType.Requirement, false),
			new Descriptor(string.Format(UI.BUILDINGEFFECTS.PRODUCES_RESEARCH_POINTS, Research.Instance.researchTypes.GetResearchType(this.researchTypeID).name), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.PRODUCES_RESEARCH_POINTS, Research.Instance.researchTypes.GetResearchType(this.researchTypeID).name), Descriptor.DescriptorType.Effect, false)
		};
	}

	[MyCmpGet]
	private Operational operational;

	public string researchTypeID;

	public float materialPerPoint = 50f;

	public float timePerPoint;

	public Tag inputMaterial;

	[MyCmpReq]
	private HighEnergyParticleStorage particleStorage;

	public Meter.Offset particleMeterOffset;

	private MeterController particleMeter;

	private static readonly EventSystem.IntraObjectHandler<NuclearResearchCenter> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<NuclearResearchCenter>(delegate(NuclearResearchCenter component, object data)
	{
		component.OnStorageChange(data);
	});

	public class States : GameStateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.inoperational;
			this.inoperational.PlayAnim("off").TagTransition(GameTags.Operational, this.requirements, false);
			this.requirements.PlayAnim("on").TagTransition(GameTags.Operational, this.inoperational, true).DefaultState(this.requirements.highEnergyParticlesNeeded);
			this.requirements.highEnergyParticlesNeeded.ToggleMainStatusItem(Db.Get().BuildingStatusItems.WaitingForHighEnergyParticles, null).EventTransition(GameHashes.OnParticleStorageChanged, this.requirements.noResearchSelected, new StateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.Transition.ConditionCallback(this.IsReady));
			this.requirements.noResearchSelected.ToggleMainStatusItem(Db.Get().BuildingStatusItems.NoResearchSelected, null).EventTransition(GameHashes.ActiveResearchChanged, this.requirements.noApplicableResearch, new StateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.Transition.ConditionCallback(this.IsResearchSelected));
			this.requirements.noApplicableResearch.EventTransition(GameHashes.ActiveResearchChanged, this.ready, new StateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.Transition.ConditionCallback(this.IsResearchApplicable)).EventTransition(GameHashes.ActiveResearchChanged, this.requirements, GameStateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.Not(new StateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.Transition.ConditionCallback(this.IsResearchSelected)));
			this.ready.Enter(delegate(NuclearResearchCenter.StatesInstance smi)
			{
				smi.CreateChore();
			}).TagTransition(GameTags.Operational, this.inoperational, true).DefaultState(this.ready.idle)
				.Exit(delegate(NuclearResearchCenter.StatesInstance smi)
				{
					smi.DestroyChore();
				});
			this.ready.idle.WorkableStartTransition((NuclearResearchCenter.StatesInstance smi) => smi.master.GetComponent<NuclearResearchCenterWorkable>(), this.ready.working).EventTransition(GameHashes.ActiveResearchChanged, this.requirements.noResearchSelected, GameStateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.Not(new StateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.Transition.ConditionCallback(this.IsResearchSelected))).EventTransition(GameHashes.ActiveResearchChanged, this.requirements.noApplicableResearch, GameStateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.Not(new StateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.Transition.ConditionCallback(this.IsResearchApplicable)))
				.EventTransition(GameHashes.ResearchPointsChanged, this.requirements.noApplicableResearch, GameStateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.Not(new StateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.Transition.ConditionCallback(this.IsResearchApplicable)))
				.EventTransition(GameHashes.OnParticleStorageEmpty, this.requirements.highEnergyParticlesNeeded, GameStateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.Not(new StateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.Transition.ConditionCallback(this.HasRadiation)));
			this.ready.working.Enter("SetActive(true)", delegate(NuclearResearchCenter.StatesInstance smi)
			{
				smi.master.operational.SetActive(true, false);
			}).Exit("SetActive(false)", delegate(NuclearResearchCenter.StatesInstance smi)
			{
				smi.master.operational.SetActive(false, false);
			}).WorkableStopTransition((NuclearResearchCenter.StatesInstance smi) => smi.master.GetComponent<NuclearResearchCenterWorkable>(), this.ready.idle)
				.WorkableCompleteTransition((NuclearResearchCenter.StatesInstance smi) => smi.master.GetComponent<NuclearResearchCenterWorkable>(), this.ready.idle);
		}

		private bool IsReady(NuclearResearchCenter.StatesInstance smi)
		{
			return smi.GetComponent<HighEnergyParticleStorage>().Particles > smi.master.materialPerPoint;
		}

		private bool IsResearchSelected(NuclearResearchCenter.StatesInstance smi)
		{
			return Research.Instance.GetActiveResearch() != null;
		}

		private bool IsResearchApplicable(NuclearResearchCenter.StatesInstance smi)
		{
			TechInstance activeResearch = Research.Instance.GetActiveResearch();
			if (activeResearch != null && activeResearch.tech.costsByResearchTypeID.ContainsKey(smi.master.researchTypeID))
			{
				float num = activeResearch.progressInventory.PointsByTypeID[smi.master.researchTypeID];
				float num2 = activeResearch.tech.costsByResearchTypeID[smi.master.researchTypeID];
				return num < num2;
			}
			return false;
		}

		private bool HasRadiation(NuclearResearchCenter.StatesInstance smi)
		{
			return !smi.GetComponent<HighEnergyParticleStorage>().IsEmpty();
		}

		public GameStateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.State inoperational;

		public NuclearResearchCenter.States.RequirementsState requirements;

		public NuclearResearchCenter.States.ReadyState ready;

		public class RequirementsState : GameStateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.State
		{
			public GameStateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.State highEnergyParticlesNeeded;

			public GameStateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.State noResearchSelected;

			public GameStateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.State noApplicableResearch;
		}

		public class ReadyState : GameStateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.State
		{
			public GameStateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.State idle;

			public GameStateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.State working;
		}
	}

	public class StatesInstance : GameStateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.GameInstance
	{
		public StatesInstance(NuclearResearchCenter master)
			: base(master)
		{
		}

		public void CreateChore()
		{
			Workable component = base.smi.master.GetComponent<NuclearResearchCenterWorkable>();
			this.chore = new WorkChore<NuclearResearchCenterWorkable>(Db.Get().ChoreTypes.Research, component, null, true, null, null, null, true, null, false, true, null, true, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
			this.chore.preemption_cb = new Func<Chore.Precondition.Context, bool>(NuclearResearchCenter.StatesInstance.CanPreemptCB);
		}

		public void DestroyChore()
		{
			this.chore.Cancel("destroy me!");
			this.chore = null;
		}

		private static bool CanPreemptCB(Chore.Precondition.Context context)
		{
			Worker component = context.chore.driver.GetComponent<Worker>();
			float num = Db.Get().AttributeConverters.ResearchSpeed.Lookup(component).Evaluate();
			Worker worker = context.consumerState.worker;
			float num2 = Db.Get().AttributeConverters.ResearchSpeed.Lookup(worker).Evaluate();
			TechInstance activeResearch = Research.Instance.GetActiveResearch();
			if (activeResearch != null)
			{
				NuclearResearchCenter.StatesInstance smi = context.chore.gameObject.GetSMI<NuclearResearchCenter.StatesInstance>();
				if (smi != null)
				{
					return num2 > num && activeResearch.PercentageCompleteResearchType(smi.master.researchTypeID) < 1f;
				}
			}
			return false;
		}

		private WorkChore<NuclearResearchCenterWorkable> chore;
	}
}
