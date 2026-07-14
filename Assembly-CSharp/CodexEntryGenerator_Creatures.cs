using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class CodexEntryGenerator_Creatures
{
	public static Dictionary<string, CodexEntry> GenerateEntries()
	{
		CodexEntryGenerator_Creatures.<>c__DisplayClass6_0 CS$<>8__locals1;
		CS$<>8__locals1.results = new Dictionary<string, CodexEntry>();
		CS$<>8__locals1.brains = Assets.GetPrefabsWithComponent<CreatureBrain>();
		CS$<>8__locals1.critterEntries = new List<ValueTuple<string, CodexEntry>>();
		CodexEntryGenerator_Creatures.<GenerateEntries>g__AddEntry|6_0("CREATURES::GUIDE", CodexEntryGenerator_Creatures.GenerateFieldGuideEntry(), "CREATURES", ref CS$<>8__locals1);
		Tag[] array = GameTags.Creatures.Species.AllSpecies_REFLECTION();
		for (int i = 0; i < array.Length; i++)
		{
			CodexEntryGenerator_Creatures.<GenerateEntries>g__PushCritterEntry|6_1(array[i], ref CS$<>8__locals1);
		}
		CodexEntryGenerator_Creatures.<GenerateEntries>g__PopAndAddAllCritterEntries|6_2(ref CS$<>8__locals1);
		return CS$<>8__locals1.results;
	}

	private static CodexEntry GenerateFieldGuideEntry()
	{
		CodexEntryGenerator_Creatures.<>c__DisplayClass7_0 CS$<>8__locals1;
		CS$<>8__locals1.generalInfoEntry = new CodexEntry("CREATURES", new List<ContentContainer>(), CODEX.CRITTERSTATUS.CRITTERSTATUS_TITLE);
		CS$<>8__locals1.generalInfoEntry.icon = Assets.GetSprite("codex_critter_emotions");
		CodexEntryGenerator_Creatures.<>c__DisplayClass7_1 CS$<>8__locals2;
		CS$<>8__locals2.subEntryContents = null;
		CS$<>8__locals2.subEntry = null;
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddSubEntry|7_0("CREATURES::GUIDE::METABOLISM", CODEX.CRITTERSTATUS.METABOLISM.TITLE, ref CS$<>8__locals1, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddImage|7_1(Assets.GetSprite("codex_metabolism"), ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBody|7_3(CODEX.CRITTERSTATUS.METABOLISM.BODY.CONTAINER1, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddSubtitle|7_2(CODEX.CRITTERSTATUS.METABOLISM.HUNGRY.TITLE, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBody|7_3(CODEX.CRITTERSTATUS.METABOLISM.HUNGRY.CONTAINER1, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddSubtitle|7_2(CODEX.CRITTERSTATUS.METABOLISM.STARVING.TITLE, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBody|7_3(string.Format(DlcManager.IsExpansion1Active() ? CODEX.CRITTERSTATUS.METABOLISM.STARVING.CONTAINER1_DLC1 : CODEX.CRITTERSTATUS.METABOLISM.STARVING.CONTAINER1_VANILLA, 10), ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddSpacer|7_4(ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddSubEntry|7_0("CREATURES::GUIDE::MOOD", CODEX.CRITTERSTATUS.MOOD.TITLE, ref CS$<>8__locals1, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddImage|7_1(Assets.GetSprite("codex_mood"), ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBody|7_3(CODEX.CRITTERSTATUS.MOOD.BODY.CONTAINER1, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddSubtitle|7_2(CODEX.CRITTERSTATUS.MOOD.HAPPY.TITLE, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBody|7_3(CODEX.CRITTERSTATUS.MOOD.HAPPY.CONTAINER1, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBody|7_3(CODEX.CRITTERSTATUS.MOOD.HAPPY.SUBTITLE, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBulletPoint|7_5(CODEX.CRITTERSTATUS.MOOD.HAPPY.HAPPY_METABOLISM, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddSubtitle|7_2(CODEX.CRITTERSTATUS.MOOD.NEUTRAL.TITLE, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBody|7_3(CODEX.CRITTERSTATUS.MOOD.NEUTRAL.CONTAINER1, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddSubtitle|7_2(CODEX.CRITTERSTATUS.MOOD.GLUM.TITLE, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBody|7_3(CODEX.CRITTERSTATUS.MOOD.GLUM.CONTAINER1, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBody|7_3(CODEX.CRITTERSTATUS.MOOD.GLUM.SUBTITLE, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBulletPoint|7_5(CODEX.CRITTERSTATUS.MOOD.GLUM.GLUMWILD_METABOLISM, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddSubtitle|7_2(CODEX.CRITTERSTATUS.MOOD.MISERABLE.TITLE, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBody|7_3(CODEX.CRITTERSTATUS.MOOD.MISERABLE.CONTAINER1, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBody|7_3(CODEX.CRITTERSTATUS.MOOD.MISERABLE.SUBTITLE, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBulletPoint|7_5(CODEX.CRITTERSTATUS.MOOD.MISERABLE.MISERABLEWILD_METABOLISM, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBulletPoint|7_5(CODEX.CRITTERSTATUS.MOOD.MISERABLE.MISERABLEWILD_FERTILITY, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddSubtitle|7_2(CODEX.CRITTERSTATUS.MOOD.HOSTILE.TITLE, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBody|7_3(DlcManager.IsExpansion1Active() ? CODEX.CRITTERSTATUS.MOOD.HOSTILE.CONTAINER1_DLC1 : CODEX.CRITTERSTATUS.MOOD.HOSTILE.CONTAINER1_VANILLA, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddSubtitle|7_2(CODEX.CRITTERSTATUS.MOOD.CONFINED.TITLE, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBody|7_3(CODEX.CRITTERSTATUS.MOOD.CONFINED.CONTAINER1, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBody|7_3(CODEX.CRITTERSTATUS.MOOD.CONFINED.SUBTITLE, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBulletPoint|7_5(CODEX.CRITTERSTATUS.MOOD.CONFINED.CONFINED_FERTILITY, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBulletPoint|7_5(CODEX.CRITTERSTATUS.MOOD.CONFINED.CONFINED_HAPPINESS, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddSubtitle|7_2(CODEX.CRITTERSTATUS.MOOD.OVERCROWDED.TITLE, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBody|7_3(CODEX.CRITTERSTATUS.MOOD.OVERCROWDED.CONTAINER1, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBody|7_3(CODEX.CRITTERSTATUS.MOOD.OVERCROWDED.SUBTITLE, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBulletPoint|7_5(CODEX.CRITTERSTATUS.MOOD.OVERCROWDED.OVERCROWDED_HAPPY1, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddSpacer|7_4(ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddSubEntry|7_0("CREATURES::GUIDE::FERTILITY", CODEX.CRITTERSTATUS.FERTILITY.TITLE, ref CS$<>8__locals1, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddImage|7_1(Assets.GetSprite("codex_reproduction"), ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBody|7_3(CODEX.CRITTERSTATUS.FERTILITY.BODY.CONTAINER1, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddSubtitle|7_2(CODEX.CRITTERSTATUS.FERTILITY.FERTILITYRATE.TITLE, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBody|7_3(CODEX.CRITTERSTATUS.FERTILITY.FERTILITYRATE.CONTAINER1, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddSubtitle|7_2(CODEX.CRITTERSTATUS.FERTILITY.EGGCHANCES.TITLE, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBody|7_3(CODEX.CRITTERSTATUS.FERTILITY.EGGCHANCES.CONTAINER1, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddSubtitle|7_2(CODEX.CRITTERSTATUS.FERTILITY.FUTURE_OVERCROWDED.TITLE, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBody|7_3(CODEX.CRITTERSTATUS.FERTILITY.FUTURE_OVERCROWDED.CONTAINER1, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBody|7_3(CODEX.CRITTERSTATUS.FERTILITY.FUTURE_OVERCROWDED.SUBTITLE, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBulletPoint|7_5(CODEX.CRITTERSTATUS.FERTILITY.FUTURE_OVERCROWDED.CRAMPED_FERTILITY, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddSubtitle|7_2(CODEX.CRITTERSTATUS.FERTILITY.INCUBATION.TITLE, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBody|7_3(CODEX.CRITTERSTATUS.FERTILITY.INCUBATION.CONTAINER1, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddSubtitle|7_2(CODEX.CRITTERSTATUS.FERTILITY.MAXAGE.TITLE, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBody|7_3(DlcManager.IsExpansion1Active() ? CODEX.CRITTERSTATUS.FERTILITY.MAXAGE.CONTAINER1_DLC1 : CODEX.CRITTERSTATUS.FERTILITY.MAXAGE.CONTAINER1_VANILLA, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddSpacer|7_4(ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddSubEntry|7_0("CREATURES::GUIDE::DOMESTICATION", CODEX.CRITTERSTATUS.DOMESTICATION.TITLE, ref CS$<>8__locals1, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddImage|7_1(Assets.GetSprite("codex_domestication"), ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBody|7_3(CODEX.CRITTERSTATUS.DOMESTICATION.BODY.CONTAINER1, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddSubtitle|7_2(CODEX.CRITTERSTATUS.DOMESTICATION.WILD.TITLE, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBody|7_3(CODEX.CRITTERSTATUS.DOMESTICATION.WILD.CONTAINER1, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBody|7_3(CODEX.CRITTERSTATUS.DOMESTICATION.WILD.SUBTITLE, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBulletPoint|7_5(CODEX.CRITTERSTATUS.DOMESTICATION.WILD.WILD_METABOLISM, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBulletPoint|7_5(CODEX.CRITTERSTATUS.DOMESTICATION.WILD.WILD_POOP, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddSubtitle|7_2(CODEX.CRITTERSTATUS.DOMESTICATION.TAME.TITLE, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBody|7_3(CODEX.CRITTERSTATUS.DOMESTICATION.TAME.CONTAINER1, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBody|7_3(CODEX.CRITTERSTATUS.DOMESTICATION.TAME.SUBTITLE, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBulletPoint|7_5(CODEX.CRITTERSTATUS.DOMESTICATION.TAME.TAME_HAPPINESS, ref CS$<>8__locals2);
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddBulletPoint|7_5(CODEX.CRITTERSTATUS.DOMESTICATION.TAME.TAME_METABOLISM, ref CS$<>8__locals2);
		return CS$<>8__locals1.generalInfoEntry;
	}

	private static CodexEntry GenerateCritterEntry(Tag speciesTag, string name, List<GameObject> brains)
	{
		CodexEntry codexEntry = null;
		List<ContentContainer> list = new List<ContentContainer>();
		foreach (GameObject gameObject in brains)
		{
			if (gameObject.GetDef<BabyMonitor.Def>() == null && Game.IsCorrectDlcActiveForCurrentSave(gameObject.GetComponent<KPrefabID>()))
			{
				Sprite sprite = null;
				CreatureBrain component = gameObject.GetComponent<CreatureBrain>();
				if (!(component.species != speciesTag))
				{
					if (codexEntry == null)
					{
						codexEntry = new CodexEntry("CREATURES", list, name);
						codexEntry.sortString = name;
						list.Add(new ContentContainer(new List<ICodexWidget>
						{
							new CodexSpacer(),
							new CodexSpacer()
						}, ContentContainer.ContentLayout.Vertical));
					}
					List<ContentContainer> list2 = new List<ContentContainer>();
					string symbolPrefix = component.symbolPrefix;
					Sprite first = Def.GetUISprite(gameObject, symbolPrefix + "ui", false).first;
					GameObject gameObject2 = Assets.TryGetPrefab(gameObject.PrefabID().ToString() + "Baby");
					if (gameObject2 != null)
					{
						sprite = Def.GetUISprite(gameObject2, "ui", false).first;
					}
					if (sprite)
					{
						CodexEntryGenerator.GenerateImageContainers(new Sprite[] { first, sprite }, list2, ContentContainer.ContentLayout.Horizontal);
					}
					else
					{
						CodexEntryGenerator.GenerateImageContainers(first, list2);
					}
					CodexEntryGenerator_Creatures.GenerateCreatureDescriptionContainers(gameObject, list2);
					SubEntry subEntry = new SubEntry(component.PrefabID().ToString(), speciesTag.ToString(), list2, component.GetProperName());
					subEntry.icon = first;
					subEntry.iconColor = Color.white;
					codexEntry.subEntries.Add(subEntry);
				}
			}
		}
		return codexEntry;
	}

	private static void GenerateCreatureDescriptionContainers(GameObject creature, List<ContentContainer> containers)
	{
		containers.Add(new ContentContainer(new List<ICodexWidget>
		{
			new CodexText(creature.GetComponent<InfoDescription>().description, CodexTextStyle.Body, null)
		}, ContentContainer.ContentLayout.Vertical));
		RobotBatteryMonitor.Def def = creature.GetDef<RobotBatteryMonitor.Def>();
		if (def != null)
		{
			Amount batteryAmount = Db.Get().Amounts.Get(def.batteryAmountId);
			float value = Db.Get().traits.Get(creature.GetComponent<Modifiers>().initialTraits[0]).SelfModifiers.Find((AttributeModifier match) => match.AttributeId == batteryAmount.maxAttribute.Id).Value;
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexText(CODEX.HEADERS.INTERNALBATTERY, CodexTextStyle.Subtitle, null),
				new CodexText("    • " + string.Format(CODEX.ROBOT_DESCRIPTORS.BATTERY.CAPACITY, value), CodexTextStyle.Body, null)
			}, ContentContainer.ContentLayout.Vertical));
		}
		if (creature.GetDef<StorageUnloadMonitor.Def>() != null)
		{
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexText(CODEX.HEADERS.INTERNALSTORAGE, CodexTextStyle.Subtitle, null),
				new CodexText("    • " + string.Format(CODEX.ROBOT_DESCRIPTORS.STORAGE.CAPACITY, creature.GetComponents<Storage>()[1].Capacity()), CodexTextStyle.Body, null)
			}, ContentContainer.ContentLayout.Vertical));
		}
		List<GameObject> prefabsWithTag = Assets.GetPrefabsWithTag((creature.PrefabID().ToString() + "Egg").ToTag());
		if (prefabsWithTag != null && prefabsWithTag.Count > 0)
		{
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexText(CODEX.HEADERS.HATCHESFROMEGG, CodexTextStyle.Subtitle, null)
			}, ContentContainer.ContentLayout.Vertical));
			foreach (GameObject gameObject in prefabsWithTag)
			{
				containers.Add(new ContentContainer(new List<ICodexWidget>
				{
					new CodexIndentedLabelWithIcon(gameObject.GetProperName(), CodexTextStyle.Body, Def.GetUISprite(gameObject, "ui", false))
				}, ContentContainer.ContentLayout.Horizontal));
			}
		}
		CritterTemperatureMonitor.Def def2 = creature.GetDef<CritterTemperatureMonitor.Def>();
		if (def2 != null)
		{
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexText(CODEX.HEADERS.COMFORTRANGE, CodexTextStyle.Subtitle, null),
				new CodexText("    • " + string.Format(CODEX.CREATURE_DESCRIPTORS.TEMPERATURE.COMFORT_RANGE, GameUtil.GetFormattedTemperature(def2.temperatureColdUncomfortable, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false), GameUtil.GetFormattedTemperature(def2.temperatureHotUncomfortable, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), CodexTextStyle.Body, null),
				new CodexText("    • " + string.Format(CODEX.CREATURE_DESCRIPTORS.TEMPERATURE.NON_LETHAL_RANGE, GameUtil.GetFormattedTemperature(def2.temperatureColdDeadly, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false), GameUtil.GetFormattedTemperature(def2.temperatureHotDeadly, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), CodexTextStyle.Body, null)
			}, ContentContainer.ContentLayout.Vertical));
		}
		Modifiers component = creature.GetComponent<Modifiers>();
		if (component != null)
		{
			Klei.AI.Attribute maxAttribute = Db.Get().Amounts.Age.maxAttribute;
			float totalValue = AttributeInstance.GetTotalValue(maxAttribute, component.GetPreModifiers(maxAttribute));
			string text;
			if (Mathf.Approximately(totalValue, 0f))
			{
				text = null;
			}
			else
			{
				text = string.Format(CODEX.CREATURE_DESCRIPTORS.MAXAGE, maxAttribute.formatter.GetFormattedValue(totalValue, GameUtil.TimeSlice.None));
			}
			if (text != null)
			{
				containers.Add(new ContentContainer(new List<ICodexWidget>
				{
					new CodexSpacer(),
					new CodexText(CODEX.HEADERS.CRITTERMAXAGE, CodexTextStyle.Subtitle, null),
					new CodexText(text, CodexTextStyle.Body, null)
				}, ContentContainer.ContentLayout.Vertical));
			}
		}
		OvercrowdingMonitor.Def def3 = creature.GetDef<OvercrowdingMonitor.Def>();
		if (def3 != null && def3.spaceRequiredPerCreature > 0)
		{
			bool flag = creature.GetDef<FishOvercrowdingMonitor.Def>() != null;
			LocString locString = (flag ? CODEX.CREATURE_DESCRIPTORS.OVERCROWDING_AQUATIC : CODEX.CREATURE_DESCRIPTORS.OVERCROWDING);
			LocString locString2 = (flag ? CODEX.CREATURE_DESCRIPTORS.CONFINED_AQUATIC : CODEX.CREATURE_DESCRIPTORS.CONFINED);
			containers.Add(new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexText(CODEX.HEADERS.CRITTEROVERCROWDING, CodexTextStyle.Subtitle, null),
				new CodexText("    • " + string.Format(locString, def3.spaceRequiredPerCreature), CodexTextStyle.Body, null),
				new CodexText("    • " + string.Format(locString2, def3.spaceRequiredPerCreature), CodexTextStyle.Body, null)
			}, ContentContainer.ContentLayout.Vertical));
		}
		string text2 = null;
		float num = 0f;
		Tag tag = default(Tag);
		Butcherable component2 = creature.GetComponent<Butcherable>();
		if (component2 != null && component2.drops != null && component2.drops.Count > 0)
		{
			text2 = (tag.Name = component2.drops.Keys.ToList<string>()[0]);
			num = component2.drops[text2];
		}
		string text3 = null;
		string text4 = null;
		if (tag.IsValid)
		{
			text3 = TagManager.GetProperName(tag, false);
			text4 = "\t" + GameUtil.GetFormattedByTag(tag, num, GameUtil.TimeSlice.None);
		}
		if (!string.IsNullOrEmpty(text3) && !string.IsNullOrEmpty(text4))
		{
			ContentContainer contentContainer = new ContentContainer(new List<ICodexWidget>
			{
				new CodexSpacer(),
				new CodexText(CODEX.HEADERS.CRITTERDROPS, CodexTextStyle.Subtitle, null)
			}, ContentContainer.ContentLayout.Vertical);
			ContentContainer contentContainer2 = new ContentContainer(new List<ICodexWidget>
			{
				new CodexIndentedLabelWithIcon(text3, CodexTextStyle.Body, Def.GetUISprite(text2, "ui", false)),
				new CodexText(text4, CodexTextStyle.Body, null)
			}, ContentContainer.ContentLayout.Vertical);
			containers.Add(contentContainer);
			containers.Add(contentContainer2);
		}
		new List<Tag>();
		Diet prefabDiet = DietManager.Instance.GetPrefabDiet(creature);
		if (prefabDiet != null)
		{
			Diet.Info[] infos = prefabDiet.infos;
			if (infos != null && infos.Length != 0)
			{
				float num2 = 0f;
				foreach (AttributeModifier attributeModifier in Db.Get().traits.Get(creature.GetComponent<Modifiers>().initialTraits[0]).SelfModifiers)
				{
					if (attributeModifier.AttributeId == Db.Get().Amounts.Calories.deltaAttribute.Id)
					{
						num2 = attributeModifier.Value;
					}
				}
				CaloriesConsumedElementProducer component3 = creature.GetComponent<CaloriesConsumedElementProducer>();
				List<ICodexWidget> list = new List<ICodexWidget>();
				foreach (Diet.Info info in infos)
				{
					if (info.consumedTags.Count != 0)
					{
						foreach (Tag tag2 in info.consumedTags)
						{
							Element element = ElementLoader.FindElementByHash(ElementLoader.GetElementID(tag2));
							if ((element.id != SimHashes.Vacuum && element.id != SimHashes.Void) || !(Assets.GetPrefab(tag2) == null))
							{
								bool flag2 = prefabDiet.IsConsumedTagAbleToBeEatenDirectly(tag2);
								float num3 = -num2 / info.caloriesPerKg;
								float num4 = num3 * info.producedConversionRate;
								if (flag2)
								{
									if (info.foodType == Diet.Info.FoodType.EatPlantDirectly)
									{
										list.Add(new CodexConversionPanel(tag2.ProperName(), tag2, num3, true, new Func<Tag, float, bool, string>(GameUtil.GetFormattedDirectPlantConsumptionValuePerCycle), info.producedElement, num4, true, null, creature));
									}
									else if (info.foodType == Diet.Info.FoodType.EatPlantStorage)
									{
										list.Add(new CodexConversionPanel(tag2.ProperName(), tag2, num3, true, new Func<Tag, float, bool, string>(GameUtil.GetFormattedPlantStorageConsumptionValuePerCycle), info.producedElement, num4, true, null, creature));
									}
									else if (info.foodType == Diet.Info.FoodType.EatPrey || info.foodType == Diet.Info.FoodType.EatButcheredPrey)
									{
										float num5 = prefabDiet.AvailableCaloriesInPrey(tag2);
										num3 = -num2 / num5;
										num4 = num3 * info.producedConversionRate * num5 / info.caloriesPerKg;
										list.Add(new CodexConversionPanel(tag2.ProperName(), tag2, num3, true, new Func<Tag, float, bool, string>(GameUtil.GetFormattedPreyConsumptionValuePerCycle), info.producedElement, num4, true, null, creature));
									}
								}
								else
								{
									list.Add(new CodexConversionPanel(tag2.ProperName(), tag2, num3, true, info.producedElement, num4, true, creature));
								}
								if (component3 != null)
								{
									list.Add(new CodexConversionPanel(CODEX.HEADERS.CRITTER_EXTRA_DIET_PRODUCTION, tag2, num3, true, component3.producedElement.CreateTag(), num3 * 1000f * component3.kgProducedPerKcalConsumed * 2f, true, creature));
								}
							}
						}
					}
				}
				ContentContainer contentContainer3 = new ContentContainer(list, ContentContainer.ContentLayout.Vertical);
				containers.Add(new ContentContainer(new List<ICodexWidget>
				{
					new CodexSpacer(),
					new CodexCollapsibleHeader(CODEX.HEADERS.DIET, contentContainer3)
				}, ContentContainer.ContentLayout.Vertical));
				containers.Add(contentContainer3);
				containers.Add(new ContentContainer(new List<ICodexWidget>
				{
					new CodexSpacer(),
					new CodexSpacer()
				}, ContentContainer.ContentLayout.Vertical));
				CodexEntryGenerator_Elements.GenerateMadeAndUsedContainers(creature.PrefabID(), containers);
			}
		}
	}

	[CompilerGenerated]
	internal static void <GenerateEntries>g__AddEntry|6_0(string entryId, CodexEntry entry, string parentEntryId = "CREATURES", ref CodexEntryGenerator_Creatures.<>c__DisplayClass6_0 A_3)
	{
		if (entry == null)
		{
			return;
		}
		entry.parentId = parentEntryId;
		CodexCache.AddEntry(entryId, entry, null);
		A_3.results.Add(entryId, entry);
	}

	[CompilerGenerated]
	internal static void <GenerateEntries>g__PushCritterEntry|6_1(Tag speciesTag, ref CodexEntryGenerator_Creatures.<>c__DisplayClass6_0 A_1)
	{
		CodexEntry codexEntry = CodexEntryGenerator_Creatures.GenerateCritterEntry(speciesTag, speciesTag.ProperName(), A_1.brains);
		if (codexEntry != null)
		{
			A_1.critterEntries.Add(new ValueTuple<string, CodexEntry>(speciesTag.ToString(), codexEntry));
		}
	}

	[CompilerGenerated]
	internal static void <GenerateEntries>g__PopAndAddAllCritterEntries|6_2(ref CodexEntryGenerator_Creatures.<>c__DisplayClass6_0 A_0)
	{
		foreach (ValueTuple<string, CodexEntry> valueTuple in A_0.critterEntries.StableSort<ValueTuple<string, CodexEntry>, string>((ValueTuple<string, CodexEntry> pair) => UI.StripLinkFormatting(pair.Item2.name)))
		{
			string item = valueTuple.Item1;
			CodexEntry item2 = valueTuple.Item2;
			CodexEntryGenerator_Creatures.<GenerateEntries>g__AddEntry|6_0(item, item2, "CREATURES", ref A_0);
		}
	}

	[CompilerGenerated]
	internal static void <GenerateFieldGuideEntry>g__AddSubEntry|7_0(string id, string name, ref CodexEntryGenerator_Creatures.<>c__DisplayClass7_0 A_2, ref CodexEntryGenerator_Creatures.<>c__DisplayClass7_1 A_3)
	{
		A_3.subEntryContents = new List<ICodexWidget>();
		A_3.subEntryContents.Add(new CodexText(name, CodexTextStyle.Title, null));
		A_3.subEntry = new SubEntry(id, "CREATURES::GUIDE", new List<ContentContainer>
		{
			new ContentContainer(A_3.subEntryContents, ContentContainer.ContentLayout.Vertical)
		}, name);
		A_2.generalInfoEntry.subEntries.Add(A_3.subEntry);
	}

	[CompilerGenerated]
	internal static void <GenerateFieldGuideEntry>g__AddImage|7_1(Sprite sprite, ref CodexEntryGenerator_Creatures.<>c__DisplayClass7_1 A_1)
	{
		A_1.subEntryContents.Add(new CodexImage(432, 1, sprite));
	}

	[CompilerGenerated]
	internal static void <GenerateFieldGuideEntry>g__AddSubtitle|7_2(string text, ref CodexEntryGenerator_Creatures.<>c__DisplayClass7_1 A_1)
	{
		CodexEntryGenerator_Creatures.<GenerateFieldGuideEntry>g__AddSpacer|7_4(ref A_1);
		A_1.subEntryContents.Add(new CodexText(text, CodexTextStyle.Subtitle, null));
	}

	[CompilerGenerated]
	internal static void <GenerateFieldGuideEntry>g__AddBody|7_3(string text, ref CodexEntryGenerator_Creatures.<>c__DisplayClass7_1 A_1)
	{
		A_1.subEntryContents.Add(new CodexText(text, CodexTextStyle.Body, null));
	}

	[CompilerGenerated]
	internal static void <GenerateFieldGuideEntry>g__AddSpacer|7_4(ref CodexEntryGenerator_Creatures.<>c__DisplayClass7_1 A_0)
	{
		A_0.subEntryContents.Add(new CodexSpacer());
	}

	[CompilerGenerated]
	internal static void <GenerateFieldGuideEntry>g__AddBulletPoint|7_5(string text, ref CodexEntryGenerator_Creatures.<>c__DisplayClass7_1 A_1)
	{
		if (text.StartsWith("    • "))
		{
			text = text.Substring("    • ".Length);
		}
		text = "<indent=13px>•<indent=21px>" + text + "</indent></indent>";
		A_1.subEntryContents.Add(new CodexText(text, CodexTextStyle.Body, null));
	}

	public const string CATEGORY_ID = "CREATURES";

	public const string GUIDE_ID = "CREATURES::GUIDE";

	public const string GUIDE_METABOLISM_ID = "CREATURES::GUIDE::METABOLISM";

	public const string GUIDE_MOOD_ID = "CREATURES::GUIDE::MOOD";

	public const string GUIDE_FERTILITY_ID = "CREATURES::GUIDE::FERTILITY";

	public const string GUIDE_DOMESTICATION_ID = "CREATURES::GUIDE::DOMESTICATION";
}
