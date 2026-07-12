using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class BionicWaterDamageMonitor : GameStateMachine<BionicWaterDamageMonitor, BionicWaterDamageMonitor.Instance, IStateMachineTarget, BionicWaterDamageMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.safe;
		this.safe.Transition(this.threat, new StateMachine<BionicWaterDamageMonitor, BionicWaterDamageMonitor.Instance, IStateMachineTarget, BionicWaterDamageMonitor.Def>.Transition.ConditionCallback(BionicWaterDamageMonitor.IsThreatened), UpdateRate.SIM_200ms);
		this.threat.Transition(this.safe, GameStateMachine<BionicWaterDamageMonitor, BionicWaterDamageMonitor.Instance, IStateMachineTarget, BionicWaterDamageMonitor.Def>.Not(new StateMachine<BionicWaterDamageMonitor, BionicWaterDamageMonitor.Instance, IStateMachineTarget, BionicWaterDamageMonitor.Def>.Transition.ConditionCallback(BionicWaterDamageMonitor.IsThreatened)), UpdateRate.SIM_200ms).Update(new Action<BionicWaterDamageMonitor.Instance, float>(BionicWaterDamageMonitor.ApplyDebuff), UpdateRate.SIM_200ms, false).Exit(new StateMachine<BionicWaterDamageMonitor, BionicWaterDamageMonitor.Instance, IStateMachineTarget, BionicWaterDamageMonitor.Def>.State.Callback(BionicWaterDamageMonitor.ClearNotification))
			.DefaultState(this.threat.idle)
			.ToggleAnims("anim_bionic_hits_kanim", 3f);
		this.threat.idle.ParamTransition<float>(this.DamageCooldown, this.threat.damage, new StateMachine<BionicWaterDamageMonitor, BionicWaterDamageMonitor.Instance, IStateMachineTarget, BionicWaterDamageMonitor.Def>.Parameter<float>.Callback(BionicWaterDamageMonitor.IsTimeToZap)).ToggleReactable(new Func<BionicWaterDamageMonitor.Instance, Reactable>(BionicWaterDamageMonitor.ZapReactable)).Update(new Action<BionicWaterDamageMonitor.Instance, float>(BionicWaterDamageMonitor.DamageTimerUpdate), UpdateRate.SIM_200ms, false)
			.Exit(new StateMachine<BionicWaterDamageMonitor, BionicWaterDamageMonitor.Instance, IStateMachineTarget, BionicWaterDamageMonitor.Def>.State.Callback(BionicWaterDamageMonitor.ResetDamageCooldown));
		this.threat.damage.Enter(new StateMachine<BionicWaterDamageMonitor, BionicWaterDamageMonitor.Instance, IStateMachineTarget, BionicWaterDamageMonitor.Def>.State.Callback(BionicWaterDamageMonitor.ApplyDamage)).Enter(new StateMachine<BionicWaterDamageMonitor, BionicWaterDamageMonitor.Instance, IStateMachineTarget, BionicWaterDamageMonitor.Def>.State.Callback(BionicWaterDamageMonitor.PlayDamageNotification)).GoTo(this.threat.idle)
			.DoNotification((BionicWaterDamageMonitor.Instance smi) => smi.CreateLiquidDamageNotification());
	}

	private static Reactable ZapReactable(BionicWaterDamageMonitor.Instance smi)
	{
		return smi.GetReactable();
	}

	private static bool IsThreatened(BionicWaterDamageMonitor.Instance smi)
	{
		return BionicWaterDamageMonitor.IsFloorWetWithIntolerantSubstance(smi);
	}

	private static bool IsTimeToZap(BionicWaterDamageMonitor.Instance smi, float time)
	{
		return time > smi.def.ZapInterval;
	}

	private static void ApplyDebuff(BionicWaterDamageMonitor.Instance smi, float dt)
	{
		smi.effects.Add("WaterDamage", true);
	}

	private static void ApplyDamage(BionicWaterDamageMonitor.Instance smi)
	{
		smi.ApplyDamage();
	}

	private static void ResetDamageCooldown(BionicWaterDamageMonitor.Instance smi)
	{
		smi.sm.DamageCooldown.Set(0f, smi, false);
	}

	private static void PlayDamageNotification(BionicWaterDamageMonitor.Instance smi)
	{
		smi.PlayDamageNotification();
	}

	private static void ClearNotification(BionicWaterDamageMonitor.Instance smi)
	{
		smi.ClearNotification(null);
	}

	private static void DamageTimerUpdate(BionicWaterDamageMonitor.Instance smi, float dt)
	{
		float damageCooldown = smi.DamageCooldown;
		smi.sm.DamageCooldown.Set(damageCooldown + dt, smi, false);
	}

	private static bool IsFloorWetWithIntolerantSubstance(BionicWaterDamageMonitor.Instance smi)
	{
		int num = Grid.PosToCell(smi);
		return Grid.IsValidCell(num) && Grid.Element[num].IsLiquid && !smi.kpid.HasTag(GameTags.HasAirtightSuit) && smi.def.IsElementIntolerable(Grid.Element[num].id);
	}

	private static string GetZapAnimName(BionicWaterDamageMonitor.Instance smi)
	{
		if (smi.GetComponent<Navigator>().CurrentNavType != NavType.Ladder)
		{
			return "zapped";
		}
		return "ladder_zapped";
	}

	public const string EFFECT_NAME = "WaterDamage";

	public GameStateMachine<BionicWaterDamageMonitor, BionicWaterDamageMonitor.Instance, IStateMachineTarget, BionicWaterDamageMonitor.Def>.State safe;

	public BionicWaterDamageMonitor.DamageStates threat;

	public StateMachine<BionicWaterDamageMonitor, BionicWaterDamageMonitor.Instance, IStateMachineTarget, BionicWaterDamageMonitor.Def>.FloatParameter DamageCooldown;

	public class Def : StateMachine.BaseDef
	{
		public bool IsElementIntolerable(SimHashes element)
		{
			for (int i = 0; i < this.IntolerantToElements.Length; i++)
			{
				if (this.IntolerantToElements[i] == element)
				{
					return true;
				}
			}
			return false;
		}

		public readonly SimHashes[] IntolerantToElements = new SimHashes[]
		{
			SimHashes.Water,
			SimHashes.DirtyWater,
			SimHashes.SaltWater,
			SimHashes.Brine
		};

		public float DamagePointsTakenPerShock = 20f;

		public float ZapInterval = 15f;
	}

	public class DamageStates : GameStateMachine<BionicWaterDamageMonitor, BionicWaterDamageMonitor.Instance, IStateMachineTarget, BionicWaterDamageMonitor.Def>.State
	{
		public GameStateMachine<BionicWaterDamageMonitor, BionicWaterDamageMonitor.Instance, IStateMachineTarget, BionicWaterDamageMonitor.Def>.State idle;

		public GameStateMachine<BionicWaterDamageMonitor, BionicWaterDamageMonitor.Instance, IStateMachineTarget, BionicWaterDamageMonitor.Def>.State damage;
	}

	public new class Instance : GameStateMachine<BionicWaterDamageMonitor, BionicWaterDamageMonitor.Instance, IStateMachineTarget, BionicWaterDamageMonitor.Def>.GameInstance
	{
		public float DamageCooldown
		{
			get
			{
				return base.sm.DamageCooldown.Get(this);
			}
		}

		public bool IsAffectedByWaterDamage
		{
			get
			{
				return this.effects.HasEffect("WaterDamage");
			}
		}

		public Instance(IStateMachineTarget master, BionicWaterDamageMonitor.Def def)
			: base(master, def)
		{
			this.health = base.GetComponent<Health>();
			this.effects = base.GetComponent<Effects>();
		}

		public void ApplyDamage()
		{
			this.health.Damage(base.def.DamagePointsTakenPerShock);
			int num = Grid.PosToCell(base.gameObject);
			if (Grid.IsValidCell(num))
			{
				this.lastElementDamagedBy = (base.smi.def.IsElementIntolerable(Grid.Element[num].id) ? Grid.Element[num] : null);
			}
		}

		public Reactable GetReactable()
		{
			SelfEmoteReactable selfEmoteReactable = new SelfEmoteReactable(base.master.gameObject, Db.Get().Emotes.Minion.WaterDamage.Id, Db.Get().ChoreTypes.WaterDamageZap, 0f, base.def.ZapInterval, float.PositiveInfinity, 0f);
			Emote waterDamage = Db.Get().Emotes.Minion.WaterDamage;
			selfEmoteReactable.SetEmote(waterDamage);
			selfEmoteReactable.preventChoreInterruption = true;
			return selfEmoteReactable;
		}

		public Notification CreateLiquidDamageNotification()
		{
			KSelectable component = base.GetComponent<KSelectable>();
			return new Notification(MISC.NOTIFICATIONS.BIONICLIQUIDDAMAGE.NAME, NotificationType.Bad, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.BIONICLIQUIDDAMAGE.TOOLTIP + notificationList.ReduceMessages(false), "/t• " + component.GetProperName(), true, 0f, new Notification.ClickCallback(this.OnNotificationClicked), base.gameObject, null, true, false, false);
		}

		private void OnNotificationClicked(object data)
		{
			if (data != null)
			{
				GameObject gameObject = (GameObject)data;
				if (gameObject != null)
				{
					GameUtil.FocusCamera(gameObject.transform, true);
				}
			}
		}

		public void ClearNotification(Notifier _notifier = null)
		{
			Notifier notifier = ((_notifier == null) ? base.gameObject.AddOrGet<Notifier>() : _notifier);
			if (this.lastNotificationPlayed != null)
			{
				notifier.Remove(this.lastNotificationPlayed);
				this.lastNotificationPlayed = null;
			}
		}

		public void PlayDamageNotification()
		{
			Notifier notifier = base.gameObject.AddOrGet<Notifier>();
			this.ClearNotification(notifier);
			Notification notification = this.CreateLiquidDamageNotification();
			notifier.Add(notification, "");
			this.lastNotificationPlayed = notification;
		}

		public Effects effects;

		private Health health;

		public Element lastElementDamagedBy;

		private Notification lastNotificationPlayed;

		[MyCmpGet]
		public KPrefabID kpid;
	}
}
