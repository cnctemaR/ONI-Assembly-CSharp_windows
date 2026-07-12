using System;

namespace Database
{
	public class DbStickerBomb : Resource
	{
		public DbStickerBomb(string id, string stickerName, string animfilename, string sticker)
			: base(id, id)
		{
			this.id = id;
			this.stickerName = stickerName;
			this.sticker = sticker;
			this.animFile = Assets.GetAnim(animfilename);
		}

		public string id;

		public string stickerName;

		public string sticker;

		public KAnimFile animFile;

		private const string stickerAnimPrefix = "idle_sticker";

		private const string stickerSymbolPrefix = "sticker";
	}
}
