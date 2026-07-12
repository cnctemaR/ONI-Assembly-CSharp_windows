using System;

namespace Database
{
	public class DbStickerBomb : PermitResource
	{
		[Obsolete("Please use constructor with dlcIds parameter")]
		public DbStickerBomb(string id, string name, string desc, PermitRarity rarity, string animfilename, string sticker)
			: this(id, name, desc, rarity, animfilename, sticker, DlcManager.AVAILABLE_ALL_VERSIONS)
		{
		}

		public DbStickerBomb(string id, string name, string desc, PermitRarity rarity, string animfilename, string sticker, string[] dlcIds)
			: base(id, name, desc, PermitCategory.Artwork, rarity, dlcIds)
		{
			this.id = id;
			this.sticker = sticker;
			this.animFile = Assets.GetAnim(animfilename);
		}

		public override PermitPresentationInfo GetPermitPresentationInfo()
		{
			return new PermitPresentationInfo
			{
				sprite = Def.GetUISpriteFromMultiObjectAnim(this.animFile, string.Format("{0}_{1}", "idle_sticker", this.sticker), false, string.Format("{0}_{1}", "sticker", this.sticker))
			};
		}

		public string id;

		public string sticker;

		public KAnimFile animFile;

		private const string stickerAnimPrefix = "idle_sticker";

		private const string stickerSymbolPrefix = "sticker";
	}
}
