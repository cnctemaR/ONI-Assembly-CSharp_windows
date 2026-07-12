using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Database;
using UnityEngine;
using UnityEngine.UI;

public class KleiPermitDioramaVis : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		this.Init();
	}

	private void Init()
	{
		if (this.initComplete)
		{
			return;
		}
		this.allVisList = ReflectionUtil.For<KleiPermitDioramaVis>(this).CollectValuesForFieldsThatInheritOrImplement<IKleiPermitDioramaVisTarget>(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		foreach (IKleiPermitDioramaVisTarget kleiPermitDioramaVisTarget in this.allVisList)
		{
			kleiPermitDioramaVisTarget.ConfigureSetup();
		}
		this.initComplete = true;
	}

	public void ConfigureWith(PermitResource permit)
	{
		if (!this.initComplete)
		{
			this.Init();
		}
		foreach (IKleiPermitDioramaVisTarget kleiPermitDioramaVisTarget in this.allVisList)
		{
			kleiPermitDioramaVisTarget.GetGameObject().SetActive(false);
		}
		KleiPermitVisUtil.ClearAnimation();
		IKleiPermitDioramaVisTarget permitVisTarget = this.GetPermitVisTarget(permit);
		permitVisTarget.GetGameObject().SetActive(true);
		permitVisTarget.ConfigureWith(permit);
		string dlcIdFrom = permit.GetDlcIdFrom();
		if (DlcManager.IsDlcId(dlcIdFrom))
		{
			this.dlcImage.gameObject.SetActive(true);
			this.dlcImage.sprite = Assets.GetSprite(DlcManager.GetDlcSmallLogo(dlcIdFrom));
			return;
		}
		this.dlcImage.gameObject.SetActive(false);
	}

	private IKleiPermitDioramaVisTarget GetPermitVisTarget(PermitResource permit)
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
			BuildLocationRule? buildLocationRule = KleiPermitVisUtil.GetBuildLocationRule(permit);
			if (buildLocationRule == null)
			{
				if (permit.DlcIds.SequenceEqual<string>(DlcManager.AVAILABLE_EXPANSION1_ONLY))
				{
					return this.buildingOnFloorVis;
				}
				return this.fallbackVis.WithError("Couldn't get BuildLocationRule on permit with id \"" + permit.Id + "\"");
			}
			else
			{
				BuildingDef buildingDef = KleiPermitVisUtil.GetBuildingDef(permit);
				if (!buildingDef.BuildingComplete.GetComponent<Bed>().IsNullOrDestroyed())
				{
					return this.buildingOnFloorVis;
				}
				BuildingFacadeResource buildingFacadeResource = permit as BuildingFacadeResource;
				if (buildingFacadeResource != null)
				{
					if (buildingFacadeResource.PrefabID.Contains("Wire") || buildingFacadeResource.PrefabID.Contains("Ribbon"))
					{
						return this.buildingWiresAndAutomationVis;
					}
					if (buildingFacadeResource.PrefabID.Contains("Logic"))
					{
						return this.buildingAutomationGatesVis;
					}
				}
				if (buildingDef.PrefabID == "RockCrusher" || buildingDef.PrefabID == "GasReservoir" || buildingDef.PrefabID == "ArcadeMachine" || buildingDef.PrefabID == "MicrobeMusher" || buildingDef.PrefabID == "FlushToilet" || buildingDef.PrefabID == "WashSink" || buildingDef.PrefabID == "Headquarters" || buildingDef.PrefabID == "GourmetCookingStation")
				{
					return this.buildingOnFloorBigVis;
				}
				if (!buildingDef.BuildingComplete.GetComponent<RocketModule>().IsNullOrDestroyed() || !buildingDef.BuildingComplete.GetComponent<RocketEngine>().IsNullOrDestroyed())
				{
					return this.buildingRocketVis;
				}
				if (buildingDef.PrefabID == "PlanterBox" || buildingDef.PrefabID == "FlowerVase")
				{
					return this.buildingOnFloorBotanicalVis;
				}
				if (buildingDef.PrefabID == "ExteriorWall")
				{
					return this.wallpaperVis;
				}
				if (buildingDef.PrefabID == "FlowerVaseHanging" || buildingDef.PrefabID == "FlowerVaseHangingFancy")
				{
					return this.buildingHangingHookBotanicalVis;
				}
				if (buildLocationRule != null)
				{
					BuildLocationRule valueOrDefault = buildLocationRule.GetValueOrDefault();
					switch (valueOrDefault)
					{
					case BuildLocationRule.OnFloor:
						break;
					case BuildLocationRule.OnFloorOverSpace:
						goto IL_02FC;
					case BuildLocationRule.OnCeiling:
						return this.buildingOnCeilingVis.WithAlignment(Alignment.Top());
					case BuildLocationRule.OnWall:
						return this.buildingOnWallVis.WithAlignment(Alignment.Left());
					case BuildLocationRule.InCorner:
						return this.buildingInCeilingCornerVis.WithAlignment(Alignment.TopLeft());
					default:
						if (valueOrDefault != BuildLocationRule.OnFoundationRotatable)
						{
							goto IL_02FC;
						}
						break;
					}
					return this.buildingOnFloorVis;
				}
				IL_02FC:
				return this.fallbackVis.WithError(string.Format("No visualization available for building with BuildLocationRule of {0}", buildLocationRule));
			}
		}
		else if (permit.Category == PermitCategory.Artwork)
		{
			BuildingDef buildingDef2 = KleiPermitVisUtil.GetBuildingDef(permit);
			if (buildingDef2.IsNullOrDestroyed())
			{
				return this.fallbackVis.WithError("Couldn't find building def for Artable " + permit.Id);
			}
			if (KleiPermitDioramaVis.<GetPermitVisTarget>g__Has|24_0<Sculpture>(buildingDef2))
			{
				if (buildingDef2.PrefabID == "WoodSculpture")
				{
					return this.artablePaintingVis;
				}
				return this.artableSculptureVis;
			}
			else
			{
				if (KleiPermitDioramaVis.<GetPermitVisTarget>g__Has|24_0<Painting>(buildingDef2))
				{
					return this.artablePaintingVis;
				}
				if (KleiPermitDioramaVis.<GetPermitVisTarget>g__Has|24_0<MonumentPart>(buildingDef2))
				{
					return this.monumentPartVis;
				}
				return this.fallbackVis.WithError("No visualization available for Artable " + permit.Id);
			}
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
	internal static bool <GetPermitVisTarget>g__Has|24_0<T>(BuildingDef buildingDef) where T : Component
	{
		return !buildingDef.BuildingComplete.GetComponent<T>().IsNullOrDestroyed();
	}

	[SerializeField]
	private Image dlcImage;

	[SerializeField]
	private KleiPermitDioramaVis_Fallback fallbackVis;

	[SerializeField]
	private KleiPermitDioramaVis_DupeEquipment equipmentVis;

	[SerializeField]
	private KleiPermitDioramaVis_BuildingOnFloor buildingOnFloorVis;

	[SerializeField]
	private KleiPermitDioramaVis_BuildingOnFloorBig buildingOnFloorBigVis;

	[SerializeField]
	private KleiPermitDioramaVis_BuildingPresentationStand buildingOnWallVis;

	[SerializeField]
	private KleiPermitDioramaVis_BuildingPresentationStand buildingOnCeilingVis;

	[SerializeField]
	private KleiPermitDioramaVis_BuildingPresentationStand buildingInCeilingCornerVis;

	[SerializeField]
	private KleiPermitDioramaVis_BuildingRocket buildingRocketVis;

	[SerializeField]
	private KleiPermitDioramaVis_BuildingOnFloor buildingOnFloorBotanicalVis;

	[SerializeField]
	private KleiPermitDioramaVis_BuildingHangingHook buildingHangingHookBotanicalVis;

	[SerializeField]
	private KleiPermitDioramaVis_WiresAndAutomation buildingWiresAndAutomationVis;

	[SerializeField]
	private KleiPermitDioramaVis_AutomationGates buildingAutomationGatesVis;

	[SerializeField]
	private KleiPermitDioramaVis_Wallpaper wallpaperVis;

	[SerializeField]
	private KleiPermitDioramaVis_ArtablePainting artablePaintingVis;

	[SerializeField]
	private KleiPermitDioramaVis_ArtableSculpture artableSculptureVis;

	[SerializeField]
	private KleiPermitDioramaVis_JoyResponseBalloon joyResponseBalloonVis;

	[SerializeField]
	private KleiPermitDioramaVis_MonumentPart monumentPartVis;

	private bool initComplete;

	private IReadOnlyList<IKleiPermitDioramaVisTarget> allVisList;

	public static PermitResource lastRenderedPermit;
}
