using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class BansheeChore : Chore<BansheeChore.StatesInstance>
{
	public BansheeChore(ChoreType chore_type, IStateMachineTarget target, Notification notification, Action<Chore> on_complete = null)
		: base(Db.Get().ChoreTypes.BansheeWail, target, target.GetComponent<ChoreProvider>(), false, on_complete, null, null, PriorityScreen.PriorityClass.compulsory, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new BansheeChore.StatesInstance(this, target.gameObject, notification);
	}

	private const string audienceEffectName = "WailedAt";

	public class StatesInstance : GameStateMachine<BansheeChore.States, BansheeChore.StatesInstance, BansheeChore, object>.GameInstance
	{
		public StatesInstance(BansheeChore master, GameObject wailer, Notification notification)
			: base(master)
		{
			base.sm.wailer.Set(wailer, base.smi, false);
			this.notification = notification;
		}

		public void FindAudience()
		{
			Navigator component = base.GetComponent<Navigator>();
			int num = (int)Grid.WorldIdx[Grid.PosToCell(base.gameObject)];
			int num2 = int.MaxValue;
			int num3 = Grid.InvalidCell;
			List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(num, false);
			for (int i = 0; i < worldItems.Count; i++)
			{
				if (!worldItems[i].IsNullOrDestroyed() && !(worldItems[i].gameObject == base.gameObject))
				{
					int num4 = Grid.PosToCell(worldItems[i]);
					if (component.CanReach(num4) && !worldItems[i].GetComponent<Effects>().HasEffect("WailedAt"))
					{
						int navigationCost = component.GetNavigationCost(num4);
						if (navigationCost < num2)
						{
							num2 = navigationCost;
							num3 = num4;
						}
					}
				}
			}
			if (num3 == Grid.InvalidCell)
			{
				num3 = this.FindIdleCell();
			}
			base.sm.targetWailLocation.Set(num3, base.smi, false);
			this.GoTo(base.sm.moveToAudience);
		}

		public int FindIdleCell()
		{
			Navigator component = base.smi.master.GetComponent<Navigator>();
			MinionPathFinderAbilities minionPathFinderAbilities = (MinionPathFinderAbilities)component.GetCurrentAbilities();
			minionPathFinderAbilities.SetIdleNavMaskEnabled(true);
			IdleCellQuery idleCellQuery = PathFinderQueries.idleCellQuery.Reset(base.GetComponent<MinionBrain>(), global::UnityEngine.Random.Range(30, 90));
			component.RunQuery(idleCellQuery);
			minionPathFinderAbilities.SetIdleNavMaskEnabled(false);
			return idleCellQuery.GetResultCell();
		}

		public void BotherAudience(float dt)
		{
			if (dt <= 0f)
			{
				return;
			}
			int num = Grid.PosToCell(base.smi.master.gameObject);
			int num2 = (int)Grid.WorldIdx[num];
			foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.GetWorldItems(num2, false))
			{
				if (!minionIdentity.IsNullOrDestroyed() && !(minionIdentity.gameObject == base.smi.master.gameObject))
				{
					int num3 = Grid.PosToCell(minionIdentity);
					if (Grid.GetCellDistance(num, Grid.PosToCell(minionIdentity)) <= STRESS.BANSHEE_WAIL_RADIUS)
					{
						HashSet<int> hashSet = new HashSet<int>();
						Grid.CollectCellsInLine(num, num3, hashSet);
						bool flag = false;
						foreach (int num4 in hashSet)
						{
							if (Grid.Solid[num4])
							{
								flag = true;
								break;
							}
						}
						if (!flag && !minionIdentity.GetComponent<Effects>().HasEffect("WailedAt"))
						{
							minionIdentity.GetComponent<Effects>().Add("WailedAt", true);
							minionIdentity.GetSMI<ThreatMonitor.Instance>().ClearMainThreat();
							new FleeChore(minionIdentity.GetComponent<IStateMachineTarget>(), base.smi.master.gameObject);
						}
					}
				}
			}
		}

		public Notification notification;
	}

	public class States : GameStateMachine<BansheeChore.States, BansheeChore.StatesInstance, BansheeChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.findAudience;
			base.Target(this.wailer);
			this.wailPreEffect = new Effect("BansheeWailing", DUPLICANTS.MODIFIERS.BANSHEE_WAILING.NAME, DUPLICANTS.MODIFIERS.BANSHEE_WAILING.TOOLTIP, 0f, true, false, true, null, -1f, 0f, null, "");
			this.wailPreEffect.Add(new AttributeModifier("AirConsumptionRate", 7.5f, null, false, false, true));
			Db.Get().effects.Add(this.wailPreEffect);
			this.wailRecoverEffect = new Effect("BansheeWailingRecovery", DUPLICANTS.MODIFIERS.BANSHEE_WAILING_RECOVERY.NAME, DUPLICANTS.MODIFIERS.BANSHEE_WAILING_RECOVERY.TOOLTIP, 0f, true, false, true, null, -1f, 0f, null, "");
			this.wailRecoverEffect.Add(new AttributeModifier("AirConsumptionRate", 1f, null, false, false, true));
			Db.Get().effects.Add(this.wailRecoverEffect);
			this.findAudience.Enter("FindAudience", delegate(BansheeChore.StatesInstance smi)
			{
				smi.FindAudience();
			}).ToggleAnims("anim_loco_banshee_kanim", 0f);
			this.moveToAudience.MoveTo((BansheeChore.StatesInstance smi) => smi.sm.targetWailLocation.Get(smi), this.wail, this.delay, false).ToggleAnims("anim_loco_banshee_kanim", 0f);
			this.wail.defaultState = this.wail.pre.DoNotification((BansheeChore.StatesInstance smi) => smi.notification);
			this.wail.pre.ToggleAnims("anim_banshee_kanim", 0f).PlayAnim("working_pre").ToggleEffect((BansheeChore.StatesInstance smi) => this.wailPreEffect)
				.OnAnimQueueComplete(this.wail.loop);
			this.wail.loop.ToggleAnims("anim_banshee_kanim", 0f).Enter(delegate(BansheeChore.StatesInstance smi)
			{
				smi.Play("working_loop", KAnim.PlayMode.Loop);
				AcousticDisturbance.Emit(smi.master.gameObject, STRESS.BANSHEE_WAIL_RADIUS);
			}).ScheduleGoTo(5f, this.wail.pst)
				.Update(delegate(BansheeChore.StatesInstance smi, float dt)
				{
					smi.BotherAudience(dt);
				}, UpdateRate.SIM_200ms, false);
			this.wail.pst.ToggleAnims("anim_banshee_kanim", 0f).QueueAnim("working_pst", false, null).EventHandlerTransition(GameHashes.AnimQueueComplete, this.recover, (BansheeChore.StatesInstance smi, object data) => true)
				.ScheduleGoTo(3f, this.recover);
			this.recover.ToggleEffect((BansheeChore.StatesInstance smi) => this.wailRecoverEffect).ToggleAnims("anim_emotes_default_kanim", 0f).QueueAnim("breathe_pre", false, null)
				.QueueAnim("breathe_loop", false, null)
				.QueueAnim("breathe_loop", false, null)
				.QueueAnim("breathe_loop", false, null)
				.QueueAnim("breathe_pst", false, null)
				.OnAnimQueueComplete(this.complete);
			this.delay.ScheduleGoTo(1f, this.wander);
			this.wander.MoveTo((BansheeChore.StatesInstance smi) => smi.FindIdleCell(), this.findAudience, this.findAudience, false).ToggleAnims("anim_loco_banshee_kanim", 0f);
			this.complete.Enter(delegate(BansheeChore.StatesInstance smi)
			{
				smi.StopSM("complete");
			});
		}

		public StateMachine<BansheeChore.States, BansheeChore.StatesInstance, BansheeChore, object>.TargetParameter wailer;

		public StateMachine<BansheeChore.States, BansheeChore.StatesInstance, BansheeChore, object>.IntParameter targetWailLocation;

		public GameStateMachine<BansheeChore.States, BansheeChore.StatesInstance, BansheeChore, object>.State findAudience;

		public GameStateMachine<BansheeChore.States, BansheeChore.StatesInstance, BansheeChore, object>.State moveToAudience;

		public BansheeChore.States.Wail wail;

		public GameStateMachine<BansheeChore.States, BansheeChore.StatesInstance, BansheeChore, object>.State recover;

		public GameStateMachine<BansheeChore.States, BansheeChore.StatesInstance, BansheeChore, object>.State delay;

		public GameStateMachine<BansheeChore.States, BansheeChore.StatesInstance, BansheeChore, object>.State wander;

		public GameStateMachine<BansheeChore.States, BansheeChore.StatesInstance, BansheeChore, object>.State complete;

		private Effect wailPreEffect;

		private Effect wailRecoverEffect;

		public class Wail : GameStateMachine<BansheeChore.States, BansheeChore.StatesInstance, BansheeChore, object>.State
		{
			public GameStateMachine<BansheeChore.States, BansheeChore.StatesInstance, BansheeChore, object>.State pre;

			public GameStateMachine<BansheeChore.States, BansheeChore.StatesInstance, BansheeChore, object>.State loop;

			public GameStateMachine<BansheeChore.States, BansheeChore.StatesInstance, BansheeChore, object>.State pst;
		}
	}
}
