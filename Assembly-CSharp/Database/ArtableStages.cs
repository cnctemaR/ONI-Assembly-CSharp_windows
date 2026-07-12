using System;
using System.Collections.Generic;
using STRINGS;

namespace Database
{
	public class ArtableStages : ResourceSet<ArtableStage>
	{
		public ArtableStage Add(string id, string name, string desc, PermitRarity rarity, string animFile, string anim, int decor_value, bool cheer_on_complete, string status_id, string prefabId, string symbolname = "")
		{
			ArtableStatusItem artableStatusItem = Db.Get().ArtableStatuses.Get(status_id);
			ArtableStage artableStage = new ArtableStage(id, name, desc, rarity, animFile, anim, decor_value, cheer_on_complete, artableStatusItem, prefabId, symbolname);
			this.resources.Add(artableStage);
			return artableStage;
		}

		public ArtableStages(ResourceSet parent)
			: base("ArtableStages", parent)
		{
			foreach (ArtableStages.Info info in ArtableStages.Infos_All)
			{
				this.Add(info.id, info.name, info.desc, info.rarity, info.animFile, info.anim, info.decor_value, info.cheer_on_complete, info.status_id, info.prefabId, info.symbolname);
			}
		}

		public List<ArtableStage> GetPrefabStages(Tag prefab_id)
		{
			return this.resources.FindAll((ArtableStage stage) => stage.prefabId == prefab_id);
		}

		public ArtableStage DefaultPrefabStage(Tag prefab_id)
		{
			return this.GetPrefabStages(prefab_id).Find((ArtableStage stage) => stage.statusItem == Db.Get().ArtableStatuses.AwaitingArting);
		}

