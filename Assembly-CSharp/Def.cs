using System;
using System.Collections.Generic;
using Klei.AI;
using UnityEngine;

[Serializable]
public class Def : ScriptableObject
{
	public virtual void InitDef()
	{
		this.Tag = TagManager.Create(this.PrefabID);
	}

	public virtual string Name
	{
		get
		{
			return null;
		}
	}

	public static Tuple<Sprite, Color> GetUISprite(object item, string animName = "ui", bool centered = false)
	{
		if (item is Substance)
		{
			return Def.GetUISprite(ElementLoader.FindElementByHash((item as Substance).elementID), animName, centered);
		}
		if (item is Element)
		{
			if ((item as Element).IsSolid)
			{
				return new Tuple<Sprite, Color>(Def.GetUISpriteFromMultiObjectAnim((item as Element).substance.anim, animName, centered, string.Empty), Color.white);
			}
			if ((item as Element).IsLiquid)
			{
				return new Tuple<Sprite, Color>(Assets.GetSprite("element_liquid"), (item as Element).substance.uiColour);
			}
			if ((item as Element).IsGas)
			{
				return new Tuple<Sprite, Color>(Assets.GetSprite("element_gas"), (item as Element).substance.uiColour);
			}
			return new Tuple<Sprite, Color>(null, Color.clear);
		}
		else if (item is GameObject)
		{
			GameObject gameObject = item as GameObject;
			if (ElementLoader.GetElement(gameObject.PrefabID()) != null)
			{
				return Def.GetUISprite(ElementLoader.GetElement(gameObject.PrefabID()), animName, centered);
			}
			CreatureBrain creatureBrain = gameObject.GetComponent<CreatureBrain>();
			if (creatureBrain != null)
			{
				animName = creatureBrain.symbolPrefix + "ui";
			}
			SpaceArtifact component = gameObject.GetComponent<SpaceArtifact>();
			if (component != null)
			{
				animName = component.GetUIAnim();
			}
			if (gameObject.HasTag(GameTags.Egg))
			{
				IncubationMonitor.Def def = gameObject.GetDef<IncubationMonitor.Def>();
				if (def != null)
				{
					GameObject prefab = Assets.GetPrefab(def.spawnedCreature);
					if (prefab)
					{
						creatureBrain = prefab.GetComponent<CreatureBrain>();
						if (creatureBrain && !string.IsNullOrEmpty(creatureBrain.symbolPrefix))
						{
							animName = creatureBrain.symbolPrefix + animName;
						}
					}
				}
			}
			KBatchedAnimController component2 = gameObject.GetComponent<KBatchedAnimController>();
			if (component2)
			{
				Sprite uispriteFromMultiObjectAnim = Def.GetUISpriteFromMultiObjectAnim(component2.AnimFiles[0], animName, centered, string.Empty);
				return new Tuple<Sprite, Color>(uispriteFromMultiObjectAnim, (!(uispriteFromMultiObjectAnim != null)) ? Color.clear : Color.white);
			}
			if (gameObject.GetComponent<Building>() != null)
			{
				Sprite uisprite = gameObject.GetComponent<Building>().Def.GetUISprite(animName, centered);
				return new Tuple<Sprite, Color>(uisprite, (!(uisprite != null)) ? Color.clear : Color.white);
			}
			global::Debug.LogWarningFormat("Can't get sprite for type {0} (no KBatchedAnimController)", new object[] { item.ToString() });
			return null;
		}
		else
		{
			if (!(item is string))
			{
				if (item is Tag)
				{
					if (ElementLoader.GetElement((Tag)item) != null)
					{
						return Def.GetUISprite(ElementLoader.GetElement((Tag)item), animName, centered);
					}
					if (Assets.GetPrefab((Tag)item) != null)
					{
						return Def.GetUISprite(Assets.GetPrefab((Tag)item), animName, centered);
					}
					if (Assets.GetSprite(((Tag)item).Name) != null)
					{
						return new Tuple<Sprite, Color>(Assets.GetSprite(((Tag)item).Name), Color.white);
					}
				}
				DebugUtil.DevAssertArgs(false, new object[]
				{
					"Can't get sprite for type ",
					item.ToString()
				});
				return null;
			}
			if (Db.Get().Amounts.Exists(item as string))
			{
				Amount amount = Db.Get().Amounts.Get(item as string);
				return new Tuple<Sprite, Color>(Assets.GetSprite(amount.uiSprite), Color.white);
			}
			if (Db.Get().Attributes.Exists(item as string))
			{
				Klei.AI.Attribute attribute = Db.Get().Attributes.Get(item as string);
				return new Tuple<Sprite, Color>(Assets.GetSprite(attribute.uiSprite), Color.white);
			}
			return Def.GetUISprite((item as string).ToTag(), animName, centered);
		}
	}

