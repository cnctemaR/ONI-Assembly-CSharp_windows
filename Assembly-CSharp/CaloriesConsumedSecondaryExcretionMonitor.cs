using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class CaloriesConsumedSecondaryExcretionMonitor : GameStateMachine<CaloriesConsumedSecondaryExcretionMonitor, CaloriesConsumedSecondaryExcretionMonitor.Instance>, IGameObjectEffectDescriptor
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.idle;
		base.serializable = StateMachine.SerializeType.Both_DEPRECATED;
		this.idle.PlayAnim("idle_loop", KAnim.PlayMode.Loop).Enter(delegate(CaloriesConsumedSecondaryExcretionMonitor.Instance smi)
		{
			this.handle = smi.gameObject.Subscribe(-2038961714, new Action<object>(smi.OnCaloriesConsumed));
		}).Exit(delegate(CaloriesConsumedSecondaryExcretionMonitor.Instance smi)
		{
			smi.gameObject.Unsubscribe(this.handle);
		});
		this.produce_pre.ScheduleGoTo(4f, this.produce);
		this.produce.QueueAnim("fart", false, null).OnAnimQueueComplete(this.idle).Enter(delegate(CaloriesConsumedSecondaryExcretionMonitor.Instance smi)
		{
			CreatureCalorieMonitor.CaloriesConsumedEvent consumptionData = smi.consumptionData;
			smi.DropElement(consumptionData.calories * 0.001f * smi.sm.kgProducedPerKcalConsumed, smi.sm.producedElement, byte.MaxValue, 0);
		});
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return new List<Descriptor>
		{
			new Descriptor(UI.BUILDINGEFFECTS.DIET_ADDITIONAL_PRODUCED.Replace("{Items}", ElementLoader.GetElement(this.producedElement.CreateTag()).name), UI.BUILDINGEFFECTS.TOOLTIPS.DIET_ADDITIONAL_PRODUCED.Replace("{Items}", ElementLoader.GetElement(this.producedElement.CreateTag()).name), Descriptor.DescriptorType.Effect, false)
		};
	}

	public GameStateMachine<CaloriesConsumedSecondaryExcretionMonitor, CaloriesConsumedSecondaryExcretionMonitor.Instance, IStateMachineTarget, object>.State idle;

	public GameStateMachine<CaloriesConsumedSecondaryExcretionMonitor, CaloriesConsumedSecondaryExcretionMonitor.Instance, IStateMachineTarget, object>.State produce_pre;

	public GameStateMachine<CaloriesConsumedSecondaryExcretionMonitor, CaloriesConsumedSecondaryExcretionMonitor.Instance, IStateMachineTarget, object>.State produce;

	public SimHashes producedElement;

	public float kgProducedPerKcalConsumed = 1f;

	private float overpressureThreshold = 2f;

	private int handle;

	public new class Instance : GameStateMachine<CaloriesConsumedSecondaryExcretionMonitor, CaloriesConsumedSecondaryExcretionMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void OnCaloriesConsumed(object data)
		{
			base.smi.consumptionData = (CreatureCalorieMonitor.CaloriesConsumedEvent)data;
			base.smi.GoTo(base.smi.sm.produce_pre);
		}

		public void DropElement(float mass, SimHashes element_id, byte disease_idx, int disease_count)
		{
			if (mass <= 0f)
			{
				return;
			}
			Element element = ElementLoader.FindElementByHash(element_id);
			float temperature = base.smi.master.GetComponent<PrimaryElement>().Temperature;
			if (element.IsGas || element.IsLiquid)
			{
				int num = Grid.PosToCell(base.transform.GetPosition());
				if (this.CheckIsOverpressure(num))
				{
					return;
				}
				SimMessages.AddRemoveSubstance(num, element_id, CellEventLogger.Instance.ElementConsumerSimUpdate, mass, temperature, disease_idx, disease_count, true, -1);
			}
			else if (element.IsSolid)
			{
				element.substance.SpawnResource(base.transform.GetPosition() + new Vector3(0f, 0.5f, 0f), mass, temperature, disease_idx, disease_count, false, true, false);
			}
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, element.name, base.gameObject.transform, 1.5f, false);
		}

		private bool CheckIsOverpressure(int cell)
		{
			return Grid.Mass[cell] > base.sm.overpressureThreshold;
		}

		public CreatureCalorieMonitor.CaloriesConsumedEvent consumptionData;
	}
}
