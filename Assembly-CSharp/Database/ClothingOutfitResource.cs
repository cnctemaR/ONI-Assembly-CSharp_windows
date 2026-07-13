using System;
using System.Linq;
using UnityEngine;

namespace Database
{
	public class ClothingOutfitResource : Resource, IHasDlcRestrictions
	{
		public string[] itemsInOutfit { get; private set; }

		public ClothingOutfitResource(string id, string[] items_in_outfit, string name, ClothingOutfitUtility.OutfitType outfitType, string[] requiredDlcIds = null, string[] forbiddenDlcIds = null)
			: base(id, name)
		{
			this.itemsInOutfit = items_in_outfit;
			this.outfitType = outfitType;
			this.requiredDlcIds = requiredDlcIds;
			this.forbiddenDlcIds = forbiddenDlcIds;
		}

		public global::Tuple<Sprite, Color> GetUISprite()
		{
			Sprite sprite = Assets.GetSprite("unknown");
			return new global::Tuple<Sprite, Color>(sprite, (sprite != null) ? Color.white : Color.clear);
		}

		public string GetDlcIdFrom()
		{
			if (this.requiredDlcIds == null)
			{
				return null;
			}
			return this.requiredDlcIds.Last<string>();
		}

		public string[] GetRequiredDlcIds()
		{
			return this.requiredDlcIds;
		}

		public string[] GetForbiddenDlcIds()
		{
			return this.forbiddenDlcIds;
		}

		public ClothingOutfitUtility.OutfitType outfitType;

		public string[] requiredDlcIds;

		public string[] forbiddenDlcIds;
	}
}
