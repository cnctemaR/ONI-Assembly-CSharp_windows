using System;
using Database;
using STRINGS;

public class Blueprints_Default : BlueprintProvider
{
	public override void SetupBlueprints()
	{
		this.SetupBuildingFacades();
		this.SetupArtables();
		this.SetupClothingItems();
		this.SetupClothingOutfits();
		this.SetupBalloonArtistFacades();
		this.SetupStickerBombFacades();
		this.SetupEquippableFacades();
		this.SetupMonumentParts();
	}

	public void SetupBuildingFacades()
	{
	}

	private void SetupArtables()
	{
		this.blueprintCollection.artables.AddRange(new ArtableInfo[]
		{
			new ArtableInfo("Canvas_Bad", BUILDINGS.PREFABS.CANVAS.FACADES.ART_A.NAME, BUILDINGS.PREFABS.CANVAS.FACADES.ART_A.DESC, PermitRarity.Universal, "painting_art_a_kanim", "art_a", 5, false, "LookingUgly", "Canvas", "canvas", null, null),
			new ArtableInfo("Canvas_Average", BUILDINGS.PREFABS.CANVAS.FACADES.ART_B.NAME, BUILDINGS.PREFABS.CANVAS.FACADES.ART_B.DESC, PermitRarity.Universal, "painting_art_b_kanim", "art_b", 10, false, "LookingOkay", "Canvas", "canvas", null, null),
			new ArtableInfo("Canvas_Good", BUILDINGS.PREFABS.CANVAS.FACADES.ART_C.NAME, BUILDINGS.PREFABS.CANVAS.FACADES.ART_C.DESC, PermitRarity.Universal, "painting_art_c_kanim", "art_c", 15, true, "LookingGreat", "Canvas", "canvas", null, null),
			new ArtableInfo("Canvas_Good2", BUILDINGS.PREFABS.CANVAS.FACADES.ART_D.NAME, BUILDINGS.PREFABS.CANVAS.FACADES.ART_D.DESC, PermitRarity.Universal, "painting_art_d_kanim", "art_d", 15, true, "LookingGreat", "Canvas", "canvas", null, null),
			new ArtableInfo("Canvas_Good3", BUILDINGS.PREFABS.CANVAS.FACADES.ART_E.NAME, BUILDINGS.PREFABS.CANVAS.FACADES.ART_E.DESC, PermitRarity.Universal, "painting_art_e_kanim", "art_e", 15, true, "LookingGreat", "Canvas", "canvas", null, null),
			new ArtableInfo("Canvas_Good4", BUILDINGS.PREFABS.CANVAS.FACADES.ART_F.NAME, BUILDINGS.PREFABS.CANVAS.FACADES.ART_F.DESC, PermitRarity.Universal, "painting_art_f_kanim", "art_f", 15, true, "LookingGreat", "Canvas", "canvas", null, null),
			new ArtableInfo("Canvas_Good5", BUILDINGS.PREFABS.CANVAS.FACADES.ART_G.NAME, BUILDINGS.PREFABS.CANVAS.FACADES.ART_G.DESC, PermitRarity.Universal, "painting_art_g_kanim", "art_g", 15, true, "LookingGreat", "Canvas", "canvas", null, null),
			new ArtableInfo("Canvas_Good6", BUILDINGS.PREFABS.CANVAS.FACADES.ART_H.NAME, BUILDINGS.PREFABS.CANVAS.FACADES.ART_H.DESC, PermitRarity.Universal, "painting_art_h_kanim", "art_h", 15, true, "LookingGreat", "Canvas", "canvas", null, null),
			new ArtableInfo("CanvasTall_Bad", BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_A.NAME, BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_A.DESC, PermitRarity.Universal, "painting_tall_art_a_kanim", "art_a", 5, false, "LookingUgly", "CanvasTall", "canvas", null, null),
			new ArtableInfo("CanvasTall_Average", BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_B.NAME, BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_B.DESC, PermitRarity.Universal, "painting_tall_art_b_kanim", "art_b", 10, false, "LookingOkay", "CanvasTall", "canvas", null, null),
			new ArtableInfo("CanvasTall_Good", BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_C.NAME, BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_C.DESC, PermitRarity.Universal, "painting_tall_art_c_kanim", "art_c", 15, true, "LookingGreat", "CanvasTall", "canvas", null, null),
			new ArtableInfo("CanvasTall_Good2", BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_D.NAME, BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_D.DESC, PermitRarity.Universal, "painting_tall_art_d_kanim", "art_d", 15, true, "LookingGreat", "CanvasTall", "canvas", null, null),
			new ArtableInfo("CanvasTall_Good3", BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_E.NAME, BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_E.DESC, PermitRarity.Universal, "painting_tall_art_e_kanim", "art_e", 15, true, "LookingGreat", "CanvasTall", "canvas", null, null),
			new ArtableInfo("CanvasTall_Good4", BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_F.NAME, BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_F.DESC, PermitRarity.Universal, "painting_tall_art_f_kanim", "art_f", 15, true, "LookingGreat", "CanvasTall", "canvas", null, null),
			new ArtableInfo("CanvasWide_Bad", BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_A.NAME, BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_A.DESC, PermitRarity.Universal, "painting_wide_art_a_kanim", "art_a", 5, false, "LookingUgly", "CanvasWide", "canvas", null, null),
			new ArtableInfo("CanvasWide_Average", BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_B.NAME, BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_B.DESC, PermitRarity.Universal, "painting_wide_art_b_kanim", "art_b", 10, false, "LookingOkay", "CanvasWide", "canvas", null, null),
			new ArtableInfo("CanvasWide_Good", BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_C.NAME, BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_C.DESC, PermitRarity.Universal, "painting_wide_art_c_kanim", "art_c", 15, true, "LookingGreat", "CanvasWide", "canvas", null, null),
			new ArtableInfo("CanvasWide_Good2", BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_D.NAME, BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_D.DESC, PermitRarity.Universal, "painting_wide_art_d_kanim", "art_d", 15, true, "LookingGreat", "CanvasWide", "canvas", null, null),
			new ArtableInfo("CanvasWide_Good3", BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_E.NAME, BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_E.DESC, PermitRarity.Universal, "painting_wide_art_e_kanim", "art_e", 15, true, "LookingGreat", "CanvasWide", "canvas", null, null),
			new ArtableInfo("CanvasWide_Good4", BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_F.NAME, BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_F.DESC, PermitRarity.Universal, "painting_wide_art_f_kanim", "art_f", 15, true, "LookingGreat", "CanvasWide", "canvas", null, null),
			new ArtableInfo("Sculpture_Bad", BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_CRAP_1.NAME, BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_CRAP_1.DESC, PermitRarity.Universal, "sculpture_crap_1_kanim", "crap_1", 5, false, "LookingUgly", "Sculpture", "", null, null),
			new ArtableInfo("Sculpture_Average", BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_GOOD_1.NAME, BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_GOOD_1.DESC, PermitRarity.Universal, "sculpture_good_1_kanim", "good_1", 10, false, "LookingOkay", "Sculpture", "", null, null),
			new ArtableInfo("Sculpture_Good1", BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_AMAZING_1.NAME, BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_AMAZING_1.DESC, PermitRarity.Universal, "sculpture_amazing_1_kanim", "amazing_1", 15, true, "LookingGreat", "Sculpture", "", null, null),
			new ArtableInfo("Sculpture_Good2", BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_AMAZING_2.NAME, BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_AMAZING_2.DESC, PermitRarity.Universal, "sculpture_amazing_2_kanim", "amazing_2", 15, true, "LookingGreat", "Sculpture", "", null, null),
			new ArtableInfo("Sculpture_Good3", BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_AMAZING_3.NAME, BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_AMAZING_3.DESC, PermitRarity.Universal, "sculpture_amazing_3_kanim", "amazing_3", 15, true, "LookingGreat", "Sculpture", "", null, null),
			new ArtableInfo("SmallSculpture_Bad", BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_CRAP.NAME, BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_CRAP.DESC, PermitRarity.Universal, "sculpture_1x2_crap_1_kanim", "crap_1", 5, false, "LookingUgly", "SmallSculpture", "", null, null),
			new ArtableInfo("SmallSculpture_Average", BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_GOOD.NAME, BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_GOOD.DESC, PermitRarity.Universal, "sculpture_1x2_good_1_kanim", "good_1", 10, false, "LookingOkay", "SmallSculpture", "", null, null),
			new ArtableInfo("SmallSculpture_Good", BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_AMAZING_1.NAME, BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_AMAZING_1.DESC, PermitRarity.Universal, "sculpture_1x2_amazing_1_kanim", "amazing_1", 15, true, "LookingGreat", "SmallSculpture", "", null, null),
			new ArtableInfo("SmallSculpture_Good2", BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_AMAZING_2.NAME, BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_AMAZING_2.DESC, PermitRarity.Universal, "sculpture_1x2_amazing_2_kanim", "amazing_2", 15, true, "LookingGreat", "SmallSculpture", "", null, null),
			new ArtableInfo("SmallSculpture_Good3", BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_AMAZING_3.NAME, BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_AMAZING_3.DESC, PermitRarity.Universal, "sculpture_1x2_amazing_3_kanim", "amazing_3", 15, true, "LookingGreat", "SmallSculpture", "", null, null),
			new ArtableInfo("IceSculpture_Bad", BUILDINGS.PREFABS.ICESCULPTURE.FACADES.ICESCULPTURE_CRAP.NAME, BUILDINGS.PREFABS.ICESCULPTURE.FACADES.ICESCULPTURE_CRAP.DESC, PermitRarity.Universal, "icesculpture_crap_kanim", "crap", 5, false, "LookingUgly", "IceSculpture", "", null, null),
			new ArtableInfo("IceSculpture_Average", BUILDINGS.PREFABS.ICESCULPTURE.FACADES.ICESCULPTURE_AMAZING_1.NAME, BUILDINGS.PREFABS.ICESCULPTURE.FACADES.ICESCULPTURE_AMAZING_1.DESC, PermitRarity.Universal, "icesculpture_idle_kanim", "idle", 10, false, "LookingOkay", "IceSculpture", "good", null, null),
			new ArtableInfo("MarbleSculpture_Bad", BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_CRAP_1.NAME, BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_CRAP_1.DESC, PermitRarity.Universal, "sculpture_marble_crap_1_kanim", "crap_1", 5, false, "LookingUgly", "MarbleSculpture", "", null, null),
			new ArtableInfo("MarbleSculpture_Average", BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_GOOD_1.NAME, BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_GOOD_1.DESC, PermitRarity.Universal, "sculpture_marble_good_1_kanim", "good_1", 10, false, "LookingOkay", "MarbleSculpture", "", null, null),
			new ArtableInfo("MarbleSculpture_Good1", BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_AMAZING_1.NAME, BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_AMAZING_1.DESC, PermitRarity.Universal, "sculpture_marble_amazing_1_kanim", "amazing_1", 15, true, "LookingGreat", "MarbleSculpture", "", null, null),
			new ArtableInfo("MarbleSculpture_Good2", BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_AMAZING_2.NAME, BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_AMAZING_2.DESC, PermitRarity.Universal, "sculpture_marble_amazing_2_kanim", "amazing_2", 15, true, "LookingGreat", "MarbleSculpture", "", null, null),
			new ArtableInfo("MarbleSculpture_Good3", BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_AMAZING_3.NAME, BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_AMAZING_3.DESC, PermitRarity.Universal, "sculpture_marble_amazing_3_kanim", "amazing_3", 15, true, "LookingGreat", "MarbleSculpture", "", null, null),
			new ArtableInfo("MetalSculpture_Bad", BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_CRAP_1.NAME, BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_CRAP_1.DESC, PermitRarity.Universal, "sculpture_metal_crap_1_kanim", "crap_1", 5, false, "LookingUgly", "MetalSculpture", "", null, null),
			new ArtableInfo("MetalSculpture_Average", BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_GOOD_1.NAME, BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_GOOD_1.DESC, PermitRarity.Universal, "sculpture_metal_good_1_kanim", "good_1", 10, false, "LookingOkay", "MetalSculpture", "", null, null),
			new ArtableInfo("MetalSculpture_Good1", BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_AMAZING_1.NAME, BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_AMAZING_1.DESC, PermitRarity.Universal, "sculpture_metal_amazing_1_kanim", "amazing_1", 15, true, "LookingGreat", "MetalSculpture", "", null, null),
			new ArtableInfo("MetalSculpture_Good2", BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_AMAZING_2.NAME, BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_AMAZING_2.DESC, PermitRarity.Universal, "sculpture_metal_amazing_2_kanim", "amazing_2", 15, true, "LookingGreat", "MetalSculpture", "", null, null),
			new ArtableInfo("MetalSculpture_Good3", BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_AMAZING_3.NAME, BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_AMAZING_3.DESC, PermitRarity.Universal, "sculpture_metal_amazing_3_kanim", "amazing_3", 15, true, "LookingGreat", "MetalSculpture", "", null, null)
		});
	}

