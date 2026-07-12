using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class GravitasCreatureManipulatorConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "GravitasCreatureManipulator";
		int num = 3;
		int num2 = 4;
		string text2 = "gravitas_critter_manipulator_kanim";
		int num3 = 250;
		float num4 = 120f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num5 = 3200f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER5;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, refined_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.BONUS.TIER2, tier2, 0.2f);
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		buildingDef.SelfHeatKilowattsWhenActive = 0f;
		buildingDef.Floodable = false;
		buildingDef.Entombable = true;
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "medium";
		buildingDef.ForegroundLayer = Grid.SceneLayer.Ground;
		buildingDef.ShowInBuildMenu = false;
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
		PrimaryElement component = go.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Steel, true);
		component.Temperature = 294.15f;
		BuildingTemplates.ExtendBuildingToGravitas(go);
		go.AddComponent<Storage>();
		Activatable activatable = go.AddComponent<Activatable>();
		activatable.synchronizeAnims = false;
		activatable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_use_remote_kanim") };
		activatable.SetWorkTime(30f);
		GravitasCreatureManipulator.Def def = go.AddOrGetDef<GravitasCreatureManipulator.Def>();
		def.pickupOffset = new CellOffset(-1, 0);
		def.dropOffset = new CellOffset(1, 0);
		def.numSpeciesToUnlockMorphMode = 5;
		def.workingDuration = 15f;
		def.cooldownDuration = 540f;
		MakeBaseSolid.Def def2 = go.AddOrGetDef<MakeBaseSolid.Def>();
		def2.solidOffsets = new CellOffset[4];
		for (int i = 0; i < 4; i++)
		{
			def2.solidOffsets[i] = new CellOffset(0, i);
		}
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			game_object.GetComponent<Activatable>().SetOffsets(OffsetGroups.LeftOrRight);
		};
	}

	public static Option<string> GetBodyContentForSpeciesTag(Tag species)
	{
		Option<string> nameForSpeciesTag = GravitasCreatureManipulatorConfig.GetNameForSpeciesTag(species);
		Option<string> descriptionForSpeciesTag = GravitasCreatureManipulatorConfig.GetDescriptionForSpeciesTag(species);
		if (nameForSpeciesTag.HasValue && descriptionForSpeciesTag.HasValue)
		{
			return GravitasCreatureManipulatorConfig.GetBodyContent(nameForSpeciesTag.Value, descriptionForSpeciesTag.Value);
		}
		return Option.None;
	}

	public static string GetBodyContentForUnknownSpecies()
	{
		return GravitasCreatureManipulatorConfig.GetBodyContent(CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.UNKNOWN_TITLE, CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.UNKNOWN);
	}

	public static string GetBodyContent(string name, string desc)
	{
		return "<size=125%><b>" + name + "</b></size><line-height=150%>\n</line-height>" + desc;
	}

	public static Option<string> GetNameForSpeciesTag(Tag species)
	{
		if (species == GameTags.Creatures.Species.HatchSpecies)
		{
			return Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.HATCHSPECIES);
		}
		if (species == GameTags.Creatures.Species.LightBugSpecies)
		{
			return Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.LIGHTBUGSPECIES);
		}
		if (species == GameTags.Creatures.Species.OilFloaterSpecies)
		{
			return Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.OILFLOATERSPECIES);
		}
		if (species == GameTags.Creatures.Species.DreckoSpecies)
		{
			return Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.DRECKOSPECIES);
		}
		if (species == GameTags.Creatures.Species.GlomSpecies)
		{
			return Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.GLOMSPECIES);
		}
		if (species == GameTags.Creatures.Species.PuftSpecies)
		{
			return Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.PUFTSPECIES);
		}
		if (species == GameTags.Creatures.Species.PacuSpecies)
		{
			return Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.PACUSPECIES);
		}
		if (species == GameTags.Creatures.Species.MooSpecies)
		{
			return Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.MOOSPECIES);
		}
		if (species == GameTags.Creatures.Species.MoleSpecies)
		{
			return Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.MOLESPECIES);
		}
		if (species == GameTags.Creatures.Species.SquirrelSpecies)
		{
			return Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.SQUIRRELSPECIES);
		}
		if (species == GameTags.Creatures.Species.CrabSpecies)
		{
			return Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.CRABSPECIES);
		}
		if (species == GameTags.Creatures.Species.DivergentSpecies)
		{
			return Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.DIVERGENTSPECIES);
		}
		if (species == GameTags.Creatures.Species.StaterpillarSpecies)
		{
			return Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.STATERPILLARSPECIES);
		}
		if (species == GameTags.Creatures.Species.BeetaSpecies)
		{
			return Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.BEETASPECIES);
		}
		if (species == GameTags.Creatures.Species.BellySpecies)
		{
			return Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.BELLYSPECIES);
		}
		if (species == GameTags.Creatures.Species.SealSpecies)
		{
			return Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.SEALSPECIES);
		}
		if (species == GameTags.Creatures.Species.DeerSpecies)
		{
			return Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.DEERSPECIES);
		}
		return Option.None;
	}

	public static Option<string> GetDescriptionForSpeciesTag(Tag species)
	{
		if (species == GameTags.Creatures.Species.HatchSpecies)
		{
			return Option.Some<string>(CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.HATCH);
		}
		if (species == GameTags.Creatures.Species.LightBugSpecies)
		{
			return Option.Some<string>(CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.LIGHTBUG);
		}
		if (species == GameTags.Creatures.Species.OilFloaterSpecies)
		{
			return Option.Some<string>(CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.OILFLOATER);
		}
		if (species == GameTags.Creatures.Species.DreckoSpecies)
		{
			return Option.Some<string>(CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.DRECKO);
		}
		if (species == GameTags.Creatures.Species.GlomSpecies)
		{
			return Option.Some<string>(CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.GLOM);
		}
		if (species == GameTags.Creatures.Species.PuftSpecies)
		{
			return Option.Some<string>(CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.PUFT);
		}
		if (species == GameTags.Creatures.Species.PacuSpecies)
		{
			return Option.Some<string>(CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.PACU);
		}
		if (species == GameTags.Creatures.Species.MooSpecies)
		{
			return Option.Some<string>(CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.MOO);
		}
		if (species == GameTags.Creatures.Species.MoleSpecies)
		{
			return Option.Some<string>(CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.MOLE);
		}
		if (species == GameTags.Creatures.Species.SquirrelSpecies)
		{
			return Option.Some<string>(CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.SQUIRREL);
		}
		if (species == GameTags.Creatures.Species.CrabSpecies)
		{
			return Option.Some<string>(CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.CRAB);
		}
		if (species == GameTags.Creatures.Species.DivergentSpecies)
		{
			return Option.Some<string>(CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.DIVERGENTSPECIES);
		}
		if (species == GameTags.Creatures.Species.StaterpillarSpecies)
		{
			return Option.Some<string>(CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.STATERPILLAR);
		}
		if (species == GameTags.Creatures.Species.BeetaSpecies)
		{
			return Option.Some<string>(CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.BEETA);
		}
		if (species == GameTags.Creatures.Species.BellySpecies)
		{
			return Option.Some<string>(CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.ICEBELLY);
		}
		if (species == GameTags.Creatures.Species.SealSpecies)
		{
			return Option.Some<string>(CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.SEAL);
		}
		if (species == GameTags.Creatures.Species.DeerSpecies)
		{
			return Option.Some<string>(CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.WOODDEER);
		}
		return Option.None;
	}

	public const string ID = "GravitasCreatureManipulator";

	public const string CODEX_ENTRY_ID = "STORYTRAITCRITTERMANIPULATOR";

	public const string INITIAL_LORE_UNLOCK_ID = "story_trait_critter_manipulator_initial";

	public const string PARKING_LORE_UNLOCK_ID = "story_trait_critter_manipulator_parking";

	public const string COMPLETED_LORE_UNLOCK_ID = "story_trait_critter_manipulator_complete";

	private const int HEIGHT = 4;

	public static class CRITTER_LORE_UNLOCK_ID
	{
		public static string For(Tag species)
		{
			return "story_trait_critter_manipulator_" + species.ToString().ToLower();
		}
	}
}
