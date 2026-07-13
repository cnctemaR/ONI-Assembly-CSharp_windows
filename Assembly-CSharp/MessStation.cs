using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/MessStation")]
public class MessStation : Workable, IDiningSeat
{
	protected override void OnPrefabInit()
	{
		this.ownable.AddAssignPrecondition(new Func<MinionAssignablesProxy, bool>(this.HasCaloriesOwnablePrecondition));
		base.OnPrefabInit();
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim(MessStation.eatAnim) };
	}

	public static bool CanBeAssignedTo(IAssignableIdentity assignee)
	{
		MinionAssignablesProxy minionAssignablesProxy = assignee as MinionAssignablesProxy;
		if (minionAssignablesProxy == null)
		{
			return false;
		}
		MinionIdentity minionIdentity = minionAssignablesProxy.target as MinionIdentity;
		return !(minionIdentity == null) && (Db.Get().Amounts.Calories.Lookup(minionIdentity) != null || (Game.IsDlcActiveForCurrentSave("DLC3_ID") && minionIdentity.model == BionicMinionConfig.MODEL));
	}

	private bool HasCaloriesOwnablePrecondition(MinionAssignablesProxy worker)
	{
		return MessStation.CanBeAssignedTo(worker);
	}

	protected override void OnCompleteWork(WorkerBase worker)
	{
		worker.GetWorkable().GetComponent<Edible>().CompleteWork(worker);
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
			list.Add(MessStation.TABLE_SALT_DESCRIPTOR);
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

	public HashedString EatAnim
	{
		get
		{
			return MessStation.eatAnim;
		}
	}

	public HashedString ReloadElectrobankAnim
	{
		get
		{
			return MessStation.reloadElectrobankAnim;
		}
	}

	public Storage FindStorage()
	{
		return base.GetComponent<Storage>();
	}

	public Operational FindOperational()
	{
		return base.GetComponent<Operational>();
	}

	public KPrefabID Diner { get; set; }

	public static readonly Descriptor TABLE_SALT_DESCRIPTOR = new Descriptor(string.Format(UI.BUILDINGEFFECTS.MESS_TABLE_SALT, TableSaltTuning.MORALE_MODIFIER), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.MESS_TABLE_SALT, TableSaltTuning.MORALE_MODIFIER), Descriptor.DescriptorType.Effect, false);

	[MyCmpGet]
	private Ownable ownable;

	private MessStation.MessStationSM.Instance smi;

	public static readonly HashedString eatAnim = "anim_eat_table_kanim";

	public static readonly HashedString reloadElectrobankAnim = "anim_bionic_eat_table_kanim";

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
				this.reservable = master.GetComponent<Reservable>();
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
				if (this.reservable == null)
				{
					return false;
				}
				if (this.reservable.ReservedBy == null)
				{
					return false;
				}
				ChoreDriver choreDriver;
				if (!this.reservable.ReservedBy.TryGetComponent<ChoreDriver>(out choreDriver))
				{
					return false;
				}
				if (!choreDriver.HasChore())
				{
					return false;
				}
				ReloadElectrobankChore reloadElectrobankChore = choreDriver.GetCurrentChore() as ReloadElectrobankChore;
				if (reloadElectrobankChore != null)
				{
					return reloadElectrobankChore.IsInstallingAtMessStation();
				}
				return choreDriver.GetCurrentChore().choreType.urge == Db.Get().Urges.Eat;
			}

			private Storage saltStorage;

			private Reservable reservable;
		}
	}
}
