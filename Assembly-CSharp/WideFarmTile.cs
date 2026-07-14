using System;
using UnityEngine;

public class WideFarmTile : GameStateMachine<WideFarmTile, WideFarmTile.Instance, IStateMachineTarget, WideFarmTile.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.root;
		this.root.EventHandler(GameHashes.OnStorageChange, new StateMachine<WideFarmTile, WideFarmTile.Instance, IStateMachineTarget, WideFarmTile.Def>.State.Callback(WideFarmTile.RefreshLiquidMeter));
	}

	private static void RefreshLiquidMeter(WideFarmTile.Instance smi)
	{
		smi.RefreshLiquidMeter();
	}

	private const string LIQUID_METER_ANIM_NAME = "meter";

	private const string LIQUID_METER_TARGET_NAME = "meter_target";

	private const string LIQUID_METER_TINT_SYMBOL_NAME = "meter_fill";

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<WideFarmTile, WideFarmTile.Instance, IStateMachineTarget, WideFarmTile.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, WideFarmTile.Def def)
			: base(master, def)
		{
			KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
			this.liquidMeter = new MeterController(component, "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.Building, Array.Empty<string>());
			this.conduitConsumer = base.GetComponent<ConduitConsumer>();
			this.storage = base.GetComponent<Storage>();
		}

		public override void StartSM()
		{
			base.StartSM();
			this.RefreshLiquidMeter();
		}

		public void RefreshLiquidMeter()
		{
			this.liquidMeter.SetPositionPercent(this.conduitConsumer.stored_mass / this.conduitConsumer.capacityKG);
			GameObject gameObject = this.storage.FindFirst(GameTags.Liquid);
			if (gameObject == null)
			{
				return;
			}
			Element element = gameObject.GetComponent<PrimaryElement>().Element;
			GameUtil.TintLiquidSymbolOnBuilding("meter_fill", this.liquidMeter.meterController, element);
		}

		private MeterController liquidMeter;

		private Storage storage;

		private ConduitConsumer conduitConsumer;
	}
}
