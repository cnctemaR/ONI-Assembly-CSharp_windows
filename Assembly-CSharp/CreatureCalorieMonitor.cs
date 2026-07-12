using System;
using System.Collections.Generic;
using System.Linq;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

public class CreatureCalorieMonitor : GameStateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.normal;
		base.serializable = StateMachine.SerializeType.Both_DEPRECATED;
		this.root.EventHandler(GameHashes.CaloriesConsumed, delegate(CreatureCalorieMonitor.Instance smi, object data)
		{
			smi.OnCaloriesConsumed(data);
		}).ToggleBehaviour(GameTags.Creatures.Poop, new StateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.Transition.ConditionCallback(CreatureCalorieMonitor.ReadyToPoop), delegate(CreatureCalorieMonitor.Instance smi)
		{
			smi.Poop();
		}).Update(new Action<CreatureCalorieMonitor.Instance, float>(CreatureCalorieMonitor.UpdateMetabolismCalorieModifier), UpdateRate.SIM_200ms, false);
		this.normal.TagTransition(GameTags.Creatures.PausedHunger, this.pause.commonPause, false).Transition(this.hungry, (CreatureCalorieMonitor.Instance smi) => smi.IsHungry(), UpdateRate.SIM_1000ms);
		this.hungry.DefaultState(this.hungry.hungry).ToggleTag(GameTags.Creatures.Hungry).EventTransition(GameHashes.CaloriesConsumed, this.normal, (CreatureCalorieMonitor.Instance smi) => !smi.IsHungry());
		this.hungry.hungry.TagTransition(GameTags.Creatures.PausedHunger, this.pause.commonPause, false).Transition(this.normal, (CreatureCalorieMonitor.Instance smi) => !smi.IsHungry(), UpdateRate.SIM_1000ms).Transition(this.hungry.outofcalories, (CreatureCalorieMonitor.Instance smi) => smi.IsOutOfCalories(), UpdateRate.SIM_1000ms)
			.ToggleStatusItem(Db.Get().CreatureStatusItems.Hungry, null);
		this.hungry.outofcalories.DefaultState(this.hungry.outofcalories.wild).Transition(this.hungry.hungry, (CreatureCalorieMonitor.Instance smi) => !smi.IsOutOfCalories(), UpdateRate.SIM_1000ms);
		this.hungry.outofcalories.wild.TagTransition(GameTags.Creatures.PausedHunger, this.pause.commonPause, false).TagTransition(GameTags.Creatures.Wild, this.hungry.outofcalories.tame, true).ToggleStatusItem(Db.Get().CreatureStatusItems.Hungry, null);
		this.hungry.outofcalories.tame.Enter("StarvationStartTime", new StateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.State.Callback(CreatureCalorieMonitor.StarvationStartTime)).Exit("ClearStarvationTime", delegate(CreatureCalorieMonitor.Instance smi)
		{
			this.starvationStartTime.Set(Mathf.Min(-(GameClock.Instance.GetTime() - this.starvationStartTime.Get(smi)), 0f), smi, false);
		}).Transition(this.hungry.outofcalories.starvedtodeath, (CreatureCalorieMonitor.Instance smi) => smi.GetDeathTimeRemaining() <= 0f, UpdateRate.SIM_1000ms)
			.TagTransition(GameTags.Creatures.PausedHunger, this.pause.starvingPause, false)
			.TagTransition(GameTags.Creatures.Wild, this.hungry.outofcalories.wild, false)
			.ToggleStatusItem(CREATURES.STATUSITEMS.STARVING.NAME, CREATURES.STATUSITEMS.STARVING.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.BadMinor, false, default(HashedString), 129022, (string str, CreatureCalorieMonitor.Instance smi) => str.Replace("{TimeUntilDeath}", GameUtil.GetFormattedCycles(smi.GetDeathTimeRemaining(), "F1", false)), null, null)
			.ToggleNotification((CreatureCalorieMonitor.Instance smi) => new Notification(CREATURES.STATUSITEMS.STARVING.NOTIFICATION_NAME, NotificationType.BadMinor, (List<Notification> notifications, object data) => CREATURES.STATUSITEMS.STARVING.NOTIFICATION_TOOLTIP + notifications.ReduceMessages(false), null, true, 0f, null, null, null, true, false, false))
			.ToggleEffect((CreatureCalorieMonitor.Instance smi) => this.outOfCaloriesTame);
		this.hungry.outofcalories.starvedtodeath.Enter(delegate(CreatureCalorieMonitor.Instance smi)
		{
			smi.GetSMI<DeathMonitor.Instance>().Kill(Db.Get().Deaths.Starvation);
		});
		this.pause.commonPause.TagTransition(GameTags.Creatures.PausedHunger, this.normal, true);
		this.pause.starvingPause.Exit("Recalculate StarvationStartTime", new StateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.State.Callback(CreatureCalorieMonitor.RecalculateStartTimeOnUnpause)).TagTransition(GameTags.Creatures.PausedHunger, this.hungry.outofcalories.tame, true);
		this.outOfCaloriesTame = new Effect("OutOfCaloriesTame", CREATURES.MODIFIERS.OUT_OF_CALORIES.NAME, CREATURES.MODIFIERS.OUT_OF_CALORIES.TOOLTIP, 0f, false, false, false, null, -1f, 0f, null, "");
		this.outOfCaloriesTame.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, -10f, CREATURES.MODIFIERS.OUT_OF_CALORIES.NAME, false, false, true));
	}

	private static bool ReadyToPoop(CreatureCalorieMonitor.Instance smi)
	{
		return smi.stomach.IsReadyToPoop() && Time.time - smi.lastMealOrPoopTime >= smi.def.minimumTimeBeforePooping && !smi.IsInsideState(smi.sm.pause);
	}

	private static void UpdateMetabolismCalorieModifier(CreatureCalorieMonitor.Instance smi, float dt)
	{
		if (smi.IsInsideState(smi.sm.pause))
		{
			return;
		}
		smi.deltaCalorieMetabolismModifier.SetValue(1f - smi.metabolism.GetTotalValue() / 100f);
	}

	private static void StarvationStartTime(CreatureCalorieMonitor.Instance smi)
	{
		if (smi.sm.starvationStartTime.Get(smi) <= 0f)
		{
			smi.sm.starvationStartTime.Set(GameClock.Instance.GetTime(), smi, false);
		}
	}

	private static void RecalculateStartTimeOnUnpause(CreatureCalorieMonitor.Instance smi)
	{
		float num = smi.sm.starvationStartTime.Get(smi);
		if (num < 0f)
		{
			float num2 = GameClock.Instance.GetTime() - Mathf.Abs(num);
			smi.sm.starvationStartTime.Set(num2, smi, false);
		}
	}

	public GameStateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.State normal;

	public CreatureCalorieMonitor.PauseStates pause;

	private CreatureCalorieMonitor.HungryStates hungry;

	private Effect outOfCaloriesTame;

	public StateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.FloatParameter starvationStartTime;

	public struct CaloriesConsumedEvent
	{
		public Tag tag;

		public float calories;
	}

	public class Def : StateMachine.BaseDef, IGameObjectEffectDescriptor
	{
		public override void Configure(GameObject prefab)
		{
			prefab.GetComponent<Modifiers>().initialAmounts.Add(Db.Get().Amounts.Calories.Id);
		}

		public List<Descriptor> GetDescriptors(GameObject obj)
		{
			List<Descriptor> list = new List<Descriptor>();
			list.Add(new Descriptor(UI.BUILDINGEFFECTS.DIET_HEADER, UI.BUILDINGEFFECTS.TOOLTIPS.DIET_HEADER, Descriptor.DescriptorType.Effect, false));
			float dailyPlantGrowthConsumption = 1f;
			CreatureCalorieMonitor.Stomach stomach = obj.GetSMI<CreatureCalorieMonitor.Instance>().stomach;
			if (stomach.diet.consumedTags.Count > 0)
			{
				float calorie_loss_per_second = 0f;
				foreach (AttributeModifier attributeModifier in Db.Get().traits.Get(obj.GetComponent<Modifiers>().initialTraits[0]).SelfModifiers)
				{
					if (attributeModifier.AttributeId == Db.Get().Amounts.Calories.deltaAttribute.Id)
					{
						calorie_loss_per_second = attributeModifier.Value;
					}
				}
				string text = string.Join(", ", stomach.diet.consumedTags.Select<KeyValuePair<Tag, float>, string>((KeyValuePair<Tag, float> t) => t.Key.ProperName()).ToArray<string>());
				string text2 = "";
				if (stomach.diet.CanEatAnyPlantDirectly)
				{
					text2 = string.Join("\n", stomach.diet.consumedTags.Select<KeyValuePair<Tag, float>, string>(delegate(KeyValuePair<Tag, float> t)
					{
						float num = -calorie_loss_per_second / t.Value;
						dailyPlantGrowthConsumption = num;
						GameObject prefab = Assets.GetPrefab(t.Key.ToString());
						IPlantConsumptionInstructions plantConsumptionInstructions = prefab.GetComponent<IPlantConsumptionInstructions>();
						plantConsumptionInstructions = ((plantConsumptionInstructions != null) ? plantConsumptionInstructions : prefab.GetSMI<IPlantConsumptionInstructions>());
						if (plantConsumptionInstructions == null)
						{
							return "";
						}
						return UI.BUILDINGEFFECTS.DIET_CONSUMED_ITEM.text.Replace("{Food}", t.Key.ProperName()).Replace("{Amount}", plantConsumptionInstructions.GetFormattedConsumptionPerCycle(num));
					}).ToArray<string>());
					text2 += "\n";
				}
				if (this.diet.CanEatAnyNonDirectlyEdiblePlant)
				{
					text2 += string.Join("\n", (from t in stomach.diet.consumedTags.FindAll((KeyValuePair<Tag, float> t) => this.diet.directlyEatenPlantInfos.FirstOrDefault<Diet.Info>((Diet.Info info) => info.consumedTags.Contains(t.Key)) == null)
						select UI.BUILDINGEFFECTS.DIET_CONSUMED_ITEM.text.Replace("{Food}", t.Key.ProperName()).Replace("{Amount}", GameUtil.GetFormattedMass(-calorie_loss_per_second / t.Value, GameUtil.TimeSlice.PerCycle, GameUtil.MetricMassFormat.Kilogram, true, "{0:0.#}"))).ToArray<string>());
				}
				list.Add(new Descriptor(UI.BUILDINGEFFECTS.DIET_CONSUMED.text.Replace("{Foodlist}", text), UI.BUILDINGEFFECTS.TOOLTIPS.DIET_CONSUMED.text.Replace("{Foodlist}", text2), Descriptor.DescriptorType.Effect, false));
			}
			if (stomach.diet.producedTags.Count > 0)
			{
				string text3 = string.Join(", ", stomach.diet.producedTags.Select<KeyValuePair<Tag, float>, string>((KeyValuePair<Tag, float> t) => t.Key.ProperName()).ToArray<string>());
				string text4 = "";
				if (stomach.diet.CanEatAnyPlantDirectly)
				{
					text4 = string.Join("\n", (from t in stomach.diet.producedTags.FindAll((KeyValuePair<Tag, float> t) => this.diet.directlyEatenPlantInfos.FirstOrDefault<Diet.Info>((Diet.Info info) => info.consumedTags.Contains(t.Key)) != null)
						select UI.BUILDINGEFFECTS.DIET_PRODUCED_ITEM_FROM_PLANT.text.Replace("{Item}", t.Key.ProperName()).Replace("{Amount}", GameUtil.GetFormattedMass(t.Value * dailyPlantGrowthConsumption, GameUtil.TimeSlice.PerCycle, GameUtil.MetricMassFormat.Kilogram, true, "{0:0.#}"))).ToArray<string>());
					text4 += "\n";
				}
				if (stomach.diet.CanEatAnyNonDirectlyEdiblePlant)
				{
					text4 += string.Join("\n", (from t in stomach.diet.producedTags.FindAll((KeyValuePair<Tag, float> t) => this.diet.directlyEatenPlantInfos.FirstOrDefault<Diet.Info>((Diet.Info info) => info.consumedTags.Contains(t.Key)) == null)
						select UI.BUILDINGEFFECTS.DIET_PRODUCED_ITEM.text.Replace("{Item}", t.Key.ProperName()).Replace("{Percent}", GameUtil.GetFormattedPercent(t.Value * 100f, GameUtil.TimeSlice.None))).ToArray<string>());
				}
				list.Add(new Descriptor(UI.BUILDINGEFFECTS.DIET_PRODUCED.text.Replace("{Items}", text3), UI.BUILDINGEFFECTS.TOOLTIPS.DIET_PRODUCED.text.Replace("{Items}", text4), Descriptor.DescriptorType.Effect, false));
			}
			return list;
		}

		public Diet diet;

		public float minConsumedCaloriesBeforePooping = 100f;

		public float maxPoopSizeKG = -1f;

		public float minimumTimeBeforePooping = 10f;

		public float deathTimer = 6000f;

		public bool storePoop;
	}

	public class PauseStates : GameStateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.State
	{
		public GameStateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.State commonPause;

		public GameStateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.State starvingPause;
	}

	public class HungryStates : GameStateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.State
	{
		public GameStateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.State hungry;

		public CreatureCalorieMonitor.HungryStates.OutOfCaloriesState outofcalories;

		public class OutOfCaloriesState : GameStateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.State
		{
			public GameStateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.State wild;

			public GameStateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.State tame;

			public GameStateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.State starvedtodeath;
		}
	}

	[SerializationConfig(MemberSerialization.OptIn)]
	public class Stomach
	{
		public Diet diet { get; private set; }

		public Stomach(GameObject owner, float minConsumedCaloriesBeforePooping, float max_poop_size_in_kg, bool storePoop)
		{
			this.diet = DietManager.Instance.GetPrefabDiet(owner);
			this.owner = owner;
			this.minConsumedCaloriesBeforePooping = minConsumedCaloriesBeforePooping;
			this.storePoop = storePoop;
			this.maxPoopSizeInKG = max_poop_size_in_kg;
		}

		public void Poop()
		{
			this.shouldContinuingPooping = true;
			float num = 0f;
			Tag tag = Tag.Invalid;
			byte b = byte.MaxValue;
			int num2 = 0;
			int num3 = 0;
			bool flag = false;
			for (int i = 0; i < this.caloriesConsumed.Count; i++)
			{
				CreatureCalorieMonitor.Stomach.CaloriesConsumedEntry caloriesConsumedEntry = this.caloriesConsumed[i];
				if (caloriesConsumedEntry.calories > 0f)
				{
					Diet.Info dietInfo = this.diet.GetDietInfo(caloriesConsumedEntry.tag);
					if (dietInfo != null && (!(tag != Tag.Invalid) || !(tag != dietInfo.producedElement)))
					{
						float num4 = ((this.maxPoopSizeInKG < 0f) ? float.MaxValue : this.maxPoopSizeInKG);
						float num5 = Mathf.Clamp(num4 - num, 0f, num4);
						float num6 = Mathf.Min(dietInfo.ConvertConsumptionMassToProducedMass(dietInfo.ConvertCaloriesToConsumptionMass(caloriesConsumedEntry.calories)), num5);
						num += num6;
						tag = dietInfo.producedElement;
						if (dietInfo.diseaseIdx != 255)
						{
							b = dietInfo.diseaseIdx;
							if (!this.storePoop && dietInfo.emmitDiseaseOnCell)
							{
								num3 += (int)(dietInfo.diseasePerKgProduced * num6);
							}
							else
							{
								num2 += (int)(dietInfo.diseasePerKgProduced * num6);
							}
						}
						caloriesConsumedEntry.calories = Mathf.Clamp(caloriesConsumedEntry.calories - dietInfo.ConvertConsumptionMassToCalories(dietInfo.ConvertProducedMassToConsumptionMass(num6)), 0f, float.MaxValue);
						this.caloriesConsumed[i] = caloriesConsumedEntry;
						flag = flag || dietInfo.produceSolidTile;
					}
				}
			}
			if (num <= 0f || tag == Tag.Invalid)
			{
				this.shouldContinuingPooping = false;
				return;
			}
			string text = null;
			Element element = ElementLoader.GetElement(tag);
			if (element != null)
			{
				text = element.name;
			}
			int num7 = Grid.PosToCell(this.owner.transform.GetPosition());
			float temperature = this.owner.GetComponent<PrimaryElement>().Temperature;
			DebugUtil.DevAssert(!this.storePoop || !flag, "Stomach cannot both store poop & create a solid tile.", null);
			if (this.storePoop)
			{
				Storage component = this.owner.GetComponent<Storage>();
				if (element == null)
				{
					GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(tag), Grid.CellToPos(num7, CellAlignment.Top, Grid.SceneLayer.Ore), Grid.SceneLayer.Ore, null, 0);
					PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
					component2.Mass = num;
					component2.AddDisease(b, num2, "CreatureCalorieMonitor.Poop");
					component2.Temperature = temperature;
					gameObject.SetActive(true);
					component.Store(gameObject, true, false, true, false);
					text = gameObject.GetProperName();
				}
				else if (element.IsLiquid)
				{
					component.AddLiquid(element.id, num, temperature, b, num2, false, true);
				}
				else if (element.IsGas)
				{
					component.AddGasChunk(element.id, num, temperature, b, num2, false, true);
				}
				else
				{
					component.AddOre(element.id, num, temperature, b, num2, false, true);
				}
			}
			else
			{
				if (element == null)
				{
					GameObject gameObject2 = GameUtil.KInstantiate(Assets.GetPrefab(tag), Grid.CellToPos(num7, CellAlignment.Top, Grid.SceneLayer.Ore), Grid.SceneLayer.Ore, null, 0);
					PrimaryElement component3 = gameObject2.GetComponent<PrimaryElement>();
					component3.Mass = num;
					component3.AddDisease(b, num2, "CreatureCalorieMonitor.Poop");
					component3.Temperature = temperature;
					gameObject2.SetActive(true);
					text = gameObject2.GetProperName();
				}
				else if (element.IsLiquid)
				{
					FallingWater.instance.AddParticle(num7, element.idx, num, temperature, b, num2, true, false, false, false);
				}
				else if (element.IsGas)
				{
					SimMessages.AddRemoveSubstance(num7, element.idx, CellEventLogger.Instance.ElementConsumerSimUpdate, num, temperature, b, num2, true, -1);
				}
				else if (flag)
				{
					int num8 = this.owner.GetComponent<Facing>().GetFrontCell();
					if (!Grid.IsValidCell(num8))
					{
						global::Debug.LogWarningFormat("{0} attemping to Poop {1} on invalid cell {2} from cell {3}", new object[] { this.owner, element.name, num8, num7 });
						num8 = num7;
					}
					SimMessages.AddRemoveSubstance(num8, element.idx, CellEventLogger.Instance.ElementConsumerSimUpdate, num, temperature, b, num2, true, -1);
				}
				else
				{
					element.substance.SpawnResource(Grid.CellToPosCCC(num7, Grid.SceneLayer.Ore), num, temperature, b, num2, false, false, false);
				}
				if (num3 > 0)
				{
					SimMessages.ModifyDiseaseOnCell(num7, b, num3);
				}
			}
			if (this.GetTotalConsumedCalories() <= 0f)
			{
				this.shouldContinuingPooping = false;
			}
			KPrefabID component4 = this.owner.GetComponent<KPrefabID>();
			if (!Game.Instance.savedInfo.creaturePoopAmount.ContainsKey(component4.PrefabTag))
			{
				Game.Instance.savedInfo.creaturePoopAmount.Add(component4.PrefabTag, 0f);
			}
			Dictionary<Tag, float> creaturePoopAmount = Game.Instance.savedInfo.creaturePoopAmount;
			Tag prefabTag = component4.PrefabTag;
			creaturePoopAmount[prefabTag] += num;
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, text, this.owner.transform, 1.5f, false);
			this.owner.Trigger(-1844238272, null);
		}

		public List<CreatureCalorieMonitor.Stomach.CaloriesConsumedEntry> GetCalorieEntries()
		{
			return this.caloriesConsumed;
		}

		public float GetTotalConsumedCalories()
		{
			float num = 0f;
			foreach (CreatureCalorieMonitor.Stomach.CaloriesConsumedEntry caloriesConsumedEntry in this.caloriesConsumed)
			{
				if (caloriesConsumedEntry.calories > 0f)
				{
					Diet.Info dietInfo = this.diet.GetDietInfo(caloriesConsumedEntry.tag);
					if (dietInfo != null && !(dietInfo.producedElement == Tag.Invalid))
					{
						num += caloriesConsumedEntry.calories;
					}
				}
			}
			return num;
		}

		public float GetFullness()
		{
			return this.GetTotalConsumedCalories() / this.minConsumedCaloriesBeforePooping;
		}

		public bool IsReadyToPoop()
		{
			float totalConsumedCalories = this.GetTotalConsumedCalories();
			return totalConsumedCalories > 0f && (this.shouldContinuingPooping || totalConsumedCalories >= this.minConsumedCaloriesBeforePooping);
		}

		public void Consume(Tag tag, float calories)
		{
			for (int i = 0; i < this.caloriesConsumed.Count; i++)
			{
				CreatureCalorieMonitor.Stomach.CaloriesConsumedEntry caloriesConsumedEntry = this.caloriesConsumed[i];
				if (caloriesConsumedEntry.tag == tag)
				{
					caloriesConsumedEntry.calories += calories;
					this.caloriesConsumed[i] = caloriesConsumedEntry;
					this.caloriesConsumed[i] = caloriesConsumedEntry;
					return;
				}
			}
			CreatureCalorieMonitor.Stomach.CaloriesConsumedEntry caloriesConsumedEntry2 = default(CreatureCalorieMonitor.Stomach.CaloriesConsumedEntry);
			caloriesConsumedEntry2.tag = tag;
			caloriesConsumedEntry2.calories = calories;
			this.caloriesConsumed.Add(caloriesConsumedEntry2);
		}

		public Tag GetNextPoopEntry()
		{
			for (int i = 0; i < this.caloriesConsumed.Count; i++)
			{
				CreatureCalorieMonitor.Stomach.CaloriesConsumedEntry caloriesConsumedEntry = this.caloriesConsumed[i];
				if (caloriesConsumedEntry.calories > 0f)
				{
					Diet.Info dietInfo = this.diet.GetDietInfo(caloriesConsumedEntry.tag);
					if (dietInfo != null && !(dietInfo.producedElement == Tag.Invalid))
					{
						return dietInfo.producedElement;
					}
				}
			}
			return Tag.Invalid;
		}

		[Serialize]
		private List<CreatureCalorieMonitor.Stomach.CaloriesConsumedEntry> caloriesConsumed = new List<CreatureCalorieMonitor.Stomach.CaloriesConsumedEntry>();

		[Serialize]
		private bool shouldContinuingPooping;

		private float minConsumedCaloriesBeforePooping;

		private float maxPoopSizeInKG;

		private GameObject owner;

		private bool storePoop;

		[Serializable]
		public struct CaloriesConsumedEntry
		{
			public Tag tag;

			public float calories;
		}
	}

	public new class Instance : GameStateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, CreatureCalorieMonitor.Def def)
			: base(master, def)
		{
			this.calories = Db.Get().Amounts.Calories.Lookup(base.gameObject);
			this.calories.value = this.calories.GetMax() * 0.9f;
			this.stomach = new CreatureCalorieMonitor.Stomach(master.gameObject, def.minConsumedCaloriesBeforePooping, def.maxPoopSizeKG, def.storePoop);
			this.metabolism = base.gameObject.GetAttributes().Add(Db.Get().CritterAttributes.Metabolism);
			this.deltaCalorieMetabolismModifier = new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, 1f, DUPLICANTS.MODIFIERS.METABOLISM_CALORIE_MODIFIER.NAME, true, false, false);
			this.calories.deltaAttribute.Add(this.deltaCalorieMetabolismModifier);
		}

		public override void StartSM()
		{
			this.prefabID = base.gameObject.GetComponent<KPrefabID>();
			base.StartSM();
		}

		public void OnCaloriesConsumed(object data)
		{
			CreatureCalorieMonitor.CaloriesConsumedEvent caloriesConsumedEvent = (CreatureCalorieMonitor.CaloriesConsumedEvent)data;
			this.calories.value += caloriesConsumedEvent.calories;
			this.stomach.Consume(caloriesConsumedEvent.tag, caloriesConsumedEvent.calories);
			this.lastMealOrPoopTime = Time.time;
		}

		public float GetDeathTimeRemaining()
		{
			return base.smi.def.deathTimer - (GameClock.Instance.GetTime() - base.sm.starvationStartTime.Get(base.smi));
		}

		public void Poop()
		{
			this.lastMealOrPoopTime = Time.time;
			this.stomach.Poop();
		}

		public float GetCalories0to1()
		{
			return this.calories.value / this.calories.GetMax();
		}

		public bool IsHungry()
		{
			return this.GetCalories0to1() < 0.9f;
		}

		public bool IsOutOfCalories()
		{
			return this.GetCalories0to1() <= 0f;
		}

		public const float HUNGRY_RATIO = 0.9f;

		public AmountInstance calories;

		[Serialize]
		public CreatureCalorieMonitor.Stomach stomach;

		public float lastMealOrPoopTime;

		public AttributeInstance metabolism;

		public AttributeModifier deltaCalorieMetabolismModifier;

		public KPrefabID prefabID;
	}
}
