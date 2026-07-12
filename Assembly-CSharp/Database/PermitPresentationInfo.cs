using System;
using STRINGS;
using UnityEngine;

namespace Database
{
	public struct PermitPresentationInfo
	{
		public string facadeFor { readonly get; private set; }

		public string rarityDetails { readonly get; private set; }

		public static Sprite GetUnknownSprite()
		{
			return Assets.GetSprite("unknown");
		}

		public bool IsOwnable()
		{
			return this.ownedCount.HasValue;
		}

		public bool IsUnlocked()
		{
			return !this.ownedCount.HasValue || 0 < this.ownedCount.Value;
		}

		public void SetFacadeForPrefabName(string prefabName)
		{
			this.facadeFor = UI.KLEI_INVENTORY_SCREEN.ITEM_FACADE_FOR.Replace("{ConfigProperName}", prefabName);
		}

		public void SetFacadeForPrefabID(string prefabId)
		{
			this.facadeFor = UI.KLEI_INVENTORY_SCREEN.ITEM_FACADE_FOR.Replace("{ConfigProperName}", Assets.GetPrefab(prefabId).GetProperName());
		}

		public void SetFacadeForText(string text)
		{
			this.facadeFor = text;
		}

		public void SetRarityDetailsFor(PermitRarity rarity)
		{
			this.rarityDetails = UI.KLEI_INVENTORY_SCREEN.ITEM_RARITY_DETAILS.Replace("{RarityName}", rarity.GetLocStringName());
		}

		public void SetRarityDetailsText(string text)
		{
			this.rarityDetails = text;
		}

		public string name;

		public string description;

		public PermitCategory category;

		public Sprite sprite;

		public string buildOverride;

		public Option<int> ownedCount;

		public bool isNone;
	}
}
