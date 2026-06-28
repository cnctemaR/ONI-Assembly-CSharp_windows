using System;
using UnityEngine;

public class AttackChore : Chore<AttackChore.StatesInstance>
{
	public AttackChore(IStateMachineTarget target, GameObject enemy)
		: base(Db.Get().ChoreTypes.Attack, target, target.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.basic, int.MaxValue, false, true, 0)
	{
		this.smi = new AttackChore.StatesInstance(this);
		this.smi.sm.attackTarget.Set(enemy, this.smi);
		base.AddPrecondition(AttackChore.IsTargetable, enemy);
	}

	protected override void OnStateMachineStop(string reason, StateMachine.Status status)
	{
		this.CleanUpMultitool();
		base.OnStateMachineStop(reason, status);
	}

	public string GetHitAnim()
	{
		Workable component = this.smi.sm.attackTarget.Get(this.smi).gameObject.GetComponent<Workable>();
		string text2;
		if (component)
		{
			string text = MultitoolController.GetAnimationStrings(component, this.gameObject.GetComponent<Worker>(), "hit")[1];
			text = text.Replace("_loop", "");
			if (text.Contains("{verb}"))
			{
				text2 = "hit";
			}
			else
			{
				text2 = text;
			}
		}
		else
		{
			text2 = "hit";
		}
		return text2;
	}

	public void OnTargetMoved(object data)
	{
		int num = Grid.PosToCell(this.smi.master.gameObject);
		if (this.smi.sm.attackTarget.Get(this.smi) == null)
		{
			this.CleanUpMultitool();
		}
		else
		{
			if (this.smi.GetCurrentState() == this.smi.sm.attack)
			{
				int num2 = Grid.PosToCell(this.smi.sm.attackTarget.Get(this.smi).gameObject);
				IApproachable component = this.smi.sm.attackTarget.Get(this.smi).gameObject.GetComponent<IApproachable>();
				if (component != null)
				{
					CellOffset[] offsets = component.GetOffsets();
					if (num == num2 || !Grid.IsCellOffsetOf(num, num2, offsets))
					{
						if (this.multiTool != null)
						{
							this.CleanUpMultitool();
						}
						this.smi.GoTo(this.smi.sm.approachtarget);
					}
				}
				else
				{
					global::Debug.Log("has no approachable", null);
				}
			}
			if (this.multiTool != null)
			{
				this.multiTool.UpdateHitEffectTarget();
			}
		}
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		this.smi.sm.attacker.Set(context.consumer.gameObject, this.smi);
		base.Begin(context);
	}

	protected override void End(string reason)
	{
		this.CleanUpMultitool();
		base.End(reason);
	}

	public void OnTargetDestroyed(object data)
	{
		this.Fail("target destroyed");
	}

	private void CleanUpMultitool()
	{
		if (this.smi.master.multiTool != null)
		{
			this.multiTool.DestroyHitEffect();
			this.multiTool.StopSM("attack complete");
			this.multiTool = null;
		}
	}

	public static Chore.Precondition IsTargetable = new Chore.Precondition
	{
		id = "IsTargetable",
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			GameObject gameObject = (GameObject)data;
			return gameObject != null && gameObject.GetComponent<FactionAlignment>().targetable;
		}
	};

	private MultitoolController.Instance multiTool;

	public class StatesInstance : GameStateMachine<AttackChore.States, AttackChore.StatesInstance, AttackChore, object>.GameInstance
	{
		public StatesInstance(AttackChore master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<AttackChore.States, AttackChore.StatesInstance, AttackChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.approachtarget;
			this.root.ToggleStatusItem(Db.Get().DuplicantStatusItems.Fighting, (AttackChore.StatesInstance smi) => smi.master.gameObject).EventHandler(GameHashes.TargetLost, delegate(AttackChore.StatesInstance smi)
			{
				smi.master.Fail("target lost");
			}).Enter(delegate(AttackChore.StatesInstance smi)
			{
				smi.master.GetComponent<Weapon>().Configure(1f, 1f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.Single, 1, 0f);
			});
			this.approachtarget.InitializeStates(this.attacker, this.attackTarget, this.attack, null, null, NavigationTactics.Range_3_ProhibitOverlap).Enter(delegate(AttackChore.StatesInstance smi)
			{
				smi.master.CleanUpMultitool();
				smi.master.Trigger(1039067354, this.attackTarget.Get(smi));
				Health component = this.attackTarget.Get(smi).GetComponent<Health>();
				if (component == null || component.IsDefeated())
				{
					smi.StopSM("target defeated");
				}
			});
			this.attack.Target(this.attacker).Enter(delegate(AttackChore.StatesInstance smi)
			{
				this.attackTarget.Get(smi).Subscribe(1088554450, new Action<object>(smi.master.OnTargetMoved));
				if (this.attackTarget != null && smi.master.multiTool == null)
				{
					smi.master.multiTool = new MultitoolController.Instance(this.attackTarget.Get(smi).GetComponent<Workable>(), smi.master.GetComponent<Worker>(), "attack", EffectPrefabs.Instance.AttackEffect);
					smi.master.multiTool.StartSM();
				}
				this.attackTarget.Get(smi).Subscribe(1969584890, new Action<object>(smi.master.OnTargetDestroyed));
				smi.ScheduleGoTo(0.5f, this.success);
			}).Update(delegate(AttackChore.StatesInstance smi)
			{
				if (smi.master.multiTool != null)
				{
					smi.master.multiTool.UpdateHitEffectTarget();
				}
			})
				.Exit(delegate(AttackChore.StatesInstance smi)
				{
					if (this.attackTarget.Get(smi) != null)
					{
						this.attackTarget.Get(smi).Unsubscribe(1088554450, new Action<object>(smi.master.OnTargetMoved));
					}
				});
			this.success.Enter("finishAttack", delegate(AttackChore.StatesInstance smi)
			{
				if (this.attackTarget.Get(smi) != null)
				{
					Transform transform = this.attackTarget.Get(smi).transform;
					Weapon component2 = this.attacker.Get(smi).gameObject.GetComponent<Weapon>();
					if (component2 != null)
					{
						component2.AttackTarget(transform.gameObject);
						Health component3 = this.attackTarget.Get(smi).GetComponent<Health>();
						if (component3 != null)
						{
							if (!component3.IsDefeated())
							{
								smi.GoTo(this.attack);
							}
							else
							{
								smi.master.CleanUpMultitool();
								smi.StopSM("target defeated");
							}
						}
					}
					else
					{
						smi.master.CleanUpMultitool();
						smi.StopSM("no weapon");
					}
				}
				else
				{
					smi.master.CleanUpMultitool();
					smi.StopSM("no target");
				}
			}).ReturnSuccess();
		}

		public StateMachine<AttackChore.States, AttackChore.StatesInstance, AttackChore, object>.TargetParameter attackTarget;

		public StateMachine<AttackChore.States, AttackChore.StatesInstance, AttackChore, object>.TargetParameter attacker;

		public GameStateMachine<AttackChore.States, AttackChore.StatesInstance, AttackChore, object>.ApproachSubState<RangedAttackable> approachtarget;

		public GameStateMachine<AttackChore.States, AttackChore.StatesInstance, AttackChore, object>.State attack;

		public GameStateMachine<AttackChore.States, AttackChore.StatesInstance, AttackChore, object>.State success;
	}
}
