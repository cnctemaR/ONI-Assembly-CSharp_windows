using System;
using System.Collections.Generic;
using UnityEngine;

public class Compost : StateMachineComponent<Compost.StatesInstance>, IEffectDescriptor, IGameObjectEffectDescriptor
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe(-1697596308, new Action<object>(this.OnStorageChanged));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		ManualDeliveryKG component = base.GetComponent<ManualDeliveryKG>();
		component.ShowStatusItem = false;
		this.temperatureAdjuster = new SimulatedTemperatureAdjuster(this.simulatedInternalTemperature, this.simulatedInternalHeatCapacity, this.simulatedThermalConductivity, base.GetComponent<Storage>());
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		this.temperatureAdjuster.CleanUp();
	}

	private void OnStorageChanged(object data)
	{
		GameObject gameObject = (GameObject)data;
		if (gameObject == null)
		{
			return;
		}
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		return this.GetDescriptors(def.BuildingComplete);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return SimulatedTemperatureAdjuster.GetDescriptors(this.simulatedInternalTemperature);
	}

	[MyCmpGet]
	private Operational operational;

	[MyCmpGet]
	private Storage storage;

	[SerializeField]
	public float flipInterval = 600f;

	[SerializeField]
	public SimHashes emitHash = SimHashes.Vacuum;

	[SerializeField]
	public float emitMassThreshold = 1f;

	[SerializeField]
	public float simulatedInternalTemperature = 323.15f;

	[SerializeField]
	public float simulatedInternalHeatCapacity = 400f;

	[SerializeField]
	public float simulatedThermalConductivity = 1000f;

	private SimulatedTemperatureAdjuster temperatureAdjuster;

	public class StatesInstance : GameStateMachine<Compost.States, Compost.StatesInstance, Compost, object>.GameInstance
	{
		public StatesInstance(Compost master)
			: base(master)
		{
		}

		public bool CanStartConverting()
		{
			return base.master.GetComponent<ElementConverter>().HasEnoughMassToStartConverting();
		}

		public bool CanContinueConverting()
		{
			return base.master.GetComponent<ElementConverter>().CanConvertAtAll();
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

		public void TryEmit()
		{
			PrimaryElement primaryElement = base.master.storage.FindPrimaryElement(base.master.emitHash);
			if (primaryElement != null && primaryElement.Mass >= base.master.emitMassThreshold)
			{
				primaryElement.Temperature = base.master.GetComponent<PrimaryElement>().Temperature;
				base.master.storage.Drop(primaryElement.gameObject);
			}
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
				.PlayAnim("off");
			this.insufficientMass.Enter("empty", delegate(Compost.StatesInstance smi)
			{
				smi.ResetWorkable();
			}).EventTransition(GameHashes.OnStorageChange, this.empty, (Compost.StatesInstance smi) => smi.IsEmpty()).EventTransition(GameHashes.OnStorageChange, this.inert, (Compost.StatesInstance smi) => smi.CanStartConverting())
				.ToggleStatusItem(Db.Get().BuildingStatusItems.AwaitingWaste, null)
				.PlayAnim("idle_half");
			this.inert.EventTransition(GameHashes.OperationalChanged, this.disabled, (Compost.StatesInstance smi) => !smi.GetComponent<Operational>().IsOperational).PlayAnim("on").ToggleStatusItem(Db.Get().BuildingStatusItems.AwaitingCompostFlip, null)
				.ToggleChore(new Func<Compost.StatesInstance, Chore>(this.CreateFlipChore), this.composting);
			this.composting.Enter("Composting", delegate(Compost.StatesInstance smi)
			{
				smi.master.operational.SetActive(true, false);
			}).EventTransition(GameHashes.OnStorageChange, this.empty, (Compost.StatesInstance smi) => !smi.CanContinueConverting()).EventTransition(GameHashes.OperationalChanged, this.disabled, (Compost.StatesInstance smi) => !smi.GetComponent<Operational>().IsOperational)
				.EventHandler(GameHashes.OnStorageChange, delegate(Compost.StatesInstance smi)
				{
					smi.TryEmit();
				})
				.ScheduleGoTo((Compost.StatesInstance smi) => smi.master.flipInterval, this.inert)
				.PlayAnims((Compost.StatesInstance smi) => Compost.States.compostingAnims, KAnim.PlayMode.Loop)
				.Exit(delegate(Compost.StatesInstance smi)
				{
					smi.master.operational.SetActive(false, false);
				});
			this.compostingPst.PlayAnim("composting_pst").OnAnimQueueComplete(this.empty);
			this.disabled.Enter("disabledEmpty", delegate(Compost.StatesInstance smi)
			{
				smi.ResetWorkable();
			}).PlayAnim("on").EventTransition(GameHashes.OperationalChanged, this.inert, (Compost.StatesInstance smi) => smi.GetComponent<Operational>().IsOperational);
			this.disabledEmpty.Enter("disabledEmpty", delegate(Compost.StatesInstance smi)
			{
				smi.ResetWorkable();
			}).PlayAnim("off").EventTransition(GameHashes.OperationalChanged, this.empty, (Compost.StatesInstance smi) => smi.GetComponent<Operational>().IsOperational);
		}

		private Chore CreateFlipChore(Compost.StatesInstance smi)
		{
			return new WorkChore<CompostWorkable>(Db.Get().ChoreTypes.FlipCompost, smi.master, null, null, true, null, null, null, true, null, true, null, false, true, true, PriorityScreen.PriorityClass.basic, int.MaxValue, false);
		}

		public GameStateMachine<Compost.States, Compost.StatesInstance, Compost, object>.State empty;

		public GameStateMachine<Compost.States, Compost.StatesInstance, Compost, object>.State insufficientMass;

		public GameStateMachine<Compost.States, Compost.StatesInstance, Compost, object>.State disabled;

		public GameStateMachine<Compost.States, Compost.StatesInstance, Compost, object>.State disabledEmpty;

		public GameStateMachine<Compost.States, Compost.StatesInstance, Compost, object>.State inert;

		public GameStateMachine<Compost.States, Compost.StatesInstance, Compost, object>.State composting;

		public GameStateMachine<Compost.States, Compost.StatesInstance, Compost, object>.State compostingPst;

		private static readonly HashedString[] compostingAnims = new HashedString[] { "composting_pre", "composting_loop" };
	}
}
