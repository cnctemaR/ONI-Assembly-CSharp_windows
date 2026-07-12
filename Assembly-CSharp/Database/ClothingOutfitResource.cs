using System;
using UnityEngine;

namespace Database
{
	public class ClothingOutfitResource : Resource
	{
		public string[] itemsInOutfit { get; private set; }

		public ClothingOutfitResource(string id, string[] items_in_outfit)
			: base(id, null, null)
		{
			this.itemsInOutfit = items_in_outfit;
		}

		public ClothingOutfitResource(string id, string[] items_in_outfit, LocString name)
			: base(name, null, null)
		{
			this.itemsInOutfit = items_in_outfit;
		}

		public global::Tuple<Sprite, Color> GetUISprite()
		{
			Sprite sprite = Assets.GetSprite("unknown");
			return new global::Tuple<Sprite, Color>(sprite, (sprite != null) ? Color.white : Color.clear);
		}
	}
}
