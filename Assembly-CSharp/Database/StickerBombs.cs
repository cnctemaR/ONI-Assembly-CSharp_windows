using System;
using STRINGS;

namespace Database
{
	public class StickerBombs : ResourceSet<DbStickerBomb>
	{
		public StickerBombs(ResourceSet parent)
			: base("StickerBombs", parent)
		{
			foreach (StickerBombs.Info info in StickerBombs.Infos)
			{
				this.Add(info.id, info.stickerName, info.animfilename, info.sticker);
			}
		}

		private DbStickerBomb Add(string id, string stickerType, string animfilename, string symbolName)
		{
			DbStickerBomb dbStickerBomb = new DbStickerBomb(id, stickerType, animfilename, symbolName);
			this.resources.Add(dbStickerBomb);
			return dbStickerBomb;
		}

		public DbStickerBomb GetRandomSticker()
		{
			return this.resources.GetRandom<DbStickerBomb>();
		}

		public static StickerBombs.Info[] Infos = new StickerBombs.Info[]
		{
			new StickerBombs.Info("a", STICKERNAMES.STICKER_A, "sticker_a_kanim", "a"),
			new StickerBombs.Info("b", STICKERNAMES.STICKER_B, "sticker_b_kanim", "b"),
			new StickerBombs.Info("c", STICKERNAMES.STICKER_C, "sticker_c_kanim", "c"),
			new StickerBombs.Info("d", STICKERNAMES.STICKER_D, "sticker_d_kanim", "d"),
			new StickerBombs.Info("e", STICKERNAMES.STICKER_E, "sticker_e_kanim", "e"),
			new StickerBombs.Info("f", STICKERNAMES.STICKER_F, "sticker_f_kanim", "f"),
			new StickerBombs.Info("g", STICKERNAMES.STICKER_G, "sticker_g_kanim", "g"),
			new StickerBombs.Info("h", STICKERNAMES.STICKER_H, "sticker_h_kanim", "h"),
			new StickerBombs.Info("rocket", STICKERNAMES.STICKER_ROCKET, "sticker_rocket_kanim", "rocket"),
			new StickerBombs.Info("paperplane", STICKERNAMES.STICKER_PAPERPLANE, "sticker_paperplane_kanim", "paperplane"),
			new StickerBombs.Info("plant", STICKERNAMES.STICKER_PLANT, "sticker_plant_kanim", "plant"),
			new StickerBombs.Info("plantpot", STICKERNAMES.STICKER_PLANTPOT, "sticker_plantpot_kanim", "plantpot"),
			new StickerBombs.Info("mushroom", STICKERNAMES.STICKER_MUSHROOM, "sticker_mushroom_kanim", "mushroom"),
			new StickerBombs.Info("mermaid", STICKERNAMES.STICKER_MERMAID, "sticker_mermaid_kanim", "mermaid"),
			new StickerBombs.Info("spacepet", STICKERNAMES.STICKER_SPACEPET, "sticker_spacepet_kanim", "spacepet"),
			new StickerBombs.Info("spacepet2", STICKERNAMES.STICKER_SPACEPET2, "sticker_spacepet2_kanim", "spacepet2"),
			new StickerBombs.Info("spacepet3", STICKERNAMES.STICKER_SPACEPET3, "sticker_spacepet3_kanim", "spacepet3"),
			new StickerBombs.Info("spacepet4", STICKERNAMES.STICKER_SPACEPET4, "sticker_spacepet4_kanim", "spacepet4"),
			new StickerBombs.Info("spacepet5", STICKERNAMES.STICKER_SPACEPET5, "sticker_spacepet5_kanim", "spacepet5"),
			new StickerBombs.Info("unicorn", STICKERNAMES.STICKER_UNICORN, "sticker_unicorn_kanim", "unicorn")
		};

		public struct Info
		{
			public Info(string id, string stickerName, string animfilename, string sticker)
			{
				this.id = id;
				this.stickerName = stickerName;
				this.animfilename = animfilename;
				this.sticker = sticker;
			}

			public string id;

			public string stickerName;

			public string animfilename;

			public string sticker;
		}
	}
}
