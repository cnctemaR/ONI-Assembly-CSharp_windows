using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class FertilizationMonitor : GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.wild;
		base.serializable = false;
		this.wild.ParamTransition<GameObject>(this.fertilizerStorage, this.replanted.decaying, (FertilizationMonitor.Instance smi, GameObject p) => p != null);
		this.replanted.Enter(delegate(FertilizationMonitor.Instance smi)
		{
			foreach (ManualDeliveryKG manualDeliveryKG in smi.gameObject.GetComponents<ManualDeliveryKG>())
			{
				manualDeliveryKG.Pause(false, "replanted");
			}
		});
		this.replanted.decaying.ParamTransition<GameObject>(this.fertilizerStorage, this.replanted.absorbing, (FertilizationMonitor.Instance smi, GameObject p) => smi.StorageHasFertilizer(0.2f)).ToggleAttributeModifier("Consuming", (FertilizationMonitor.Instance smi) => smi.consumptionRate).TriggerOnEnter(GameHashes.Fertilized, null)
			.Update(delegate(FertilizationMonitor.Instance smi)
			{
				if (smi.Starved())
				{
					smi.GoTo(this.replanted.starved);
				}
			})
			.Target(this.fertilizerStorage)
			.EventTransition(GameHashes.OnStorageChange, this.replanted.absorbing, (FertilizationMonitor.Instance smi) => smi.StorageHasFertilizer(0.2f));
		this.replanted.absorbing.ParamTransition<GameObject>(this.fertilizerStorage, this.replanted.decaying, (FertilizationMonitor.Instance smi, GameObject p) => !smi.StorageHasFertilizer(0.2f)).TriggerOnEnter(GameHashes.Fertilized, null).ToggleAttributeModifier("Absorbing", (FertilizationMonitor.Instance smi) => smi.absorptionRate)
			.Update(delegate(FertilizationMonitor.Instance smi)
			{
				smi.AbsorbFertilizer(smi.deltatime);
			})
			.Target(this.fertilizerStorage)
			.EventTransition(GameHashes.OnStorageChange, this.replanted.decaying, (FertilizationMonitor.Instance smi) => !smi.StorageHasFertilizer(0.2f));
		this.replanted.starved.ToggleStatusItem(Db.Get().CreatureStatusItems.NeedsFertilizer, null).TriggerOnEnter(GameHashes.Unfertilized, null).Target(this.fertilizerStorage)
			.EventTransition(GameHashes.OnStorageChange, this.replanted.absorbing, (FertilizationMonitor.Instance smi) => smi.StorageHasFertilizer(0.2f));
	}

	public StateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget>.TargetParameter fertilizerStorage;

	public GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget>.State wild;

	public FertilizationMonitor.ReplantedStates replanted;

	public class ReplantedStates : GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget>.State
	{
		public GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget>.State decaying;

		public GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget>.State absorbing;

		public GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget>.State starved;
	}

	public new class Instance : GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget>.GameInstance
	{
		public Instance(IStateMachineTarget master, ElementConverter.ConsumedElement[] consumedElements)
			: base(master)
		{
			this.consumptionRate = new AttributeModifier(Db.Get().Amounts.Fertilization.deltaAttribute.Id, -0.16666667f, CREATURES.STATS.FERTILIZATION.CONSUME_MODIFIER, false);
			this.absorptionRate = new AttributeModifier(Db.Get().Amounts.Fertilization.deltaAttribute.Id, 0.16666667f, CREATURES.STATS.FERTILIZATION.ABSORBING_MODIFIER, false);
			Crop component = master.GetComponent<Crop>();
			if (component != null)
			{
				this.SetStorage(component.PlanterStorage);
			}
			this.consumedElements = consumedElements;
			for (int i = 0; i < this.consumedElements.Length; i++)
			{
				this.consumedElements[i].accumulator = new Accumulator("ElementsConsumed", base.master.GetComponent<KPrefabID>(), 3f);
			}
		}

		public void SetStorage(Storage storage)
		{
			base.sm.fertilizerStorage.Set(storage, base.smi);
			foreach (ManualDeliveryKG manualDeliveryKG in base.smi.gameObject.GetComponents<ManualDeliveryKG>())
			{
				manualDeliveryKG.SetStorage(storage);
			}
		}

		public bool Starved()
		{
			return base.gameObject.GetAmounts().Get(Db.Get().Amounts.Fertilization).value == 0f;
		}

		public bool StorageHasFertilizer(float dt)
		{
			Storage storage = base.sm.fertilizerStorage.Get<Storage>(base.smi);
			if (storage == null || this.consumedElements == null)
			{
				return false;
			}
			bool flag = true;
			List<GameObject> items = storage.items;
			foreach (ElementConverter.ConsumedElement consumedElement in this.consumedElements)
			{
				float num = 0f;
				foreach (GameObject gameObject in items)
				{
					if (gameObject.HasTag(consumedElement.tag))
					{
						num += gameObject.GetComponent<PrimaryElement>().Mass;
					}
				}
				if (num < consumedElement.amount * dt)
				{
					flag = false;
					break;
				}
			}
			return flag;
		}

		public void AbsorbFertilizer(float dt)
		{
			Storage storage = base.sm.fertilizerStorage.Get<Storage>(base.smi);
			if (storage == null || this.consumedElements == null)
			{
				return;
			}
			if (!this.StorageHasFertilizer(dt))
			{
				return;
			}
			foreach (ElementConverter.ConsumedElement consumedElement in this.consumedElements)
			{
				float num = consumedElement.amount * dt;
				consumedElement.accumulator.Accumulate(num);
				List<PrimaryElement> list = storage.FindPrimaryElements(consumedElement.tag);
				foreach (PrimaryElement primaryElement in list)
				{
					float num2 = Mathf.Min(num, primaryElement.Mass);
					primaryElement.Mass -= num2;
					num -= num2;
					if (num == 0f)
					{
						break;
					}
				}
				Debug.Assert(num == 0f);
				storage.Trigger(-1697596308, base.gameObject);
			}
		}

		public AttributeModifier consumptionRate;

		public AttributeModifier absorptionRate;

		private ElementConverter.ConsumedElement[] consumedElements;
	}
}
