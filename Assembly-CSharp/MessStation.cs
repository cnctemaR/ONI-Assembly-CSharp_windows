using System;
using System.Collections.Generic;
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
		Storage component = go.GetComponent<Storage>();
		if (component != null)
		{
			foreach (Garnish garnish in Garnish.All)
			{
				if (component.Has(garnish.itemTag))
				{
					list.Add(garnish.descriptor);
				}
			}
		}
		return list;
	}

	public bool HasGarnish
	{
		get
		{
			return this.smi.HasGarnish;
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
			this.salt.none.Transition(this.salt.salty, (MessStation.MessStationSM.Instance smi) => smi.HasGarnish, UpdateRate.SIM_200ms).PlayAnim("off");
			this.salt.salty.Transition(this.salt.none, (MessStation.MessStationSM.Instance smi) => !smi.HasGarnish, UpdateRate.SIM_200ms).PlayAnim("salt").EventTransition(GameHashes.EatStart, this.eating, null);
			this.eating.Transition(this.salt.salty, (MessStation.MessStationSM.Instance smi) => smi.HasGarnish && !smi.IsEating(), UpdateRate.SIM_200ms).Transition(this.salt.none, (MessStation.MessStationSM.Instance smi) => !smi.HasGarnish && !smi.IsEating(), UpdateRate.SIM_200ms).PlayAnim("off");
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
				this.garnishStorage = master.GetComponent<Storage>();
				this.reservable = master.GetComponent<Reservable>();
				this.symbolOverrideController = master.GetComponent<SymbolOverrideController>();
				this.garnishStorage.Subscribe(-1697596308, delegate(object _)
				{
					this.UpdateGarnishOverride();
				});
				this.UpdateGarnishOverride();
			}

			public bool HasGarnish
			{
				get
				{
					return Garnish.HasAny(this.garnishStorage);
				}
			}

			public void UpdateGarnishOverride()
			{
				if (this.symbolOverrideController == null)
				{
					return;
				}
				Garnish active = Garnish.GetActive(this.garnishStorage);
				KAnim.Build.Symbol symbol = ((active != null) ? active.GetOverrideSymbol() : null);
				if (symbol != null)
				{
					this.symbolOverrideController.AddSymbolOverride(MessStation.MessStationSM.Instance.SALT_SYMBOL, symbol, 0);
					return;
				}
				this.symbolOverrideController.RemoveSymbolOverride(MessStation.MessStationSM.Instance.SALT_SYMBOL, 0);
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

			private Storage garnishStorage;

			private Reservable reservable;

			private SymbolOverrideController symbolOverrideController;

			private static readonly HashedString SALT_SYMBOL = "saltshaker";
		}
	}
}
