using System;
using UnityEngine;

namespace Database
{
	public class MonumentPartResource : PermitResource
	{
		public KAnimFile AnimFile { get; private set; }

		public string SymbolName { get; private set; }

		public string State { get; private set; }

		public MonumentPartResource(string id, string animFilename, string state, string symbolName, MonumentPartResource.Part part)
			: base(id, "TODO:DbMonumentParts", "TODO:DbMonumentParts", PermitCategory.Artwork, PermitRarity.Unknown, DlcManager.AVAILABLE_ALL_VERSIONS)
		{
			this.AnimFile = Assets.GetAnim(animFilename);
			this.SymbolName = symbolName;
			this.State = state;
			this.part = part;
		}

		public global::Tuple<Sprite, Color> GetUISprite()
		{
			Sprite sprite = Assets.GetSprite("unknown");
			return new global::Tuple<Sprite, Color>(sprite, (sprite != null) ? Color.white : Color.clear);
		}

		public override PermitPresentationInfo GetPermitPresentationInfo()
		{
			PermitPresentationInfo permitPresentationInfo = default(PermitPresentationInfo);
			permitPresentationInfo.sprite = this.GetUISprite().first;
			permitPresentationInfo.SetFacadeForText("_monument part");
			return permitPresentationInfo;
		}

		public MonumentPartResource.Part part;

		public enum Part
		{
			Bottom,
			Middle,
			Top
		}
	}
}
