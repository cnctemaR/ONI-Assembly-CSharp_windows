using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

namespace Database
{
	public class ClothingItemResource : PermitResource
	{
		public string animFilename { get; private set; }

		public KAnimFile AnimFile { get; private set; }

		public ClothingItemResource(string id, string name, string desc, PermitCategory category, PermitRarity rarity, string animFile)
			: base(id, name, desc, category, rarity)
		{
			this.AnimFile = Assets.GetAnim(animFile);
			this.animFilename = animFile;
		}

		public global::Tuple<Sprite, Color> GetUISprite()
		{
			if (this.AnimFile == null)
			{
				global::Debug.LogError("Clothing AnimFile is null: " + this.animFilename);
			}
			Sprite uisprite = ClothingItemResource.GetUISprite(this.AnimFile);
			return new global::Tuple<Sprite, Color>(uisprite, (uisprite != null) ? Color.white : Color.clear);
		}

		public override PermitPresentationInfo GetPermitPresentationInfo()
		{
			PermitPresentationInfo permitPresentationInfo = default(PermitPresentationInfo);
			permitPresentationInfo.sprite = this.GetUISprite().first;
			permitPresentationInfo.SetFacadeForText(UI.KLEI_INVENTORY_SCREEN.CLOTHING_ITEM_FACADE_FOR);
			return permitPresentationInfo;
		}

		public static Sprite GetUISprite(KAnimFile animFile)
		{
			if (ClothingItemResource.knownNoSpriteAvailble.Contains(animFile))
			{
				return Assets.GetSprite("unknown");
			}
			Sprite sprite;
			if (ClothingItemResource.knownUISprites.TryGetValue(animFile, out sprite))
			{
				return sprite;
			}
			Option<Sprite> option = ClothingItemResource.MaybeGenerateUISprite(animFile);
			if (option.HasValue)
			{
				ClothingItemResource.knownUISprites.Add(animFile, option.Value);
				return option.Value;
			}
			ClothingItemResource.knownNoSpriteAvailble.Add(animFile);
			return Assets.GetSprite("unknown");
		}

		public static Option<Sprite> MaybeGenerateUISprite(KAnimFile animFile)
		{
			if (animFile == null)
			{
				return default(Option<Sprite>);
			}
			if (animFile.GetData() == null)
			{
				return default(Option<Sprite>);
			}
			KAnim.Build build = animFile.GetData().build;
			if (build.textureCount == 0)
			{
				return default(Option<Sprite>);
			}
			Texture2D texture = build.GetTexture(0);
			if (texture == null || !texture)
			{
				return default(Option<Sprite>);
			}
			KAnim.Build.Symbol symbol = build.GetSymbol("ui");
			if (symbol == null)
			{
				return default(Option<Sprite>);
			}
			int firstFrameIdx = symbol.firstFrameIdx;
			if (firstFrameIdx < 0 || firstFrameIdx >= build.frames.Length)
			{
				return default(Option<Sprite>);
			}
			KAnim.Build.SymbolFrame symbolFrame = build.frames[firstFrameIdx];
			float x = symbolFrame.uvMin.x;
			float x2 = symbolFrame.uvMax.x;
			float y = symbolFrame.uvMax.y;
			float y2 = symbolFrame.uvMin.y;
			Rect rect = new Rect
			{
				x = (float)((int)((float)texture.width * x)),
				y = (float)((int)((float)texture.height * y)),
				width = (float)((int)((float)texture.width * Mathf.Abs(x2 - x))),
				height = (float)((int)((float)texture.height * Mathf.Abs(y2 - y)))
			};
			float num = 100f;
			if (rect.width != 0f)
			{
				float num2 = Mathf.Abs(symbolFrame.bboxMax.x - symbolFrame.bboxMin.x);
				num = 100f / (num2 / rect.width);
			}
			Sprite sprite = Sprite.Create(texture, rect, Vector2.zero, num, 0U, SpriteMeshType.FullRect);
			sprite.name = string.Format("{0}:{1}:{2}:{3}", new object[] { texture.name, animFile.name, symbolFrame.sourceFrameNum, false });
			return sprite;
		}

		private static Dictionary<KAnimFile, Sprite> knownUISprites = new Dictionary<KAnimFile, Sprite>();

		private static HashSet<KAnimFile> knownNoSpriteAvailble = new HashSet<KAnimFile>();
	}
}
