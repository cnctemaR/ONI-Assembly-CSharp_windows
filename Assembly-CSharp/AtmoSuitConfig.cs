using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class AtmoSuitConfig : IEquipmentConfig
{
	public EquipmentDef CreateEquipmentDef()
	{
		Dictionary<string, float> dictionary = new Dictionary<string, float>();
		dictionary.Add(SimHashes.Cuprite.ToString(), 300f);
		dictionary.Add("BasicFabric", 2f);
		List<AttributeModifier> list = new List<AttributeModifier>();
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.INSULATION, (float)global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_INSULATION, global::STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME, false, false));
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.ATHLETICS, (float)global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_ATHLETICS, global::STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME, false, false));
		list.Add(new AttributeModifier(global::TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.THERMAL_CONDUCTIVITY_BARRIER, global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_THERMAL_CONDUCTIVITY_BARRIER, global::STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME, false, false));
		list.Add(new AttributeModifier(Db.Get().Attributes.Digging.Id, (float)global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_DIGGING, global::STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME, false, false));
		list.Add(new AttributeModifier(Db.Get().Amounts.Bladder.deltaAttribute.Id, global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_BLADDER, global::STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME, false, false));
		list.Add(new AttributeModifier(Db.Get().Attributes.ScaldingThreshold.Id, (float)global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_SCALDING, global::STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.NAME, false, false));
		Tag[] array = new Tag[] { GameTags.Suit };
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef("Atmo_Suit", global::TUNING.EQUIPMENT.SUITS.SLOT, global::TUNING.EQUIPMENT.SUITS.FABRICATOR, (float)global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME, SimHashes.Dirt, dictionary, (float)global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_MASS, "suit_oxygen_kanim", string.Empty, "body_oxygen_kanim", list, null, true, EntityTemplates.CollisionShape.CIRCLE, 0.325f, 0.325f, array);
		equipmentDef.RecipeDescription = global::STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.RECIPE_DESC;
		equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("SoakingWet"));
		equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("WetFeet"));
		equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("PoppedEarDrums"));
		equipmentDef.EffectImmunites.Add(Db.Get().effects.Get("Unclean"));
		string helmet_name = "helmet_name";
		equipmentDef.OnEquipCallBack = delegate(Equippable eq)
		{
			Grid.SceneLayer sceneLayer = Grid.SceneLayer.Move;
			GameObject gameObject = new GameObject(helmet_name);
			gameObject.SetActive(false);
			KPrefabID kprefabID = gameObject.AddComponent<KPrefabID>();
			PrimaryElement primaryElement = gameObject.AddComponent<PrimaryElement>();
			primaryElement.ElementID = eq.GetComponent<PrimaryElement>().ElementID;
			primaryElement.Temperature = eq.GetComponent<PrimaryElement>().Temperature;
			kprefabID.PrefabTag = GameTags.Helmet;
			HashedString hashedString = new HashedString("snapto_neck");
			gameObject.transform.parent = eq.assignee.GetSoleOwner().transform;
			gameObject.transform.localPosition = new Vector3(0f, 0f, Grid.GetLayerZ(sceneLayer));
			KBatchedAnimController kbatchedAnimController = gameObject.AddComponent<KBatchedAnimController>();
			kbatchedAnimController.SetAnims(new KAnimFile[] { Assets.GetAnim("body_comp_default_kanim") }, true);
			KAnimFile anim = Assets.GetAnim("helm_oxygen_kanim");
			KAnim.Build.Symbol symbol = anim.GetData().build.symbols[0];
			foreach (KAnim.Build.Symbol symbol2 in anim.GetData().build.symbols)
			{
				if (symbol2.hash.HashValue == hashedString.HashValue)
				{
					symbol = symbol2;
					break;
				}
			}
			kbatchedAnimController.AddSymbolOverride(hashedString, anim.batchTag, symbol, false);
			kbatchedAnimController.ShowSymbol(hashedString);
			kbatchedAnimController.isMovable = true;
			kbatchedAnimController.sceneLayer = sceneLayer;
			kbatchedAnimController.Play("ah", KAnim.PlayMode.Once, 1f, 0f);
			primaryElement.ForcePermanentDiseaseContainer(true);
			primaryElement.SetDiseaseVisualProvider(eq.gameObject);
			KBatchedAnimTracker kbatchedAnimTracker = gameObject.AddComponent<KBatchedAnimTracker>();
			kbatchedAnimTracker.symbol = new HashedString("snapTo_headshape");
			kbatchedAnimTracker.offset = new Vector3(0f, 0f, 0f);
			gameObject.SetActive(true);
			eq.assignee.GetSoleOwner().GetComponent<Navigator>().SetFlags(PathFinder.PotentialPath.Flags.HasSuit | PathFinder.PotentialPath.Flags.UnlimitedSubmergedTravel);
		};
		equipmentDef.OnUnequipCallBack = delegate(Equippable eq)
		{
			eq.assignee.GetSoleOwner().GetComponent<Navigator>().ClearFlags(PathFinder.PotentialPath.Flags.HasSuit | PathFinder.PotentialPath.Flags.UnlimitedSubmergedTravel);
			foreach (object obj in eq.assignee.GetSoleOwner().transform)
			{
				Transform transform = (Transform)obj;
				if (!(transform == null))
				{
					if (transform.name == helmet_name)
					{
						Util.KDestroyGameObject(transform.gameObject);
					}
				}
			}
		};
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "Atmo_Suit");
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "Helmet");
		return equipmentDef;
	}

	public void DoPostConfigure(GameObject go)
	{
		SuitTank suitTank = go.AddComponent<SuitTank>();
		suitTank.element = "Oxygen";
		suitTank.capacity = 75f;
		go.AddComponent<SuitDiseaseHandler>();
	}

	public const string ID = "Atmo_Suit";
}
