using System;
using UnityEngine;

public class LargeImpactorStatus : GameStateMachine<LargeImpactorStatus, LargeImpactorStatus.Instance, IStateMachineTarget, LargeImpactorStatus.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.alive;
		this.alive.ParamTransition<bool>(this.HasArrived, this.landing, GameStateMachine<LargeImpactorStatus, LargeImpactorStatus.Instance, IStateMachineTarget, LargeImpactorStatus.Def>.IsTrue).ParamTransition<int>(this.Health, this.destroyed, GameStateMachine<LargeImpactorStatus, LargeImpactorStatus.Instance, IStateMachineTarget, LargeImpactorStatus.Def>.IsZero_Int).EventHandler(GameHashes.MissileDamageEncountered, new GameStateMachine<LargeImpactorStatus, LargeImpactorStatus.Instance, IStateMachineTarget, LargeImpactorStatus.Def>.GameEvent.Callback(LargeImpactorStatus.HandleIncommingDamage))
			.ToggleStatusItem(Db.Get().MiscStatusItems.ImpactorHealth, null)
			.EventTransition(GameHashes.ClusterDestinationReached, this.landing, null)
			.UpdateTransition(this.landing, new Func<LargeImpactorStatus.Instance, float, bool>(LargeImpactorStatus.CheckArrivalUpdate), UpdateRate.SIM_200ms, false);
		this.landing.Enter(new StateMachine<LargeImpactorStatus, LargeImpactorStatus.Instance, IStateMachineTarget, LargeImpactorStatus.Def>.State.Callback(LargeImpactorStatus.SetHasArrived)).TriggerOnEnter(GameHashes.LargeImpactorArrived, null);
		this.destroyed.TriggerOnEnter(GameHashes.Died, null);
	}

	private static void HandleIncommingDamage(LargeImpactorStatus.Instance smi, object obj)
	{
		LargeImpactorStatus.DealDamage(smi, (obj as MissileLongRangeConfig.DamageEventPayload).damage);
	}

	private static void SetHasArrived(LargeImpactorStatus.Instance smi)
	{
		smi.sm.HasArrived.Set(true, smi, false);
	}

	private static void DealDamage(LargeImpactorStatus.Instance smi, int damage)
	{
		smi.DealDamage(damage);
	}

	private static void DeleteObject(LargeImpactorStatus.Instance smi)
	{
		smi.gameObject.DeleteObject();
	}

	private static bool CheckArrivalUpdate(LargeImpactorStatus.Instance smi, float dt)
	{
		return smi.TimeRemainingBeforeCollision <= 0f;
	}

	public StateMachine<LargeImpactorStatus, LargeImpactorStatus.Instance, IStateMachineTarget, LargeImpactorStatus.Def>.IntParameter Health;

	public StateMachine<LargeImpactorStatus, LargeImpactorStatus.Instance, IStateMachineTarget, LargeImpactorStatus.Def>.BoolParameter HasArrived;

	public GameStateMachine<LargeImpactorStatus, LargeImpactorStatus.Instance, IStateMachineTarget, LargeImpactorStatus.Def>.State alive;

	public GameStateMachine<LargeImpactorStatus, LargeImpactorStatus.Instance, IStateMachineTarget, LargeImpactorStatus.Def>.State landing;

	public GameStateMachine<LargeImpactorStatus, LargeImpactorStatus.Instance, IStateMachineTarget, LargeImpactorStatus.Def>.State destroyed;

	public class Def : StateMachine.BaseDef
	{
		public int MAX_HEALTH;

		public string EventID;
	}

	public new class Instance : GameStateMachine<LargeImpactorStatus, LargeImpactorStatus.Instance, IStateMachineTarget, LargeImpactorStatus.Def>.GameInstance
	{
		public int Health
		{
			get
			{
				return base.sm.Health.Get(this);
			}
		}

		public float ArrivalTime
		{
			get
			{
				if (!(this.clusterTraveler == null))
				{
					return this.ArrivalTime_SO;
				}
				return this.ArrivalTime_Vanilla;
			}
		}

		public float TimeRemainingBeforeCollision
		{
			get
			{
				if (!(this.clusterTraveler == null))
				{
					return this.TimeRemainingBeforeCollision_SO;
				}
				return this.TimeRemainingBeforeCollision_Vanilla;
			}
		}

		private float ArrivalTime_Vanilla
		{
			get
			{
				return this.eventInstance.eventStartTime * 600f + LargeImpactorEvent.GetImpactTime();
			}
		}

		private float TimeRemainingBeforeCollision_Vanilla
		{
			get
			{
				return Mathf.Clamp(this.ArrivalTime_Vanilla - GameUtil.GetCurrentTimeInCycles() * 600f, 0f, float.MaxValue);
			}
		}

		private float ArrivalTime_SO
		{
			get
			{
				return GameUtil.GetCurrentTimeInCycles() * 600f + this.TimeRemainingBeforeCollision_SO;
			}
		}

		private float TimeRemainingBeforeCollision_SO
		{
			get
			{
				return Mathf.Clamp(this.clusterTraveler.EstimatedTimeToReachDestination(), 0f, float.MaxValue);
			}
		}

		public Instance(IStateMachineTarget master, LargeImpactorStatus.Def def)
			: base(master, def)
		{
			base.sm.Health.Set(def.MAX_HEALTH, base.smi, false);
		}

		public override void StartSM()
		{
			this.clusterTraveler = base.GetComponent<ClusterTraveler>();
			this.eventInstance = GameplayEventManager.Instance.GetGameplayEventInstance(base.def.EventID, -1);
			base.StartSM();
		}

		public void DealDamage(int damage)
		{
			int num = Mathf.Clamp(this.Health - damage, 0, base.def.MAX_HEALTH);
			base.sm.Health.Set(num, this, false);
			Action<int> onDamaged = this.OnDamaged;
			if (onDamaged == null)
			{
				return;
			}
			onDamaged(this.Health);
		}

		public Action<int> OnDamaged;

		private ClusterTraveler clusterTraveler;

		private GameplayEventInstance eventInstance;
	}
}
