using System;
using Klei.AI;

public class TakeMedicineChore : Chore<TakeMedicineChore.StatesInstance>
{
	public TakeMedicineChore(MedicinalPill master)
		: base(Db.Get().ChoreTypes.TakeMedicine, master, null, false, null, null, null, int.MaxValue, false, true)
	{
		this.medicine = master;
		this.pickupable = this.medicine.GetComponent<Pickupable>();
		this.smi = new TakeMedicineChore.StatesInstance(this);
		base.AddPrecondition(ChorePreconditions.CanPickup, this.pickupable);
		base.AddPrecondition(TakeMedicineChore.CanCure, this);
	}

	// Note: this type is marked as 'beforefieldinit'.
	static TakeMedicineChore()
	{
		Chore.Precondition precondition = default(Chore.Precondition);
		precondition.id = "CanCure";
		precondition.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			TakeMedicineChore takeMedicineChore = (TakeMedicineChore)data;
			Diseases diseases = context.consumer.GetDiseases();
			foreach (DiseaseInstance diseaseInstance in diseases)
			{
				foreach (string text in takeMedicineChore.medicine.curedDiseases)
				{
					if (Diseases.CanCure(text, diseaseInstance.modifier.Id) && !diseaseInstance.HasTakenPill(takeMedicineChore.medicine.gameObject.GetProperName()))
					{
						return true;
					}
				}
			}
			return false;
		};
		TakeMedicineChore.CanCure = precondition;
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

	public static Chore.Precondition CanCure;

	public class StatesInstance : GameStateMachine<TakeMedicineChore.States, TakeMedicineChore.StatesInstance, TakeMedicineChore>.GameInstance
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
			this.takemedicine.ToggleWork("TakeMedicine", delegate(TakeMedicineChore.StatesInstance smi)
			{
				MedicinalPill medicinalPill = this.chunk.Get<MedicinalPill>(smi);
				Worker worker = this.eater.Get<Worker>(smi);
				worker.StartWork(medicinalPill, 1f);
			}, (TakeMedicineChore.StatesInstance smi) => this.chunk.Get<MedicinalPill>(smi) != null, delegate(TakeMedicineChore.StatesInstance smi)
			{
				this.chunk.Get<MedicinalPill>(smi).gameObject.DeleteObject();
			}, null, null);
		}

		public StateMachine<TakeMedicineChore.States, TakeMedicineChore.StatesInstance, TakeMedicineChore>.TargetParameter eater;

		public StateMachine<TakeMedicineChore.States, TakeMedicineChore.StatesInstance, TakeMedicineChore>.TargetParameter source;

		public StateMachine<TakeMedicineChore.States, TakeMedicineChore.StatesInstance, TakeMedicineChore>.TargetParameter chunk;

		public StateMachine<TakeMedicineChore.States, TakeMedicineChore.StatesInstance, TakeMedicineChore>.FloatParameter requestedpillcount;

		public StateMachine<TakeMedicineChore.States, TakeMedicineChore.StatesInstance, TakeMedicineChore>.FloatParameter actualpillcount;

		public GameStateMachine<TakeMedicineChore.States, TakeMedicineChore.StatesInstance, TakeMedicineChore>.FetchSubState fetch;

		public GameStateMachine<TakeMedicineChore.States, TakeMedicineChore.StatesInstance, TakeMedicineChore>.State takemedicine;
	}
}
