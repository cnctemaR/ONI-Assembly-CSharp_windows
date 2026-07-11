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
		this.accumulatedDisease = SimUtil.DiseaseInfo.Invalid;
		this.smi.SetActive(true);
		base.OnStartWork(worker);
	}

	protected override void OnStopWork(Worker worker)
	{
		this.smi.SetActive(false);
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
		component.Add(this.showerEffect, true);
		HygieneMonitor.Instance instance = worker.GetSMI<HygieneMonitor.Instance>();
		if (instance != null)
		{
			instance.SetDirtiness(0f);
		}
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		PrimaryElement component = worker.GetComponent<PrimaryElement>();
		if (component.DiseaseCount > 0)
		{
			SimUtil.DiseaseInfo diseaseInfo = new SimUtil.DiseaseInfo
			{
				idx = component.DiseaseIdx,
				count = Mathf.CeilToInt((float)component.DiseaseCount * (1f - Mathf.Pow(this.fractionalDiseaseRemoval, dt)) - (float)this.absoluteDiseaseRemoval)
			};
			component.ModifyDiseaseCount(-diseaseInfo.count, "Shower.RemoveDisease");
			this.accumulatedDisease = SimUtil.CalculateFinalDiseaseInfo(this.accumulatedDisease, diseaseInfo);
			Storage component2 = base.GetComponent<Storage>();
			PrimaryElement primaryElement = component2.FindPrimaryElement(this.outputTargetElement);
			if (primaryElement != null)
			{
				PrimaryElement component3 = primaryElement.GetComponent<PrimaryElement>();
				component3.AddDisease(this.accumulatedDisease.idx, this.accumulatedDisease.count, "Shower.RemoveDisease");
				this.accumulatedDisease = SimUtil.DiseaseInfo.Invalid;
			}
		}
		return false;
	}

	protected override void OnAbortWork(Worker worker)
	{
		base.OnAbortWork(worker);
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
		Effect.AddModifierDescriptions(base.gameObject, list, this.showerEffect, true);
		return list;
	}

	private Shower.ShowerSM.Instance smi;

	public string showerEffect = "Showered";

	public SimHashes outputTargetElement;

	public float fractionalDiseaseRemoval;

	public int absoluteDiseaseRemoval;

	private SimUtil.DiseaseInfo accumulatedDisease;

	public const float WATER_PER_USE = 5f;

	private static readonly string[] EffectsRemoved = new string[] { "SoakingWet", "WetFeet" };

	public class ShowerSM : GameStateMachine<Shower.ShowerSM, Shower.ShowerSM.Instance, Shower>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.unoperational;
			this.root.Update(new Action<Shower.ShowerSM.Instance, float>(this.UpdateStatusItems), UpdateRate.SIM_200ms, false);
			this.unoperational.EventTransition(GameHashes.OperationalChanged, this.operational, (Shower.ShowerSM.Instance smi) => smi.IsOperational).PlayAnim("off");
			this.operational.DefaultState(this.operational.not_ready).EventTransition(GameHashes.OperationalChanged, this.unoperational, (Shower.ShowerSM.Instance smi) => !smi.IsOperational);
			this.operational.not_ready.EventTransition(GameHashes.OnStorageChange, this.operational.ready, (Shower.ShowerSM.Instance smi) => smi.IsReady()).PlayAnim("off");
			this.operational.ready.ToggleChore(new Func<Shower.ShowerSM.Instance, Chore>(this.CreateShowerChore), this.operational.not_ready);
		}

		private Chore CreateShowerChore(Shower.ShowerSM.Instance smi)
		{
			ChoreType shower = Db.Get().ChoreTypes.Shower;
			Shower master = smi.master;
			ScheduleBlockType hygiene = Db.Get().ScheduleBlockTypes.Hygiene;
			return new WorkChore<Shower>(shower, master, null, null, true, null, null, null, false, hygiene, false, true, null, false, true, false, PriorityScreen.PriorityClass.emergency, 5, false);
		}

		private void UpdateStatusItems(Shower.ShowerSM.Instance smi, float dt)
		{
			if (smi.OutputFull())
			{
				smi.master.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.OutputPipeFull, this);
			}
			else
			{
				smi.master.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.OutputPipeFull, false);
			}
		}

		public GameStateMachine<Shower.ShowerSM, Shower.ShowerSM.Instance, Shower, object>.State unoperational;

		public Shower.ShowerSM.OperationalState operational;

		public class OperationalState : GameStateMachine<Shower.ShowerSM, Shower.ShowerSM.Instance, Shower, object>.State
		{
			public GameStateMachine<Shower.ShowerSM, Shower.ShowerSM.Instance, Shower, object>.State not_ready;

			public GameStateMachine<Shower.ShowerSM, Shower.ShowerSM.Instance, Shower, object>.State ready;
		}

		public new class Instance : GameStateMachine<Shower.ShowerSM, Shower.ShowerSM.Instance, Shower, object>.GameInstance
		{
			public Instance(Shower master)
				: base(master)
			{
				this.operational = master.GetComponent<Operational>();
				this.consumer = master.GetComponent<ConduitConsumer>();
				this.dispenser = master.GetComponent<ConduitDispenser>();
			}

			public bool IsOperational
			{
				get
				{
					return this.operational.IsOperational && this.consumer.IsConnected && this.dispenser.IsConnected;
				}
			}

			public void SetActive(bool active)
			{
				this.operational.SetActive(active, false);
			}

			private bool HasSufficientMass()
			{
				bool flag = false;
				PrimaryElement primaryElement = base.GetComponent<Storage>().FindPrimaryElement(SimHashes.Water);
				if (primaryElement != null)
				{
					flag = primaryElement.Mass >= 5f;
				}
				return flag;
			}

			public bool OutputFull()
			{
				PrimaryElement primaryElement = base.GetComponent<Storage>().FindPrimaryElement(SimHashes.DirtyWater);
				return primaryElement != null && primaryElement.Mass >= 5f;
			}

			public bool IsReady()
			{
				return this.HasSufficientMass() && !this.OutputFull();
			}

			private Operational operational;

			private ConduitConsumer consumer;

			private ConduitDispenser dispenser;
		}
	}
}
