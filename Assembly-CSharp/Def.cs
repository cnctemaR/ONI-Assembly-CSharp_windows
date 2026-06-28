using System;
using UnityEngine;

[Serializable]
public class Def : ScriptableObject
{
	public virtual void InitDef()
	{
		this.Tag = TagManager.Create(this.PrefabID, base.name);
	}

	public virtual string Name
	{
		get
		{
			return null;
		}
	}

	public static Sprite GetUISpriteFromMultiObjectAnim(KAnimFile AnimFile, string animName = "ui")
	{
		Sprite sprite;
		if (AnimFile == null)
		{
			Output.LogWarning(new object[] { animName, "missing Anim File" });
			sprite = null;
		}
		else if (AnimFile == null)
		{
			sprite = null;
		}
		else
		{
			KAnimFileData data = AnimFile.GetData();
			if (data == null)
			{
				Output.LogWarning(new object[] { animName, "KAnimFileData is null" });
				sprite = null;
			}
			else
			{
				KAnim.Build build = data.build;
				if (build == null)
				{
					sprite = null;
				}
				else
				{
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
						Output.LogWarning(new object[] { "missing '" + animName + "' anim" });
						sprite = null;
					}
					else if (data.elementCount == 0)
					{
						sprite = null;
					}
					else
					{
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
							sprite = null;
						}
						else
						{
							KAnim.Build.SymbolFrame symbolFrame = symbol.GetFrame(frameElement.frame).symbolFrame;
							if (symbolFrame == null)
							{
								Output.LogWarning(new object[] { animName, "SymbolFrame [", frameElement.frame, "] is missing" });
								sprite = null;
							}
							else
							{
								Texture2D texture = build.GetTexture(0);
								float x = symbolFrame.uv0.x;
								float x2 = symbolFrame.uv1.x;
								float y = symbolFrame.uv2.y;
								float y2 = symbolFrame.uv0.y;
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
								Sprite sprite2 = Sprite.Create(texture, rect, new Vector2(0f, 0f), num4, 0U, SpriteMeshType.FullRect);
								sprite2.name = ":" + frameElement.frame.ToString();
								sprite = sprite2;
							}
						}
					}
				}
			}
		}
		return sprite;
	}

	public string PrefabID;

	public Tag Tag;
}
