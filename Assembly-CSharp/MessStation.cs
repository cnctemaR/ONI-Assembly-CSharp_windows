using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/MessStation")]
public class MessStation : Workable, IGameObjectEffectDescriptor
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_use_machine_kanim") };
	}

	protected override void OnCompleteWork(Worker worker)
	{
		worker.workable.GetComponent<Edible>().CompleteWork(worker);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.smi = new MessStation.MessStationSM.Instance(this);
		this.smi.StartSM();
	}

	public override List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (go.GetComponent<Storage>().Has(TableSaltConfig.ID.ToTag()))
		{
			list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.MESS_TABLE_SALT, TableSaltTuning.MORALE_MODIFIER), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.MESS_TABLE_SALT, TableSaltTuning.MORALE_MODIFIER), Descriptor.DescriptorType.Effect, false));
		}
		return list;
	}

	public bool HasSalt
	{
		get
		{
			return this.smi.HasSalt;
		}
	}

	private MessStation.MessStationSM.Instance smi;

	public class MessStationSM : GameStateMachine<MessStation.MessStationSM, MessStation.MessStationSM.Instance, MessStation>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.salt.none;
			this.salt.none.Transition(this.salt.salty, (MessStation.MessStationSM.Instance smi) => smi.HasSalt, UpdateRate.SIM_200ms).PlayAnim("off");
			this.salt.salty.Transition(this.salt.none, (MessStation.MessStationSM.Instance smi) => !smi.HasSalt, UpdateRate.SIM_200ms).PlayAnim("salt").EventTransition(GameHashes.EatStart, this.eating, null);
			this.eating.Transition(this.salt.salty, (MessStation.MessStationSM.Instance smi) => smi.HasSalt && !smi.IsEating(), UpdateRate.SIM_200ms).Transition(this.salt.none, (MessStation.MessStationSM.Instance smi) => !smi.HasSalt && !smi.IsEating(), UpdateRate.SIM_200ms).PlayAnim("off");
		}

		public MessStation.MessStationSM.SaltState salt;

		public GameStateMachine<MessStation.MessStationSM, MessStation.MessStationSM.Instance, MessStation, object>.State eating;

		public class SaltState : GameStateMachine<MessStation.MessStationSM, MessStation.MessStationSM.Instance, MessStation, object>.State
		{
			public GameStateMachine<MessStation.MessStationSM, MessStation.MessStationSM.Instance, MessStation, object>.State none;

			public GameStateMachine<MessStation.MessStationSM, MessStation.MessStationSM.Instance, MessStation, object>.State salty;
		}

		public new class Instance : GameStateMachine<MessStation.MessStationSM, MessStation.MessStationSM.Instance, MessStation, object>.GameInstance
		{
			public Instance(MessStation master)
				: base(master)
			{
				this.saltStorage = master.GetComponent<Storage>();
				this.assigned = master.GetComponent<Assignable>();
			}

			public bool HasSalt
			{
				get
				{
					return this.saltStorage.Has(TableSaltConfig.ID.ToTag());
				}
			}

			public bool IsEating()
			{
				if (this.assigned != null && this.assigned.assignee != null)
				{
					GameObject targetGameObject = this.assigned.assignee.GetSoleOwner().GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
					if (targetGameObject)
					{
						ChoreDriver component = targetGameObject.GetComponent<ChoreDriver>();
						return component != null && component.HasChore() && component.GetCurrentChore().choreType.urge == Db.Get().Urges.Eat;
					}
				}
				return false;
			}

			private Storage saltStorage;

			private Assignable assigned;
		}
	}
}
