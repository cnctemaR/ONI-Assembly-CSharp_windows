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
		});
		this.replanted.Enter(delegate(FertilizationMonitor.Instance smi)
		{
			foreach (ManualDeliveryKG manualDeliveryKG in smi.gameObject.GetComponents<ManualDeliveryKG>())
			{
				manualDeliveryKG.Pause(false, "replanted");
			}
			smi.UpdateFertilization(0.033333335f);
		}).Target(this.fertilizerStorage).EventHandler(GameHashes.OnStorageChange, delegate(FertilizationMonitor.Instance smi)
		{
			smi.UpdateFertilization(0.2f);
		})
			.Target(this.masterTarget);
		this.replanted.fertilized.DefaultState(this.replanted.fertilized.decaying).TriggerOnEnter(this.ResourceRecievedEvent, null);
		this.replanted.fertilized.decaying.DefaultState(this.replanted.fertilized.decaying.normal).ToggleAttributeModifier("Consuming", (FertilizationMonitor.Instance smi) => smi.consumptionRate, null).ParamTransition<bool>(this.hasCorrectFertilizer, this.replanted.fertilized.absorbing, (FertilizationMonitor.Instance smi, bool p) => p)
			.Update(delegate(FertilizationMonitor.Instance smi)
			{
				if (smi.Starved())
				{
					smi.GoTo(this.replanted.starved);
				}
			});
		this.replanted.fertilized.decaying.normal.ParamTransition<bool>(this.hasIncorrectFertilizer, this.replanted.fertilized.decaying.wrongFert, (FertilizationMonitor.Instance smi, bool p) => p);
		this.replanted.fertilized.decaying.wrongFert.ParamTransition<bool>(this.hasIncorrectFertilizer, this.replanted.fertilized.decaying.normal, (FertilizationMonitor.Instance smi, bool p) => !p);
		this.replanted.fertilized.absorbing.DefaultState(this.replanted.fertilized.absorbing.normal).ParamTransition<bool>(this.hasCorrectFertilizer, this.replanted.fertilized.decaying, (FertilizationMonitor.Instance smi, bool p) => !p).ToggleAttributeModifier("Absorbing", (FertilizationMonitor.Instance smi) => smi.absorptionRate, null)
			.Update(delegate(FertilizationMonitor.Instance smi)
			{
				if (!smi.master.gameObject.HasTag(GameTags.Wilting))
				{
					smi.AbsorbFertilizer(smi.deltatime);
				}
			});
		this.replanted.fertilized.absorbing.normal.ParamTransition<bool>(this.hasIncorrectFertilizer, this.replanted.fertilized.absorbing.wrongFert, (FertilizationMonitor.Instance smi, bool p) => p);
		this.replanted.fertilized.absorbing.wrongFert.ParamTransition<bool>(this.hasIncorrectFertilizer, this.replanted.fertilized.absorbing.normal, (FertilizationMonitor.Instance smi, bool p) => !p);
		this.replanted.starved.DefaultState(this.replanted.starved.normal).TriggerOnEnter(this.ResourceDepletedEvent, null).ParamTransition<bool>(this.hasCorrectFertilizer, this.replanted.fertilized, (FertilizationMonitor.Instance smi, bool p) => p);
		this.replanted.starved.normal.ParamTransition<bool>(this.hasIncorrectFertilizer, this.replanted.starved.wrongFert, (FertilizationMonitor.Instance smi, bool p) => p);
		this.replanted.starved.wrongFert.ParamTransition<bool>(this.hasIncorrectFertilizer, this.replanted.starved.normal, (FertilizationMonitor.Instance smi, bool p) => !p);
	}

	public StateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget, FertilizationMonitor.Instance.Def>.TargetParameter fertilizerStorage;

	public StateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget, FertilizationMonitor.Instance.Def>.BoolParameter hasCorrectFertilizer;

	public StateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget, FertilizationMonitor.Instance.Def>.BoolParameter hasIncorrectFertilizer;

	public GameHashes ResourceRecievedEvent = GameHashes.Fertilized;

	public GameHashes ResourceDepletedEvent = GameHashes.Unfertilized;

	public GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget, FertilizationMonitor.Instance.Def>.State wild;

	public GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget, FertilizationMonitor.Instance.Def>.State unfertilizable;

	public FertilizationMonitor.ReplantedStates replanted;

	public struct FertilizerInfo
	{
		public Tag tag;

		public float massConsumptionRate;
	}

	public class VariableFertilizerStates : GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget, FertilizationMonitor.Instance.Def>.State
	{
		public GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget, FertilizationMonitor.Instance.Def>.State normal;

		public GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget, FertilizationMonitor.Instance.Def>.State wrongFert;
	}

	public class FertilizedStates : GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget, FertilizationMonitor.Instance.Def>.State
	{
		public FertilizationMonitor.VariableFertilizerStates decaying;

		public FertilizationMonitor.VariableFertilizerStates absorbing;
	}

	public class ReplantedStates : GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget, FertilizationMonitor.Instance.Def>.State
	{
		public FertilizationMonitor.FertilizedStates fertilized;

		public FertilizationMonitor.VariableFertilizerStates starved;
	}

	public new class Instance : GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget, FertilizationMonitor.Instance.Def>.GameInstance, IWiltCause
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

		public float total_fertilizer_available
		{
			get
			{
				return this.total_available_mass;
			}
		}

		public virtual StatusItem GetStarvedStatusItem()
		{
			return Db.Get().CreatureStatusItems.NeedsFertilizer;
		}

		public virtual StatusItem GetIncorrectFertStatusItem()
		{
			return Db.Get().CreatureStatusItems.WrongFertilizer;
		}

		public virtual StatusItem GetIncorrectFertStatusItemMajor()
		{
			return Db.Get().CreatureStatusItems.WrongFertilizerMajor;
		}

		protected virtual void AddAmounts(GameObject gameObject)
		{
			Amounts amounts = gameObject.GetAmounts();
			this.fertilization = amounts.Add(new AmountInstance(Db.Get().Amounts.Fertilization, gameObject));
		}

		public WiltCondition.Condition[] Conditions
		{
			get
			{
				return new WiltCondition.Condition[] { WiltCondition.Condition.Fertilized };
			}
		}

		public string WiltStateString
		{
			get
			{
				string text = "";
				if (base.smi.IsInsideState(base.smi.sm.replanted.fertilized.decaying.wrongFert))
				{
					text = this.GetIncorrectFertStatusItemMajor().resolveStringCallback(CREATURES.STATUSITEMS.WRONGFERTILIZERMAJOR.NAME, this);
				}
				else if (base.smi.IsInsideState(base.smi.sm.replanted.fertilized.absorbing.wrongFert))
				{
					text = this.GetIncorrectFertStatusItem().resolveStringCallback(CREATURES.STATUSITEMS.WRONGFERTILIZER.NAME, this);
				}
				else if (base.smi.IsInsideState(base.smi.sm.replanted.starved))
				{
					text = this.GetStarvedStatusItem().resolveStringCallback(CREATURES.STATUSITEMS.NEEDSFERTILIZER.NAME, this);
				}
				else if (base.smi.IsInsideState(base.smi.sm.replanted.starved.wrongFert))
				{
					text = this.GetIncorrectFertStatusItemMajor().resolveStringCallback(CREATURES.STATUSITEMS.WRONGFERTILIZERMAJOR.NAME, this);
				}
				return text;
			}
		}

		protected virtual void MakeModifiers()
		{
			this.consumptionRate = new AttributeModifier(Db.Get().Amounts.Fertilization.deltaAttribute.Id, -0.16666667f, CREATURES.STATS.FERTILIZATION.CONSUME_MODIFIER, false, false, true);
			this.absorptionRate = new AttributeModifier(Db.Get().Amounts.Fertilization.deltaAttribute.Id, 1.6666666f, CREATURES.STATS.FERTILIZATION.ABSORBING_MODIFIER, false, false, true);
		}

		public void SetStorage(object obj)
		{
			Storage storage = (Storage)obj;
			base.sm.fertilizerStorage.Set(storage, base.smi);
			foreach (ManualDeliveryKG manualDeliveryKG in base.smi.gameObject.GetComponents<ManualDeliveryKG>())
			{
				bool flag = false;
				foreach (FertilizationMonitor.FertilizerInfo fertilizerInfo in base.def.consumedElements)
				{
					if (manualDeliveryKG.requestedItemTag == fertilizerInfo.tag)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					manualDeliveryKG.SetStorage(storage);
					manualDeliveryKG.enabled = true;
				}
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

		public void UpdateFertilization(float dt)
		{
			if (base.def.consumedElements != null)
			{
				Storage storage = base.sm.fertilizerStorage.Get<Storage>(base.smi);
				if (!(storage == null))
				{
					bool flag = true;
					bool flag2 = false;
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
							else if (gameObject.HasTag(base.def.wrongFertilizerTestTag))
							{
								flag2 = true;
							}
						}
						this.total_available_mass = num;
						if (num < fertilizerInfo.massConsumptionRate * dt)
						{
							flag = false;
							break;
						}
					}
					base.sm.hasCorrectFertilizer.Set(flag, base.smi);
					base.sm.hasIncorrectFertilizer.Set(flag2, base.smi);
				}
			}
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
						if (base.sm.hasCorrectFertilizer.Get(base.smi))
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

		protected AmountInstance fertilization;

		private float total_available_mass;

		public class Def : StateMachine.Instance.BaseDef, IGameObjectEffectDescriptor
		{
			public List<Descriptor> GetDescriptors(GameObject obj)
			{
				List<Descriptor> list2;
				if (this.consumedElements.Length > 0)
				{
					List<Descriptor> list = new List<Descriptor>();
					foreach (FertilizationMonitor.FertilizerInfo fertilizerInfo in this.consumedElements)
					{
						list.Add(new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.IDEAL_FERTILIZER, fertilizerInfo.tag.ProperName(), GameUtil.GetFormattedMass(-fertilizerInfo.massConsumptionRate, GameUtil.TimeSlice.PerCycle, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.IDEAL_FERTILIZER, fertilizerInfo.tag.ProperName(), GameUtil.GetFormattedMass(fertilizerInfo.massConsumptionRate, GameUtil.TimeSlice.PerCycle, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), Descriptor.DescriptorType.Requirement, false));
					}
					list2 = list;
				}
				else
				{
					list2 = null;
				}
				return list2;
			}

			public Tag wrongFertilizerTestTag;

			public FertilizationMonitor.FertilizerInfo[] consumedElements;
		}
	}
}
