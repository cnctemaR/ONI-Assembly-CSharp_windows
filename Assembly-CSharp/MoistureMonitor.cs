using System;
using System.Collections.Generic;
using System.Text;
using Database;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class MoistureMonitor : GameStateMachine<MoistureMonitor, MoistureMonitor.Instance, IStateMachineTarget, MoistureMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.onDryLand;
		this.root.ToggleStateMachine((MoistureMonitor.Instance smi) => new LubricatedMovementMonitor.Instance(smi.master)).EventHandler(GameHashes.Happy, delegate(MoistureMonitor.Instance smi)
		{
			this.ToggleUnhappyModifier(smi, false);
		}).EventHandler(GameHashes.Unhappy, delegate(MoistureMonitor.Instance smi)
		{
			this.ToggleUnhappyModifier(smi, true);
		})
			.EventHandler(GameHashes.TagsChanged, new GameStateMachine<MoistureMonitor, MoistureMonitor.Instance, IStateMachineTarget, MoistureMonitor.Def>.GameEvent.Callback(this.UpdateTags))
			.EventTransition(GameHashes.Died, this.dead, null)
			.Enter(delegate(MoistureMonitor.Instance smi)
			{
				if (smi.HasTag(GameTags.Creatures.Wild))
				{
					smi.attributes.Add(smi.wildMucusModifier);
				}
			});
		this.onDryLand.UpdateTransition(this.inLiquid, new Func<MoistureMonitor.Instance, float, bool>(MoistureMonitor.IsInLiquid), UpdateRate.SIM_200ms, false).ToggleAttributeModifier("dry", (MoistureMonitor.Instance smi) => smi.baseMoistureModifier, null).ToggleAttributeModifier("mucus", (MoistureMonitor.Instance smi) => smi.onDryLandMucusModifier, null)
			.UpdateTransition(this.secreting, new Func<MoistureMonitor.Instance, float, bool>(this.IsMucusEnough), UpdateRate.SIM_200ms, false);
		this.inLiquid.UpdateTransition(this.onDryLand, (MoistureMonitor.Instance smi, float dt) => !MoistureMonitor.IsInLiquid(smi, dt), UpdateRate.SIM_200ms, false).ToggleAttributeModifier("wet", (MoistureMonitor.Instance smi) => smi.wetMoistureModifier, null);
		this.secreting.ToggleBehaviour(GameTags.Creatures.Behaviours.SecretingMucusBehavior, new StateMachine<MoistureMonitor, MoistureMonitor.Instance, IStateMachineTarget, MoistureMonitor.Def>.Transition.ConditionCallback(MoistureMonitor.CanProduceLubricant), delegate(MoistureMonitor.Instance smi)
		{
			smi.GoTo(this.onDryLand);
		});
		this.dead.DoNothing();
	}

	private static bool IsInLiquid(MoistureMonitor.Instance smi, float _)
	{
		return Grid.IsSubstantialLiquid(Grid.PosToCell(smi), 0.02f);
	}

	private bool IsMucusEnough(MoistureMonitor.Instance smi, float _)
	{
		return smi.mucusAmount.value >= (smi.HasTag(GameTags.Creatures.Dry) ? 2f : 10f);
	}

	private void UpdateTags(MoistureMonitor.Instance smi, object data)
	{
		if (data is TagChangedEventData)
		{
			TagChangedEventData tagChangedEventData = (TagChangedEventData)data;
			if (tagChangedEventData.tag == GameTags.Creatures.Wild)
			{
				if (tagChangedEventData.added)
				{
					smi.attributes.Add(smi.wildMucusModifier);
					return;
				}
				smi.attributes.Remove(smi.wildMucusModifier);
			}
		}
	}

	private void ToggleUnhappyModifier(MoistureMonitor.Instance smi, bool enabled)
	{
		if (enabled)
		{
			smi.attributes.Add(smi.unhappyMucusModifier);
			return;
		}
		smi.attributes.Remove(smi.unhappyMucusModifier);
	}

	private static bool CanProduceLubricant(MoistureMonitor.Instance smi)
	{
		if (smi.effects.HasEffect(MoistureMonitor.RECENTLY_PRODUCED_LUBRICANT_EFFECT))
		{
			return false;
		}
		int num = Grid.CellBelow(Grid.PosToCell(smi));
		return Grid.IsValidCell(num) && Grid.IsSolidCell(num);
	}

	private GameStateMachine<MoistureMonitor, MoistureMonitor.Instance, IStateMachineTarget, MoistureMonitor.Def>.State onDryLand;

	private GameStateMachine<MoistureMonitor, MoistureMonitor.Instance, IStateMachineTarget, MoistureMonitor.Def>.State inLiquid;

	private GameStateMachine<MoistureMonitor, MoistureMonitor.Instance, IStateMachineTarget, MoistureMonitor.Def>.State secreting;

	public GameStateMachine<MoistureMonitor, MoistureMonitor.Instance, IStateMachineTarget, MoistureMonitor.Def>.State dead;

	public static readonly HashedString RECENTLY_PRODUCED_LUBRICANT_EFFECT = "RecentlyProducedLubricant";

	public class Def : StateMachine.BaseDef, IGameObjectEffectDescriptor
	{
		public override void Configure(GameObject prefab)
		{
			global::Database.Amounts amounts = Db.Get().Amounts;
			List<string> initialAmounts = prefab.GetComponent<Modifiers>().initialAmounts;
			initialAmounts.Add(amounts.Moisture.Id);
			initialAmounts.Add(amounts.Mucus.Id);
		}

		private static void AppendMucusModifierTooltipBullet(StringBuilder tooltipBuilder, AttributeModifier modifier, bool showSign)
		{
			string text = (modifier.IsMultiplier ? GameUtil.GetFormattedPercent(modifier.Value * 100f, GameUtil.TimeSlice.None) : GameUtil.GetFormattedMass(modifier.Value, GameUtil.TimeSlice.PerCycle, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
			if (showSign)
			{
				text = GameUtil.AddPositiveSign(text, modifier.Value > 0f);
			}
			tooltipBuilder.AppendFormat(DUPLICANTS.ATTRIBUTES.MODIFIER_ENTRY, modifier.GetDescription(), text);
		}

		public float GetMaxModification()
		{
			return this.onDryLandModifier;
		}

		public List<Descriptor> GetDescriptors(GameObject obj)
		{
			string text = ElementLoader.FindElementByHash(this.lubricant).tag.ProperName();
			float delta = this.onDryLandModifier;
			AmountInstance amountInstance = Db.Get().Amounts.Mucus.Lookup(obj);
			if (amountInstance != null)
			{
				delta = amountInstance.GetDelta();
			}
			string formattedMass = GameUtil.GetFormattedMass(delta, GameUtil.TimeSlice.PerCycle, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
			string text2 = GlobalStringBuilderPool.ReturnAndFree(GlobalStringBuilderPool.Alloc().Append(UI.BUILDINGEFFECTS.MUCUS_SECRETION).Replace("{Item}", text)
				.Replace("{Rate}", formattedMass));
			StringBuilder stringBuilder = GlobalStringBuilderPool.Alloc();
			stringBuilder.Append(UI.BUILDINGEFFECTS.TOOLTIPS.MUCUS_SECRETION);
			stringBuilder.Replace("{Item}", text);
			stringBuilder.Replace("{Rate}", formattedMass);
			if (amountInstance != null)
			{
				ArrayRef<AttributeModifier> modifiers = amountInstance.deltaAttribute.Modifiers;
				int num = -1;
				for (int i = 0; i < modifiers.Count; i++)
				{
					if (modifiers[i].GetDescription() == CREATURES.MODIFIERS.MUCUS.BASE_RATE)
					{
						num = i;
						break;
					}
				}
				if (num >= 0)
				{
					MoistureMonitor.Def.AppendMucusModifierTooltipBullet(stringBuilder, modifiers[num], false);
				}
				for (int j = 0; j < modifiers.Count; j++)
				{
					if (j != num)
					{
						MoistureMonitor.Def.AppendMucusModifierTooltipBullet(stringBuilder, modifiers[j], true);
					}
				}
			}
			string text3 = GlobalStringBuilderPool.ReturnAndFree(stringBuilder);
			return new List<Descriptor>
			{
				new Descriptor(text2, text3, Descriptor.DescriptorType.Effect, false)
			};
		}

		private const float DEFAULT_ON_DRY_LAND_MODIFIER = 0.05f;

		private const float DEFAULT_DRY_RATE = -0.05f;

		private const float DEFAULT_SOAK_RATE = 10f;

		public float onDryLandModifier = 0.05f;

		public SimHashes lubricant;

		public float lubricantTemperatureKelvin;

		[Tooltip("Stop producing more Mucus when this much is stored")]
		public float sufficientMoistureThreshold = 10f;

		[Tooltip("Decrease moisture at this rate while on dry land")]
		public float dryRate = -0.05f;

		[Tooltip("Increase moisture at this rate while inside liquid")]
		public float soakRate = 10f;
	}

	public new class Instance : GameStateMachine<MoistureMonitor, MoistureMonitor.Instance, IStateMachineTarget, MoistureMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, MoistureMonitor.Def def)
			: base(master, def)
		{
			global::Database.Amounts amounts = Db.Get().Amounts;
			this.moisture = amounts.Moisture.Lookup(base.gameObject);
			this.moisture.value = this.moisture.GetMax();
			this.baseMoistureModifier = new AttributeModifier(this.moisture.amount.deltaAttribute.Id, def.dryRate, CREATURES.MODIFIERS.MOISTURE_LOSS_RATE.NAME, false, false, true);
			this.wetMoistureModifier = new AttributeModifier(this.moisture.amount.deltaAttribute.Id, def.soakRate, CREATURES.MODIFIERS.MOISTURE_GAIN_RATE.NAME, false, false, true);
			this.attributes = master.gameObject.GetAttributes();
			this.mucusAmount = amounts.Mucus.Lookup(base.gameObject);
			this.mucusAmount.value = this.mucusAmount.GetMax();
			this.onDryLandMucusModifier = new AttributeModifier(this.mucusAmount.amount.deltaAttribute.Id, def.onDryLandModifier, CREATURES.MODIFIERS.MUCUS.ON_DRY_LAND, false, false, true);
			this.unhappyMucusModifier = new AttributeModifier(this.mucusAmount.amount.deltaAttribute.Id, -0.5f, CREATURES.MODIFIERS.MUCUS.UNHAPPY, true, false, true);
			this.wildMucusModifier = new AttributeModifier(this.mucusAmount.amount.deltaAttribute.Id, -0.75f, CREATURES.MODIFIERS.MUCUS.WILD, true, false, true);
		}

		public void ProduceLubricant()
		{
			float value = this.mucusAmount.value;
			if (value > 0f)
			{
				BubbleManager.instance.SpawnBubble(base.def.lubricant, base.transform.GetPosition(), value, base.def.lubricantTemperatureKelvin, BubbleManager.Disease.None, null);
				base.Trigger(1151073968, null);
				this.effects.Add(MoistureMonitor.RECENTLY_PRODUCED_LUBRICANT_EFFECT, true);
				this.mucusAmount.value = 0f;
			}
		}

		private const float UNHAPPY_MUCUS_MODIFIER = -0.5f;

		private const float WILD_MUCUS_MODIFIER = -0.75f;

		[MyCmpReq]
		public Effects effects;

		public AmountInstance mucusAmount;

		public AttributeModifier onDryLandMucusModifier;

		public AttributeModifier wildMucusModifier;

		public AttributeModifier unhappyMucusModifier;

		public Klei.AI.Attributes attributes;

		public WildnessMonitor.Instance wildnessMonitor;

		public AmountInstance moisture;

		public AttributeModifier baseMoistureModifier;

		public AttributeModifier wetMoistureModifier;
	}
}
