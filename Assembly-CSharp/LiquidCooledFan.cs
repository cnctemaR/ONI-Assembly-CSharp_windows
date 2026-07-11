using System;
using System.Collections.Generic;
using Klei;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LiquidCooledFan : StateMachineComponent<LiquidCooledFan.StatesInstance>, IEffectDescriptor
{
	public bool HasMaterial()
	{
		ListPool<GameObject, LiquidCooledFan>.PooledList pooledList = ListPool<GameObject, LiquidCooledFan>.Allocate();
		base.smi.master.gasStorage.Find(GameTags.Water, pooledList);
		if (pooledList.Count > 0)
		{
			global::Debug.LogWarning("Liquid Cooled fan Gas storage contains water - A duplicant probably delivered to the wrong storage - moving it to liquid storage.", null);
			foreach (GameObject gameObject in pooledList)
			{
				base.smi.master.gasStorage.Transfer(gameObject, base.smi.master.liquidStorage, false, false);
			}
		}
		pooledList.Recycle();
		this.UpdateMeter();
		return this.liquidStorage.MassStored() > 0f;
	}

	public void CheckWorking()
	{
		if (base.smi.master.workable.worker == null)
		{
			base.smi.GoTo(base.smi.sm.unworkable);
		}
	}

	private void UpdateUnworkableStatusItems()
	{
		KSelectable component = base.GetComponent<KSelectable>();
		if (!base.smi.EnvironmentNeedsCooling())
		{
			if (!component.HasStatusItem(Db.Get().BuildingStatusItems.CannotCoolFurther))
			{
				component.AddStatusItem(Db.Get().BuildingStatusItems.CannotCoolFurther, null);
			}
		}
		else if (component.HasStatusItem(Db.Get().BuildingStatusItems.CannotCoolFurther))
		{
			component.RemoveStatusItem(Db.Get().BuildingStatusItems.CannotCoolFurther, false);
		}
		if (!base.smi.EnvironmentHighEnoughPressure())
		{
			if (!component.HasStatusItem(Db.Get().BuildingStatusItems.UnderPressure))
			{
				component.AddStatusItem(Db.Get().BuildingStatusItems.UnderPressure, null);
			}
		}
		else if (component.HasStatusItem(Db.Get().BuildingStatusItems.UnderPressure))
		{
			component.RemoveStatusItem(Db.Get().BuildingStatusItems.UnderPressure, false);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Behind, new string[] { "meter_target", "meter_waterbody", "meter_waterlevel" });
		base.GetComponent<ElementConsumer>().EnableConsumption(true);
		base.smi.StartSM();
		base.smi.master.waterConsumptionAccumulator = Game.Instance.accumulators.Add("waterConsumptionAccumulator", this);
		base.GetComponent<ElementConsumer>().storage = this.gasStorage;
		base.GetComponent<ManualDeliveryKG>().SetStorage(this.liquidStorage);
	}

	private void UpdateMeter()
	{
		this.meter.SetPositionPercent(Mathf.Clamp01(this.liquidStorage.MassStored() / this.liquidStorage.capacityKg));
	}

	private void EmitContents()
	{
		if (this.gasStorage.items.Count == 0)
		{
			return;
		}
		float num = 0.1f;
		float num2 = num;
		PrimaryElement primaryElement = null;
		for (int i = 0; i < this.gasStorage.items.Count; i++)
		{
			PrimaryElement component = this.gasStorage.items[i].GetComponent<PrimaryElement>();
			if (component.Mass > num2 && component.Element.IsGas)
			{
				primaryElement = component;
				num2 = primaryElement.Mass;
			}
		}
		if (primaryElement != null)
		{
			SimMessages.AddRemoveSubstance(Grid.CellRight(Grid.CellAbove(Grid.PosToCell(base.gameObject))), ElementLoader.GetElementIndex(primaryElement.ElementID), CellEventLogger.Instance.ExhaustSimUpdate, primaryElement.Mass, primaryElement.Temperature, primaryElement.DiseaseIdx, primaryElement.DiseaseCount, -1);
			this.gasStorage.ConsumeIgnoringDisease(primaryElement.gameObject);
		}
	}

	private void CoolContents(float dt)
	{
		if (this.gasStorage.items.Count == 0)
		{
			return;
		}
		float num = float.PositiveInfinity;
		float num2 = 0f;
		foreach (GameObject gameObject in this.gasStorage.items)
		{
			PrimaryElement primaryElement = gameObject.GetComponent<PrimaryElement>();
			if (!(primaryElement == null) && primaryElement.Mass >= 0.1f && primaryElement.Temperature >= this.minCooledTemperature)
			{
				float thermalEnergy = GameUtil.GetThermalEnergy(primaryElement);
				if (num > thermalEnergy)
				{
					num = thermalEnergy;
				}
			}
		}
		foreach (GameObject gameObject2 in this.gasStorage.items)
		{
			PrimaryElement primaryElement = gameObject2.GetComponent<PrimaryElement>();
			if (!(primaryElement == null) && primaryElement.Mass >= 0.1f && primaryElement.Temperature >= this.minCooledTemperature)
			{
				float num3 = Mathf.Min(num, 10f);
				GameUtil.DeltaThermalEnergy(primaryElement, -num3);
				num2 += num3;
			}
		}
		float num4 = Mathf.Abs(num2 * this.waterKGConsumedPerKJ);
		Game.Instance.accumulators.Accumulate(base.smi.master.waterConsumptionAccumulator, num4);
		if (num4 != 0f)
		{
			SimUtil.DiseaseInfo diseaseInfo;
			float num5;
			this.liquidStorage.ConsumeAndGetDisease(GameTags.Water, num4, out diseaseInfo, out num5);
			SimMessages.ModifyDiseaseOnCell(Grid.PosToCell(base.gameObject), diseaseInfo.idx, diseaseInfo.count);
			this.UpdateMeter();
		}
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.HEATCONSUMED, GameUtil.GetFormattedWattage(this.coolingKilowatts, GameUtil.WattageFormatterUnit.Automatic)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.HEATCONSUMED, GameUtil.GetFormattedJoules(this.coolingKilowatts, "F1", GameUtil.TimeSlice.None)), Descriptor.DescriptorType.Effect);
		list.Add(descriptor);
		return list;
	}

	[SerializeField]
	public float coolingKilowatts;

	[SerializeField]
	public float minCooledTemperature;

	[SerializeField]
	public float minEnvironmentMass;

	[SerializeField]
	public float waterKGConsumedPerKJ;

	[SerializeField]
	public Vector2I minCoolingRange;

	[SerializeField]
	public Vector2I maxCoolingRange;

	private float flowRate = 0.3f;

	[SerializeField]
	public Storage gasStorage;

	[SerializeField]
	public Storage liquidStorage;

	[MyCmpAdd]
	private LiquidCooledFanWorkable workable;

	[MyCmpGet]
	private Operational operational;

	private HandleVector<int>.Handle waterConsumptionAccumulator = HandleVector<int>.InvalidHandle;

	private MeterController meter;

	public class StatesInstance : GameStateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan, object>.GameInstance
	{
		public StatesInstance(LiquidCooledFan smi)
			: base(smi)
		{
		}

		public bool IsWorkable()
		{
			bool flag = false;
			if (base.master.operational.IsOperational && this.EnvironmentNeedsCooling() && base.smi.master.HasMaterial() && base.smi.EnvironmentHighEnoughPressure())
			{
				flag = true;
			}
			return flag;
		}

		public bool EnvironmentNeedsCooling()
		{
			bool flag = false;
			int num = Grid.PosToCell(base.transform.GetPosition());
			for (int i = base.master.minCoolingRange.y; i < base.master.maxCoolingRange.y; i++)
			{
				for (int j = base.master.minCoolingRange.x; j < base.master.maxCoolingRange.x; j++)
				{
					CellOffset cellOffset = new CellOffset(j, i);
					int num2 = Grid.OffsetCell(num, cellOffset);
					if (Grid.Temperature[num2] > base.master.minCooledTemperature)
					{
						flag = true;
						break;
					}
				}
			}
			return flag;
		}

		public bool EnvironmentHighEnoughPressure()
		{
			int num = Grid.PosToCell(base.transform.GetPosition());
			for (int i = base.master.minCoolingRange.y; i < base.master.maxCoolingRange.y; i++)
			{
				for (int j = base.master.minCoolingRange.x; j < base.master.maxCoolingRange.x; j++)
				{
					CellOffset cellOffset = new CellOffset(j, i);
					int num2 = Grid.OffsetCell(num, cellOffset);
					if (Grid.Mass[num2] >= base.master.minEnvironmentMass)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	public class States : GameStateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.unworkable;
			this.root.Enter(delegate(LiquidCooledFan.StatesInstance smi)
			{
				smi.master.workable.SetWorkTime(float.PositiveInfinity);
			});
			this.workable.ToggleChore(new Func<LiquidCooledFan.StatesInstance, Chore>(this.CreateUseChore), this.work_pst).EventTransition(GameHashes.ActiveChanged, this.workable.consuming, (LiquidCooledFan.StatesInstance smi) => smi.master.workable.worker != null).EventTransition(GameHashes.OperationalChanged, this.workable.consuming, (LiquidCooledFan.StatesInstance smi) => smi.master.workable.worker != null)
				.Transition(this.unworkable, (LiquidCooledFan.StatesInstance smi) => !smi.IsWorkable(), UpdateRate.SIM_200ms);
			this.work_pst.Update("LiquidFanEmitCooledContents", delegate(LiquidCooledFan.StatesInstance smi, float dt)
			{
				smi.master.EmitContents();
			}, UpdateRate.SIM_200ms, false).ScheduleGoTo(2f, this.unworkable);
			this.unworkable.Update("LiquidFanEmitCooledContents", delegate(LiquidCooledFan.StatesInstance smi, float dt)
			{
				smi.master.EmitContents();
			}, UpdateRate.SIM_200ms, false).Update("LiquidFanUnworkableStatusItems", delegate(LiquidCooledFan.StatesInstance smi, float dt)
			{
				smi.master.UpdateUnworkableStatusItems();
			}, UpdateRate.SIM_200ms, false).Transition(this.workable.waiting, (LiquidCooledFan.StatesInstance smi) => smi.IsWorkable(), UpdateRate.SIM_200ms)
				.Enter(delegate(LiquidCooledFan.StatesInstance smi)
				{
					smi.master.UpdateUnworkableStatusItems();
				})
				.Exit(delegate(LiquidCooledFan.StatesInstance smi)
				{
					smi.master.UpdateUnworkableStatusItems();
				});
			this.workable.consuming.EventTransition(GameHashes.OperationalChanged, this.unworkable, (LiquidCooledFan.StatesInstance smi) => smi.master.workable.worker == null).EventHandler(GameHashes.ActiveChanged, delegate(LiquidCooledFan.StatesInstance smi)
			{
				smi.master.CheckWorking();
			}).Enter(delegate(LiquidCooledFan.StatesInstance smi)
			{
				if (!smi.EnvironmentNeedsCooling() || !smi.master.HasMaterial() || !smi.EnvironmentHighEnoughPressure())
				{
					smi.GoTo(this.unworkable);
				}
				ElementConsumer component = smi.master.GetComponent<ElementConsumer>();
				component.consumptionRate = smi.master.flowRate;
				component.RefreshConsumptionRate();
			})
				.Update(delegate(LiquidCooledFan.StatesInstance smi, float dt)
				{
					smi.master.CoolContents(dt);
				}, UpdateRate.SIM_200ms, false)
				.ScheduleGoTo(12f, this.workable.emitting)
				.Exit(delegate(LiquidCooledFan.StatesInstance smi)
				{
					ElementConsumer component2 = smi.master.GetComponent<ElementConsumer>();
					component2.consumptionRate = 0f;
					component2.RefreshConsumptionRate();
				});
			this.workable.emitting.EventTransition(GameHashes.ActiveChanged, this.unworkable, (LiquidCooledFan.StatesInstance smi) => smi.master.workable.worker == null).EventTransition(GameHashes.OperationalChanged, this.unworkable, (LiquidCooledFan.StatesInstance smi) => smi.master.workable.worker == null).ScheduleGoTo(3f, this.workable.consuming)
				.Update("LiquidFanEmitCooledContents", delegate(LiquidCooledFan.StatesInstance smi, float dt)
				{
					smi.master.EmitContents();
				}, UpdateRate.SIM_200ms, false);
			this.workable.emitting.EventTransition(GameHashes.ActiveChanged, this.unworkable, (LiquidCooledFan.StatesInstance smi) => smi.master.workable.worker == null).EventTransition(GameHashes.OperationalChanged, this.unworkable, (LiquidCooledFan.StatesInstance smi) => smi.master.workable.worker == null).ScheduleGoTo(3f, this.workable.consuming)
				.Update(delegate(LiquidCooledFan.StatesInstance smi, float dt)
				{
					smi.master.CoolContents(dt);
				}, UpdateRate.SIM_200ms, false)
				.Update("LiquidFanEmitCooledContents", delegate(LiquidCooledFan.StatesInstance smi, float dt)
				{
					smi.master.EmitContents();
				}, UpdateRate.SIM_200ms, false);
		}

		private Chore CreateUseChore(LiquidCooledFan.StatesInstance smi)
		{
			return new WorkChore<LiquidCooledFanWorkable>(Db.Get().ChoreTypes.LiquidCooledFan, smi.master.workable, null, null, true, null, null, null, true, null, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 0, false);
		}

		public LiquidCooledFan.States.Workable workable;

		public GameStateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan, object>.State unworkable;

		public GameStateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan, object>.State work_pst;

		public class Workable : GameStateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan, object>.State
		{
			public GameStateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan, object>.State waiting;

			public GameStateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan, object>.State consuming;

			public GameStateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan, object>.State emitting;
		}
	}
}
