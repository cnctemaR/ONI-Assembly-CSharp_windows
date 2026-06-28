using System;
using UnityEngine;

public class Compost : StateMachineComponent<Compost.StatesInstance>
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.Subscribe(-1697596308, new EventSystem.EventHandler(this.OnStorageChanged));
		base.GetComponent<Storage>().choreType = Db.Get().ChoreTypes.FetchCritical;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		ManualDeliveryKG component = base.GetComponent<ManualDeliveryKG>();
		component.ShowStatusItem = false;
		base.smi.StartSM();
	}

	private void OnStorageChanged(object data)
	{
		GameObject gameObject = (GameObject)data;
		if (gameObject == null)
		{
			return;
		}
	}

	private const float FlipInterval = 600f;

	[MyCmpGet]
	private Operational operational;

	[MyCmpGet]
	private Storage storage;

	public class StatesInstance : GameStateMachine<Compost.States, Compost.StatesInstance, Compost>.GameInstance
	{
		public StatesInstance(Compost master)
			: base(master)
		{
		}

		public bool HasSufficientMass()
		{
			return base.master.GetComponent<ElementConverter>().HasEnoughMass();
		}

		public bool IsEmpty()
		{
			return base.master.storage.IsEmpty();
		}

		public void ResetWorkable()
		{
			CompostWorkable component = base.master.GetComponent<CompostWorkable>();
			component.ShowProgressBar(false);
			component.WorkTimeRemaining = component.GetWorkTime();
		}
	}

	public class States : GameStateMachine<Compost.States, Compost.StatesInstance, Compost>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.empty;
			base.serializable = true;
			this.empty.Enter("empty", delegate(Compost.StatesInstance smi)
			{
				smi.ResetWorkable();
			}).EventTransition(GameHashes.OnStorageChange, this.insufficientMass, (Compost.StatesInstance smi) => !smi.IsEmpty()).EventTransition(GameHashes.OperationalChanged, this.disabledEmpty, (Compost.StatesInstance smi) => !smi.GetComponent<Operational>().IsOperational)
				.ToggleStatusItem(Db.Get().BuildingStatusItems.AwaitingWaste, null)
				.PlayAnim("off", KAnim.PlayMode.Once, null);
			this.insufficientMass.Enter("empty", delegate(Compost.StatesInstance smi)
			{
				smi.ResetWorkable();
			}).EventTransition(GameHashes.OnStorageChange, this.empty, (Compost.StatesInstance smi) => smi.IsEmpty()).EventTransition(GameHashes.OnStorageChange, this.inert, (Compost.StatesInstance smi) => smi.HasSufficientMass())
				.ToggleStatusItem(Db.Get().BuildingStatusItems.AwaitingWaste, null)
				.PlayAnim("idle_half", KAnim.PlayMode.Once, null);
			this.inert.EventTransition(GameHashes.OperationalChanged, this.disabled, (Compost.StatesInstance smi) => !smi.GetComponent<Operational>().IsOperational).PlayAnim("on", KAnim.PlayMode.Once, null).ToggleStatusItem(Db.Get().BuildingStatusItems.AwaitingCompostFlip, null)
				.ToggleChore(new Func<Compost.StatesInstance, Chore>(this.CreateFlipChore), this.composting, false);
			this.composting.Enter("Composting", delegate(Compost.StatesInstance smi)
			{
				smi.master.operational.SetActive(true, false);
			}).EventTransition(GameHashes.OnStorageChange, this.empty, (Compost.StatesInstance smi) => !smi.HasSufficientMass()).EventTransition(GameHashes.OperationalChanged, this.disabled, (Compost.StatesInstance smi) => !smi.GetComponent<Operational>().IsOperational)
				.ScheduleGoTo(600f, this.inert)
				.PlayAnims((Compost.StatesInstance smi) => Compost.States.compostingStrings, KAnim.PlayMode.Loop)
				.Exit(delegate(Compost.StatesInstance smi)
				{
					smi.master.operational.SetActive(false, false);
				});
			this.compostingPst.PlayAnim("composting_pst", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.empty);
			this.disabled.Enter("disabledEmpty", delegate(Compost.StatesInstance smi)
			{
				smi.ResetWorkable();
			}).PlayAnim("on", KAnim.PlayMode.Once, null).EventTransition(GameHashes.OperationalChanged, this.inert, (Compost.StatesInstance smi) => smi.GetComponent<Operational>().IsOperational);
			this.disabledEmpty.Enter("disabledEmpty", delegate(Compost.StatesInstance smi)
			{
				smi.ResetWorkable();
			}).PlayAnim("off", KAnim.PlayMode.Once, null).EventTransition(GameHashes.OperationalChanged, this.empty, (Compost.StatesInstance smi) => smi.GetComponent<Operational>().IsOperational);
		}

		private Chore CreateFlipChore(Compost.StatesInstance smi)
		{
			return new WorkChore<CompostWorkable>(Db.Get().ChoreTypes.FlipCompost, smi.master, null, true, null, null, null, true, null, true, default(Tag), null, false, true);
		}

		public GameStateMachine<Compost.States, Compost.StatesInstance, Compost>.State empty;

		public GameStateMachine<Compost.States, Compost.StatesInstance, Compost>.State insufficientMass;

		public GameStateMachine<Compost.States, Compost.StatesInstance, Compost>.State disabled;

		public GameStateMachine<Compost.States, Compost.StatesInstance, Compost>.State disabledEmpty;

		public GameStateMachine<Compost.States, Compost.StatesInstance, Compost>.State inert;

		public GameStateMachine<Compost.States, Compost.StatesInstance, Compost>.State composting;

		public GameStateMachine<Compost.States, Compost.StatesInstance, Compost>.State compostingPst;

		private static readonly string[] compostingStrings = new string[] { "composting_pre", "composting_loop" };
	}
}
