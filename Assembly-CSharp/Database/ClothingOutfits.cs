using System;
using System.Collections.Generic;
using STRINGS;

namespace Database
{
	public class ClothingOutfits : ResourceSet<ClothingOutfitResource>
	{
		public ClothingOutfits(ResourceSet parent, ClothingItems items_resource)
			: base("ClothingOutfits", parent)
		{
			base.Initialize();
			this.Add("BasicBlack", new string[] { "TopBasicBlack", "BottomBasicBlack", "GlovesBasicBlack", "ShoesBasicBlack" }, UI.OUTFITS.BASIC_BLACK.NAME);
			this.Add("BasicWhite", new string[] { "TopBasicWhite", "BottomBasicWhite", "GlovesBasicWhite", "ShoesBasicWhite" }, UI.OUTFITS.BASIC_WHITE.NAME);
			this.Add("BasicRed", new string[] { "TopBasicRed", "BottomBasicRed", "GlovesBasicRed", "ShoesBasicRed" }, UI.OUTFITS.BASIC_RED.NAME);
			this.Add("BasicOrange", new string[] { "TopBasicOrange", "BottomBasicOrange", "GlovesBasicOrange", "ShoesBasicOrange" }, UI.OUTFITS.BASIC_ORANGE.NAME);
			this.Add("BasicYellow", new string[] { "TopBasicYellow", "BottomBasicYellow", "GlovesBasicYellow", "ShoesBasicYellow" }, UI.OUTFITS.BASIC_YELLOW.NAME);
			this.Add("BasicGreen", new string[] { "TopBasicGreen", "BottomBasicGreen", "GlovesBasicGreen", "ShoesBasicGreen" }, UI.OUTFITS.BASIC_GREEN.NAME);
			this.Add("BasicAqua", new string[] { "TopBasicAqua", "BottomBasicAqua", "GlovesBasicAqua", "ShoesBasicAqua" }, UI.OUTFITS.BASIC_AQUA.NAME);
			this.Add("BasicPurple", new string[] { "TopBasicPurple", "BottomBasicPurple", "GlovesBasicPurple", "ShoesBasicPurple" }, UI.OUTFITS.BASIC_PURPLE.NAME);
			this.Add("BasicPinkOrchid", new string[] { "TopBasicPinkOrchid", "BottomBasicPinkOrchid", "GlovesBasicPinkOrchid", "ShoesBasicPinkOrchid" }, UI.OUTFITS.BASIC_PINK_ORCHID.NAME);
			this.Add("BasicDeepRed", new string[] { "TopRaglanDeepRed", "ShortsBasicDeepRed", "GlovesAthleticRedDeep", "SocksAthleticDeepRed" }, UI.OUTFITS.BASIC_DEEPRED.NAME);
			this.Add("BasicOrangeSatsuma", new string[] { "TopRaglanSatsuma", "ShortsBasicSatsuma", "GlovesAthleticOrangeSatsuma", "SocksAthleticOrangeSatsuma" }, UI.OUTFITS.BASIC_SATSUMA.NAME);
			this.Add("BasicLemon", new string[] { "TopRaglanLemon", "ShortsBasicYellowcake", "GlovesAthleticYellowLemon", "SocksAthleticYellowLemon" }, UI.OUTFITS.BASIC_LEMON.NAME);
			this.Add("BasicBlueCobalt", new string[] { "TopRaglanCobalt", "ShortsBasicBlueCobalt", "GlovesAthleticBlueCobalt", "SocksAthleticBlueCobalt" }, UI.OUTFITS.BASIC_BLUE_COBALT.NAME);
			this.Add("BasicGreenKelly", new string[] { "TopRaglanKellyGreen", "ShortsBasicKellyGreen", "GlovesAthleticGreenKelly", "SocksAthleticGreenKelly" }, UI.OUTFITS.BASIC_GREEN_KELLY.NAME);
			this.Add("BasicPinkFlamingo", new string[] { "TopRaglanFlamingo", "ShortsBasicPinkFlamingo", "GlovesAthleticPinkFlamingo", "SocksAthleticPinkFlamingo" }, UI.OUTFITS.BASIC_PINK_FLAMINGO.NAME);
			this.Add("BasicGreyCharcoal", new string[] { "TopRaglanCharcoal", "ShortsBasicCharcoal", "GlovesAthleticGreyCharcoal", "SocksAthleticGreyCharcoal" }, UI.OUTFITS.BASIC_GREY_CHARCOAL.NAME);
			ClothingOutfitUtility.LoadClothingOutfitData(this);
		}

		public void Add(string id, string[] items_in_outfit, LocString name)
		{
			ClothingOutfitResource clothingOutfitResource = new ClothingOutfitResource(id, items_in_outfit, name);
			this.resources.Add(clothingOutfitResource);
		}

		public void SetDuplicantPersonalityOutfit(string personalityId, Option<string> outfit_id, ClothingOutfitUtility.OutfitType outfit_type = ClothingOutfitUtility.OutfitType.Clothing)
		{
			Db.Get().Personalities.Get(personalityId).Internal_SetOutfit(outfit_type, outfit_id);
			CustomClothingOutfits.Instance.Internal_SetDuplicantPersonalityOutfit(personalityId, outfit_id, outfit_type);
		}

		public class ClothingOutfitInfo
		{
			public string id { get; set; }

			public string name { get; set; }

			public List<ClothingOutfits.ClothingOutfitInfo.ClothingItem> items { get; set; }

			public class ClothingItem
			{
				public string id { get; set; }

				public string name { get; set; }

				public string description { get; set; }

				public string category { get; set; }

				public string animFilename { get; set; }
			}
		}
	}
}
