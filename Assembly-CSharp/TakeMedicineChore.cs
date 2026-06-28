using System;

public class TakeMedicineChore : Chore<TakeMedicineChore.StatesInstance>
{
	public TakeMedicineChore(MedicinalPill master)
		: base(Db.Get().ChoreTypes.TakeMedicine, master, null, false, null, null, null, PriorityScreen.PriorityClass.basic, int.MaxValue, false, true, 0)
	{
		this.medicine = master;
		this.pickupable = this.medicine.GetComponent<Pickupable>();
		this.smi = new TakeMedicineChore.StatesInstance(this);
		base.AddPrecondition(ChorePreconditions.CanPickup, this.pickupable);
		base.AddPrecondition(TakeMedicineChore.CanCure, this);
		base.AddPrecondition(TakeMedicineChore.IsConsumptionPermitted, this);
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		this.smi.sm.source.Set(this.pickupable.gameObject, this.smi);
		this.smi.sm.requestedpillcount.Set(1f, this.smi);
		this.smi.sm.eater.Set(context.consumer.gameObject, this.smi);
		base.Begin(context);
	}

	private Pickupable pickupable;

	private MedicinalPill medicine;

	public static Chore.Precondition CanCure = new Chore.Precondition
	{
		id = "CanCure",
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			TakeMedicineChore takeMedicineChore = (TakeMedicineChore)data;
			return takeMedicineChore.medicine.CanBeTakenBy(context.consumer.gameObject);
		}
	};

	public static Chore.Precondition IsConsumptionPermitted = new Chore.Precondition
	{
		id = "IsConsumptionPermitted",
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			TakeMedicineChore takeMedicineChore2 = (TakeMedicineChore)data;
			ConsumableConsumer component = context.consumer.GetComponent<ConsumableConsumer>();
			return component == null || component.IsPermitted(takeMedicineChore2.medicine.PrefabID().Name);
		}
	};

	public class StatesInstance : GameStateMachine<TakeMedicineChore.States, TakeMedicineChore.StatesInstance, TakeMedicineChore, object>.GameInstance
	{
		public StatesInstance(TakeMedicineChore master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<TakeMedicineChore.States, TakeMedicineChore.StatesInstance, TakeMedicineChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.fetch;
			base.Target(this.eater);
			this.fetch.InitializeStates(this.eater, this.source, this.chunk, this.requestedpillcount, this.actualpillcount, this.takemedicine, null);
			this.takemedicine.ToggleAnims("anim_eat_floor_kanim", 0f).ToggleWork("TakeMedicine", delegate(TakeMedicineChore.StatesInstance smi)
			{
				MedicinalPill medicinalPill = this.chunk.Get<MedicinalPill>(smi);
				Worker worker = this.eater.Get<Worker>(smi);
				worker.StartWork(new Worker.StartWorkInfo(medicinalPill));
			}, (TakeMedicineChore.StatesInstance smi) => this.chunk.Get<MedicinalPill>(smi) != null, delegate(TakeMedicineChore.StatesInstance smi)
			{
				this.chunk.Get<MedicinalPill>(smi).gameObject.DeleteObject();
			}, null, null);
		}

		public StateMachine<TakeMedicineChore.States, TakeMedicineChore.StatesInstance, TakeMedicineChore, object>.TargetParameter eater;

		public StateMachine<TakeMedicineChore.States, TakeMedicineChore.StatesInstance, TakeMedicineChore, object>.TargetParameter source;

		public StateMachine<TakeMedicineChore.States, TakeMedicineChore.StatesInstance, TakeMedicineChore, object>.TargetParameter chunk;

		public StateMachine<TakeMedicineChore.States, TakeMedicineChore.StatesInstance, TakeMedicineChore, object>.FloatParameter requestedpillcount;

		public StateMachine<TakeMedicineChore.States, TakeMedicineChore.StatesInstance, TakeMedicineChore, object>.FloatParameter actualpillcount;

		public GameStateMachine<TakeMedicineChore.States, TakeMedicineChore.StatesInstance, TakeMedicineChore, object>.FetchSubState fetch;

		public GameStateMachine<TakeMedicineChore.States, TakeMedicineChore.StatesInstance, TakeMedicineChore, object>.State takemedicine;
	}
}
