using System;
using UnityEngine;

public class RecoverFromColdChore : Chore<RecoverFromColdChore.Instance>
{
	public RecoverFromColdChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.RecoverWarmth, target, target.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.personalNeeds, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new RecoverFromColdChore.Instance(this, target.gameObject);
		ColdImmunityMonitor.Instance coldImmunityMonitor = target.gameObject.GetSMI<ColdImmunityMonitor.Instance>();
		Func<int> func = () => coldImmunityMonitor.WarmUpCell;
		base.AddPrecondition(ChorePreconditions.instance.CanMoveToDynamicCell, func);
	}

	public class States : GameStateMachine<RecoverFromColdChore.States, RecoverFromColdChore.Instance, RecoverFromColdChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.approach;
			base.Target(this.entityRecovering);
			this.root.Enter("CreateLocator", delegate(RecoverFromColdChore.Instance smi)
			{
				smi.CreateLocator();
			}).Enter("UpdateImmunityProvider", delegate(RecoverFromColdChore.Instance smi)
			{
				smi.UpdateImmunityProvider();
			}).Exit("DestroyLocator", delegate(RecoverFromColdChore.Instance smi)
			{
				smi.DestroyLocator();
			})
				.Update("UpdateLocator", delegate(RecoverFromColdChore.Instance smi, float dt)
				{
					smi.UpdateLocator();
				}, UpdateRate.SIM_200ms, true)
				.Update("UpdateColdImmunityProvider", delegate(RecoverFromColdChore.Instance smi, float dt)
				{
					smi.UpdateImmunityProvider();
				}, UpdateRate.SIM_200ms, true);
			this.approach.InitializeStates(this.entityRecovering, this.locator, this.recover, null, null, null);
			this.recover.OnTargetLost(this.coldImmunityProvider, null).ToggleAnims(new Func<RecoverFromColdChore.Instance, HashedString>(RecoverFromColdChore.States.GetAnimFileName)).DefaultState(this.recover.pre)
				.ToggleTag(GameTags.RecoveringWarmnth);
			this.recover.pre.Face(this.coldImmunityProvider, 0f).PlayAnim(new Func<RecoverFromColdChore.Instance, string>(RecoverFromColdChore.States.GetPreAnimName), KAnim.PlayMode.Once).OnAnimQueueComplete(this.recover.loop);
			this.recover.loop.PlayAnim(new Func<RecoverFromColdChore.Instance, string>(RecoverFromColdChore.States.GetLoopAnimName), KAnim.PlayMode.Once).OnAnimQueueComplete(this.recover.pst);
			this.recover.pst.QueueAnim(new Func<RecoverFromColdChore.Instance, string>(RecoverFromColdChore.States.GetPstAnimName), false, null).OnAnimQueueComplete(this.complete);
			this.complete.DefaultState(this.complete.evaluate);
			this.complete.evaluate.EnterTransition(this.complete.success, new StateMachine<RecoverFromColdChore.States, RecoverFromColdChore.Instance, RecoverFromColdChore, object>.Transition.ConditionCallback(RecoverFromColdChore.States.IsImmunityProviderStillValid)).EnterTransition(this.complete.fail, GameStateMachine<RecoverFromColdChore.States, RecoverFromColdChore.Instance, RecoverFromColdChore, object>.Not(new StateMachine<RecoverFromColdChore.States, RecoverFromColdChore.Instance, RecoverFromColdChore, object>.Transition.ConditionCallback(RecoverFromColdChore.States.IsImmunityProviderStillValid)));
			this.complete.success.Enter(new StateMachine<RecoverFromColdChore.States, RecoverFromColdChore.Instance, RecoverFromColdChore, object>.State.Callback(RecoverFromColdChore.States.ApplyColdImmunityEffect)).ReturnSuccess();
			this.complete.fail.ReturnFailure();
		}

		public static bool IsImmunityProviderStillValid(RecoverFromColdChore.Instance smi)
		{
			ColdImmunityProvider.Instance lastKnownImmunityProvider = smi.lastKnownImmunityProvider;
			return lastKnownImmunityProvider != null && lastKnownImmunityProvider.CanBeUsed;
		}

		public static void ApplyColdImmunityEffect(RecoverFromColdChore.Instance smi)
		{
			ColdImmunityProvider.Instance lastKnownImmunityProvider = smi.lastKnownImmunityProvider;
			if (lastKnownImmunityProvider != null)
			{
				lastKnownImmunityProvider.ApplyImmunityEffect(smi.gameObject, true);
			}
		}

		public static HashedString GetAnimFileName(RecoverFromColdChore.Instance smi)
		{
			return RecoverFromColdChore.States.GetAnimFromColdImmunityProvider(smi, (ColdImmunityProvider.Instance p) => p.AnimFileName);
		}

		public static string GetPreAnimName(RecoverFromColdChore.Instance smi)
		{
			return RecoverFromColdChore.States.GetAnimFromColdImmunityProvider(smi, (ColdImmunityProvider.Instance p) => p.PreAnimName);
		}

		public static string GetLoopAnimName(RecoverFromColdChore.Instance smi)
		{
			return RecoverFromColdChore.States.GetAnimFromColdImmunityProvider(smi, (ColdImmunityProvider.Instance p) => p.LoopAnimName);
		}

		public static string GetPstAnimName(RecoverFromColdChore.Instance smi)
		{
			return RecoverFromColdChore.States.GetAnimFromColdImmunityProvider(smi, (ColdImmunityProvider.Instance p) => p.PstAnimName);
		}

		public static string GetAnimFromColdImmunityProvider(RecoverFromColdChore.Instance smi, Func<ColdImmunityProvider.Instance, string> getCallback)
		{
			ColdImmunityProvider.Instance lastKnownImmunityProvider = smi.lastKnownImmunityProvider;
			if (lastKnownImmunityProvider != null)
			{
				return getCallback(lastKnownImmunityProvider);
			}
			return null;
		}

		public GameStateMachine<RecoverFromColdChore.States, RecoverFromColdChore.Instance, RecoverFromColdChore, object>.ApproachSubState<IApproachable> approach;

		public GameStateMachine<RecoverFromColdChore.States, RecoverFromColdChore.Instance, RecoverFromColdChore, object>.PreLoopPostState recover;

		public GameStateMachine<RecoverFromColdChore.States, RecoverFromColdChore.Instance, RecoverFromColdChore, object>.State remove_suit;

		public RecoverFromColdChore.States.CompleteStates complete;

		public StateMachine<RecoverFromColdChore.States, RecoverFromColdChore.Instance, RecoverFromColdChore, object>.TargetParameter coldImmunityProvider;

		public StateMachine<RecoverFromColdChore.States, RecoverFromColdChore.Instance, RecoverFromColdChore, object>.TargetParameter entityRecovering;

		public StateMachine<RecoverFromColdChore.States, RecoverFromColdChore.Instance, RecoverFromColdChore, object>.TargetParameter locator;

		public class CompleteStates : GameStateMachine<RecoverFromColdChore.States, RecoverFromColdChore.Instance, RecoverFromColdChore, object>.State
		{
			public GameStateMachine<RecoverFromColdChore.States, RecoverFromColdChore.Instance, RecoverFromColdChore, object>.State evaluate;

			public GameStateMachine<RecoverFromColdChore.States, RecoverFromColdChore.Instance, RecoverFromColdChore, object>.State fail;

			public GameStateMachine<RecoverFromColdChore.States, RecoverFromColdChore.Instance, RecoverFromColdChore, object>.State success;
		}
	}

	public class Instance : GameStateMachine<RecoverFromColdChore.States, RecoverFromColdChore.Instance, RecoverFromColdChore, object>.GameInstance
	{
		public ColdImmunityProvider.Instance lastKnownImmunityProvider
		{
			get
			{
				if (!(base.sm.coldImmunityProvider.Get(this) == null))
				{
					return base.sm.coldImmunityProvider.Get(this).GetSMI<ColdImmunityProvider.Instance>();
				}
				return null;
			}
		}

		public ColdImmunityMonitor.Instance coldImmunityMonitor
		{
			get
			{
				return base.sm.entityRecovering.Get(this).GetSMI<ColdImmunityMonitor.Instance>();
			}
		}

		public Instance(RecoverFromColdChore master, GameObject entityRecovering)
			: base(master)
		{
			base.sm.entityRecovering.Set(entityRecovering, this, false);
			ColdImmunityMonitor.Instance coldImmunityMonitor = this.coldImmunityMonitor;
			if (coldImmunityMonitor.NearestImmunityProvider != null && !coldImmunityMonitor.NearestImmunityProvider.isMasterNull)
			{
				base.sm.coldImmunityProvider.Set(coldImmunityMonitor.NearestImmunityProvider.gameObject, this, false);
			}
		}

		public void CreateLocator()
		{
			GameObject gameObject = ChoreHelpers.CreateLocator("RecoverWarmthLocator", Vector3.zero);
			base.sm.locator.Set(gameObject, this, false);
			this.UpdateLocator();
		}

		public void UpdateImmunityProvider()
		{
			ColdImmunityProvider.Instance nearestImmunityProvider = this.coldImmunityMonitor.NearestImmunityProvider;
			base.sm.coldImmunityProvider.Set((nearestImmunityProvider == null || nearestImmunityProvider.isMasterNull) ? null : nearestImmunityProvider.gameObject, this, false);
		}

		public void UpdateLocator()
		{
			int num = this.coldImmunityMonitor.WarmUpCell;
			if (num == Grid.InvalidCell)
			{
				num = Grid.PosToCell(base.sm.entityRecovering.Get<Transform>(base.smi).GetPosition());
				this.DestroyLocator();
			}
			else
			{
				Vector3 vector = Grid.CellToPosCBC(num, Grid.SceneLayer.Move);
				base.sm.locator.Get<Transform>(base.smi).SetPosition(vector);
			}
			this.targetCell = num;
		}

		public void DestroyLocator()
		{
			ChoreHelpers.DestroyLocator(base.sm.locator.Get(this));
			base.sm.locator.Set(null, this);
		}

		private int targetCell;
	}
}
