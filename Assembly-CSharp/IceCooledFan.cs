using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class IceCooledFan : StateMachineComponent<IceCooledFan.StatesInstance>
{
	public bool HasMaterial()
	{
		this.UpdateMeter();
		return this.iceStorage.MassStored() > 0f;
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
				return;
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
		this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[] { "meter_target", "meter_waterbody", "meter_waterlevel" });
		base.smi.StartSM();
		base.GetComponent<ManualDeliveryKG>().SetStorage(this.iceStorage);
	}

	private void UpdateMeter()
	{
		float num = 0f;
		foreach (GameObject gameObject in this.iceStorage.items)
		{
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			num += component.Temperature;
		}
		num /= (float)this.iceStorage.items.Count;
		float num2 = Mathf.Clamp01((num - this.LOW_ICE_TEMP) / (this.targetTemperature - this.LOW_ICE_TEMP));
		this.meter.SetPositionPercent(1f - num2);
	}

	private void DoCooling(float dt)
	{
		float num = this.coolingRate * dt;
		foreach (GameObject gameObject in this.iceStorage.items)
		{
			GameUtil.DeltaThermalEnergy(gameObject.GetComponent<PrimaryElement>(), num, this.targetTemperature);
		}
		for (int i = this.iceStorage.items.Count; i > 0; i--)
		{
			GameObject gameObject2 = this.iceStorage.items[i - 1];
			if (gameObject2 != null && gameObject2.GetComponent<PrimaryElement>().Temperature > gameObject2.GetComponent<PrimaryElement>().Element.highTemp && gameObject2.GetComponent<PrimaryElement>().Element.HasTransitionUp)
			{
				PrimaryElement component = gameObject2.GetComponent<PrimaryElement>();
				this.iceStorage.AddLiquid(component.Element.highTempTransitionTarget, component.Mass, component.Temperature, component.DiseaseIdx, component.DiseaseCount, false, true);
				this.iceStorage.ConsumeIgnoringDisease(gameObject2);
			}
		}
		for (int j = this.iceStorage.items.Count; j > 0; j--)
		{
			GameObject gameObject3 = this.iceStorage.items[j - 1];
			if (gameObject3 != null && gameObject3.GetComponent<PrimaryElement>().Temperature >= this.targetTemperature)
			{
				this.iceStorage.Transfer(gameObject3, this.liquidStorage, true, true);
			}
		}
		if (!this.liquidStorage.IsEmpty())
		{
			this.liquidStorage.DropAll(false, false, new Vector3(1f, 0f, 0f), true, null);
		}
		this.UpdateMeter();
	}

	[SerializeField]
	public float minCooledTemperature;

	[SerializeField]
	public float minEnvironmentMass;

	[SerializeField]
	public float coolingRate;

	[SerializeField]
	public float targetTemperature;

	[SerializeField]
	public Vector2I minCoolingRange;

	[SerializeField]
	public Vector2I maxCoolingRange;

	[SerializeField]
	public Storage iceStorage;

	[SerializeField]
	public Storage liquidStorage;

	[SerializeField]
	public Tag consumptionTag;

	private float LOW_ICE_TEMP = 173.15f;

	[MyCmpAdd]
	private IceCooledFanWorkable workable;

	[MyCmpGet]
	private Operational operational;

	private MeterController meter;

	public class StatesInstance : GameStateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.GameInstance
	{
		public StatesInstance(IceCooledFan smi)
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

	public class States : GameStateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.unworkable;
			this.root.Enter(delegate(IceCooledFan.StatesInstance smi)
			{
				smi.master.workable.SetWorkTime(float.PositiveInfinity);
			});
			this.workable.ToggleChore(new Func<IceCooledFan.StatesInstance, Chore>(this.CreateUseChore), this.work_pst).EventTransition(GameHashes.ActiveChanged, this.workable.cooling, (IceCooledFan.StatesInstance smi) => smi.master.workable.worker != null).EventTransition(GameHashes.OperationalChanged, this.workable.cooling, (IceCooledFan.StatesInstance smi) => smi.master.workable.worker != null)
				.Transition(this.unworkable, (IceCooledFan.StatesInstance smi) => !smi.IsWorkable(), UpdateRate.SIM_200ms);
			this.workable.cooling.EventTransition(GameHashes.OperationalChanged, this.unworkable, (IceCooledFan.StatesInstance smi) => smi.master.workable.worker == null).EventHandler(GameHashes.ActiveChanged, delegate(IceCooledFan.StatesInstance smi)
			{
				smi.master.CheckWorking();
			}).Enter(delegate(IceCooledFan.StatesInstance smi)
			{
				smi.master.gameObject.GetComponent<ManualDeliveryKG>().Pause(true, "Working");
				if (!smi.EnvironmentNeedsCooling() || !smi.master.HasMaterial() || !smi.EnvironmentHighEnoughPressure())
				{
					smi.GoTo(this.unworkable);
				}
			})
				.Update("IceCooledFanCooling", delegate(IceCooledFan.StatesInstance smi, float dt)
				{
					smi.master.DoCooling(dt);
				}, UpdateRate.SIM_200ms, false)
				.Exit(delegate(IceCooledFan.StatesInstance smi)
				{
					if (!smi.master.HasMaterial())
					{
						smi.master.gameObject.GetComponent<ManualDeliveryKG>().Pause(false, "Working");
					}
					smi.master.liquidStorage.DropAll(false, false, default(Vector3), true, null);
				});
			this.work_pst.ScheduleGoTo(2f, this.unworkable);
			this.unworkable.Update("IceFanUnworkableStatusItems", delegate(IceCooledFan.StatesInstance smi, float dt)
			{
				smi.master.UpdateUnworkableStatusItems();
			}, UpdateRate.SIM_200ms, false).Transition(this.workable.waiting, (IceCooledFan.StatesInstance smi) => smi.IsWorkable(), UpdateRate.SIM_200ms).Enter(delegate(IceCooledFan.StatesInstance smi)
			{
				smi.master.UpdateUnworkableStatusItems();
			})
				.Exit(delegate(IceCooledFan.StatesInstance smi)
				{
					smi.master.UpdateUnworkableStatusItems();
				});
		}

		private Chore CreateUseChore(IceCooledFan.StatesInstance smi)
		{
			return new WorkChore<IceCooledFanWorkable>(Db.Get().ChoreTypes.IceCooledFan, smi.master.workable, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
		}

		public IceCooledFan.States.Workable workable;

		public GameStateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.State unworkable;

		public GameStateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.State work_pst;

		public class Workable : GameStateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.State
		{
			public GameStateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.State waiting;

			public GameStateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.State cooling;
		}
	}
}
