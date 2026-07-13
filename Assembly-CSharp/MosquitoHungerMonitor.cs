using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class MosquitoHungerMonitor : StateMachineComponent<MosquitoHungerMonitor.Instance>
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	private static void ClearTarget(MosquitoHungerMonitor.Instance smi)
	{
		smi.sm.victim.Set(null, smi);
	}

	public static bool IsFed(MosquitoHungerMonitor.Instance smi)
	{
		return smi.IsFed;
	}

	public static bool HasValidVictim(MosquitoHungerMonitor.Instance smi)
	{
		return MosquitoHungerMonitor.HasValidVictim(smi, smi.Victim);
	}

	public static bool HasValidVictim(MosquitoHungerMonitor.Instance smi, GameObject victimParam)
	{
		return victimParam != null && !MosquitoHungerMonitor.IsVictimForbidden(smi, victimParam.GetComponent<KPrefabID>(), true);
	}

	public static void LookForVictim(MosquitoHungerMonitor.Instance smi)
	{
		CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell(smi));
		if (cavityForCell == null)
		{
			return;
		}
		int myWorldId = smi.GetMyWorldId();
		List<KPrefabID> list = new List<KPrefabID>();
		if (smi.master.CanBiteMinions)
		{
			List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(myWorldId, false);
			for (int i = 0; i < worldItems.Count; i++)
			{
				KPrefabID component = worldItems[i].GetComponent<KPrefabID>();
				if (!MosquitoHungerMonitor.IsVictimForbidden(smi, component, true))
				{
					list.Add(component);
				}
			}
		}
		for (int j = 0; j < cavityForCell.creatures.Count; j++)
		{
			KPrefabID kprefabID = cavityForCell.creatures[j];
			if (kprefabID.HasAnyTags(smi.master.AllowedTargetTags) && !MosquitoHungerMonitor.IsVictimForbidden(smi, kprefabID, false))
			{
				list.Add(kprefabID);
			}
		}
		KPrefabID kprefabID2 = ((list.Count > 0) ? list.GetRandom<KPrefabID>() : null);
		smi.sm.victim.Set(kprefabID2, smi);
	}

	private static bool IsVictimForbidden(MosquitoHungerMonitor.Instance smi, KPrefabID victim, bool mustBeInSameCavity = false)
	{
		int num = Grid.PosToCell(victim);
		if (mustBeInSameCavity)
		{
			CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell(smi));
			if (Game.Instance.roomProber.GetCavityForCell(num) != cavityForCell)
			{
				return true;
			}
		}
		if (victim.HasAnyTags(smi.master.ForbiddenTargetTags))
		{
			return true;
		}
		Effects component = victim.GetComponent<Effects>();
		if (component.HasEffect("DupeMosquitoBite") || component.HasEffect("CritterMosquitoBite") || component.HasEffect("DupeMosquitoBiteSuppressed") || component.HasEffect("CritterMosquitoBiteSuppressed"))
		{
			return true;
		}
		OccupyArea component2 = victim.GetComponent<OccupyArea>();
		return !smi.navigator.CanReach(num, component2.OccupiedCellsOffsets);
	}

	public static void InitiatePokeBehaviour(MosquitoHungerMonitor.Instance smi)
	{
		PokeMonitor.Instance smi2 = smi.GetSMI<PokeMonitor.Instance>();
		CellOffset[] array = smi.Victim.GetComponent<OccupyArea>().OccupiedCellsOffsets;
		for (int i = 0; i < 1; i++)
		{
			array = array.Expand();
		}
		smi2.InitiatePoke(smi.Victim, array);
	}

	public static void AbortPokeBehaviour(MosquitoHungerMonitor.Instance smi)
	{
		PokeMonitor.Instance smi2 = smi.GetSMI<PokeMonitor.Instance>();
		if (smi2 != null)
		{
			smi2.AbortPoke();
		}
	}

	public static void OnVictimPoked(MosquitoHungerMonitor.Instance smi, object victimOBJ)
	{
		if (victimOBJ == null)
		{
			return;
		}
		GameObject gameObject = (GameObject)victimOBJ;
		Effects component = gameObject.GetComponent<Effects>();
		bool flag = gameObject.HasTag(GameTags.BaseMinion);
		bool flag2 = false;
		foreach (string text in MosquitoHungerMonitor.ImmunityEffectNames)
		{
			flag2 = flag2 || component.HasEffect(text);
		}
		if (flag)
		{
			component.Add(flag2 ? "DupeMosquitoBiteSuppressed" : "DupeMosquitoBite", true);
		}
		else
		{
			component.Add(flag2 ? "CritterMosquitoBiteSuppressed" : "CritterMosquitoBite", true);
		}
		smi.ApplyFedEffect();
	}

	public const string DupeMosquitoBiteEffectName = "DupeMosquitoBite";

	public const string CritterMosquitoBiteEffectName = "CritterMosquitoBite";

	public const string Dupe_SUPPRESSED_MosquitoBiteEffectName = "DupeMosquitoBiteSuppressed";

	public const string Critter_SUPPRESSED_MosquitoBiteEffectName = "CritterMosquitoBiteSuppressed";

	public const string MosquitoFedEffectName = "MosquitoFed";

	public const int ReachabilityPadding = 1;

	public bool CanBiteMinions = true;

	public List<Tag> AllowedTargetTags;

	public List<Tag> ForbiddenTargetTags;

	public static string[] ImmunityEffectNames = new string[] { "HistamineSuppression" };

	public class States : GameStateMachine<MosquitoHungerMonitor.States, MosquitoHungerMonitor.Instance, MosquitoHungerMonitor>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			base.serializable = StateMachine.SerializeType.Never;
			default_state = this.satisfied;
			this.satisfied.EventTransition(GameHashes.EffectRemoved, this.hungry, GameStateMachine<MosquitoHungerMonitor.States, MosquitoHungerMonitor.Instance, MosquitoHungerMonitor, object>.Not(new StateMachine<MosquitoHungerMonitor.States, MosquitoHungerMonitor.Instance, MosquitoHungerMonitor, object>.Transition.ConditionCallback(MosquitoHungerMonitor.IsFed))).Enter(new StateMachine<MosquitoHungerMonitor.States, MosquitoHungerMonitor.Instance, MosquitoHungerMonitor, object>.State.Callback(MosquitoHungerMonitor.ClearTarget));
			this.hungry.EventTransition(GameHashes.EffectAdded, this.satisfied, new StateMachine<MosquitoHungerMonitor.States, MosquitoHungerMonitor.Instance, MosquitoHungerMonitor, object>.Transition.ConditionCallback(MosquitoHungerMonitor.IsFed)).DefaultState(this.hungry.lookingForVictim);
			this.hungry.lookingForVictim.ToggleStatusItem(CREATURES.STATUSITEMS.HUNGRY.NAME, CREATURES.STATUSITEMS.HUNGRY.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, null).ParamTransition<GameObject>(this.victim, this.hungry.chaseVictim, GameStateMachine<MosquitoHungerMonitor.States, MosquitoHungerMonitor.Instance, MosquitoHungerMonitor, object>.IsNotNull).PreBrainUpdate(new Action<MosquitoHungerMonitor.Instance>(MosquitoHungerMonitor.LookForVictim));
			this.hungry.chaseVictim.ParamTransition<GameObject>(this.victim, this.hungry.lookingForVictim, GameStateMachine<MosquitoHungerMonitor.States, MosquitoHungerMonitor.Instance, MosquitoHungerMonitor, object>.IsNull).EventTransition(GameHashes.TargetLost, this.hungry.lookingForVictim, null).Enter(new StateMachine<MosquitoHungerMonitor.States, MosquitoHungerMonitor.Instance, MosquitoHungerMonitor, object>.State.Callback(MosquitoHungerMonitor.InitiatePokeBehaviour))
				.EventHandler(GameHashes.EntityPoked, new GameStateMachine<MosquitoHungerMonitor.States, MosquitoHungerMonitor.Instance, MosquitoHungerMonitor, object>.GameEvent.Callback(MosquitoHungerMonitor.OnVictimPoked))
				.Exit(new StateMachine<MosquitoHungerMonitor.States, MosquitoHungerMonitor.Instance, MosquitoHungerMonitor, object>.State.Callback(MosquitoHungerMonitor.AbortPokeBehaviour))
				.Exit(new StateMachine<MosquitoHungerMonitor.States, MosquitoHungerMonitor.Instance, MosquitoHungerMonitor, object>.State.Callback(MosquitoHungerMonitor.ClearTarget))
				.Target(this.victim)
				.EventTransition(GameHashes.TagsChanged, this.hungry.lookingForVictim, GameStateMachine<MosquitoHungerMonitor.States, MosquitoHungerMonitor.Instance, MosquitoHungerMonitor, object>.Not(new StateMachine<MosquitoHungerMonitor.States, MosquitoHungerMonitor.Instance, MosquitoHungerMonitor, object>.Transition.ConditionCallback(MosquitoHungerMonitor.HasValidVictim)))
				.EventTransition(GameHashes.EffectAdded, this.hungry.lookingForVictim, GameStateMachine<MosquitoHungerMonitor.States, MosquitoHungerMonitor.Instance, MosquitoHungerMonitor, object>.Not(new StateMachine<MosquitoHungerMonitor.States, MosquitoHungerMonitor.Instance, MosquitoHungerMonitor, object>.Transition.ConditionCallback(MosquitoHungerMonitor.HasValidVictim)));
		}

		public GameStateMachine<MosquitoHungerMonitor.States, MosquitoHungerMonitor.Instance, MosquitoHungerMonitor, object>.State satisfied;

		public MosquitoHungerMonitor.States.HungryStates hungry;

		public StateMachine<MosquitoHungerMonitor.States, MosquitoHungerMonitor.Instance, MosquitoHungerMonitor, object>.TargetParameter victim;

		public class HungryStates : GameStateMachine<MosquitoHungerMonitor.States, MosquitoHungerMonitor.Instance, MosquitoHungerMonitor, object>.State
		{
			public GameStateMachine<MosquitoHungerMonitor.States, MosquitoHungerMonitor.Instance, MosquitoHungerMonitor, object>.State lookingForVictim;

			public GameStateMachine<MosquitoHungerMonitor.States, MosquitoHungerMonitor.Instance, MosquitoHungerMonitor, object>.State chaseVictim;
		}
	}

	public class Instance : GameStateMachine<MosquitoHungerMonitor.States, MosquitoHungerMonitor.Instance, MosquitoHungerMonitor, object>.GameInstance
	{
		public GameObject Victim
		{
			get
			{
				return base.sm.victim.Get(this);
			}
		}

		public bool IsFed
		{
			get
			{
				return this.effects.HasEffect("MosquitoFed");
			}
		}

		public Navigator navigator { get; private set; }

		public Instance(MosquitoHungerMonitor master)
			: base(master)
		{
			this.effects = base.GetComponent<Effects>();
			this.navigator = base.GetComponent<Navigator>();
		}

		public void ApplyFedEffect()
		{
			this.effects.Add("MosquitoFed", true);
		}

		private Effects effects;
	}
}
