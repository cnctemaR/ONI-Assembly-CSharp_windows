using System;
using KSerialization;
using UnityEngine;

public class HeatCompressor : StateMachineComponent<HeatCompressor.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, new string[] { "meter_target", "meter_fill", "meter_frame", "meter_OL" });
		this.meter.gameObject.GetComponent<KBatchedAnimController>().SetDirty();
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab("HeatCube"), base.transform.GetPosition());
		gameObject.SetActive(true);
		this.heatCubeStorage.Store(gameObject, true, false, true, false);
		base.smi.StartSM();
	}

	public void SetStorage(Storage inputStorage, Storage outputStorage, Storage heatCubeStorage)
	{
		this.inputStorage = inputStorage;
		this.outputStorage = outputStorage;
		this.heatCubeStorage = heatCubeStorage;
	}

	public void CompressHeat(HeatCompressor.StatesInstance smi, float dt)
	{
		smi.heatRemovalTimer -= dt;
		float num = this.heatRemovalRate * dt / (float)this.inputStorage.items.Count;
		foreach (GameObject gameObject in this.inputStorage.items)
		{
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			float lowTemp = component.Element.lowTemp;
			GameUtil.DeltaThermalEnergy(component, -num, lowTemp);
			this.energyCompressed += num;
		}
		if (smi.heatRemovalTimer <= 0f)
		{
			for (int i = this.inputStorage.items.Count; i > 0; i--)
			{
				GameObject gameObject2 = this.inputStorage.items[i - 1];
				if (gameObject2)
				{
					this.inputStorage.Transfer(gameObject2, this.outputStorage, false, true);
				}
			}
			smi.StartNewHeatRemoval();
		}
		foreach (GameObject gameObject3 in this.heatCubeStorage.items)
		{
			GameUtil.DeltaThermalEnergy(gameObject3.GetComponent<PrimaryElement>(), this.energyCompressed / (float)this.heatCubeStorage.items.Count, 100000f);
		}
		this.energyCompressed = 0f;
	}

	public void EjectHeatCube()
	{
		this.heatCubeStorage.DropAll(base.transform.GetPosition(), false, false, default(Vector3), true, null);
	}

	[MyCmpReq]
	private Operational operational;

	private MeterController meter;

	public Storage inputStorage;

	public Storage outputStorage;

	public Storage heatCubeStorage;

	public float heatRemovalRate = 100f;

	public float heatRemovalTime = 100f;

	[Serialize]
	public float energyCompressed;

	public float heat_sink_active_time = 9000f;

	[Serialize]
	public float time_active;

	public float MAX_CUBE_TEMPERATURE = 3000f;

	public class StatesInstance : GameStateMachine<HeatCompressor.States, HeatCompressor.StatesInstance, HeatCompressor, object>.GameInstance
	{
		public StatesInstance(HeatCompressor master)
			: base(master)
		{
		}

		public void UpdateMeter()
		{
			float remainingCharge = this.GetRemainingCharge();
			base.master.meter.SetPositionPercent(remainingCharge);
		}

		public float GetRemainingCharge()
		{
			PrimaryElement primaryElement = base.smi.master.heatCubeStorage.FindFirstWithMass(GameTags.IndustrialIngredient, 0f);
			float num = 1f;
			if (primaryElement != null)
			{
				num = Mathf.Clamp01(primaryElement.GetComponent<PrimaryElement>().Temperature / base.smi.master.MAX_CUBE_TEMPERATURE);
			}
			return num;
		}

		public bool CanWork()
		{
			return this.GetRemainingCharge() < 1f && base.smi.master.heatCubeStorage.items.Count > 0;
		}

		public void StartNewHeatRemoval()
		{
			this.heatRemovalTimer = base.smi.master.heatRemovalTime;
		}

		[Serialize]
		public float heatRemovalTimer;
	}

	public class States : GameStateMachine<HeatCompressor.States, HeatCompressor.StatesInstance, HeatCompressor>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.inactive;
			base.serializable = StateMachine.SerializeType.Both_DEPRECATED;
			this.root.EventTransition(GameHashes.OperationalChanged, this.inactive, (HeatCompressor.StatesInstance smi) => !smi.GetComponent<Operational>().IsOperational);
			this.inactive.Enter(delegate(HeatCompressor.StatesInstance smi)
			{
				smi.UpdateMeter();
			}).PlayAnim("idle").Transition(this.dropCube, (HeatCompressor.StatesInstance smi) => smi.GetRemainingCharge() >= 1f, UpdateRate.SIM_200ms)
				.Transition(this.active, (HeatCompressor.StatesInstance smi) => smi.GetComponent<Operational>().IsOperational && smi.CanWork(), UpdateRate.SIM_200ms);
			this.active.Enter(delegate(HeatCompressor.StatesInstance smi)
			{
				smi.GetComponent<Operational>().SetActive(true, false);
				smi.StartNewHeatRemoval();
			}).PlayAnim("working_loop", KAnim.PlayMode.Loop).Update(delegate(HeatCompressor.StatesInstance smi, float dt)
			{
				smi.master.time_active += dt;
				smi.UpdateMeter();
				smi.master.CompressHeat(smi, dt);
			}, UpdateRate.SIM_200ms, false)
				.Transition(this.dropCube, (HeatCompressor.StatesInstance smi) => smi.GetRemainingCharge() >= 1f, UpdateRate.SIM_200ms)
				.Transition(this.inactive, (HeatCompressor.StatesInstance smi) => !smi.CanWork(), UpdateRate.SIM_200ms)
				.Exit(delegate(HeatCompressor.StatesInstance smi)
				{
					smi.GetComponent<Operational>().SetActive(false, false);
				});
			this.dropCube.Enter(delegate(HeatCompressor.StatesInstance smi)
			{
				smi.master.EjectHeatCube();
				smi.GoTo(this.inactive);
			});
		}

		public GameStateMachine<HeatCompressor.States, HeatCompressor.StatesInstance, HeatCompressor, object>.State active;

		public GameStateMachine<HeatCompressor.States, HeatCompressor.StatesInstance, HeatCompressor, object>.State inactive;

		public GameStateMachine<HeatCompressor.States, HeatCompressor.StatesInstance, HeatCompressor, object>.State dropCube;
	}
}
