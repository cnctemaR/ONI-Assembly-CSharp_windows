using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class BionicUpgradeComponentConfig : IMultiEntityConfig
{
	public static string GenerateTooltipForBooster(BionicUpgradeComponent booster)
	{
		string text = "<b>" + booster.GetProperName() + "</b>";
		InfoDescription component = booster.gameObject.GetComponent<InfoDescription>();
		if (component != null)
		{
			text = text + "\n" + component.description;
		}
		return text + "\n\n" + BionicUpgradeComponentConfig.UpgradesData[booster.PrefabID()].stateMachineDescription;
	}

	public static Tag[] GetBoostersWithSkillPerk(string perkID)
	{
		return (from data in BionicUpgradeComponentConfig.UpgradesData
			where data.Value.skillPerks.Contains(perkID)
			select data into kvp
			select kvp.Key).ToArray<Tag>();
	}

	public AttributeModifier[] CreateBoosterModifiers(string name, Dictionary<string, float> attributes)
	{
		AttributeModifier[] array = new AttributeModifier[attributes.Count];
		string text = Strings.Get("STRINGS.ITEMS.BIONIC_BOOSTERS." + name.ToUpper() + ".NAME");
		int num = 0;
		foreach (KeyValuePair<string, float> keyValuePair in attributes)
		{
			Klei.AI.Attribute attribute = Db.Get().Attributes.Get(keyValuePair.Key);
			array[num] = new AttributeModifier(attribute.Id, keyValuePair.Value, text, false, false, true);
			num++;
		}
		return array;
	}

	public List<GameObject> CreatePrefabs()
	{
		List<GameObject> list = new List<GameObject>();
		if (!DlcManager.IsContentSubscribed("DLC3_ID"))
		{
			return list;
		}
		BionicUpgradeComponentConfig.<>c__DisplayClass27_0 CS$<>8__locals1 = new BionicUpgradeComponentConfig.<>c__DisplayClass27_0();
		string text = "Booster_Dig1";
		AttributeModifier[] array = this.CreateBoosterModifiers(text, new Dictionary<string, float>
		{
			{
				Db.Get().Attributes.Digging.Id,
				5f
			},
			{
				Db.Get().Attributes.Athletics.Id,
				2f
			}
		});
		SkillPerk[] array2 = new SkillPerk[] { Db.Get().SkillPerks.CanDigVeryFirm };
		BionicUpgradeComponentConfig.<>c__DisplayClass27_0 CS$<>8__locals2 = CS$<>8__locals1;
		string text2 = text;
		AttributeModifier[] array3 = array;
		CS$<>8__locals2.skill_worker_def = new BionicUpgrade_SkilledWorker.Def(text2, Db.Get().Attributes.Digging.Id, array3, array2, new string[] { "hat_role_mining1", "hat_role_mining2" });
		list.Add(BionicUpgradeComponentConfig.CreateNewUpgradeComponent(text, null, null, 0f, (StateMachine.Instance smi) => new BionicUpgrade_SkilledWorker.Instance(smi.GetMaster(), CS$<>8__locals1.skill_worker_def), CS$<>8__locals1.skill_worker_def.GetDescription() + "\n\n" + string.Format(global::STRINGS.ITEMS.BIONIC_BOOSTERS.FABRICATION_SOURCE, global::STRINGS.BUILDINGS.PREFABS.CRAFTINGTABLE.NAME), DlcManager.DLC3, "upgrade_disc_kanim", "basic_excavation_0", SimHashes.Creature, null, BionicUpgradeComponentConfig.BoosterType.Basic, true, true, array2));
		BionicUpgradeComponentConfig.<>c__DisplayClass27_1 CS$<>8__locals3 = new BionicUpgradeComponentConfig.<>c__DisplayClass27_1();
		string text3 = "Booster_Construct1";
		AttributeModifier[] array4 = this.CreateBoosterModifiers(text3, new Dictionary<string, float>
		{
			{
				Db.Get().Attributes.Construction.Id,
				5f
			},
			{
				Db.Get().Attributes.Athletics.Id,
				2f
			}
		});
		SkillPerk[] array5 = new SkillPerk[] { Db.Get().SkillPerks.CanDemolish };
		BionicUpgradeComponentConfig.<>c__DisplayClass27_1 CS$<>8__locals4 = CS$<>8__locals3;
		string text4 = text3;
		array3 = array4;
		CS$<>8__locals4.skill_worker_def = new BionicUpgrade_SkilledWorker.Def(text4, Db.Get().Attributes.Construction.Id, array3, array5, new string[] { "hat_role_building1", "hat_role_building2", "hat_role_building3" });
		list.Add(BionicUpgradeComponentConfig.CreateNewUpgradeComponent(text3, null, null, 0f, (StateMachine.Instance smi) => new BionicUpgrade_SkilledWorker.Instance(smi.GetMaster(), CS$<>8__locals3.skill_worker_def), CS$<>8__locals3.skill_worker_def.GetDescription() + "\n\n" + string.Format(global::STRINGS.ITEMS.BIONIC_BOOSTERS.FABRICATION_SOURCE, global::STRINGS.BUILDINGS.PREFABS.CRAFTINGTABLE.NAME), DlcManager.DLC3, "upgrade_disc_kanim", "basic_construction_0", SimHashes.Creature, null, BionicUpgradeComponentConfig.BoosterType.Basic, true, true, array5));
		BionicUpgradeComponentConfig.<>c__DisplayClass27_2 CS$<>8__locals5 = new BionicUpgradeComponentConfig.<>c__DisplayClass27_2();
		string text5 = "Booster_Carry1";
		AttributeModifier[] array6 = this.CreateBoosterModifiers(text5, new Dictionary<string, float>
		{
			{
				Db.Get().Attributes.Strength.Id,
				5f
			},
			{
				Db.Get().Attributes.Athletics.Id,
				2f
			}
		});
		SkillPerk[] array7 = new SkillPerk[] { Db.Get().SkillPerks.IncreasedCarryBionics };
		BionicUpgradeComponentConfig.<>c__DisplayClass27_2 CS$<>8__locals6 = CS$<>8__locals5;
		string text6 = text5;
		array3 = array6;
		CS$<>8__locals6.skill_worker_def = new BionicUpgrade_SkilledWorker.Def(text6, Db.Get().Attributes.Athletics.Id, array3, array7, new string[] { "hat_role_hauling1", "hat_role_hauling2" });
		list.Add(BionicUpgradeComponentConfig.CreateNewUpgradeComponent(text5, null, null, 0f, (StateMachine.Instance smi) => new BionicUpgrade_SkilledWorker.Instance(smi.GetMaster(), CS$<>8__locals5.skill_worker_def), CS$<>8__locals5.skill_worker_def.GetDescription() + "\n\n" + string.Format(global::STRINGS.ITEMS.BIONIC_BOOSTERS.FABRICATION_SOURCE, global::STRINGS.BUILDINGS.PREFABS.CRAFTINGTABLE.NAME), DlcManager.DLC3, "upgrade_disc_kanim", "basic_strength_0", SimHashes.Creature, null, BionicUpgradeComponentConfig.BoosterType.Basic, false, true, array7));
		BionicUpgradeComponentConfig.<>c__DisplayClass27_3 CS$<>8__locals7 = new BionicUpgradeComponentConfig.<>c__DisplayClass27_3();
		string text7 = "Booster_Research1";
		AttributeModifier[] array8 = this.CreateBoosterModifiers(text7, new Dictionary<string, float>
		{
			{
				Db.Get().Attributes.Learning.Id,
				5f
			},
			{
				Db.Get().Attributes.Athletics.Id,
				2f
			}
		});
		SkillPerk[] array9 = new SkillPerk[]
		{
			Db.Get().SkillPerks.AllowAdvancedResearch,
			Db.Get().SkillPerks.CanStudyWorldObjects,
			Db.Get().SkillPerks.AllowGeyserTuning,
			Db.Get().SkillPerks.AllowChemistry
		};
		BionicUpgradeComponentConfig.<>c__DisplayClass27_3 CS$<>8__locals8 = CS$<>8__locals7;
		string text8 = text7;
		array3 = array8;
		CS$<>8__locals8.skill_worker_def = new BionicUpgrade_SkilledWorker.Def(text8, Db.Get().Attributes.Learning.Id, array3, array9, new string[] { "hat_role_research1", "hat_role_research2" });
		list.Add(BionicUpgradeComponentConfig.CreateNewUpgradeComponent(text7, null, null, 0f, (StateMachine.Instance smi) => new BionicUpgrade_SkilledWorker.Instance(smi.GetMaster(), CS$<>8__locals7.skill_worker_def), CS$<>8__locals7.skill_worker_def.GetDescription() + "\n\n" + string.Format(global::STRINGS.ITEMS.BIONIC_BOOSTERS.FABRICATION_SOURCE, global::STRINGS.BUILDINGS.PREFABS.CRAFTINGTABLE.NAME), DlcManager.DLC3, "upgrade_disc_kanim", "science_4", SimHashes.Creature, null, BionicUpgradeComponentConfig.BoosterType.Basic, false, true, array9));
		BionicUpgradeComponentConfig.<>c__DisplayClass27_4 CS$<>8__locals9 = new BionicUpgradeComponentConfig.<>c__DisplayClass27_4();
		string text9 = "Booster_Medicine1";
		AttributeModifier[] array10 = this.CreateBoosterModifiers(text9, new Dictionary<string, float>
		{
			{
				Db.Get().Attributes.Caring.Id,
				5f
			},
			{
				Db.Get().Attributes.Athletics.Id,
				2f
			}
		});
		SkillPerk[] array11 = new SkillPerk[]
		{
			Db.Get().SkillPerks.CanCompound,
			Db.Get().SkillPerks.CanDoctor,
			Db.Get().SkillPerks.CanAdvancedMedicine
		};
		BionicUpgradeComponentConfig.<>c__DisplayClass27_4 CS$<>8__locals10 = CS$<>8__locals9;
		string text10 = text9;
		array3 = array10;
		CS$<>8__locals10.skill_worker_def = new BionicUpgrade_SkilledWorker.Def(text10, Db.Get().Attributes.DoctoredLevel.Id, array3, array11, new string[] { "hat_role_medicalaid1", "hat_role_medicalaid2", "hat_role_medicalaid3" });
		list.Add(BionicUpgradeComponentConfig.CreateNewUpgradeComponent(text9, null, null, 0f, (StateMachine.Instance smi) => new BionicUpgrade_SkilledWorker.Instance(smi.GetMaster(), CS$<>8__locals9.skill_worker_def), CS$<>8__locals9.skill_worker_def.GetDescription() + "\n\n" + string.Format(global::STRINGS.ITEMS.BIONIC_BOOSTERS.FABRICATION_SOURCE, global::STRINGS.BUILDINGS.PREFABS.CRAFTINGTABLE.NAME), DlcManager.DLC3, "upgrade_disc_kanim", "medicine_0", SimHashes.Creature, null, BionicUpgradeComponentConfig.BoosterType.Basic, true, true, array11));
		BionicUpgradeComponentConfig.<>c__DisplayClass27_5 CS$<>8__locals11 = new BionicUpgradeComponentConfig.<>c__DisplayClass27_5();
		string text11 = "Booster_Dig2";
		SkillPerk[] array13;
		if (!DlcManager.IsExpansion1Active())
		{
			SkillPerk[] array12 = new SkillPerk[2];
			array12[0] = Db.Get().SkillPerks.CanDigNearlyImpenetrable;
			array13 = array12;
			array12[1] = Db.Get().SkillPerks.CanDigSuperDuperHard;
		}
		else
		{
			SkillPerk[] array14 = new SkillPerk[3];
			array14[0] = Db.Get().SkillPerks.CanDigNearlyImpenetrable;
			array14[1] = Db.Get().SkillPerks.CanDigSuperDuperHard;
			array13 = array14;
			array14[2] = Db.Get().SkillPerks.CanDigRadioactiveMaterials;
		}
		SkillPerk[] array15 = array13;
		AttributeModifier[] array16 = this.CreateBoosterModifiers(text11, new Dictionary<string, float>
		{
			{
				Db.Get().Attributes.Digging.Id,
				5f
			},
			{
				Db.Get().Attributes.Athletics.Id,
				2f
			}
		});
		string[] array17;
		if (!DlcManager.IsExpansion1Active())
		{
			(array17 = new string[1])[0] = "hat_role_mining3";
		}
		else
		{
			string[] array18 = new string[2];
			array18[0] = "hat_role_mining3";
			array17 = array18;
			array18[1] = "hat_role_mining4";
		}
		string[] array19 = array17;
		BionicUpgradeComponentConfig.<>c__DisplayClass27_5 CS$<>8__locals12 = CS$<>8__locals11;
		string text12 = text11;
		array3 = array16;
		CS$<>8__locals12.skill_worker_def = new BionicUpgrade_SkilledWorker.Def(text12, Db.Get().Attributes.Digging.Id, array3, array15, array19);
		list.Add(BionicUpgradeComponentConfig.CreateNewUpgradeComponent(text11, null, null, 0f, (StateMachine.Instance smi) => new BionicUpgrade_SkilledWorker.Instance(smi.GetMaster(), CS$<>8__locals11.skill_worker_def), CS$<>8__locals11.skill_worker_def.GetDescription() + "\n\n" + string.Format(global::STRINGS.ITEMS.BIONIC_BOOSTERS.FABRICATION_SOURCE, global::STRINGS.BUILDINGS.PREFABS.ADVANCEDCRAFTINGTABLE.NAME), DlcManager.DLC3, "upgrade_disc_kanim", "excavation_1", SimHashes.Creature, null, BionicUpgradeComponentConfig.BoosterType.Intermediate, true, true, array15));
		BionicUpgradeComponentConfig.<>c__DisplayClass27_6 CS$<>8__locals13 = new BionicUpgradeComponentConfig.<>c__DisplayClass27_6();
		string text13 = "Booster_Farm1";
		List<SkillPerk> list2 = new List<SkillPerk>
		{
			Db.Get().SkillPerks.CanFarmTinker,
			Db.Get().SkillPerks.CanFarmStation,
			Db.Get().SkillPerks.CanSalvagePlantFiber
		};
		if (DlcManager.IsExpansion1Active())
		{
			list2.Add(Db.Get().SkillPerks.CanIdentifyMutantSeeds);
		}
		AttributeModifier[] array20 = this.CreateBoosterModifiers(text13, new Dictionary<string, float>
		{
			{
				Db.Get().Attributes.Botanist.Id,
				5f
			},
			{
				Db.Get().Attributes.Athletics.Id,
				2f
			}
		});
		BionicUpgradeComponentConfig.<>c__DisplayClass27_6 CS$<>8__locals14 = CS$<>8__locals13;
		string text14 = text13;
		array3 = array20;
		CS$<>8__locals14.skill_worker_def = new BionicUpgrade_SkilledWorker.Def(text14, Db.Get().Attributes.Botanist.Id, array3, list2.ToArray(), new string[] { "hat_role_farming1", "hat_role_farming2", "hat_role_farming3" });
		list.Add(BionicUpgradeComponentConfig.CreateNewUpgradeComponent(text13, null, null, 0f, (StateMachine.Instance smi) => new BionicUpgrade_SkilledWorker.Instance(smi.GetMaster(), CS$<>8__locals13.skill_worker_def), CS$<>8__locals13.skill_worker_def.GetDescription() + "\n\n" + string.Format(global::STRINGS.ITEMS.BIONIC_BOOSTERS.FABRICATION_SOURCE, global::STRINGS.BUILDINGS.PREFABS.ADVANCEDCRAFTINGTABLE.NAME), DlcManager.DLC3, "upgrade_disc_kanim", "agriculture_0", SimHashes.Creature, null, BionicUpgradeComponentConfig.BoosterType.Intermediate, true, false, list2.ToArray()));
		BionicUpgradeComponentConfig.<>c__DisplayClass27_7 CS$<>8__locals15 = new BionicUpgradeComponentConfig.<>c__DisplayClass27_7();
		string text15 = "Booster_Ranch1";
		AttributeModifier[] array21 = this.CreateBoosterModifiers(text15, new Dictionary<string, float>
		{
			{
				Db.Get().Attributes.Ranching.Id,
				5f
			},
			{
				Db.Get().Attributes.Athletics.Id,
				2f
			}
		});
		SkillPerk[] array22 = new SkillPerk[]
		{
			Db.Get().SkillPerks.CanWrangleCreatures,
			Db.Get().SkillPerks.CanUseRanchStation,
			Db.Get().SkillPerks.CanUseMilkingStation
		};
		BionicUpgradeComponentConfig.<>c__DisplayClass27_7 CS$<>8__locals16 = CS$<>8__locals15;
		string text16 = text15;
		array3 = array21;
		CS$<>8__locals16.skill_worker_def = new BionicUpgrade_SkilledWorker.Def(text16, Db.Get().Attributes.Ranching.Id, array3, array22, new string[] { "hat_role_rancher1", "hat_role_rancher2" });
		list.Add(BionicUpgradeComponentConfig.CreateNewUpgradeComponent(text15, null, null, 0f, (StateMachine.Instance smi) => new BionicUpgrade_SkilledWorker.Instance(smi.GetMaster(), CS$<>8__locals15.skill_worker_def), CS$<>8__locals15.skill_worker_def.GetDescription() + "\n\n" + string.Format(global::STRINGS.ITEMS.BIONIC_BOOSTERS.FABRICATION_SOURCE, global::STRINGS.BUILDINGS.PREFABS.ADVANCEDCRAFTINGTABLE.NAME), DlcManager.DLC3, "upgrade_disc_kanim", "ranching_0", SimHashes.Creature, null, BionicUpgradeComponentConfig.BoosterType.Intermediate, true, false, array22));
		BionicUpgradeComponentConfig.<>c__DisplayClass27_8 CS$<>8__locals17 = new BionicUpgradeComponentConfig.<>c__DisplayClass27_8();
		string text17 = "Booster_Cook1";
		AttributeModifier[] array23 = this.CreateBoosterModifiers(text17, new Dictionary<string, float>
		{
			{
				Db.Get().Attributes.Cooking.Id,
				5f
			},
			{
				Db.Get().Attributes.Athletics.Id,
				2f
			}
		});
		SkillPerk[] array24 = new SkillPerk[]
		{
			Db.Get().SkillPerks.CanElectricGrill,
			Db.Get().SkillPerks.CanDeepFry,
			Db.Get().SkillPerks.CanGasRange,
			Db.Get().SkillPerks.CanSpiceGrinder
		};
		BionicUpgradeComponentConfig.<>c__DisplayClass27_8 CS$<>8__locals18 = CS$<>8__locals17;
		string text18 = text17;
		array3 = array23;
		CS$<>8__locals18.skill_worker_def = new BionicUpgrade_SkilledWorker.Def(text18, Db.Get().Attributes.Cooking.Id, array3, array24, new string[] { "hat_role_cooking1", "hat_role_cooking2" });
		list.Add(BionicUpgradeComponentConfig.CreateNewUpgradeComponent(text17, null, null, 0f, (StateMachine.Instance smi) => new BionicUpgrade_SkilledWorker.Instance(smi.GetMaster(), CS$<>8__locals17.skill_worker_def), CS$<>8__locals17.skill_worker_def.GetDescription() + "\n\n" + string.Format(global::STRINGS.ITEMS.BIONIC_BOOSTERS.FABRICATION_SOURCE, global::STRINGS.BUILDINGS.PREFABS.ADVANCEDCRAFTINGTABLE.NAME), DlcManager.DLC3, "upgrade_disc_kanim", "cooking_0", SimHashes.Creature, null, BionicUpgradeComponentConfig.BoosterType.Intermediate, true, true, array24));
		BionicUpgradeComponentConfig.<>c__DisplayClass27_9 CS$<>8__locals19 = new BionicUpgradeComponentConfig.<>c__DisplayClass27_9();
		string text19 = "Booster_Art1";
		List<SkillPerk> list3 = new List<SkillPerk>
		{
			Db.Get().SkillPerks.CanArt,
			Db.Get().SkillPerks.CanClothingAlteration,
			Db.Get().SkillPerks.CanArtGreat
		};
		if (DlcManager.FeatureClusterSpaceEnabled())
		{
			list3.Add(Db.Get().SkillPerks.CanStudyArtifact);
		}
		AttributeModifier[] array25 = this.CreateBoosterModifiers(text19, new Dictionary<string, float>
		{
			{
				Db.Get().Attributes.Art.Id,
				5f
			},
			{
				Db.Get().Attributes.Athletics.Id,
				2f
			}
		});
		BionicUpgradeComponentConfig.<>c__DisplayClass27_9 CS$<>8__locals20 = CS$<>8__locals19;
		string text20 = text19;
		array3 = array25;
		CS$<>8__locals20.skill_worker_def = new BionicUpgrade_SkilledWorker.Def(text20, Db.Get().Attributes.Art.Id, array3, list3.ToArray(), new string[] { "hat_role_art1", "hat_role_art2", "hat_role_art3" });
		list.Add(BionicUpgradeComponentConfig.CreateNewUpgradeComponent(text19, null, null, 0f, (StateMachine.Instance smi) => new BionicUpgrade_SkilledWorker.Instance(smi.GetMaster(), CS$<>8__locals19.skill_worker_def), CS$<>8__locals19.skill_worker_def.GetDescription() + "\n\n" + string.Format(global::STRINGS.ITEMS.BIONIC_BOOSTERS.FABRICATION_SOURCE, global::STRINGS.BUILDINGS.PREFABS.ADVANCEDCRAFTINGTABLE.NAME), DlcManager.DLC3, "upgrade_disc_kanim", "creativity_0", SimHashes.Creature, null, BionicUpgradeComponentConfig.BoosterType.Intermediate, true, false, list3.ToArray()));
		BionicUpgradeComponentConfig.<>c__DisplayClass27_10 CS$<>8__locals21 = new BionicUpgradeComponentConfig.<>c__DisplayClass27_10();
		string text21 = "Booster_Research2";
		List<SkillPerk> list4 = new List<SkillPerk> { Db.Get().SkillPerks.CanMissionControl };
		if (DlcManager.FeatureClusterSpaceEnabled())
		{
			list4.Add(Db.Get().SkillPerks.CanUseClusterTelescope);
			list4.Add(Db.Get().SkillPerks.AllowOrbitalResearch);
		}
		else
		{
			list4.Add(Db.Get().SkillPerks.AllowInterstellarResearch);
		}
		string[] array26;
		if (!DlcManager.IsExpansion1Active())
		{
			(array26 = new string[1])[0] = "hat_role_research3";
		}
		else
		{
			string[] array27 = new string[2];
			array27[0] = "hat_role_research3";
			array26 = array27;
			array27[1] = "hat_role_research4";
		}
		string[] array28 = array26;
		AttributeModifier[] array29 = this.CreateBoosterModifiers(text21, new Dictionary<string, float>
		{
			{
				Db.Get().Attributes.Learning.Id,
				5f
			},
			{
				Db.Get().Attributes.Athletics.Id,
				2f
			}
		});
		BionicUpgradeComponentConfig.<>c__DisplayClass27_10 CS$<>8__locals22 = CS$<>8__locals21;
		string text22 = text21;
		array3 = array29;
		CS$<>8__locals22.skill_worker_def = new BionicUpgrade_SkilledWorker.Def(text22, Db.Get().Attributes.Learning.Id, array3, list4.ToArray(), array28);
		list.Add(BionicUpgradeComponentConfig.CreateNewUpgradeComponent(text21, null, null, 0f, (StateMachine.Instance smi) => new BionicUpgrade_SkilledWorker.Instance(smi.GetMaster(), CS$<>8__locals21.skill_worker_def), CS$<>8__locals21.skill_worker_def.GetDescription() + "\n\n" + string.Format(global::STRINGS.ITEMS.BIONIC_BOOSTERS.FABRICATION_SOURCE, global::STRINGS.BUILDINGS.PREFABS.ADVANCEDCRAFTINGTABLE.NAME), DlcManager.DLC3, "upgrade_disc_kanim", "science_2", SimHashes.Creature, null, BionicUpgradeComponentConfig.BoosterType.Intermediate, false, false, list4.ToArray()));
		if (DlcManager.IsExpansion1Active())
		{
			BionicUpgradeComponentConfig.<>c__DisplayClass27_11 CS$<>8__locals23 = new BionicUpgradeComponentConfig.<>c__DisplayClass27_11();
			string text23 = "Booster_Research3";
			AttributeModifier[] array30 = this.CreateBoosterModifiers(text23, new Dictionary<string, float>
			{
				{
					Db.Get().Attributes.Learning.Id,
					5f
				},
				{
					Db.Get().Attributes.Athletics.Id,
					2f
				}
			});
			SkillPerk[] array31 = new SkillPerk[] { Db.Get().SkillPerks.AllowNuclearResearch };
			BionicUpgradeComponentConfig.<>c__DisplayClass27_11 CS$<>8__locals24 = CS$<>8__locals23;
			string text24 = text23;
			array3 = array30;
			CS$<>8__locals24.skill_worker_def = new BionicUpgrade_SkilledWorker.Def(text24, Db.Get().Attributes.Learning.Id, array3, array31, new string[] { "hat_role_research5" });
			list.Add(BionicUpgradeComponentConfig.CreateNewUpgradeComponent(text23, null, null, 0f, (StateMachine.Instance smi) => new BionicUpgrade_SkilledWorker.Instance(smi.GetMaster(), CS$<>8__locals23.skill_worker_def), CS$<>8__locals23.skill_worker_def.GetDescription() + "\n\n" + string.Format(global::STRINGS.ITEMS.BIONIC_BOOSTERS.FABRICATION_SOURCE, global::STRINGS.BUILDINGS.PREFABS.ADVANCEDCRAFTINGTABLE.NAME), DlcManager.DLC3, "upgrade_disc_kanim", "science_3", SimHashes.Creature, null, BionicUpgradeComponentConfig.BoosterType.Intermediate, false, false, array31));
		}
		if (DlcManager.IsExpansion1Active())
		{
			BionicUpgradeComponentConfig.<>c__DisplayClass27_12 CS$<>8__locals25 = new BionicUpgradeComponentConfig.<>c__DisplayClass27_12();
			string text25 = "Booster_Pilot1";
			AttributeModifier[] array32 = this.CreateBoosterModifiers(text25, new Dictionary<string, float>
			{
				{
					Db.Get().Attributes.SpaceNavigation.Id,
					5f
				},
				{
					Db.Get().Attributes.Athletics.Id,
					2f
				}
			});
			SkillPerk[] array33 = new SkillPerk[] { Db.Get().SkillPerks.CanUseRocketControlStation };
			BionicUpgradeComponentConfig.<>c__DisplayClass27_12 CS$<>8__locals26 = CS$<>8__locals25;
			string text26 = text25;
			array3 = array32;
			CS$<>8__locals26.skill_worker_def = new BionicUpgrade_SkilledWorker.Def(text26, Db.Get().Attributes.SpaceNavigation.Id, array3, array33, new string[] { "hat_role_astronaut1", "hat_role_astronaut2" });
			list.Add(BionicUpgradeComponentConfig.CreateNewUpgradeComponent(text25, null, null, 0f, (StateMachine.Instance smi) => new BionicUpgrade_SkilledWorker.Instance(smi.GetMaster(), CS$<>8__locals25.skill_worker_def), CS$<>8__locals25.skill_worker_def.GetDescription() + "\n\n" + string.Format(global::STRINGS.ITEMS.BIONIC_BOOSTERS.FABRICATION_SOURCE, global::STRINGS.BUILDINGS.PREFABS.ADVANCEDCRAFTINGTABLE.NAME), DlcManager.DLC3, "upgrade_disc_kanim", "piloting_0", SimHashes.Creature, null, BionicUpgradeComponentConfig.BoosterType.Intermediate, false, false, array33));
		}
		if (DlcManager.IsPureVanilla())
		{
			string text27 = "Booster_PilotVanilla1";
			AttributeModifier[] array34 = this.CreateBoosterModifiers(text27, new Dictionary<string, float> { 
			{
				Db.Get().Attributes.Athletics.Id,
				3f
			} });
			SkillPerk[] array35 = new SkillPerk[] { Db.Get().SkillPerks.CanUseRockets };
			BionicUpgrade_SkilledWorker.Def skill_worker_def = new BionicUpgrade_SkilledWorker.Def(text27, null, array34, array35, new string[] { "hat_role_astronaut1", "hat_role_astronaut2" });
			list.Add(BionicUpgradeComponentConfig.CreateNewUpgradeComponent(text27, null, null, 0f, (StateMachine.Instance smi) => new BionicUpgrade_SkilledWorker.Instance(smi.GetMaster(), skill_worker_def), skill_worker_def.GetDescription() + "\n\n" + string.Format(global::STRINGS.ITEMS.BIONIC_BOOSTERS.FABRICATION_SOURCE, global::STRINGS.BUILDINGS.PREFABS.ADVANCEDCRAFTINGTABLE.NAME), DlcManager.DLC3, "upgrade_disc_kanim", "piloting_vanilla_0", SimHashes.Creature, null, BionicUpgradeComponentConfig.BoosterType.Intermediate, false, false, array35));
		}
		BionicUpgradeComponentConfig.<>c__DisplayClass27_14 CS$<>8__locals28 = new BionicUpgradeComponentConfig.<>c__DisplayClass27_14();
		string text28 = "Booster_Suits1";
		AttributeModifier[] array36 = this.CreateBoosterModifiers(text28, new Dictionary<string, float> { 
		{
			Db.Get().Attributes.Athletics.Id,
			5f
		} });
		SkillPerk[] array37 = new SkillPerk[]
		{
			Db.Get().SkillPerks.ExosuitDurability,
			Db.Get().SkillPerks.ExosuitExpertise
		};
		BionicUpgradeComponentConfig.<>c__DisplayClass27_14 CS$<>8__locals29 = CS$<>8__locals28;
		string text29 = text28;
		array3 = array36;
		CS$<>8__locals29.skill_worker_def = new BionicUpgrade_SkilledWorker.Def(text29, Db.Get().Attributes.Athletics.Id, array3, array37, new string[] { "hat_role_suits1", "hat_role_suits2" });
		list.Add(BionicUpgradeComponentConfig.CreateNewUpgradeComponent(text28, null, null, 0f, (StateMachine.Instance smi) => new BionicUpgrade_SkilledWorker.Instance(smi.GetMaster(), CS$<>8__locals28.skill_worker_def), CS$<>8__locals28.skill_worker_def.GetDescription() + "\n\n" + string.Format(global::STRINGS.ITEMS.BIONIC_BOOSTERS.FABRICATION_SOURCE, global::STRINGS.BUILDINGS.PREFABS.ADVANCEDCRAFTINGTABLE.NAME), DlcManager.DLC3, "upgrade_disc_kanim", "suits_0", SimHashes.Creature, null, BionicUpgradeComponentConfig.BoosterType.Intermediate, true, false, array37));
		BionicUpgradeComponentConfig.<>c__DisplayClass27_15 CS$<>8__locals30 = new BionicUpgradeComponentConfig.<>c__DisplayClass27_15();
		string text30 = "Booster_Tidy1";
		AttributeModifier[] array38 = this.CreateBoosterModifiers(text30, new Dictionary<string, float>
		{
			{
				Db.Get().Attributes.Strength.Id,
				5f
			},
			{
				Db.Get().Attributes.Athletics.Id,
				2f
			}
		});
		SkillPerk[] array39 = new SkillPerk[]
		{
			Db.Get().SkillPerks.CanDoPlumbing,
			Db.Get().SkillPerks.CanMakeMissiles
		};
		BionicUpgradeComponentConfig.<>c__DisplayClass27_15 CS$<>8__locals31 = CS$<>8__locals30;
		string text31 = text30;
		array3 = array38;
		CS$<>8__locals31.skill_worker_def = new BionicUpgrade_SkilledWorker.Def(text31, Db.Get().Attributes.Strength.Id, array3, array39, new string[] { "hat_role_basekeeping1", "hat_role_basekeeping2", "hat_role_pyrotechnics" });
		list.Add(BionicUpgradeComponentConfig.CreateNewUpgradeComponent(text30, null, null, 0f, (StateMachine.Instance smi) => new BionicUpgrade_SkilledWorker.Instance(smi.GetMaster(), CS$<>8__locals30.skill_worker_def), CS$<>8__locals30.skill_worker_def.GetDescription() + "\n\n" + string.Format(global::STRINGS.ITEMS.BIONIC_BOOSTERS.FABRICATION_SOURCE, global::STRINGS.BUILDINGS.PREFABS.ADVANCEDCRAFTINGTABLE.NAME), DlcManager.DLC3, "upgrade_disc_kanim", "tidy_0", SimHashes.Creature, null, BionicUpgradeComponentConfig.BoosterType.Intermediate, false, false, array39));
		BionicUpgradeComponentConfig.<>c__DisplayClass27_16 CS$<>8__locals32 = new BionicUpgradeComponentConfig.<>c__DisplayClass27_16();
		string text32 = "Booster_Op1";
		AttributeModifier[] array40 = this.CreateBoosterModifiers(text32, new Dictionary<string, float>
		{
			{
				Db.Get().Attributes.Machinery.Id,
				5f
			},
			{
				Db.Get().Attributes.Athletics.Id,
				2f
			}
		});
		SkillPerk[] array41 = new SkillPerk[]
		{
			Db.Get().SkillPerks.CanPowerTinker,
			Db.Get().SkillPerks.CanCraftElectronics
		};
		BionicUpgradeComponentConfig.<>c__DisplayClass27_16 CS$<>8__locals33 = CS$<>8__locals32;
		string text33 = text32;
		array3 = array40;
		CS$<>8__locals33.skill_worker_def = new BionicUpgrade_SkilledWorker.Def(text33, Db.Get().Attributes.Machinery.Id, array3, array41, new string[] { "hat_role_technicals1", "hat_role_technicals2" });
		list.Add(BionicUpgradeComponentConfig.CreateNewUpgradeComponent(text32, null, null, 0f, (StateMachine.Instance smi) => new BionicUpgrade_SkilledWorker.Instance(smi.GetMaster(), CS$<>8__locals32.skill_worker_def), CS$<>8__locals32.skill_worker_def.GetDescription() + "\n\n" + string.Format(global::STRINGS.ITEMS.BIONIC_BOOSTERS.FABRICATION_SOURCE, global::STRINGS.BUILDINGS.PREFABS.ADVANCEDCRAFTINGTABLE.NAME), DlcManager.DLC3, "upgrade_disc_kanim", "machinery_0", SimHashes.Creature, null, BionicUpgradeComponentConfig.BoosterType.Intermediate, true, false, array41));
		BionicUpgradeComponentConfig.<>c__DisplayClass27_17 CS$<>8__locals34 = new BionicUpgradeComponentConfig.<>c__DisplayClass27_17();
		string text34 = "Booster_Op2";
		AttributeModifier[] array42 = this.CreateBoosterModifiers(text34, new Dictionary<string, float>
		{
			{
				Db.Get().Attributes.Machinery.Id,
				5f
			},
			{
				Db.Get().Attributes.Athletics.Id,
				2f
			}
		});
		SkillPerk[] array43 = new SkillPerk[] { Db.Get().SkillPerks.ConveyorBuild };
		BionicUpgradeComponentConfig.<>c__DisplayClass27_17 CS$<>8__locals35 = CS$<>8__locals34;
		string text35 = text34;
		array3 = array42;
		CS$<>8__locals35.skill_worker_def = new BionicUpgrade_SkilledWorker.Def(text35, Db.Get().Attributes.Machinery.Id, array3, array43, new string[] { "hat_role_engineering1" });
		list.Add(BionicUpgradeComponentConfig.CreateNewUpgradeComponent(text34, null, null, 0f, (StateMachine.Instance smi) => new BionicUpgrade_SkilledWorker.Instance(smi.GetMaster(), CS$<>8__locals34.skill_worker_def), CS$<>8__locals34.skill_worker_def.GetDescription() + "\n\n" + string.Format(global::STRINGS.ITEMS.BIONIC_BOOSTERS.FABRICATION_SOURCE, global::STRINGS.BUILDINGS.PREFABS.ADVANCEDCRAFTINGTABLE.NAME), DlcManager.DLC3, "upgrade_disc_kanim", "machinery_1", SimHashes.Creature, null, BionicUpgradeComponentConfig.BoosterType.Advanced, false, false, array43));
		list.RemoveAll((GameObject t) => t == null);
		return list;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public static Tag GetBionicUpgradePrefabIDWithTraitID(string traitID)
	{
		foreach (Tag tag in BionicUpgradeComponentConfig.UpgradesData.Keys)
		{
			BionicUpgradeComponentConfig.BionicUpgradeData bionicUpgradeData = BionicUpgradeComponentConfig.UpgradesData[tag];
			if (bionicUpgradeData.relatedTrait != null && bionicUpgradeData.relatedTrait == traitID)
			{
				return tag;
			}
		}
		return Tag.Invalid;
	}

	public static GameObject CreateNewUpgradeComponent(string id, string name = null, string desc = null, float wattageCost = 0f, Func<StateMachine.Instance, StateMachine.Instance> stateMachine = null, string sm_description = "", string[] dlcIDs = null, string animFile = "upgrade_disc_kanim", string animStateName = "object", SimHashes element = SimHashes.Creature, string craftTechUnlockID = null, BionicUpgradeComponentConfig.BoosterType booster = BionicUpgradeComponentConfig.BoosterType.Basic, bool isStartingBooster = false, bool isCarePackage = false, SkillPerk[] skillPerks = null)
	{
		if (!DlcManager.IsAllContentSubscribed(dlcIDs))
		{
			return null;
		}
		if (name == null)
		{
			name = Strings.Get("STRINGS.ITEMS.BIONIC_BOOSTERS." + id.ToUpper() + ".NAME");
		}
		if (desc == null)
		{
			desc = Strings.Get("STRINGS.ITEMS.BIONIC_BOOSTERS." + id.ToUpper() + ".DESC");
		}
		string ID = id;
		TechItem techItem = new TechItem(ID, Db.Get().TechItems, Strings.Get("STRINGS.RESEARCH.OTHER_TECH_ITEMS." + id.ToUpper() + ".NAME"), Strings.Get("STRINGS.RESEARCH.OTHER_TECH_ITEMS." + id.ToUpper() + ".DESC"), (string a, bool b) => Def.GetUISprite(Assets.GetPrefab(ID), "ui", false).first, craftTechUnlockID, DlcManager.DLC3, null, false);
		if (!craftTechUnlockID.IsNullOrWhiteSpace())
		{
			Db.Get().Techs.Get(craftTechUnlockID).AddUnlockedItemIDs(new string[] { techItem.Id });
		}
		GameObject gameObject = EntityTemplates.CreateLooseEntity(ID, name, desc, 25f, true, Assets.GetAnim(animFile), animStateName, Grid.SceneLayer.Ore, EntityTemplates.CollisionShape.RECTANGLE, 0.6f, 0.45f, true, SORTORDER.ARTIFACTS, element, new List<Tag>
		{
			GameTags.BionicUpgrade,
			GameTags.MiscPickupable,
			GameTags.NotRoomAssignable
		});
		gameObject.AddOrGet<OccupyArea>().SetCellOffsets(EntityTemplates.GenerateOffsets(1, 1));
		DecorProvider decorProvider = gameObject.AddOrGet<DecorProvider>();
		decorProvider.SetValues(DECOR.NONE);
		decorProvider.overrideName = gameObject.GetProperName();
		gameObject.AddOrGet<BionicUpgradeComponent>().slotID = Db.Get().AssignableSlots.BionicUpgrade.Id;
		gameObject.AddOrGet<KSelectable>();
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.AddTag(GameTags.PedestalDisplayable, false);
		component.requiredDlcIds = dlcIDs;
		component.SetUnityEditorConfigOverride("BionicUpgradeComponentConfig");
		string text = null;
		if (isStartingBooster)
		{
			text = "StartWith" + id;
			DUPLICANTSTATS.BIONICUPGRADETRAITS.Add(new DUPLICANTSTATS.TraitVal
			{
				id = text,
				requiredDlcIds = DlcManager.DLC3
			});
			TraitUtil.CreateBionicUpgradeTrait(text, sm_description)();
		}
		BionicUpgradeComponentConfig.UpgradesData.Add(component.PrefabTag, new BionicUpgradeComponentConfig.BionicUpgradeData(wattageCost, animStateName, text, booster, stateMachine, sm_description, isCarePackage, skillPerks.Select<SkillPerk, string>((SkillPerk perk) => perk.Id).ToArray<string>()));
		if (!BionicUpgradeComponentConfig.BASIC_BOOSTERS.Contains(ID))
		{
			ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement("PowerStationTools", 8f)
			};
			ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement(ID.ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
			};
			ComplexRecipe complexRecipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID(ID, array, array2), array, array2);
			complexRecipe.time = INDUSTRIAL.RECIPES.STANDARD_FABRICATION_TIME;
			complexRecipe.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.ADVANCEDCRAFTINGTABLE.BIONIC_COMPONENT_RECIPE_DESC, global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.POWER_STATION_TOOLS.NAME, name) + "\n\n" + BionicUpgradeComponentConfig.UpgradesData[ID].stateMachineDescription;
			complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
			complexRecipe.fabricators = new List<Tag> { "AdvancedCraftingTable" };
			complexRecipe.requiredTech = craftTechUnlockID;
			complexRecipe.sortOrder = 3;
			complexRecipe.runTimeDescription = () => BionicUpgradeComponentConfig.GetColonyBoosterAssignmentString(ID);
		}
		else
		{
			ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement("PowerStationTools", (float)((booster == BionicUpgradeComponentConfig.BoosterType.Basic) ? 2 : 4), true)
			};
			ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement(ID, 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
			};
			ComplexRecipe complexRecipe2 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID(ID, array3, array4), array3, array4, DlcManager.DLC3);
			complexRecipe2.time = INDUSTRIAL.RECIPES.STANDARD_FABRICATION_TIME * 2f;
			complexRecipe2.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.ADVANCEDCRAFTINGTABLE.BIONIC_COMPONENT_RECIPE_DESC, global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.POWER_STATION_TOOLS.NAME, name) + "\n\n" + BionicUpgradeComponentConfig.UpgradesData[ID].stateMachineDescription;
			complexRecipe2.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
			complexRecipe2.fabricators = new List<Tag> { "CraftingTable" };
			complexRecipe2.sortOrder = 1;
			complexRecipe2.runTimeDescription = () => BionicUpgradeComponentConfig.GetColonyBoosterAssignmentString(ID);
		}
		return gameObject;
	}

	public static string GetColonyBoosterAssignmentString(string boosterID)
	{
		int num = 0;
		foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.GetWorldItems(ClusterManager.Instance.activeWorldId, false))
		{
			if (minionIdentity.HasTag(GameTags.Minions.Models.Bionic))
			{
				BionicUpgradesMonitor.Instance smi = minionIdentity.GetSMI<BionicUpgradesMonitor.Instance>();
				if (smi != null && smi.upgradeComponentSlots != null)
				{
					foreach (BionicUpgradesMonitor.UpgradeComponentSlot upgradeComponentSlot in smi.upgradeComponentSlots)
					{
						if (upgradeComponentSlot.HasUpgradeComponentAssigned && upgradeComponentSlot.assignedUpgradeComponent.PrefabID() == boosterID)
						{
							num++;
							break;
						}
					}
				}
			}
		}
		if (num == 0)
		{
			return string.Format(global::STRINGS.BUILDINGS.PREFABS.ADVANCEDCRAFTINGTABLE.COLONY_HAS_BOOSTER_ASSIGNED_NONE, Array.Empty<object>());
		}
		return string.Format(global::STRINGS.BUILDINGS.PREFABS.ADVANCEDCRAFTINGTABLE.COLONY_HAS_BOOSTER_ASSIGNED_COUNT, num);
	}

	public const string DEFAULT_ANIM_FILE_NAME = "upgrade_disc_kanim";

	public const string STARTING_TRAIT_PREFIX = "StartWith";

	public const string Booster_Dig1 = "Booster_Dig1";

	public const string Booster_Construct1 = "Booster_Construct1";

	public const string Booster_Dig2 = "Booster_Dig2";

	public const string Booster_Farm1 = "Booster_Farm1";

	public const string Booster_Ranch1 = "Booster_Ranch1";

	public const string Booster_Cook1 = "Booster_Cook1";

	public const string Booster_Art1 = "Booster_Art1";

	public const string Booster_Research1 = "Booster_Research1";

	public const string Booster_Research2 = "Booster_Research2";

	public const string Booster_Research3 = "Booster_Research3";

	public const string Booster_Pilot1 = "Booster_Pilot1";

	public const string Booster_PilotVanilla1 = "Booster_PilotVanilla1";

	public const string Booster_Suits1 = "Booster_Suits1";

	public const string Booster_Carry1 = "Booster_Carry1";

	public const string Booster_Op1 = "Booster_Op1";

	public const string Booster_Op2 = "Booster_Op2";

	public const string Booster_Medicine1 = "Booster_Medicine1";

	public const string Booster_Tidy1 = "Booster_Tidy1";

	public static List<string> BASIC_BOOSTERS = new List<string> { "Booster_Dig1", "Booster_Construct1", "Booster_Carry1", "Booster_Research1", "Booster_Medicine1" };

	public static Dictionary<Tag, BionicUpgradeComponentConfig.BionicUpgradeData> UpgradesData = new Dictionary<Tag, BionicUpgradeComponentConfig.BionicUpgradeData>();

	public enum BoosterType
	{
		Basic,
		Intermediate,
		Advanced,
		Sleep,
		Space,
		Special
	}

	public class BionicUpgradeData
	{
		public float WattageCost { get; private set; }

		public Func<StateMachine.Instance, StateMachine.Instance> stateMachine { get; private set; }

		public string uiAnimName
		{
			get
			{
				if (!(this.animStateName == "object"))
				{
					return "ui_" + this.animStateName;
				}
				return "ui";
			}
		}

		public string relatedTrait { get; private set; }

		public BionicUpgradeComponentConfig.BoosterType Booster { get; private set; }

		public bool isCarePackage { get; private set; }

		public BionicUpgradeData(float cost, string animStateName, string relatedTrait, BionicUpgradeComponentConfig.BoosterType booster, Func<StateMachine.Instance, StateMachine.Instance> smi, string stateMachineDescription, bool isCarePackage, string[] skillPerkIds = null)
		{
			this.WattageCost = cost;
			this.stateMachine = smi;
			this.stateMachineDescription = stateMachineDescription;
			this.animStateName = animStateName;
			this.relatedTrait = relatedTrait;
			this.Booster = booster;
			this.isCarePackage = isCarePackage;
			this.skillPerks = skillPerkIds;
		}

		private const string DEFAULT_ANIM_STATE_NAME = "object";

		public string stateMachineDescription;

		public string animStateName = "object";

		public string[] skillPerks = new string[0];
	}
}
