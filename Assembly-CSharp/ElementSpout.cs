using System;
using UnityEngine;

public class ElementSpout : StateMachineComponent<ElementSpout.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		int num = Grid.PosToCell(base.transform.GetPosition());
		Grid.Objects[num, 2] = base.gameObject;
		base.smi.StartSM();
	}

	public void SetEmitter(ElementEmitter emitter)
	{
		this.emitter = emitter;
	}

	public void ConfigureEmissionSettings(float emissionPollFrequency = 3f, float emissionIrregularity = 1.5f, float maxPressure = 1.5f, float perEmitAmount = 0.5f)
	{
		this.maxPressure = maxPressure;
		this.emissionPollFrequency = emissionPollFrequency;
		this.emissionIrregularity = emissionIrregularity;
		this.perEmitAmount = perEmitAmount;
	}

	[SerializeField]
	private ElementEmitter emitter;

	[MyCmpAdd]
	private KBatchedAnimController anim;

	public float maxPressure = 1.5f;

	public float emissionPollFrequency = 3f;

	public float emissionIrregularity = 1.5f;

	public float perEmitAmount = 0.5f;

	public class StatesInstance : GameStateMachine<ElementSpout.States, ElementSpout.StatesInstance, ElementSpout, object>.GameInstance
	{
		public StatesInstance(ElementSpout smi)
			: base(smi)
		{
		}

		private bool CanEmitOnCell(int cell, float max_pressure, Element.State expected_state)
		{
			return Grid.Cell[cell].mass < max_pressure && (Grid.Element[cell].IsState(expected_state) || Grid.Element[cell].IsVacuum);
		}

		public bool CanEmitAnywhere()
		{
			int num = Grid.PosToCell(base.smi.transform.GetPosition());
			int num2 = Grid.CellLeft(num);
			int num3 = Grid.CellRight(num);
			int num4 = Grid.CellAbove(num);
			Element.State state = base.smi.master.emitter.outputElement.element.state;
			return false || this.CanEmitOnCell(num, base.smi.master.maxPressure, state) || this.CanEmitOnCell(num2, base.smi.master.maxPressure, state) || this.CanEmitOnCell(num3, base.smi.master.maxPressure, state) || this.CanEmitOnCell(num4, base.smi.master.maxPressure, state);
		}
	}

	public class States : GameStateMachine<ElementSpout.States, ElementSpout.StatesInstance, ElementSpout>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idle;
			this.idle.DefaultState(this.idle.unblocked).Enter(delegate(ElementSpout.StatesInstance smi)
			{
				smi.Play("idle", KAnim.PlayMode.Once);
			}).ScheduleGoTo((ElementSpout.StatesInstance smi) => smi.master.emissionPollFrequency, this.emit);
			this.idle.unblocked.ToggleStatusItem(Db.Get().MiscStatusItems.SpoutPressureBuilding, null).Transition(this.idle.blocked, (ElementSpout.StatesInstance smi) => !smi.CanEmitAnywhere(), UpdateRate.SIM_200ms);
			this.idle.blocked.ToggleStatusItem(Db.Get().MiscStatusItems.SpoutOverPressure, null).Transition(this.idle.blocked, (ElementSpout.StatesInstance smi) => smi.CanEmitAnywhere(), UpdateRate.SIM_200ms);
			this.emit.DefaultState(this.emit.unblocked).Enter(delegate(ElementSpout.StatesInstance smi)
			{
				float num = 1f + global::UnityEngine.Random.Range(0f, smi.master.emissionIrregularity);
				float num2 = smi.master.perEmitAmount / num;
				smi.master.emitter.SetEmitting(true);
				smi.master.emitter.emissionFrequency = 1f;
				smi.master.emitter.outputElement.massGenerationRate = num2;
				smi.ScheduleGoTo(num, this.idle);
			});
			this.emit.unblocked.ToggleStatusItem(Db.Get().MiscStatusItems.SpoutEmitting, null).Enter(delegate(ElementSpout.StatesInstance smi)
			{
				smi.Play("emit", KAnim.PlayMode.Once);
				smi.master.emitter.SetEmitting(true);
			}).Transition(this.emit.blocked, (ElementSpout.StatesInstance smi) => !smi.CanEmitAnywhere(), UpdateRate.SIM_200ms);
			this.emit.blocked.ToggleStatusItem(Db.Get().MiscStatusItems.SpoutOverPressure, null).Enter(delegate(ElementSpout.StatesInstance smi)
			{
				smi.Play("idle", KAnim.PlayMode.Once);
				smi.master.emitter.SetEmitting(false);
			}).Transition(this.emit.unblocked, (ElementSpout.StatesInstance smi) => smi.CanEmitAnywhere(), UpdateRate.SIM_200ms);
		}

		public ElementSpout.States.Idle idle;

		public ElementSpout.States.Emitting emit;

		public class Idle : GameStateMachine<ElementSpout.States, ElementSpout.StatesInstance, ElementSpout, object>.State
		{
			public GameStateMachine<ElementSpout.States, ElementSpout.StatesInstance, ElementSpout, object>.State unblocked;

			public GameStateMachine<ElementSpout.States, ElementSpout.StatesInstance, ElementSpout, object>.State blocked;
		}

		public class Emitting : GameStateMachine<ElementSpout.States, ElementSpout.StatesInstance, ElementSpout, object>.State
		{
			public GameStateMachine<ElementSpout.States, ElementSpout.StatesInstance, ElementSpout, object>.State unblocked;

			public GameStateMachine<ElementSpout.States, ElementSpout.StatesInstance, ElementSpout, object>.State blocked;
		}
	}
}
