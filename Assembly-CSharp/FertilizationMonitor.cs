using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class FertilizationMonitor : GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget, FertilizationMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.wild;
		base.serializable = StateMachine.SerializeType.Never;
		this.wild.ParamTransition<GameObject>(this.fertilizerStorage, this.unfertilizable, (FertilizationMonitor.Instance smi, GameObject p) => p != null);
		this.unfertilizable.EnterTransition(this.replanted, (FertilizationMonitor.Instance smi) => smi.AcceptsFertilizer());
		this.replanted.Enter(delegate(FertilizationMonitor.Instance smi)
		{
			ManualDeliveryKG[] components = smi.gameObject.GetComponents<ManualDeliveryKG>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].Pause(false, "replanted");
			}
			smi.UpdateFertilization(0.2f);
		}).ParamTransition<bool>(this.isFertilized, this.replanted.fertilized, (FertilizationMonitor.Instance _, bool status) => status).ParamTransition<bool>(this.isFertilized, this.replanted.starved, (FertilizationMonitor.Instance _, bool status) => !status)
			.Target(this.fertilizerStorage)
			.EventHandler(GameHashes.OnStorageChange, delegate(FertilizationMonitor.Instance smi)
			{
				smi.UpdateFertilization(0.2f);
			})
			.Target(this.masterTarget);
		this.replanted.fertilized.DefaultState(this.replanted.fertilized.absorbing).TriggerOnEnter(GameHashes.Fertilized, null).EnterTransition(this.replanted.fertilized.wilting, (FertilizationMonitor.Instance smi) => smi.wiltCondition.IsWilting());
		this.replanted.fertilized.absorbing.ToggleAttributeModifier("Absorbing", (FertilizationMonitor.Instance smi) => smi.absorptionRate, null).EventTransition(GameHashes.Wilt, this.replanted.fertilized.wilting, null).Enter(delegate(FertilizationMonitor.Instance smi)
		{
			smi.StartAbsorbing();
		})
			.Exit(delegate(FertilizationMonitor.Instance smi)
			{
				smi.StopAbsorbing();
			});
		this.replanted.fertilized.wilting.EventTransition(GameHashes.WiltRecover, this.replanted.fertilized.absorbing, null);
		this.replanted.starved.TriggerOnEnter(GameHashes.Unfertilized, null);
	}

	public StateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget, FertilizationMonitor.Def>.TargetParameter fertilizerStorage;

	public StateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget, FertilizationMonitor.Def>.BoolParameter isFertilized;

	public GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget, FertilizationMonitor.Def>.State wild;

	public GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget, FertilizationMonitor.Def>.State unfertilizable;

	public FertilizationMonitor.ReplantedStates replanted;

	public class Def : StateMachine.BaseDef, IGameObjectEffectDescriptor
	{
		public List<Descriptor> GetDescriptors(GameObject obj)
		{
			if (this.consumedElements.Length == 0)
			{
				return null;
			}
			List<Descriptor> list = new List<Descriptor>();
			float preModifiedAttributeValue = obj.GetComponent<Modifiers>().GetPreModifiedAttributeValue(Db.Get().PlantAttributes.FertilizerUsageMod);
			foreach (PlantElementAbsorber.ConsumeInfo consumeInfo in this.consumedElements)
			{
				float num = consumeInfo.massConsumptionRate * preModifiedAttributeValue;
				list.Add(new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.IDEAL_FERTILIZER, consumeInfo.tag.ProperName(), GameUtil.GetFormattedMass(-num, GameUtil.TimeSlice.PerCycle, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.IDEAL_FERTILIZER, consumeInfo.tag.ProperName(), GameUtil.GetFormattedMass(num, GameUtil.TimeSlice.PerCycle, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), Descriptor.DescriptorType.Requirement, false));
			}
			return list;
		}

		public PlantElementAbsorber.ConsumeInfo[] ScaleConsumedElements(float scale)
		{
			PlantElementAbsorber.ConsumeInfo[] array = new PlantElementAbsorber.ConsumeInfo[this.consumedElements.Length];
			for (int i = 0; i < this.consumedElements.Length; i++)
			{
				PlantElementAbsorber.ConsumeInfo consumeInfo = this.consumedElements[i];
				consumeInfo.massConsumptionRate *= scale;
				array[i] = consumeInfo;
			}
			return array;
		}

		public PlantElementAbsorber.ConsumeInfo[] consumedElements;
	}

	public enum FertilizerStatus
	{
		Starved,
		Correct
	}

	public class FertilizedStates : GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget, FertilizationMonitor.Def>.State
	{
		public GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget, FertilizationMonitor.Def>.State absorbing;

		public GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget, FertilizationMonitor.Def>.State wilting;
	}

	public class ReplantedStates : GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget, FertilizationMonitor.Def>.State
	{
		public FertilizationMonitor.FertilizedStates fertilized;

		public GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget, FertilizationMonitor.Def>.State starved;
	}

	public new class Instance : GameStateMachine<FertilizationMonitor, FertilizationMonitor.Instance, IStateMachineTarget, FertilizationMonitor.Def>.GameInstance, IWiltCause
	{
		public float total_fertilizer_available
		{
			get
			{
				return PlantElementAbsorber.FindLargest(this.storage, this.consumedElements);
			}
		}

		public Instance(IStateMachineTarget master, FertilizationMonitor.Def def)
			: base(master, def)
		{
			this.AddAmounts(base.gameObject);
			this.MakeModifiers();
			master.Subscribe(1309017699, new Action<object>(this.SetStorage));
			float totalValue = base.gameObject.GetAttributes().Get(Db.Get().PlantAttributes.FertilizerUsageMod).GetTotalValue();
			this.consumedElements = def.ScaleConsumedElements(totalValue);
		}

		public virtual StatusItem GetStarvedStatusItem()
		{
			return Db.Get().CreatureStatusItems.NeedsFertilizer;
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
				if (!base.smi.IsInsideState(base.smi.sm.replanted.starved))
				{
					return "";
				}
				return this.GetStarvedStatusItem().resolveStringCallback(CREATURES.STATUSITEMS.NEEDSFERTILIZER.NAME, this);
			}
		}

		protected virtual void MakeModifiers()
		{
			this.absorptionRate = new AttributeModifier(Db.Get().Amounts.Fertilization.deltaAttribute.Id, 1.6666666f, CREATURES.STATS.FERTILIZATION.ABSORBING_MODIFIER, false, false, true);
		}

		public void SetStorage(object obj)
		{
			this.storage = (Storage)obj;
			base.sm.fertilizerStorage.Set(this.storage, base.smi);
			IrrigationMonitor.Instance.DumpIncorrectFertilizers(this.storage, base.smi.gameObject);
			foreach (ManualDeliveryKG manualDeliveryKG in base.smi.gameObject.GetComponents<ManualDeliveryKG>())
			{
				bool flag = false;
				foreach (PlantElementAbsorber.ConsumeInfo consumeInfo in base.def.consumedElements)
				{
					if (manualDeliveryKG.RequestedItemTag == consumeInfo.tag)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					manualDeliveryKG.SetStorage(this.storage);
					manualDeliveryKG.enabled = true;
				}
			}
		}

		public virtual bool AcceptsFertilizer()
		{
			PlantablePlot plantablePlot;
			return base.sm.fertilizerStorage.Get(this).TryGetComponent<PlantablePlot>(out plantablePlot) && plantablePlot.AcceptsFertilizer;
		}

		public void UpdateFertilization(float dt)
		{
			if (base.def.consumedElements == null)
			{
				return;
			}
			if (this.storage == null)
			{
				return;
			}
			if (dt == 0f)
			{
				return;
			}
			bool flag = PlantElementAbsorber.PlanConsume(this.storage, this.consumedElements, dt, null);
			base.sm.isFertilized.Set(flag, base.smi, false);
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
			this.absorberHandle = Game.Instance.plantElementAbsorbers.Add(this.storage, this.consumedElements);
		}

		public void StopAbsorbing()
		{
			if (!this.absorberHandle.IsValid())
			{
				return;
			}
			this.absorberHandle = Game.Instance.plantElementAbsorbers.Remove(this.absorberHandle);
		}

		public AttributeModifier absorptionRate;

		protected AmountInstance fertilization;

		private Storage storage;

		private HandleVector<int>.Handle absorberHandle = HandleVector<int>.InvalidHandle;

		[MyCmpReq]
		public WiltCondition wiltCondition;

		private readonly PlantElementAbsorber.ConsumeInfo[] consumedElements;
	}
}