	private void SetupClothingItems()
	{
	}

	private void SetupClothingOutfits()
	{
	}

	private void SetupBalloonArtistFacades()
	{
	}

	private void SetupStickerBombFacades()
	{
		this.blueprintCollection.stickerBombFacades.AddRange(new StickerBombFacadeInfo[]
		{
			new StickerBombFacadeInfo("a", STICKERNAMES.STICKER_A, "TODO:DbStickers", PermitRarity.Unknown, "sticker_a_kanim", "a", null, null),
			new StickerBombFacadeInfo("b", STICKERNAMES.STICKER_B, "TODO:DbStickers", PermitRarity.Unknown, "sticker_b_kanim", "b", null, null),
			new StickerBombFacadeInfo("c", STICKERNAMES.STICKER_C, "TODO:DbStickers", PermitRarity.Unknown, "sticker_c_kanim", "c", null, null),
			new StickerBombFacadeInfo("d", STICKERNAMES.STICKER_D, "TODO:DbStickers", PermitRarity.Unknown, "sticker_d_kanim", "d", null, null),
			new StickerBombFacadeInfo("e", STICKERNAMES.STICKER_E, "TODO:DbStickers", PermitRarity.Unknown, "sticker_e_kanim", "e", null, null),
			new StickerBombFacadeInfo("f", STICKERNAMES.STICKER_F, "TODO:DbStickers", PermitRarity.Unknown, "sticker_f_kanim", "f", null, null),
			new StickerBombFacadeInfo("g", STICKERNAMES.STICKER_G, "TODO:DbStickers", PermitRarity.Unknown, "sticker_g_kanim", "g", null, null),
			new StickerBombFacadeInfo("h", STICKERNAMES.STICKER_H, "TODO:DbStickers", PermitRarity.Unknown, "sticker_h_kanim", "h", null, null),
			new StickerBombFacadeInfo("rocket", STICKERNAMES.STICKER_ROCKET, "TODO:DbStickers", PermitRarity.Unknown, "sticker_rocket_kanim", "rocket", null, null),
			new StickerBombFacadeInfo("paperplane", STICKERNAMES.STICKER_PAPERPLANE, "TODO:DbStickers", PermitRarity.Unknown, "sticker_paperplane_kanim", "paperplane", null, null),
			new StickerBombFacadeInfo("plant", STICKERNAMES.STICKER_PLANT, "TODO:DbStickers", PermitRarity.Unknown, "sticker_plant_kanim", "plant", null, null),
			new StickerBombFacadeInfo("plantpot", STICKERNAMES.STICKER_PLANTPOT, "TODO:DbStickers", PermitRarity.Unknown, "sticker_plantpot_kanim", "plantpot", null, null),
			new StickerBombFacadeInfo("mushroom", STICKERNAMES.STICKER_MUSHROOM, "TODO:DbStickers", PermitRarity.Unknown, "sticker_mushroom_kanim", "mushroom", null, null),
			new StickerBombFacadeInfo("mermaid", STICKERNAMES.STICKER_MERMAID, "TODO:DbStickers", PermitRarity.Unknown, "sticker_mermaid_kanim", "mermaid", null, null),
			new StickerBombFacadeInfo("spacepet", STICKERNAMES.STICKER_SPACEPET, "TODO:DbStickers", PermitRarity.Unknown, "sticker_spacepet_kanim", "spacepet", null, null),
			new StickerBombFacadeInfo("spacepet2", STICKERNAMES.STICKER_SPACEPET2, "TODO:DbStickers", PermitRarity.Unknown, "sticker_spacepet2_kanim", "spacepet2", null, null),
			new StickerBombFacadeInfo("spacepet3", STICKERNAMES.STICKER_SPACEPET3, "TODO:DbStickers", PermitRarity.Unknown, "sticker_spacepet3_kanim", "spacepet3", null, null),
			new StickerBombFacadeInfo("spacepet4", STICKERNAMES.STICKER_SPACEPET4, "TODO:DbStickers", PermitRarity.Unknown, "sticker_spacepet4_kanim", "spacepet4", null, null),
			new StickerBombFacadeInfo("spacepet5", STICKERNAMES.STICKER_SPACEPET5, "TODO:DbStickers", PermitRarity.Unknown, "sticker_spacepet5_kanim", "spacepet5", null, null),
			new StickerBombFacadeInfo("unicorn", STICKERNAMES.STICKER_UNICORN, "TODO:DbStickers", PermitRarity.Unknown, "sticker_unicorn_kanim", "unicorn", null, null)
		});
	}

