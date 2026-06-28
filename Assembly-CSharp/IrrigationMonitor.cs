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
		this.replanted.irrigated.DefaultState(this.replanted.irrigated.absorbing).TriggerOnEnter(this.ResourceRecievedEvent, null);
		this.replanted.irrigated.absorbing.DefaultState(this.replanted.irrigated.absorbing.normal).ParamTransition<bool>(this.hasCorrectLiquid, this.replanted.starved, (IrrigationMonitor.Instance smi, bool p) => !p).ToggleAttributeModifier("Absorbing", (IrrigationMonitor.Instance smi) => smi.absorptionRate, null)
			.Enter(delegate(IrrigationMonitor.Instance smi)
			{
				smi.StartAbsorbing();
			})
			.EventHandler(GameHashes.Wilt, delegate(IrrigationMonitor.Instance smi)
			{
				smi.StopAbsorbing();
			})
			.EventHandler(GameHashes.WiltRecover, delegate(IrrigationMonitor.Instance smi)
			{
				smi.StartAbsorbing();
			})
			.Exit(delegate(IrrigationMonitor.Instance smi)
			{
				smi.StopAbsorbing();
			});
		this.replanted.irrigated.absorbing.normal.ParamTransition<bool>(this.hasIncorrectLiquid, this.replanted.irrigated.absorbing.wrongLiquid, (IrrigationMonitor.Instance smi, bool p) => p);
		this.replanted.irrigated.absorbing.wrongLiquid.ParamTransition<bool>(this.hasIncorrectLiquid, this.replanted.irrigated.absorbing.normal, (IrrigationMonitor.Instance smi, bool p) => !p);
		this.replanted.starved.DefaultState(this.replanted.starved.normal).TriggerOnEnter(this.ResourceDepletedEvent, null).ParamTransition<bool>(this.enoughCorrectLiquidToRecover, this.replanted.irrigated.absorbing, (IrrigationMonitor.Instance smi, bool p) => p && this.hasCorrectLiquid.Get(smi))
			.ParamTransition<bool>(this.hasCorrectLiquid, this.replanted.irrigated.absorbing, (IrrigationMonitor.Instance smi, bool p) => p && this.enoughCorrectLiquidToRecover.Get(smi));
		this.replanted.starved.normal.ParamTransition<bool>(this.hasIncorrectLiquid, this.replanted.starved.wrongLiquid, (IrrigationMonitor.Instance smi, bool p) => p);
		this.replanted.starved.wrongLiquid.ParamTransition<bool>(this.hasIncorrectLiquid, this.replanted.starved.normal, (IrrigationMonitor.Instance smi, bool p) => !p);
	}

	public StateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Instance.Def>.TargetParameter resourceStorage;

	public StateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Instance.Def>.BoolParameter hasCorrectLiquid;

	public StateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Instance.Def>.BoolParameter hasIncorrectLiquid;

	public StateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Instance.Def>.BoolParameter enoughCorrectLiquidToRecover;

	public GameHashes ResourceRecievedEvent = GameHashes.LiquidResourceRecieved;

	public GameHashes ResourceDepletedEvent = GameHashes.LiquidResourceEmpty;

	public GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Instance.Def>.State wild;

	public GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Instance.Def>.State unfertilizable;

	public IrrigationMonitor.ReplantedStates replanted;

	public class VariableIrrigationStates : GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Instance.Def>.State
	{
		public GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Instance.Def>.State normal;

		public GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Instance.Def>.State wrongLiquid;
	}

	public class Irrigated : GameStateMachine<IrrigationMonitor, IrrigationMonitor.Instance, IStateMachineTarget, IrrigationMonitor.Instance.Def>.State
	{
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

		public static void DumpIncorrectFertilizers(Storage storage, GameObject go)
		{
			if (storage == null)
			{
				return;
			}
			if (go == null)
			{
				return;
			}
			IrrigationMonitor.Instance smi = go.GetSMI<IrrigationMonitor.Instance>();
			PlantElementAbsorber.ConsumeInfo[] array = null;
			if (smi != null)
			{
				array = smi.def.consumedElements;
			}
			IrrigationMonitor.Instance.DumpIncorrectFertilizers(storage, array, false);
			FertilizationMonitor.Instance smi2 = go.GetSMI<FertilizationMonitor.Instance>();
			PlantElementAbsorber.ConsumeInfo[] array2 = null;
			if (smi2 != null)
			{
				array2 = smi2.def.consumedElements;
			}
			IrrigationMonitor.Instance.DumpIncorrectFertilizers(storage, array2, true);
		}

		private static void DumpIncorrectFertilizers(Storage storage, PlantElementAbsorber.ConsumeInfo[] consumed_infos, bool validate_solids)
		{
			if (storage == null)
			{
				return;
			}
			for (int i = storage.items.Count - 1; i >= 0; i--)
			{
				GameObject gameObject = storage.items[i];
				if (!(gameObject == null))
				{
					PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
					if (!(component == null))
					{
						if (!(gameObject.GetComponent<ElementChunk>() == null))
						{
							if (validate_solids)
							{
								if (!component.Element.IsSolid)
								{
									goto IL_0105;
								}
							}
							else if (!component.Element.IsLiquid)
							{
								goto IL_0105;
							}
							bool flag = false;
							KPrefabID component2 = component.GetComponent<KPrefabID>();
							if (consumed_infos != null)
							{
								foreach (PlantElementAbsorber.ConsumeInfo consumeInfo in consumed_infos)
								{
									if (component2.HasTag(consumeInfo.tag))
									{
										flag = true;
										break;
									}
								}
							}
							if (!flag)
							{
								storage.Drop(gameObject);
							}
						}
					}
				}
				IL_0105:;
			}
		}

		public void SetStorage(object obj)
		{
			this.storage = (Storage)obj;
			base.sm.resourceStorage.Set(this.storage, base.smi);
			IrrigationMonitor.Instance.DumpIncorrectFertilizers(this.storage, base.smi.gameObject);
			foreach (ManualDeliveryKG manualDeliveryKG in base.smi.gameObject.GetComponents<ManualDeliveryKG>())
			{
				bool flag = false;
				foreach (PlantElementAbsorber.ConsumeInfo consumeInfo in base.def.consumedElements)
				{
					if (manualDeliveryKG.requestedItemTag == consumeInfo.tag)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					manualDeliveryKG.SetStorage(this.storage);
					manualDeliveryKG.enabled = !this.storage.gameObject.GetComponent<PlantablePlot>().has_liquid_pipe_input;
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
				string text = string.Empty;
				if (base.smi.IsInsideState(base.smi.sm.replanted.irrigated.absorbing.wrongLiquid))
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
			if (base.def.consumedElements == null)
			{
				return;
			}
			Storage storage = base.sm.resourceStorage.Get<Storage>(base.smi);
			bool flag = true;
			bool flag2 = false;
			bool flag3 = true;
			if (storage != null)
			{
				List<GameObject> items = storage.items;
				for (int i = 0; i < base.def.consumedElements.Length; i++)
				{
					float num = 0f;
					PlantElementAbsorber.ConsumeInfo consumeInfo = base.def.consumedElements[i];
					for (int j = 0; j < items.Count; j++)
					{
						GameObject gameObject = items[j];
						if (gameObject.HasTag(consumeInfo.tag))
						{
							num += gameObject.GetComponent<PrimaryElement>().Mass;
						}
						else if (gameObject.HasTag(base.def.wrongIrrigationTestTag))
						{
							flag2 = true;
						}
					}
					this.total_available_mass = num;
					if (num < consumeInfo.massConsumptionRate * dt)
					{
						flag = false;
						break;
					}
					if (num < consumeInfo.massConsumptionRate * (dt * 30f))
					{
						flag3 = false;
						break;
					}
				}
			}
			else
			{
				flag = false;
				flag3 = false;
				flag2 = false;
			}
			base.sm.hasCorrectLiquid.Set(flag, base.smi);
			base.sm.hasIncorrectLiquid.Set(flag2, base.smi);
			base.sm.enoughCorrectLiquidToRecover.Set(flag3 && flag, base.smi);
		}

		public void StartAbsorbing()
		{
			if (this.absorberHandle.IsValid())
			{
				return;
			}
			if (base.def.consumedElements == null || base.def.consumedElements.Length == 0)
			{
				return;
			}
			GameObject gameObject = base.smi.gameObject;
			this.absorberHandle = Game.Instance.plantElementAbsorbers.Add(this.storage, base.def.consumedElements);
		}

		public void StopAbsorbing()
		{
			if (!this.absorberHandle.IsValid())
			{
				return;
			}
			this.absorberHandle = Game.Instance.plantElementAbsorbers.Remove(this.absorberHandle);
		}

		public AttributeModifier consumptionRate;

		public AttributeModifier absorptionRate;

		protected AmountInstance irrigation;

		private float total_available_mass;

		private Storage storage;

		private HandleVector<int>.Handle absorberHandle = HandleVector<int>.InvalidHandle;

		public class Def : StateMachine.Instance.BaseDef, IGameObjectEffectDescriptor
		{
			public List<Descriptor> GetDescriptors(GameObject obj)
			{
				if (this.consumedElements.Length > 0)
				{
					List<Descriptor> list = new List<Descriptor>();
					foreach (PlantElementAbsorber.ConsumeInfo consumeInfo in this.consumedElements)
					{
						list.Add(new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.IDEAL_FERTILIZER, consumeInfo.tag.ProperName(), GameUtil.GetFormattedMass(-consumeInfo.massConsumptionRate, GameUtil.TimeSlice.PerCycle, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.IDEAL_FERTILIZER, consumeInfo.tag.ProperName(), GameUtil.GetFormattedMass(consumeInfo.massConsumptionRate, GameUtil.TimeSlice.PerCycle, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), Descriptor.DescriptorType.Requirement, false));
					}
					return list;
				}
				return null;
			}

			public Tag wrongIrrigationTestTag;

			public PlantElementAbsorber.ConsumeInfo[] consumedElements;
		}
	}
}
