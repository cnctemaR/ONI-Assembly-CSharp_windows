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
		base.serializable = true;
		this.root.EventHandler(GameHashes.CaloriesConsumed, delegate(CreatureCalorieMonitor.Instance smi, object data)
		{
			smi.OnCaloriesConsumed(data);
		}).ToggleBehaviour(GameTags.Creatures.Poop, (CreatureCalorieMonitor.Instance smi) => smi.stomach.IsReadyToPoop() && Time.time - smi.lastMealOrPoopTime > smi.def.minimumTimeBeforePooping, delegate(CreatureCalorieMonitor.Instance smi)
		{
			smi.Poop();
		}).Update(new Action<CreatureCalorieMonitor.Instance, float>(CreatureCalorieMonitor.UpdateMetabolismCalorieModifier), UpdateRate.SIM_200ms, false);
		this.normal.Transition(this.hungry, (CreatureCalorieMonitor.Instance smi) => smi.IsHungry(), UpdateRate.SIM_1000ms);
		this.hungry.DefaultState(this.hungry.hungry).ToggleTag(GameTags.Creatures.Hungry).EventTransition(GameHashes.CaloriesConsumed, this.normal, (CreatureCalorieMonitor.Instance smi) => !smi.IsHungry());
		this.hungry.hungry.Transition(this.normal, (CreatureCalorieMonitor.Instance smi) => !smi.IsHungry(), UpdateRate.SIM_1000ms).Transition(this.hungry.outofcalories, (CreatureCalorieMonitor.Instance smi) => smi.IsOutOfCalories(), UpdateRate.SIM_1000ms).ToggleStatusItem(CREATURES.STATUSITEMS.HUNGRY.NAME, CREATURES.STATUSITEMS.HUNGRY.TOOLTIP, string.Empty, StatusItem.IconType.Info, (NotificationType)0, false, SimViewMode.None, 0, null, null, null);
		this.hungry.outofcalories.DefaultState(this.hungry.outofcalories.wild).Transition(this.hungry.hungry, (CreatureCalorieMonitor.Instance smi) => !smi.IsOutOfCalories(), UpdateRate.SIM_1000ms);
		this.hungry.outofcalories.wild.TagTransition(GameTags.Creatures.Wild, this.hungry.outofcalories.tame, true).ToggleStatusItem(CREATURES.STATUSITEMS.HUNGRY.NAME, CREATURES.STATUSITEMS.HUNGRY.TOOLTIP, string.Empty, StatusItem.IconType.Info, (NotificationType)0, false, SimViewMode.None, 0, null, null, null);
		this.hungry.outofcalories.tame.Enter("StarvationStartTime", delegate(CreatureCalorieMonitor.Instance smi)
		{
			this.starvationStartTime.Set(GameClock.Instance.GetTime(), smi);
		}).Transition(this.hungry.outofcalories.starvedtodeath, (CreatureCalorieMonitor.Instance smi) => smi.GetDeathTimeRemaining() <= 0f, UpdateRate.SIM_1000ms).TagTransition(GameTags.Creatures.Wild, this.hungry.outofcalories.wild, false)
			.ToggleStatusItem(CREATURES.STATUSITEMS.STARVING.NAME, CREATURES.STATUSITEMS.STARVING.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, 0, (string str, CreatureCalorieMonitor.Instance smi) => str.Replace("{TimeUntilDeath}", GameUtil.GetFormattedCycles(smi.GetDeathTimeRemaining(), "F1")), null, null)
			.ToggleNotification((CreatureCalorieMonitor.Instance smi) => new Notification(CREATURES.STATUSITEMS.STARVING.NOTIFICATION_NAME, NotificationType.BadMinor, HashedString.Invalid, (List<Notification> notifications, object data) => CREATURES.STATUSITEMS.STARVING.NOTIFICATION_TOOLTIP + notifications.ReduceMessages(false), null, true, 0f, null, null, null))
			.ToggleEffect((CreatureCalorieMonitor.Instance smi) => this.outOfCaloriesTame);
		this.hungry.outofcalories.starvedtodeath.Enter(delegate(CreatureCalorieMonitor.Instance smi)
		{
			smi.GetSMI<DeathMonitor.Instance>().Kill(Db.Get().Deaths.Starvation);
		});
		this.outOfCaloriesTame = new Effect("OutOfCaloriesTame", CREATURES.MODIFIERS.OUT_OF_CALORIES.NAME, CREATURES.MODIFIERS.OUT_OF_CALORIES.TOOLTIP, 0f, false, false, false);
		this.outOfCaloriesTame.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, -10f, CREATURES.MODIFIERS.OUT_OF_CALORIES.NAME, false, false, true));
	}

	private static void UpdateMetabolismCalorieModifier(CreatureCalorieMonitor.Instance smi, float dt)
	{
		smi.deltaCalorieMetabolismModifier.SetValue(1f - Db.Get().CritterAttributes.Metabolism.Lookup(smi.gameObject).GetTotalValue() / 100f);
	}

	public GameStateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.State normal;

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
			if (this.diet.consumedTags.Count > 0)
			{
				WildnessMonitor.Def def = obj.GetDef<WildnessMonitor.Def>();
				float calorie_loss_per_second = 0f;
				foreach (AttributeModifier attributeModifier in def.tameEffect.SelfModifiers)
				{
					if (attributeModifier.AttributeId == Db.Get().Amounts.Calories.deltaAttribute.Id)
					{
						calorie_loss_per_second = attributeModifier.Value;
					}
				}
				string text = string.Join(", ", this.diet.consumedTags.Select<KeyValuePair<Tag, float>, string>((KeyValuePair<Tag, float> t) => t.Key.ProperName()).ToArray<string>());
				string text2 = string.Join("\n", this.diet.consumedTags.Select<KeyValuePair<Tag, float>, string>((KeyValuePair<Tag, float> t) => UI.BUILDINGEFFECTS.DIET_CONSUMED_ITEM.text.Replace("{Food}", t.Key.ProperName()).Replace("{Amount}", GameUtil.GetFormattedMass(calorie_loss_per_second / t.Value, GameUtil.TimeSlice.PerCycle, GameUtil.MetricMassFormat.Kilogram, true, "{0:0.#}"))).ToArray<string>());
				list.Add(new Descriptor(UI.BUILDINGEFFECTS.DIET_CONSUMED.text.Replace("{Foodlist}", text), UI.BUILDINGEFFECTS.TOOLTIPS.DIET_CONSUMED.text.Replace("{Foodlist}", text2), Descriptor.DescriptorType.Effect, false));
			}
			if (this.diet.producedTags.Count > 0)
			{
				string text3 = string.Join(", ", this.diet.producedTags.Select<KeyValuePair<Tag, float>, string>((KeyValuePair<Tag, float> t) => t.Key.ProperName()).ToArray<string>());
				string text4 = string.Join("\n", this.diet.producedTags.Select<KeyValuePair<Tag, float>, string>((KeyValuePair<Tag, float> t) => UI.BUILDINGEFFECTS.DIET_PRODUCED_ITEM.text.Replace("{Item}", t.Key.ProperName()).Replace("{Percent}", GameUtil.GetFormattedPercent(t.Value * 100f, GameUtil.TimeSlice.None))).ToArray<string>());
				list.Add(new Descriptor(UI.BUILDINGEFFECTS.DIET_PRODUCED.text.Replace("{Items}", text3), UI.BUILDINGEFFECTS.TOOLTIPS.DIET_PRODUCED.text.Replace("{Items}", text4), Descriptor.DescriptorType.Effect, false));
			}
			return list;
		}

		public Diet diet;

		public float minPoopSizeInCalories = 100f;

		public float minimumTimeBeforePooping = 10f;

		public float deathTimer = 6000f;
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
		public Stomach(Diet diet, GameObject owner, float min_poop_size_in_calories)
		{
			this.diet = diet;
			this.owner = owner;
			this.minPoopSizeInCalories = min_poop_size_in_calories;
		}

		public void Poop()
		{
			float num = 0f;
			Tag tag = Tag.Invalid;
			byte b = byte.MaxValue;
			int num2 = 0;
			for (int i = 0; i < this.caloriesConsumed.Count; i++)
			{
				CreatureCalorieMonitor.Stomach.CaloriesConsumedEntry caloriesConsumedEntry = this.caloriesConsumed[i];
				if (caloriesConsumedEntry.calories > 0f)
				{
					Diet.Info dietInfo = this.diet.GetDietInfo(new TagBits(caloriesConsumedEntry.tag));
					if (dietInfo != null)
					{
						if (!(tag != Tag.Invalid) || !(tag != dietInfo.producedElement))
						{
							num += dietInfo.ConvertConsumptionMassToProducedMass(dietInfo.ConvertCaloriesToConsumptionMass(caloriesConsumedEntry.calories));
							tag = dietInfo.producedElement;
							b = dietInfo.diseaseIdx;
							num2 = (int)(dietInfo.diseasePerKgProduced * num);
							caloriesConsumedEntry.calories = 0f;
							this.caloriesConsumed[i] = caloriesConsumedEntry;
						}
					}
				}
			}
			if (num <= 0f || tag == Tag.Invalid)
			{
				return;
			}
			Element element = ElementLoader.GetElement(tag);
			int num3 = Grid.PosToCell(this.owner.transform.GetPosition());
			float temperature = this.owner.GetComponent<PrimaryElement>().Temperature;
			if (element.IsLiquid)
			{
				FallingWater.instance.AddParticle(num3, (byte)ElementLoader.GetElementIndex(element.id), num, temperature, b, num2, true, false, false, false);
			}
			else
			{
				element.substance.SpawnResource(Grid.CellToPosCCC(num3, Grid.SceneLayer.Ore), num, temperature, b, num2, false, false);
			}
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, element.name, this.owner.transform, 1.5f, false);
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
					Diet.Info dietInfo = this.diet.GetDietInfo(new TagBits(caloriesConsumedEntry.tag));
					if (dietInfo != null)
					{
						if (!(dietInfo.producedElement == Tag.Invalid))
						{
							num += caloriesConsumedEntry.calories;
						}
					}
				}
			}
			return num;
		}

		public float GetFullness()
		{
			float totalConsumedCalories = this.GetTotalConsumedCalories();
			return totalConsumedCalories / this.minPoopSizeInCalories;
		}

		public bool IsReadyToPoop()
		{
			float totalConsumedCalories = this.GetTotalConsumedCalories();
			return totalConsumedCalories > 0f && totalConsumedCalories >= this.minPoopSizeInCalories;
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
					return;
				}
			}
			CreatureCalorieMonitor.Stomach.CaloriesConsumedEntry caloriesConsumedEntry2 = default(CreatureCalorieMonitor.Stomach.CaloriesConsumedEntry);
			caloriesConsumedEntry2.tag = tag;
			caloriesConsumedEntry2.calories = calories;
			this.caloriesConsumed.Add(caloriesConsumedEntry2);
		}

		[Serialize]
		private List<CreatureCalorieMonitor.Stomach.CaloriesConsumedEntry> caloriesConsumed = new List<CreatureCalorieMonitor.Stomach.CaloriesConsumedEntry>();

		private float minPoopSizeInCalories;

		private Diet diet;

		private GameObject owner;

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
			this.stomach = new CreatureCalorieMonitor.Stomach(def.diet, master.gameObject, def.minPoopSizeInCalories);
			this.metabolism = base.gameObject.GetAttributes().Add(Db.Get().CritterAttributes.Metabolism);
			this.deltaCalorieMetabolismModifier = new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, 1f, DUPLICANTS.MODIFIERS.METABOLISM_CALORIE_MODIFIER.NAME, true, false, false);
			this.calories.deltaAttribute.Add(DUPLICANTS.MODIFIERS.METABOLISM_CALORIE_MODIFIER.NAME, this.deltaCalorieMetabolismModifier);
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

		private float GetCalories0to1()
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

		public AmountInstance calories;

		[Serialize]
		public CreatureCalorieMonitor.Stomach stomach;

		public float lastMealOrPoopTime;

		public AttributeInstance metabolism;

		public AttributeModifier deltaCalorieMetabolismModifier;
	}
}
