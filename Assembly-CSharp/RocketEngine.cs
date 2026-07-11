using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class RocketEngine : StateMachineComponent<RocketEngine.StatesInstance>, IEffectDescriptor
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		RequireAttachedComponent requireAttachedComponent = new RequireAttachedComponent(base.gameObject.GetComponent<AttachableBuilding>(), typeof(FuelTank), UI.STARMAP.COMPONENT.FUEL_TANK);
		base.GetComponent<RocketModule>().AddCondition(requireAttachedComponent);
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		return null;
	}

	public float thrustAmount = (float)ROCKETRY.MODULE_THRUST_SCORE.ENGINES.MEDIUM;

	public float exhaustEmitRate = 50f;

	public float exhaustTemperature = 1500f;

	public SpawnFXHashes explosionEffectHash;

	public SimHashes exhaustElement = SimHashes.CarbonDioxide;

	public class StatesInstance : GameStateMachine<RocketEngine.States, RocketEngine.StatesInstance, RocketEngine, object>.GameInstance
	{
		public StatesInstance(RocketEngine smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<RocketEngine.States, RocketEngine.StatesInstance, RocketEngine>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idle;
			this.idle.PlayAnim("grounded", KAnim.PlayMode.Loop).EventTransition(GameHashes.IgniteEngine, this.burning, null);
			this.burning.EventTransition(GameHashes.LandRocket, this.burnComplete, null).PlayAnim("launch_pre").QueueAnim("launch_loop", true, null)
				.Update(delegate(RocketEngine.StatesInstance smi, float dt)
				{
					Vector3 vector = smi.master.gameObject.transform.position + smi.master.GetComponent<KBatchedAnimController>().Offset;
					int num = Grid.PosToCell(vector);
					int num2 = Grid.CellBelow(num);
					if (Grid.IsValidCell(num))
					{
						SimMessages.EmitMass(num, (byte)ElementLoader.GetElementIndex(smi.master.exhaustElement), dt * smi.master.exhaustEmitRate, smi.master.exhaustTemperature, 0, 0, 0);
					}
					Game.Instance.SpawnFX(smi.master.explosionEffectHash, num2, 0f);
					int num3 = 10;
					for (int i = 1; i < num3; i++)
					{
						int num4 = Grid.OffsetCell(num, -1, -i);
						int num5 = Grid.OffsetCell(num, 0, -i);
						int num6 = Grid.OffsetCell(num, 1, -i);
						if (Grid.IsValidCell(num4))
						{
							SimMessages.ModifyEnergy(num4, smi.master.exhaustTemperature / (float)(i + 1), 3200f, SimMessages.EnergySourceID.Burner);
						}
						if (Grid.IsValidCell(num5))
						{
							SimMessages.ModifyEnergy(num5, smi.master.exhaustTemperature / (float)i, 3200f, SimMessages.EnergySourceID.Burner);
						}
						if (Grid.IsValidCell(num6))
						{
							SimMessages.ModifyEnergy(num6, smi.master.exhaustTemperature / (float)(i + 1), 3200f, SimMessages.EnergySourceID.Burner);
						}
					}
				}, UpdateRate.SIM_200ms, false);
			this.burnComplete.PlayAnim("grounded", KAnim.PlayMode.Loop).EventTransition(GameHashes.IgniteEngine, this.burning, null);
		}

		public GameStateMachine<RocketEngine.States, RocketEngine.StatesInstance, RocketEngine, object>.State idle;

		public GameStateMachine<RocketEngine.States, RocketEngine.StatesInstance, RocketEngine, object>.State burning;

		public GameStateMachine<RocketEngine.States, RocketEngine.StatesInstance, RocketEngine, object>.State burnComplete;
	}
}
