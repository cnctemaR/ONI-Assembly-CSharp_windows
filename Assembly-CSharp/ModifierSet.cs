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
		this.effects = new ResourceSet<Effect>("Effects", this.Root);
		this.traits = new ModifierSet.TraitSet();
		this.traitGroups = new ModifierSet.TraitGroupSet();
		this.FertilityModifiers = new FertilityModifiers();
		this.Amounts = new global::Database.Amounts();
		this.Amounts.Load();
		this.AttributeConverters = new global::Database.AttributeConverters();
		this.LoadEffects();
		this.LoadFertilityModifiers();
	}

	public static float ConvertValue(float value, Units units)
	{
		if (units == Units.PerDay)
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
				Effect effect = new Effect(modifierInfo.Id, text, text2, modifierInfo.Duration * 600f, modifierInfo.ShowInUI && modifierInfo.Type != "Need", modifierInfo.TriggerFloatingText, modifierInfo.IsBad, modifierInfo.EmoteAnim, modifierInfo.EmoteCooldown, modifierInfo.StompGroup);
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
		Effect effect2 = new Effect("Ranched", global::STRINGS.CREATURES.MODIFIERS.RANCHED.NAME, global::STRINGS.CREATURES.MODIFIERS.RANCHED.TOOLTIP, 600f, true, true, false, null, 0f, null);
		effect2.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, 5f, global::STRINGS.CREATURES.MODIFIERS.RANCHED.NAME, false, false, true));
		effect2.Add(new AttributeModifier(Db.Get().Amounts.Wildness.deltaAttribute.Id, -0.09166667f, global::STRINGS.CREATURES.MODIFIERS.RANCHED.NAME, false, false, true));
		this.effects.Add(effect2);
		Effect effect3 = new Effect("EggSong", global::STRINGS.CREATURES.MODIFIERS.INCUBATOR_SONG.NAME, global::STRINGS.CREATURES.MODIFIERS.INCUBATOR_SONG.TOOLTIP, 600f, true, false, false, null, 0f, null);
		effect3.Add(new AttributeModifier(Db.Get().Amounts.Incubation.deltaAttribute.Id, 4f, global::STRINGS.CREATURES.MODIFIERS.INCUBATOR_SONG.NAME, true, false, true));
		this.effects.Add(effect3);
		Reactable.ReactablePrecondition reactablePrecondition = delegate(GameObject go, Navigator.ActiveTransition n)
		{
			int num = Grid.PosToCell(go);
			return Grid.IsValidCell(num) && Grid.IsGas(num);
		};
		Effect effect4 = this.effects.Get("WetFeet");
		effect4.AddEmotePrecondition(reactablePrecondition);
		Effect effect5 = this.effects.Get("SoakingWet");
		effect5.AddEmotePrecondition(reactablePrecondition);
	}

	public Trait CreateTrait(string id, string name, string description, string group_name, bool should_save, ChoreGroup[] disabled_chore_groups, bool positive_trait, bool is_valid_starter_trait)
	{
		Trait trait = new Trait(id, name, description, 0f, should_save, disabled_chore_groups, positive_trait, is_valid_starter_trait);
		this.traits.Add(trait);
		if (group_name == string.Empty || group_name == null)
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

	public TextAsset modifiersFile;

	public ModifierSet.ModifierInfos modifierInfos;

	public ModifierSet.TraitSet traits;

	public ResourceSet<Effect> effects;

	public ModifierSet.TraitGroupSet traitGroups;

	public FertilityModifiers FertilityModifiers;

	public global::Database.Attributes Attributes;

	public BuildingAttributes BuildingAttributes;

	public CritterAttributes CritterAttributes;

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

		public float Tier;

		public string Notes;

		public string StompGroup;

		public bool IsBad;

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
