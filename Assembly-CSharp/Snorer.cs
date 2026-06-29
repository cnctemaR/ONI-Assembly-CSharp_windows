using System;
using Klei.AI;
using UnityEngine;

[SkipSaveFileSerialization]
public class Snorer : StateMachineComponent<Snorer.StatesInstance>
{
	protected override void OnPrefabInit()
	{
		base.Subscribe(1623392196, new Action<object>(this.OnDeath));
		base.Subscribe(-1117766961, new Action<object>(this.OnRevived));
	}

	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	private void OnDeath(object data)
	{
		base.enabled = false;
	}

	private void OnRevived(object data)
	{
		base.enabled = true;
	}

	public void ModifyTrait(Trait t)
	{
	}

	private static readonly HashedString HeadHash = "snapTo_mouth";

	public class StatesInstance : GameStateMachine<Snorer.States, Snorer.StatesInstance, Snorer, object>.GameInstance
	{
		public StatesInstance(Snorer master)
			: base(master)
		{
		}

		public bool IsSleeping()
		{
			StaminaMonitor.Instance smi = base.master.GetSMI<StaminaMonitor.Instance>();
			return smi != null && smi.IsSleeping();
		}

		public void StartSmallSnore()
		{
			this.snoreHandle = GameScheduler.Instance.Schedule("snorelines", 2f, new Action<object>(this.StartSmallSnoreInternal), null, null);
		}

		private void StartSmallSnoreInternal(object data)
		{
			this.snoreHandle.ClearScheduler();
			KBatchedAnimController component = base.smi.master.GetComponent<KBatchedAnimController>();
			bool flag;
			Matrix4x4 symbolTransform = component.GetSymbolTransform(Snorer.HeadHash, out flag);
			if (flag)
			{
				Vector4 column = symbolTransform.GetColumn(3);
				Vector3 vector = column;
				vector.z = Grid.GetLayerZ(Grid.SceneLayer.FXFront);
				this.snoreEffect = FXHelpers.CreateEffect("snore_fx_kanim", vector, SceneOrganizer.Instance.GetFolder(Folder.FX).transform, false, Grid.SceneLayer.Front, false);
				this.snoreEffect.destroyOnAnimComplete = true;
				this.snoreEffect.Play("snore", KAnim.PlayMode.Loop, 1f, 0f);
			}
		}

		public void StopSmallSnore()
		{
			this.snoreHandle.ClearScheduler();
			if (this.snoreEffect != null)
			{
				this.snoreEffect.PlayMode = KAnim.PlayMode.Once;
			}
			this.snoreEffect = null;
		}

		public void StartSnoreBGEffect()
		{
			AcousticDisturbance.Emit(base.smi.master.gameObject, 3);
		}

		public void StopSnoreBGEffect()
		{
		}

		private SchedulerHandle snoreHandle;

		private KBatchedAnimController snoreEffect;

		private KBatchedAnimController snoreBGEffect;

		private const float BGEmissionRadius = 3f;
	}

	public class States : GameStateMachine<Snorer.States, Snorer.StatesInstance, Snorer>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idle;
			this.idle.Transition(this.sleeping, (Snorer.StatesInstance smi) => smi.IsSleeping(), UpdateRate.SIM_200ms);
			this.sleeping.DefaultState(this.sleeping.quiet).Enter(delegate(Snorer.StatesInstance smi)
			{
				smi.StartSmallSnore();
			}).Exit(delegate(Snorer.StatesInstance smi)
			{
				smi.StopSmallSnore();
			})
				.Transition(this.idle, (Snorer.StatesInstance smi) => !smi.master.GetSMI<StaminaMonitor.Instance>().IsSleeping(), UpdateRate.SIM_200ms);
			this.sleeping.quiet.Enter("ScheduleNextSnore", delegate(Snorer.StatesInstance smi)
			{
				smi.ScheduleGoTo(this.GetNewInterval(), this.sleeping.snoring);
			});
			this.sleeping.snoring.Enter(delegate(Snorer.StatesInstance smi)
			{
				smi.StartSnoreBGEffect();
			}).ToggleExpression(Db.Get().Expressions.Relief, null).ScheduleGoTo(3f, this.sleeping.quiet)
				.Exit(delegate(Snorer.StatesInstance smi)
				{
					smi.StopSnoreBGEffect();
				});
		}

		private float GetNewInterval()
		{
			float num = Util.GaussianRandom(5f, 1f);
			num = Mathf.Max(num, 3f);
			return Mathf.Min(num, 10f);
		}

		public GameStateMachine<Snorer.States, Snorer.StatesInstance, Snorer, object>.State idle;

		public Snorer.States.SleepStates sleeping;

		public class SleepStates : GameStateMachine<Snorer.States, Snorer.StatesInstance, Snorer, object>.State
		{
			public GameStateMachine<Snorer.States, Snorer.StatesInstance, Snorer, object>.State quiet;

			public GameStateMachine<Snorer.States, Snorer.StatesInstance, Snorer, object>.State snoring;
		}
	}

	private struct CellInfo
	{
		public override int GetHashCode()
		{
			return this.cell;
		}

		public override bool Equals(object obj)
		{
			Snorer.CellInfo cellInfo = (Snorer.CellInfo)obj;
			return this.cell == cellInfo.cell;
		}

		public int cell;

		public int depth;
	}
}
