using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class FertilizationMonitor : GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget, FertilizationMonitor.Instance.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.wild;
		base.serializable = false;
		this.wild.ParamTransition<GameObject>(this.fertilizerStorage, this.unfertilizable, (FertilizationMonitor.Instance smi, GameObject p) => p != null);
		this.unfertilizable.Enter(delegate(FertilizationMonitor.Instance smi)
		{
			if (smi.AcceptsFertilizer())
			{
				smi.GoTo(this.replanted.fertilized);
			}
		}).ToggleStatusItem((FertilizationMonitor.Instance smi) => smi.GetNotAcceptedStatusItem(), (FertilizationMonitor.Instance smi) => smi);
		this.replanted.Enter(delegate(FertilizationMonitor.Instance smi)
		{
			foreach (ManualDeliveryKG manualDeliveryKG in smi.gameObject.GetComponents<ManualDeliveryKG>())
			{
				manualDeliveryKG.Pause(false, "replanted");
			}
		});
		this.replanted.fertilized.TriggerOnEnter(GameHashes.Fertilized, null).DefaultState(this.replanted.fertilized.decaying);
		this.replanted.fertilized.decaying.ParamTransition<GameObject>(this.fertilizerStorage, this.replanted.fertilized.absorbing, (FertilizationMonitor.Instance smi, GameObject p) => smi.StorageHasFertilizer(0.2f)).ToggleAttributeModifier("Consuming", (FertilizationMonitor.Instance smi) => smi.consumptionRate, null).ToggleAttributeModifier("FertCondition", (FertilizationMonitor.Instance smi) => smi.goodConditionModifier, null)
			.Update(delegate(FertilizationMonitor.Instance smi)
			{
				if (smi.Starved())
				{
					smi.GoTo(this.replanted.starved);
				}
			})
			.Target(this.fertilizerStorage)
			.EventTransition(GameHashes.OnStorageChange, this.replanted.fertilized.absorbing, (FertilizationMonitor.Instance smi) => smi.StorageHasFertilizer(0.2f));
		this.replanted.fertilized.absorbing.ParamTransition<GameObject>(this.fertilizerStorage, this.replanted.fertilized.decaying, (FertilizationMonitor.Instance smi, GameObject p) => !smi.StorageHasFertilizer(0.2f)).ToggleAttributeModifier("Absorbing", (FertilizationMonitor.Instance smi) => smi.absorptionRate, null).ToggleAttributeModifier("FertCondition", (FertilizationMonitor.Instance smi) => smi.goodConditionModifier, null)
			.Update(delegate(FertilizationMonitor.Instance smi)
			{
				smi.AbsorbFertilizer(smi.deltatime);
			})
			.Target(this.fertilizerStorage)
			.EventTransition(GameHashes.OnStorageChange, this.replanted.fertilized.decaying, (FertilizationMonitor.Instance smi) => !smi.StorageHasFertilizer(0.2f));
		this.replanted.starved.ToggleStatusItem((FertilizationMonitor.Instance smi) => smi.GetStarvedStatusItem(), (FertilizationMonitor.Instance smi) => smi).ToggleAttributeModifier("BadFertCondition", (FertilizationMonitor.Instance smi) => smi.badConditionModifier, null).TriggerOnEnter(GameHashes.Unfertilized, null)
			.Target(this.fertilizerStorage)
			.EventTransition(GameHashes.OnStorageChange, this.replanted.fertilized, (FertilizationMonitor.Instance smi) => smi.StorageHasFertilizer(0.2f));
	}

	public StateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget, FertilizationMonitor.Instance.Def>.TargetParameter fertilizerStorage;

	public GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget, FertilizationMonitor.Instance.Def>.State wild;

	public GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget, FertilizationMonitor.Instance.Def>.State unfertilizable;

	public FertilizationMonitor.ReplantedStates replanted;

	public struct FertilizerInfo
	{
		public Tag tag;

		public float massConsumptionRate;
	}

	public class FertilizedStates : GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget, FertilizationMonitor.Instance.Def>.State
	{
		public GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget, FertilizationMonitor.Instance.Def>.State decaying;

		public GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget, FertilizationMonitor.Instance.Def>.State absorbing;
	}

	public class ReplantedStates : GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget, FertilizationMonitor.Instance.Def>.State
	{
		public FertilizationMonitor.FertilizedStates fertilized;

		public GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget, FertilizationMonitor.Instance.Def>.State starved;
	}

	public new class Instance : GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget, FertilizationMonitor.Instance.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, FertilizationMonitor.Instance.Def def)
			: base(master, def)
		{
			this.AddAmounts(base.gameObject);
			this.MakeModifiers();
			this.accumulators = new Accumulator[def.consumedElements.Length];
			for (int i = 0; i < def.consumedElements.Length; i++)
			{
				this.accumulators[i] = new Accumulator("ElementsConsumed", base.master.GetComponent<KPrefabID>(), 3f);
			}
			master.Subscribe(1309017699, new Action<object>(this.SetStorage));
		}

		public virtual StatusItem GetStarvedStatusItem()
		{
			return Db.Get().CreatureStatusItems.NeedsFertilizer;
		}

		public virtual StatusItem GetNotAcceptedStatusItem()
		{
			return Db.Get().CreatureStatusItems.CantAcceptFertilizer;
		}

		protected virtual void AddAmounts(GameObject gameObject)
		{
			Amounts amounts = gameObject.GetAmounts();
			this.fertilization = amounts.Add(new AmountInstance(Db.Get().Amounts.Fertilization, gameObject));
		}

		protected virtual void MakeModifiers()
		{
			this.consumptionRate = new AttributeModifier(Db.Get().Amounts.Fertilization.deltaAttribute.Id, -0.16666667f, CREATURES.STATS.FERTILIZATION.CONSUME_MODIFIER, false, false);
			this.absorptionRate = new AttributeModifier(Db.Get().Amounts.Fertilization.deltaAttribute.Id, 1.6666666f, CREATURES.STATS.FERTILIZATION.ABSORBING_MODIFIER, false, false);
			AmountInstance amountInstance = Db.Get().Amounts.Maturity.Lookup(base.gameObject);
			this.badConditionModifier = new AttributeModifier(Db.Get().Amounts.YieldBonus.deltaAttribute.Id, 0f / amountInstance.GetMax(), CREATURES.STATS.YIELDBONUS.MODIFIERS.NOT_FERTILIZED, false, false);
			this.goodConditionModifier = new AttributeModifier(Db.Get().Amounts.YieldBonus.deltaAttribute.Id, 0.00041666668f / amountInstance.GetMax(), CREATURES.STATS.YIELDBONUS.MODIFIERS.FERTILIZED, false, false);
		}

		public void SetStorage(object obj)
		{
			Storage storage = (Storage)obj;
			base.sm.fertilizerStorage.Set(storage, base.smi);
			foreach (ManualDeliveryKG manualDeliveryKG in base.smi.gameObject.GetComponents<ManualDeliveryKG>())
			{
				manualDeliveryKG.SetStorage(storage);
			}
		}

		public virtual bool AcceptsFertilizer()
		{
			PlantablePlot component = base.sm.fertilizerStorage.Get(this).GetComponent<PlantablePlot>();
			return component != null && component.AcceptsFertilizer;
		}

		public bool Starved()
		{
			return this.fertilization.value == 0f;
		}

		public bool StorageHasFertilizer(float dt)
		{
			if (base.def.consumedElements == null)
			{
				return false;
			}
			Storage storage = base.sm.fertilizerStorage.Get<Storage>(base.smi);
			if (storage == null)
			{
				return false;
			}
			bool flag = true;
			List<GameObject> items = storage.items;
			for (int i = 0; i < base.def.consumedElements.Length; i++)
			{
				FertilizationMonitor.FertilizerInfo fertilizerInfo = base.def.consumedElements[i];
				float num = 0f;
				for (int j = 0; j < items.Count; j++)
				{
					GameObject gameObject = items[j];
					if (gameObject.HasTag(fertilizerInfo.tag))
					{
						num += gameObject.GetComponent<PrimaryElement>().Mass;
					}
				}
				if (num < fertilizerInfo.massConsumptionRate * dt)
				{
					flag = false;
					break;
				}
			}
			return flag;
		}

		public void AbsorbFertilizer(float dt)
		{
			using (new KProfiler.Region("AbsorbFertilizer", null))
			{
				if (base.def.consumedElements != null)
				{
					Storage storage = base.sm.fertilizerStorage.Get<Storage>(base.smi);
					if (!(storage == null))
					{
						if (this.StorageHasFertilizer(dt))
						{
							for (int i = 0; i < base.def.consumedElements.Length; i++)
							{
								float num = base.def.consumedElements[i].massConsumptionRate * dt;
								PrimaryElement primaryElement = storage.FindFirstWithMass(base.def.consumedElements[i].tag);
								while (primaryElement != null)
								{
									float num2 = Mathf.Min(num, primaryElement.Mass);
									primaryElement.Mass -= num2;
									num -= num2;
									storage.Trigger(-1697596308, primaryElement.gameObject);
									this.accumulators[i].Accumulate(num2);
									if (num <= 0f)
									{
										break;
									}
									primaryElement = storage.FindFirstWithMass(base.def.consumedElements[i].tag);
								}
							}
						}
					}
				}
			}
		}

		private Accumulator[] accumulators;

		public AttributeModifier consumptionRate;

		public AttributeModifier absorptionRate;

		public AttributeModifier badConditionModifier;

		public AttributeModifier goodConditionModifier;

		protected AmountInstance fertilization;

		public class Def : StateMachine.Instance.BaseDef, IGameObjectEffectDescriptor
		{
			public List<Descriptor> GetDescriptors(GameObject obj)
			{
				if (this.consumedElements.Length > 0)
				{
					List<Descriptor> list = new List<Descriptor>();
					foreach (FertilizationMonitor.FertilizerInfo fertilizerInfo in this.consumedElements)
					{
						list.Add(new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.IDEAL_FERTILIZER, fertilizerInfo.tag.ProperName(), GameUtil.GetFormattedMass(-fertilizerInfo.massConsumptionRate, GameUtil.TimeSlice.PerCycle, true, "{0:0.#}")), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.IDEAL_FERTILIZER, fertilizerInfo.tag.ProperName(), GameUtil.GetFormattedMass(fertilizerInfo.massConsumptionRate, GameUtil.TimeSlice.PerCycle, true, "{0:0.#}")), Descriptor.DescriptorType.CropOptimumCondition, false));
					}
					return list;
				}
				return null;
			}

			public FertilizationMonitor.FertilizerInfo[] consumedElements;
		}
	}
}
