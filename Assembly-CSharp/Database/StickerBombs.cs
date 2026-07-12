using System;
using STRINGS;

namespace Database
{
	public class StickerBombs : ResourceSet<DbStickerBomb>
	{
		private DbStickerBomb Add(string id, string stickerType, string animfilename, string symbolName)
		{
			DbStickerBomb dbStickerBomb = new DbStickerBomb(id, stickerType, animfilename, symbolName);
			this.resources.Add(dbStickerBomb);
			return dbStickerBomb;
		}

		public StickerBombs(ResourceSet parent)
			: base("StickerBombs", parent)
		{
			this.Add("a", STICKERNAMES.STICKER_A, "sticker_a_kanim", "a");
			this.Add("b", STICKERNAMES.STICKER_B, "sticker_b_kanim", "b");
			this.Add("c", STICKERNAMES.STICKER_C, "sticker_c_kanim", "c");
			this.Add("d", STICKERNAMES.STICKER_D, "sticker_d_kanim", "d");
			this.Add("e", STICKERNAMES.STICKER_E, "sticker_e_kanim", "e");
			this.Add("f", STICKERNAMES.STICKER_F, "sticker_f_kanim", "f");
			this.Add("g", STICKERNAMES.STICKER_G, "sticker_g_kanim", "g");
			this.Add("h", STICKERNAMES.STICKER_H, "sticker_h_kanim", "h");
			this.Add("rocket", STICKERNAMES.STICKER_ROCKET, "sticker_rocket_kanim", "rocket");
			this.Add("paperplane", STICKERNAMES.STICKER_PAPERPLANE, "sticker_paperplane_kanim", "paperplane");
			this.Add("plant", STICKERNAMES.STICKER_PLANT, "sticker_plant_kanim", "plant");
			this.Add("plantpot", STICKERNAMES.STICKER_PLANTPOT, "sticker_plantpot_kanim", "plantpot");
			this.Add("mushroom", STICKERNAMES.STICKER_MUSHROOM, "sticker_mushroom_kanim", "mushroom");
			this.Add("mermaid", STICKERNAMES.STICKER_MERMAID, "sticker_mermaid_kanim", "mermaid");
			this.Add("spacepet", STICKERNAMES.STICKER_SPACEPET, "sticker_spacepet_kanim", "spacepet");
			this.Add("spacepet2", STICKERNAMES.STICKER_SPACEPET2, "sticker_spacepet2_kanim", "spacepet2");
			this.Add("spacepet3", STICKERNAMES.STICKER_SPACEPET3, "sticker_spacepet3_kanim", "spacepet3");
			this.Add("spacepet4", STICKERNAMES.STICKER_SPACEPET4, "sticker_spacepet4_kanim", "spacepet4");
			this.Add("spacepet5", STICKERNAMES.STICKER_SPACEPET5, "sticker_spacepet5_kanim", "spacepet5");
			this.Add("unicorn", STICKERNAMES.STICKER_UNICORN, "sticker_unicorn_kanim", "unicorn");
		}

		public DbStickerBomb GetRandomSticker()
		{
			return this.resources.GetRandom<DbStickerBomb>();
		}
	}
}
