using System;
using System.Collections.Generic;
using UnityEngine;

public class ContactConductivePipeBridge : GameStateMachine<ContactConductivePipeBridge, ContactConductivePipeBridge.Instance, IStateMachineTarget, ContactConductivePipeBridge.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.noLiquid;
		this.root.PlayAnim("on", KAnim.PlayMode.Loop).Update("", new Action<ContactConductivePipeBridge.Instance, float>(ContactConductivePipeBridge.Flow200ms), UpdateRate.SIM_200ms, false);
	}

	private static void ExpirationTimerUpdate(ContactConductivePipeBridge.Instance smi, float dt)
	{
		float num = smi.sm.noLiquidTimer.Get(smi);
		num -= dt;
		smi.sm.noLiquidTimer.Set(num, smi, false);
	}

	private static void Flow200ms(ContactConductivePipeBridge.Instance smi, float dt)
	{
		if (smi.storage != null && smi.storage.items.Count > 0)
		{
			ContactConductivePipeBridge.ExchangeStorageTemperatureWithBuilding200ms(smi, smi.storage, smi.building, smi.tag, dt);
			List<GameObject> items = smi.storage.items;
			for (int i = 0; i < items.Count; i++)
			{
				PrimaryElement component = items[i].GetComponent<PrimaryElement>();
				if (component.Mass > 0f)
				{
					float num = ((smi.def.type == ConduitType.Liquid) ? Game.Instance.liquidConduitFlow : Game.Instance.gasConduitFlow).AddElement(smi.outputCell, component.ElementID, component.Mass, component.Temperature, component.DiseaseIdx, component.DiseaseCount);
					component.KeepZeroMassObject = true;
					float num2 = num / component.Mass;
					int num3 = (int)((float)component.DiseaseCount * num2);
					component.Mass -= num;
					component.ModifyDiseaseCount(-num3, "ContactConductivePipeBridge.Flow200ms");
				}
			}
		}
	}

	private static void ExchangeStorageTemperatureWithBuilding200ms(ContactConductivePipeBridge.Instance smi, Storage storage, Building building, Tag tag, float dt)
	{
		List<GameObject> items = storage.items;
		PrimaryElement component = building.GetComponent<PrimaryElement>();
		float num = component.Element.thermalConductivity * building.Def.ThermalConductivity;
		for (int i = 0; i < items.Count; i++)
		{
			PrimaryElement component2 = items[i].GetComponent<PrimaryElement>();
			if (component2.Mass > 0f && component2.HasTag(tag))
			{
				PrimaryElement primaryElement = component2;
				float num2 = primaryElement.Mass * primaryElement.Element.specificHeatCapacity;
				float num3 = building.Def.MassForTemperatureModification * component.Element.specificHeatCapacity;
				float temperature = component.Temperature;
				float temperature2 = primaryElement.Temperature;
				float finalContentTemperature = ContactConductivePipeBridge.GetFinalContentTemperature(ContactConductivePipeBridge.GetKilloJoulesTransfered(ContactConductivePipeBridge.CalculateMaxWattsTransfered(temperature, num, temperature2, primaryElement.Element.thermalConductivity), dt, temperature, num3, temperature2, num2), temperature, num3, temperature2, num2);
				float finalBuildingTemperature = ContactConductivePipeBridge.GetFinalBuildingTemperature(temperature2, finalContentTemperature, num2, temperature, num3);
				if ((finalBuildingTemperature >= 0f && finalBuildingTemperature <= 10000f) & (finalContentTemperature >= 0f && finalContentTemperature <= 10000f))
				{
					primaryElement.Temperature = finalContentTemperature;
					component.Temperature = finalBuildingTemperature;
				}
			}
		}
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
			this.outputCell = this.building.GetUtilityOutputCell();
			this.structureHandle = GameComps.StructureTemperatures.GetHandle(base.gameObject);
		}

		protected override void OnCleanUp()
		{
			base.OnCleanUp();
			this.storage.DropAll(false, false, default(Vector3), true, null);
		}

		public ConduitType type = ConduitType.Liquid;

		public HandleVector<int>.Handle structureHandle;

		public int outputCell = -1;

		[MyCmpGet]
		public Storage storage;

		[MyCmpGet]
		public Building building;

		[MyCmpGet]
		public ConduitDispenser conduitDispenser;
	}
}
