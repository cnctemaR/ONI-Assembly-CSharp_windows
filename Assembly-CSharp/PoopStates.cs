using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class PoopStates : GameStateMachine<PoopStates, PoopStates.Instance, IStateMachineTarget, PoopStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.assess;
		this.assess.Enter(new StateMachine<PoopStates, PoopStates.Instance, IStateMachineTarget, PoopStates.Def>.State.Callback(PoopStates.FindPoopStation)).EnterTransition(this.stationPoop, new StateMachine<PoopStates, PoopStates.Instance, IStateMachineTarget, PoopStates.Def>.Transition.ConditionCallback(PoopStates.AttemptToReservePoopStation)).EnterTransition(this.complain, new StateMachine<PoopStates, PoopStates.Instance, IStateMachineTarget, PoopStates.Def>.Transition.ConditionCallback(PoopStates.IsThereAPoopStationNearby))
			.EnterGoTo(this.wildPoop);
		this.complain.DefaultState(this.complain.complain);
		this.complain.complain.ParamTransition<int>(this.RemainingAttempts, this.wildPoop, GameStateMachine<PoopStates, PoopStates.Instance, IStateMachineTarget, PoopStates.Def>.IsLTEZero_int).EnterTransition(this.complain.end, new StateMachine<PoopStates, PoopStates.Instance, IStateMachineTarget, PoopStates.Def>.Transition.ConditionCallback(PoopStates.CanNotComplain)).ToggleAnims((PoopStates.Instance smi) => smi.def.emoteAnimFile)
			.Enter(new StateMachine<PoopStates, PoopStates.Instance, IStateMachineTarget, PoopStates.Def>.State.Callback(PoopStates.DisplayThoughtBubble))
			.PlayAnim("react_neg")
			.OnAnimQueueComplete(this.complain.end)
			.Exit(new StateMachine<PoopStates, PoopStates.Instance, IStateMachineTarget, PoopStates.Def>.State.Callback(PoopStates.ClearThoughtBubble));
		this.complain.end.Enter(new StateMachine<PoopStates, PoopStates.Instance, IStateMachineTarget, PoopStates.Def>.State.Callback(PoopStates.ConsumeAttempt)).EnterGoTo(null);
		GameStateMachine<PoopStates, PoopStates.Instance, IStateMachineTarget, PoopStates.Def>.State state = this.stationPoop.ParamTransition<GameObject>(this.PoopStation, this.wildPoop, GameStateMachine<PoopStates, PoopStates.Instance, IStateMachineTarget, PoopStates.Def>.IsNull);
		string text = "Unused";
		string text2 = "Unused";
		string text3 = "";
		StatusItem.IconType iconType = StatusItem.IconType.Info;
		NotificationType notificationType = NotificationType.Neutral;
		bool flag = false;
		StatusItemCategory statusItemCategory = Db.Get().StatusItemCategories.Main;
		state.ToggleStatusItem(text, text2, text3, iconType, notificationType, flag, default(HashedString), 129022, (string str, PoopStates.Instance smi) => smi.def.statusItemName, (string str, PoopStates.Instance smi) => smi.def.statusItemTooltip, statusItemCategory).DefaultState(this.stationPoop.approachPoopSpot).EventHandlerTransition(GameHashes.PoopStationUpdate, this.wildPoop, new Func<PoopStates.Instance, object, bool>(PoopStates.IsPoopStationStillValid))
			.Exit(new StateMachine<PoopStates, PoopStates.Instance, IStateMachineTarget, PoopStates.Def>.State.Callback(PoopStates.ClearReservationFromPoopStation));
		this.stationPoop.approachPoopSpot.MoveTo(new Func<PoopStates.Instance, int>(PoopStates.GetPoopStationCell), this.stationPoop.pooping, this.wildPoop, false);
		this.stationPoop.pooping.DefaultState(this.stationPoop.pooping.pre);
		this.stationPoop.pooping.pre.EnterTransition(this.stationPoop.pooping.loop, (PoopStates.Instance smi) => PoopStates.GetPoopStationPoop_PRE_AnimName(smi) == null).Enter(delegate(PoopStates.Instance smi)
		{
			PoopStates.PlayAnimOnStation(smi, PoopStates.GetPoopStationPoop_PRE_AnimName(smi), KAnim.PlayMode.Once);
		}).PlayAnim(new Func<PoopStates.Instance, string>(PoopStates.GetPoopStationPoop_PRE_AnimName), KAnim.PlayMode.Once)
			.OnAnimQueueComplete(this.stationPoop.pooping.loop);
		this.stationPoop.pooping.loop.EnterTransition(this.stationPoop.pooping.pst, (PoopStates.Instance smi) => PoopStates.GetPoopStationPoop_LOOP_AnimName(smi) == null).Enter(delegate(PoopStates.Instance smi)
		{
			PoopStates.PlayAnimOnStation(smi, PoopStates.GetPoopStationPoop_LOOP_AnimName(smi), KAnim.PlayMode.Loop);
		}).PlayAnim(new Func<PoopStates.Instance, string>(PoopStates.GetPoopStationPoop_LOOP_AnimName), KAnim.PlayMode.Loop)
			.ScheduleGoTo(5f, this.stationPoop.pooping.pst);
		this.stationPoop.pooping.pst.EnterTransition(this.stationPoop.end, (PoopStates.Instance smi) => PoopStates.GetPoopStationPoop_PST_AnimName(smi) == null).Enter(delegate(PoopStates.Instance smi)
		{
			PoopStates.PlayAnimOnStation(smi, PoopStates.GetPoopStationPoop_PST_AnimName(smi), KAnim.PlayMode.Once);
		}).PlayAnim(new Func<PoopStates.Instance, string>(PoopStates.GetPoopStationPoop_PST_AnimName), KAnim.PlayMode.Once)
			.OnAnimQueueComplete(this.stationPoop.end);
		this.stationPoop.end.PlayAnim("idle_loop", KAnim.PlayMode.Loop).Enter(new StateMachine<PoopStates, PoopStates.Instance, IStateMachineTarget, PoopStates.Def>.State.Callback(PoopStates.ResetAttempts)).TriggerOnEnter(GameHashes.PoopStatesCompleted, new Func<PoopStates.Instance, object>(PoopStates.GetPoopData))
			.BehaviourComplete(GameTags.Creatures.Poop, false);
		GameStateMachine<PoopStates, PoopStates.Instance, IStateMachineTarget, PoopStates.Def>.State state2 = this.wildPoop;
		string text4 = "Unused";
		string text5 = "Unused";
		string text6 = "";
		StatusItem.IconType iconType2 = StatusItem.IconType.Info;
		NotificationType notificationType2 = NotificationType.Neutral;
		bool flag2 = false;
		statusItemCategory = Db.Get().StatusItemCategories.Main;
		state2.ToggleStatusItem(text4, text5, text6, iconType2, notificationType2, flag2, default(HashedString), 129022, (string str, PoopStates.Instance smi) => smi.def.statusItemName, (string str, PoopStates.Instance smi) => smi.def.statusItemTooltip, statusItemCategory).DefaultState(this.wildPoop.pooping);
		this.wildPoop.pooping.PlayAnim("poop", KAnim.PlayMode.Once).OnAnimQueueComplete(this.wildPoop.end);
		this.wildPoop.end.PlayAnim("idle_loop", KAnim.PlayMode.Loop).Enter(new StateMachine<PoopStates, PoopStates.Instance, IStateMachineTarget, PoopStates.Def>.State.Callback(PoopStates.ResetAttempts)).TriggerOnEnter(GameHashes.PoopStatesCompleted, (PoopStates.Instance smi) => null)
			.BehaviourComplete(GameTags.Creatures.Poop, false);
	}

	private static void DisplayThoughtBubble(PoopStates.Instance smi)
	{
		global::Tuple<Sprite, Color> uisprite = global::Def.GetUISprite(smi.PoopStationObject, "ui", false);
		NameDisplayScreen.Instance.SetThoughtBubbleDisplay(smi.gameObject, true, "", Assets.GetSprite("bubble_alert"), uisprite.first);
	}

	private static void ClearThoughtBubble(PoopStates.Instance smi)
	{
		NameDisplayScreen.Instance.SetThoughtBubbleDisplay(smi.gameObject, false, null, null, null);
	}

	private static int GetPoopStationCell(PoopStates.Instance smi)
	{
		return smi.GetPoopStationCell();
	}

	private static bool IsThereAPoopStationNearby(PoopStates.Instance smi)
	{
		return smi.PoopStationObject != null;
	}

	private static bool AttemptToReservePoopStation(PoopStates.Instance smi)
	{
		return smi.AttemptToReservePoopStation();
	}

	private static bool IsPoopStationStillValid(PoopStates.Instance smi, object o)
	{
		return smi.IsPoopStationStillValid();
	}

	private static bool CanNotComplain(PoopStates.Instance smi)
	{
		return !smi.def.canComplain;
	}

	private static void ConsumeAttempt(PoopStates.Instance smi)
	{
		smi.ConsumeAttempt();
	}

	private static void ResetAttempts(PoopStates.Instance smi)
	{
		smi.ResetAttempt();
	}

	private static void FindPoopStation(PoopStates.Instance smi)
	{
		smi.FindPoopStation();
	}

	private static void PlayAnimOnStation(PoopStates.Instance smi, string animName, KAnim.PlayMode playmode)
	{
		smi.PlayAnimOnStation(animName, playmode);
	}

	private static void ClearReservationFromPoopStation(PoopStates.Instance smi)
	{
		smi.ClearReservationFromPoopStation();
	}

	private static PoopData GetPoopData(PoopStates.Instance smi)
	{
		return smi.GetPoopData();
	}

	private static string GetPoopStationPoop_PRE_AnimName(PoopStates.Instance smi)
	{
		string text = smi.GetPoopStationAnimName(0);
		if (text == null)
		{
			text = "poop";
		}
		return text;
	}

	private static string GetPoopStationPoop_LOOP_AnimName(PoopStates.Instance smi)
	{
		return smi.GetPoopStationAnimName(1);
	}

	private static string GetPoopStationPoop_PST_AnimName(PoopStates.Instance smi)
	{
		return smi.GetPoopStationAnimName(2);
	}

	public const float POOP_LOOP_DURATION = 5f;

	public const float ATTEMPT_COOLDOWN = 10f;

	public const int ATTEMPT_TIMES = 3;

	public const string POOP_ANIM_NAME = "poop";

	public const string IDLE_ANIM_NAME = "idle_loop";

	public const string COMPLAIN_ANIM_NAME = "react_neg";

	public const string WAITING_ANIM_NAME = "idle_loop";

	public static Chore.Precondition IsInCooldownPrecondition = new Chore.Precondition
	{
		id = "IsPoopStateInCooldown",
		sortOrder = 1,
		description = DUPLICANTS.CHORES.PRECONDITIONS.IS_POOP_COOLDOWN,
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return !((PoopStates.Instance)data).IsInCooldown;
		}
	};

	public GameStateMachine<PoopStates, PoopStates.Instance, IStateMachineTarget, PoopStates.Def>.State assess;

	public PoopStates.ComplainState complain;

	public PoopStates.PoopOnStationState stationPoop;

	public PoopStates.WildPoopState wildPoop;

	public StateMachine<PoopStates, PoopStates.Instance, IStateMachineTarget, PoopStates.Def>.IntParameter RemainingAttempts = new StateMachine<PoopStates, PoopStates.Instance, IStateMachineTarget, PoopStates.Def>.IntParameter(3);

	public StateMachine<PoopStates, PoopStates.Instance, IStateMachineTarget, PoopStates.Def>.TargetParameter PoopStation;

	public class Def : StateMachine.BaseDef
	{
		public Def(KAnimFile emoteAnimFile, string status_item_name, string status_item_tooltip, bool canComplain)
		{
			this.canComplain = canComplain;
			this.emoteAnimFile = emoteAnimFile;
			this.statusItemName = status_item_name;
			this.statusItemTooltip = status_item_tooltip;
		}

		public KAnimFile emoteAnimFile;

		public string statusItemName;

		public string statusItemTooltip;

		public bool canComplain;
	}

	public class PoopOnStationState : GameStateMachine<PoopStates, PoopStates.Instance, IStateMachineTarget, PoopStates.Def>.State
	{
		public GameStateMachine<PoopStates, PoopStates.Instance, IStateMachineTarget, PoopStates.Def>.State approachPoopSpot;

		public GameStateMachine<PoopStates, PoopStates.Instance, IStateMachineTarget, PoopStates.Def>.PreLoopPostState pooping;

		public GameStateMachine<PoopStates, PoopStates.Instance, IStateMachineTarget, PoopStates.Def>.State end;
	}

	public class ComplainState : GameStateMachine<PoopStates, PoopStates.Instance, IStateMachineTarget, PoopStates.Def>.State
	{
		public GameStateMachine<PoopStates, PoopStates.Instance, IStateMachineTarget, PoopStates.Def>.State complain;

		public GameStateMachine<PoopStates, PoopStates.Instance, IStateMachineTarget, PoopStates.Def>.State end;
	}

	public class WildPoopState : GameStateMachine<PoopStates, PoopStates.Instance, IStateMachineTarget, PoopStates.Def>.State
	{
		public GameStateMachine<PoopStates, PoopStates.Instance, IStateMachineTarget, PoopStates.Def>.State pooping;

		public GameStateMachine<PoopStates, PoopStates.Instance, IStateMachineTarget, PoopStates.Def>.State end;
	}

	public new class Instance : GameStateMachine<PoopStates, PoopStates.Instance, IStateMachineTarget, PoopStates.Def>.GameInstance
	{
		public bool IsInCooldown
		{
			get
			{
				return this.TimePassedSinceLastAttempt < 10f;
			}
		}

		public float TimePassedSinceLastAttempt
		{
			get
			{
				return Time.time - this.lastTimeWeAttemptedToGo;
			}
		}

		public GameObject PoopStationObject
		{
			get
			{
				return base.sm.PoopStation.Get(this);
			}
		}

		public IPoopStation PoopStation
		{
			get
			{
				if (this.PoopStationObject == null)
				{
					return null;
				}
				IPoopStation component = this.PoopStationObject.GetComponent<IPoopStation>();
				return (component == null) ? this.PoopStationObject.GetSMI<IPoopStation>() : component;
			}
		}

		public Instance(Chore<PoopStates.Instance> chore, PoopStates.Def def)
			: base(chore, def)
		{
			this.prefabID = base.GetComponent<KPrefabID>();
			this.navigator = base.GetComponent<Navigator>();
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Poop);
			chore.AddPrecondition(PoopStates.IsInCooldownPrecondition, this);
		}

		public void ConsumeAttempt()
		{
			this.lastTimeWeAttemptedToGo = Time.time;
			base.sm.RemainingAttempts.Set(base.sm.RemainingAttempts.Get(base.smi) - 1, this, false);
		}

		public void ResetAttempt()
		{
			this.lastTimeWeAttemptedToGo = -10f;
			base.sm.RemainingAttempts.Set(3, this, false);
		}

		public int GetPoopStationCell()
		{
			if (this.PoopStationObject == null)
			{
				return Grid.InvalidCell;
			}
			return Grid.PosToCell(this.PoopStationObject);
		}

		public bool IsPoopStationStillValid()
		{
			IPoopStation poopStation = this.PoopStation;
			return poopStation != null && poopStation.IsPoopStationOperational() && poopStation.GetCurrentPoopStationUser() == base.gameObject;
		}

		public bool AttemptToReservePoopStation()
		{
			IPoopStation poopStation = this.PoopStation;
			return poopStation != null && poopStation.AttemptToReservePoopStation(base.gameObject);
		}

		public bool IsPoopStationOperational()
		{
			IPoopStation poopStation = this.PoopStation;
			return poopStation != null && poopStation.IsPoopStationOperational();
		}

		public void ClearReservationFromPoopStation()
		{
			IPoopStation poopStation = this.PoopStation;
			if (poopStation == null)
			{
				return;
			}
			poopStation.ClearPoopStationUser(base.gameObject);
		}

		public void PlayAnimOnStation(string animName, KAnim.PlayMode playMode)
		{
			IPoopStation poopStation = this.PoopStation;
			if (poopStation == null)
			{
				return;
			}
			poopStation.PlayPoopStationAnim(animName, playMode);
		}

		public string GetPoopStationAnimName(int index)
		{
			IPoopStation poopStation = this.PoopStation;
			if (poopStation == null)
			{
				return null;
			}
			string[] poopingAnimNames = poopStation.GetPoopingAnimNames();
			if (poopingAnimNames == null || index >= poopingAnimNames.Length)
			{
				return null;
			}
			return poopingAnimNames[index];
		}

		public PoopData GetPoopData()
		{
			IPoopStation poopStation = this.PoopStation;
			if (poopStation == null)
			{
				return null;
			}
			return poopStation.GetPoopData();
		}

		public void FindPoopStation()
		{
			IPoopStation poopStation = null;
			bool flag = false;
			int num = ((this.navigator == null) ? 32 : this.navigator.maxProbeRadiusX);
			int myWorldId = base.gameObject.GetMyWorldId();
			int num2 = Grid.PosToCell(base.gameObject);
			List<IPoopStation> items = Components.PoopStations.GetItems(myWorldId);
			int num3 = -1;
			float num4 = -1f;
			foreach (IPoopStation poopStation2 in items)
			{
				if (poopStation2.IsUserCompatibleWithPoopStation(this.prefabID))
				{
					bool flag2 = poopStation2.IsPoopStationOperational();
					int num5 = Grid.PosToCell(poopStation2.GetPoopStationObject());
					if (Grid.GetCellDistance(num2, num5) <= num && flag2)
					{
						int navigationCost = this.navigator.GetNavigationCost(num5);
						if (navigationCost != -1)
						{
							float availablePoopCapacityPercentage = poopStation2.GetAvailablePoopCapacityPercentage();
							bool flag3 = availablePoopCapacityPercentage > num4;
							if (num3 == -1 || flag3 || (navigationCost < num3 && availablePoopCapacityPercentage == num4))
							{
								GameObject currentPoopStationUser = poopStation2.GetCurrentPoopStationUser();
								bool flag4 = currentPoopStationUser == null || currentPoopStationUser == base.gameObject;
								if (poopStation == null || !flag || flag4)
								{
									num3 = navigationCost;
									poopStation = poopStation2;
									num4 = availablePoopCapacityPercentage;
									flag = flag4;
								}
							}
						}
					}
				}
			}
			base.sm.PoopStation.Set((poopStation == null) ? null : poopStation.GetPoopStationObject(), this, false);
		}

		private KPrefabID prefabID;

		private Navigator navigator;

		private float lastTimeWeAttemptedToGo = -10f;
	}
}
