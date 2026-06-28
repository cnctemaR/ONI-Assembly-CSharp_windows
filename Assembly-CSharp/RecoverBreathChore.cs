using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class RecoverBreathChore : Chore<RecoverBreathChore.StatesInstance>
{
	public RecoverBreathChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.RecoverBreath, target, target.GetComponent<ChoreProvider>(), false, null, null, null, int.MaxValue, false, true, 0)
	{
		this.smi = new RecoverBreathChore.StatesInstance(this, target.gameObject);
		base.AddPrecondition(ChorePreconditions.IsNotRedAlert, null);
	}

	public class StatesInstance : GameStateMachine<RecoverBreathChore.States, RecoverBreathChore.StatesInstance, RecoverBreathChore, object>.GameInstance
	{
		public StatesInstance(RecoverBreathChore master, GameObject recoverer)
			: base(master)
		{
			base.sm.recoverer.Set(recoverer, base.smi);
			Klei.AI.Attribute deltaAttribute = Db.Get().Amounts.Breath.deltaAttribute;
			float num = 3f;
			this.recoveringbreath = new AttributeModifier(deltaAttribute.Id, num, DUPLICANTS.MODIFIERS.RECOVERINGBREATH.NAME, false, false);
		}

		public void CreateLocator()
		{
			GameObject gameObject = ChoreHelpers.CreateLocator("SleepLocator", Vector3.zero);
			base.sm.locator.Set(gameObject, this);
			this.UpdateLocator();
		}

		public void UpdateLocator()
		{
			int num = base.sm.recoverer.GetSMI<BreathMonitor.Instance>(base.smi).GetRecoverCell();
			if (num == Grid.InvalidCell)
			{
				num = Grid.PosToCell(base.sm.recoverer.Get<Transform>(base.smi).position);
			}
			Vector3 vector = Grid.CellToPosCBC(num, Grid.SceneLayer.Move);
			base.sm.locator.Get<Transform>(base.smi).SetPosition(vector);
		}

		public void DestroyLocator()
		{
			ChoreHelpers.DestroyLocator(base.sm.locator.Get(this));
			base.sm.locator.Set(null, this);
		}

		public AttributeModifier recoveringbreath;
	}

	public class States : GameStateMachine<RecoverBreathChore.States, RecoverBreathChore.StatesInstance, RecoverBreathChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.approach;
			base.Target(this.recoverer);
			this.root.Enter("CreateLocator", delegate(RecoverBreathChore.StatesInstance smi)
			{
				smi.CreateLocator();
			}).Exit("DestroyLocator", delegate(RecoverBreathChore.StatesInstance smi)
			{
				smi.DestroyLocator();
			}).Update("UpdateLocator", delegate(RecoverBreathChore.StatesInstance smi)
			{
				smi.UpdateLocator();
			});
			this.approach.InitializeStates(this.recoverer, this.locator, this.recover, null, null, null);
			this.recover.DefaultState(this.recover.pre).ToggleAttributeModifier("Recovering Breath", (RecoverBreathChore.StatesInstance smi) => smi.recoveringbreath, null);
			this.recover.pre.PlayAnim("breathe_pre", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.recover.loop);
			this.recover.loop.PlayAnim("breathe_loop", KAnim.PlayMode.Loop, null);
			this.recover.pst.QueueAnim("breathe_pst", false, null).OnAnimQueueComplete(null);
		}

		public GameStateMachine<RecoverBreathChore.States, RecoverBreathChore.StatesInstance, RecoverBreathChore, object>.ApproachSubState<Approachable> approach;

		public GameStateMachine<RecoverBreathChore.States, RecoverBreathChore.StatesInstance, RecoverBreathChore, object>.PLPState recover;

		public StateMachine<RecoverBreathChore.States, RecoverBreathChore.StatesInstance, RecoverBreathChore, object>.TargetParameter recoverer;

		public StateMachine<RecoverBreathChore.States, RecoverBreathChore.StatesInstance, RecoverBreathChore, object>.TargetParameter locator;
	}
}
