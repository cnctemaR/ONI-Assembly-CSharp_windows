using System;
using STRINGS;
using UnityEngine;

public class ContactConductivePipeBridge : GameStateMachine<ContactConductivePipeBridge, ContactConductivePipeBridge.Instance, IStateMachineTarget, ContactConductivePipeBridge.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.noLiquid;
		this.noLiquid.PlayAnim("off", KAnim.PlayMode.Once).ParamTransition<float>(this.noLiquidTimer, this.withLiquid, GameStateMachine<ContactConductivePipeBridge, ContactConductivePipeBridge.Instance, IStateMachineTarget, ContactConductivePipeBridge.Def>.IsGTZero);
		this.withLiquid.Update(new Action<ContactConductivePipeBridge.Instance, float>(ContactConductivePipeBridge.ExpirationTimerUpdate), UpdateRate.SIM_200ms, false).PlayAnim("on", KAnim.PlayMode.Loop).ParamTransition<float>(this.noLiquidTimer, this.noLiquid, GameStateMachine<ContactConductivePipeBridge, ContactConductivePipeBridge.Instance, IStateMachineTarget, ContactConductivePipeBridge.Def>.IsLTEZero);
	}

	private static void ExpirationTimerUpdate(ContactConductivePipeBridge.Instance smi, float dt)
	{
		float num = smi.sm.noLiquidTimer.Get(smi);
		num -= dt;
		smi.sm.noLiquidTimer.Set(num, smi, false);
	}

	private static float CalculateMaxWattsTransfered(float buildingTemperature, float building_thermal_conductivity, float content_temperature, float content_thermal_conductivity)
	{
		float num = 1f;
		float num2 = 1f;
		float num3 = 50f;
		float num4 = content_temperature - buildingTemperature;
		float num5 = (content_thermal_conductivity + building_thermal_conductivity) * 0.5f;
		return num4 * num5 * num * num3 / num2;
	}

	private static float GetKilloJoulesTransfered(float maxWattsTransfered, float dt, float building_Temperature, float building_heat_capacity, float content_temperature, float content_heat_capacity)
	{
		float num = maxWattsTransfered * dt / 1000f;
		float num2 = Mathf.Min(content_temperature, building_Temperature);
		float num3 = Mathf.Max(content_temperature, building_Temperature);
		float num4 = content_temperature - num / content_heat_capacity;
		float num5 = building_Temperature + num / building_heat_capacity;
		float num6 = Mathf.Clamp(num4, num2, num3);
		num5 = Mathf.Clamp(num5, num2, num3);
		float num7 = Mathf.Abs(num6 - content_temperature);
		float num8 = Mathf.Abs(num5 - building_Temperature);
		float num9 = num7 * content_heat_capacity;
		float num10 = num8 * building_heat_capacity;
		return Mathf.Min(num9, num10) * Mathf.Sign(maxWattsTransfered);
	}

	private static float GetFinalContentTemperature(float KJT, float building_Temperature, float building_heat_capacity, float content_temperature, float content_heat_capacity)
	{
		float num = -KJT;
		float num2 = Mathf.Max(0f, content_temperature + num / content_heat_capacity);
		float num3 = Mathf.Max(0f, building_Temperature - num / building_heat_capacity);
		if ((content_temperature - building_Temperature) * (num2 - num3) < 0f)
		{
			return content_temperature * content_heat_capacity / (content_heat_capacity + building_heat_capacity) + building_Temperature * building_heat_capacity / (content_heat_capacity + building_heat_capacity);
		}
		return num2;
	}

	private static float GetFinalBuildingTemperature(float content_temperature, float content_final_temperature, float content_heat_capacity, float building_temperature, float building_heat_capacity)
	{
		float num = (content_temperature - content_final_temperature) * content_heat_capacity;
		float num2 = Mathf.Min(content_temperature, building_temperature);
		float num3 = Mathf.Max(content_temperature, building_temperature);
		float num4 = num / building_heat_capacity;
		return Mathf.Clamp(building_temperature + num4, num2, num3);
	}

	private const string loopAnimName = "on";

	private const string loopAnim_noWater = "off";

	private GameStateMachine<ContactConductivePipeBridge, ContactConductivePipeBridge.Instance, IStateMachineTarget, ContactConductivePipeBridge.Def>.State withLiquid;

	private GameStateMachine<ContactConductivePipeBridge, ContactConductivePipeBridge.Instance, IStateMachineTarget, ContactConductivePipeBridge.Def>.State noLiquid;

	private StateMachine<ContactConductivePipeBridge, ContactConductivePipeBridge.Instance, IStateMachineTarget, ContactConductivePipeBridge.Def>.FloatParameter noLiquidTimer;

	public class Def : StateMachine.BaseDef
	{
		public ConduitType type = ConduitType.Liquid;

		public float pumpKGRate;
	}

	public new class Instance : GameStateMachine<ContactConductivePipeBridge, ContactConductivePipeBridge.Instance, IStateMachineTarget, ContactConductivePipeBridge.Def>.GameInstance
	{
		public Tag tag
		{
			get
			{
				if (this.type != ConduitType.Liquid)
				{
					return GameTags.Gas;
				}
				return GameTags.Liquid;
			}
		}

		public Instance(IStateMachineTarget master, ContactConductivePipeBridge.Def def)
			: base(master, def)
		{
		}

		public override void StartSM()
		{
			base.StartSM();
			this.inputCell = this.building.GetUtilityInputCell();
			this.outputCell = this.building.GetUtilityOutputCell();
			this.structureHandle = GameComps.StructureTemperatures.GetHandle(base.gameObject);
			Conduit.GetFlowManager(this.type).AddConduitUpdater(new Action<float>(this.Flow), ConduitFlowPriority.Default);
		}

		protected override void OnCleanUp()
		{
			base.OnCleanUp();
			Conduit.GetFlowManager(this.type).RemoveConduitUpdater(new Action<float>(this.Flow));
		}

		private void Flow(float dt)
		{
			ConduitFlow flowManager = Conduit.GetFlowManager(this.type);
			if (flowManager.HasConduit(this.inputCell) && flowManager.HasConduit(this.outputCell))
			{
				ConduitFlow.ConduitContents contents = flowManager.GetContents(this.inputCell);
				ConduitFlow.ConduitContents contents2 = flowManager.GetContents(this.outputCell);
				float num = Mathf.Min(contents.mass, base.def.pumpKGRate * dt);
				if (flowManager.CanMergeContents(contents, contents2, num))
				{
					base.smi.sm.noLiquidTimer.Set(1.5f, base.smi, false);
					float amountAllowedForMerging = flowManager.GetAmountAllowedForMerging(contents, contents2, num);
					if (amountAllowedForMerging > 0f)
					{
						float num2 = this.ExchangeStorageTemperatureWithBuilding(contents, amountAllowedForMerging, dt);
						float num3 = ((base.def.type == ConduitType.Liquid) ? Game.Instance.liquidConduitFlow : Game.Instance.gasConduitFlow).AddElement(this.outputCell, contents.element, amountAllowedForMerging, num2, contents.diseaseIdx, contents.diseaseCount);
						if (amountAllowedForMerging != num3)
						{
							global::Debug.Log("Mass Differs By: " + (amountAllowedForMerging - num3).ToString());
						}
						flowManager.RemoveElement(this.inputCell, num3);
					}
				}
			}
		}

		private float ExchangeStorageTemperatureWithBuilding(ConduitFlow.ConduitContents content, float mass, float dt)
		{
			PrimaryElement component = this.building.GetComponent<PrimaryElement>();
			float num = component.Element.thermalConductivity * this.building.Def.ThermalConductivity;
			if (mass > 0f)
			{
				Element element = ElementLoader.FindElementByHash(content.element);
				float num2 = mass * element.specificHeatCapacity;
				float num3 = this.building.Def.MassForTemperatureModification * component.Element.specificHeatCapacity;
				float temperature = component.Temperature;
				float temperature2 = content.temperature;
				float killoJoulesTransfered = ContactConductivePipeBridge.GetKilloJoulesTransfered(ContactConductivePipeBridge.CalculateMaxWattsTransfered(temperature, num, temperature2, element.thermalConductivity), dt, temperature, num3, temperature2, num2);
				float finalContentTemperature = ContactConductivePipeBridge.GetFinalContentTemperature(killoJoulesTransfered, temperature, num3, temperature2, num2);
				float finalBuildingTemperature = ContactConductivePipeBridge.GetFinalBuildingTemperature(temperature2, finalContentTemperature, num2, temperature, num3);
				if ((finalBuildingTemperature >= 0f && finalBuildingTemperature <= 10000f) & (finalContentTemperature >= 0f && finalContentTemperature <= 10000f))
				{
					GameComps.StructureTemperatures.ProduceEnergy(base.smi.structureHandle, killoJoulesTransfered, BUILDING.STATUSITEMS.OPERATINGENERGY.PIPECONTENTS_TRANSFER, Time.time);
					return finalContentTemperature;
				}
			}
			return 0f;
		}

		public ConduitType type = ConduitType.Liquid;

		public HandleVector<int>.Handle structureHandle;

		public int inputCell = -1;

		public int outputCell = -1;

		[MyCmpGet]
		public Building building;
	}
}
