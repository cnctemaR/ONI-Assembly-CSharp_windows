using System;
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

	public static Tuple<Sprite, Color> GetUISprite(object item, string animName = "ui")
	{
		if (item is Substance)
		{
			return Def.GetUISprite(ElementLoader.FindElementByHash((item as Substance).elementID), "ui");
		}
		if (item is Element)
		{
			if ((item as Element).IsSolid)
			{
				return new Tuple<Sprite, Color>(Def.GetUISpriteFromMultiObjectAnim((item as Element).substance.anim, animName), Color.white);
			}
			if ((item as Element).IsLiquid)
			{
				return new Tuple<Sprite, Color>(Assets.GetSprite("element_liquid"), (item as Element).substance.debugColour);
			}
			if ((item as Element).IsGas)
			{
				return new Tuple<Sprite, Color>(Assets.GetSprite("element_gas"), (item as Element).substance.debugColour);
			}
			return new Tuple<Sprite, Color>(null, Color.clear);
		}
		else if (item is GameObject)
		{
			if (ElementLoader.GetElement((item as GameObject).PrefabID()) != null)
			{
				return Def.GetUISprite(ElementLoader.GetElement((item as GameObject).PrefabID()), "ui");
			}
			CreatureBrain component = (item as GameObject).GetComponent<CreatureBrain>();
			if (component != null)
			{
				animName = component.symbolPrefix + "ui";
			}
			return new Tuple<Sprite, Color>(Def.GetUISpriteFromMultiObjectAnim((item as GameObject).GetComponent<KBatchedAnimController>().AnimFiles[0], animName), Color.white);
		}
		else
		{
			if (item is string)
			{
				return Def.GetUISprite((item as string).ToTag(), "ui");
			}
			if (item is Tag)
			{
				if (ElementLoader.GetElement((Tag)item) != null)
				{
					return Def.GetUISprite(ElementLoader.GetElement((Tag)item), "ui");
				}
				if (Assets.GetPrefab((Tag)item) != null)
				{
					return Def.GetUISprite(Assets.GetPrefab((Tag)item), "ui");
				}
			}
			global::Debug.LogErrorFormat("Can't get sprite for type {0}", new object[] { item.ToString() });
			return null;
		}
	}

	public static Sprite GetUISpriteFromMultiObjectAnim(KAnimFile AnimFile, string animName = "ui")
	{
		if (AnimFile == null)
		{
			Output.LogWarning(new object[] { animName, "missing Anim File" });
			return null;
		}
		if (AnimFile == null)
		{
			return null;
		}
		KAnimFileData data = AnimFile.GetData();
		if (data == null)
		{
			Output.LogWarning(new object[] { animName, "KAnimFileData is null" });
			return null;
		}
		KAnim.Build build = data.build;
		if (build == null)
		{
			return null;
		}
		KAnim.Anim.Frame frame = KAnim.Anim.Frame.InvalidFrame;
		if (data != null)
		{
			for (int i = 0; i < data.animCount; i++)
			{
				KAnim.Anim anim = data.GetAnim(i);
				if (anim.name == animName)
				{
					frame = anim.GetFrame(AnimFile.GetData().batchTag, 0);
				}
			}
		}
		if (!frame.IsValid())
		{
			Output.LogWarning(new object[] { string.Format("missing '{0}' anim in '{1}'", animName, AnimFile) });
			return null;
		}
		if (data.elementCount == 0)
		{
			return null;
		}
		KAnim.Anim.FrameElement frameElement = data.GetAnimFrameElement(data.elementCount - 1);
		KAnimHashedString kanimHashedString = new KAnimHashedString(animName);
		for (int j = 0; j < data.elementCount; j++)
		{
			frameElement = data.GetAnimFrameElement(j);
			if (frameElement.symbol == kanimHashedString)
			{
				break;
			}
		}
		KAnim.Build.Symbol symbol = build.GetSymbol(frameElement.symbol);
		if (symbol == null)
		{
			Output.LogWarning(new object[] { animName, "placeSymbol [", frameElement.symbol, "] is missing" });
			return null;
		}
		KAnim.Build.SymbolFrame symbolFrame = symbol.GetFrame(frameElement.frame).symbolFrame;
		if (symbolFrame == null)
		{
			Output.LogWarning(new object[] { animName, "SymbolFrame [", frameElement.frame, "] is missing" });
			return null;
		}
		Texture2D texture = build.GetTexture(0);
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
		Sprite sprite = Sprite.Create(texture, rect, new Vector2(0f, 0f), num4, 0U, SpriteMeshType.FullRect);
		sprite.name = ":" + frameElement.frame.ToString();
		return sprite;
	}

	public string PrefabID;

	public Tag Tag;
}