	private void SetupEquippableFacades()
	{
		this.blueprintCollection.equippableFacades.AddRange(new EquippableFacadeInfo[]
		{
			new EquippableFacadeInfo("clubshirt", EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.CLUBSHIRT, "n/a", PermitRarity.Unknown, "CustomClothing", "body_shirt_clubshirt_kanim", "shirt_clubshirt_kanim", null, null),
			new EquippableFacadeInfo("cummerbund", EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.CUMMERBUND, "n/a", PermitRarity.Unknown, "CustomClothing", "body_shirt_cummerbund_kanim", "shirt_cummerbund_kanim", null, null),
			new EquippableFacadeInfo("decor_02", EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.DECOR_02, "n/a", PermitRarity.Unknown, "CustomClothing", "body_shirt_decor02_kanim", "shirt_decor02_kanim", null, null),
			new EquippableFacadeInfo("decor_03", EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.DECOR_03, "n/a", PermitRarity.Unknown, "CustomClothing", "body_shirt_decor03_kanim", "shirt_decor03_kanim", null, null),
			new EquippableFacadeInfo("decor_04", EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.DECOR_04, "n/a", PermitRarity.Unknown, "CustomClothing", "body_shirt_decor04_kanim", "shirt_decor04_kanim", null, null),
			new EquippableFacadeInfo("decor_05", EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.DECOR_05, "n/a", PermitRarity.Unknown, "CustomClothing", "body_shirt_decor05_kanim", "shirt_decor05_kanim", null, null),
			new EquippableFacadeInfo("gaudysweater", EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.GAUDYSWEATER, "n/a", PermitRarity.Unknown, "CustomClothing", "body_shirt_gaudysweater_kanim", "shirt_gaudysweater_kanim", null, null),
			new EquippableFacadeInfo("limone", EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.LIMONE, "n/a", PermitRarity.Unknown, "CustomClothing", "body_suit_limone_kanim", "suit_limone_kanim", null, null),
			new EquippableFacadeInfo("mondrian", EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.MONDRIAN, "n/a", PermitRarity.Unknown, "CustomClothing", "body_shirt_mondrian_kanim", "shirt_mondrian_kanim", null, null),
			new EquippableFacadeInfo("overalls", EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.OVERALLS, "n/a", PermitRarity.Unknown, "CustomClothing", "body_suit_overalls_kanim", "suit_overalls_kanim", null, null),
			new EquippableFacadeInfo("triangles", EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.TRIANGLES, "n/a", PermitRarity.Unknown, "CustomClothing", "body_shirt_triangles_kanim", "shirt_triangles_kanim", null, null),
			new EquippableFacadeInfo("workout", EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.WORKOUT, "n/a", PermitRarity.Unknown, "CustomClothing", "body_suit_workout_kanim", "suit_workout_kanim", null, null)
		});
	}