	public static Sprite GetUISpriteFromMultiObjectAnim(KAnimFile animFile, string animName = "ui", bool centered = false, string symbolName = "")
	{
		Tuple<KAnimFile, string, bool> tuple = new Tuple<KAnimFile, string, bool>(animFile, animName, centered);
		if (Def.knownUISprites.ContainsKey(tuple))
		{
			return Def.knownUISprites[tuple];
		}
		if (animFile == null)
		{
			DebugUtil.LogWarningArgs(new object[] { animName, "missing Anim File" });
			return null;
		}
		KAnimFileData data = animFile.GetData();
		if (data == null)
		{
			DebugUtil.LogWarningArgs(new object[] { animName, "KAnimFileData is null" });
			return null;
		}
		if (data.build == null)
		{
			return null;
		}
		KAnim.Anim.Frame frame = KAnim.Anim.Frame.InvalidFrame;
		for (int i = 0; i < data.animCount; i++)
		{
			KAnim.Anim anim = data.GetAnim(i);
			if (anim.name == animName)
			{
				frame = anim.GetFrame(data.batchTag, 0);
			}
		}
		if (!frame.IsValid())
		{
			DebugUtil.LogWarningArgs(new object[] { string.Format("missing '{0}' anim in '{1}'", animName, animFile) });
			return null;
		}
		if (data.elementCount == 0)
		{
			return null;
		}
		KAnim.Anim.FrameElement frameElement = default(KAnim.Anim.FrameElement);
		if (string.IsNullOrEmpty(symbolName))
		{
			symbolName = animName;
		}
		frameElement = data.FindAnimFrameElement(symbolName);
		KAnim.Build.Symbol symbol = data.build.GetSymbol(frameElement.symbol);
		if (symbol == null)
		{
			DebugUtil.LogWarningArgs(new object[] { animFile.name, animName, "placeSymbol [", frameElement.symbol, "] is missing" });
			return null;
		}
		int frame2 = frameElement.frame;
		KAnim.Build.SymbolFrame symbolFrame = symbol.GetFrame(frame2).symbolFrame;
		if (symbolFrame == null)
		{
			DebugUtil.LogWarningArgs(new object[] { animName, "SymbolFrame [", frameElement.frame, "] is missing" });
			return null;
		}
		Texture2D texture = data.build.GetTexture(0);
		float x = symbolFrame.uvMin.x;
		float x2 = symbolFrame.uvMax.x;
		float y = symbolFrame.uvMax.y;
		float y2 = symbolFrame.uvMin.y;
		int num = (int)((float)texture.width * Mathf.Abs(x2 - x));
		int num2 = (int)((float)texture.height * Mathf.Abs(y2 - y));
		float num3 = Mathf.Abs(symbolFrame.bboxMax.x - symbolFrame.bboxMin.x);
		Rect rect = default(Rect);
		rect.width = (float)num;
		rect.height = (float)num2;
		rect.x = (float)((int)((float)texture.width * x));
		rect.y = (float)((int)((float)texture.height * y));
		float num4 = 100f;
		if (num != 0)
		{
			num4 = 100f / (num3 / (float)num);
		}
		Sprite sprite = Sprite.Create(texture, rect, (!centered) ? Vector2.zero : new Vector2(0.5f, 0.5f), num4, 0U, SpriteMeshType.FullRect);
		sprite.name = string.Format("{0}:{1}:{2}:{3}", new object[]
		{
			texture.name,
			animName,
			frameElement.frame.ToString(),
			centered
		});
		Def.knownUISprites[tuple] = sprite;
		return sprite;
	}

	public string PrefabID;

	public Tag Tag;

	private static Dictionary<Tuple<KAnimFile, string, bool>, Sprite> knownUISprites = new Dictionary<Tuple<KAnimFile, string, bool>, Sprite>();
}
