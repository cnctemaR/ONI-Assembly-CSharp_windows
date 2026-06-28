using System;
using UnityEngine;

public class Geyser : StateMachineComponent<Geyser.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	public void SetEmitter(ElementEmitter emitter)
	{
		this.emitter = emitter;
	}

	public void cycleType()
	{
		this.current_emissionType = ((this.current_emissionType != this.emission_a) ? ((this.emission_a == null) ? this.emission_b : this.emission_a) : ((this.emission_b == null) ? this.emission_a : this.emission_b));
	}

	[SerializeField]
	private ElementEmitter emitter;

	public float idleDuration;

	public Geyser.EmissionType emission_a;

	public Geyser.EmissionType emission_b;

	public Geyser.EmissionType current_emissionType;

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
				smi.master.current_emissionType = smi.master.emission_a;
				smi.SetEmissionElement(smi.master.current_emissionType);
			});
			this.idle.PlayAnim("inactive", KAnim.PlayMode.Loop).ScheduleGoTo((Geyser.StatesInstance smi) => smi.master.idleDuration, this.pre_erupt);
			this.pre_erupt.PlayAnim("shake", KAnim.PlayMode.Loop).ToggleMainStatusItem(Db.Get().MiscStatusItems.SpoutPressureBuilding).ScheduleGoTo((Geyser.StatesInstance smi) => smi.master.current_emissionType.duration_pre, this.erupt)
				.Enter("SetEmissionElement", delegate(Geyser.StatesInstance smi)
				{
					smi.SetEmissionElement(smi.master.current_emissionType);
				});
			this.erupt.DefaultState(this.erupt.erupting).ScheduleGoTo((Geyser.StatesInstance smi) => smi.master.current_emissionType.duration_erupt, this.post_erupt).Enter(delegate(Geyser.StatesInstance smi)
			{
				smi.master.emitter.SetEmitting(true);
			})
				.Exit(delegate(Geyser.StatesInstance smi)
				{
					smi.master.cycleType();
					smi.master.emitter.SetEmitting(false);
				});
			this.erupt.erupting.EventTransition(GameHashes.EmitterBlocked, this.erupt.overpressure, (Geyser.StatesInstance smi) => smi.GetComponent<ElementEmitter>().isEmitterBlocked).Enter(delegate(Geyser.StatesInstance smi)
			{
				smi.master.GetComponent<KBatchedAnimController>().Play(smi.master.current_emissionType.animation, KAnim.PlayMode.Loop, 1f, 0f);
			}).EventTransition(GameHashes.EmitterBlocked, this.erupt.overpressure, null);
			this.erupt.overpressure.EventTransition(GameHashes.EmitterUnblocked, this.erupt.erupting, (Geyser.StatesInstance smi) => !smi.GetComponent<ElementEmitter>().isEmitterBlocked).ToggleMainStatusItem(Db.Get().MiscStatusItems.SpoutOverPressure).PlayAnim("inactive", KAnim.PlayMode.Loop);
			this.post_erupt.PlayAnim("shake", KAnim.PlayMode.Loop).ScheduleGoTo((Geyser.StatesInstance smi) => smi.master.current_emissionType.duration_pst, this.idle);
		}

		public StateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.ObjectParameter<Geyser.EmissionType> emitType_a;

		public StateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.ObjectParameter<Geyser.EmissionType> emitType_b;

		public StateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.ObjectParameter<Geyser.EmissionType> current_emissionType;

		public GameStateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.State idle;

		public GameStateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.State pre_erupt;

		public Geyser.States.EruptState erupt;

		public GameStateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.State post_erupt;

		public class EruptState : GameStateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.State
		{
			public GameStateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.State erupting;

			public GameStateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.State overpressure;
		}
	}

	[Serializable]
	public class EmissionType
	{
		public EmissionType(float duration_pre, float duration_erupt, float duration_pst, ElementConverter.OutputElement emission_element, string override_animation = "")
		{
			this.duration_pre = duration_pre;
			this.duration_erupt = duration_erupt;
			this.duration_pst = duration_pst;
			this.emissionElement = emission_element;
			if (override_animation != "")
			{
				this.animation = override_animation;
			}
		}

		public float duration_pre;

		public float duration_erupt;

		public float duration_pst;

		public ElementConverter.OutputElement emissionElement;

		public string animation = "erupt";
	}
}