	private void SetupMonumentParts()
	{
		this.blueprintCollection.monumentParts.AddRange(new MonumentPartInfo[]
		{
			new MonumentPartInfo("bottom_option_a", BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_A.NAME, BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_A.DESC, PermitRarity.Universal, "monument_base_a_kanim", "option_a", "straight_legs", MonumentPartResource.Part.Bottom, null, null),
			new MonumentPartInfo("bottom_option_b", BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_B.NAME, BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_B.DESC, PermitRarity.Universal, "monument_base_b_kanim", "option_b", "wide_stance", MonumentPartResource.Part.Bottom, null, null),
			new MonumentPartInfo("bottom_option_c", BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_C.NAME, BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_C.DESC, PermitRarity.Universal, "monument_base_c_kanim", "option_c", "hmmm_legs", MonumentPartResource.Part.Bottom, null, null),
			new MonumentPartInfo("bottom_option_d", BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_D.NAME, BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_D.DESC, PermitRarity.Universal, "monument_base_d_kanim", "option_d", "sitting_stool", MonumentPartResource.Part.Bottom, null, null),
			new MonumentPartInfo("bottom_option_e", BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_E.NAME, BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_E.DESC, PermitRarity.Universal, "monument_base_e_kanim", "option_e", "wide_stance2", MonumentPartResource.Part.Bottom, null, null),
			new MonumentPartInfo("bottom_option_f", BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_F.NAME, BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_F.DESC, PermitRarity.Universal, "monument_base_f_kanim", "option_f", "posing1", MonumentPartResource.Part.Bottom, null, null),
			new MonumentPartInfo("bottom_option_g", BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_G.NAME, BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_G.DESC, PermitRarity.Universal, "monument_base_g_kanim", "option_g", "knee_kick", MonumentPartResource.Part.Bottom, null, null),
			new MonumentPartInfo("bottom_option_h", BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_H.NAME, BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_H.DESC, PermitRarity.Universal, "monument_base_h_kanim", "option_h", "step_on_hatches", MonumentPartResource.Part.Bottom, null, null),
			new MonumentPartInfo("bottom_option_i", BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_I.NAME, BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_I.DESC, PermitRarity.Universal, "monument_base_i_kanim", "option_i", "sit_on_tools", MonumentPartResource.Part.Bottom, null, null),
			new MonumentPartInfo("bottom_option_j", BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_J.NAME, BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_J.DESC, PermitRarity.Universal, "monument_base_j_kanim", "option_j", "water_pacu", MonumentPartResource.Part.Bottom, null, null),
			new MonumentPartInfo("bottom_option_k", BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_K.NAME, BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_K.DESC, PermitRarity.Universal, "monument_base_k_kanim", "option_k", "sit_on_eggs", MonumentPartResource.Part.Bottom, null, null),
			new MonumentPartInfo("mid_option_a", BUILDINGS.PREFABS.MONUMENTMIDDLE.FACADES.OPTION_A.NAME, BUILDINGS.PREFABS.MONUMENTMIDDLE.FACADES.OPTION_A.DESC, PermitRarity.Universal, "monument_mid_a_kanim", "option_a", "thumbs_up", MonumentPartResource.Part.Middle, null, null),
			new MonumentPartInfo("mid_option_b", BUILDINGS.PREFABS.MONUMENTMIDDLE.FACADES.OPTION_B.NAME, BUILDINGS.PREFABS.MONUMENTMIDDLE.FACADES.OPTION_B.DESC, PermitRarity.Universal, "monument_mid_b_kanim", "option_b", "wrench", MonumentPartResource.Part.Middle, null, null),
			new MonumentPartInfo("mid_option_c", BUILDINGS.PREFABS.MONUMENTMIDDLE.FACADES.OPTION_C.NAME, BUILDINGS.PREFABS.MONUMENTMIDDLE.FACADES.OPTION_C.DESC, PermitRarity.Universal, "monument_mid_c_kanim", "option_c", "hmmm", MonumentPartResource.Part.Middle, null, null),
			new MonumentPartInfo("mid_option_d", BUILDINGS.PREFABS.MONUMENTMIDDLE.FACADES.OPTION_D.NAME, BUILDINGS.PREFABS.MONUMENTMIDDLE.FACADES.OPTION_D.DESC, PermitRarity.Universal, "monument_mid_d_kanim", "option_d", "hips_hands", MonumentPartResource.Part.Middle, null, null),
			new MonumentPartInfo("mid_option_e", BUILDINGS.PREFABS.MONUMENTMIDDLE.FACADES.OPTION_E.NAME, BUILDINGS.PREFABS.MONUMENTMIDDLE.FACADES.OPTION_E.DESC, PermitRarity.Universal, "monument_mid_e_kanim", "option_e", "hold_face", MonumentPartResource.Part.Middle, null, null),
			new MonumentPartInfo("mid_option_f", BUILDINGS.PREFABS.MONUMENTMIDDLE.FACADES.OPTION_F.NAME, BUILDINGS.PREFABS.MONUMENTMIDDLE.FACADES.OPTION_F.DESC, PermitRarity.Universal, "monument_mid_f_kanim", "option_f", "finger_gun", MonumentPartResource.Part.Middle, null, null),
			new MonumentPartInfo("mid_option_g", BUILDINGS.PREFABS.MONUMENTMIDDLE.FACADES.OPTION_G.NAME, BUILDINGS.PREFABS.MONUMENTMIDDLE.FACADES.OPTION_G.DESC, PermitRarity.Universal, "monument_mid_g_kanim", "option_g", "model_pose", MonumentPartResource.Part.Middle, null, null),
			new MonumentPartInfo("mid_option_h", BUILDINGS.PREFABS.MONUMENTMIDDLE.FACADES.OPTION_H.NAME, BUILDINGS.PREFABS.MONUMENTMIDDLE.FACADES.OPTION_H.DESC, PermitRarity.Universal, "monument_mid_h_kanim", "option_h", "punch", MonumentPartResource.Part.Middle, null, null),
			new MonumentPartInfo("mid_option_i", BUILDINGS.PREFABS.MONUMENTMIDDLE.FACADES.OPTION_I.NAME, BUILDINGS.PREFABS.MONUMENTMIDDLE.FACADES.OPTION_I.DESC, PermitRarity.Universal, "monument_mid_i_kanim", "option_i", "holding_hatch", MonumentPartResource.Part.Middle, null, null),
			new MonumentPartInfo("mid_option_j", BUILDINGS.PREFABS.MONUMENTMIDDLE.FACADES.OPTION_J.NAME, BUILDINGS.PREFABS.MONUMENTMIDDLE.FACADES.OPTION_J.DESC, PermitRarity.Universal, "monument_mid_j_kanim", "option_j", "model_pose2", MonumentPartResource.Part.Middle, null, null),
			new MonumentPartInfo("mid_option_k", BUILDINGS.PREFABS.MONUMENTMIDDLE.FACADES.OPTION_K.NAME, BUILDINGS.PREFABS.MONUMENTMIDDLE.FACADES.OPTION_K.DESC, PermitRarity.Universal, "monument_mid_k_kanim", "option_k", "balancing", MonumentPartResource.Part.Middle, null, null),
			new MonumentPartInfo("mid_option_l", BUILDINGS.PREFABS.MONUMENTMIDDLE.FACADES.OPTION_L.NAME, BUILDINGS.PREFABS.MONUMENTMIDDLE.FACADES.OPTION_L.DESC, PermitRarity.Universal, "monument_mid_l_kanim", "option_l", "holding_babies", MonumentPartResource.Part.Middle, null, null),
			new MonumentPartInfo("top_option_a", BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_A.NAME, BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_A.DESC, PermitRarity.Universal, "monument_upper_a_kanim", "option_a", "leira", MonumentPartResource.Part.Top, null, null),
			new MonumentPartInfo("top_option_b", BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_B.NAME, BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_B.DESC, PermitRarity.Universal, "monument_upper_b_kanim", "option_b", "mae", MonumentPartResource.Part.Top, null, null),
			new MonumentPartInfo("top_option_c", BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_C.NAME, BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_C.DESC, PermitRarity.Universal, "monument_upper_c_kanim", "option_c", "puft", MonumentPartResource.Part.Top, null, null),
			new MonumentPartInfo("top_option_d", BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_D.NAME, BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_D.DESC, PermitRarity.Universal, "monument_upper_d_kanim", "option_d", "nikola", MonumentPartResource.Part.Top, null, null),
			new MonumentPartInfo("top_option_e", BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_E.NAME, BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_E.DESC, PermitRarity.Universal, "monument_upper_e_kanim", "option_e", "burt", MonumentPartResource.Part.Top, null, null),
			new MonumentPartInfo("top_option_f", BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_F.NAME, BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_F.DESC, PermitRarity.Universal, "monument_upper_f_kanim", "option_f", "rowan", MonumentPartResource.Part.Top, null, null),
			new MonumentPartInfo("top_option_g", BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_G.NAME, BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_G.DESC, PermitRarity.Universal, "monument_upper_g_kanim", "option_g", "nisbet", MonumentPartResource.Part.Top, null, null),
			new MonumentPartInfo("top_option_h", BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_H.NAME, BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_H.DESC, PermitRarity.Universal, "monument_upper_h_kanim", "option_h", "joshua", MonumentPartResource.Part.Top, null, null),
			new MonumentPartInfo("top_option_i", BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_I.NAME, BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_I.DESC, PermitRarity.Universal, "monument_upper_i_kanim", "option_i", "ren", MonumentPartResource.Part.Top, null, null),
			new MonumentPartInfo("top_option_j", BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_J.NAME, BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_J.DESC, PermitRarity.Universal, "monument_upper_j_kanim", "option_j", "hatch", MonumentPartResource.Part.Top, null, null),
			new MonumentPartInfo("top_option_k", BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_K.NAME, BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_K.DESC, PermitRarity.Universal, "monument_upper_k_kanim", "option_k", "drecko", MonumentPartResource.Part.Top, null, null),
			new MonumentPartInfo("top_option_l", BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_L.NAME, BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_L.DESC, PermitRarity.Universal, "monument_upper_l_kanim", "option_l", "driller", MonumentPartResource.Part.Top, null, null),
			new MonumentPartInfo("top_option_m", BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_M.NAME, BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_M.DESC, PermitRarity.Universal, "monument_upper_m_kanim", "option_m", "gassymoo", MonumentPartResource.Part.Top, null, null),
			new MonumentPartInfo("top_option_n", BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_N.NAME, BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_N.DESC, PermitRarity.Universal, "monument_upper_n_kanim", "option_n", "glom", MonumentPartResource.Part.Top, null, null),
			new MonumentPartInfo("top_option_o", BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_O.NAME, BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_O.DESC, PermitRarity.Universal, "monument_upper_o_kanim", "option_o", "lightbug", MonumentPartResource.Part.Top, null, null),
			new MonumentPartInfo("top_option_p", BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_P.NAME, BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_P.DESC, PermitRarity.Universal, "monument_upper_p_kanim", "option_p", "slickster", MonumentPartResource.Part.Top, null, null),
			new MonumentPartInfo("top_option_q", BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_Q.NAME, BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_Q.DESC, PermitRarity.Universal, "monument_upper_q_kanim", "option_q", "pacu", MonumentPartResource.Part.Top, null, null)
		});
		this.blueprintCollection.monumentParts.AddRange(new MonumentPartInfo[]
		{
			new MonumentPartInfo("bottom_option_l", BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_L.NAME, BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_L.DESC, PermitRarity.Universal, "monument_base_l_kanim", "option_l", "rocketnosecone", MonumentPartResource.Part.Bottom, DlcManager.EXPANSION1, null),
			new MonumentPartInfo("bottom_option_m", BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_M.NAME, BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_M.DESC, PermitRarity.Universal, "monument_base_m_kanim", "option_m", "rocketsugarengine", MonumentPartResource.Part.Bottom, DlcManager.EXPANSION1, null),
			new MonumentPartInfo("bottom_option_n", BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_N.NAME, BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_N.DESC, PermitRarity.Universal, "monument_base_n_kanim", "option_n", "rocketnCO2", MonumentPartResource.Part.Bottom, DlcManager.EXPANSION1, null),
			new MonumentPartInfo("bottom_option_o", BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_O.NAME, BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_O.DESC, PermitRarity.Universal, "monument_base_o_kanim", "option_o", "rocketpetro", MonumentPartResource.Part.Bottom, DlcManager.EXPANSION1, null),
			new MonumentPartInfo("bottom_option_p", BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_P.NAME, BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_P.DESC, PermitRarity.Universal, "monument_base_p_kanim", "option_p", "rocketnoseconesmall", MonumentPartResource.Part.Bottom, DlcManager.EXPANSION1, null),
			new MonumentPartInfo("bottom_option_q", BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_Q.NAME, BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_Q.DESC, PermitRarity.Universal, "monument_base_q_kanim", "option_q", "rocketradengine", MonumentPartResource.Part.Bottom, DlcManager.EXPANSION1, null),
			new MonumentPartInfo("bottom_option_r", BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_R.NAME, BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_R.DESC, PermitRarity.Universal, "monument_base_r_kanim", "option_r", "sweepyoff", MonumentPartResource.Part.Bottom, DlcManager.EXPANSION1, null),
			new MonumentPartInfo("bottom_option_s", BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_S.NAME, BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_S.DESC, PermitRarity.Universal, "monument_base_s_kanim", "option_s", "sweepypeek", MonumentPartResource.Part.Bottom, DlcManager.EXPANSION1, null),
			new MonumentPartInfo("bottom_option_t", BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_T.NAME, BUILDINGS.PREFABS.MONUMENTBOTTOM.FACADES.OPTION_T.DESC, PermitRarity.Universal, "monument_base_t_kanim", "option_t", "sweepy", MonumentPartResource.Part.Bottom, DlcManager.EXPANSION1, null),
			new MonumentPartInfo("mid_option_m", BUILDINGS.PREFABS.MONUMENTMIDDLE.FACADES.OPTION_M.NAME, BUILDINGS.PREFABS.MONUMENTMIDDLE.FACADES.OPTION_M.DESC, PermitRarity.Universal, "monument_mid_m_kanim", "option_m", "rocket", MonumentPartResource.Part.Middle, DlcManager.EXPANSION1, null),
			new MonumentPartInfo("mid_option_n", BUILDINGS.PREFABS.MONUMENTMIDDLE.FACADES.OPTION_N.NAME, BUILDINGS.PREFABS.MONUMENTMIDDLE.FACADES.OPTION_N.DESC, PermitRarity.Universal, "monument_mid_n_kanim", "option_n", "holding_baby_worm", MonumentPartResource.Part.Middle, DlcManager.EXPANSION1, null),
			new MonumentPartInfo("mid_option_o", BUILDINGS.PREFABS.MONUMENTMIDDLE.FACADES.OPTION_O.NAME, BUILDINGS.PREFABS.MONUMENTMIDDLE.FACADES.OPTION_O.DESC, PermitRarity.Universal, "monument_mid_o_kanim", "option_o", "holding_baby_blarva_critter", MonumentPartResource.Part.Middle, DlcManager.EXPANSION1, null),
			new MonumentPartInfo("top_option_r", BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_R.NAME, BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_R.DESC, PermitRarity.Universal, "monument_upper_r_kanim", "option_r", "bee", MonumentPartResource.Part.Top, DlcManager.EXPANSION1, null),
			new MonumentPartInfo("top_option_s", BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_S.NAME, BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_S.DESC, PermitRarity.Universal, "monument_upper_s_kanim", "option_s", "critter", MonumentPartResource.Part.Top, DlcManager.EXPANSION1, null),
			new MonumentPartInfo("top_option_t", BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_T.NAME, BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_T.DESC, PermitRarity.Universal, "monument_upper_t_kanim", "option_t", "caterpillar", MonumentPartResource.Part.Top, DlcManager.EXPANSION1, null),
			new MonumentPartInfo("top_option_u", BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_U.NAME, BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_U.DESC, PermitRarity.Universal, "monument_upper_u_kanim", "option_u", "worm", MonumentPartResource.Part.Top, DlcManager.EXPANSION1, null),
			new MonumentPartInfo("top_option_v", BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_V.NAME, BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_V.DESC, PermitRarity.Universal, "monument_upper_v_kanim", "option_v", "scout_bot", MonumentPartResource.Part.Top, DlcManager.EXPANSION1, null),
			new MonumentPartInfo("top_option_w", BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_W.NAME, BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_W.DESC, PermitRarity.Universal, "monument_upper_w_kanim", "option_w", "MiMa", MonumentPartResource.Part.Top, DlcManager.EXPANSION1, null),
			new MonumentPartInfo("top_option_x", BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_X.NAME, BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_X.DESC, PermitRarity.Universal, "monument_upper_x_kanim", "option_x", "Stinky", MonumentPartResource.Part.Top, DlcManager.EXPANSION1, null),
			new MonumentPartInfo("top_option_y", BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_Y.NAME, BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_Y.DESC, PermitRarity.Universal, "monument_upper_y_kanim", "option_y", "Harold", MonumentPartResource.Part.Top, DlcManager.EXPANSION1, null),
			new MonumentPartInfo("top_option_z", BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_Z.NAME, BUILDINGS.PREFABS.MONUMENTTOP.FACADES.OPTION_Z.DESC, PermitRarity.Universal, "monument_upper_z_kanim", "option_z", "Nails", MonumentPartResource.Part.Top, DlcManager.EXPANSION1, null)
		});
	}
}
