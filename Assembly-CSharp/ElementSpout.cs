using System;
using UnityEngine;

public class ElementSpout : StateMachineComponent<ElementSpout.StatesInstance>
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

	private Vector2 emitPoint = Vector2.zero;

	public class StatesInstance : GameStateMachine<ElementSpout.States, ElementSpout.StatesInstance, ElementSpout, object>.GameInstance
	{
		public StatesInstance(ElementSpout smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<ElementSpout.States, ElementSpout.StatesInstance, ElementSpout>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idle;
			this.idle.Enter(delegate(ElementSpout.StatesInstance smi)
			{
				smi.Play("idle", KAnim.PlayMode.Once);
				Grid.Objects[Grid.PosToCell(smi.gameObject.transform.position), 2] = smi.gameObject;
				bool flag = Grid.Cell[Grid.CellLeft(Grid.PosToCell(smi.transform.position))].mass < smi.master.maxPressure;
				bool flag2 = Grid.Cell[Grid.CellRight(Grid.PosToCell(smi.transform.position))].mass < smi.master.maxPressure;
				bool flag3 = Grid.Cell[Grid.CellAbove(Grid.PosToCell(smi.transform.position))].mass < smi.master.maxPressure;
				bool flag4 = Grid.Cell[Grid.PosToCell(smi.transform.position)].mass < smi.master.maxPressure;
				bool flag5 = true;
				smi.master.emitPoint = Vector2.zero;
				if (flag4)
				{
					smi.master.emitPoint = Vector2.zero;
				}
				else if (flag3)
				{
					smi.master.emitPoint = Vector2.up;
				}
				else if (flag && flag2)
				{
					smi.master.emitPoint = ((global::UnityEngine.Random.Range(0, 100) <= 50) ? Vector2.right : Vector2.left);
				}
				else if (flag)
				{
					smi.master.emitPoint = Vector2.left;
				}
				else if (flag2)
				{
					smi.master.emitPoint = Vector2.right;
				}
				else
				{
					flag5 = false;
				}
				if (flag5)
				{
					smi.GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.SpoutPressureBuilding, this);
					smi.ScheduleGoTo(smi.master.emissionPollFrequency, this.emit);
				}
				else
				{
					smi.GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.SpoutOverPressure, this);
					smi.ScheduleGoTo(smi.master.emissionPollFrequency, this.overPressure);
				}
			});
			this.emit.Enter(delegate(ElementSpout.StatesInstance smi)
			{
				smi.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.SpoutPressureBuilding, false);
				smi.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.SpoutOverPressure, false);
				smi.GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.SpoutEmitting, this);
				smi.Play("emit", KAnim.PlayMode.Once);
				smi.master.emitter.outputElement.outputElementOffset = smi.master.emitPoint;
				smi.master.emitter.ForceEmit(smi.master.perEmitAmount, -1f);
				smi.ScheduleGoTo(1f + global::UnityEngine.Random.Range(0f, smi.master.emissionIrregularity), this.idle);
			}).Exit(delegate(ElementSpout.StatesInstance smi)
			{
				smi.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.SpoutEmitting, false);
			});
			this.overPressure.Enter(delegate(ElementSpout.StatesInstance smi)
			{
				smi.GoTo(this.idle);
			}).Exit(delegate(ElementSpout.StatesInstance smi)
			{
			});
		}

		public GameStateMachine<ElementSpout.States, ElementSpout.StatesInstance, ElementSpout, object>.State idle;

		public GameStateMachine<ElementSpout.States, ElementSpout.StatesInstance, ElementSpout, object>.State emit;

		public GameStateMachine<ElementSpout.States, ElementSpout.StatesInstance, ElementSpout, object>.State overPressure;
	}
}
