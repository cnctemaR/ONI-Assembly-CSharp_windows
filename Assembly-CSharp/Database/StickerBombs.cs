using System;
using STRINGS;

namespace Database
{
	public class StickerBombs : ResourceSet<DbStickerBomb>
	{
		public StickerBombs(ResourceSet parent)
			: base("StickerBombs", parent)
		{
			foreach (StickerBombs.Info info in StickerBombs.Infos_All)
			{
				this.Add(info.id, info.stickerName, info.rarity, info.animfilename, info.sticker);
			}
		}

		private DbStickerBomb Add(string id, string stickerType, PermitRarity rarity, string animfilename, string symbolName)
		{
			DbStickerBomb dbStickerBomb = new DbStickerBomb(id, stickerType, rarity, animfilename, symbolName);
			this.resources.Add(dbStickerBomb);
			return dbStickerBomb;
		}

		public DbStickerBomb GetRandomSticker()
		{
			return this.resources.GetRandom<DbStickerBomb>();
		}

		public static StickerBombs.Info[] Infos_Default = new StickerBombs.Info[]
		{
			new StickerBombs.Info("a", STICKERNAMES.STICKER_A, PermitRarity.Universal, "sticker_a_kanim", "a"),
			new StickerBombs.Info("b", STICKERNAMES.STICKER_B, PermitRarity.Universal, "sticker_b_kanim", "b"),
			new StickerBombs.Info("c", STICKERNAMES.STICKER_C, PermitRarity.Universal, "sticker_c_kanim", "c"),
			new StickerBombs.Info("d", STICKERNAMES.STICKER_D, PermitRarity.Universal, "sticker_d_kanim", "d"),
			new StickerBombs.Info("e", STICKERNAMES.STICKER_E, PermitRarity.Universal, "sticker_e_kanim", "e"),
			new StickerBombs.Info("f", STICKERNAMES.STICKER_F, PermitRarity.Universal, "sticker_f_kanim", "f"),
			new StickerBombs.Info("g", STICKERNAMES.STICKER_G, PermitRarity.Universal, "sticker_g_kanim", "g"),
			new StickerBombs.Info("h", STICKERNAMES.STICKER_H, PermitRarity.Universal, "sticker_h_kanim", "h"),
			new StickerBombs.Info("rocket", STICKERNAMES.STICKER_ROCKET, PermitRarity.Universal, "sticker_rocket_kanim", "rocket"),
			new StickerBombs.Info("paperplane", STICKERNAMES.STICKER_PAPERPLANE, PermitRarity.Universal, "sticker_paperplane_kanim", "paperplane"),
			new StickerBombs.Info("plant", STICKERNAMES.STICKER_PLANT, PermitRarity.Universal, "sticker_plant_kanim", "plant"),
			new StickerBombs.Info("plantpot", STICKERNAMES.STICKER_PLANTPOT, PermitRarity.Universal, "sticker_plantpot_kanim", "plantpot"),
			new StickerBombs.Info("mushroom", STICKERNAMES.STICKER_MUSHROOM, PermitRarity.Universal, "sticker_mushroom_kanim", "mushroom"),
			new StickerBombs.Info("mermaid", STICKERNAMES.STICKER_MERMAID, PermitRarity.Universal, "sticker_mermaid_kanim", "mermaid"),
			new StickerBombs.Info("spacepet", STICKERNAMES.STICKER_SPACEPET, PermitRarity.Universal, "sticker_spacepet_kanim", "spacepet"),
			new StickerBombs.Info("spacepet2", STICKERNAMES.STICKER_SPACEPET2, PermitRarity.Universal, "sticker_spacepet2_kanim", "spacepet2"),
			new StickerBombs.Info("spacepet3", STICKERNAMES.STICKER_SPACEPET3, PermitRarity.Universal, "sticker_spacepet3_kanim", "spacepet3"),
			new StickerBombs.Info("spacepet4", STICKERNAMES.STICKER_SPACEPET4, PermitRarity.Universal, "sticker_spacepet4_kanim", "spacepet4"),
			new StickerBombs.Info("spacepet5", STICKERNAMES.STICKER_SPACEPET5, PermitRarity.Universal, "sticker_spacepet5_kanim", "spacepet5"),
			new StickerBombs.Info("unicorn", STICKERNAMES.STICKER_UNICORN, PermitRarity.Universal, "sticker_unicorn_kanim", "unicorn")
		};

		public static StickerBombs.Info[] Infos_Skins = new StickerBombs.Info[0];

		public static StickerBombs.Info[] Infos_All = StickerBombs.Infos_Default.Concat<StickerBombs.Info>(StickerBombs.Infos_Skins);

		public struct Info
		{
			public Info(string id, string stickerName, PermitRarity rarity, string animfilename, string sticker)
			{
				this.id = id;
				this.stickerName = stickerName;
				this.rarity = rarity;
				this.animfilename = animfilename;
				this.sticker = sticker;
			}

			public string id;

			public string stickerName;

			public PermitRarity rarity;

			public string animfilename;

			public string sticker;
		}
	}
}
