using System;
using UnityEngine;

public class Geyser : StateMachineComponent<Geyser.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.sm.preType.Set(this.preEmissionElement, base.smi);
		base.smi.sm.emitType.Set(this.emissionElement, base.smi);
		base.smi.sm.postType.Set(this.postEmissionElement, base.smi);
		base.smi.StartSM();
	}

	public void SetEmitter(ElementEmitter emitter)
	{
		this.emitter = emitter;
	}

	[SerializeField]
	private ElementEmitter emitter;

	public float idleDuration;

	public Geyser.EmissionType preEmissionElement;

	public Geyser.EmissionType emissionElement;

	public Geyser.EmissionType postEmissionElement;

	public class StatesInstance : GameStateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.GameInstance
	{
		public StatesInstance(Geyser smi)
			: base(smi)
		{
		}

		public void SetEmissionElement(Geyser.EmissionType type)
		{
			base.master.emitter.outputElement = type.emissionElement;
		}
	}

	public class States : GameStateMachine<Geyser.States, Geyser.StatesInstance, Geyser>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idle;
			base.serializable = true;
			this.root.Enter(delegate(Geyser.StatesInstance smi)
			{
				smi.master.emitter.SetEmitting(false);
			});
			this.idle.PlayAnim("inactive", KAnim.PlayMode.Loop, null).ToggleMainStatusItem(Db.Get().MiscStatusItems.SpoutPressureBuilding).ScheduleGoTo((Geyser.StatesInstance smi) => smi.master.idleDuration, this.pre_erupt);
			this.pre_erupt.InitializeStates(this, this.preType).PlayAnim("shake", KAnim.PlayMode.Loop, null).ScheduleGoTo((Geyser.StatesInstance smi) => smi.master.preEmissionElement.duration, this.erupt);
			this.erupt.InitializeStates(this, this.emitType).PlayAnim("erupt", KAnim.PlayMode.Loop, null).ScheduleGoTo((Geyser.StatesInstance smi) => smi.master.emissionElement.duration, this.post_erupt);
			this.post_erupt.InitializeStates(this, this.postType).PlayAnim("shake", KAnim.PlayMode.Loop, null).ScheduleGoTo((Geyser.StatesInstance smi) => smi.master.postEmissionElement.duration, this.idle);
		}

		public StateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.ObjectParameter<Geyser.EmissionType> preType;

		public StateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.ObjectParameter<Geyser.EmissionType> emitType;

		public StateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.ObjectParameter<Geyser.EmissionType> postType;

		public GameStateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.State idle;

		public Geyser.States.EmitStates pre_erupt;

		public Geyser.States.EmitStates erupt;

		public Geyser.States.EmitStates post_erupt;

		public class EmitStates : GameStateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.State
		{
			public GameStateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.State InitializeStates(Geyser.States parent, StateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.ObjectParameter<Geyser.EmissionType> type)
			{
				base.root.DefaultState(this.over_pressure).Enter(delegate(Geyser.StatesInstance smi)
				{
					smi.SetEmissionElement(type.Get(smi));
					smi.master.emitter.SetEmitting(true);
				}).Exit(delegate(Geyser.StatesInstance smi)
				{
					smi.master.emitter.SetEmitting(false);
				});
				this.emitting.ToggleMainStatusItem(Db.Get().MiscStatusItems.SpoutEmitting).EventTransition(GameHashes.EmitterBlocked, this.over_pressure, null);
				this.over_pressure.ToggleMainStatusItem(Db.Get().MiscStatusItems.SpoutOverPressure).EventTransition(GameHashes.EmitterUnblocked, this.emitting, null);
				return this;
			}

			public GameStateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.State emitting;

			public GameStateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.State over_pressure;
		}
	}

	[Serializable]
	public class EmissionType
	{
		public EmissionType(float duration, ElementConverter.OutputElement emissionElement)
		{
			this.duration = duration;
			this.emissionElement = emissionElement;
		}

		public float duration;

		public ElementConverter.OutputElement emissionElement;
	}
}
