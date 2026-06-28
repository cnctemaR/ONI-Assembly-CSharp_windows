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
		List<GameObject> list = base.smi.master.gasStorage.Find(GameTags.Water);
		if (list != null && list.Count > 0)
		{
			global::Debug.LogWarning("Liquid Cooled fan Gas storage contains water - A duplicant probably delivered to the wrong stroage - moving it to liquid storage.", null);
			foreach (GameObject gameObject in list)
			{
				base.smi.master.gasStorage.Transfer(gameObject, base.smi.master.liquidStorage, false, false);
			}
		}
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
		base.smi.master.waterConsumptionAccumulator = new Accumulator("waterConsumptionAccumulator", this, 1f);
		base.GetComponent<ElementConsumer>().storage = this.gasStorage;
		base.GetComponent<ManualDeliveryKG>().SetStorage(this.liquidStorage);
	}

	private void UpdateMeter()
	{
		this.meter.SetPositionPercent(Mathf.Clamp01(this.liquidStorage.MassStored() / this.liquidStorage.capacityKg));
	}

	private void EmitContents()
	{
		if (this.gasStorage.items.Count != 0)
		{
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
	}

	private void CoolContents(float dt)
	{
		if (this.gasStorage.items.Count != 0)
		{
			float num = float.PositiveInfinity;
			float num2 = 0f;
			foreach (GameObject gameObject in this.gasStorage)
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
			foreach (GameObject gameObject2 in this.gasStorage)
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
			base.smi.master.waterConsumptionAccumulator.Accumulate(num4);
			if (num4 != 0f)
			{
				SimUtil.DiseaseInfo diseaseInfo;
				float num5;
				this.liquidStorage.ConsumeAndGetDisease(GameTags.Water, num4, out diseaseInfo, out num5);
				SimMessages.ModifyDiseaseOnCell(Grid.PosToCell(base.gameObject), diseaseInfo.idx, diseaseInfo.count);
				this.UpdateMeter();
			}
		}
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.HEATCONSUMED, GameUtil.GetFormattedWattage(this.coolingKilowatts, GameUtil.WattageFormatterUnit.Automatic)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.HEATCONSUMED, GameUtil.GetFormattedJoules(this.coolingKilowatts, "F1")), Descriptor.DescriptorType.Effect);
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

	public Accumulator waterConsumptionAccumulator;

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
			if (base.master.operational.IsOperational && base.master.HasMaterial())
			{
				flag = true;
			}
			return flag;
		}

		public bool EnvironmentNeedsCooling()
		{
			bool flag = false;
			int num = Grid.PosToCell(base.transform.position);
			for (int i = base.master.minCoolingRange.y; i < base.master.maxCoolingRange.y; i++)
			{
				for (int j = base.master.minCoolingRange.x; j < base.master.maxCoolingRange.x; j++)
				{
					CellOffset cellOffset = new CellOffset(j, i);
					int num2 = Grid.OffsetCell(num, cellOffset);
					if (Grid.Cell[num2].temperature > base.master.minCooledTemperature)
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
			int num = Grid.PosToCell(base.transform.position);
			for (int i = base.master.minCoolingRange.y; i < base.master.maxCoolingRange.y; i++)
			{
				for (int j = base.master.minCoolingRange.x; j < base.master.maxCoolingRange.x; j++)
				{
					CellOffset cellOffset = new CellOffset(j, i);
					int num2 = Grid.OffsetCell(num, cellOffset);
					if (Grid.Cell[num2].mass >= base.master.minEnvironmentMass)
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
				.Transition(this.unworkable, (LiquidCooledFan.StatesInstance smi) => !smi.master.HasMaterial())
				.Transition(this.unworkable, (LiquidCooledFan.StatesInstance smi) => !smi.EnvironmentNeedsCooling());
			this.work_pst.ToggleSchedulePeriodic("LiquidFanEmitCooledContents", 0.25f, delegate(LiquidCooledFan.StatesInstance smi)
			{
				smi.master.EmitContents();
			}).ScheduleGoTo(2f, this.unworkable);
			this.unworkable.ToggleSchedulePeriodic("LiquidFanEmitCooledContents", 0.25f, delegate(LiquidCooledFan.StatesInstance smi)
			{
				smi.master.EmitContents();
			}).ToggleSchedulePeriodic("LiquidFanUnworkableStatusItems", 0.5f, delegate(LiquidCooledFan.StatesInstance smi)
			{
				smi.master.UpdateUnworkableStatusItems();
			}).Transition(this.workable.waiting, (LiquidCooledFan.StatesInstance smi) => smi.EnvironmentNeedsCooling() && smi.master.HasMaterial() && smi.EnvironmentHighEnoughPressure())
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
				smi.master.GetComponent<ElementConsumer>().consumptionRate = smi.master.flowRate;
				smi.master.GetComponent<ElementConsumer>().RefreshConsumptionRate();
			})
				.Update(delegate(LiquidCooledFan.StatesInstance smi)
				{
					smi.master.CoolContents(smi.dt);
				})
				.ScheduleGoTo(12f, this.workable.emitting)
				.Exit(delegate(LiquidCooledFan.StatesInstance smi)
				{
					smi.master.GetComponent<ElementConsumer>().consumptionRate = 0f;
					smi.master.GetComponent<ElementConsumer>().RefreshConsumptionRate();
				});
			this.workable.emitting.EventTransition(GameHashes.ActiveChanged, this.unworkable, (LiquidCooledFan.StatesInstance smi) => smi.master.workable.worker == null).EventTransition(GameHashes.OperationalChanged, this.unworkable, (LiquidCooledFan.StatesInstance smi) => smi.master.workable.worker == null).ScheduleGoTo(3f, this.workable.consuming)
				.ToggleSchedulePeriodic("LiquidFanEmitCooledContents", 0.25f, delegate(LiquidCooledFan.StatesInstance smi)
				{
					smi.master.EmitContents();
				});
			this.workable.emitting.EventTransition(GameHashes.ActiveChanged, this.unworkable, (LiquidCooledFan.StatesInstance smi) => smi.master.workable.worker == null).EventTransition(GameHashes.OperationalChanged, this.unworkable, (LiquidCooledFan.StatesInstance smi) => smi.master.workable.worker == null).ScheduleGoTo(3f, this.workable.consuming)
				.Update(delegate(LiquidCooledFan.StatesInstance smi)
				{
					smi.master.CoolContents(smi.dt);
				})
				.ToggleSchedulePeriodic("LiquidFanEmitCooledContents", 0.25f, delegate(LiquidCooledFan.StatesInstance smi)
				{
					smi.master.EmitContents();
				});
		}

		private Chore CreateUseChore(LiquidCooledFan.StatesInstance smi)
		{
			return new WorkChore<LiquidCooledFanWorkable>(Db.Get().ChoreTypes.LiquidCooledFan, smi.master.workable, null, true, null, null, null, true, null, true, default(Tag), null, false, true, true, PriorityScreen.PriorityClass.basic, int.MaxValue);
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
