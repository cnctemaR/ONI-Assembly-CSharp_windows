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
		this.schedule_fart.ScheduleGoTo((CaloriesConsumedSecondaryExcretionMonitor.Instance smi) => global::UnityEngine.Random.Range(3f, 6f), this.needs_to_fart);
		this.needs_to_fart.Enter(new StateMachine<CaloriesConsumedSecondaryExcretionMonitor, CaloriesConsumedSecondaryExcretionMonitor.Instance, IStateMachineTarget, object>.State.Callback(CaloriesConsumedSecondaryExcretionMonitor.CreateChore)).ToggleUrge(Db.Get().Urges.Fart).EventHandler(GameHashes.BeginChore, delegate(CaloriesConsumedSecondaryExcretionMonitor.Instance smi, object o)
		{
			smi.OnStartChore(o);
		});
	}

	public static void CreateChore(CaloriesConsumedSecondaryExcretionMonitor.Instance smi)
	{
		CreatureCalorieMonitor.CaloriesConsumedEvent consumptionData = smi.consumptionData;
		new FartChore(smi.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.Fart, consumptionData.calories * 0.001f * smi.sm.kgProducedPerKcalConsumed, smi.sm.producedElement, byte.MaxValue, 0, smi.sm.overpressureThreshold);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return new List<Descriptor>
		{
			new Descriptor(UI.BUILDINGEFFECTS.DIET_ADDITIONAL_PRODUCED.Replace("{Items}", ElementLoader.GetElement(this.producedElement.CreateTag()).name), UI.BUILDINGEFFECTS.TOOLTIPS.DIET_ADDITIONAL_PRODUCED.Replace("{Items}", ElementLoader.GetElement(this.producedElement.CreateTag()).name), Descriptor.DescriptorType.Effect, false)
		};
	}

	public GameStateMachine<CaloriesConsumedSecondaryExcretionMonitor, CaloriesConsumedSecondaryExcretionMonitor.Instance, IStateMachineTarget, object>.State idle;

	public GameStateMachine<CaloriesConsumedSecondaryExcretionMonitor, CaloriesConsumedSecondaryExcretionMonitor.Instance, IStateMachineTarget, object>.State schedule_fart;

	public GameStateMachine<CaloriesConsumedSecondaryExcretionMonitor, CaloriesConsumedSecondaryExcretionMonitor.Instance, IStateMachineTarget, object>.State needs_to_fart;

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

		public void OnStartChore(object o)
		{
			if (((Chore)o).SatisfiesUrge(Db.Get().Urges.Fart))
			{
				this.GoTo(base.sm.idle);
			}
		}

		public void OnCaloriesConsumed(object data)
		{
			base.smi.consumptionData = (CreatureCalorieMonitor.CaloriesConsumedEvent)data;
			base.smi.GoTo(base.smi.sm.schedule_fart);
		}

		public CreatureCalorieMonitor.CaloriesConsumedEvent consumptionData;
	}
}