		public static ArtableStages.Info[] Infos_Default = new ArtableStages.Info[]
		{
			new ArtableStages.Info("Canvas_Bad", BUILDINGS.PREFABS.CANVAS.FACADES.ART_A.NAME, BUILDINGS.PREFABS.CANVAS.FACADES.ART_A.DESC, PermitRarity.Universal, "painting_art_a_kanim", "art_a", 5, false, "LookingUgly", "Canvas", "canvas"),
			new ArtableStages.Info("Canvas_Average", BUILDINGS.PREFABS.CANVAS.FACADES.ART_B.NAME, BUILDINGS.PREFABS.CANVAS.FACADES.ART_B.DESC, PermitRarity.Universal, "painting_art_b_kanim", "art_b", 10, false, "LookingOkay", "Canvas", "canvas"),
			new ArtableStages.Info("Canvas_Good", BUILDINGS.PREFABS.CANVAS.FACADES.ART_C.NAME, BUILDINGS.PREFABS.CANVAS.FACADES.ART_C.DESC, PermitRarity.Universal, "painting_art_c_kanim", "art_c", 15, true, "LookingGreat", "Canvas", "canvas"),
			new ArtableStages.Info("Canvas_Good2", BUILDINGS.PREFABS.CANVAS.FACADES.ART_D.NAME, BUILDINGS.PREFABS.CANVAS.FACADES.ART_D.DESC, PermitRarity.Universal, "painting_art_d_kanim", "art_d", 15, true, "LookingGreat", "Canvas", "canvas"),
			new ArtableStages.Info("Canvas_Good3", BUILDINGS.PREFABS.CANVAS.FACADES.ART_E.NAME, BUILDINGS.PREFABS.CANVAS.FACADES.ART_E.DESC, PermitRarity.Universal, "painting_art_e_kanim", "art_e", 15, true, "LookingGreat", "Canvas", "canvas"),
			new ArtableStages.Info("Canvas_Good4", BUILDINGS.PREFABS.CANVAS.FACADES.ART_F.NAME, BUILDINGS.PREFABS.CANVAS.FACADES.ART_F.DESC, PermitRarity.Universal, "painting_art_f_kanim", "art_f", 15, true, "LookingGreat", "Canvas", "canvas"),
			new ArtableStages.Info("Canvas_Good5", BUILDINGS.PREFABS.CANVAS.FACADES.ART_G.NAME, BUILDINGS.PREFABS.CANVAS.FACADES.ART_G.DESC, PermitRarity.Universal, "painting_art_g_kanim", "art_g", 15, true, "LookingGreat", "Canvas", "canvas"),
			new ArtableStages.Info("Canvas_Good6", BUILDINGS.PREFABS.CANVAS.FACADES.ART_H.NAME, BUILDINGS.PREFABS.CANVAS.FACADES.ART_H.DESC, PermitRarity.Universal, "painting_art_h_kanim", "art_h", 15, true, "LookingGreat", "Canvas", "canvas"),
			new ArtableStages.Info("CanvasTall_Bad", BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_A.NAME, BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_A.DESC, PermitRarity.Universal, "painting_tall_art_a_kanim", "art_a", 5, false, "LookingUgly", "CanvasTall", "canvas"),
			new ArtableStages.Info("CanvasTall_Average", BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_B.NAME, BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_B.DESC, PermitRarity.Universal, "painting_tall_art_b_kanim", "art_b", 10, false, "LookingOkay", "CanvasTall", "canvas"),
			new ArtableStages.Info("CanvasTall_Good", BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_C.NAME, BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_C.DESC, PermitRarity.Universal, "painting_tall_art_c_kanim", "art_c", 15, true, "LookingGreat", "CanvasTall", "canvas"),
			new ArtableStages.Info("CanvasTall_Good2", BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_D.NAME, BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_D.DESC, PermitRarity.Universal, "painting_tall_art_d_kanim", "art_d", 15, true, "LookingGreat", "CanvasTall", "canvas"),
			new ArtableStages.Info("CanvasTall_Good3", BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_E.NAME, BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_E.DESC, PermitRarity.Universal, "painting_tall_art_e_kanim", "art_e", 15, true, "LookingGreat", "CanvasTall", "canvas"),
			new ArtableStages.Info("CanvasTall_Good4", BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_F.NAME, BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_F.DESC, PermitRarity.Universal, "painting_tall_art_f_kanim", "art_f", 15, true, "LookingGreat", "CanvasTall", "canvas"),
			new ArtableStages.Info("CanvasWide_Bad", BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_A.NAME, BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_A.DESC, PermitRarity.Universal, "painting_wide_art_a_kanim", "art_a", 5, false, "LookingUgly", "CanvasWide", "canvas"),
			new ArtableStages.Info("CanvasWide_Average", BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_B.NAME, BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_B.DESC, PermitRarity.Universal, "painting_wide_art_b_kanim", "art_b", 10, false, "LookingOkay", "CanvasWide", "canvas"),
			new ArtableStages.Info("CanvasWide_Good", BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_C.NAME, BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_C.DESC, PermitRarity.Universal, "painting_wide_art_c_kanim", "art_c", 15, true, "LookingGreat", "CanvasWide", "canvas"),
			new ArtableStages.Info("CanvasWide_Good2", BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_D.NAME, BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_D.DESC, PermitRarity.Universal, "painting_wide_art_d_kanim", "art_d", 15, true, "LookingGreat", "CanvasWide", "canvas"),
			new ArtableStages.Info("CanvasWide_Good3", BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_E.NAME, BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_E.DESC, PermitRarity.Universal, "painting_wide_art_e_kanim", "art_e", 15, true, "LookingGreat", "CanvasWide", "canvas"),
			new ArtableStages.Info("CanvasWide_Good4", BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_F.NAME, BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_F.DESC, PermitRarity.Universal, "painting_wide_art_f_kanim", "art_f", 15, true, "LookingGreat", "CanvasWide", "canvas"),
			new ArtableStages.Info("Sculpture_Bad", BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_CRAP_1.NAME, BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_CRAP_1.DESC, PermitRarity.Universal, "sculpture_crap_1_kanim", "crap_1", 5, false, "LookingUgly", "Sculpture", ""),
			new ArtableStages.Info("Sculpture_Average", BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_GOOD_1.NAME, BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_GOOD_1.DESC, PermitRarity.Universal, "sculpture_good_1_kanim", "good_1", 10, false, "LookingOkay", "Sculpture", ""),
			new ArtableStages.Info("Sculpture_Good1", BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_AMAZING_1.NAME, BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_AMAZING_1.DESC, PermitRarity.Universal, "sculpture_amazing_1_kanim", "amazing_1", 15, true, "LookingGreat", "Sculpture", ""),
			new ArtableStages.Info("Sculpture_Good2", BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_AMAZING_2.NAME, BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_AMAZING_2.DESC, PermitRarity.Universal, "sculpture_amazing_2_kanim", "amazing_2", 15, true, "LookingGreat", "Sculpture", ""),
			new ArtableStages.Info("Sculpture_Good3", BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_AMAZING_3.NAME, BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_AMAZING_3.DESC, PermitRarity.Universal, "sculpture_amazing_3_kanim", "amazing_3", 15, true, "LookingGreat", "Sculpture", ""),
			new ArtableStages.Info("SmallSculpture_Bad", BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_CRAP.NAME, BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_CRAP.DESC, PermitRarity.Universal, "sculpture_1x2_crap_1_kanim", "crap_1", 5, false, "LookingUgly", "SmallSculpture", ""),
			new ArtableStages.Info("SmallSculpture_Average", BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_GOOD.NAME, BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_GOOD.DESC, PermitRarity.Universal, "sculpture_1x2_good_1_kanim", "good_1", 10, false, "LookingOkay", "SmallSculpture", ""),
			new ArtableStages.Info("SmallSculpture_Good", BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_AMAZING_1.NAME, BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_AMAZING_1.DESC, PermitRarity.Universal, "sculpture_1x2_amazing_1_kanim", "amazing_1", 15, true, "LookingGreat", "SmallSculpture", ""),
			new ArtableStages.Info("SmallSculpture_Good2", BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_AMAZING_2.NAME, BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_AMAZING_2.DESC, PermitRarity.Universal, "sculpture_1x2_amazing_2_kanim", "amazing_2", 15, true, "LookingGreat", "SmallSculpture", ""),
			new ArtableStages.Info("SmallSculpture_Good3", BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_AMAZING_3.NAME, BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_AMAZING_3.DESC, PermitRarity.Universal, "sculpture_1x2_amazing_3_kanim", "amazing_3", 15, true, "LookingGreat", "SmallSculpture", ""),
			new ArtableStages.Info("IceSculpture_Bad", BUILDINGS.PREFABS.ICESCULPTURE.FACADES.ICESCULPTURE_CRAP.NAME, BUILDINGS.PREFABS.ICESCULPTURE.FACADES.ICESCULPTURE_CRAP.DESC, PermitRarity.Universal, "icesculpture_crap_kanim", "crap", 5, false, "LookingUgly", "IceSculpture", ""),
			new ArtableStages.Info("IceSculpture_Average", BUILDINGS.PREFABS.ICESCULPTURE.FACADES.ICESCULPTURE_AMAZING_1.NAME, BUILDINGS.PREFABS.ICESCULPTURE.FACADES.ICESCULPTURE_AMAZING_1.DESC, PermitRarity.Universal, "icesculpture_idle_kanim", "idle", 10, false, "LookingOkay", "IceSculpture", "good"),
			new ArtableStages.Info("MarbleSculpture_Bad", BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_CRAP_1.NAME, BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_CRAP_1.DESC, PermitRarity.Universal, "sculpture_marble_crap_1_kanim", "crap_1", 5, false, "LookingUgly", "MarbleSculpture", ""),
			new ArtableStages.Info("MarbleSculpture_Average", BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_GOOD_1.NAME, BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_GOOD_1.DESC, PermitRarity.Universal, "sculpture_marble_good_1_kanim", "good_1", 10, false, "LookingOkay", "MarbleSculpture", ""),
			new ArtableStages.Info("MarbleSculpture_Good1", BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_AMAZING_1.NAME, BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_AMAZING_1.DESC, PermitRarity.Universal, "sculpture_marble_amazing_1_kanim", "amazing_1", 15, true, "LookingGreat", "MarbleSculpture", ""),
			new ArtableStages.Info("MarbleSculpture_Good2", BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_AMAZING_2.NAME, BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_AMAZING_2.DESC, PermitRarity.Universal, "sculpture_marble_amazing_2_kanim", "amazing_2", 15, true, "LookingGreat", "MarbleSculpture", ""),
			new ArtableStages.Info("MarbleSculpture_Good3", BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_AMAZING_3.NAME, BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_AMAZING_3.DESC, PermitRarity.Universal, "sculpture_marble_amazing_3_kanim", "amazing_3", 15, true, "LookingGreat", "MarbleSculpture", ""),
			new ArtableStages.Info("MetalSculpture_Bad", BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_CRAP_1.NAME, BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_CRAP_1.DESC, PermitRarity.Universal, "sculpture_metal_crap_1_kanim", "crap_1", 5, false, "LookingUgly", "MetalSculpture", ""),
			new ArtableStages.Info("MetalSculpture_Average", BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_GOOD_1.NAME, BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_GOOD_1.DESC, PermitRarity.Universal, "sculpture_metal_good_1_kanim", "good_1", 10, false, "LookingOkay", "MetalSculpture", ""),
			new ArtableStages.Info("MetalSculpture_Good1", BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_AMAZING_1.NAME, BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_AMAZING_1.DESC, PermitRarity.Universal, "sculpture_metal_amazing_1_kanim", "amazing_1", 15, true, "LookingGreat", "MetalSculpture", ""),
			new ArtableStages.Info("MetalSculpture_Good2", BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_AMAZING_2.NAME, BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_AMAZING_2.DESC, PermitRarity.Universal, "sculpture_metal_amazing_2_kanim", "amazing_2", 15, true, "LookingGreat", "MetalSculpture", ""),
			new ArtableStages.Info("MetalSculpture_Good3", BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_AMAZING_3.NAME, BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_AMAZING_3.DESC, PermitRarity.Universal, "sculpture_metal_amazing_3_kanim", "amazing_3", 15, true, "LookingGreat", "MetalSculpture", "")
		};

