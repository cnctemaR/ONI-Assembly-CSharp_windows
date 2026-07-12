using System;
using UnityEngine;

namespace Database
{
	public class ClothingOutfitResource : Resource
	{
		public string[] itemsInOutfit { get; private set; }

		public ClothingOutfitResource(string id, string[] items_in_outfit, LocString name, ClothingOutfitUtility.OutfitType outfitType)
			: base(id, name)
		{
			this.itemsInOutfit = items_in_outfit;
			this.outfitType = outfitType;
			string[] itemsInOutfit = this.itemsInOutfit;
			for (int i = 0; i < itemsInOutfit.Length; i++)
			{
				string itemId = itemsInOutfit[i];
				int num = Array.FindIndex<ClothingItems.Info>(ClothingItems.Infos_All, (ClothingItems.Info e) => e.id == itemId);
				if (num < 0)
				{
					DebugUtil.DevAssert(false, string.Concat(new string[] { "Outfit \"", this.Id, "\" contains an item that doesn't exist. Given item id: \"", itemId, "\"" }), null);
				}
				else
				{
					ClothingItems.Info info = ClothingItems.Infos_All[num];
					if (info.outfitType != this.outfitType)
					{
						DebugUtil.DevAssert(false, string.Format("Outfit \"{0}\" contains an item that has a mis-matched outfit type. Defined outfit's type: \"{1}\". Given item: {{ id: \"{2}\" forOutfitType: \"{3}\" }}", new object[] { this.Id, this.outfitType, itemId, info.outfitType }), null);
					}
				}
			}
		}

		public global::Tuple<Sprite, Color> GetUISprite()
		{
			Sprite sprite = Assets.GetSprite("unknown");
			return new global::Tuple<Sprite, Color>(sprite, (sprite != null) ? Color.white : Color.clear);
		}

		public ClothingOutfitUtility.OutfitType outfitType;
	}
}
