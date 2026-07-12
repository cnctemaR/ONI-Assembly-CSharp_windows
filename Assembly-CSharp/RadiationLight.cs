using System;
using UnityEngine;

public class RadiationLight : StateMachineComponent<RadiationLight.StatesInstance>
{
	public void UpdateMeter()
	{
		this.meter.SetPositionPercent(Mathf.Clamp01(this.storage.MassStored() / this.storage.capacityKg));
	}

	public bool HasEnoughFuel()
	{
		return this.elementConverter.HasEnoughMassToStartConverting(false);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		this.UpdateMeter();
	}

	[MyCmpGet]
	private Operational operational;

	[MyCmpGet]
	private Storage storage;

	[MyCmpGet]
	private RadiationEmitter emitter;

	[MyCmpGet]
	private ElementConverter elementConverter;

	private MeterController meter;

	public Tag elementToConsume;

	public float consumptionRate;

	public class StatesInstance : GameStateMachine<RadiationLight.States, RadiationLight.StatesInstance, RadiationLight, object>.GameInstance
	{
		public StatesInstance(RadiationLight smi)
			: base(smi)
		{
			if (base.GetComponent<Rotatable>().IsRotated)
			{
				RadiationEmitter component = base.GetComponent<RadiationEmitter>();
				component.emitDirection = 180f;
				component.emissionOffset = Vector3.left;
			}
			this.ToggleEmitter(false);
			smi.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[] { "meter_target" });
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Radiation, true);
		}

		public void ToggleEmitter(bool on)
		{
			base.smi.master.operational.SetActive(on, false);
			base.smi.master.emitter.SetEmitting(on);
		}
	}

	public class States : GameStateMachine<RadiationLight.States, RadiationLight.StatesInstance, RadiationLight>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.ready.idle;
			this.root.EventHandler(GameHashes.OnStorageChange, delegate(RadiationLight.StatesInstance smi)
			{
				smi.master.UpdateMeter();
			});
			this.waiting.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, this.ready.idle, (RadiationLight.StatesInstance smi) => smi.master.operational.IsOperational);
			this.ready.EventTransition(GameHashes.OperationalChanged, this.waiting, (RadiationLight.StatesInstance smi) => !smi.master.operational.IsOperational).DefaultState(this.ready.idle);
			this.ready.idle.PlayAnim("off").EventTransition(GameHashes.OnStorageChange, this.ready.on, (RadiationLight.StatesInstance smi) => smi.master.HasEnoughFuel());
			this.ready.on.PlayAnim("on").Enter(delegate(RadiationLight.StatesInstance smi)
			{
				smi.ToggleEmitter(true);
			}).EventTransition(GameHashes.OnStorageChange, this.ready.idle, (RadiationLight.StatesInstance smi) => !smi.master.HasEnoughFuel())
				.Exit(delegate(RadiationLight.StatesInstance smi)
				{
					smi.ToggleEmitter(false);
				});
		}

		public GameStateMachine<RadiationLight.States, RadiationLight.StatesInstance, RadiationLight, object>.State waiting;

		public RadiationLight.States.ReadyStates ready;

		public class ReadyStates : GameStateMachine<RadiationLight.States, RadiationLight.StatesInstance, RadiationLight, object>.State
		{
			public GameStateMachine<RadiationLight.States, RadiationLight.StatesInstance, RadiationLight, object>.State idle;

			public GameStateMachine<RadiationLight.States, RadiationLight.StatesInstance, RadiationLight, object>.State on;
		}
	}
}
