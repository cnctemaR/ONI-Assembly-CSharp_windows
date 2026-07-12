using System;
using System.Collections.Generic;
using STRINGS;

namespace Database
{
	public class ArtableStages : ResourceSet<ArtableStage>
	{
		public ArtableStage Add(string id, string name, string animFile, string anim, int decor_value, bool cheer_on_complete, ArtableStatusItem status_item, string prefabId, string symbolname = "")
		{
			ArtableStage artableStage = new ArtableStage(id, name, animFile, anim, decor_value, cheer_on_complete, status_item, prefabId, symbolname);
			this.resources.Add(artableStage);
			return artableStage;
		}

		public ArtableStages(ResourceSet parent)
			: base("ArtableStages", parent)
		{
			this.Add("Canvas_Bad", BUILDINGS.PREFABS.CANVAS.POORQUALITYNAME, "painting_art_a_kanim", "art_a", 5, false, Db.Get().ArtableStatuses.Ugly, "Canvas", "canvas");
			this.Add("Canvas_Average", BUILDINGS.PREFABS.CANVAS.AVERAGEQUALITYNAME, "painting_art_b_kanim", "art_b", 10, false, Db.Get().ArtableStatuses.Okay, "Canvas", "canvas");
			this.Add("Canvas_Good", BUILDINGS.PREFABS.CANVAS.EXCELLENTQUALITYNAME, "painting_art_c_kanim", "art_c", 15, true, Db.Get().ArtableStatuses.Great, "Canvas", "canvas");
			this.Add("Canvas_Good2", BUILDINGS.PREFABS.CANVAS.EXCELLENTQUALITYNAME, "painting_art_d_kanim", "art_d", 15, true, Db.Get().ArtableStatuses.Great, "Canvas", "canvas");
			this.Add("Canvas_Good3", BUILDINGS.PREFABS.CANVAS.EXCELLENTQUALITYNAME, "painting_art_e_kanim", "art_e", 15, true, Db.Get().ArtableStatuses.Great, "Canvas", "canvas");
			this.Add("Canvas_Good4", BUILDINGS.PREFABS.CANVAS.EXCELLENTQUALITYNAME, "painting_art_f_kanim", "art_f", 15, true, Db.Get().ArtableStatuses.Great, "Canvas", "canvas");
			this.Add("Canvas_Good5", BUILDINGS.PREFABS.CANVAS.EXCELLENTQUALITYNAME, "painting_art_g_kanim", "art_g", 15, true, Db.Get().ArtableStatuses.Great, "Canvas", "canvas");
			this.Add("Canvas_Good6", BUILDINGS.PREFABS.CANVAS.EXCELLENTQUALITYNAME, "painting_art_h_kanim", "art_h", 15, true, Db.Get().ArtableStatuses.Great, "Canvas", "canvas");
			this.Add("CanvasTall_Bad", BUILDINGS.PREFABS.CANVAS.POORQUALITYNAME, "painting_tall_art_a_kanim", "art_a", 5, false, Db.Get().ArtableStatuses.Ugly, "CanvasTall", "canvas");
			this.Add("CanvasTall_Average", BUILDINGS.PREFABS.CANVAS.AVERAGEQUALITYNAME, "painting_tall_art_b_kanim", "art_b", 10, false, Db.Get().ArtableStatuses.Okay, "CanvasTall", "canvas");
			this.Add("CanvasTall_Good", BUILDINGS.PREFABS.CANVAS.EXCELLENTQUALITYNAME, "painting_tall_art_c_kanim", "art_c", 15, true, Db.Get().ArtableStatuses.Great, "CanvasTall", "canvas");
			this.Add("CanvasTall_Good2", BUILDINGS.PREFABS.CANVAS.EXCELLENTQUALITYNAME, "painting_tall_art_d_kanim", "art_d", 15, true, Db.Get().ArtableStatuses.Great, "CanvasTall", "canvas");
			this.Add("CanvasTall_Good3", BUILDINGS.PREFABS.CANVAS.EXCELLENTQUALITYNAME, "painting_tall_art_e_kanim", "art_e", 15, true, Db.Get().ArtableStatuses.Great, "CanvasTall", "canvas");
			this.Add("CanvasTall_Good4", BUILDINGS.PREFABS.CANVAS.EXCELLENTQUALITYNAME, "painting_tall_art_f_kanim", "art_f", 15, true, Db.Get().ArtableStatuses.Great, "CanvasTall", "canvas");
			this.Add("CanvasWide_Bad", BUILDINGS.PREFABS.CANVAS.POORQUALITYNAME, "painting_wide_art_a_kanim", "art_a", 5, false, Db.Get().ArtableStatuses.Ugly, "CanvasWide", "canvas");
			this.Add("CanvasWide_Average", BUILDINGS.PREFABS.CANVAS.AVERAGEQUALITYNAME, "painting_wide_art_b_kanim", "art_b", 10, false, Db.Get().ArtableStatuses.Okay, "CanvasWide", "canvas");
			this.Add("CanvasWide_Good", BUILDINGS.PREFABS.CANVAS.EXCELLENTQUALITYNAME, "painting_wide_art_c_kanim", "art_c", 15, true, Db.Get().ArtableStatuses.Great, "CanvasWide", "canvas");
			this.Add("CanvasWide_Good2", BUILDINGS.PREFABS.CANVAS.EXCELLENTQUALITYNAME, "painting_wide_art_d_kanim", "art_d", 15, true, Db.Get().ArtableStatuses.Great, "CanvasWide", "canvas");
			this.Add("CanvasWide_Good3", BUILDINGS.PREFABS.CANVAS.EXCELLENTQUALITYNAME, "painting_wide_art_e_kanim", "art_e", 15, true, Db.Get().ArtableStatuses.Great, "CanvasWide", "canvas");
			this.Add("CanvasWide_Good4", BUILDINGS.PREFABS.CANVAS.EXCELLENTQUALITYNAME, "painting_wide_art_f_kanim", "art_f", 15, true, Db.Get().ArtableStatuses.Great, "CanvasWide", "canvas");
			this.Add("Sculpture_Bad", BUILDINGS.PREFABS.SMALLSCULPTURE.POORQUALITYNAME, "sculpture_crap_1_kanim", "crap_1", 5, false, Db.Get().ArtableStatuses.Ugly, "Sculpture", "");
			this.Add("Sculpture_Average", BUILDINGS.PREFABS.SMALLSCULPTURE.AVERAGEQUALITYNAME, "sculpture_good_1_kanim", "good_1", 10, false, Db.Get().ArtableStatuses.Okay, "Sculpture", "");
			this.Add("Sculpture_Good1", BUILDINGS.PREFABS.SMALLSCULPTURE.EXCELLENTQUALITYNAME, "sculpture_amazing_1_kanim", "amazing_1", 15, true, Db.Get().ArtableStatuses.Great, "Sculpture", "");
			this.Add("Sculpture_Good2", BUILDINGS.PREFABS.SMALLSCULPTURE.EXCELLENTQUALITYNAME, "sculpture_amazing_2_kanim", "amazing_2", 15, true, Db.Get().ArtableStatuses.Great, "Sculpture", "");
			this.Add("Sculpture_Good3", BUILDINGS.PREFABS.SMALLSCULPTURE.EXCELLENTQUALITYNAME, "sculpture_amazing_3_kanim", "amazing_3", 15, true, Db.Get().ArtableStatuses.Great, "Sculpture", "");
			this.Add("SmallSculpture_Bad", BUILDINGS.PREFABS.SMALLSCULPTURE.POORQUALITYNAME, "sculpture_1x2_crap_1_kanim", "crap_1", 5, false, Db.Get().ArtableStatuses.Ugly, "SmallSculpture", "");
			this.Add("SmallSculpture_Average", BUILDINGS.PREFABS.SMALLSCULPTURE.AVERAGEQUALITYNAME, "sculpture_1x2_good_1_kanim", "good_1", 10, false, Db.Get().ArtableStatuses.Okay, "SmallSculpture", "");
			this.Add("SmallSculpture_Good", BUILDINGS.PREFABS.SMALLSCULPTURE.EXCELLENTQUALITYNAME, "sculpture_1x2_amazing_1_kanim", "amazing_1", 15, true, Db.Get().ArtableStatuses.Great, "SmallSculpture", "");
			this.Add("SmallSculpture_Good2", BUILDINGS.PREFABS.SMALLSCULPTURE.EXCELLENTQUALITYNAME, "sculpture_1x2_amazing_2_kanim", "amazing_2", 15, true, Db.Get().ArtableStatuses.Great, "SmallSculpture", "");
			this.Add("SmallSculpture_Good3", BUILDINGS.PREFABS.SMALLSCULPTURE.EXCELLENTQUALITYNAME, "sculpture_1x2_amazing_3_kanim", "amazing_3", 15, true, Db.Get().ArtableStatuses.Great, "SmallSculpture", "");
			this.Add("IceSculpture_Bad", BUILDINGS.PREFABS.ICESCULPTURE.POORQUALITYNAME, "icesculpture_crap_kanim", "crap", 5, false, Db.Get().ArtableStatuses.Ugly, "IceSculpture", "");
			this.Add("IceSculpture_Average", BUILDINGS.PREFABS.ICESCULPTURE.AVERAGEQUALITYNAME, "icesculpture_idle_kanim", "idle", 10, false, Db.Get().ArtableStatuses.Okay, "IceSculpture", "good");
			this.Add("MarbleSculpture_Bad", BUILDINGS.PREFABS.MARBLESCULPTURE.POORQUALITYNAME, "sculpture_marble_crap_1_kanim", "crap_1", 5, false, Db.Get().ArtableStatuses.Ugly, "MarbleSculpture", "");
			this.Add("MarbleSculpture_Average", BUILDINGS.PREFABS.MARBLESCULPTURE.AVERAGEQUALITYNAME, "sculpture_marble_good_1_kanim", "good_1", 10, false, Db.Get().ArtableStatuses.Okay, "MarbleSculpture", "");
			this.Add("MarbleSculpture_Good1", BUILDINGS.PREFABS.MARBLESCULPTURE.EXCELLENTQUALITYNAME, "sculpture_marble_amazing_1_kanim", "amazing_1", 15, true, Db.Get().ArtableStatuses.Great, "MarbleSculpture", "");
			this.Add("MarbleSculpture_Good2", BUILDINGS.PREFABS.MARBLESCULPTURE.EXCELLENTQUALITYNAME, "sculpture_marble_amazing_2_kanim", "amazing_2", 15, true, Db.Get().ArtableStatuses.Great, "MarbleSculpture", "");
			this.Add("MarbleSculpture_Good3", BUILDINGS.PREFABS.MARBLESCULPTURE.EXCELLENTQUALITYNAME, "sculpture_marble_amazing_3_kanim", "amazing_3", 15, true, Db.Get().ArtableStatuses.Great, "MarbleSculpture", "");
			this.Add("MetalSculpture_Bad", BUILDINGS.PREFABS.METALSCULPTURE.POORQUALITYNAME, "sculpture_metal_crap_1_kanim", "crap_1", 5, false, Db.Get().ArtableStatuses.Ugly, "MetalSculpture", "");
			this.Add("MetalSculpture_Average", BUILDINGS.PREFABS.METALSCULPTURE.AVERAGEQUALITYNAME, "sculpture_metal_good_1_kanim", "good_1", 10, false, Db.Get().ArtableStatuses.Okay, "MetalSculpture", "");
			this.Add("MetalSculpture_Good1", BUILDINGS.PREFABS.METALSCULPTURE.EXCELLENTQUALITYNAME, "sculpture_metal_amazing_1_kanim", "amazing_1", 15, true, Db.Get().ArtableStatuses.Great, "MetalSculpture", "");
			this.Add("MetalSculpture_Good2", BUILDINGS.PREFABS.METALSCULPTURE.EXCELLENTQUALITYNAME, "sculpture_metal_amazing_2_kanim", "amazing_2", 15, true, Db.Get().ArtableStatuses.Great, "MetalSculpture", "");
			this.Add("MetalSculpture_Good3", BUILDINGS.PREFABS.METALSCULPTURE.EXCELLENTQUALITYNAME, "sculpture_metal_amazing_3_kanim", "amazing_3", 15, true, Db.Get().ArtableStatuses.Great, "MetalSculpture", "");
		}

		public List<ArtableStage> GetPrefabStages(Tag prefab_id)
		{
			return this.resources.FindAll((ArtableStage stage) => stage.prefabId == prefab_id);
		}

		public ArtableStage DefaultPrefabStage(Tag prefab_id)
		{
			return this.GetPrefabStages(prefab_id).Find((ArtableStage stage) => stage.statusItem == Db.Get().ArtableStatuses.Ready);
		}
	}
}
