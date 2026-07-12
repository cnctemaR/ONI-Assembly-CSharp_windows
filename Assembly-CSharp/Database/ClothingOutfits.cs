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
			this.Add("BasicBlack", new string[] { "TopBasicBlack", "BottomBasicBlack", "GlovesBasicBlack", "ShoesBasicBlack" }, UI.OUTFITS.BASIC_BLACK.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("BasicWhite", new string[] { "TopBasicWhite", "BottomBasicWhite", "GlovesBasicWhite", "ShoesBasicWhite" }, UI.OUTFITS.BASIC_WHITE.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("BasicRed", new string[] { "TopBasicRed", "BottomBasicRed", "GlovesBasicRed", "ShoesBasicRed" }, UI.OUTFITS.BASIC_RED.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("BasicOrange", new string[] { "TopBasicOrange", "BottomBasicOrange", "GlovesBasicOrange", "ShoesBasicOrange" }, UI.OUTFITS.BASIC_ORANGE.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("BasicYellow", new string[] { "TopBasicYellow", "BottomBasicYellow", "GlovesBasicYellow", "ShoesBasicYellow" }, UI.OUTFITS.BASIC_YELLOW.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("BasicGreen", new string[] { "TopBasicGreen", "BottomBasicGreen", "GlovesBasicGreen", "ShoesBasicGreen" }, UI.OUTFITS.BASIC_GREEN.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("BasicAqua", new string[] { "TopBasicAqua", "BottomBasicAqua", "GlovesBasicAqua", "ShoesBasicAqua" }, UI.OUTFITS.BASIC_AQUA.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("BasicPurple", new string[] { "TopBasicPurple", "BottomBasicPurple", "GlovesBasicPurple", "ShoesBasicPurple" }, UI.OUTFITS.BASIC_PURPLE.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("BasicPinkOrchid", new string[] { "TopBasicPinkOrchid", "BottomBasicPinkOrchid", "GlovesBasicPinkOrchid", "ShoesBasicPinkOrchid" }, UI.OUTFITS.BASIC_PINK_ORCHID.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("BasicDeepRed", new string[] { "TopRaglanDeepRed", "ShortsBasicDeepRed", "GlovesAthleticRedDeep", "SocksAthleticDeepRed" }, UI.OUTFITS.BASIC_DEEPRED.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("BasicOrangeSatsuma", new string[] { "TopRaglanSatsuma", "ShortsBasicSatsuma", "GlovesAthleticOrangeSatsuma", "SocksAthleticOrangeSatsuma" }, UI.OUTFITS.BASIC_SATSUMA.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("BasicLemon", new string[] { "TopRaglanLemon", "ShortsBasicYellowcake", "GlovesAthleticYellowLemon", "SocksAthleticYellowLemon" }, UI.OUTFITS.BASIC_LEMON.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("BasicBlueCobalt", new string[] { "TopRaglanCobalt", "ShortsBasicBlueCobalt", "GlovesAthleticBlueCobalt", "SocksAthleticBlueCobalt" }, UI.OUTFITS.BASIC_BLUE_COBALT.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("BasicGreenKelly", new string[] { "TopRaglanKellyGreen", "ShortsBasicKellyGreen", "GlovesAthleticGreenKelly", "SocksAthleticGreenKelly" }, UI.OUTFITS.BASIC_GREEN_KELLY.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("BasicPinkFlamingo", new string[] { "TopRaglanFlamingo", "ShortsBasicPinkFlamingo", "GlovesAthleticPinkFlamingo", "SocksAthleticPinkFlamingo" }, UI.OUTFITS.BASIC_PINK_FLAMINGO.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("BasicGreyCharcoal", new string[] { "TopRaglanCharcoal", "ShortsBasicCharcoal", "GlovesAthleticGreyCharcoal", "SocksAthleticGreyCharcoal" }, UI.OUTFITS.BASIC_GREY_CHARCOAL.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("JellypuffBlueberry", new string[] { "TopJellypuffJacketBlueberry", "GlovesCufflessBlueberry" }, UI.OUTFITS.JELLYPUFF_BLUEBERRY.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("JellypuffGrape", new string[] { "TopJellypuffJacketGrape", "GlovesCufflessGrape" }, UI.OUTFITS.JELLYPUFF_GRAPE.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("JellypuffLemon", new string[] { "TopJellypuffJacketLemon", "GlovesCufflessLemon" }, UI.OUTFITS.JELLYPUFF_LEMON.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("JellypuffLime", new string[] { "TopJellypuffJacketLime", "GlovesCufflessLime" }, UI.OUTFITS.JELLYPUFF_LIME.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("JellypuffSatsuma", new string[] { "TopJellypuffJacketSatsuma", "GlovesCufflessSatsuma" }, UI.OUTFITS.JELLYPUFF_SATSUMA.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("JellypuffStrawberry", new string[] { "TopJellypuffJacketStrawberry", "GlovesCufflessStrawberry" }, UI.OUTFITS.JELLYPUFF_STRAWBERRY.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("JellypuffWatermelon", new string[] { "TopJellypuffJacketWatermelon", "GlovesCufflessWatermelon" }, UI.OUTFITS.JELLYPUFF_WATERMELON.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("Athlete", new string[] { "TopAthlete", "PantsAthlete", "GlovesAthlete", "ShoesBasicBlack" }, UI.OUTFITS.ATHLETE.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("Circuit", new string[] { "TopCircuitGreen", "PantsCircuitGreen", "GlovesCircuitGreen" }, UI.OUTFITS.CIRCUIT.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("AtmoLimone", new string[] { "AtmoHelmetLimone", "AtmoSuitBasicYellow", "AtmoGlovesLime", "AtmoBeltBasicLime", "AtmoShoesBasicYellow" }, UI.OUTFITS.ATMOSUIT_LIMONE.NAME, ClothingOutfitUtility.OutfitType.AtmoSuit);
			this.Add("AtmoPuft", new string[] { "AtmoHelmetPuft", "AtmoSuitPuft", "AtmoGlovesPuft", "AtmoBeltPuft", "AtmoShoesPuft" }, UI.OUTFITS.ATMOSUIT_PUFT.NAME, ClothingOutfitUtility.OutfitType.AtmoSuit);
			this.Add("AtmoSparkleRed", new string[] { "AtmoHelmetSparkleRed", "AtmoSuitSparkleRed", "AtmoGlovesSparkleRed", "AtmoBeltSparkleRed", "AtmoShoesSparkleBlack" }, UI.OUTFITS.ATMOSUIT_SPARKLE_RED.NAME, ClothingOutfitUtility.OutfitType.AtmoSuit);
			this.Add("AtmoSparkleBlue", new string[] { "AtmoHelmetSparkleBlue", "AtmoSuitSparkleBlue", "AtmoGlovesSparkleBlue", "AtmoBeltSparkleBlue", "AtmoShoesSparkleBlack" }, UI.OUTFITS.ATMOSUIT_SPARKLE_BLUE.NAME, ClothingOutfitUtility.OutfitType.AtmoSuit);
			this.Add("AtmoSparkleGreen", new string[] { "AtmoHelmetSparkleGreen", "AtmoSuitSparkleGreen", "AtmoGlovesSparkleGreen", "AtmoBeltSparkleGreen", "AtmoShoesSparkleBlack" }, UI.OUTFITS.ATMOSUIT_SPARKLE_GREEN.NAME, ClothingOutfitUtility.OutfitType.AtmoSuit);
			this.Add("AtmoSparkleLavender", new string[] { "AtmoHelmetSparklePurple", "AtmoSuitSparkleLavender", "AtmoGlovesSparkleLavender", "AtmoBeltSparkleLavender", "AtmoShoesSparkleBlack" }, UI.OUTFITS.ATMOSUIT_SPARKLE_LAVENDER.NAME, ClothingOutfitUtility.OutfitType.AtmoSuit);
			this.Add("AtmoConfetti", new string[] { "AtmoHelmetConfetti", "AtmoSuitConfetti", "AtmoGlovesGold", "AtmoBeltBasicGold", "AtmoShoesStealth" }, UI.OUTFITS.ATMOSUIT_CONFETTI.NAME, ClothingOutfitUtility.OutfitType.AtmoSuit);
			this.Add("AtmoEggplant", new string[] { "AtmoHelmetEggplant", "AtmoSuitCrispEggplant", "AtmoGlovesEggplant", "AtmoBeltEggplant", "AtmoShoesEggplant" }, UI.OUTFITS.ATMOSUIT_BASIC_PURPLE.NAME, ClothingOutfitUtility.OutfitType.AtmoSuit);
			this.Add("CanadianTuxedo", new string[] { "TopDenimBlue", "PantsJeans", "GlovesDenimBlue", "ShoesDenimBlue" }, UI.OUTFITS.CANUXTUX.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("Researcher", new string[] { "TopResearcher", "PantsResearch", "GlovesBasicBrownKhaki", "ShoesBasicGray" }, UI.OUTFITS.NERD.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("UndiesExec", new string[] { "TopUndershirtExecutive", "BottomBriefsExecutive" }, UI.OUTFITS.GONCHIES_STRAWBERRY.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("UndiesUnderling", new string[] { "TopUndershirtUnderling", "BottomBriefsUnderling" }, UI.OUTFITS.GONCHIES_SATSUMA.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("UndiesGroupthink", new string[] { "TopUndershirtGroupthink", "BottomBriefsGroupthink" }, UI.OUTFITS.GONCHIES_LEMON.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("UndiesStakeholder", new string[] { "TopUndershirtStakeholder", "BottomBriefsStakeholder" }, UI.OUTFITS.GONCHIES_LIME.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("UndiesAdmin", new string[] { "TopUndershirtAdmin", "BottomBriefsAdmin" }, UI.OUTFITS.GONCHIES_BLUEBERRY.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("UndiesBuzzword", new string[] { "TopUndershirtBuzzword", "BottomBriefsBuzzword" }, UI.OUTFITS.GONCHIES_GRAPE.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("UndiesSynergy", new string[] { "TopUndershirtSynergy", "BottomBriefsSynergy" }, UI.OUTFITS.GONCHIES_WATERMELON.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			this.Add("RebelGiOutfit", new string[] { "TopRebelGi", "PantsRebelGi", "GlovesCufflessBlack" }, UI.OUTFITS.REBELGI.NAME, ClothingOutfitUtility.OutfitType.Clothing);
			ClothingOutfitUtility.LoadClothingOutfitData(this);
		}

		public void Add(string id, string[] items_in_outfit, LocString name, ClothingOutfitUtility.OutfitType outfitType)
		{
			ClothingOutfitResource clothingOutfitResource = new ClothingOutfitResource(id, items_in_outfit, name, outfitType);
			this.resources.Add(clothingOutfitResource);
		}

		public void SetDuplicantPersonalityOutfit(string personalityId, Option<string> outfit_id, ClothingOutfitUtility.OutfitType outfit_type)
		{
			Db.Get().Personalities.Get(personalityId).Internal_SetSelectedTemplateOutfitId(outfit_type, outfit_id);
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
