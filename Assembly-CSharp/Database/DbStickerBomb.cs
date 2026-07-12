using System;

namespace Database
{
	public class DbStickerBomb : PermitResource
	{
		public DbStickerBomb(string id, string stickerName, string animfilename, string sticker)
			: base(id, PermitCategory.Artwork, PermitRarity.Unknown)
		{
			this.id = id;
			this.stickerName = stickerName;
			this.sticker = sticker;
			this.animFile = Assets.GetAnim(animfilename);
		}

		public override PermitPresentationInfo GetPermitPresentationInfo()
		{
			PermitPresentationInfo permitPresentationInfo = default(PermitPresentationInfo);
			permitPresentationInfo.name = this.stickerName;
			permitPresentationInfo.sprite = Def.GetUISpriteFromMultiObjectAnim(this.animFile, string.Format("{0}_{1}", "idle_sticker", this.sticker), false, string.Format("{0}_{1}", "sticker", this.sticker));
			permitPresentationInfo.category = this.PermitCategory;
			permitPresentationInfo.SetRarityDetailsFor(this.Rarity);
			permitPresentationInfo.ownedCount = PermitItems.GetOwnedCount(this);
			return permitPresentationInfo;
		}

		public string id;

		public string stickerName;

		public string sticker;

		public KAnimFile animFile;

		private const string stickerAnimPrefix = "idle_sticker";

		private const string stickerSymbolPrefix = "sticker";
	}
}
