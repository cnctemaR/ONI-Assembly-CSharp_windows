using System;
using System.Collections.Generic;
using Klei;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class Shower : Workable, IEffectDescriptor, IGameObjectEffectDescriptor
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.resetProgressOnStop = true;
		this.smi = new Shower.ShowerSM.Instance(this);
		this.smi.StartSM();
	}

	protected override void OnStartWork(Worker worker)
	{
		HygieneMonitor.Instance instance = worker.GetSMI<HygieneMonitor.Instance>();
		base.WorkTimeRemaining = this.workTime * instance.GetDirtiness();
		base.OnStartWork(worker);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		Effects component = worker.GetComponent<Effects>();
		for (int i = 0; i < Shower.EffectsRemoved.Length; i++)
		{
			string text = Shower.EffectsRemoved[i];
			component.Remove(text);
		}
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		HygieneMonitor.Instance instance = worker.GetSMI<HygieneMonitor.Instance>();
		if (instance != null)
		{
			instance.SetDirtiness(1f - this.GetPercentComplete());
		}
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (Shower.EffectsRemoved.Length > 0)
		{
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(UI.BUILDINGEFFECTS.REMOVESEFFECTSUBTITLE, UI.BUILDINGEFFECTS.TOOLTIPS.REMOVESEFFECTSUBTITLE, Descriptor.DescriptorType.Effect);
			list.Add(descriptor);
			for (int i = 0; i < Shower.EffectsRemoved.Length; i++)
			{
				string text = Shower.EffectsRemoved[i];
				string text2 = Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + text.ToUpper() + ".NAME");
				string text3 = Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + text.ToUpper() + ".CAUSE");
				Descriptor descriptor2 = default(Descriptor);
				descriptor2.IncreaseIndent();
				descriptor2.SetupDescriptor("• " + string.Format(UI.BUILDINGEFFECTS.REMOVEDEFFECT, text2), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.REMOVEDEFFECT, text3), Descriptor.DescriptorType.Effect);
				list.Add(descriptor2);
			}
		}
		return list;
	}

	private Shower.ShowerSM.Instance smi;

	public SimHashes outputTargetElement;

	public float fractionalDiseaseRemoval;

	public int absoluteDiseaseRemoval;

	private static readonly string[] EffectsRemoved = new string[] { "Unclean", "SoakingWet", "WetFeet" };

	public class ShowerSM : GameStateMachine<Shower.ShowerSM, Shower.ShowerSM.Instance, Shower>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.unoperational;
			this.unoperational.EventTransition(GameHashes.OperationalChanged, this.operational, (Shower.ShowerSM.Instance smi) => smi.IsOperational).PlayAnim("off");
			this.operational.DefaultState(this.operational.idle).EventTransition(GameHashes.OperationalChanged, this.unoperational, (Shower.ShowerSM.Instance smi) => !smi.IsOperational).ToggleChore((Shower.ShowerSM.Instance smi) => new WorkChore<Shower>(Db.Get().ChoreTypes.Shower, smi.master, null, null, true, null, null, null, false, null, true, null, false, true, false, PriorityScreen.PriorityClass.emergency, 0, false), this.unoperational);
			this.operational.idle.WorkableStartTransition((Shower.ShowerSM.Instance smi) => smi.master, this.operational.showering);
			this.operational.showering.WorkableStopTransition((Shower.ShowerSM.Instance smi) => smi.master, this.operational.exiting).Enter(delegate(Shower.ShowerSM.Instance smi)
			{
				smi.master.GetComponent<Operational>().SetActive(true, false);
			}).Update(delegate(Shower.ShowerSM.Instance smi, float dt)
			{
				smi.RemoveDisease(dt);
			}, UpdateRate.SIM_200ms, false)
				.PlayAnims((Shower.ShowerSM.Instance smi) => Shower.ShowerSM.workingAnims, KAnim.PlayMode.Loop)
				.Exit(delegate(Shower.ShowerSM.Instance smi)
				{
					smi.master.GetComponent<Operational>().SetActive(false, false);
				});
			this.operational.exiting.PlayAnim("working_pst").OnAnimQueueComplete(this.unoperational);
		}

		public GameStateMachine<Shower.ShowerSM, Shower.ShowerSM.Instance, Shower, object>.State unoperational;

		public Shower.ShowerSM.OperationalStates operational;

		private static readonly HashedString[] workingAnims = new HashedString[] { "working_pre", "working_loop" };

		public class OperationalStates : GameStateMachine<Shower.ShowerSM, Shower.ShowerSM.Instance, Shower, object>.State
		{
			public GameStateMachine<Shower.ShowerSM, Shower.ShowerSM.Instance, Shower, object>.State idle;

			public GameStateMachine<Shower.ShowerSM, Shower.ShowerSM.Instance, Shower, object>.State showering;

			public GameStateMachine<Shower.ShowerSM, Shower.ShowerSM.Instance, Shower, object>.State exiting;
		}

		public new class Instance : GameStateMachine<Shower.ShowerSM, Shower.ShowerSM.Instance, Shower, object>.GameInstance
		{
			public Instance(Shower master)
				: base(master)
			{
				this.operational = master.GetComponent<Operational>();
				this.consumer = master.GetComponent<ConduitConsumer>();
				this.dispenser = master.GetComponent<ConduitDispenser>();
				this.accumulatedDisease = SimUtil.DiseaseInfo.Invalid;
			}

			public bool IsOperational
			{
				get
				{
					return this.operational.IsOperational && this.consumer.IsConnected && this.dispenser.IsConnected;
				}
			}

			public void RemoveDisease(float dt)
			{
				PrimaryElement component = base.master.worker.GetComponent<PrimaryElement>();
				if (component.DiseaseCount > 0)
				{
					SimUtil.DiseaseInfo diseaseInfo = new SimUtil.DiseaseInfo
					{
						idx = component.DiseaseIdx,
						count = Mathf.CeilToInt((float)component.DiseaseCount * (1f - Mathf.Pow(base.master.fractionalDiseaseRemoval, dt)) - (float)base.master.absoluteDiseaseRemoval)
					};
					component.ModifyDiseaseCount(-diseaseInfo.count, "Shower.RemoveDisease");
					this.accumulatedDisease = SimUtil.CalculateFinalDiseaseInfo(this.accumulatedDisease, diseaseInfo);
					Storage component2 = base.master.GetComponent<Storage>();
					PrimaryElement primaryElement = component2.FindPrimaryElement(base.master.outputTargetElement);
					if (primaryElement != null)
					{
						PrimaryElement component3 = primaryElement.GetComponent<PrimaryElement>();
						component3.AddDisease(this.accumulatedDisease.idx, this.accumulatedDisease.count, "Shower.RemoveDisease");
						this.accumulatedDisease = SimUtil.DiseaseInfo.Invalid;
					}
				}
			}

			private Operational operational;

			private ConduitConsumer consumer;

			private ConduitDispenser dispenser;

			private SimUtil.DiseaseInfo accumulatedDisease;
		}
	}
}
