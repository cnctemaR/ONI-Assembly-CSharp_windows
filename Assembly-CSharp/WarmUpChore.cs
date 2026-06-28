using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class WarmUpChore : Chore<WarmUpChore.StatesInstance>
{
	public WarmUpChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.Warmup, target, target.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.basic, int.MaxValue, false, true, 0)
	{
		this.smi = new WarmUpChore.StatesInstance(this, target.gameObject);
		base.AddPrecondition(ChorePreconditions.IsNotRedAlert, null);
	}

	public class StatesInstance : GameStateMachine<WarmUpChore.States, WarmUpChore.StatesInstance, WarmUpChore, object>.GameInstance
	{
		public StatesInstance(WarmUpChore master, GameObject recoverer)
			: base(master)
		{
			base.sm.recoverer.Set(recoverer, base.smi);
			this.primaryElement = recoverer.GetComponent<PrimaryElement>();
			Klei.AI.Attribute deltaAttribute = Db.Get().Amounts.Temperature.deltaAttribute;
			this.warmingUp = new AttributeModifier(deltaAttribute.Id, 0f, DUPLICANTS.MODIFIERS.WARMINGUP.NAME, false, true, false);
			CreatureSimTemperatureTransfer component = base.smi.master.GetComponent<CreatureSimTemperatureTransfer>();
			component.NonSimTemperatureModifiers.Add(this.warmingUp);
		}

		public void CreateLocator()
		{
			GameObject gameObject = ChoreHelpers.CreateLocator("WarmUpLocator", Vector3.zero);
			base.sm.locator.Set(gameObject, this);
			this.UpdateLocator();
		}

		public void UpdateLocator()
		{
			int num = base.sm.recoverer.GetSMI<TemperatureMonitor.Instance>(base.smi).GetWarmUpCell();
			if (num == Grid.InvalidCell)
			{
				num = Grid.PosToCell(base.sm.recoverer.Get<Transform>(base.smi).position);
				this.noAvailableTarget = true;
			}
			else
			{
				this.noAvailableTarget = false;
			}
			Vector3 vector = Grid.CellToPosCBC(num, Grid.SceneLayer.Move);
			base.sm.locator.Get<Transform>(base.smi).SetPosition(vector);
		}

		public void DestroyLocator()
		{
			ChoreHelpers.DestroyLocator(base.sm.locator.Get(this));
			base.sm.locator.Set(null, this);
		}

		public AttributeModifier warmingUp;

		public bool noAvailableTarget = true;

		public float tempChangePerSecond = 0.125f;

		public PrimaryElement primaryElement;
	}

	public class States : GameStateMachine<WarmUpChore.States, WarmUpChore.StatesInstance, WarmUpChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.NoLocationAvailable;
			base.Target(this.recoverer);
			this.root.Enter("CreateLocator", delegate(WarmUpChore.StatesInstance smi)
			{
				smi.CreateLocator();
			}).Exit("DestroyLocator", delegate(WarmUpChore.StatesInstance smi)
			{
				smi.DestroyLocator();
			}).Update("UpdateLocator", delegate(WarmUpChore.StatesInstance smi)
			{
				smi.UpdateLocator();
			});
			this.NoLocationAvailable.Update(delegate(WarmUpChore.StatesInstance smi)
			{
				if (!smi.noAvailableTarget)
				{
					smi.GoTo(this.approach);
				}
			});
			this.approach.InitializeStates(this.recoverer, this.locator, this.recover, null, null, null).Transition(this.NoLocationAvailable, (WarmUpChore.StatesInstance smi) => smi.noAvailableTarget);
			this.recover.DefaultState(this.recover.pre).ToggleAnims("anim_idle_hot_kanim_kanim", 30f).ToggleAttributeModifier("Warming Up", (WarmUpChore.StatesInstance smi) => smi.warmingUp, null)
				.Enter(delegate(WarmUpChore.StatesInstance smi)
				{
					smi.warmingUp.SetValue(smi.tempChangePerSecond);
				})
				.Exit(delegate(WarmUpChore.StatesInstance smi)
				{
					smi.warmingUp.SetValue(0f);
				});
			this.recover.pre.PlayAnim("idle_pre").OnAnimQueueComplete(this.recover.loop);
			this.recover.loop.PlayAnim("idle_default", KAnim.PlayMode.Loop);
			this.recover.pst.QueueAnim("idle_pst", false, null).OnAnimQueueComplete(null);
		}

		public GameStateMachine<WarmUpChore.States, WarmUpChore.StatesInstance, WarmUpChore, object>.State NoLocationAvailable;

		public GameStateMachine<WarmUpChore.States, WarmUpChore.StatesInstance, WarmUpChore, object>.ApproachSubState<Approachable> approach;

		public GameStateMachine<WarmUpChore.States, WarmUpChore.StatesInstance, WarmUpChore, object>.PLPState recover;

		public StateMachine<WarmUpChore.States, WarmUpChore.StatesInstance, WarmUpChore, object>.TargetParameter recoverer;

		public StateMachine<WarmUpChore.States, WarmUpChore.StatesInstance, WarmUpChore, object>.TargetParameter locator;
	}
}
