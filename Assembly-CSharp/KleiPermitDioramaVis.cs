using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Database;
using UnityEngine;

public class KleiPermitDioramaVis : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		this.allVisList = ReflectionUtil.For<KleiPermitDioramaVis>(this).CollectValuesForFieldsThatInheritOrImplement<IKleiPermitDioramaVisTarget>(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		foreach (IKleiPermitDioramaVisTarget kleiPermitDioramaVisTarget in this.allVisList)
		{
			kleiPermitDioramaVisTarget.ConfigureSetup();
		}
	}

	public void ConfigureWith(PermitResource permit)
	{
		foreach (IKleiPermitDioramaVisTarget kleiPermitDioramaVisTarget in this.allVisList)
		{
			kleiPermitDioramaVisTarget.GetGameObject().SetActive(false);
		}
		IKleiPermitDioramaVisTarget permitVisTarget = this.GetPermitVisTarget(permit);
		permitVisTarget.GetGameObject().SetActive(true);
		permitVisTarget.ConfigureWith(permit);
	}

	public IKleiPermitDioramaVisTarget GetPermitVisTarget(PermitResource permit)
	{
		KleiPermitDioramaVis.lastRenderedPermit = permit;
		if (permit == null)
		{
			return this.fallbackVis.WithError(string.Format("Given invalid permit: {0}", permit));
		}
		if (permit.Category == PermitCategory.Equipment || permit.Category == PermitCategory.DupeTops || permit.Category == PermitCategory.DupeBottoms || permit.Category == PermitCategory.DupeGloves || permit.Category == PermitCategory.DupeShoes || permit.Category == PermitCategory.DupeHats || permit.Category == PermitCategory.DupeAccessories || permit.Category == PermitCategory.AtmoSuitHelmet || permit.Category == PermitCategory.AtmoSuitBody || permit.Category == PermitCategory.AtmoSuitGloves || permit.Category == PermitCategory.AtmoSuitBelt || permit.Category == PermitCategory.AtmoSuitShoes)
		{
			return this.equipmentVis;
		}
		if (permit.Category == PermitCategory.Building)
		{
			bool flag;
			BuildLocationRule buildLocationRule;
			KleiPermitVisUtil.GetBuildLocationRule(permit).Deconstruct(out flag, out buildLocationRule);
			bool flag2 = flag;
			BuildLocationRule buildLocationRule2 = buildLocationRule;
			if (!flag2)
			{
				return this.fallbackVis.WithError("Couldn't get BuildLocationRule on permit with id \"" + permit.Id + "\"");
			}
			switch (buildLocationRule2)
			{
			case BuildLocationRule.OnFloor:
				return this.buildingOnFloorVis;
			case BuildLocationRule.OnCeiling:
			{
				string prefabID = KleiPermitVisUtil.GetBuildingDef(permit).Value.PrefabID;
				if (prefabID == "FlowerVaseHanging" || prefabID == "FlowerVaseHangingFancy")
				{
					return this.buildingHangingHookVis;
				}
				return this.buildingPresentationStandVis.WithAlignment(Alignment.Top());
			}
			case BuildLocationRule.OnWall:
				return this.buildingPresentationStandVis.WithAlignment(Alignment.Left());
			case BuildLocationRule.InCorner:
				return this.buildingPresentationStandVis.WithAlignment(Alignment.TopLeft());
			case BuildLocationRule.NotInTiles:
				return this.pedestalAndItemVis;
			}
			return this.fallbackVis.WithError(string.Format("No visualization available for building with BuildLocationRule of {0}", buildLocationRule2));
		}
		else if (permit.Category == PermitCategory.Artwork)
		{
			bool flag;
			BuildingDef buildingDef;
			KleiPermitVisUtil.GetBuildingDef(permit).Deconstruct(out flag, out buildingDef);
			bool flag3 = flag;
			BuildingDef buildingDef2 = buildingDef;
			if (!flag3)
			{
				return this.fallbackVis.WithError("Couldn't find building def for Artable " + permit.Id);
			}
			ArtableStage artableStage = (ArtableStage)permit;
			if (KleiPermitDioramaVis.<GetPermitVisTarget>g__Has|14_0<Sculpture>(buildingDef2))
			{
				return this.artableSculptureVis;
			}
			if (KleiPermitDioramaVis.<GetPermitVisTarget>g__Has|14_0<Painting>(buildingDef2))
			{
				return this.artablePaintingVis;
			}
			return this.fallbackVis.WithError("No visualization available for Artable " + permit.Id);
		}
		else
		{
			if (permit.Category != PermitCategory.JoyResponse)
			{
				return this.fallbackVis.WithError("No visualization has been defined for permit with id \"" + permit.Id + "\"");
			}
			if (permit is BalloonArtistFacadeResource)
			{
				return this.joyResponseBalloonVis;
			}
			return this.fallbackVis.WithError("No visualization available for JoyResponse " + permit.Id);
		}
	}

	public static Sprite GetDioramaBackground(PermitCategory permitCategory)
	{
		switch (permitCategory)
		{
		case PermitCategory.DupeTops:
		case PermitCategory.DupeBottoms:
		case PermitCategory.DupeGloves:
		case PermitCategory.DupeShoes:
		case PermitCategory.DupeHats:
		case PermitCategory.DupeAccessories:
			return Assets.GetSprite("screen_bg_clothing");
		case PermitCategory.AtmoSuitHelmet:
		case PermitCategory.AtmoSuitBody:
		case PermitCategory.AtmoSuitGloves:
		case PermitCategory.AtmoSuitBelt:
		case PermitCategory.AtmoSuitShoes:
			return Assets.GetSprite("screen_bg_atmosuit");
		case PermitCategory.Building:
			return Assets.GetSprite("screen_bg_buildings");
		case PermitCategory.Artwork:
			return Assets.GetSprite("screen_bg_art");
		case PermitCategory.JoyResponse:
			return Assets.GetSprite("screen_bg_joyresponse");
		}
		return null;
	}

	public static Sprite GetDioramaBackground(ClothingOutfitUtility.OutfitType outfitType)
	{
		switch (outfitType)
		{
		case ClothingOutfitUtility.OutfitType.Clothing:
			return Assets.GetSprite("screen_bg_clothing");
		case ClothingOutfitUtility.OutfitType.JoyResponse:
			return Assets.GetSprite("screen_bg_joyresponse");
		case ClothingOutfitUtility.OutfitType.AtmoSuit:
			return Assets.GetSprite("screen_bg_atmosuit");
		default:
			return null;
		}
	}

	[CompilerGenerated]
	internal static bool <GetPermitVisTarget>g__Has|14_0<T>(BuildingDef buildingDef) where T : Component
	{
		return !buildingDef.BuildingComplete.GetComponent<T>().IsNullOrDestroyed();
	}

	[SerializeField]
	private KleiPermitDioramaVis_Fallback fallbackVis;

	[SerializeField]
	private KleiPermitDioramaVis_DupeEquipment equipmentVis;

	[SerializeField]
	private KleiPermitDioramaVis_BuildingOnFloor buildingOnFloorVis;

	[SerializeField]
	private KleiPermitDioramaVis_BuildingPresentationStand buildingPresentationStandVis;

	[SerializeField]
	private KleiPermitDioramaVis_BuildingPresentationStandHanging buildingPresentationStandHangingVis;

	[SerializeField]
	private KleiPermitDioramaVis_BuildingHangingHook buildingHangingHookVis;

	[SerializeField]
	private KleiPermitDioramaVis_PedestalAndItem pedestalAndItemVis;

	[SerializeField]
	private KleiPermitDioramaVis_ArtablePainting artablePaintingVis;

	[SerializeField]
	private KleiPermitDioramaVis_ArtableSculpture artableSculptureVis;

	[SerializeField]
	private KleiPermitDioramaVis_JoyResponseBalloon joyResponseBalloonVis;

	private IReadOnlyList<IKleiPermitDioramaVisTarget> allVisList;

	public static PermitResource lastRenderedPermit;
}
