using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Polymerizer : StateMachineComponent<Polymerizer.StatesInstance>
{
	protected override void OnSpawn()
	{
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		this.plasticMeter = new MeterController(component, "meter_target", "meter", Meter.Offset.Infront, new Vector3(0f, 0f, 0f), null);
		this.oilMeter = new MeterController(component, "meter2_target", "meter2", Meter.Offset.Infront, new Vector3(0f, 0f, 0f), null);
		component.StopHidingSymbol(new KAnimHashedString("meter_target"), true);
		float num = 0f;
		PrimaryElement primaryElement = this.storage.FindPrimaryElement(SimHashes.Petroleum);
		if (primaryElement != null)
		{
			num = Mathf.Clamp01(primaryElement.Mass / this.consumer.capacityKG);
		}
		this.oilMeter.SetPositionPercent(num);
		base.smi.StartSM();
		this.Subscribe(-1697596308, new Action<object>(this.OnStorageChanged));
	}

	private void TryEmit()
	{
		GameObject gameObject = this.storage.FindFirst(this.emitTag);
		if (gameObject != null)
		{
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			this.UpdatePercentDone(component);
			this.TryEmit(component);
		}
	}

	private void TryEmit(PrimaryElement primary_elem)
	{
		if (primary_elem.Mass >= this.emitMass)
		{
			this.plasticMeter.SetPositionPercent(0f);
			GameObject gameObject = this.storage.Drop(primary_elem.gameObject);
			Rotatable component = base.GetComponent<Rotatable>();
			Vector3 vector = component.transform.position + component.GetRotatedOffset(this.emitOffset);
			int num = Grid.PosToCell(vector);
			if (Grid.Solid[num])
			{
				vector += component.GetRotatedOffset(Vector3.left);
			}
			gameObject.transform.position = vector;
			PrimaryElement primaryElement = this.storage.FindPrimaryElement(this.exhaustElement);
			if (primaryElement != null)
			{
				int num2 = Grid.PosToCell(vector);
				SimMessages.AddRemoveSubstance(num2, primaryElement.ElementID, null, primaryElement.Mass, primaryElement.Temperature, primaryElement.DiseaseIdx, primaryElement.DiseaseCount, -1);
				primaryElement.Mass = 0f;
				primaryElement.ModifyDiseaseCount(int.MinValue, "Polymerizer.Exhaust");
			}
		}
	}

	private void UpdatePercentDone(PrimaryElement primary_elem)
	{
		float num = Mathf.Clamp01(primary_elem.Mass / this.emitMass);
		this.plasticMeter.SetPositionPercent(num);
	}

	private void OnStorageChanged(object data)
	{
		GameObject gameObject = (GameObject)data;
		if (gameObject == null)
		{
			return;
		}
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		if (component.ElementID == SimHashes.Petroleum)
		{
			float num = Mathf.Clamp01(component.Mass / this.consumer.capacityKG);
			this.oilMeter.SetPositionPercent(num);
		}
	}

	[SerializeField]
	public float maxMass = 2.5f;

	[SerializeField]
	public float emitMass = 1f;

	[SerializeField]
	public Tag emitTag;

	[SerializeField]
	public Vector3 emitOffset = Vector3.zero;

	[SerializeField]
	public SimHashes exhaustElement = SimHashes.Vacuum;

	[MyCmpAdd]
	private Storage storage;

	[MyCmpReq]
	private Operational operational;

	[MyCmpGet]
	private ConduitConsumer consumer;

	[MyCmpGet]
	private ElementConverter converter;

	private MeterController plasticMeter;

	private MeterController oilMeter;

	public class StatesInstance : GameStateMachine<Polymerizer.States, Polymerizer.StatesInstance, Polymerizer, object>.GameInstance
	{
		public StatesInstance(Polymerizer smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<Polymerizer.States, Polymerizer.StatesInstance, Polymerizer>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.off;
			this.root.EventTransition(GameHashes.OperationalChanged, this.off, (Polymerizer.StatesInstance smi) => !smi.master.operational.IsOperational);
			this.off.EventTransition(GameHashes.OperationalChanged, this.on, (Polymerizer.StatesInstance smi) => smi.master.operational.IsOperational);
			this.on.EventTransition(GameHashes.OnStorageChange, this.converting, (Polymerizer.StatesInstance smi) => smi.master.converter.CanConvertAtAll());
			this.converting.Enter("Ready", delegate(Polymerizer.StatesInstance smi)
			{
				smi.master.operational.SetActive(true, false);
			}).EventHandler(GameHashes.OnStorageChange, delegate(Polymerizer.StatesInstance smi)
			{
				smi.master.TryEmit();
			}).EventTransition(GameHashes.OnStorageChange, this.on, (Polymerizer.StatesInstance smi) => !smi.master.converter.CanConvertAtAll())
				.Exit("Ready", delegate(Polymerizer.StatesInstance smi)
				{
					smi.master.operational.SetActive(false, false);
				});
		}

		public GameStateMachine<Polymerizer.States, Polymerizer.StatesInstance, Polymerizer, object>.State off;

		public GameStateMachine<Polymerizer.States, Polymerizer.StatesInstance, Polymerizer, object>.State on;

		public GameStateMachine<Polymerizer.States, Polymerizer.StatesInstance, Polymerizer, object>.State converting;
	}
}
