using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LiquidCooledFan : StateMachineComponent<LiquidCooledFan.StatesInstance>, IEffectDescriptor
{
	public bool HasMaterial()
	{
		Storage component = base.GetComponent<Storage>();
		ManualDeliveryKG[] components = base.GetComponents<ManualDeliveryKG>();
		foreach (ManualDeliveryKG manualDeliveryKG in components)
		{
			List<PrimaryElement> list = component.FindPrimaryElements(manualDeliveryKG.requestedItemTag);
			if (list.Count == 0)
			{
				return false;
			}
		}
		return true;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Behind, new string[] { "meter_target", "meter_waterbody", "meter_waterlevel" });
		base.smi.StartSM();
	}

	private void CoolEnvironment(float dt)
	{
		int num = Grid.PosToCell(this.transform.position);
		float num2 = this.coolingKilowatts * dt;
		Vector2I vector2I = this.maxCoolingRange - this.minCoolingRange;
		float num3 = num2 / (float)(vector2I.x * vector2I.y);
		for (int i = this.minCoolingRange.y; i < this.maxCoolingRange.y; i++)
		{
			for (int j = this.minCoolingRange.x; j < this.maxCoolingRange.x; j++)
			{
				CellOffset cellOffset = new CellOffset(j, i);
				int num4 = Grid.OffsetCell(num, cellOffset);
				if (Grid.Cell[num4].temperature > this.minCooledTemperature)
				{
					SimMessages.ModifyEnergy(num4, -num3, SimMessages.EnergySourceID.LiquidCooledFan);
				}
				if (num2 <= 0f)
				{
					break;
				}
			}
		}
	}

	public int DescriptionOrder { get; set; }

	public List<Descriptor> GetRequirementDescriptions(BuildingDef def)
	{
		return null;
	}

	public List<Descriptor> GetEffectDescriptions(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(string.Format(UI.LISTENTRYSTRINGNOLINEBREAK, string.Format(UI.BUILDINGEFFECTS.HEATCONSUMED, GameUtil.GetFormattedWattage(this.coolingKilowatts))), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.HEATCONSUMED, GameUtil.GetFormattedWattage(this.coolingKilowatts)));
		list.Add(descriptor);
		return list;
	}

	[SerializeField]
	public float coolingKilowatts = 10f;

	[SerializeField]
	public float minCooledTemperature = 287f;

	[SerializeField]
	public Vector2I minCoolingRange;

	[SerializeField]
	public Vector2I maxCoolingRange;

	[MyCmpAdd]
	private Storage storage;

	[MyCmpAdd]
	private LiquidCooledFanWorkable workable;

	[MyCmpGet]
	private Operational operational;

	private MeterController meter;

	public class StatesInstance : GameStateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan>.GameInstance
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
	}

	public class States : GameStateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.cooled;
			this.root.Update("Metering", delegate(LiquidCooledFan.StatesInstance smi)
			{
				float num = Mathf.Clamp01(smi.master.storage.MassStored() / smi.master.storage.capacityKg);
				smi.master.meter.SetPositionPercent(num);
			}).Transition(this.cooled, (LiquidCooledFan.StatesInstance smi) => !smi.EnvironmentNeedsCooling());
			this.cooled.Transition(this.needsmass, (LiquidCooledFan.StatesInstance smi) => smi.EnvironmentNeedsCooling()).ToggleStatusItem(Db.Get().BuildingStatusItems.CannotCoolFurther, null);
			this.needsmass.EventTransition(GameHashes.OnStorageChange, this.workable, (LiquidCooledFan.StatesInstance smi) => smi.IsWorkable());
			this.workable.DefaultState(this.workable.waiting).EventTransition(GameHashes.OnStorageChange, this.cooled, (LiquidCooledFan.StatesInstance smi) => !smi.IsWorkable()).EventTransition(GameHashes.OperationalChanged, this.cooled, (LiquidCooledFan.StatesInstance smi) => !smi.IsWorkable())
				.ToggleChore(new Func<LiquidCooledFan.StatesInstance, Chore>(this.CreateUseChore), this.cooled, false);
			this.workable.waiting.ToggleStatusItem(Db.Get().BuildingStatusItems.PendingWork, null).EventTransition(GameHashes.ActiveChanged, this.workable.cooling, (LiquidCooledFan.StatesInstance smi) => smi.master.operational.IsActive);
			this.workable.cooling.ToggleStatusItem(Db.Get().BuildingStatusItems.Cooling, null).Update("Cooling", delegate(LiquidCooledFan.StatesInstance smi)
			{
				smi.master.CoolEnvironment(smi.dt);
			}).EventTransition(GameHashes.ActiveChanged, this.workable.waiting, (LiquidCooledFan.StatesInstance smi) => !smi.master.operational.IsActive);
		}

		private Chore CreateUseChore(LiquidCooledFan.StatesInstance smi)
		{
			return new WorkChore<LiquidCooledFanWorkable>(Db.Get().ChoreTypes.LiquidCooledFan, smi.master.workable, null, true, null, null, null, true, null, true, default(Tag), null, false, true);
		}

		public GameStateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan>.State cooled;

		public GameStateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan>.State needsmass;

		public LiquidCooledFan.States.Workable workable;

		public class Workable : GameStateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan>.State
		{
			public GameStateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan>.State waiting;

			public GameStateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan>.State cooling;
		}
	}
}
