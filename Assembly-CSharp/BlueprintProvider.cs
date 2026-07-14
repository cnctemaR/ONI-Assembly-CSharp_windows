using System;
using System.Collections.Generic;
using Database;

public abstract class BlueprintProvider : IHasDlcRestrictions
{
	protected void AddBuilding(string prefabConfigId, PermitRarity rarity, string permitId, string animFile)
	{
		this.blueprintCollection.buildingFacades.Add(new BuildingFacadeInfo(permitId, Strings.Get("STRINGS.BLUEPRINTS." + permitId.ToUpper() + ".NAME"), Strings.Get("STRINGS.BLUEPRINTS." + permitId.ToUpper() + ".DESC"), rarity, prefabConfigId, animFile, null, this.requiredDlcIds, this.forbiddenDlcIds, null));
	}

	protected void AddBuildingWithData(string prefabConfigId, PermitRarity rarity, string permitId, string animFile, Dictionary<string, string> data)
	{
		this.blueprintCollection.buildingFacades.Add(new BuildingFacadeInfo(permitId, Strings.Get("STRINGS.BLUEPRINTS." + permitId.ToUpper() + ".NAME"), Strings.Get("STRINGS.BLUEPRINTS." + permitId.ToUpper() + ".DESC"), rarity, prefabConfigId, animFile, null, this.requiredDlcIds, this.forbiddenDlcIds, data));
	}

	protected void AddBuildingWithInteract(string prefabConfigId, PermitRarity rarity, string permitId, string animFile, Dictionary<string, string> interact_anim)
	{
		this.blueprintCollection.buildingFacades.Add(new BuildingFacadeInfo(permitId, Strings.Get("STRINGS.BLUEPRINTS." + permitId.ToUpper() + ".NAME"), Strings.Get("STRINGS.BLUEPRINTS." + permitId.ToUpper() + ".DESC"), rarity, prefabConfigId, animFile, interact_anim, this.requiredDlcIds, this.forbiddenDlcIds, null));
	}

	protected void AddClothing(BlueprintProvider.ClothingType clothingType, PermitRarity rarity, string permitId, string animFile)
	{
		this.blueprintCollection.clothingItems.Add(new ClothingItemInfo(permitId, Strings.Get("STRINGS.BLUEPRINTS." + permitId.ToUpper() + ".NAME"), Strings.Get("STRINGS.BLUEPRINTS." + permitId.ToUpper() + ".DESC"), (PermitCategory)clothingType, rarity, animFile, this.requiredDlcIds, this.forbiddenDlcIds));
	}

	protected BlueprintProvider.ArtableInfoAuthoringHelper AddArtable(BlueprintProvider.ArtableType artableType, PermitRarity rarity, string permitId, string animFile)
	{
		string text;
		switch (artableType)
		{
		case BlueprintProvider.ArtableType.Painting:
			text = "Canvas";
			break;
		case BlueprintProvider.ArtableType.PaintingTall:
			text = "CanvasTall";
			break;
		case BlueprintProvider.ArtableType.PaintingWide:
			text = "CanvasWide";
			break;
		case BlueprintProvider.ArtableType.Sculpture:
			text = "Sculpture";
			break;
		case BlueprintProvider.ArtableType.SculptureSmall:
			text = "SmallSculpture";
			break;
		case BlueprintProvider.ArtableType.SculptureIce:
			text = "IceSculpture";
			break;
		case BlueprintProvider.ArtableType.SculptureMetal:
			text = "MetalSculpture";
			break;
		case BlueprintProvider.ArtableType.SculptureMarble:
			text = "MarbleSculpture";
			break;
		case BlueprintProvider.ArtableType.SculptureWood:
			text = "WoodSculpture";
			break;
		case BlueprintProvider.ArtableType.FossilSculpture:
			text = "FossilSculpture";
			break;
		case BlueprintProvider.ArtableType.CeilingFossilSculpture:
			text = "CeilingFossilSculpture";
			break;
		default:
			text = null;
			break;
		}
		bool flag = true;
		if (text == null)
		{
			DebugUtil.DevAssert(false, "Failed to get buildingConfigId from " + artableType.ToString(), null);
			flag = false;
		}
		BlueprintProvider.ArtableInfoAuthoringHelper artableInfoAuthoringHelper;
		if (flag)
		{
			KAnimFile kanimFile;
			ArtableInfo artableInfo = new ArtableInfo(permitId, Strings.Get("STRINGS.BLUEPRINTS." + permitId.ToUpper() + ".NAME"), Strings.Get("STRINGS.BLUEPRINTS." + permitId.ToUpper() + ".DESC"), rarity, animFile, (!Assets.TryGetAnim(animFile, out kanimFile)) ? null : kanimFile.GetData().GetAnim(0).name, 0, false, "error", text, "", this.requiredDlcIds, this.forbiddenDlcIds);
			artableInfoAuthoringHelper = new BlueprintProvider.ArtableInfoAuthoringHelper(artableType, artableInfo);
			artableInfoAuthoringHelper.Quality(BlueprintProvider.ArtableQuality.LookingGreat);
			this.blueprintCollection.artables.Add(artableInfo);
		}
		else
		{
			artableInfoAuthoringHelper = default(BlueprintProvider.ArtableInfoAuthoringHelper);
		}
		return artableInfoAuthoringHelper;
	}

