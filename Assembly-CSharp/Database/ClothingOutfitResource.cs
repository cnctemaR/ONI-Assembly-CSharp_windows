using System;
using UnityEngine;

namespace Database
{
	public class ClothingOutfitResource : Resource, IBlueprintDlcInfo
	{
		public string[] itemsInOutfit { get; private set; }

		public string[] dlcIds { get; set; } = DlcManager.AVAILABLE_ALL_VERSIONS;

		public ClothingOutfitResource(string id, string[] items_in_outfit, string name, ClothingOutfitUtility.OutfitType outfitType)
			: base(id, name)
		{
			this.itemsInOutfit = items_in_outfit;
			this.outfitType = outfitType;
		}

		public global::Tuple<Sprite, Color> GetUISprite()
		{
			Sprite sprite = Assets.GetSprite("unknown");
			return new global::Tuple<Sprite, Color>(sprite, (sprite != null) ? Color.white : Color.clear);
		}

		public string GetDlcIdFrom()
		{
			if (this.dlcIds == DlcManager.AVAILABLE_ALL_VERSIONS || this.dlcIds == DlcManager.AVAILABLE_VANILLA_ONLY)
			{
				return null;
			}
			if (this.dlcIds.Length == 0)
			{
				return null;
			}
			return this.dlcIds[0];
		}

		public ClothingOutfitUtility.OutfitType outfitType;
	}
}