		public static ArtableStages.Info[] Infos_Skins = new ArtableStages.Info[]
		{
			new ArtableStages.Info("Canvas_Good7", BUILDINGS.PREFABS.CANVAS.FACADES.ART_I.NAME, BUILDINGS.PREFABS.CANVAS.FACADES.ART_I.DESC, PermitRarity.Decent, "painting_art_i_kanim", "art_i", 15, true, "LookingGreat", "Canvas", "canvas"),
			new ArtableStages.Info("Canvas_Good8", BUILDINGS.PREFABS.CANVAS.FACADES.ART_J.NAME, BUILDINGS.PREFABS.CANVAS.FACADES.ART_J.DESC, PermitRarity.Decent, "painting_art_j_kanim", "art_j", 15, true, "LookingGreat", "Canvas", "canvas"),
			new ArtableStages.Info("Canvas_Good9", BUILDINGS.PREFABS.CANVAS.FACADES.ART_K.NAME, BUILDINGS.PREFABS.CANVAS.FACADES.ART_K.DESC, PermitRarity.Decent, "painting_art_k_kanim", "art_k", 15, true, "LookingGreat", "Canvas", "canvas"),
			new ArtableStages.Info("CanvasTall_Good5", BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_G.NAME, BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_G.DESC, PermitRarity.Decent, "painting_tall_art_g_kanim", "art_g", 15, true, "LookingGreat", "CanvasTall", "canvas"),
			new ArtableStages.Info("CanvasTall_Good6", BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_H.NAME, BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_H.DESC, PermitRarity.Decent, "painting_tall_art_h_kanim", "art_h", 15, true, "LookingGreat", "CanvasTall", "canvas"),
			new ArtableStages.Info("CanvasTall_Good7", BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_I.NAME, BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_I.DESC, PermitRarity.Decent, "painting_tall_art_i_kanim", "art_i", 15, true, "LookingGreat", "CanvasTall", "canvas"),
			new ArtableStages.Info("CanvasWide_Good5", BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_G.NAME, BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_G.DESC, PermitRarity.Decent, "painting_wide_art_g_kanim", "art_g", 15, true, "LookingGreat", "CanvasWide", "canvas"),
			new ArtableStages.Info("CanvasWide_Good6", BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_H.NAME, BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_H.DESC, PermitRarity.Decent, "painting_wide_art_h_kanim", "art_h", 15, true, "LookingGreat", "CanvasWide", "canvas"),
			new ArtableStages.Info("CanvasWide_Good7", BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_I.NAME, BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_I.DESC, PermitRarity.Decent, "painting_wide_art_i_kanim", "art_i", 15, true, "LookingGreat", "CanvasWide", "canvas"),
			new ArtableStages.Info("Sculpture_Good4", BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_AMAZING_4.NAME, BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_AMAZING_4.DESC, PermitRarity.Decent, "sculpture_amazing_4_kanim", "amazing_4", 15, true, "LookingGreat", "Sculpture", ""),
			new ArtableStages.Info("SmallSculpture_Good4", BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_AMAZING_4.NAME, BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_AMAZING_4.DESC, PermitRarity.Decent, "sculpture_1x2_amazing_4_kanim", "amazing_4", 15, true, "LookingGreat", "SmallSculpture", ""),
			new ArtableStages.Info("MetalSculpture_Good4", BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_AMAZING_4.NAME, BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_AMAZING_4.DESC, PermitRarity.Decent, "sculpture_metal_amazing_4_kanim", "amazing_4", 15, true, "LookingGreat", "MetalSculpture", ""),
			new ArtableStages.Info("MarbleSculpture_Good4", BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_AMAZING_4.NAME, BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_AMAZING_4.DESC, PermitRarity.Decent, "sculpture_marble_amazing_4_kanim", "amazing_4", 15, true, "LookingGreat", "MarbleSculpture", ""),
			new ArtableStages.Info("MarbleSculpture_Good5", BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_AMAZING_5.NAME, BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_AMAZING_5.DESC, PermitRarity.Decent, "sculpture_marble_amazing_5_kanim", "amazing_5", 15, true, "LookingGreat", "MarbleSculpture", ""),
			new ArtableStages.Info("IceSculpture_Average2", BUILDINGS.PREFABS.ICESCULPTURE.FACADES.ICESCULPTURE_AMAZING_2.NAME, BUILDINGS.PREFABS.ICESCULPTURE.FACADES.ICESCULPTURE_AMAZING_2.DESC, PermitRarity.Decent, "icesculpture_idle_2_kanim", "idle_2", 10, false, "LookingOkay", "IceSculpture", ""),
			new ArtableStages.Info("Canvas_Good10", BUILDINGS.PREFABS.CANVAS.FACADES.ART_L.NAME, BUILDINGS.PREFABS.CANVAS.FACADES.ART_L.DESC, PermitRarity.Decent, "painting_art_l_kanim", "art_l", 15, true, "LookingGreat", "Canvas", "canvas"),
			new ArtableStages.Info("Canvas_Good11", BUILDINGS.PREFABS.CANVAS.FACADES.ART_M.NAME, BUILDINGS.PREFABS.CANVAS.FACADES.ART_M.DESC, PermitRarity.Decent, "painting_art_m_kanim", "art_m", 15, true, "LookingGreat", "Canvas", "canvas"),
			new ArtableStages.Info("CanvasTall_Good8", BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_J.NAME, BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_J.DESC, PermitRarity.Decent, "painting_tall_art_j_kanim", "art_j", 15, true, "LookingGreat", "CanvasTall", "canvas"),
			new ArtableStages.Info("CanvasTall_Good9", BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_K.NAME, BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_K.DESC, PermitRarity.Decent, "painting_tall_art_k_kanim", "art_k", 15, true, "LookingGreat", "CanvasTall", "canvas"),
			new ArtableStages.Info("CanvasWide_Good8", BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_J.NAME, BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_J.DESC, PermitRarity.Decent, "painting_wide_art_j_kanim", "art_j", 15, true, "LookingGreat", "CanvasWide", "canvas"),
			new ArtableStages.Info("CanvasWide_Good9", BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_K.NAME, BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_K.DESC, PermitRarity.Decent, "painting_wide_art_k_kanim", "art_k", 15, true, "LookingGreat", "CanvasWide", "canvas"),
			new ArtableStages.Info("Canvas_Good13", BUILDINGS.PREFABS.CANVAS.FACADES.ART_O.NAME, BUILDINGS.PREFABS.CANVAS.FACADES.ART_O.DESC, PermitRarity.Decent, "painting_art_o_kanim", "art_o", 15, true, "LookingGreat", "Canvas", ""),
			new ArtableStages.Info("CanvasWide_Good10", BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_L.NAME, BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_L.DESC, PermitRarity.Decent, "painting_wide_art_l_kanim", "art_l", 15, true, "LookingGreat", "CanvasWide", ""),
			new ArtableStages.Info("CanvasTall_Good11", BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_M.NAME, BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_M.DESC, PermitRarity.Decent, "painting_tall_art_m_kanim", "art_m", 15, true, "LookingGreat", "CanvasTall", ""),
			new ArtableStages.Info("Sculpture_Good5", BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_AMAZING_5.NAME, BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_AMAZING_5.DESC, PermitRarity.Decent, "sculpture_amazing_5_kanim", "amazing_5", 15, true, "LookingGreat", "Sculpture", ""),
			new ArtableStages.Info("SmallSculpture_Good5", BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_AMAZING_5.NAME, BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_AMAZING_5.DESC, PermitRarity.Decent, "sculpture_1x2_amazing_5_kanim", "amazing_5", 15, true, "LookingGreat", "SmallSculpture", ""),
			new ArtableStages.Info("SmallSculpture_Good6", BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_AMAZING_6.NAME, BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_AMAZING_6.DESC, PermitRarity.Decent, "sculpture_1x2_amazing_6_kanim", "amazing_6", 15, true, "LookingGreat", "SmallSculpture", ""),
			new ArtableStages.Info("MetalSculpture_Good5", BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_AMAZING_5.NAME, BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_AMAZING_5.DESC, PermitRarity.Decent, "sculpture_metal_amazing_5_kanim", "amazing_5", 15, true, "LookingGreat", "MetalSculpture", ""),
			new ArtableStages.Info("IceSculpture_Average3", BUILDINGS.PREFABS.ICESCULPTURE.FACADES.ICESCULPTURE_AMAZING_3.NAME, BUILDINGS.PREFABS.ICESCULPTURE.FACADES.ICESCULPTURE_AMAZING_3.DESC, PermitRarity.Decent, "icesculpture_idle_3_kanim", "idle_3", 10, true, "LookingOkay", "IceSculpture", ""),
			new ArtableStages.Info("Canvas_Good12", BUILDINGS.PREFABS.CANVAS.FACADES.ART_N.NAME, BUILDINGS.PREFABS.CANVAS.FACADES.ART_N.DESC, PermitRarity.Decent, "painting_art_n_kanim", "art_n", 15, true, "LookingGreat", "Canvas", ""),
			new ArtableStages.Info("Canvas_Good14", BUILDINGS.PREFABS.CANVAS.FACADES.ART_P.NAME, BUILDINGS.PREFABS.CANVAS.FACADES.ART_P.DESC, PermitRarity.Decent, "painting_art_p_kanim", "art_p", 15, true, "LookingGreat", "Canvas", ""),
			new ArtableStages.Info("CanvasWide_Good11", BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_M.NAME, BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_M.DESC, PermitRarity.Decent, "painting_wide_art_m_kanim", "art_m", 15, true, "LookingGreat", "CanvasWide", ""),
			new ArtableStages.Info("CanvasTall_Good10", BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_L.NAME, BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_L.DESC, PermitRarity.Decent, "painting_tall_art_l_kanim", "art_l", 15, true, "LookingGreat", "CanvasTall", ""),
			new ArtableStages.Info("Sculpture_Good6", BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_AMAZING_6.NAME, BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_AMAZING_6.DESC, PermitRarity.Decent, "sculpture_amazing_6_kanim", "amazing_6", 15, true, "LookingGreat", "Sculpture", ""),
			new ArtableStages.Info("Canvas_Good15", BUILDINGS.PREFABS.CANVAS.FACADES.ART_Q.NAME, BUILDINGS.PREFABS.CANVAS.FACADES.ART_Q.DESC, PermitRarity.Decent, "painting_art_q_kanim", "art_q", 15, true, "LookingGreat", "Canvas", ""),
			new ArtableStages.Info("CanvasTall_Good14", BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_P.NAME, BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_P.DESC, PermitRarity.Decent, "painting_tall_art_p_kanim", "art_p", 15, true, "LookingGreat", "CanvasTall", "")
		};

		public static ArtableStages.Info[] Infos_All = ArtableStages.Infos_Default.Concat<ArtableStages.Info>(ArtableStages.Infos_Skins);

		public struct Info
		{
			public Info(string id, string name, string desc, PermitRarity rarity, string animFile, string anim, int decor_value, bool cheer_on_complete, string status_id, string prefabId, string symbolname = "")
			{
				this.id = id;
				this.name = name;
				this.desc = desc;
				this.rarity = rarity;
				this.animFile = animFile;
				this.anim = anim;
				this.decor_value = decor_value;
				this.cheer_on_complete = cheer_on_complete;
				this.status_id = status_id;
				this.prefabId = prefabId;
				this.symbolname = symbolname;
			}

			public string id;

			public string name;

			public string desc;

			public PermitRarity rarity;

			public string animFile;

			public string anim;

			public int decor_value;

			public bool cheer_on_complete;

			public string status_id;

			public string prefabId;

			public string symbolname;
		}
	}
}
