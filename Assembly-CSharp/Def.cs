using System;
using UnityEngine;

[Serializable]
public class Def : ScriptableObject
{
	public virtual void InitDef()
	{
		this.Tag = TagManager.Create(this.PrefabID, null);
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
		if (AnimFile != null && AnimFile.GetData() != null && AnimFile.GetData().anims != null)
		{
			for (int i = 0; i < AnimFile.GetData().anims.Length; i++)
			{
				if (AnimFile.GetData().anims[i].name == animName)
				{
					frame = AnimFile.GetData().anims[i].GetFrame(AnimFile.GetData().batchTag, 0);
				}
			}
		}
		if (!frame.IsValid())
		{
			Output.LogWarning(new object[] { "missing '" + animName + "' anim" });
			return null;
		}
		if (data.animFrameElements == null || data.animFrameElements.Length == 0)
		{
			return null;
		}
		int num = data.animFrameElements.Length;
		KAnim.Anim.FrameElement frameElement = data.animFrameElements[num - 1];
		KAnimHashedString kanimHashedString = new KAnimHashedString(animName);
		for (int j = 0; j < num; j++)
		{
			frameElement = data.animFrameElements[j];
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
		Texture2D texture2D = build.textures[0];
		float x = symbolFrame.uv0.x;
		float x2 = symbolFrame.uv1.x;
		float y = symbolFrame.uv2.y;
		float y2 = symbolFrame.uv0.y;
		int num2 = (int)((float)texture2D.width * Mathf.Abs(x2 - x));
		int num3 = (int)((float)texture2D.height * Mathf.Abs(y2 - y));
		float num4 = Mathf.Abs(symbolFrame.bboxMax.x - symbolFrame.bboxMin.x);
		Rect rect = default(Rect);
		rect.width = (float)num2;
		rect.height = (float)num3;
		rect.x = (float)((int)((float)texture2D.width * x));
		rect.y = (float)((int)((float)texture2D.height * y));
		float num5 = 100f;
		if (num2 != 0)
		{
			num5 = 100f / (num4 / (float)num2);
		}
		Sprite sprite = Sprite.Create(texture2D, rect, new Vector2(0f, 0f), num5, 0U, SpriteMeshType.FullRect);
		sprite.name = ":" + frameElement.frame.ToString();
		return sprite;
	}

	public string PrefabID;

	public Tag Tag;
}