	protected void AddJoyResponse(BlueprintProvider.JoyResponseType joyResponseType, PermitRarity rarity, string permitId, string animFile)
	{
		if (joyResponseType == BlueprintProvider.JoyResponseType.BallonSet)
		{
			this.blueprintCollection.balloonArtistFacades.Add(new BalloonArtistFacadeInfo(permitId, Strings.Get("STRINGS.BLUEPRINTS." + permitId.ToUpper() + ".NAME"), Strings.Get("STRINGS.BLUEPRINTS." + permitId.ToUpper() + ".DESC"), rarity, animFile, BalloonArtistFacadeType.ThreeSet, this.requiredDlcIds, this.forbiddenDlcIds));
			return;
		}
		throw new NotImplementedException("Missing case for " + joyResponseType.ToString());
	}

	protected void AddOutfit(BlueprintProvider.OutfitType outfitType, string outfitId, string[] permitIdList)
	{
		this.blueprintCollection.outfits.Add(new ClothingOutfitResource(outfitId, permitIdList, Strings.Get("STRINGS.BLUEPRINTS." + outfitId.ToUpper() + ".NAME"), (ClothingOutfitUtility.OutfitType)outfitType, this.requiredDlcIds, this.forbiddenDlcIds));
	}

	protected void AddMonumentPart(BlueprintProvider.MonumentPart part, PermitRarity rarity, string permitId, string animFile)
	{
		string text = "";
		switch (part)
		{
		case BlueprintProvider.MonumentPart.Bottom:
			text = "base";
			break;
		case BlueprintProvider.MonumentPart.Middle:
			text = "mid";
			break;
		case BlueprintProvider.MonumentPart.Top:
			text = "top";
			break;
		}
		this.blueprintCollection.monumentParts.Add(new MonumentPartInfo(permitId, Strings.Get("STRINGS.BLUEPRINTS." + permitId.ToUpper() + ".NAME"), Strings.Get("STRINGS.BLUEPRINTS." + permitId.ToUpper() + ".DESC"), rarity, animFile, permitId.Replace("permit_", ""), text, (MonumentPartResource.Part)part, this.requiredDlcIds, this.forbiddenDlcIds));
	}

	public virtual string[] GetRequiredDlcIds()
	{
		return this.requiredDlcIds;
	}

	public virtual string[] GetForbiddenDlcIds()
	{
		return this.forbiddenDlcIds;
	}

	public abstract void SetupBlueprints();

	public void Internal_PreSetupBlueprints()
	{
		this.requiredDlcIds = this.GetRequiredDlcIds();
		this.forbiddenDlcIds = this.GetForbiddenDlcIds();
	}

	public BlueprintCollection blueprintCollection;

	private string[] requiredDlcIds;

	private string[] forbiddenDlcIds;

	public enum ArtableType
	{
		Painting,
		PaintingTall,
		PaintingWide,
		Sculpture,
		SculptureSmall,
		SculptureIce,
		SculptureMetal,
		SculptureMarble,
		SculptureWood,
		FossilSculpture,
		CeilingFossilSculpture
	}

	public enum ArtableQuality
	{
		LookingGreat,
		LookingOkay,
		LookingUgly
	}

	public enum ClothingType
	{
		DupeTops = 1,
		DupeBottoms,
		DupeGloves,
		DupeShoes,
		DupeHats,
		DupeAccessories,
		AtmoSuitHelmet,
		AtmoSuitBody,
		AtmoSuitGloves,
		AtmoSuitBelt,
		AtmoSuitShoes,
		JetSuitHelmet = 18,
		JetSuitBody,
		JetSuitGloves,
		JetSuitShoes
	}

	public enum OutfitType
	{
		Clothing,
		AtmoSuit = 2,
		JetSuit
	}

	public enum JoyResponseType
	{
		BallonSet
	}

	public enum MonumentPart
	{
		Bottom,
		Top = 2,
		Middle = 1
	}

	protected readonly ref struct ArtableInfoAuthoringHelper
	{
		public ArtableInfoAuthoringHelper(BlueprintProvider.ArtableType artableType, ArtableInfo artableInfo)
		{
			this.artableType = artableType;
			this.artableInfo = artableInfo;
		}

		public void Quality(BlueprintProvider.ArtableQuality artableQuality)
		{
			if (this.artableInfo == null)
			{
				return;
			}
			int num;
			int num2;
			int num3;
			if (this.artableType == BlueprintProvider.ArtableType.SculptureWood)
			{
				num = 4;
				num2 = 8;
				num3 = 12;
			}
			else
			{
				num = 5;
				num2 = 10;
				num3 = 15;
			}
			int num4;
			bool flag;
			string text;
			switch (artableQuality)
			{
			case BlueprintProvider.ArtableQuality.LookingGreat:
				num4 = num3;
				flag = true;
				text = "LookingGreat";
				break;
			case BlueprintProvider.ArtableQuality.LookingOkay:
				num4 = num2;
				flag = false;
				text = "LookingOkay";
				break;
			case BlueprintProvider.ArtableQuality.LookingUgly:
				num4 = num;
				flag = false;
				text = "LookingUgly";
				break;
			default:
				throw new ArgumentException();
			}
			this.artableInfo.decor_value = num4;
			this.artableInfo.cheer_on_complete = flag;
			this.artableInfo.status_id = text;
		}

		private readonly BlueprintProvider.ArtableType artableType;

		private readonly ArtableInfo artableInfo;
	}
}
