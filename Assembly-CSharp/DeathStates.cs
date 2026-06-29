using System;
using STRINGS;
using UnityEngine;

internal class DeathStates : GameStateMachine<DeathStates, DeathStates.Instance, IStateMachineTarget, DeathStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.loop;
		GameStateMachine<DeathStates, DeathStates.Instance, IStateMachineTarget, DeathStates.Def>.State state = this.loop;
		string text = CREATURES.STATUSITEMS.DEAD.NAME;
		string text2 = CREATURES.STATUSITEMS.DEAD.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		state.ToggleStatusItem(text, text2, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, 63486, null, null, main).Enter("EnableGravity", delegate(DeathStates.Instance smi)
		{
			smi.EnableGravityIfNecessary();
		}).PlayAnim("Death")
			.OnAnimQueueComplete(this.pst)
			.Exit("DisableGravity", delegate(DeathStates.Instance smi)
			{
				smi.DisableGravity();
			});
		this.pst.TriggerOnEnter(GameHashes.DeathAnimComplete, null).Enter("Butcher", delegate(DeathStates.Instance smi)
		{
			if (smi.gameObject.GetComponent<Butcherable>() != null)
			{
				smi.GetComponent<Butcherable>().OnButcherComplete();
			}
		}).Enter("Destroy", delegate(DeathStates.Instance smi)
		{
			smi.gameObject.DeleteObject();
		})
			.BehaviourComplete(GameTags.Creatures.Die, false);
	}

	private GameStateMachine<DeathStates, DeathStates.Instance, IStateMachineTarget, DeathStates.Def>.State loop;

	private GameStateMachine<DeathStates, DeathStates.Instance, IStateMachineTarget, DeathStates.Def>.State pst;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<DeathStates, DeathStates.Instance, IStateMachineTarget, DeathStates.Def>.GameInstance
	{
		public Instance(Chore<DeathStates.Instance> chore, DeathStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Die);
		}

		public void EnableGravityIfNecessary()
		{
			if (base.HasTag(GameTags.Creatures.Flyer))
			{
				GameComps.Gravities.Add(base.smi.gameObject, Vector2.zero, null);
			}
		}

		public void DisableGravity()
		{
			if (GameComps.Gravities.Has(base.smi.gameObject))
			{
				GameComps.Gravities.Remove(base.smi.gameObject);
			}
		}
	}
}
