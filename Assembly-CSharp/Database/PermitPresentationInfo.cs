using System;
using STRINGS;
using UnityEngine;

namespace Database
{
	public struct PermitPresentationInfo
	{
		public string facadeFor { readonly get; private set; }

		public static Sprite GetUnknownSprite()
		{
			return Assets.GetSprite("unknown");
		}

		public void SetFacadeForPrefabName(string prefabName)
		{
			this.facadeFor = UI.KLEI_INVENTORY_SCREEN.ITEM_FACADE_FOR.Replace("{ConfigProperName}", prefabName);
		}

		public void SetFacadeForPrefabID(string prefabId)
		{
			if (Assets.TryGetPrefab(prefabId) == null)
			{
				this.facadeFor = UI.KLEI_INVENTORY_SCREEN.ITEM_DLC_REQUIRED;
				return;
			}
			this.facadeFor = UI.KLEI_INVENTORY_SCREEN.ITEM_FACADE_FOR.Replace("{ConfigProperName}", Assets.GetPrefab(prefabId).GetProperName());
		}

		public void SetFacadeForText(string text)
		{
			this.facadeFor = text;
		}

		public Sprite sprite;
	}
}
