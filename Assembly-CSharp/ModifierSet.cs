using System;
using System.Collections.Generic;
using Database;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class ModifierSet : ScriptableObject
{
	public virtual void Initialize()
	{
		this.ResourceTable = new List<Resource>();
		this.Root = new ResourceSet<Resource>("Root", null);
		this.modifierInfos = new ModifierSet.ModifierInfos();
		this.modifierInfos.Load(this.modifiersFile);
		this.Attributes = new global::Database.Attributes(this.Root);
		this.BuildingAttributes = new BuildingAttributes(this.Root);
		this.CritterAttributes = new CritterAttributes(this.Root);
		this.PlantAttributes = new PlantAttributes(this.Root);
		this.effects = new ResourceSet<Effect>("Effects", this.Root);
		this.traits = new ModifierSet.TraitSet();
		this.traitGroups = new ModifierSet.TraitGroupSet();
		this.FertilityModifiers = new FertilityModifiers();
		this.MooSongModifiers = new MooSongModifiers();
		this.Amounts = new global::Database.Amounts();
		this.Amounts.Load();
		this.AttributeConverters = new global::Database.AttributeConverters();
		this.LoadEffects();
		this.LoadFertilityModifiers();
		this.LoadMooSongsModifiers();
	}

	public static float ConvertValue(float value, Units units)
	{
		if (Units.PerDay == units)
		{
			return value * 0.0016666667f;
		}
		return value;
	}

	private void LoadEffects()
	{
		foreach (ModifierSet.ModifierInfo modifierInfo in this.modifierInfos)
		{
			if (!this.effects.Exists(modifierInfo.Id) && (modifierInfo.Type == "Effect" || modifierInfo.Type == "Base" || modifierInfo.Type == "Need"))
			{
				string text = Strings.Get(string.Format("STRINGS.DUPLICANTS.MODIFIERS.{0}.NAME", modifierInfo.Id.ToUpper()));
				string text2 = Strings.Get(string.Format("STRINGS.DUPLICANTS.MODIFIERS.{0}.TOOLTIP", modifierInfo.Id.ToUpper()));
				Effect effect = new Effect(modifierInfo.Id, text, text2, modifierInfo.Duration * 600f, modifierInfo.ShowInUI && modifierInfo.Type != "Need", modifierInfo.TriggerFloatingText, modifierInfo.IsBad, modifierInfo.EmoteAnim, modifierInfo.EmoteCooldown, modifierInfo.StompGroup, modifierInfo.CustomIcon);
				effect.stompPriority = modifierInfo.StompPriority;
				foreach (ModifierSet.ModifierInfo modifierInfo2 in this.modifierInfos)
				{
					if (modifierInfo2.Id == modifierInfo.Id)
					{
						effect.Add(new AttributeModifier(modifierInfo2.Attribute, ModifierSet.ConvertValue(modifierInfo2.Value, modifierInfo2.Units), text, modifierInfo2.Multiplier, false, true));
					}
				}
				this.effects.Add(effect);
			}
		}
		Reactable.ReactablePrecondition reactablePrecondition = delegate(GameObject go, Navigator.ActiveTransition n)
		{
			int num = Grid.PosToCell(go);
			return Grid.IsValidCell(num) && Grid.IsGas(num);
		};
		this.effects.Get("WetFeet").AddEmotePrecondition(reactablePrecondition);
		this.effects.Get("SoakingWet").AddEmotePrecondition(reactablePrecondition);
		Effect effect2 = new Effect("PassedOutSleep", DUPLICANTS.MODIFIERS.PASSEDOUTSLEEP.NAME, DUPLICANTS.MODIFIERS.PASSEDOUTSLEEP.TOOLTIP, 0f, true, true, true, null, 0f, null, true, "status_item_exhausted", -1f);
		effect2.Add(new AttributeModifier(Db.Get().Amounts.Stamina.deltaAttribute.Id, 0.6666667f, DUPLICANTS.MODIFIERS.PASSEDOUTSLEEP.NAME, false, false, true));
		effect2.Add(new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, -0.033333335f, DUPLICANTS.MODIFIERS.PASSEDOUTSLEEP.NAME, false, false, true));
		this.effects.Add(effect2);
		Effect effect3 = new Effect("WarmTouch", DUPLICANTS.MODIFIERS.WARMTOUCH.NAME, DUPLICANTS.MODIFIERS.WARMTOUCH.TOOLTIP, 120f, new string[] { "WetFeet" }, true, true, false, null, 0f, null, false, "", -1f);
		this.effects.Add(effect3);
		Effect effect4 = new Effect("WarmTouchFood", DUPLICANTS.MODIFIERS.WARMTOUCHFOOD.NAME, DUPLICANTS.MODIFIERS.WARMTOUCHFOOD.TOOLTIP, 600f, new string[] { "WetFeet" }, true, true, false, null, 0f, null, false, "", -1f);
		this.effects.Add(effect4);
		Effect effect5 = new Effect("RefreshingTouch", DUPLICANTS.MODIFIERS.REFRESHINGTOUCH.NAME, DUPLICANTS.MODIFIERS.REFRESHINGTOUCH.TOOLTIP, 120f, true, true, false, null, -1f, 0f, null, "");
		this.effects.Add(effect5);
		Effect effect6 = new Effect("GunkSick", DUPLICANTS.MODIFIERS.GUNKSICK.NAME, DUPLICANTS.MODIFIERS.GUNKSICK.TOOLTIP, 0f, true, true, true, null, -1f, 0f, null, "");
		effect6.Add(new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0.033333335f, DUPLICANTS.MODIFIERS.GUNKSICK.NAME, false, false, true));
		this.effects.Add(effect6);
		Effect effect7 = new Effect("ExpellingGunk", DUPLICANTS.MODIFIERS.EXPELLINGGUNK.NAME, DUPLICANTS.MODIFIERS.EXPELLINGGUNK.TOOLTIP, 0f, true, true, true, null, -1f, 0f, null, "");
		effect7.Add(new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0.083333336f, DUPLICANTS.MODIFIERS.GUNKSICK.NAME, false, false, true));
		this.effects.Add(effect7);
		Effect effect8 = new Effect("GunkHungover", DUPLICANTS.MODIFIERS.GUNKHUNGOVER.NAME, DUPLICANTS.MODIFIERS.GUNKHUNGOVER.TOOLTIP, 600f, true, false, true, null, -1f, 0f, null, "");
		effect8.Add(new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0.033333335f, DUPLICANTS.MODIFIERS.GUNKHUNGOVER.NAME, false, false, true));
		this.effects.Add(effect8);
		Effect effect9 = new Effect("NoLubricationMinor", DUPLICANTS.MODIFIERS.NOLUBRICATIONMINOR.NAME, DUPLICANTS.MODIFIERS.NOLUBRICATIONMINOR.TOOLTIP, 0f, true, true, true, null, -1f, 0f, null, "");
		effect9.Add(new AttributeModifier(Db.Get().Attributes.Athletics.Id, -4f, DUPLICANTS.MODIFIERS.NOLUBRICATIONMINOR.NAME, false, false, true));
		effect9.Add(new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0.025f, DUPLICANTS.MODIFIERS.NOLUBRICATIONMINOR.NAME, false, false, true));
		this.effects.Add(effect9);
		Effect effect10 = new Effect("NoLubricationMajor", DUPLICANTS.MODIFIERS.NOLUBRICATIONMAJOR.NAME, DUPLICANTS.MODIFIERS.NOLUBRICATIONMAJOR.TOOLTIP, 0f, true, true, true, null, -1f, 0f, null, "");
		effect10.Add(new AttributeModifier(Db.Get().Attributes.Athletics.Id, -8f, DUPLICANTS.MODIFIERS.NOLUBRICATIONMAJOR.NAME, false, false, true));
		effect10.Add(new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0.05f, DUPLICANTS.MODIFIERS.NOLUBRICATIONMINOR.NAME, false, false, true));
		this.effects.Add(effect10);
		Effect effect11 = new Effect("BionicOffline", DUPLICANTS.MODIFIERS.BIONICOFFLINE.NAME, DUPLICANTS.MODIFIERS.BIONICOFFLINE.TOOLTIP, 0f, false, true, true, null, -1f, 0f, null, "");
		effect11.Add(new AttributeModifier(Db.Get().Amounts.BionicOil.deltaAttribute.Id, 0f, DUPLICANTS.MODIFIERS.BIONICOFFLINE.NAME, false, false, true));
		this.effects.Add(effect11);
		Effect effect12 = new Effect("BionicBedTimeEffect", DUPLICANTS.MODIFIERS.BIONICBEDTIMEEFFECT.NAME, DUPLICANTS.MODIFIERS.BIONICBEDTIMEEFFECT.TOOLTIP, 0f, false, false, false, null, -1f, 0f, null, "");
		effect12.Add(new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, -0.033333335f, DUPLICANTS.MODIFIERS.BIONICBEDTIMEEFFECT.NAME, false, false, true));
		this.effects.Add(effect12);
		Effect effect13 = new Effect("BionicWaterStress", DUPLICANTS.MODIFIERS.BIONICWATERSTRESS.NAME, DUPLICANTS.MODIFIERS.BIONICWATERSTRESS.TOOLTIP, 0f, true, true, true, null, -1f, 0f, null, "");
		effect13.Add(new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0.33333334f, DUPLICANTS.MODIFIERS.BIONICWATERSTRESS.NAME, false, false, true));
		this.effects.Add(effect13);
		Effect effect14 = new Effect("RecentlySlippedTracker", DUPLICANTS.MODIFIERS.SLIPPED.NAME, DUPLICANTS.MODIFIERS.SLIPPED.TOOLTIP, 100f, false, false, true, null, -1f, 0f, null, "");
		this.effects.Add(effect14);
		foreach (Effect effect15 in BionicOilMonitor.LUBRICANT_TYPE_EFFECT.Values)
		{
			this.effects.Add(effect15);
		}
		this.CreateRoomEffects();
		this.CreateCritteEffects();
	}

	private void CreateRoomEffects()
	{
	}

	private void CreateMosquitoEffects()
	{
		Effect effect = new Effect("MosquitoFed", global::STRINGS.CREATURES.MODIFIERS.MOSQUITO_FED.NAME, global::STRINGS.CREATURES.MODIFIERS.MOSQUITO_FED.TOOLTIP, 600f, true, false, false, null, -1f, 0f, null, "");
		float num = 0.4f;
		float num2 = 0.9f / num - 1f;
		effect.Add(new AttributeModifier(Db.Get().Amounts.Fertility.deltaAttribute.Id, num2, effect.Name, true, false, true));
		this.effects.Add(effect);
		Effect effect2 = new Effect("DupeMosquitoBite", global::STRINGS.CREATURES.MODIFIERS.DUPE_MOSQUITO_BITE.NAME, global::STRINGS.CREATURES.MODIFIERS.DUPE_MOSQUITO_BITE.TOOLTIP, 600f, true, true, true, null, -1f, 0f, null, "");
		effect2.Add(new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0.016666668f, global::STRINGS.CREATURES.MODIFIERS.DUPE_MOSQUITO_BITE.NAME, false, false, true));
		effect2.Add(new AttributeModifier(Db.Get().Attributes.Sneezyness.Id, 5f, global::STRINGS.CREATURES.MODIFIERS.DUPE_MOSQUITO_BITE.NAME, false, false, true));
		effect2.Add(new AttributeModifier(Db.Get().Attributes.Athletics.Id, -1f, global::STRINGS.CREATURES.MODIFIERS.DUPE_MOSQUITO_BITE.NAME, false, false, true));
		this.effects.Add(effect2);
		Effect effect3 = new Effect("DupeMosquitoBiteSuppressed", global::STRINGS.CREATURES.MODIFIERS.DUPE_MOSQUITO_BITE_SUPPRESSED.NAME, global::STRINGS.CREATURES.MODIFIERS.DUPE_MOSQUITO_BITE_SUPPRESSED.TOOLTIP, 600f, false, false, false, null, -1f, 0f, null, "");
		this.effects.Add(effect3);
		Effect effect4 = new Effect("CritterMosquitoBite", global::STRINGS.CREATURES.MODIFIERS.CRITTER_MOSQUITO_BITE.NAME, global::STRINGS.CREATURES.MODIFIERS.CRITTER_MOSQUITO_BITE.TOOLTIP, 300f, true, true, true, null, -1f, 0f, null, "");
		effect4.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, -1f, global::STRINGS.CREATURES.MODIFIERS.CRITTER_MOSQUITO_BITE.NAME, false, false, true));
		this.effects.Add(effect4);
		Effect effect5 = new Effect("CritterMosquitoBiteSuppressed", global::STRINGS.CREATURES.MODIFIERS.CRITTER_MOSQUITO_BITE_SUPPRESSED.NAME, global::STRINGS.CREATURES.MODIFIERS.CRITTER_MOSQUITO_BITE_SUPPRESSED.TOOLTIP, 300f, false, false, false, null, -1f, 0f, null, "");
		this.effects.Add(effect5);
	}

	public void CreateCritteEffects()
	{
		Effect effect = new Effect("Ranched", global::STRINGS.CREATURES.MODIFIERS.RANCHED.NAME, global::STRINGS.CREATURES.MODIFIERS.RANCHED.TOOLTIP, 600f, true, true, false, null, -1f, 0f, null, "");
		effect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, 5f, global::STRINGS.CREATURES.MODIFIERS.RANCHED.NAME, false, false, true));
		effect.Add(new AttributeModifier(Db.Get().Amounts.Wildness.deltaAttribute.Id, -0.09166667f, global::STRINGS.CREATURES.MODIFIERS.RANCHED.NAME, false, false, true));
		this.effects.Add(effect);
		Effect effect2 = new Effect("HadMilk", global::STRINGS.CREATURES.MODIFIERS.GOTMILK.NAME, global::STRINGS.CREATURES.MODIFIERS.GOTMILK.TOOLTIP, 600f, true, true, false, null, -1f, 0f, null, "");
		effect2.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, 5f, global::STRINGS.CREATURES.MODIFIERS.GOTMILK.NAME, false, false, true));
		this.effects.Add(effect2);
		Effect effect3 = new Effect("EggSong", global::STRINGS.CREATURES.MODIFIERS.INCUBATOR_SONG.NAME, global::STRINGS.CREATURES.MODIFIERS.INCUBATOR_SONG.TOOLTIP, 600f, true, false, false, null, -1f, 0f, null, "");
		effect3.Add(new AttributeModifier(Db.Get().Amounts.Incubation.deltaAttribute.Id, 4f, global::STRINGS.CREATURES.MODIFIERS.INCUBATOR_SONG.NAME, true, false, true));
		this.effects.Add(effect3);
		Effect effect4 = new Effect("EggHug", global::STRINGS.CREATURES.MODIFIERS.EGGHUG.NAME, global::STRINGS.CREATURES.MODIFIERS.EGGHUG.TOOLTIP, 600f, true, true, false, null, -1f, 0f, null, "");
		effect4.Add(new AttributeModifier(Db.Get().Amounts.Incubation.deltaAttribute.Id, 1f, global::STRINGS.CREATURES.MODIFIERS.EGGHUG.NAME, true, false, true));
		this.effects.Add(effect4);
		Effect effect5 = new Effect("HuggingFrenzy", global::STRINGS.CREATURES.MODIFIERS.HUGGINGFRENZY.NAME, global::STRINGS.CREATURES.MODIFIERS.HUGGINGFRENZY.TOOLTIP, 600f, true, false, false, null, -1f, 0f, null, "");
		this.effects.Add(effect5);
		Effect effect6 = new Effect("DivergentCropTended", global::STRINGS.CREATURES.MODIFIERS.DIVERGENTPLANTTENDED.NAME, global::STRINGS.CREATURES.MODIFIERS.DIVERGENTPLANTTENDED.TOOLTIP, 600f, true, true, false, null, -1f, 0f, null, "");
		effect6.Add(new AttributeModifier(Db.Get().Amounts.Maturity.deltaAttribute.Id, 0.05f, global::STRINGS.CREATURES.MODIFIERS.DIVERGENTPLANTTENDED.NAME, true, false, true));
		effect6.Add(new AttributeModifier(Db.Get().Amounts.Maturity2.deltaAttribute.Id, 0.05f, global::STRINGS.CREATURES.MODIFIERS.DIVERGENTPLANTTENDED.NAME, true, false, true));
		this.effects.Add(effect6);
		Effect effect7 = new Effect("DivergentCropTendedWorm", global::STRINGS.CREATURES.MODIFIERS.DIVERGENTPLANTTENDEDWORM.NAME, global::STRINGS.CREATURES.MODIFIERS.DIVERGENTPLANTTENDEDWORM.TOOLTIP, 600f, true, true, false, null, -1f, 0f, null, "");
		effect7.Add(new AttributeModifier(Db.Get().Amounts.Maturity.deltaAttribute.Id, 0.5f, global::STRINGS.CREATURES.MODIFIERS.DIVERGENTPLANTTENDEDWORM.NAME, true, false, true));
		effect7.Add(new AttributeModifier(Db.Get().Amounts.Maturity2.deltaAttribute.Id, 0.5f, global::STRINGS.CREATURES.MODIFIERS.DIVERGENTPLANTTENDEDWORM.NAME, true, false, true));
		this.effects.Add(effect7);
		Effect effect8 = new Effect("MooWellFed", global::STRINGS.CREATURES.MODIFIERS.MOOWELLFED.NAME, global::STRINGS.CREATURES.MODIFIERS.MOOWELLFED.TOOLTIP, 1f, true, true, false, null, -1f, 0f, null, "");
		effect8.Add(new AttributeModifier(Db.Get().Amounts.Beckoning.deltaAttribute.Id, MooTuning.WELLFED_EFFECT, global::STRINGS.CREATURES.MODIFIERS.MOOWELLFED.NAME, false, false, true));
		effect8.Add(new AttributeModifier(Db.Get().Amounts.MilkProduction.deltaAttribute.Id, MooTuning.MILK_PRODUCTION_PERCENTAGE_PER_SECOND, global::STRINGS.CREATURES.MODIFIERS.MOOWELLFED.NAME, false, false, true));
		this.effects.Add(effect8);
		Effect effect9 = new Effect("HuskyMooFed", global::STRINGS.CREATURES.MODIFIERS.HUSKYMOOFED.NAME, global::STRINGS.CREATURES.MODIFIERS.HUSKYMOOFED.TOOLTIP, 1f, true, true, false, null, -1f, 0f, null, "");
		effect9.Add(new AttributeModifier(Db.Get().Amounts.Beckoning.deltaAttribute.Id, MooTuning.WELLFED_EFFECT, global::STRINGS.CREATURES.MODIFIERS.HUSKYMOOFED.NAME, false, false, true));
		this.effects.Add(effect9);
		Effect effect10 = new Effect("HuskyMooWellFed", global::STRINGS.CREATURES.MODIFIERS.HUSKYMOOWELLFED.NAME, global::STRINGS.CREATURES.MODIFIERS.HUSKYMOOWELLFED.TOOLTIP, 1f, true, true, false, null, -1f, 0f, null, "");
		effect10.Add(new AttributeModifier(Db.Get().Amounts.MilkProduction.deltaAttribute.Id, MooTuning.MILK_PRODUCTION_PERCENTAGE_PER_SECOND, () => GameUtil.SafeStringFormat(global::STRINGS.CREATURES.STATS.MILKPRODUCTION.DISPLAYED_NAME, new object[] { UI.StripLinkFormatting(ElementLoader.FindElementByHash(DieselMooConfig.MILK_ELEMENT).name) }), () => global::STRINGS.CREATURES.MODIFIERS.HUSKYMOOWELLFED.NAME, false, false));
		this.effects.Add(effect10);
		Effect effect11 = new Effect("WoodDeerWellFed", global::STRINGS.CREATURES.MODIFIERS.DEERWELLFED.NAME, global::STRINGS.CREATURES.MODIFIERS.DEERWELLFED.TOOLTIP, 1f, true, true, false, null, -1f, 0f, null, "");
		effect11.Add(new AttributeModifier(Db.Get().Amounts.ScaleGrowth.deltaAttribute.Id, 100f / (WoodDeerConfig.ANTLER_GROWTH_TIME_IN_CYCLES * 600f), () => global::STRINGS.CREATURES.STATS.SCALEGROWTH.DISPLAYED_NAME["WoodDeer"], () => global::STRINGS.CREATURES.MODIFIERS.DEERWELLFED.NAME, false, false));
		this.effects.Add(effect11);
		Effect effect12 = new Effect("GlassDeerWellFed", global::STRINGS.CREATURES.MODIFIERS.DEERWELLFED.NAME, global::STRINGS.CREATURES.MODIFIERS.DEERWELLFED.TOOLTIP, 1f, true, true, false, null, -1f, 0f, null, "");
		effect12.Add(new AttributeModifier(Db.Get().Amounts.ScaleGrowth.deltaAttribute.Id, 0.027777778f, () => global::STRINGS.CREATURES.STATS.SCALEGROWTH.DISPLAYED_NAME["GlassDeer"], () => global::STRINGS.CREATURES.MODIFIERS.DEERWELLFED.NAME, false, false));
		this.effects.Add(effect12);
		Effect effect13 = new Effect("IceBellyWellFed", global::STRINGS.CREATURES.MODIFIERS.ICEBELLYWELLFED.NAME, global::STRINGS.CREATURES.MODIFIERS.ICEBELLYWELLFED.TOOLTIP, 1f, true, true, false, null, -1f, 0f, null, "");
		effect13.Add(new AttributeModifier(Db.Get().Amounts.ScaleGrowth.deltaAttribute.Id, 100f / (IceBellyConfig.SCALE_GROWTH_TIME_IN_CYCLES * 600f), () => global::STRINGS.CREATURES.STATS.SCALEGROWTH.DISPLAYED_NAME["IceBelly"], () => global::STRINGS.CREATURES.MODIFIERS.ICEBELLYWELLFED.NAME, false, false));
		this.effects.Add(effect13);
		Effect effect14 = new Effect("GoldBellyWellFed", global::STRINGS.CREATURES.MODIFIERS.GOLDBELLYWELLFED.NAME, global::STRINGS.CREATURES.MODIFIERS.GOLDBELLYWELLFED.TOOLTIP, 1f, true, true, false, null, -1f, 0f, null, "");
		effect14.Add(new AttributeModifier(Db.Get().Amounts.ScaleGrowth.deltaAttribute.Id, 0.016666668f, () => global::STRINGS.CREATURES.STATS.SCALEGROWTH.DISPLAYED_NAME["GoldBelly"], () => global::STRINGS.CREATURES.MODIFIERS.GOLDBELLYWELLFED.NAME, false, false));
		this.effects.Add(effect14);
		Effect effect15 = new Effect("ButterflyPollinated", global::STRINGS.CREATURES.MODIFIERS.BUTTERFLYPOLLINATED.NAME, global::STRINGS.CREATURES.MODIFIERS.BUTTERFLYPOLLINATED.TOOLTIP, 600f, true, true, false, null, -1f, 0f, null, "");
		effect15.Add(new AttributeModifier(Db.Get().Amounts.Maturity.deltaAttribute.Id, 0.25f, global::STRINGS.CREATURES.MODIFIERS.BUTTERFLYPOLLINATED.NAME, true, false, true));
		effect15.Add(new AttributeModifier(Db.Get().Amounts.Maturity2.deltaAttribute.Id, 0.25f, global::STRINGS.CREATURES.MODIFIERS.BUTTERFLYPOLLINATED.NAME, true, false, true));
		this.effects.Add(effect15);
		Effect effect16 = new Effect(PollinationMonitor.INITIALLY_POLLINATED_EFFECT, global::STRINGS.CREATURES.MODIFIERS.INITIALLYPOLLINATED.NAME, global::STRINGS.CREATURES.MODIFIERS.INITIALLYPOLLINATED.TOOLTIP, 600f, false, false, false, null, -1f, 0f, null, "");
		this.effects.Add(effect16);
		Effect effect17 = new Effect("RaptorWellFed", global::STRINGS.CREATURES.MODIFIERS.RAPTORWELLFED.NAME, global::STRINGS.CREATURES.MODIFIERS.RAPTORWELLFED.TOOLTIP, 1f, true, true, false, null, -1f, 0f, null, "");
		effect17.Add(new AttributeModifier(Db.Get().Amounts.ScaleGrowth.deltaAttribute.Id, 100f / (RaptorConfig.SCALE_GROWTH_TIME_IN_CYCLES * 600f), () => global::STRINGS.CREATURES.STATS.SCALEGROWTH.DISPLAYED_NAME["Raptor"], () => global::STRINGS.CREATURES.MODIFIERS.RAPTORWELLFED.NAME, false, false));
		this.effects.Add(effect17);
		Effect effect18 = new Effect("PredatorFailedHunt", global::STRINGS.CREATURES.MODIFIERS.HUNT_FAILED.NAME, global::STRINGS.CREATURES.MODIFIERS.HUNT_FAILED.TOOLTIP, 45f, true, false, true, null, -1f, 0f, null, "");
		effect18.tag = new Tag?(GameTags.Creatures.SuppressedDiet);
		this.effects.Add(effect18);
		Effect effect19 = new Effect("PreyEvadedHunt", global::STRINGS.CREATURES.MODIFIERS.EVADED_HUNT.NAME, global::STRINGS.CREATURES.MODIFIERS.EVADED_HUNT.TOOLTIP, 10f, true, false, false, null, -1f, 0f, null, "");
		this.effects.Add(effect19);
		this.CreateMosquitoEffects();
	}

	public Trait CreateTrait(string id, string name, string description, string group_name, bool should_save, ChoreGroup[] disabled_chore_groups, bool positive_trait, bool is_valid_starter_trait)
	{
		return this.CreateTrait(id, name, description, group_name, should_save, disabled_chore_groups, positive_trait, is_valid_starter_trait, null, null);
	}

	public Trait CreateTrait(string id, string name, string description, string group_name, bool should_save, ChoreGroup[] disabled_chore_groups, bool positive_trait, bool is_valid_starter_trait, string[] requiredDlcIds, string[] forbiddenDlcIds)
	{
		Trait trait = new Trait(id, name, description, 0f, should_save, disabled_chore_groups, positive_trait, is_valid_starter_trait, requiredDlcIds, forbiddenDlcIds);
		this.traits.Add(trait);
		if (group_name == "" || group_name == null)
		{
			group_name = "Default";
		}
		TraitGroup traitGroup = this.traitGroups.TryGet(group_name);
		if (traitGroup == null)
		{
			traitGroup = new TraitGroup(group_name, group_name, group_name != "Default");
			this.traitGroups.Add(traitGroup);
		}
		traitGroup.Add(trait);
		return trait;
	}

	public FertilityModifier CreateFertilityModifier(string id, Tag targetTag, string name, string description, Func<string, string> tooltipCB, FertilityModifier.FertilityModFn applyFunction)
	{
		FertilityModifier fertilityModifier = new FertilityModifier(id, targetTag, name, description, tooltipCB, applyFunction);
		this.FertilityModifiers.Add(fertilityModifier);
		return fertilityModifier;
	}

	public MooSongModifier CreateMooSongModifier(string id, Tag targetTag, string name, string description, Func<string, string> tooltipCB, MooSongModifier.MooSongModFn applyFunction)
	{
		MooSongModifier mooSongModifier = new MooSongModifier(id, targetTag, name, description, tooltipCB, applyFunction);
		this.MooSongModifiers.Add(mooSongModifier);
		return mooSongModifier;
	}

	protected void LoadTraits()
	{
		TRAITS.TRAIT_CREATORS.ForEach(delegate(global::System.Action action)
		{
			action();
		});
	}

	protected void LoadFertilityModifiers()
	{
		global::TUNING.CREATURES.EGG_CHANCE_MODIFIERS.MODIFIER_CREATORS.ForEach(delegate(global::System.Action action)
		{
			action();
		});
	}

	protected void LoadMooSongsModifiers()
	{
		global::TUNING.CREATURES.MOO_SONG_MODIFIERS.MODIFIER_CREATORS.ForEach(delegate(global::System.Action action)
		{
			action();
		});
	}

	public TextAsset modifiersFile;

	public ModifierSet.ModifierInfos modifierInfos;

	public ModifierSet.TraitSet traits;

	public ResourceSet<Effect> effects;

	public ModifierSet.TraitGroupSet traitGroups;

	public FertilityModifiers FertilityModifiers;

	public MooSongModifiers MooSongModifiers;

	public global::Database.Attributes Attributes;

	public BuildingAttributes BuildingAttributes;

	public CritterAttributes CritterAttributes;

	public PlantAttributes PlantAttributes;

	public global::Database.Amounts Amounts;

	public global::Database.AttributeConverters AttributeConverters;

	public ResourceSet Root;

	public List<Resource> ResourceTable;

	public class ModifierInfo : Resource
	{
		public string Type;

		public string Attribute;

		public float Value;

		public Units Units;

		public bool Multiplier;

		public float Duration;

		public bool ShowInUI;

		public string StompGroup;

		public int StompPriority;

		public bool IsBad;

		public string CustomIcon;

		public bool TriggerFloatingText;

		public string EmoteAnim;

		public float EmoteCooldown;
	}

	[Serializable]
	public class ModifierInfos : ResourceLoader<ModifierSet.ModifierInfo>
	{
	}

	[Serializable]
	public class TraitSet : ResourceSet<Trait>
	{
	}

	[Serializable]
	public class TraitGroupSet : ResourceSet<TraitGroup>
	{
	}
}
