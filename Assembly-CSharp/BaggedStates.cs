using System;
using KSerialization;
using STRINGS;
using UnityEngine;

public class BaggedStates : GameStateMachine<BaggedStates, BaggedStates.Instance, IStateMachineTarget, BaggedStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.bagged;
		base.serializable = true;
		this.root.ToggleStatusItem(CREATURES.STATUSITEMS.BAGGED.NAME, CREATURES.STATUSITEMS.BAGGED.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		this.bagged.Enter(new StateMachine<BaggedStates, BaggedStates.Instance, IStateMachineTarget, BaggedStates.Def>.State.Callback(BaggedStates.BagStart)).ToggleTag(GameTags.Creatures.Deliverable).PlayAnim("trussed", KAnim.PlayMode.Loop)
			.TagTransition(GameTags.Creatures.Bagged, null, true)
			.Transition(this.escape, new StateMachine<BaggedStates, BaggedStates.Instance, IStateMachineTarget, BaggedStates.Def>.Transition.ConditionCallback(BaggedStates.ShouldEscape), UpdateRate.SIM_4000ms)
			.EventHandler(GameHashes.OnStore, new StateMachine<BaggedStates, BaggedStates.Instance, IStateMachineTarget, BaggedStates.Def>.State.Callback(BaggedStates.OnStore))
			.Exit(new StateMachine<BaggedStates, BaggedStates.Instance, IStateMachineTarget, BaggedStates.Def>.State.Callback(BaggedStates.BagEnd));
		this.escape.Enter(new StateMachine<BaggedStates, BaggedStates.Instance, IStateMachineTarget, BaggedStates.Def>.State.Callback(BaggedStates.Unbag)).PlayAnim("escape").OnAnimQueueComplete(null);
	}

	private static void BagStart(BaggedStates.Instance smi)
	{
		if (smi.baggedTime == 0f)
		{
			smi.baggedTime = GameClock.Instance.GetTime();
		}
		smi.UpdateFaller(true);
	}

	private static void BagEnd(BaggedStates.Instance smi)
	{
		smi.baggedTime = 0f;
		smi.UpdateFaller(false);
	}

	private static void Unbag(BaggedStates.Instance smi)
	{
		Baggable component = smi.gameObject.GetComponent<Baggable>();
		if (component)
		{
			component.Free();
		}
	}

	private static void OnStore(BaggedStates.Instance smi)
	{
		smi.UpdateFaller(true);
	}

	private static bool ShouldEscape(BaggedStates.Instance smi)
	{
		return !smi.gameObject.HasTag(GameTags.Stored) && GameClock.Instance.GetTime() - smi.baggedTime >= smi.def.escapeTime;
	}

	public GameStateMachine<BaggedStates, BaggedStates.Instance, IStateMachineTarget, BaggedStates.Def>.State bagged;

	public GameStateMachine<BaggedStates, BaggedStates.Instance, IStateMachineTarget, BaggedStates.Def>.State escape;

	public class Def : StateMachine.BaseDef
	{
		public float escapeTime = 300f;
	}

	public new class Instance : GameStateMachine<BaggedStates, BaggedStates.Instance, IStateMachineTarget, BaggedStates.Def>.GameInstance
	{
		public Instance(Chore<BaggedStates.Instance> chore, BaggedStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(BaggedStates.Instance.IsBagged, null);
		}

		public void UpdateFaller(bool bagged)
		{
			bool flag = bagged && !base.gameObject.HasTag(GameTags.Stored);
			bool flag2 = GameComps.Fallers.Has(base.gameObject);
			if (flag != flag2)
			{
				if (flag)
				{
					GameComps.Fallers.Add(base.gameObject, Vector2.zero);
					return;
				}
				GameComps.Fallers.Remove(base.gameObject);
			}
		}

		[Serialize]
		public float baggedTime;

		public static readonly Chore.Precondition IsBagged = new Chore.Precondition
		{
			id = "IsBagged",
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				return context.consumerState.prefabid.HasTag(GameTags.Creatures.Bagged);
			}
		};
	}
}
