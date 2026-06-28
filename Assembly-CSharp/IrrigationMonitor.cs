using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class IrrigationMonitor : GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Instance.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.wild;
		base.serializable = false;
		this.wild.ParamTransition<GameObject>(this.resourceStorage, this.unfertilizable, (IrrigationMonitor.Instance smi, GameObject p) => p != null);
		this.unfertilizable.Enter(delegate(IrrigationMonitor.Instance smi)
		{
			if (smi.AcceptsLiquid())
			{
				smi.GoTo(this.replanted.irrigated);
			}
		});
		this.replanted.Enter(delegate(IrrigationMonitor.Instance smi)
		{
			foreach (ManualDeliveryKG manualDeliveryKG in smi.gameObject.GetComponents<ManualDeliveryKG>())
			{
				manualDeliveryKG.Pause(false, "replanted");
			}
			smi.UpdateIrrigation(0.033333335f);
		}).Target(this.resourceStorage).EventHandler(GameHashes.OnStorageChange, delegate(IrrigationMonitor.Instance smi)
		{
			smi.UpdateIrrigation(0.2f);
		})
			.Target(this.masterTarget);
		this.replanted.irrigated.DefaultState(this.replanted.irrigated.decaying).TriggerOnEnter(this.ResourceRecievedEvent, null);
		this.replanted.irrigated.decaying.DefaultState(this.replanted.irrigated.decaying.normal).ToggleAttributeModifier("Consuming", (IrrigationMonitor.Instance smi) => smi.consumptionRate, null).ParamTransition<bool>(this.hasCorrectLiquid, this.replanted.irrigated.absorbing, (IrrigationMonitor.Instance smi, bool p) => p)
			.Update(delegate(IrrigationMonitor.Instance smi)
			{
				if (smi.Starved())
				{
					smi.GoTo(this.replanted.starved);
				}
			});
		this.replanted.irrigated.decaying.normal.ParamTransition<bool>(this.hasIncorrectLiquid, this.replanted.irrigated.decaying.wrongLiquid, (IrrigationMonitor.Instance smi, bool p) => p);
		this.replanted.irrigated.decaying.wrongLiquid.ParamTransition<bool>(this.hasIncorrectLiquid, this.replanted.irrigated.decaying.normal, (IrrigationMonitor.Instance smi, bool p) => !p);
		this.replanted.irrigated.absorbing.DefaultState(this.replanted.irrigated.absorbing.normal).ParamTransition<bool>(this.hasCorrectLiquid, this.replanted.irrigated.decaying, (IrrigationMonitor.Instance smi, bool p) => !p).ToggleAttributeModifier("Absorbing", (IrrigationMonitor.Instance smi) => smi.absorptionRate, null)
			.Update(delegate(IrrigationMonitor.Instance smi)
			{
				if (!smi.gameObject.HasTag(GameTags.Wilting))
				{
					smi.AbsorbLiquid(smi.deltatime);
				}
			});
		this.replanted.irrigated.absorbing.normal.ParamTransition<bool>(this.hasIncorrectLiquid, this.replanted.irrigated.absorbing.wrongLiquid, (IrrigationMonitor.Instance smi, bool p) => p);
		this.replanted.irrigated.absorbing.wrongLiquid.ParamTransition<bool>(this.hasIncorrectLiquid, this.replanted.irrigated.absorbing.normal, (IrrigationMonitor.Instance smi, bool p) => !p);
		this.replanted.starved.DefaultState(this.replanted.starved.normal).TriggerOnEnter(this.ResourceDepletedEvent, null).ParamTransition<bool>(this.hasCorrectLiquid, this.replanted.irrigated, (IrrigationMonitor.Instance smi, bool p) => p);
		this.replanted.starved.normal.ParamTransition<bool>(this.hasIncorrectLiquid, this.replanted.starved.wrongLiquid, (IrrigationMonitor.Instance smi, bool p) => p);
		this.replanted.starved.wrongLiquid.ParamTransition<bool>(this.hasIncorrectLiquid, this.replanted.starved.normal, (IrrigationMonitor.Instance smi, bool p) => !p);
	}

	public StateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Instance.Def>.TargetParameter resourceStorage;

	public StateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Instance.Def>.BoolParameter hasCorrectLiquid;

	public StateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Instance.Def>.BoolParameter hasIncorrectLiquid;

	public GameHashes ResourceRecievedEvent = GameHashes.LiquidResourceRecieved;

	public GameHashes ResourceDepletedEvent = GameHashes.LiquidResourceEmpty;

	public GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Instance.Def>.State wild;

	public GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Instance.Def>.State unfertilizable;

	public IrrigationMonitor.ReplantedStates replanted;

	public struct LiquidResourceInfo
	{
		public LiquidResourceInfo(Tag tag, float mass_consumption_rate)
		{
			this.tag = tag;
			this.massConsumptionRate = mass_consumption_rate;
		}

		public Tag tag;

		public float massConsumptionRate;
	}

	public class VariableIrrigationStates : GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Instance.Def>.State
	{
		public GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Instance.Def>.State normal;

		public GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Instance.Def>.State wrongLiquid;
	}

	public class Irrigated : GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Instance.Def>.State
	{
		public IrrigationMonitor.VariableIrrigationStates decaying;

		public IrrigationMonitor.VariableIrrigationStates absorbing;
	}

	public class ReplantedStates : GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Instance.Def>.State
	{
		public IrrigationMonitor.Irrigated irrigated;

		public IrrigationMonitor.VariableIrrigationStates starved;
	}

	public new class Instance : GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Instance.Def>.GameInstance, IWiltCause
	{
		public Instance(IStateMachineTarget master, IrrigationMonitor.Instance.Def def)
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

		public float total_fertilizer_available
		{
			get
			{
				return this.total_available_mass;
			}
		}

		public virtual StatusItem GetStarvedStatusItem()
		{
			return Db.Get().CreatureStatusItems.NeedsIrrigation;
		}

		public virtual StatusItem GetIncorrectLiquidStatusItem()
		{
			return Db.Get().CreatureStatusItems.WrongIrrigation;
		}

		public virtual StatusItem GetIncorrectLiquidStatusItemMajor()
		{
			return Db.Get().CreatureStatusItems.WrongIrrigationMajor;
		}

		protected virtual void AddAmounts(GameObject gameObject)
		{
			Amounts amounts = gameObject.GetAmounts();
			this.irrigation = amounts.Add(new AmountInstance(Db.Get().Amounts.Irrigation, gameObject));
		}

		protected virtual void MakeModifiers()
		{
			this.consumptionRate = new AttributeModifier(Db.Get().Amounts.Irrigation.deltaAttribute.Id, -0.16666667f, CREATURES.STATS.IRRIGATION.CONSUME_MODIFIER, false, false, true);
			this.absorptionRate = new AttributeModifier(Db.Get().Amounts.Irrigation.deltaAttribute.Id, 1.6666666f, CREATURES.STATS.IRRIGATION.ABSORBING_MODIFIER, false, false, true);
		}

		public void SetStorage(object obj)
		{
			Storage storage = (Storage)obj;
			base.sm.resourceStorage.Set(storage, base.smi);
			foreach (ManualDeliveryKG manualDeliveryKG in base.smi.gameObject.GetComponents<ManualDeliveryKG>())
			{
				bool flag = false;
				foreach (IrrigationMonitor.LiquidResourceInfo liquidResourceInfo in base.def.consumedElements)
				{
					if (manualDeliveryKG.requestedItemTag == liquidResourceInfo.tag)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					manualDeliveryKG.SetStorage(storage);
					manualDeliveryKG.enabled = !storage.gameObject.GetComponent<PlantablePlot>().has_liquid_pipe_input;
				}
			}
		}

		public WiltCondition.Condition[] Conditions
		{
			get
			{
				return new WiltCondition.Condition[] { WiltCondition.Condition.Irrigation };
			}
		}

		public string WiltStateString
		{
			get
			{
				string text = "";
				if (base.smi.IsInsideState(base.smi.sm.replanted.irrigated.decaying.wrongLiquid))
				{
					text = this.GetIncorrectLiquidStatusItemMajor().resolveStringCallback(CREATURES.STATUSITEMS.WRONGIRRIGATIONMAJOR.NAME, this);
				}
				else if (base.smi.IsInsideState(base.smi.sm.replanted.irrigated.absorbing.wrongLiquid))
				{
					text = this.GetIncorrectLiquidStatusItem().resolveStringCallback(CREATURES.STATUSITEMS.WRONGIRRIGATION.NAME, this);
				}
				else if (base.smi.IsInsideState(base.smi.sm.replanted.starved))
				{
					text = this.GetStarvedStatusItem().resolveStringCallback(CREATURES.STATUSITEMS.NEEDSIRRIGATION.NAME, this);
				}
				else if (base.smi.IsInsideState(base.smi.sm.replanted.starved.wrongLiquid))
				{
					text = this.GetIncorrectLiquidStatusItemMajor().resolveStringCallback(CREATURES.STATUSITEMS.WRONGIRRIGATIONMAJOR.NAME, this);
				}
				return text;
			}
		}

		public virtual bool AcceptsLiquid()
		{
			PlantablePlot component = base.sm.resourceStorage.Get(this).GetComponent<PlantablePlot>();
			return component != null && component.AcceptsIrrigation;
		}

		public bool Starved()
		{
			return this.irrigation.value == 0f;
		}

		public void UpdateIrrigation(float dt)
		{
			if (base.def.consumedElements != null)
			{
				Storage storage = base.sm.resourceStorage.Get<Storage>(base.smi);
				if (!(storage == null))
				{
					bool flag = true;
					bool flag2 = false;
					List<GameObject> items = storage.items;
					for (int i = 0; i < base.def.consumedElements.Length; i++)
					{
						IrrigationMonitor.LiquidResourceInfo liquidResourceInfo = base.def.consumedElements[i];
						float num = 0f;
						for (int j = 0; j < items.Count; j++)
						{
							GameObject gameObject = items[j];
							if (gameObject.HasTag(liquidResourceInfo.tag))
							{
								num += gameObject.GetComponent<PrimaryElement>().Mass;
							}
							else if (gameObject.HasTag(base.def.wrongIrrigationTestTag))
							{
								flag2 = true;
							}
						}
						this.total_available_mass = num;
						if (num < liquidResourceInfo.massConsumptionRate * dt)
						{
							flag = false;
							break;
						}
					}
					base.sm.hasCorrectLiquid.Set(flag, base.smi);
					base.sm.hasIncorrectLiquid.Set(flag2, base.smi);
				}
			}
		}

		public void AbsorbLiquid(float dt)
		{
			using (new KProfiler.Region("AbsorbFertilizer", null))
			{
				if (base.def.consumedElements != null)
				{
					Storage storage = base.sm.resourceStorage.Get<Storage>(base.smi);
					if (!(storage == null))
					{
						if (base.sm.hasCorrectLiquid.Get(base.smi))
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

		protected AmountInstance irrigation;

		private float total_available_mass;

		public class Def : StateMachine.Instance.BaseDef, IGameObjectEffectDescriptor
		{
			public List<Descriptor> GetDescriptors(GameObject obj)
			{
				List<Descriptor> list2;
				if (this.consumedElements.Length > 0)
				{
					List<Descriptor> list = new List<Descriptor>();
					foreach (IrrigationMonitor.LiquidResourceInfo liquidResourceInfo in this.consumedElements)
					{
						list.Add(new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.IDEAL_FERTILIZER, liquidResourceInfo.tag.ProperName(), GameUtil.GetFormattedMass(-liquidResourceInfo.massConsumptionRate, GameUtil.TimeSlice.PerCycle, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.IDEAL_FERTILIZER, liquidResourceInfo.tag.ProperName(), GameUtil.GetFormattedMass(liquidResourceInfo.massConsumptionRate, GameUtil.TimeSlice.PerCycle, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), Descriptor.DescriptorType.Requirement, false));
					}
					list2 = list;
				}
				else
				{
					list2 = null;
				}
				return list2;
			}

			public Tag wrongIrrigationTestTag;

			public IrrigationMonitor.LiquidResourceInfo[] consumedElements;
		}
	}
}
