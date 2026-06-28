using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;

public class Shower : BuildingWorkable, IEffectDescriptor
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.smi = new Shower.ShowerSM.Instance(this);
		this.smi.StartSM();
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

	public int DescriptionOrder { get; set; }

	public List<Descriptor> GetRequirementDescriptions(BuildingDef def)
	{
		return null;
	}

	public List<Descriptor> GetEffectDescriptions(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (Shower.EffectsRemoved.Length > 0)
		{
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(string.Format(UI.LISTENTRYSTRINGNOLINEBREAK, UI.BUILDINGEFFECTS.REMOVESEFFECTSUBTITLE), UI.BUILDINGEFFECTS.TOOLTIPS.FABRICATES);
			list.Add(descriptor);
			for (int i = 0; i < Shower.EffectsRemoved.Length; i++)
			{
				string text = Shower.EffectsRemoved[i];
				string text2 = Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + text.ToUpper() + ".NAME");
				string text3 = Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + text.ToUpper() + ".CAUSE");
				Descriptor descriptor2 = default(Descriptor);
				string text4 = UI.LISTENTRYTAB + UI.LISTENTRYTAB + "• ";
				descriptor2.SetupDescriptor(text4 + string.Format(string.Format(UI.LISTENTRYSTRINGNOLINEBREAK, UI.BUILDINGEFFECTS.REMOVEDEFFECT), text2), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.REMOVEDEFFECT, text3));
				list.Add(descriptor2);
			}
		}
		return list;
	}

	private Shower.ShowerSM.Instance smi;

	private static readonly string[] EffectsRemoved = new string[] { "DirtyHands", "Unclean" };

	public class ShowerSM : GameStateMachine<Shower.ShowerSM, Shower.ShowerSM.Instance>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.unoperational;
			this.unoperational.EventTransition(GameHashes.OperationalChanged, this.operational, (Shower.ShowerSM.Instance smi) => smi.IsOperational).PlayAnim("off", KAnim.PlayMode.Once, null);
			this.operational.DefaultState(this.operational.idle).EventTransition(GameHashes.OperationalChanged, this.unoperational, (Shower.ShowerSM.Instance smi) => !smi.IsOperational).ToggleChore((Shower.ShowerSM.Instance smi) => new WorkChore<Shower>(Db.Get().ChoreTypes.Shower, smi.master, null, true, null, null, null, false, null, true, default(Tag), null, false, true), this.unoperational, false);
			this.operational.idle.EventTransition(GameHashes.WorkStarted, this.operational.showering, null);
			this.operational.showering.EventTransition(GameHashes.WorkStopped, this.operational.exiting, null).Enter(delegate(Shower.ShowerSM.Instance smi)
			{
				smi.master.GetComponent<Operational>().SetActive(true, false);
			}).PlayAnims((Shower.ShowerSM.Instance smi) => new string[] { "working_pre", "working_loop" }, KAnim.PlayMode.Loop)
				.Exit(delegate(Shower.ShowerSM.Instance smi)
				{
					smi.master.GetComponent<Operational>().SetActive(false, false);
				});
			this.operational.exiting.PlayAnim("working_pst", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.unoperational);
		}

		public GameStateMachine<Shower.ShowerSM, Shower.ShowerSM.Instance, IStateMachineTarget>.State unoperational;

		public Shower.ShowerSM.OperationalStates operational;

		public class OperationalStates : GameStateMachine<Shower.ShowerSM, Shower.ShowerSM.Instance, IStateMachineTarget>.State
		{
			public GameStateMachine<Shower.ShowerSM, Shower.ShowerSM.Instance, IStateMachineTarget>.State idle;

			public GameStateMachine<Shower.ShowerSM, Shower.ShowerSM.Instance, IStateMachineTarget>.State showering;

			public GameStateMachine<Shower.ShowerSM, Shower.ShowerSM.Instance, IStateMachineTarget>.State exiting;
		}

		public new class Instance : GameStateMachine<Shower.ShowerSM, Shower.ShowerSM.Instance, IStateMachineTarget>.GameInstance
		{
			public Instance(IStateMachineTarget master)
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

			private Operational operational;

			private ConduitConsumer consumer;

			private ConduitDispenser dispenser;
		}
	}
}
