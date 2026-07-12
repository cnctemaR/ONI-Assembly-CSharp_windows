using System;

namespace Database
{
	public class StickerBombs : ResourceSet<DbStickerBomb>
	{
		public StickerBombs(ResourceSet parent)
			: base("StickerBombs", parent)
		{
			foreach (StickerBombFacadeInfo stickerBombFacadeInfo in Blueprints.Get().all.stickerBombFacades)
			{
				this.Add(stickerBombFacadeInfo.id, stickerBombFacadeInfo.name, stickerBombFacadeInfo.desc, stickerBombFacadeInfo.rarity, stickerBombFacadeInfo.animFile, stickerBombFacadeInfo.sticker, stickerBombFacadeInfo.dlcIds);
			}
		}

		[Obsolete("Please use Add(...) with dlcIds parameter")]
		private DbStickerBomb Add(string id, string name, string desc, PermitRarity rarity, string animfilename, string symbolName)
		{
			return this.Add(id, name, desc, rarity, animfilename, symbolName, DlcManager.AVAILABLE_ALL_VERSIONS);
		}

		private DbStickerBomb Add(string id, string name, string desc, PermitRarity rarity, string animfilename, string symbolName, string[] dlcIds)
		{
			DbStickerBomb dbStickerBomb = new DbStickerBomb(id, name, desc, rarity, animfilename, symbolName, dlcIds);
			this.resources.Add(dbStickerBomb);
			return dbStickerBomb;
		}

		public DbStickerBomb GetRandomSticker()
		{
			return this.resources.GetRandom<DbStickerBomb>();
		}
	}
}
