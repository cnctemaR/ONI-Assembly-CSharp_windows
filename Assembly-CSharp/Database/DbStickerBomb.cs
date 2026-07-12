using System;

namespace Database
{
	public class DbStickerBomb : PermitResource
	{
		public DbStickerBomb(string id, string stickerName, PermitRarity rarity, string animfilename, string sticker)
			: base(id, stickerName, "TODO:DbStickers", PermitCategory.Artwork, rarity, DlcManager.AVAILABLE_ALL_VERSIONS)
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
