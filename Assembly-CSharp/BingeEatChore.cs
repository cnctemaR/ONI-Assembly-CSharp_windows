using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class BingeEatChore : Chore<BingeEatChore.StatesInstance>
{
	public BingeEatChore(IStateMachineTarget target, Action<Chore> on_complete = null)
		: base(Db.Get().ChoreTypes.BingeEat, target, target.GetComponent<ChoreProvider>(), false, on_complete, null, null, PriorityScreen.PriorityClass.emergency, 0, false, true, 0, null)
	{
		this.smi = new BingeEatChore.StatesInstance(this, target.gameObject);
		base.Subscribe(1121894420, new Action<object>(this.OnEat));
	}

	private void OnEat(object data)
	{
		Edible edible = (Edible)data;
		if (edible != null)
		{
			this.smi.sm.bingeremaining.Set(Mathf.Max(0f, this.smi.sm.bingeremaining.Get(this.smi) - edible.unitsConsumed), this.smi);
		}
	}

	public override void Cleanup()
	{
		base.Cleanup();
		base.Unsubscribe(1121894420, new Action<object>(this.OnEat));
	}

	public class StatesInstance : GameStateMachine<BingeEatChore.States, BingeEatChore.StatesInstance, BingeEatChore, object>.GameInstance
	{
		public StatesInstance(BingeEatChore master, GameObject eater)
			: base(master)
		{
			base.sm.eater.Set(eater, base.smi);
			base.sm.bingeremaining.Set(2f, base.smi);
		}

		public void FindFood()
		{
			Navigator component = base.GetComponent<Navigator>();
			int num = int.MaxValue;
			Edible edible = null;
			if (base.sm.bingeremaining.Get(base.smi) <= 0f)
			{
				this.GoTo(base.sm.eat_pst);
				return;
			}
			foreach (Edible edible2 in Components.Edibles)
			{
				if (!(edible2 == null))
				{
					if (!(edible2 == base.sm.ediblesource.Get<Edible>(base.smi)))
					{
						if (edible2.GetComponent<Pickupable>().UnreservedAmount > 0f)
						{
							if (edible2.GetComponent<Pickupable>().CouldBePickedUp(base.gameObject))
							{
								int navigationCost = component.GetNavigationCost(edible2);
								if (navigationCost != PathProber.InvalidCost)
								{
									if (navigationCost < num)
									{
										num = navigationCost;
										edible = edible2;
									}
								}
							}
						}
					}
				}
			}
			base.sm.ediblesource.Set(edible, base.smi);
			base.sm.requestedfoodunits.Set(base.sm.bingeremaining.Get(base.smi), base.smi);
			if (edible == null)
			{
				this.GoTo(base.sm.cantFindFood);
			}
			else
			{
				this.GoTo(base.sm.fetch);
			}
		}

		public bool IsBingeEating()
		{
			return base.sm.isBingeEating.Get(base.smi);
		}
	}

	public class States : GameStateMachine<BingeEatChore.States, BingeEatChore.StatesInstance, BingeEatChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.findfood;
			base.Target(this.eater);
			this.bingeEatingEffect = new Effect("Binge_Eating", DUPLICANTS.MODIFIERS.BINGE_EATING.NAME, DUPLICANTS.MODIFIERS.BINGE_EATING.TOOLTIP, 0f, true, false, true);
			this.bingeEatingEffect.Add(new AttributeModifier(Db.Get().Attributes.Decor.Id, -30f, DUPLICANTS.MODIFIERS.BINGE_EATING.NAME, false, false, true));
			this.bingeEatingEffect.Add(new AttributeModifier("CaloriesDelta", -6666.6665f, DUPLICANTS.MODIFIERS.BINGE_EATING.NAME, false, false, true));
			Db.Get().effects.Add(this.bingeEatingEffect);
			this.root.ToggleEffect((BingeEatChore.StatesInstance smi) => this.bingeEatingEffect);
			this.noTarget.GoTo(this.finish);
			this.eat_pst.ToggleAnims("anim_eat_overeat_kanim", 0f).PlayAnim("working_pst").OnAnimQueueComplete(this.finish);
			this.finish.Enter(delegate(BingeEatChore.StatesInstance smi)
			{
				smi.StopSM("complete/no more food");
			});
			this.findfood.Enter("FindFood", delegate(BingeEatChore.StatesInstance smi)
			{
				smi.FindFood();
			});
			this.fetch.InitializeStates(this.eater, this.ediblesource, this.ediblechunk, this.requestedfoodunits, this.actualfoodunits, this.eat, this.cantFindFood);
			this.eat.ToggleAnims("anim_eat_overeat_kanim", 0f).QueueAnim("working_loop", true, null).Enter(delegate(BingeEatChore.StatesInstance smi)
			{
				this.isBingeEating.Set(true, smi);
			})
				.DoEat(this.ediblechunk, this.actualfoodunits, this.findfood, this.findfood)
				.Exit("ClearIsBingeEating", delegate(BingeEatChore.StatesInstance smi)
				{
					this.isBingeEating.Set(false, smi);
				});
			this.cantFindFood.ToggleAnims("anim_interrupt_binge_eat_kanim", 0f).PlayAnim("interrupt_binge_eat").OnAnimQueueComplete(this.noTarget);
		}

		public StateMachine<BingeEatChore.States, BingeEatChore.StatesInstance, BingeEatChore, object>.TargetParameter eater;

		public StateMachine<BingeEatChore.States, BingeEatChore.StatesInstance, BingeEatChore, object>.TargetParameter ediblesource;

		public StateMachine<BingeEatChore.States, BingeEatChore.StatesInstance, BingeEatChore, object>.TargetParameter ediblechunk;

		public StateMachine<BingeEatChore.States, BingeEatChore.StatesInstance, BingeEatChore, object>.BoolParameter isBingeEating;

		public StateMachine<BingeEatChore.States, BingeEatChore.StatesInstance, BingeEatChore, object>.FloatParameter requestedfoodunits;

		public StateMachine<BingeEatChore.States, BingeEatChore.StatesInstance, BingeEatChore, object>.FloatParameter actualfoodunits;

		public StateMachine<BingeEatChore.States, BingeEatChore.StatesInstance, BingeEatChore, object>.FloatParameter bingeremaining;

		public GameStateMachine<BingeEatChore.States, BingeEatChore.StatesInstance, BingeEatChore, object>.State noTarget;

		public GameStateMachine<BingeEatChore.States, BingeEatChore.StatesInstance, BingeEatChore, object>.State findfood;

		public GameStateMachine<BingeEatChore.States, BingeEatChore.StatesInstance, BingeEatChore, object>.State eat;

		public GameStateMachine<BingeEatChore.States, BingeEatChore.StatesInstance, BingeEatChore, object>.State eat_pst;

		public GameStateMachine<BingeEatChore.States, BingeEatChore.StatesInstance, BingeEatChore, object>.State cantFindFood;

		public GameStateMachine<BingeEatChore.States, BingeEatChore.StatesInstance, BingeEatChore, object>.State finish;

		public GameStateMachine<BingeEatChore.States, BingeEatChore.StatesInstance, BingeEatChore, object>.FetchSubState fetch;

		private Effect bingeEatingEffect;
	}
}
