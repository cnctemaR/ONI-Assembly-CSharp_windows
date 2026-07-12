using System;
using System.Collections.Generic;
using UnityEngine;

public class CritterCondo : GameStateMachine<CritterCondo, CritterCondo.Instance, IStateMachineTarget, CritterCondo.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.inoperational;
		this.inoperational.PlayAnim("off").EventTransition(GameHashes.UpdateRoom, this.operational, new StateMachine<CritterCondo, CritterCondo.Instance, IStateMachineTarget, CritterCondo.Def>.Transition.ConditionCallback(CritterCondo.IsOperational));
		this.operational.PlayAnim("on", KAnim.PlayMode.Loop).EventTransition(GameHashes.UpdateRoom, this.inoperational, GameStateMachine<CritterCondo, CritterCondo.Instance, IStateMachineTarget, CritterCondo.Def>.Not(new StateMachine<CritterCondo, CritterCondo.Instance, IStateMachineTarget, CritterCondo.Def>.Transition.ConditionCallback(CritterCondo.IsOperational)));
	}

	private static bool IsOperational(CritterCondo.Instance smi)
	{
		return smi.def.IsCritterCondoOperationalCb(smi);
	}

	public GameStateMachine<CritterCondo, CritterCondo.Instance, IStateMachineTarget, CritterCondo.Def>.State inoperational;

	public GameStateMachine<CritterCondo, CritterCondo.Instance, IStateMachineTarget, CritterCondo.Def>.State operational;

	public class Def : StateMachine.BaseDef, IGameObjectEffectDescriptor
	{
		public List<Descriptor> GetDescriptors(GameObject go)
		{
			return new List<Descriptor>();
		}

		public Func<CritterCondo.Instance, bool> IsCritterCondoOperationalCb;

		public StatusItem moveToStatusItem;

		public StatusItem interactStatusItem;

		public bool underWaterCondo;
	}

	public new class Instance : GameStateMachine<CritterCondo, CritterCondo.Instance, IStateMachineTarget, CritterCondo.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, CritterCondo.Def def)
			: base(master, def)
		{
		}

		public override void StartSM()
		{
			base.StartSM();
			Components.CritterCondos.Add(base.smi.GetMyWorldId(), this);
		}

		protected override void OnCleanUp()
		{
			Components.CritterCondos.Remove(base.smi.GetMyWorldId(), this);
		}

		public bool IsReserved()
		{
			return base.HasTag(GameTags.Creatures.ReservedByCreature);
		}

		public void SetReserved(bool isReserved)
		{
			if (isReserved)
			{
				base.GetComponent<KPrefabID>().SetTag(GameTags.Creatures.ReservedByCreature, true);
				return;
			}
			if (base.HasTag(GameTags.Creatures.ReservedByCreature))
			{
				base.GetComponent<KPrefabID>().RemoveTag(GameTags.Creatures.ReservedByCreature);
				return;
			}
			global::Debug.LogWarningFormat(base.smi.gameObject, "Tried to unreserve a condo that wasn't reserved", Array.Empty<object>());
		}

		public int GetInteractStartCell()
		{
			return Grid.PosToCell(this);
		}

		public bool CanBeReserved()
		{
			return !this.IsReserved() && CritterCondo.IsOperational(this);
		}
	}
}
