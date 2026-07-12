using System;
using UnityEngine;

namespace Database
{
	public class EquippableFacadeResource : PermitResource
	{
		public string BuildOverride { get; private set; }

		public string DefID { get; private set; }

		public KAnimFile AnimFile { get; private set; }

		public EquippableFacadeResource(string id, string name, string buildOverride, string defID, string animFile)
			: base(id, name, PermitCategory.Equipment, PermitRarity.Unknown)
		{
			this.DefID = defID;
			this.BuildOverride = buildOverride;
			this.AnimFile = Assets.GetAnim(animFile);
		}

		public global::Tuple<Sprite, Color> GetUISprite()
		{
			if (this.AnimFile == null)
			{
				global::Debug.LogError("Facade AnimFile is null: " + this.DefID);
			}
			Sprite uispriteFromMultiObjectAnim = Def.GetUISpriteFromMultiObjectAnim(this.AnimFile, "ui", false, "");
			return new global::Tuple<Sprite, Color>(uispriteFromMultiObjectAnim, (uispriteFromMultiObjectAnim != null) ? Color.white : Color.clear);
		}

		public override PermitPresentationInfo GetPermitPresentationInfo()
		{
			PermitPresentationInfo permitPresentationInfo = default(PermitPresentationInfo);
			permitPresentationInfo.name = Strings.Get("STRINGS.EQUIPMENT.PREFABS." + this.DefID.ToUpper() + ".FACADES." + this.Name.ToUpper());
			permitPresentationInfo.sprite = this.GetUISprite().first;
			permitPresentationInfo.category = this.PermitCategory;
			permitPresentationInfo.buildOverride = this.BuildOverride;
			permitPresentationInfo.SetRarityDetailsFor(this.Rarity);
			permitPresentationInfo.ownedCount = PermitItems.GetOwnedCount(this);
			GameObject gameObject = Assets.TryGetPrefab(this.DefID);
			if (gameObject == null || !gameObject)
			{
				permitPresentationInfo.SetFacadeForPrefabID(this.DefID);
			}
			else
			{
				permitPresentationInfo.SetFacadeForPrefabName(gameObject.GetProperName());
			}
			return permitPresentationInfo;
		}
	}
}
